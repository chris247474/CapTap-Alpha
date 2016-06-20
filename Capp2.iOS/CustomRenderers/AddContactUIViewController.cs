using System;
using UIKit;
using Contacts;
using ContactsUI;
using Xamarin.Forms.Platform.iOS;
using Capp2.iOS;
using Xamarin.Forms;
using Capp2;
using System.Linq;
using Foundation;


[assembly:ExportRenderer(typeof(AddEditContactNativePage), typeof(AddContactUIViewController))]

namespace Capp2.iOS
{
	public class AddContactUIViewController:PageRenderer
	{
		public override void ViewDidLoad ()
		{
			Console.WriteLine ("AddContactUIViewController Viewdidload");
			base.ViewDidLoad ();

			//throw new NotImplementedException ("PresentNativeAddContactView still under construction");
			// Create a new Mutable Contact (read/write)
			// and attach it to the editor
			NSError error;
			var store = new CNContactStore ();
			var fetchKeys = new [] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.PhoneNumbers }; //contactviewmodel
			var mutableContact = new CNMutableContact ();
			var cnContact = new CNContact ();
			CNContactViewController editor; 

			if (ShouldEditContact ()) {
				Console.WriteLine ("ShouldEditContact()");

				//contactviewmodel
				mutableContact = store.GetUnifiedContacts 
					(CNContact.GetPredicateForContacts (App.CurrentContact.LastName), fetchKeys, out error)
					.Where (person => person.GivenName == App.CurrentContact.FirstName &&
					App.CurrentContact.Number == person.PhoneNumbers [0].Value.StringValue)
					.FirstOrDefault ().MutableCopy () as CNMutableContact;
				
				Console.WriteLine("Contact: {0}", mutableContact.GivenName);
				if (error != null) {
					throw new Exception (error.ToString ()); 
				} else {
					Console.WriteLine("no error");
					editor = CNContactViewController.FromContact (mutableContact);
					Console.WriteLine ("CNContactViewController.From... done");
				}
			} else {
				editor = CNContactViewController.FromNewContact (mutableContact);
			}

			Console.WriteLine ("configuring CNContactViewController");
			// Configure editor
			editor.ContactStore = store;
			editor.AllowsActions = true;
			editor.AllowsEditing = true;
			editor.Delegate = new CNViewControllerDelegate ();
			Console.WriteLine ("done configuring CNContactViewController");

			// Display picker
			var nav = 
				UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers.First().
				ChildViewControllers.Last().ChildViewControllers.First();
			var navcontrol = nav as UINavigationController;
			navcontrol.PushViewController (editor, true);

			Console.WriteLine ("Done w function");
		}

		bool ShouldEditContact(){
			return (App.CurrentContact == null) ? false : true;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			Console.WriteLine ("AddContactUIViewController viewdidappear");
		}
	}
}

