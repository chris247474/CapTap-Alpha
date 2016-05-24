using System;
using Xamarin.Forms;
using System.Diagnostics;

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
	}
}

