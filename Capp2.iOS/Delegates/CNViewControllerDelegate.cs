using System;
using ContactsUI;
using Contacts;
using UIKit;
using System.Linq;
using Xamarin.Forms;
using Capp2.iOS.Helpers;

namespace Capp2.iOS
{
	public class CNViewControllerDelegate: CNContactViewControllerDelegate
	{
		int success;

		//ContactViewModel contactHandler;

		public override void DidComplete (CNContactViewController viewController, Contacts.CNContact contact)
		{
			Console.WriteLine ("In DidComplete");

			//contactHandler = new ContactViewModel (App.CurrentContact);

			try{
				if (FieldsFilled(contact)) { 
					Console.WriteLine("fieldsfilled addcontactsnative");
					if (ShouldEditContact ()) {    
						Console.WriteLine("should editcontact");
						App.CurrentContact.Name = contact.GivenName + " " + contact.FamilyName;
						App.CurrentContact.FirstName = contact.GivenName;
						App.CurrentContact.LastName = contact.FamilyName;
						App.CurrentContact.Number = contact.PhoneNumbers [0].Value.StringValue;//contactviewmodel
						App.CurrentContact.Aff = contact.OrganizationName;
						success = App.Database.UpdateItem (App.CurrentContact);
						Console.WriteLine("Updated");
						//DB will still load old version of contact
						MessagingCenter.Send (this, Values.DONEADDINGCONTACTNATIVE);
					} else {
						Console.WriteLine("Should not editcontact");
						Console.WriteLine("Saving number {0}", contact.PhoneNumbers[0].Value.StringValue);
						success = App.Database.SaveItem (
							new ContactData{
								Name = contact.GivenName + " " + contact.FamilyName,
								FirstName = contact.GivenName,
								LastName = contact.FamilyName,
								Playlist = App.CapPage.playlist,
								Number = 
									contact.PhoneNumbers[0].Value.StringValue,//contactviewmodel
								Aff = contact.OrganizationName,
							}
						);
						/*Console.WriteLine("Label number {0}", 
							contact.PhoneNumbers[0].Label);//App.contactFuncs.MakeDBContactCallable(contact.PhoneNumbers[0].Value.StringValue, true));
						*/Console.WriteLine("Saved number {0}", 
							contact.PhoneNumbers[0].Value.StringValue);
					}

					NotifyUserSavedStatus ();
					App.CapPage.refresh();
				}
			}catch(Exception e){
				AlertHelper.Alert ("Cancelling!", "");
				Console.WriteLine ("CNViewControllerDelegate error: {0}", e.Message);
				NavigationHelper.PopNavToRootThenOpenToCAPPInPlaylist (App.CapPage.playlist);
			}

			App.CurrentContact = null;
		}
		void NotifyUserSavedStatus(){
			if (success == 1) {
				AlertHelper.Alert ("Contacts updated!", "");
				MessagingCenter.Send("", Values.DONEADDINGCONTACT);
				App.CapPage.refresh ();
			} else {
				AlertHelper.Alert ("Couldn't save changes. Please try again", "");
			}
		}
		bool FieldsFilled(CNContact contact){
			if (!string.IsNullOrWhiteSpace (contact.GivenName) && !string.IsNullOrWhiteSpace (contact.FamilyName) &&
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

