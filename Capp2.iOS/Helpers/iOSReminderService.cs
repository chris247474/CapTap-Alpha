

using System;

namespace Capp2.iOS.Helpers
{
    class iOSReminderService: IReminderService
    {
        public void Remind(DateTime date, string message, string title) {
            int interval = 0;
            if (date.Millisecond > DateTime.Now.Millisecond)
            {
                interval = date.Millisecond - DateTime.Now.Millisecond;
            }
            else {
                interval = 0;
            }

            var notification = new UIKit.UILocalNotification();
            //notifier.Remind(DateTime.Now.AddMilliseconds(0), "Text Confirmation", "BOM Confirmation texted to " + name);
            notification.FireDate = Foundation.NSDate.FromTimeIntervalSinceNow(interval);
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
