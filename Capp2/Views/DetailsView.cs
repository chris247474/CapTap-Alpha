using System;

using Xamarin.Forms;
using System.Globalization;

namespace Capp2
{
	public class DetailsView : ContentView
	{
		public Label name, info;
		bool infoSwapped = false;

		public DetailsView (ContactData contact)
		{
			BindingContext = contact;

			name = new Label () {
				//Text = contact.Name,
				FontSize = 20,
				//FontFamily = Device.OnPlatform("HelveticaNeue-Bold","sans-serif-black",null),
				FontAttributes = FontAttributes.Bold,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Black
			};
			name.SetBinding (Label.TextProperty, new Binding(){Path = "Name"});

			var namelist = new Label () {
				Text = contact.Playlist,
				FontSize = 15,
				//FontFamily = Device.OnPlatform("HelveticaNeue-Light","sans-serif-light",null),
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.FromHex("#666")
			};
			//namelist.SetBinding (Label.TextProperty, "Playlist");

			var numbers = new Label () {
				Text = CallHelper.GetNumbers(contact),
				FontSize = 15,
				//FontFamily = Device.OnPlatform(/*"HelveticaNeue-Light"*/"SF-UI","sans-serif-light",null),
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.FromHex("#666")
			};

			info = new Label {
				Text = GetCallInfo(contact),
				FontSize = 16,
				//FontFamily = Device.OnPlatform(/*"HelveticaNeue-Light"*/"SF-UI","sans-serif-light",null),
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Black
			};
			/*info.GestureRecognizers.Add(new TapGestureRecognizer{Command = new Command(()=>{
				if(infoSwapped){
					info.Text = GetCallInfo(contact);
					infoSwapped = false;
				}else{
					info.Text = GetGeneralInfo(contact);
					infoSwapped = true;
				}
			})});*/

			var stack = new StackLayout () {
				Padding = new Thickness(20,10),
				Children = {
					name,
					namelist, 
					contact.HasDefaultImage_Large ? UIBuilder.EmptyStack() : UIBuilder.CreateTripleEmtyStackSpace(), 
					info
				}
			};

			Content = stack;
		}

		string GetCallInfo(ContactData contact){
			string infotext = "\n";
			if (contact.Called.Date > DateTime.MinValue) {
				infotext = infotext  + string.Format ("We called {0} last {1}", contact.Name, 
					contact.Called.ToString ("MMMM dd, yyyy"));

				if (contact.Appointed.Date > DateTime.MinValue) {
					infotext = infotext  + string.Format ("\nbooked {0} for a meeting last {1}", contact.FirstName, 
						contact.Appointed.ToString ("MMMM dd, yyyy"));

					if (contact.Presented.Date > DateTime.MinValue) {
						infotext = infotext  + string.Format ("\nand showed up {0}", 
							contact.Presented.ToString ("MMMM dd, yyyy"));

						if (contact.Purchased.Date > DateTime.MinValue) {
							infotext  = infotext  + string.Format ("\nthen purchased {0}",
								contact.Purchased.ToString ("MMMM dd, yyyy"));
						} else {
							infotext  += "\nand hasn't purchased";
						}
					} else {
						infotext  += "\nand hasn't shown up yet";
					}
				} else {
					infotext  += string.Format ("\n{0} hasn't been booked before", 
						contact.FirstName);
					if (contact.IsSetForNextCall) {
						infotext  += string.Format (", but said we should call back on {0}", 
							contact.NextCall.ToString ("MMMM dd, yyyy"));
					}
				}

			} else {
				infotext  += string.Format ("{0} hasn't been called yet", contact.Name);
			}

			return infotext;
		}

		string GetGeneralInfo(ContactData contact){
			string info = string.Empty;

			info += CallHelper.GetNumbers (contact);
			if (!string.IsNullOrWhiteSpace (contact.Aff)) {
				info += string.Format ("\n\nWorks at {0}", contact.Aff);
			}

			return info;
		}
	}
}


