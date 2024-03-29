﻿using System;
using Capp2.iOS.Helpers;
using Xamarin.Forms;
using AddressBook;
using Foundation;
using Acr.UserDialogs;
using UIKit;
using Capp2.Helpers;
using System.Threading.Tasks;
using MessageUI;
using System.Linq;
using Plugin.Contacts.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Contacts;
using ContactsUI;

[assembly: Dependency(typeof(PhoneContacts))]
namespace Capp2.iOS.Helpers
{
    class PhoneContacts : IPhoneContacts
	{
		ABAddressBook abb = new ABAddressBook();
		ABPerson[] contacts = null;

		public PhoneContacts(){
			Console.WriteLine ("Storing all contacts in iOS memory");
			contacts = abb.GetPeople ();
		}
		CNMutableContact GetMatchingPerson(CNContact[] contacts)
		{
			CNMutableContact mutableContact;
			for (int c = 0; c < contacts.Length; c++)
			{
				Console.WriteLine("Checking if {0} {1} is who we're looking for",
								  contacts[c].GivenName, contacts[c].FamilyName);
				if (string.Equals(contacts[c].GivenName, App.CurrentContact.FirstName) &&
					string.Equals(contacts[c].FamilyName, App.CurrentContact.LastName) &&
					string.Equals(contacts[c].OrganizationName, App.CurrentContact.Aff) /*&&
					    NumberExistsIn(contactVM.GetNumbers(), contacts[c].PhoneNumbers)*/)
				{
					mutableContact = (contacts[c].MutableCopy() as CNMutableContact);
					Console.WriteLine("Contact: {0} {1} Aff: {2}", mutableContact.GivenName, mutableContact.FamilyName
									  , mutableContact.PhoneNumbers[0].Value.StringValue);
					return mutableContact;
				}
			}
			return null;
		}
		public void ShowAddContactDialogue(bool ShouldEditContact) {
			Console.WriteLine("edit contact? {0}", ShouldEditContact);
			// Create a new Mutable Contact (read/write)
			// and attach it to the editor
			NSError error;
			var store = new CNContactStore();
			var fetchKeys = new[] { CNContactViewController.DescriptorForRequiredKeys };
			var mutableContact = new CNMutableContact();
			CNContactViewController editor;

			if (ShouldEditContact)
			{
				Console.WriteLine("ShouldEditContact()");

				var contacts = store.GetUnifiedContacts(
					CNContact.GetPredicateForContacts(App.CurrentContact.LastName), fetchKeys, out error);

				mutableContact = GetMatchingPerson(contacts);
				if (!mutableContact.AreKeysAvailable(CNContactOptions.PhoneticFamilyName))
				{
					mutableContact = store.GetUnifiedContact(mutableContact.Identifier, fetchKeys, out error).
									  MutableCopy() as CNMutableContact;
					Console.WriteLine("Contact: {0} {1} Aff: {2}", mutableContact.GivenName, mutableContact.FamilyName
									  , mutableContact.PhoneNumbers[0].Value.StringValue);
				}

				editor = CNContactViewController.FromContact(mutableContact);
				Console.WriteLine("CNContactViewController.From... done");
			}
			else {
				Console.WriteLine("New contact");
				editor = CNContactViewController.FromNewContact(mutableContact);
			}

			Console.WriteLine("configuring CNContactViewController");
			// Configure editor
			editor.ContactStore = store;
			editor.AllowsActions = true;
			editor.AllowsEditing = true;
			editor.Delegate = new CNViewControllerDelegate();
			Console.WriteLine("done configuring CNContactViewController");

			MessagingCenter.Subscribe<CNViewControllerDelegate>(this, Values.DONEADDINGCONTACTNATIVE, (args) =>
			{
				Console.WriteLine("recieved DONEADDINGCONTACTNATIVE message");
				NavigationHelper.PopNavToRootThenOpenToCAPPInPlaylist(App.CapPage.playlist, 500, 1000, 500);
			});

			// Display picker
			var navcontrol = iOSNavigationHelper.GetUINavigationController(
					UIApplication.SharedApplication.KeyWindow.RootViewController) as UINavigationController;

			//Console.WriteLine("navcontrol is {0}", navcontrol.GetType());
			navcontrol.PushViewController(editor, true);

			Console.WriteLine("Done w function");
		}

