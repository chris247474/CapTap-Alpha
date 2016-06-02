using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using FAB.Forms;
using Acr.UserDialogs;
using XLabs.Forms.Controls;
using Capp2.Helpers;
using System.Collections.Generic;

namespace Capp2
{
	public static class UIBuilder
	{
		public static string ChooseRandomProfilePicBackground(string[] images){
			if (images == null || images.Length == 0) {
				throw new ArgumentException ("param is null or empty");
			}

			Random r = new Random ();
			int result = 0;

			for (int c = 0; c < images.Length; c++) {
				result = r.Next (0, images.Length);
			}

			return images[result];
		}

		public static string GetGradientBackground(string hex){
			/*if (string.Equals(hex, Values.MaterialDesignOrange)) {
			}else if (string.Equals(hex, Values.CAPPTUTORIALCOLOR_Purple)) {
				return "PurpleGradientBackground.png";
			}else if (string.Equals(hex, Values.GOOGLEBLUE)) {
				return "BlueGradientBackground.png";
			}else if (string.Equals(hex, Values.CAPPTUTORIALCOLOR_Green)) {
				return "GreenGradientBackground.png";
			}*/

			return "OrangeGradientBackground.png";
		}

		public static string ExtractInitials(ContactData contact){
			if(contact == null) throw new ArgumentException("ExtractInitials contact argument is null");

			var firstname = contact.FirstName.ToCharArray ();
			var lastname = contact.LastName.ToCharArray ();
			string initials = "";

			if (!char.IsWhiteSpace (firstname[0])) {
				initials += firstname [0];
			}
			if (!char.IsWhiteSpace (lastname[0])) {
				initials += lastname [0];
			}

			return initials;
		}

		public static string ChooseContactPicColor(){
			return "";
		}

		public static void PlaceInitialsTextOnImage(RelativeLayout layout, double NameFont, CircleImage circleImage, 
			ContactData contact)
		{
			var letters = ExtractInitials (contact);

			if (letters.Length > 2) {
				throw new ArgumentException ("DrawLetterTextOnImage allows a max of 2 letters to be drawn on an image");
			}

			var label = new Label{
				Text = letters,
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label))*4,
				BackgroundColor = Color.Transparent,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};
			label.Opacity = label.Opacity / 2;

