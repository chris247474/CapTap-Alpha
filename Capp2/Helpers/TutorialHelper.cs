using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using FAB.Forms;

namespace Capp2
{
	public static class TutorialHelper
	{
		static Label InfoLabel, DoneLabel, AutoCallInfoLabel, InfoLabel2, InfoLabel3;
		static StackLayout stack;
		static FloatingActionButton fab;
		static bool continuePressed = false;
		static double continuePositionY;
		static double continuePositionX;
		static Image img;

		public static async Task OpenNamelist(ContentPage page, string text, Color background){
			var layout = ((RelativeLayout)page.Content);

			InfoLabel = UIBuilder.CreateTutorialLabel (text, NamedSize.Large, FontAttributes.None);

			stack = new StackLayout { 
				BackgroundColor = new Color (
					background.R, 
					background.G, 
					background.B, 
					0.9
				),
				Children = {
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						HorizontalOptions = LayoutOptions.Center,
						Padding = new Thickness(60),
						Children = {
							UIBuilder.CreateEmptyStackSpace(),
							InfoLabel,
							UIBuilder.CreateEmptyStackSpace(),
							DoneLabel
						}
					}
				}
			};

			ContentView content = new ContentView{
				Content = stack
			};

			layout.Children.Add(
				content.Content,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				Constraint.RelativeToParent((parent) => {
					return parent.Height;
				})
			);

			DoneLabel = UIBuilder.CreateTutorialLabel ("Got it", NamedSize.Large, FontAttributes.Bold, 
				LineBreakMode.WordWrap, new Command(()=>{
					layout.Children.Remove(content.Content);
				}));
		}

		public static async Task HowToAddNumbers(ContentPage page, string text, Color background){
			var layout = ((RelativeLayout)page.Content);

			InfoLabel = UIBuilder.CreateTutorialLabel (text, NamedSize.Large, FontAttributes.None);

			img = UIBuilder.CreateTappableImage ("UpFromBottomLeft.png", LayoutOptions.Center, Aspect.AspectFit, new Command (() => {
			}), InfoLabel.FontSize);

			stack = new StackLayout { 
				BackgroundColor = new Color (
					background.R, 
					background.G, 
					background.B, 
					0.9
				),
				Children = {
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						HorizontalOptions = LayoutOptions.Center,
						Padding = new Thickness(60),
						Children = {
							UIBuilder.CreateEmptyStackSpace(),
							new StackLayout{
								Orientation = StackOrientation.Horizontal,
								Children = {
									InfoLabel,
									new StackLayout{
										HorizontalOptions = LayoutOptions.End,
										Children = { img }
									}
								}
							}
						}
					}
				}
			};

			ContentView content = new ContentView{
				Content = stack
			};

			layout.Children.Add(
				content.Content,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				Constraint.RelativeToParent((parent) => {
					return parent.Height;
				})
			);

