using System;

using Xamarin.Forms;

namespace Capp2
{
	public class HomeView : ContentView
	{
		public HomeView()
		{
			BackgroundColor = Color.FromHex(Values.CAPPTUTORIALCOLOR_Orange);

			var label = new Label {
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White
			};

			Image img = UIBuilder.CreateTappableImage ("", LayoutOptions.FillAndExpand, Aspect.AspectFit, 
				new Command (() => {
					//DependencyService.Get<IVideoHelper>().PlayVideo();
			}), label.FontSize, 30, 75);

			img.SetBinding (Image.SourceProperty, "ImageSource");
			label.SetBinding(Label.TextProperty, "Title");
			this.SetBinding(ContentView.BackgroundColorProperty, "Background");

			Content = new StackLayout {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children = {
					label
				}
			};
		}
	}
}


