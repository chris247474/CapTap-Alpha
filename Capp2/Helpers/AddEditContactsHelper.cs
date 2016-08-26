using System;
using Xamarin.Forms;

namespace Capp2
{
	public static class AddEditContactsHelper
	{
		public static void AddContact() { 
			App.CurrentContact = null;
			DependencyService.Get<IPhoneContacts>().
							 ShowAddContactDialogue(!(App.CurrentContact == null));
		}

		public static void EditContact(ContactData contact) { 
			App.CurrentContact = contact;
			DependencyService.Get<IPhoneContacts>().
							 ShowAddContactDialogue(!(App.CurrentContact == null));
		}
	}
}