			layout.Children.Add(//change to relativetoview
				label,
				xConstraint: Constraint.RelativeToView (circleImage, (parent, view) => {
					return view.X+(label.Width*0.2);
				}),
				//Constraint.RelativeToParent(parent => circleImage.Width*0.59),
				yConstraint: Constraint.RelativeToView (circleImage, (parent, view) => {
					return view.Y*1.255;
				}),
				//Constraint.RelativeToParent(parent => circleImage.Height*0.37)
				widthConstraint: Constraint.RelativeToView (circleImage, (parent, view) => {
					return view.Width;
				}),
				heightConstraint: Constraint.RelativeToView (circleImage, (parent, view) => {
					return view.Height*0.66;
				})
			); 
		}

		public static async Task ShowConfirmAsync(Command cmd){
			var YesNo = new ConfirmConfig () { 
				Title = "AutoCall", 
				OkText = "AutoCall",
				Message = "Start calling?",
				CancelText = "Nvm",
			};
			YesNo.OnConfirm = new Action<bool>(delegate(bool obj) {
				cmd.Execute(null);
			});

			await UserDialogs.Instance.ConfirmAsync (YesNo);
		}

		/*public static CarouselView CreateCarouselView(List<VideoChooserItem> videos){
			CarouselView carousel = new CarouselView{
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				ItemsSource = videos, 
				ItemTemplate = new DataTemplate(() => {
					return new CarouselTemplateView();
				}),
			};
			return carousel;
		}*/

		public static StackLayout CreateDataTemplateLabelEmptySpace(){
			return new StackLayout {
				HorizontalOptions = LayoutOptions.EndAndExpand,
				Children = { new Label{ WidthRequest = 20 } }
			};
		}

		public static ScrollView CreateTutorialVideoPickerView(VideoChooserItem[] videos){
			if (videos == null || videos.Length < 1) {
				throw new ArgumentNullException ("param must not be null and array must not be empty");
			}

			var stack = new StackLayout { 
				Orientation = StackOrientation.Vertical,
				//Padding = new Thickness(10, 0),
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			for (int c = 0;c < videos.Length; c++) {
				stack.Children.Add (CreateTappableImageWithBottomLabel(videos[c].ImagePath, videos[c].LabelText,
					videos[c].VideoPath));
			}
					
			return new ScrollView{
				Orientation = ScrollOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Content = stack
			};
		}
		public static Grid CreateDynamicGrid(int rows, int cols){
			Grid grid = new Grid ();
			grid.RowDefinitions = new RowDefinitionCollection();
			grid.ColumnDefinitions = new ColumnDefinitionCollection();

			for (int c = 0; c < rows; c++) 
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });  
			}
			for (int x = 0; x < cols; x++) 
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			}
			return grid;
		}
		public static StackLayout CreateTappableImageWithBottomLabel(string imagePath, string labelText, string videotoplay){
			Label lbl = new Label{
				Text = labelText,
				FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
			};
			Image img = CreateTappableImage (imagePath, LayoutOptions.Center, Aspect.AspectFit, new Command(()=>{
				DependencyService.Get<IVideoHelper>().PlayVideo(videotoplay);
			}), lbl.FontSize, 60, 30);

			var stack = new StackLayout{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(10),
				Children = {
					img,
					/*AddElementToObjectWithXYConstraint (img, PlayIcon, 
						Constraint.RelativeToParent((parent) =>  { return parent.Width/2; }), 
						Constraint.RelativeToParent((parent) =>  { return parent.Height/2; })
					),*/
					lbl,
				}
			};
			return stack;
		}

		public static Label CreateTutorialLabel(string text, NamedSize fontSize = NamedSize.Medium, FontAttributes fontattributes = FontAttributes.Bold, 
			LineBreakMode linebreakmode = LineBreakMode.WordWrap, Command tapCommand = null)
		{
			Label lbl = new Label{
				Text = text,
				TextColor = Color.White, 
				FontSize = Device.GetNamedSize (fontSize, typeof(Label)),
				FontAttributes = fontattributes, 
				LineBreakMode = linebreakmode, 
			};
			lbl.GestureRecognizers.Add (new TapGestureRecognizer{Command = tapCommand});
			return lbl;
		}

		public static Label CreateLabel(NamedSize fontSize){
			return new Label{ 
				FontSize = Device.GetNamedSize (fontSize, typeof(Label)),
			};
		}

		public static string GetPlatformFABIcon(){
			if(Device.OS == TargetPlatform.Android)  return "ic_add_white_24dp";
			else if(Device.OS == TargetPlatform.iOS) return "Plus-100";
			return string.Empty;
		}

		public static FloatingActionButton CreateFAB(string icon, FabSize size, Color NormalColor, Color PressedColor)
		{
			FloatingActionButton normalFab = new FloatingActionButton();
			normalFab.Clicked += (sender, e) => {
				UIAnimationHelper.ZoomUnZoomElement (normalFab);
			};
			normalFab.Size = size;
			normalFab.Source = icon; 
			normalFab.HasShadow = true;
			normalFab.NormalColor = NormalColor;
			normalFab.Opacity = 0.9;
			normalFab.PressedColor = PressedColor;

			return normalFab;
		}

		public static void AddElementRelativeToViewonRelativeLayoutParent(RelativeLayout parentlayout, 
			View child, 
			Constraint xConst,
			Constraint yConst)
		{
			/*parentlayout.Children.Add (
				child, Constraint.RelativeToView (referenceChildView, (Parent, sibling) => {
					return sibling.X + 20;
				}), Constraint.RelativeToView (child, (parent, sibling) => {
					return sibling.Y + 10;
				}), Constraint.RelativeToParent((parent) => {
					return parent.Width * .5;
				}), Constraint.RelativeToParent((parent) => {
					return parent.Height * .5;
			}));*/

			parentlayout.Children.Add(  
				child,
				xConstraint: xConst, 
				yConstraint: yConst 
			);
		} 

		public static RelativeLayout AddFABToViewWrapRelativeLayout(View viewToAddTo, FloatingActionButton child, string icon,
			Command FabTapped = null)
		{
			RelativeLayout parentlayout = new RelativeLayout();

			parentlayout.VerticalOptions = LayoutOptions.FillAndExpand;
			parentlayout.HorizontalOptions = LayoutOptions.FillAndExpand;
			parentlayout.Children.Add(
				viewToAddTo,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
				heightConstraint: Constraint.RelativeToParent(parent => parent.Height)
			);

			CircleImage img = new CircleImage ();
			img = CreateTappableCircleImage (icon, 
				LayoutOptions.Fill, Aspect.AspectFit, new Command(()=>{
					UIAnimationHelper.ShrinkUnshrinkElement (child, 100);
					UIAnimationHelper.ShrinkUnshrinkElement (img, 100);
					FabTapped.Execute(null);
				}));
			img.InputTransparent = true;

			if (Device.OS == TargetPlatform.iOS) {
				parentlayout.Children.Add(  
					child,
					xConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Width - child.Width) - 30; }), 
					yConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Height - child.Height) - 45; }) 
				);
				parentlayout.Children.Add (
					img,
					Constraint.RelativeToView (child, (parent, view) => {
						return view.X + (view.Width*0.31);
					}),
					Constraint.RelativeToView (child, (parent, view) => {
						return view.Y + (view.Height*0.31);
					})
				);
				/*if (Device.OS == TargetPlatform.iOS) {
					parentlayout.Children.Add (
						img,
						Constraint.RelativeToView (child, (parent, view) => {
							return view.X + (view.Width*0.25);//0.31
						}),
						Constraint.RelativeToView (child, (parent, view) => {
							return view.Y + (view.Height*0.25);//0.31
						}),
						Constraint.RelativeToView (child, (parent, view) => {
							return view.Width*0.5;
						}),
						Constraint.RelativeToView (child, (parent, view) => {
							return view.Height*0.5;
						})
					);
				}*/
			} else {
				parentlayout.Children.Add(  
					child,
					xConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Width - child.Width) - 45; }), 
					yConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Height - child.Height) - 45; }) 
				);
			}
			child.SizeChanged += (sender, args) => { parentlayout.ForceLayout(); }; 
			child.SetBinding (FloatingActionButton.CommandProperty, new Binding (){ Source = FabTapped });
			
			return parentlayout;
		}

		/*public static StackLayout AddFloatingActionButtonToStackLayout(View view, string icon, Command FabTapped, 
			Color NormalColor, Color PressedColor)
		{
			var layout = new RelativeLayout ();
			layout.VerticalOptions = LayoutOptions.FillAndExpand;
			layout.HorizontalOptions = LayoutOptions.FillAndExpand;
			layout.Children.Add(
				view,
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
		}*/


		public static RelativeLayout AddFloatingActionButtonToViewWrapWithRelativeLayout(View view, string icon, Command FabTapped, Color NormalColor, Color PressedColor){
			var layout = new RelativeLayout ();
			layout.VerticalOptions = LayoutOptions.FillAndExpand;
			layout.HorizontalOptions = LayoutOptions.FillAndExpand;
			layout.Children.Add(
				view,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
				heightConstraint: Constraint.RelativeToParent(parent => parent.Height)
			);

			return AddFloatingActionButtonToRelativeLayout(layout, icon, new Command(() => {
				if(!App.UsingSearch){
					Debug.WriteLine("SearchBar not being used");
					FabTapped.Execute(new object());
				}else{
					Debug.WriteLine("SearchBar being used, FAB action cancelled");
				}
			}), NormalColor, PressedColor);
		}

		static string SwapIconsOnPressIfNeeded(FloatingActionButton fab, Image img, string normalIcon, string PressedIcon, string tempicon){
			Device.OnPlatform (
				() => {
					if(string.Equals(tempicon, normalIcon)){
						img.Source = PressedIcon;
						tempicon = PressedIcon;
					}else if(string.Equals(tempicon, PressedIcon)){
						img.Source = normalIcon;
						tempicon = normalIcon;
					}
				},
				()=>{
					if(string.Equals(tempicon, normalIcon)){
						fab.Source = PressedIcon;
						tempicon = PressedIcon;
					}else if(string.Equals(tempicon, PressedIcon)){
						fab.Source = normalIcon;
						tempicon = normalIcon;
					}
				}
			);

			return tempicon;
		}

		public static RelativeLayout AddFloatingActionButtonToRelativeLayout(RelativeLayout layout, string icon, 
			Command FabTapped, Color NormalColor, Color PressedColor, string PressedIcon = "")
		{
			string tempicon = icon;

            var normalFab = new FAB.Forms.FloatingActionButton();

			Image img = new Image();
			img = CreateTappableImage (icon, 
				LayoutOptions.Fill, Aspect.AspectFit, new Command(()=>{
					Debug.WriteLine("icon tapped");
					UIAnimationHelper.ShrinkUnshrinkElement (normalFab, 100);
					UIAnimationHelper.ShrinkUnshrinkElement (img, 100);

					if(!string.IsNullOrWhiteSpace(PressedIcon)){
						tempicon = SwapIconsOnPressIfNeeded(normalFab, img, icon, PressedIcon, tempicon);
					}

					FabTapped.Execute(null);
				}));
			img.InputTransparent = true;

			normalFab.Clicked += (sender, e) => {
				Debug.WriteLine("Clicked");
				UIAnimationHelper.ZoomUnZoomElement (normalFab);

				if(!string.IsNullOrWhiteSpace(PressedIcon)){
					tempicon = SwapIconsOnPressIfNeeded(normalFab, img, icon, PressedIcon, tempicon);
				}

				Debug.WriteLine("FabTapped");
			};

			//normalFab.Source = FileImageSource.FromFile("Plus.png");
			normalFab.Size = FabSize.Normal;
			normalFab.HasShadow = true;
			normalFab.NormalColor = NormalColor;
			normalFab.Opacity = 0.9;
			normalFab.PressedColor = PressedColor;

			if (Device.OS == TargetPlatform.iOS) {
				layout.Children.Add(
					normalFab,
					xConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Width - normalFab.Width) - 30; }),
					yConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Height - normalFab.Height) - 45; })
				);
			} else {
				layout.Children.Add(
					normalFab,
					xConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Width - normalFab.Width) - 45; }),
					yConstraint: Constraint.RelativeToParent((parent) =>  { return (parent.Height - normalFab.Height) - 45; })
				);
			}
			normalFab.SizeChanged += (sender, args) => { layout.ForceLayout(); };
			normalFab.SetBinding (FloatingActionButton.CommandProperty, new Binding(){Source = FabTapped});

			if (Device.OS == TargetPlatform.iOS) {
				layout.Children.Add (
					img,
					Constraint.RelativeToView (normalFab, (parent, view) => {
						return view.X + (view.Width*0.31);//0.25
					}),
					Constraint.RelativeToView (normalFab, (parent, view) => {
						return view.Y + (view.Height*0.31);//0.25
					})//,
					/*Constraint.RelativeToView (normalFab, (parent, view) => {
						return view.Width*0.5;
					}),
					Constraint.RelativeToView (normalFab, (parent, view) => {
						return view.Height*0.5;
					})*/
				);
			}

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
					HorizontalOptions = LayoutOptions.FillAndExpand,
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
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Padding = new Thickness (15, 5, 5, 15),
					Children = { parent, child }
				};
			}
			throw new NotImplementedException("UIBuilder.AddElementToObjectWithXYConstraintDependingOniOSAndAndroidLayouts() is not implmented in platforms other than iOS and Android");
		}

		public static RelativeLayout AddElementToObjectWithXYConstraint(
			View parentElement, View ElementToAdd, Constraint xOnParent, Constraint yOnParent)
		{ 
			RelativeLayout layout = new RelativeLayout ();
			layout.HorizontalOptions = LayoutOptions.FillAndExpand;
			layout.VerticalOptions = LayoutOptions.FillAndExpand;
			layout.Children.Add(
				parentElement,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
				heightConstraint: Constraint.RelativeToParent(parent => parent.Height)
			);
			layout.Children.Add (
				ElementToAdd,
				xConstraint: xOnParent/*Constraint.RelativeToParent((parent) =>  { return (parent.Width - ElementToAdd.Width) - xOnParent; })*/,
				yConstraint: yOnParent/*Constraint.RelativeToParent((parent) =>  { return (parent.Height - ElementToAdd.Height) - yOnParent; })*/
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

		public static StackLayout CreateTextTemplateSetting(Binding binding, SettingsViewModel settings, string title, string icon){
			Editor TemplateEntry = new Editor ();
			StackLayout TemplateStack = new StackLayout ();
			//TemplateStack.BindingContext = settings;
			//TemplateEntry.BindingContext = settings;
			bool EntryShown = false;

			TemplateEntry.SetBinding <SettingsViewModel>(Editor.TextProperty, pref => pref.DailyEmailTemplateSettings);
			TemplateEntry.HorizontalOptions = LayoutOptions.Center;
			//TemplateEntry.Text = settings.DailyEmailTemplateSettings;

			TemplateStack = new StackLayout{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					UIBuilder.CreateSetting ("", title, 
						new TapGestureRecognizer {Command = new Command (() => 
							{
								UIAnimationHelper.ShrinkUnshrinkElement(TemplateStack);
								if (!EntryShown) {
									TemplateEntry.Focus ();
									TemplateStack.Children.Insert (1, TemplateEntry);
									EntryShown = true;
									//TemplateEntry.Text = settings.DailyEmailTemplateSettings;
									Debug.WriteLine (TemplateEntry.Text);
								}else{
									TemplateStack.Children.Remove (TemplateEntry);
									EntryShown = false;
									Debug.WriteLine (Settings.DailyEmailTemplateSettings);
								}
							}
						)}, true),
					UIBuilder.CreateSeparator (Color.Gray, 0.3),
				}
			};

			return TemplateStack;
		}


		public static StackLayout CreateModalXPopper(Command CloseCommand, string text = "", 
			string icon = "Close.png", double opacity = 1)
		{
			Image DoneImage = new Image ();

			Label MainLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				Text = text,
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center,
				Opacity = opacity,
			};

			DoneImage = UIBuilder.CreateTappableImage (icon, LayoutOptions.Start, Aspect.AspectFit, new Command(() => {
				UIAnimationHelper.ShrinkUnshrinkElement(DoneImage);
				CloseCommand.Execute(null);
			}), MainLabel.FontSize);
			DoneImage.Opacity = opacity;

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

		public static Image CreateTappableImage(string icon, LayoutOptions layout, Aspect aspect, 
			Command handlerCommand = null, double fontsize = 0, double xscale = 1.5, double yscale = 1.5)
		{
			Image img = new Image ();

			TapGestureRecognizer handler = new TapGestureRecognizer{Command = new Command(() => {
				UIAnimationHelper.ShrinkUnshrinkElement(img);
				handlerCommand.Execute(null);
			})};

			if (fontsize > 0) {
				img = new Image{
					Source = icon,
					HorizontalOptions = layout,
					Aspect = aspect,
					HeightRequest = fontsize *yscale,
					WidthRequest = fontsize *xscale,
				};
			} else {
				img = new Image{
					Source = icon,
					HorizontalOptions = layout,
					Aspect = aspect,
				};
			}
			img.GestureRecognizers.Add (handler);
			return img;
		}

		public static CircleImage CreateTappableCircleImage(string icon, LayoutOptions layout, Aspect aspect, Command handlerCommand){
			var img = new CircleImage{
				Source = icon,
				HorizontalOptions = layout,
				Aspect = aspect,
			};

			TapGestureRecognizer handler = new TapGestureRecognizer{
				Command = new Command(() => {
					UIAnimationHelper.ShrinkUnshrinkElement(img);
					handlerCommand.Execute(null);
				})
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
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Color.FromHex ("#AAAAAA"),
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
			Label lbl = new Label {
				Text = name,
				LineBreakMode = LineBreakMode.WordWrap,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Color.FromHex ("#AAAAAA"),
			};

			if (center) {
				layout = LayoutOptions.CenterAndExpand;
			}else{
				layout = LayoutOptions.StartAndExpand;
			}

			StackLayout Setting = new StackLayout { 
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = layout,
				Padding = new Thickness(5),
				Children = {
					new Image{
						Source = FileImageSource.FromFile(icon),
						HorizontalOptions = LayoutOptions.Start,
						HeightRequest = lbl.FontSize*1.5,
						WidthRequest = lbl.FontSize*1.5,
					},
					lbl,
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

