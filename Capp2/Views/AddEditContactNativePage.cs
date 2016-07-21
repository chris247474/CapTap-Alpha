using System;

using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Capp2
{
	public class AddEditContactNativePage : ContentPage
	{
		public AddEditContactNativePage (ContactData contact = null)
		{
			Debug.WriteLine ("AddContactNativePage");
			App.CurrentContact = contact;
		}

		public static async Task OpenNativeContactsUI(ContactData contact = null) { 
			await App.NavPage.PushAsync(new AddEditContactNativePage(contact));
			await App.NavPage.PopAsync(false);
		}
	}
}


