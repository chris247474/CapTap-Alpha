using System;
using Capp2.Droid;
using Android.Content;
using Xamarin.Forms;
using Android.App;
using Android.OS;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidReminderService))]
namespace Capp2.Droid
{
	public class AndroidReminderService : IReminderService
	{
		#region IReminderService implementation

		public void Remind (DateTime dateTime, string title, string message/*, string number, string name*/)
		{
            System.Console.WriteLine("ENTERED AndroidReminderService");
			Intent alarmIntent = new Intent(Forms.Context, typeof(AlarmReceiver));
			alarmIntent.PutExtra ("message", message);
			alarmIntent.PutExtra ("title", title);
			//alarmIntent.PutExtra ("number", number);
			//alarmIntent.PutExtra ("name", name);

			PendingIntent pendingIntent = PendingIntent.GetBroadcast(Forms.Context, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
			AlarmManager alarmManager = (AlarmManager) Forms.Context.GetSystemService(Context.AlarmService);

			//TODO: For demo set after 5 seconds.
			alarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime () + 5 * 1000, pendingIntent);


		}

		#endregion
	}
}