		public async Task PresentNativeAddContactView(){
			//throw new NotImplementedException ("PresentNativeAddContactView still under construction");
			// Create a new Mutable Contact (read/write)
			// and attach it to the editor
			var store = new CNContactStore ();
			var mutableContact = new CNMutableContact ();
			var contactData = new ContactData ();
			var editor = CNContactViewController.FromNewContact (mutableContact);
			var addNewContactDelegate = new CNViewControllerDelegate ();

			// Configure editor
			editor.ContactStore = store;
			editor.AllowsActions = true;
			editor.AllowsEditing = true;
			editor.Delegate = addNewContactDelegate;

			// Display picker
			var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;//ParentViewController as UINavigationController;
			if (vc.PresentedViewController != null)
			{
				vc = vc.PresentedViewController;
				Console.WriteLine ("presentedviewcontroller retrieved");

			}
			vc.PresentViewController (editor, true, null);//.PushViewController (editor, true);
		}

		public ContactData CNMutableContactToContactData(CNMutableContact contact, string playlist){
			if(IsCNMutableContactUsable(contact)){
				return ConvertToContactData (contact, playlist);
			}
			return null;
		}

		public ContactData ConvertToContactData(CNMutableContact contact, string playlist){
			return new ContactData{ 
				FirstName = contact.GivenName,
				LastName = contact.FamilyName,
				Number = contact.PhoneNumbers.FirstOrDefault().Value.StringValue,//convert to ContactDataViewModel
				//Number2 = listpArr[1].Number,
				//Number3 = listpArr[2].Number,
				//Number4 = listpArr[3].Number,
				//Number5 = listpArr[4].Number,
				Playlist = playlist,
				Aff = contact.OrganizationName
			};

			//make numbers callable
		}

		bool IsCNMutableContactUsable(CNMutableContact contact){
			if (contact.PhoneNumbers.Count () > 0 && !string.IsNullOrWhiteSpace (contact.FamilyName)
				&& !string.IsNullOrWhiteSpace(contact.GivenName)) 
			{
				return true;
			}
			return false;
		}

		public async Task Share (string message)
		{
			var messagecontent = message;
			var msg = UIActivity.FromObject (messagecontent);

			var item = NSObject.FromObject (msg);
			var activityItems = new[] { item }; 
			var activityController = new UIActivityViewController (activityItems, null);

			var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

			while (topController.PresentedViewController != null) {
				topController = topController.PresentedViewController;
			}

			topController.PresentViewController (activityController, true, () => {
				/*App.SettingsHelper.DailyEmailTemplateSettings = Settings.DailyEmailTemplateDefault;
				Console.WriteLine("Daily Email Template: {0}", App.SettingsHelper.DailyEmailTemplateSettings);
				Console.WriteLine("Daily Email Template Default: {0}", Settings.DailyEmailTemplateDefault);*/
			});

		}

		public async Task SendSMS (string number){
			var smsTo = NSUrl.FromString("sms:"+number);
			UIApplication.SharedApplication.OpenUrl(smsTo);
		}

		public async Task<List<ContactData>> GetProfilePicPerPerson(List<ContactData> contactList){
			ABAddressBook ab = new ABAddressBook();
			var contacts = ab.GetPeople();
			var contactListArr = contactList.ToArray ();
			//int c = 0;

			try{
				//Settings.Count = ContinueImportingFromLastImageIfNotDone(Settings.Count);
				for(;Settings.Count < contactListArr.Length;Settings.Count++){
					for(int x = 0;x < contacts.Length;x++){
						App.DeviceImageCtr = x;
						if(string.Equals(contactListArr[Settings.Count].FirstName, contacts[x].FirstName) && 
							string.Equals(contactListArr[Settings.Count].LastName, contacts[x].LastName) && 
							contacts[x].HasImage)
						{
							Console.WriteLine("starting inner loop iteration: {0} for contacts index {1}", x, Settings.Count);
							//for listview row
							contactListArr[Settings.Count].PicStringBase64 = 
								SaveImageThenGetPath(contactListArr[Settings.Count], 
									contacts[x].GetImage(ABPersonImageFormat.Thumbnail), 
									ABPersonImageFormat.Thumbnail);
							//Console.WriteLine("assigned small pic");

							//for single page
							contactListArr[Settings.Count].LargePic = 
								SaveImageThenGetPath(contactListArr[Settings.Count],
									contacts[x].GetImage(ABPersonImageFormat.OriginalSize), 
									ABPersonImageFormat.OriginalSize);
							//Console.WriteLine("assigned big pic");
							//update db for bindings
							App.Database.UpdateItem(contactListArr[Settings.Count]);
						}
					}
				}
				Console.WriteLine("Done w all images, {0}", Settings.Count);//App.DeviceImageCtr);
				Settings.Count = 0;
				App.ImageImportingDone = true;
				return contactListArr.ToList<ContactData>();
			}catch(Exception e){
				Console.WriteLine ("PhoneContacts.GetProfilePicPerPerson() iOS error: {0}", e.Message);
			}
			return contactList;
		}

