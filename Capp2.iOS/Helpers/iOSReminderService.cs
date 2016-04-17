

namespace Capp2.iOS.Helpers
{
    static class iOSReminderService
    {
        public static void Notify(string message, string title, int secondstillnotif) {
            var notification = new UIKit.UILocalNotification();
            //notifier.Remind(DateTime.Now.AddMilliseconds(0), "Text Confirmation", "BOM Confirmation texted to " + name);
            notification.FireDate = Foundation.NSDate.FromTimeIntervalSinceNow(secondstillnotif);
            notification.AlertAction = title;//"View Alert";
            notification.AlertBody = message;

            //modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            // set the sound to be the default sound
            notification.SoundName = UIKit.UILocalNotification.DefaultSoundName;

            // schedule it
            UIKit.UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }
    }
}
