using System;

using Xamarin.Forms;

namespace Capp2
{
	public class CarouselTemplateView : ContentView
	{
		public CarouselTemplateView (Command OnPop = null)
		{
			Content = CreateView (OnPop);
		}

		StackLayout CreateView(Command OnPop){
			Label TitleLabel = new Label{
				//Text = labelText,
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				BackgroundColor = Color.Transparent,
			};
			TitleLabel.SetBinding(Label.TextProperty, new Binding(){Path = "LabelText"});

			Label DetailLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
			};
			DetailLabel.SetBinding(Label.TextProperty, new Binding(){Path = "DetailText"});

			Image img = UIBuilder.CreateTappableImage ("", LayoutOptions.Center, Aspect.AspectFit, new Command(()=>{
				DependencyService.Get<IVideoHelper>().PlayVideo((this.BindingContext as VideoChooserItem).VideoPath);
			}), TitleLabel.FontSize, 40*0.77, 20*0.77);
			img.SetBinding(Image.SourceProperty, new Binding(){Path = "ImagePath"});

			return new StackLayout {
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Start,
				BackgroundColor = Color.Transparent,
				Padding = new Thickness(30, 0),
				Children = {
					UIBuilder.CreateEmptyStackSpace(),
					UIBuilder.CreateModalXPopper(new Command(()=>{
						if(OnPop != null) OnPop.Execute(null);
						else Navigation.PopModalAsync();
					}), "", "Close-Thin.png", 0.8),
					TitleLabel, 
					new StackLayout{
						Padding = new Thickness(10),
						Children = { DetailLabel }
					},
					UIBuilder.CreateEmptyStackSpace(),
					img, 
					//button indicator
					//backgorund gradient
				}
			};
		}
	}
}


