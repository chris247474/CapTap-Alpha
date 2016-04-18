using Acr.UserDialogs;
using Capp2.Helpers;
using Foundation;
using System;
using UIKit;
using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;

namespace Capp2.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : XFormsApplicationDelegate//global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //var notiftest = new Helpers.iOSReminderService();
            //notiftest.Remind(DateTime.Now, "test", "test");

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
            
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            //viewController.PresentViewController(okayAlertController, true, null);
            UserDialogs.Instance.ShowSuccess(notification.AlertBody, 3000);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}


