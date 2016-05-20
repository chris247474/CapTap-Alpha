using System;

using Xamarin.Forms;

namespace Capp2
{
	public class CAPPTutorialView : ContentPage
	{
		Label InfoLabel, DoneLabel;
		StackLayout stack;

		public CAPPTutorialView (string TutorialText, Color background)
		{
			CreateUIElements (TutorialText);
			Content = CreateView (background);
		}
		void CreateUIElements(string tutorialtext){
			DoneLabel = UIBuilder.CreateLabel(NamedSize.Large);
			DoneLabel.Text = "CONTINUE";
			DoneLabel.GestureRecognizers.Add (new TapGestureRecognizer{Command = new Command(() => {
				Navigation.PopModalAsync();
			})});

			InfoLabel = UIBuilder.CreateLabel (NamedSize.Large);
			InfoLabel.LineBreakMode = LineBreakMode.WordWrap;
			InfoLabel.Text = tutorialtext;

			InfoLabel.TextColor = DoneLabel.TextColor = Color.White;
		}
		View CreateView(Color background){
			this.Opacity = 0.3;
			//this.BackgroundColor = background;
			this.BackgroundColor = Color.Transparent;

			stack = new StackLayout{ 
				Children = {
					UIBuilder.CreateEmptyStackSpace(),
					UIBuilder.CreateEmptyStackSpace(),

					new StackLayout{
						Orientation = StackOrientation.Vertical,
						HorizontalOptions = LayoutOptions.Center,
						Padding = new Thickness(30),
						Children = {
							InfoLabel, DoneLabel
						}
					}
				}
			};

			return stack;
		}
	}
}


