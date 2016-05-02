using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using FAB.Forms;
using Acr.UserDialogs;

namespace Capp2
{
	public static class UIBuilder
	{
		public static string GetPlatformFABIcon(){
			if(Device.OS == TargetPlatform.Android)  return "ic_add_white_24dp.png";
			else if(Device.OS == TargetPlatform.iOS) return "Add";
			return string.Empty;
		}
		public static StackLayout AddFloatingActionButtonToStackLayout(StackLayout stack, string icon, Command FabTapped, Color NormalColor, Color PressedColor){
			var layout = new RelativeLayout ();
			layout.Children.Add(
				stack,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
				heightConstraint: Constraint.RelativeToParent(parent => parent.Height)
			);

			return new StackLayout{
				BackgroundColor = Color.Transparent,
				Children = {
					AddFloatingActionButtonToRelativeLayout(layout, icon, FabTapped, NormalColor, PressedColor)
				}
			};
		}

		public static RelativeLayout AddFloatingActionButtonToRelativeLayout(RelativeLayout layout, string icon, Command FabTapped, Color NormalColor, Color PressedColor){
            var normalFab = new FAB.Forms.FloatingActionButton();
			normalFab.Clicked += (sender, e) => {
				UIAnimationHelper.ZoomUnZoomElement (normalFab);

			};

            normalFab.Source = icon;
			normalFab.Size = FabSize.Normal;
			normalFab.HasShadow = true;
			normalFab.NormalColor = NormalColor;
			normalFab.Opacity = 0.9;
			normalFab.PressedColor = PressedColor;

			layout.Children.Add(
				normalFab,
				xConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Width - normalFab.Width) - 45; }),
				yConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Height - normalFab.Height) - 45; })
			);
			normalFab.SizeChanged += (sender, args) => { layout.ForceLayout(); };
			normalFab.SetBinding (FloatingActionButton.CommandProperty, new Binding(){Source = FabTapped});

			return layout;
		}

		public static View AddElementToObjectDependingOniOSAndAndroidListViewShortNameBinding(
			View parent, View child)
		{
			var ListViewGroupShortNameBindingFiller = new Label{ 
				WidthRequest = 20
			};

			if (Device.OS == TargetPlatform.iOS) {
				return new StackLayout {
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.StartAndExpand,
					Padding = new Thickness (15, 5, 5, 15),
					Children = { 
						parent,
						child,
						ListViewGroupShortNameBindingFiller
					}
				};
			} else if (Device.OS == TargetPlatform.Android) {
				return new StackLayout {
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.StartAndExpand,
					Padding = new Thickness (15, 5, 5, 15),
					Children = { parent, child }
				};
			}
			throw new NotImplementedException("UIBuilder.AddElementToObjectWithXYConstraintDependingOniOSAndAndroidLayouts() is not implmented in platforms other than iOS and Android");
		}

		public static RelativeLayout AddElementToObjectWithXYConstraint(
			View parentElement, View ElementToAdd, double xOnParent, double yOnParent)
		{ 
			RelativeLayout layout = new RelativeLayout ();
			layout.Children.Add(
				parentElement,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
				heightConstraint: Constraint.RelativeToParent(parent => parent.Height)
			);
			layout.Children.Add (
				ElementToAdd,
				xConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Width - ElementToAdd.Width) - xOnParent; }),
				yConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Height - ElementToAdd.Height) - yOnParent; })
			);
			Debug.WriteLine ("Parent row width: {0}, Parent row height: {1}, child row width {2}, child row height {3}", 
				parentElement.Width, parentElement.Height, ElementToAdd.Width, ElementToAdd.Height);
			return layout;
		}
		public static StackLayout ComposeInfoPageStack(){
			return new StackLayout{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(20),
				Children = {
					ComposeParagraph("", ""),

				}
			};
		}
		public static StackLayout ComposeParagraph(string headerStr, string bodyStr){
			Label headerLabel = new Label{ 
				Text = headerStr,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center,
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label))
			};
			Label bodyLabel = new Label{ 
				Text = bodyStr,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center,
				FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label))
			};

			return new StackLayout{ 
				Orientation = StackOrientation.Vertical,
				//Padding = new Thickness(20),
				Children = {
					headerLabel,
					//spacing?
					bodyLabel,
				}
			};
		}

		public static StackLayout CreateModalXPopper(Command CloseCommand, string text = "", string icon = "clear-Small.png"){
			Image DoneImage = new Image ();
			DoneImage = UIBuilder.CreateTappableImage ("clear-Small.png", LayoutOptions.Start, Aspect.AspectFit, new Command(() => {
				UIAnimationHelper.ShrinkUnshrinkElement(DoneImage);
				CloseCommand.Execute(null);
			}));
			Label MainLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				Text = text,
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center
			};

			if(string.IsNullOrWhiteSpace(text)){
				return new StackLayout{ 
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					Padding = new Thickness(0,0,0, 7),
					Children = {
						DoneImage
					}
				};
			}else{
				return new StackLayout{ 
					Orientation = StackOrientation.Horizontal,
					Padding = new Thickness(0,0,0, 7),
					Children = {
						DoneImage, MainLabel
					}
				};
			}
		}

		public static Image CreateTappableImage(string icon, LayoutOptions layout, Aspect aspect, Command handlerCommand){
			TapGestureRecognizer handler = new TapGestureRecognizer{Command = handlerCommand};
			var img = new Image{
				Source = icon,
				HorizontalOptions = layout,
				Aspect = aspect,
			};
			img.GestureRecognizers.Add (handler);
			return img;
		}
		public static StackLayout CreateEmptyStackSpace(){
			var EmptyLabel = new Label{ 
				Text = "  "
			};
			return new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Children = { EmptyLabel }
			};
		}
		public static StackLayout CreateSettingsHeader(string header){
			var MainLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				Text = header,
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center
			};
			return new StackLayout {
				Orientation = StackOrientation.Vertical,
				Children = { 
					UIBuilder.CreateEmptyStackSpace (),
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						Children = { MainLabel }
					},
					UIBuilder.CreateEmptyStackSpace (),
					UIBuilder.CreateSeparator (Color.Gray, 0.3),
				}
			};
		}
		public static StackLayout CreateSetting(string icon, string name, TapGestureRecognizer handler, bool center = false){
			LayoutOptions layout;
			if (center) {
				layout = LayoutOptions.CenterAndExpand;
			}else{
				layout = LayoutOptions.StartAndExpand;
			}

			StackLayout Setting = new StackLayout { 
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = layout,
				Padding = new Thickness(20),
				Children = {
					new Image{
						Source = icon,
						HorizontalOptions = LayoutOptions.Start
					},
					new Label{
						Text = name,
						LineBreakMode = LineBreakMode.WordWrap,
						HorizontalTextAlignment = TextAlignment.Center
					},

				}
			};
			Setting.GestureRecognizers.Add (handler);
			return Setting;
		}
		public static BoxView CreateSeparator(Color borderColor, double opacity){
			return new BoxView () { Color = Color.Gray, HeightRequest = 1, Opacity = opacity  };
		}
		public static StackLayout CreateSeparatorWithBottomSpacing(Color borderColor, double opacity, double bottomspacing = 5){
			return new StackLayout{ 
				Orientation = StackOrientation.Vertical,
				Children = {
					new BoxView() { Color = Color.Gray, HeightRequest = 1, Opacity = 0.5},
					new BoxView() { Color = Color.White, HeightRequest = bottomspacing  },
				}
			};
		}
		public static StackLayout CreateSeparatorWithTopSpacing(Color borderColor, double opacity, double topspacing = 5){
			return new StackLayout{ 
				Orientation = StackOrientation.Vertical,
				Children = {
					new BoxView() { Color = Color.White, HeightRequest = topspacing  },
					new BoxView() { Color = Color.Gray, HeightRequest = 1, Opacity = 0.5},
				}
			};
		}
		public static StackLayout CreateSeparatorWithTopBottomSpacing(Color borderColor, double opacity, double topbottomspacing = 5){
			return new StackLayout{ 
				Orientation = StackOrientation.Vertical,
				Children = {
					new BoxView() { Color = Color.White, HeightRequest = topbottomspacing  },
					new BoxView() { Color = Color.Gray, HeightRequest = 1, Opacity = 0.5},
					new BoxView() { Color = Color.White, HeightRequest = topbottomspacing  },
				}
			};
		}
		public static BoxView CreateWhiteSpacing(double spacing = 5){
			return new BoxView () { Color = Color.White, HeightRequest = spacing  };
		}
	}
}

