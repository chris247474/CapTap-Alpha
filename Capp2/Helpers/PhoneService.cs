using Acr.UserDialogs;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Capp2.Helpers
{
    public static class PhoneService
    {
        public static async Task Dial(string number) {
            try {
                var phoneDialer = CrossMessaging.Current.PhoneDialer;
                if (phoneDialer.CanMakePhoneCall) {
                    phoneDialer.MakePhoneCall(number);
                } else {
                    Debug.WriteLine("PhoneService.Dial(string) isn't allowed");
                    UserDialogs.Instance.WarnToast("Hmm. Call failed. Pls try again");
                }
            } catch (Exception e) {
                Debug.WriteLine("PhoneService.Dial(string) error: {0}", e.Message);
                UserDialogs.Instance.WarnToast("Hmm. Call failed. Pls try again");
            }
        }

        public static void SendSMS(string number, string text, string name, string ConfirmOrBOM, string TodayOrTomorrow = null) {
            var notifier = DependencyService.Get<IReminderService>();
            try {
                var smsMessenger = CrossMessaging.Current.SmsMessenger;
                if (smsMessenger.CanSendSms)
                {
                    Debug.WriteLine("Messaging available");
                    smsMessenger.SendSms(number, text);
                    //return true;

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
                }
                else {
                    Debug.WriteLine("PhoneService.SendSMS(string, string) isn't allowed");
                    UserDialogs.Instance.WarnToast("Your phone can't seem to send a text. Please try again");
                    //return false;
                }
            } catch (Exception e) {
                Debug.WriteLine("PhoneService.SendSMS() error: {0}", e.Message);
                //return false;
            }
        }
    }
}
