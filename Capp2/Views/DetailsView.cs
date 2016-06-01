using System;

using Xamarin.Forms;
using System.Globalization;

namespace Capp2
{
	public class DetailsView : ContentView
	{
		public Label name;

		public DetailsView (ContactData contact)
		{
			BindingContext = contact;

			name = new Label () {
				Text = contact.Name,
				FontSize = 20,
				FontFamily = Device.OnPlatform("HelveticaNeue-Bold","sans-serif-black",null),
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Black
			};
			//name.SetBinding (Label.TextProperty, "Name");

			var namelist = new Label () {
				Text = contact.Playlist,
				FontSize = 15,
				FontFamily = Device.OnPlatform("HelveticaNeue-Light","sans-serif-light",null),
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.FromHex("#666")
			};
			//namelist.SetBinding (Label.TextProperty, "Playlist");

			var info = new Label () {
				Text = "\n\n",
				FontSize = 14,
				FontFamily = Device.OnPlatform("HelveticaNeue","sans-serif",null),
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Black
			};
			if (contact.Called.Date > DateTime.MinValue) {
				info.Text = info.Text + string.Format ("We called {0} last {1}", contact.Name, 
					contact.Called.ToString ("U", CultureInfo.CurrentCulture));
				
				if (contact.Appointed.Date > DateTime.MinValue) {
					info.Text = info.Text + string.Format ("\nbooked {0} for a presentation last {1}", contact.FirstName, 
						contact.Appointed.ToString ("U", CultureInfo.CurrentCulture));
					
					if (contact.Presented.Date > DateTime.MinValue) {
						info.Text = info.Text + string.Format ("\nand showed up {0}", 
							contact.Presented.ToString ("U", CultureInfo.CurrentCulture));

						if (contact.Purchased.Date > DateTime.MinValue) {
							info.Text = info.Text + string.Format ("\nthen signed up/bought products {0}",
								contact.Purchased.ToString ("U", CultureInfo.CurrentCulture));
						} else {
							info.Text = info.Text + "\nand hasn't signed up/bought products";
						}
					} else {
						info.Text = info.Text + "\nand hasn't shown up yet";
					}
				} else {
					info.Text = info.Text + string.Format ("\n{0} hasn't been booked before", contact.FirstName);
				}

			} else {
				info.Text = info.Text + string.Format ("{0} hasn't been called yet", contact.Name);
			}

			var stack = new StackLayout () {
				Padding = new Thickness(20,10),
				Children = {
					name,
					namelist,
					info
				}
			};

			Content = stack;
		}
	}
}


