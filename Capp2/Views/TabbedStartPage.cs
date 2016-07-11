using System;
using Xamarin.Forms;

namespace Capp2
{
	public class TabbedStartPage:TabbedPage
	{
		public TabbedStartPage ()
		{
			App.StartTabbedPage = this; 

			Device.OnPlatform(
				() => App.NavPage = new NavigationPage(new PlaylistPage()){
				}, 
				() => App.NavPage = new NavigationPage(new PlaylistPage()){
					BarBackgroundColor = Color.FromHex (Values.GOOGLEBLUE),
					BarTextColor = Color.White,
				}
			);
			App.NavPage.Icon = "placeholder-contact-male.png";
			App.NavPage.Title = "Contacts";

			Children.Add (App.NavPage);
			Children.Add (new SettingsPage ());

			App.NavPage.Navigation.PopAsync();
			App.NavPage.Navigation.PushAsync(new CAPP(App.DefaultNamelist));
		}
	}
}

