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

[assembly: Dependency(typeof(PhoneContacts))]
namespace Capp2.iOS.Helpers
{
    class PhoneContacts : IPhoneContacts
    {
        public bool SaveContactToDevice(string firstName, string lastName, string phone, string aff)
        {
            try {
                ABAddressBook ab = new ABAddressBook();
                ABPerson p = new ABPerson();

                p.FirstName = firstName;
                p.LastName = lastName;
                p.Organization = aff;

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

        public async Task SendSMS(string number, string message, string name, string ConfirmOrBOM, string TodayOrTomorrow = null)
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
            }
            catch (Exception)
            {
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
        }
    }
}
