using System;

using Xamarin.Forms;
using System.Diagnostics;

namespace Capp2
{
	public class AddEditContactNativePage : ContentPage
	{
		public AddEditContactNativePage (ContactData contact = null)
		{
			Debug.WriteLine ("AddContactNativePage");
			App.CurrentContact = contact;
		}
	}
}


