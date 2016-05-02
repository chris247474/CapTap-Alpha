using System;
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
		public async Task<ContactData> GetProfilePic(ContactData contact){
			

			try{
				for(int x = 0;x < contacts.Length/2;x++){
					if(string.Equals(contact.FirstName, contacts[x].FirstName) && 
						string.Equals(contact.LastName, contacts[x].LastName) && 
						contacts[x].HasImage)
					{
						/*contact.PicStringBase64 = ToBase64String(
							contacts[x].GetImage(ABPersonImageFormat.Thumbnail)
						);*/
						Console.WriteLine("Found {0} {1}'s image. Adding it", contact.FirstName, contact.LastName);
					}
				}

				/*var contacts = ab.GetPeople().Where(x => x.HasImage).Where(x => x.FirstName == contact.FirstName).Where(x => x.LastName == contact.LastName).Select(x => x);
				var contactsarr = contacts.ToArray ();
				Console.WriteLine ("{0} Contacts found", contactsarr.Length);
				contact.PicStringBase64 = ToBase64String(contactsarr[0].GetImage(ABPersonImageFormat.Thumbnail));*/
			}catch(Exception e){
				Console.WriteLine ("PhoneContacts.GetProfilePic() iOS error: {0}", e.Message);
			}
			return contact;
		}

		public async Task<List<ContactData>> GetProfilePicPerPerson(List<ContactData> contactList){
			ABAddressBook ab = new ABAddressBook();
			var contacts = ab.GetPeople();
			var contactListArr = contactList.ToArray ();

			try{
				for(int c = 0;c < contactListArr.Length;c++){
					for(int x = 0;x < contacts.Length;x++){
						if(string.Equals(contactListArr[c].FirstName, contacts[x].FirstName) && 
							string.Equals(contactListArr[c].LastName, contacts[x].LastName) && 
							contacts[x].HasImage)
						{
							contactListArr[c].PicStringBase64 = ToBase64String(
							contacts[x].GetImage(ABPersonImageFormat.Thumbnail)
							);
							Console.WriteLine("Found {0} {1}'s image. Adding it", contactListArr[c].FirstName, contactListArr[c].LastName);
						}
					}
				}
				return contactListArr.ToList();
			}catch(Exception e){
				Console.WriteLine ("PhoneContacts.GetProfilePicPerPerson() iOS error: {0}", e.Message);
			}
			return contactList;
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

		public async Task<bool> SendSMS(string number, string message, string name, string ConfirmOrBOM, string TodayOrTomorrow = null)
        {
            var notifier = new iOSReminderService();
            try
            {
                //in iOS, only way to send text is by hand which means no programmatical sms send. This calls Messages without inputting initial text
                //var smsTo = NSUrl.FromString("sms:"+number);
                //UIApplication.SharedApplication.OpenUrl(smsTo);

                //inputs initial text
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                if (MFMessageComposeViewController.CanSendText)
                {
                    MFMessageComposeViewController messageController =
                        new MFMessageComposeViewController();

                    messageController.Finished += (sender, e) => {
						System.Console.WriteLine("iOS Messages opened. AutoCallStatus: {0}", App.AutoCallStatus.ToString());
                        if (App.AutoCallStatus)
                        {
							System.Console.WriteLine("AutoCallStatus true. Sending iOSDONEWITHCALL message to CAPP");
							MessagingCenter.Send("", Values.iOSDONEWITHCALL);//Android allows programmatically sending an SMS, but iOS requires user to press Send via UI
							System.Console.WriteLine("iOSDONEWITHCALL message sent!");
						}
                        if (string.Equals(ConfirmOrBOM, Values.BOM))
                        {
							//replace all notifiers with native Alert Dialogue

							//too many alerts to user at the same time, this one isn't necessary
							//notifier.Remind(DateTime.Now.AddMilliseconds(0), "BOM Confirmation texted to " + name, "Text Confirmation");

							//AlertHelper.Alert(App.Current.MainPage, "Text Sent!", "Sent BOM Meetup Text to "+name, "OK");
                        }
                        else {
                            if (string.Equals(TodayOrTomorrow, Values.TODAY))
                            {
                                notifier.Remind(DateTime.Now.AddMilliseconds(0), "Texted " + name + " for later", "Confirming " + name);
								//AlertHelper.Alert(App.CapPage, "Confirming " + name,  "Texted " + name + " for today's meeting", "OK");

							}
                            else {
                                notifier.Remind(DateTime.Now.AddMilliseconds(0), "Texted " + name + " for tomorrow", "Confirming " + name);
								//AlertHelper.Alert(App.CapPage, "Confirming " + name,  "Texted " + name + " for tomorrow's meeting", "OK");

							}
                        }
                        messageController.DismissViewController(true, null);
                    };
                    
                    messageController.Body = message;
                    messageController.Recipients = new string[] {number };
                    vc.PresentModalViewController(messageController, false);
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