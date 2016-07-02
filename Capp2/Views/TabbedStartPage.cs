using System;
using Xamarin.Forms;

namespace Capp2
{
	public class TabbedStartPage:TabbedPage
	{
		public TabbedStartPage ()
		{
			Device.OnPlatform(
				() => App.NavPage = new NavigationPage(new PlaylistPage{StartColor = App.StartColor, 
					EndColor = App.EndColor}){
					//BarBackgroundColor = Color.White,
					//BarTextColor = Color.FromHex(Values.GOOGLEBLUE),
				}, 
				() => App.NavPage = new NavigationPage(new PlaylistPage{StartColor = App.StartColor, EndColor = App.EndColor}){
					BarBackgroundColor = Color.FromHex (Values.GOOGLEBLUE),
					BarTextColor = Color.White,
				}
			);
			App.NavPage.Icon = "placeholder-contact-male.png";
			App.NavPage.Title = "Contacts";

			Children.Add (App.NavPage);
			Children.Add (new SettingsPage ());

		}
	}
}

