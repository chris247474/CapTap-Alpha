using System;
using System.Diagnostics;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace Capp2
{
	public class CarouselTemplateView : ContentView
	{
		RelativeLayout relativeLayout = new RelativeLayout();
		Command OnPop;

		public CarouselTemplateView (Command OnPop = null)
		{
			this.OnPop = OnPop;
			Content = CreateView(OnPop);
		}

		RelativeLayout CreateView(Command OnPop){
			Label TitleLabel, DetailLabel;
			Image img;
			StackLayout ModalXPopper;

			TitleLabel = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				BackgroundColor = Color.Transparent,
			};
			TitleLabel.SetBinding(Label.TextProperty, new Binding() { Path = "LabelText" });
			TitleLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;

			DetailLabel = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.White,
				BackgroundColor = Color.Transparent,
			};
			DetailLabel.SetBinding(Label.TextProperty, new Binding() { Path = "DetailText" });
			DetailLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;

			img = UIBuilder.CreateTappableImage(Aspect.AspectFit, new Command(() => { }));
			img.SetBinding(Image.SourceProperty, new Binding() { Path = "ImagePath" });

			//var browser = new BindableGIFWebView();
			//browser.SetBinding(BindableGIFWebView.GIFSourceProperty, new Binding() { Path = "GIFSource" });

			ModalXPopper = UIBuilder.CreateModalXPopper(new Command(() =>
			{
				if (OnPop != null) OnPop.Execute(null);
				else Navigation.PopModalAsync();
			}), "", "Close-Thin.png", 0.8);
			ModalXPopper.HorizontalOptions = LayoutOptions.CenterAndExpand;

			var TitleDetailStack = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(0, 30, 0, 5),
				Children = {
					ModalXPopper, TitleLabel, DetailLabel
				}
			};

			relativeLayout.Children.Add(
				TitleDetailStack,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) => {
					return parent.Width;
				})
			);

			relativeLayout.Children.Add(
				img,
				Constraint.Constant(0),
				Constraint.RelativeToView(TitleDetailStack, (parent, view) =>
				{
					return view.Y + view.Height;
				})
			);

			return relativeLayout;
		}
	}
}