		/*int ContinueImportingFromLastImageIfNotDone(int c){
			if(!IsImageImportingDone()){
				Console.WriteLine("Resuming background image importing at index {0}", c);
				return Settings.Count;
			}
			return c;
		}*/

		bool IsImageImportingDone(){
			if (App.DeviceImageCtr == 0)
				return true;
			else
				return false;
		}

		string SaveDefaultImage(ContactData contact){
			string filename = System.IO.Path.Combine (Environment.GetFolderPath 
				(Environment.SpecialFolder.Personal), 
				"placeholder-contact-male.png");

			Console.WriteLine("Assigned default image to {0} {1}. Saving it as {1}", 
				contact.FirstName, contact.LastName, filename);
			
			return filename;
		}
		string SaveImageThenGetPath(ContactData contact, NSData image, ABPersonImageFormat format){
			string filename = "";

			try{
				if(format == ABPersonImageFormat.Thumbnail){
					filename = System.IO.Path.Combine (Environment.GetFolderPath
						(Environment.SpecialFolder.Personal), 
						string.Format("{0}.jpg", contact.ID)); 
				}else{
					filename = System.IO.Path.Combine (Environment.GetFolderPath
						(Environment.SpecialFolder.Personal), 
						string.Format("{0}-large.jpg", contact.ID));
				}

				image.Save (filename, true);

				Console.WriteLine("Found {0} {1}'s image. Saving it as {2}", 
					contact.FirstName, contact.LastName, filename);
				
				return filename;
			}catch(Exception e){
				Console.WriteLine ("Error in SaveImageThenGetPath(): {0}", e.Message);
			}
			return string.Empty;
		}
		public bool SaveContactToDevice(string firstName, string lastName, string phone, string aff)
        {
            try {
                ABAddressBook ab = new ABAddressBook();
                ABPerson p = new ABPerson();

                p.FirstName = firstName;
                p.LastName = lastName;
                p.Organization = aff;
				//p.GetImage(ABPersonImageFormat.Thumbnail).

                ABMutableMultiValue<string> phones = new ABMutableStringMultiValue();
                phones.Add(phone, ABPersonPhoneLabel.Mobile);

                p.SetPhones(phones);

                ab.Add(p);
                ab.Save();

				UserDialogs.Instance.ShowSuccess("Contact saved: " + firstName + " " + lastName, 2000);

                return true;
            } catch (Exception e) {
                System.Console.WriteLine("[iOS.PhoneContacts] Couldn't save contact: {0} {1}, {2}", firstName, lastName, e.Message);
				UserDialogs.Instance.ShowError("Failed to save contact: "+ firstName + " " + lastName + ". Pls try again.", 2000);
			}
            return false;
        }
		public bool SaveContactToDevice(string firstName, string lastName, 
			List<CNLabeledValue<CNPhoneNumber>> Phones, string aff, bool alert = true)
		{
			try {
				ABAddressBook ab = new ABAddressBook();
				ABPerson p = new ABPerson();

				p.FirstName = firstName;
				p.LastName = lastName;
				p.Organization = aff;
				//p.GetImage(ABPersonImageFormat.Thumbnail).

				ABMutableMultiValue<string> phones = new ABMutableStringMultiValue();
				foreach(CNLabeledValue<CNPhoneNumber> number in Phones){
					phones.Add(number.Value.StringValue, ABPersonPhoneLabel.Mobile);
				}

				p.SetPhones(phones);

				ab.Add(p);
				ab.Save();

				if(alert) UserDialogs.Instance.ShowSuccess("Contact saved: " + firstName + " " + lastName, 2000);

				return true;
			} catch (Exception e) {
				System.Console.WriteLine("[iOS.PhoneContacts] Couldn't save contact: {0} {1}, {2}", firstName, lastName, e.Message);
				if(alert) UserDialogs.Instance.ShowError("Failed to save contact: "+ firstName + " " + lastName + ". Pls try again.", 2000);
			}
			return false;
		}
		public async Task<bool> SendSMS(string number, string message, string name, 
			string ConfirmOrBOM, bool AutoCall = false, string TodayOrTomorrow = null)
        {
            var notifier = new iOSReminderService();
            try
            {
				var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;

                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;

                }
                if (MFMessageComposeViewController.CanSendText)
                {
					Console.WriteLine ("SMS available");

                    MFMessageComposeViewController messageController =
                        new MFMessageComposeViewController();

                    messageController.Finished += (sender, e) => {
						Console.WriteLine("sms sent: {0}", messageController.Body);
						if (AutoCall)
                        {
							System.Console.WriteLine("AutoCallStatus true. Sending iOSDONEWITHCALL message to CAPP");
							MessagingCenter.Send("", Values.iOSDONEWITHCALL);//Android allows programmatically sending an SMS, but iOS requires user to press Send via UI
							System.Console.WriteLine("iOSDONEWITHCALL message sent!");
						}
                        if (string.Equals(ConfirmOrBOM, Values.BOM))
                        {
							//too many alerts to user at the same time, this one isn't necessary
							//notifier.Remind(DateTime.Now.AddMilliseconds(0), "BOM Confirmation texted to " + name, "Text Confirmation");
                        }
                        else {
							MessagingCenter.Send("", Values.DONEWITHCONFIRMTEXT);
                            if (string.Equals(TodayOrTomorrow, Values.TODAY))
                            {
								Console.WriteLine("About to send notification for today");
								try { 
									notifier.Remind(DateTime.Now.AddMilliseconds(0), "Texted " + name + " for later", "Confirming " + name);
								} catch (Exception exc) { Console.WriteLine("Error sending todays notification: {0}", exc.Message);}
								//AlertHelper.Alert(App.CapPage, "Confirming " + name,  "Texted " + name + " for today's meeting", "OK");
							}
                            else {
								Console.WriteLine("About to send notification for tomorrow");
								try
								{
									notifier.Remind(DateTime.Now.AddMilliseconds(0), "Texted " + name + " for tomorrow", "Confirming " + name);
								}catch (Exception ex) { Console.WriteLine("Error sending tomorrows notification: {0}", ex.Message); }
								//AlertHelper.Alert(App.CapPage, "Confirming " + name,  "Texted " + name + " for tomorrow's meeting", "OK");
							}
                        }
                        messageController.DismissViewController(true, null);
                    };
                    
                    messageController.Body = message;
                    messageController.Recipients = new string[] {number };
                    vc.PresentModalViewController(messageController, false);
				}else{
					Console.WriteLine ("Can't send text");
				}
				return true;
            }
            catch (Exception e)
            {
				Console.WriteLine ("PhoneContacts.SendSMS() error: {0}", e.Message);
                if (string.Equals(ConfirmOrBOM, Values.BOM))
                {
                    notifier.Remind(DateTime.Now.AddMilliseconds(0), "BOM Confirmation failed sending to " + name, "Text Confirmation Failed");
                }
                else {

                    if (string.Equals(TodayOrTomorrow, Values.TODAY))
                    {
                        notifier.Remind(DateTime.Now.AddMilliseconds(0), "SMS failed to send", "Couldn't confirm " + name + " for later's meeting");
                    }
                    else {
                        notifier.Remind(DateTime.Now.AddMilliseconds(0), "SMS failed to send", "Couldn't confirm " + name + " for tomorrow's meeting");
                    }
                }
            }
			return false;
        }
		public byte[] ToByte (NSData data)
		{
			byte[] result = new byte[data.Length];
			Marshal.Copy (data.Bytes, result, 0, (int) data.Length);
			return result;
		}
		public string ToBase64String (NSData data)
		{
			return Convert.ToBase64String (ToByte (data));
		}
    }
}


/* if (string.Equals(ConfirmOrBOM, Values.BOM))
                 {
                     notifier.Remind(DateTime.Now.AddMilliseconds(0), "BOM Confirmation failed sending to " + name, "Device can't send SMS");
                 }

                 if (string.Equals(TodayOrTomorrow, Values.TODAY))
                 {
                     notifier.Remind(DateTime.Now.AddMilliseconds(0), "Device can't send SMS", "Couldn't confirm " + name + " for later's meeting");
                 }
                 else {
                     notifier.Remind(DateTime.Now.AddMilliseconds(0), "Device can't send SMS", "Couldn't confirm " + name + " for tomorrow's meeting");
                 }*/