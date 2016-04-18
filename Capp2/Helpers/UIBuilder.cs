using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using FAB.Forms;

namespace Capp2
{
	public static class UIBuilder
	{
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
				Children = {
					AddFloatingActionButtonToRelativeLayout(layout, icon, FabTapped, NormalColor, PressedColor)
				}
			};
		}
		public static RelativeLayout AddFloatingActionButtonToRelativeLayout(RelativeLayout layout, string icon, Command FabTapped, Color NormalColor, Color PressedColor){
            var normalFab = new FAB.Forms.FloatingActionButton();

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
		public static Image CreateTappableImage(string source, Command TapAction = null){
			Image img = new Image{
				Aspect = Aspect.AspectFit,
				Source = source
			};
			if (TapAction != null) {
				img.GestureRecognizers.Add (new TapGestureRecognizer{ Command = TapAction});
			} else {
				//img.GestureRecognizers.Add (new TapGestureRecognizer{ Command = new Command (() => Util.ChangeProfilePic (img)) });
			}
			return img;
		}


		public static StackLayout CreateSetting(string icon, string name, TapGestureRecognizer handler){
			
			StackLayout Setting = new StackLayout { 
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Start,
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

