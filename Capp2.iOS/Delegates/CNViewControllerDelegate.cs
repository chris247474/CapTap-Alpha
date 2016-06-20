using System;
using ContactsUI;
using Contacts;

namespace Capp2.iOS
{
	public class CNViewControllerDelegate: CNContactViewControllerDelegate
	{
		int success;

		public override void DidComplete (CNContactViewController viewController, Contacts.CNContact contact)
		{
			Console.WriteLine ("In DidComplete");

			try{
				if (FieldsFilled(contact)) {
					if (ShouldEditContact ()) {
						App.CurrentContact.FirstName = contact.GivenName;
						App.CurrentContact.LastName = contact.FamilyName;
						App.CurrentContact.Number = contact.PhoneNumbers [0].Value.StringValue;//contactviewmodel
						App.CurrentContact.Aff = contact.OrganizationName;
						success = App.Database.UpdateItem (App.CurrentContact);
					} else {
						success = App.Database.SaveItem (
							new ContactData{
								FirstName = contact.GivenName,
								LastName = contact.FamilyName,
								Playlist = App.CapPage.playlist,
								Number = contact.PhoneNumbers[0].Value.StringValue,//contactviewmodel
								Aff = contact.OrganizationName,
							}
						);
					}

					NotifyUserSavedStatus ();
				}
			}catch(Exception e){
				Console.WriteLine ("Error in CNViewController: {0}", e.Message);
			}

			App.CurrentContact = null;
		}
		void NotifyUserSavedStatus(){
			if (success == 1) {
				AlertHelper.Alert ("Contacts updated!", "");
				App.CapPage.refresh ();
			} else {
				AlertHelper.Alert ("Couldn't save changes. Please try again", "");
			}
		}
		bool FieldsFilled(CNContact contact){
			if (!string.IsNullOrWhiteSpace(contact.GivenName) && !string.IsNullOrWhiteSpace(contact.FamilyName) &&
			   contact.PhoneNumbers.Length > 0) {
				return true;
			}
			return false;
		}
		bool ShouldEditContact(){
			return (App.CurrentContact == null) ? false : true;
		}
	}
}

