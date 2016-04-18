using Acr.UserDialogs;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static async Task SendSMS(string number, string text) {
            try {
                var smsMessenger = CrossMessaging.Current.SmsMessenger;
                if (smsMessenger.CanSendSms)
                    smsMessenger.SendSms(number, text);
                else {
                    Debug.WriteLine("PhoneService.SendSMS(string, string) isn't allowed");
                    UserDialogs.Instance.WarnToast("Your phone can't seem to send a text. Please try again");
                }
            } catch (Exception e) { Debug.WriteLine("PhoneService.SendSMS() error: {0}", e.Message); }
        }
    }
}
