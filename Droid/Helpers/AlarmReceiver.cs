using System;
using Android.Content;
using Android.App;
using Android.Support.V4.App;
using Android.Telephony;
using Android.Widget;

namespace Capp2.Droid
{
	[BroadcastReceiver]
	public class AlarmReceiver : BroadcastReceiver 
	{
		public override void OnReceive (Context context, Intent intent)
		{
			var message = intent.GetStringExtra ("message");
			var title = intent.GetStringExtra ("title");
			//var number = intent.GetStringExtra ("number");
			//var name = intent.GetStringExtra ("name");

			var smsIntent = new Intent (context, typeof(SendSMSActivity));
			var contentIntent = PendingIntent.GetActivity (context, 0, smsIntent, PendingIntentFlags.CancelCurrent);
			var manager = NotificationManagerCompat.From (context);

			var style = new NotificationCompat.BigTextStyle();
			style.BigText(message);

			//sendsms? PhoneContacts

			//Generate a notification with just short text and small icon
			var builder = new NotificationCompat.Builder (context)
				.SetContentIntent (contentIntent)
				.SetSmallIcon (Resource.Drawable.Phone)
				.SetContentTitle(title)
				.SetContentText(message)
				.SetStyle(style)
				.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
				.SetAutoCancel(true)
				/*.Extend(wearableExtender)*/;


			var notification = builder.Build();
			manager.Notify(0, notification);
		}
	}
}

