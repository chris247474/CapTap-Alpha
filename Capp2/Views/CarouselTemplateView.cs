using System;

using Xamarin.Forms;

namespace Capp2
{
	public class CarouselTemplateView : ContentView
	{
		public CarouselTemplateView ()
		{
			Content = CreateView ();
		}

		StackLayout CreateView(){
			Label lbl = new Label{
				//Text = labelText,
				FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
			};
			lbl.SetBinding(Label.TextProperty, new Binding(){Path = "LabelText"});

			Image img = UIBuilder.CreateTappableImage ("", LayoutOptions.Center, Aspect.AspectFit, new Command(()=>{
				DependencyService.Get<IVideoHelper>().PlayVideo((this.BindingContext as VideoChooserItem).VideoPath);
			}), lbl.FontSize, 60, 30);
			img.SetBinding(Image.SourceProperty, new Binding(){Path = "ImagePath"});

			return new StackLayout {
				Padding = new Thickness (10),
				Children = {
					img, lbl
				}
			};
		}
	}
}


