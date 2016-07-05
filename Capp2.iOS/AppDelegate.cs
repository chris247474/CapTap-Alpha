using Acr.UserDialogs;
using Foundation;
using System;
using UIKit;
using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using System.Threading.Tasks;
using Capp2.iOS.Helpers;
using System.Linq;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp;
using FFImageLoading.Forms.Touch;
//using HockeyApp.iOS;

namespace Capp2.iOS
{
    [Register("AppDelegate")]

    public class AppDelegate : XFormsApplicationDelegate//global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
		public override UIWindow Window {
			get;
			set;
		}

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			//var manager = BITHockeyManager.SharedHockeyManager;
			//manager.Configure("3bb7ee25bab74a39992a2d27b2b55ca1");
			//manager.StartManager();

            // check for a notification
            if (options != null)
            {
                // check for a local notification
                if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {
                    var localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                    if (localNotification != null)
                    {
                        UIAlertController okayAlertController = UIAlertController.Create(localNotification.AlertAction, localNotification.AlertBody, UIAlertControllerStyle.Alert);
                        okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        // viewController.PresentViewController(okayAlertController, true, null);
                        UserDialogs.Instance.ShowSuccess(localNotification.AlertBody, 3000);

                        // reset our badge
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                    }
                }
            }

			//SyncFusion charts
			new SfChartRenderer();

            //implement corresponding Init on Android
            #region Resolver Init
            SimpleContainer container = new SimpleContainer();
            container.Register<IDevice>(t => AppleDevice.CurrentDevice);
            container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
            container.Register<INetwork>(t => t.Resolve<IDevice>().Network);
            container.Register<IPhoneService>(t => t.Resolve<IDevice>().PhoneService);//for sms
            try
            {
                Resolver.SetResolver(container.GetResolver());
            }
            catch (Exception)
            {
                //ResetResolver prevents crashing when you tap local notifications that bring you back to the app
                Resolver.ResetResolver(container.GetResolver());
            }
            //implement corresponding Init on Android
            #endregion

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                );

                app.RegisterUserNotificationSettings(notificationSettings);
            }

            global::Xamarin.Forms.Forms.Init();

			//FFImageLoading
			CachedImageRenderer.Init();

			//FABForms
			new FAB.iOS.FloatingActionButtonRenderer();

			//FireBase for Google AdMobs Analytics
			IFirebaseConfig config = new FirebaseConfig
			{
				AuthSecret = "ABNRVFSSpRqCXZBusVQVw2XltaZcjqxJPe2uEpYM",
				BasePath = "https://admob-app-id-1021936400.firebaseio.com"
			};
			IFirebaseClient client = new FirebaseClient(config);

			SetUIStyles (app);
            
            LoadApplication(new App());
			//DidEnterBackground (app);

			/*new System.Threading.Thread(new System.Threading.ThreadStart(async () => {
				PhoneContacts PhoneFunc = new PhoneContacts();
				await PhoneFunc.GetProfilePicPerPerson(App.Database.GetItems(Values.ALLPLAYLISTPARAM).ToList());
			})).Start();*/

			//App.ImageImportingDone = true;
            return base.FinishedLaunching(app, options);
        }

		//resets user keyboard to default iOS keyboard - layouts crash when custom keyboard is used
		public override bool ShouldAllowExtensionPointIdentifier(UIApplication application, NSString extensionPointIdentifier)
		{
			return extensionPointIdentifier != UIExtensionPointIdentifier.Keyboard;
		}

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            UserDialogs.Instance.ShowSuccess(notification.AlertBody, 3000);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

		public override void DidEnterBackground (UIApplication application)
		{
			base.DidEnterBackground (application);
			Console.WriteLine ("In Background iOS");

			nint taskID = UIApplication.SharedApplication.BeginBackgroundTask( () => {});

			if (!App.ImageImportingDone) {
				Console.WriteLine ("Image importing not yet started, starting");
				new Task (async () => {
					PhoneContacts PhoneFunc = new PhoneContacts();

					await PhoneFunc.GetProfilePicPerPerson(App.Database.GetItems(Values.ALLPLAYLISTPARAM).ToList<ContactData>());

					UIApplication.SharedApplication.EndBackgroundTask(taskID);
				}).Start();
			}
		}

		void SetUIStyles(UIApplication iOSApp){
			var win = new UIWindow(UIScreen.MainScreen.Bounds);

			/*var StartColor = ColorFromHex(Values.PURPLE);  
			var EndColor = ColorFromHex(Values.GOOGLEBLUE);*/

			//UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(43, 132, 211); //bar background 

			//UINavigationBar.Appearance.TintColor = ColorFromHex(Values.GOOGLEBLUE); //Tint color of button items  
			/*UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes() { 
				Font = UIFont.FromName("HelveticaNeue-Light", (nfloat)20f), TextColor = UIColor.White 
			});*/

			/*var gradientLayer = new CAGradientLayer ();
			gradientLayer.Frame = win.Bounds;//View.Bounds;
			gradientLayer.Colors = new CGColor[] { StartColor.CGColor, EndColor.CGColor };
			win.Layer.InsertSublayer (gradientLayer, 0);*/
		}
		void SetGradientLayerAsiOSBackground(UIWindow win, UIColor StartColor, UIColor EndColor)
		{
			
		}
		UIColor ColorFromHex (string hexValue, float alpha = 1.0f)
		{
			var colorString = hexValue.Replace ("#", "");
			if (alpha > 1.0f) {
				alpha = 1.0f;
			} else if (alpha < 0.0f) {
				alpha = 0.0f;
			}

			float red, green, blue;

			switch (colorString.Length) 
			{
			case 3 : // #RGB
				{
					red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
					green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
					blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
					return UIColor.FromRGBA(red, green, blue, alpha);
				}
			case 6 : // #RRGGBB
				{
					red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
					green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
					blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;

					return UIColor.FromRGBA(red, green, blue, alpha);
				}   

			default :
				throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

			}
		}
    }
}


