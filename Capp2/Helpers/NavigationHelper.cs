using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Capp2
{
	public static class NavigationHelper
	{
		public static void ClearModals(Page nav){
			var ModalCount = nav.Navigation.ModalStack.Count;
			for(int c = 0;c < ModalCount;c++){
				Debug.WriteLine("Poppping modal {0}", c);
				nav.Navigation.PopModalAsync();
			}
		}

		public static void ClearNavigationStack(Page page){
			var NavStackCount = page.Navigation.NavigationStack.Count;
			for(int c = 0;c < NavStackCount;c++){
				Debug.WriteLine("Poppping modal {0}", c);
				page.Navigation.PopModalAsync();
			}
		}

		public static async void PopNavToRootThenOpenToCAPPInPlaylist(string playlist = Values.ALLPLAYLISTPARAM, 
			int startdelay = 0, int middelay = 1000, int enddelay = 0
		){
			await Task.Delay (startdelay);

			if (App.MasterDetailPage == null)
			{
				/*App.NavPage = new NavigationPage(new PlaylistPage());
				App.NavPage.Icon = "placeholder-contact-male.png";
				App.NavPage.Title = "Contacts";
				App.StartTabbedPage.Children.RemoveAt(0);
				App.StartTabbedPage.Children.Insert(0, App.NavPage);
				App.StartTabbedPage.CurrentPage = App.NavPage;*/
				RefreshNavPageInTabbedPageRoot();
				await Task.Delay(middelay);
				App.NavPage.Navigation.PushAsync(new CAPP(playlist));
				await Task.Delay(enddelay);
			}
			else {
				App.NavPage = new NavigationPage(new PlaylistPage());
				await Task.Delay(middelay);
				App.MasterDetailPage.Detail = App.NavPage;
				App.NavPage.Navigation.PushAsync(new CAPP(playlist));
				await Task.Delay(enddelay);
			}

		}

		public static async Task PopToRootThenOpenToNamelistPage(int startdelay = 0, int middelay = 0, int enddelay = 0) {
			await Task.Delay(startdelay);
			if (App.MasterDetailPage != null)
			{
				App.NavPage = new NavigationPage(new PlaylistPage());
				App.MasterDetailPage.Detail = App.NavPage;
			}
			else NavigationHelper.RefreshNavPageInTabbedPageRoot();
			await Task.Delay(enddelay);
		}

		public static void RefreshNavPageInTabbedPageRoot() { 
			App.NavPage = new NavigationPage(new PlaylistPage());
			App.NavPage.Icon = "placeholder-contact-male.png";
			App.NavPage.Title = "Contacts";
			App.StartTabbedPage.Children.RemoveAt(0);
			App.StartTabbedPage.Children.Insert(0, App.NavPage);
			App.StartTabbedPage.CurrentPage = App.NavPage;
		}
	}
}