			while (true) {
				await UIAnimationHelper.ZoomUnZoomElement (img, 1.3, 1000, true);
				await Task.Delay (1000);
			}
		}

		public static async Task HowToMakeANamelist(ContentPage page, string text, Color background){
			var layout = ((RelativeLayout)page.Content);

			InfoLabel = UIBuilder.CreateTutorialLabel (text, NamedSize.Large, FontAttributes.None);

			img = UIBuilder.CreateTappableImage ("DownFromUpperLeft.png", LayoutOptions.Center, Aspect.AspectFit, new Command (() => {
			}), InfoLabel.FontSize);

			stack = new StackLayout { 
				BackgroundColor = new Color (
					background.R, 
					background.G, 
					background.B, 
					0.9
				),
				Children = {
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						HorizontalOptions = LayoutOptions.Center,
						Padding = new Thickness(60),
						Children = {
							UIBuilder.CreateEmptyStackSpace(),
							new StackLayout{
								Orientation = StackOrientation.Horizontal,
								Children = {
									InfoLabel,
									new StackLayout{
										HorizontalOptions = LayoutOptions.End,
										Children = { img }
									}
								}
							}
						}
					}
				}
			};

			ContentView content = new ContentView{
				Content = stack
			};

			layout.Children.Add(
				content.Content,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				Constraint.RelativeToParent((parent) => {
					return parent.Height;
				})
			);

			while (true) {
				await UIAnimationHelper.ZoomUnZoomElement (img, 1.3, 1000, true);
				await Task.Delay (1000);
			}
		}
		public static async Task ShowTip_HowToGoBackToPlaylistPage(ContentPage page, string text, Color background){
			var layout = ((RelativeLayout)page.Content);

			InfoLabel = UIBuilder.CreateTutorialLabel (text, NamedSize.Large, FontAttributes.None);

			img = UIBuilder.CreateTappableImage ("UpArrow.png", LayoutOptions.Center, Aspect.AspectFit, new Command (() => {
			}), InfoLabel.FontSize);

			stack = new StackLayout { 
				BackgroundColor = new Color (
					background.R, 
					background.G, 
					background.B, 
					0.9
				),
				Children = {
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						HorizontalOptions = LayoutOptions.Center,
						Padding = new Thickness(60),
						Children = {
							UIBuilder.CreateEmptyStackSpace(),
							new StackLayout{
								Orientation = StackOrientation.Horizontal,
								Children = {
									img, InfoLabel
								}
							}
						}
					}
				}
			};

			ContentView content = new ContentView{
				Content = stack
			};

			layout.Children.Add(
				content.Content,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				Constraint.RelativeToParent((parent) => {
					return parent.Height;
				})
			);

			while (true) {
				await UIAnimationHelper.ZoomUnZoomElement (img, 1.3, 1000, true);
				await Task.Delay (1000);
			}
		}

		public static async Task ShowTip_Welcome(ContentPage page, string TutorialText, Color background){
			var layout = ((RelativeLayout)page.Content);

			Debug.WriteLine ("assigned layout");

			fab = UIBuilder.CreateFAB ("", FabSize.Normal, Color.FromHex (Values.GOOGLEBLUE), 
				Color.FromHex (Values.GOOGLEBLUE));

			Debug.WriteLine ("created fab");

			AutoCallInfoLabel = UIBuilder.CreateTutorialLabel (
				"Introducing AUTOCALL\n\nDouble your daily yes calls.\n\nNo more dialling. " +
				"No more typing every text. \nNo more papers to lose.\nNo losing track of your sched\n\n" +
				"CapTap does it all for you",
				NamedSize.Small, FontAttributes.Bold, LineBreakMode.WordWrap,
				new Command(() => {
					UIAnimationHelper.ShrinkUnshrinkElement(AutoCallInfoLabel);
				})
			);

			Debug.WriteLine ("created autocallinfolabel");

			DoneLabel = UIBuilder.CreateTutorialLabel ("Continue", NamedSize.Large, FontAttributes.Bold,
				LineBreakMode.WordWrap, new Command (async () => {
					await ShowAutoCallTip (page, layout, fab);
				}));

			InfoLabel = UIBuilder.CreateLabel (NamedSize.Large);
			InfoLabel.LineBreakMode = LineBreakMode.WordWrap;
			InfoLabel.Text = TutorialText;
			InfoLabel.FontAttributes = FontAttributes.Bold;


			InfoLabel2 = UIBuilder.CreateTutorialLabel ("The fastest way to burn through your namelists!", 
				NamedSize.Medium);
			InfoLabel3 = UIBuilder.CreateTutorialLabel ("Let's press continue to see how it works!", 
				NamedSize.Medium);

			InfoLabel.TextColor = Color.White;

			stack = new StackLayout{ 
				BackgroundColor = new Color (
					background.R, 
					background.G, 
					background.B, 
					0.9
				),
				Children = {
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						HorizontalOptions = LayoutOptions.Center,
						Padding = new Thickness(60),
						Children = {
							InfoLabel,
							UIBuilder.CreateEmptyStackSpace(),

							InfoLabel2, 
							UIBuilder.CreateEmptyStackSpace(),

							InfoLabel3, 
							UIBuilder.CreateEmptyStackSpace(),
							UIBuilder.CreateEmptyStackSpace(),

							DoneLabel
						}
					}
				}
			};
			Debug.WriteLine ("created stack, assinging to contentview");

			ContentView content = new ContentView{
				Content = stack
			};
			Debug.WriteLine ("assigned to contentview, adding tip overlay");

			layout.Children.Add(
				content.Content,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				Constraint.RelativeToParent((parent) => {
					return parent.Height;
				})
			);

			App.NavPage.Navigation.PushModalAsync (page);
		} 

		public static async Task ShowAutoCallTip(ContentPage page, RelativeLayout layout, FloatingActionButton fab){
			AutoCallInfoLabel.Opacity = 0;
			continuePositionX = DoneLabel.X;
			continuePositionY = DoneLabel.Y;

			page.Content = UIBuilder.AddFABToViewWrapRelativeLayout(layout, fab, new Command(() => {}));
			layout = ((RelativeLayout)page.Content);

			UIBuilder.AddElementRelativeToViewonRelativeLayoutParent(layout, AutoCallInfoLabel,
				Constraint.RelativeToParent((parent) =>  { return (parent.Width - fab.Width) * 0.15; }),
				Constraint.RelativeToParent((parent) =>  { return (parent.Height * 0.67) ; })
			);

			await Task.Delay(100);
			DoneLabel.Opacity = 0;
			//UIAnimationHelper.SwitchLabelText (InfoLabel, "Welcome to CapTap!!!", 1000);
			InfoLabel2.FadeTo(0, 500);
			InfoLabel3.FadeTo (0, 500);

			UIAnimationHelper.FlyFromLeft(AutoCallInfoLabel, 1000, true);
			await AutoCallInfoLabel.FadeTo(1, 125, Easing.CubicInOut);
			//DoneLabel.FadeTo (1);
			//DoneLabel.TranslateTo (DoneLabel.X, continuePositionX, 800, Easing.SinOut); 
			//await UIAnimationHelper.ZoomUnZoomElement(fab, 1.3, 500);
			await UIAnimationHelper.ZoomUnZoomElement(fab, 1.3, 500);


			ResetContinueLabel (layout, new Command (() => {
				TransitionWelcomeToPlaylistPageTip(layout);
			}));
		}

		public static async Task ResetContinueLabel(RelativeLayout layout, Command ContinueCommand){
			continuePressed = false;
			layout.Children.Remove (DoneLabel);

			//await Task.Delay (3000);

			DoneLabel.GestureRecognizers.Clear ();
			DoneLabel.GestureRecognizers.Add (new TapGestureRecognizer{Command = new Command(()=>{
				continuePressed = true;
				ContinueCommand.Execute(null);
			})});

			UIBuilder.AddElementRelativeToViewonRelativeLayoutParent(layout, DoneLabel,
				Constraint.RelativeToParent((parent) =>  { return (continuePositionX); }),
				Constraint.RelativeToParent((parent) =>  { return (continuePositionY) ; })
			);

			DoneLabel.FadeTo (1);

			while (!continuePressed) {
				await UIAnimationHelper.ZoomUnZoomElement (DoneLabel, 1.1, 1000, true);
				await Task.Delay (1000);
			}
		}

		public static async Task TransitionWelcomeToPlaylistPageTip(RelativeLayout layout){
			AutoCallInfoLabel.FadeTo (0, 800, Easing.SinOut);
			InfoLabel2.FadeTo (0);
			InfoLabel3.FadeTo (0);
			fab.FadeTo(0, 800, Easing.SinOut);
			UIAnimationHelper.SwitchLabelText (InfoLabel, "Let's try it out!", 1000);

			ResetContinueLabel (layout, new Command (() => {
				MessagingCenter.Send("", Values.DONEWAUTOCALLTIP);
				App.NavPage.Navigation.PopModalAsync();
			}));
		}
	}
}

