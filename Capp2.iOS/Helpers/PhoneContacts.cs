using System;
using Capp2.iOS.Helpers;
using Xamarin.Forms;
using AddressBook;
using Foundation;
using Acr.UserDialogs;
using UIKit;
using Capp2.Helpers;
using System.Threading.Tasks;

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

                UserDialogs.Instance.SuccessToast("Contact saved: " + firstName + " " + lastName, null, 2000);
                return true;
            } catch (Exception e) {
                System.Console.WriteLine("[iOS.PhoneContacts] Couldn't save contact: {0} {1}, {2}", firstName, lastName, e.Message);
                UserDialogs.Instance.ErrorToast("Failed to save contact: "+ firstName + " " + lastName + ". Pls try again.", null, 2000);
            }
            return false;
        }

        public async Task SendSMS(string number, string message, string name, string ConfirmOrBOM, string TodayOrTomorrow = null)
        {
            var notifier = new iOSReminderService();
            try {
                //send sms
                //in iOS, only way to send text is by hand which means no programmatical sms send
                // var smsTo = NSUrl.FromString("sms:"+number);
                //UIApplication.SharedApplication.OpenUrl(smsTo);

                //XLabs cross platform impl, above ios native doesnt allow predefined sms text passed to Messaging app
                await PhoneService.SendSMS(number, message);

                if (string.Equals(ConfirmOrBOM, Values.BOM))
                {
                    notifier.Remind(DateTime.Now.AddMilliseconds(0), "BOM Confirmation texted to " + name, "Text Confirmation");//"View Alert";
                }
                else {
                    if (string.Equals(TodayOrTomorrow, Values.TODAY))
                    {
                        notifier.Remind(DateTime.Now.AddMilliseconds(0), "Texted " + name + " for later", "Confirming " + name);
                    }
                    else {
                        notifier.Remind(DateTime.Now.AddMilliseconds(0), "Texted " + name + " for tomorrow", "Confirming " + name);
                    }
                }
               // return true;
            } catch (Exception) {
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
             //   return false;
            }
        }
    }
}
