using System;
using Capp2.iOS;
using Xamarin.Forms;
using XLabs.Platform.Services.Email;
using UIKit;
using MessageUI;

[assembly: Dependency(typeof(Capp2.iOS.EmailService))]

namespace Capp2.iOS
{
	public class EmailService:IEmailService
	{
		public EmailService ()
		{
		}

		public void SendDailyEmail(){
			
		}

		public void SendEmail(string recipient = "", string body = "", string subject = ""){
			try{
				var window = UIApplication.SharedApplication.KeyWindow;
				var vc = window.RootViewController;
				while (vc.PresentedViewController != null)
				{
					vc = vc.PresentedViewController;
				}

				MFMailComposeViewController mailController;
				if (MFMailComposeViewController.CanSendMail) {
					// do mail operations here
				}

				mailController = new MFMailComposeViewController ();
				if(!string.IsNullOrWhiteSpace(recipient)){
					mailController.SetToRecipients (new string[]{recipient});
				}
				mailController.SetSubject (subject);
				mailController.SetMessageBody (body, false);

				mailController.Finished += ( object s, MFComposeResultEventArgs args) => {
					Console.WriteLine ("Email sent: {0}", args.Result.ToString ());
					args.Controller.DismissViewController (true, null);
				};

				vc.PresentViewController (mailController, true, null);
			}catch(Exception e){
				Console.WriteLine ("SendEmail error iOS: {0}", e.Message);
			}
		}
	}
}

