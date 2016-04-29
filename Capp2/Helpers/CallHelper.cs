using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Capp2
{
	//move autocall and call functionality here
	public static class CallHelper
	{
		static TimeSpan StartTime = new TimeSpan(0,0,0,0,0);

		public static void ShowUserYesCallTime(Page page, bool AutoCall){
			App.SingleCallTimeEllapsed = StopTimer();

			var min = App.SingleCallTimeEllapsed.Minutes;
			var seconds = App.SingleCallTimeEllapsed.Seconds;

			if (AutoCall) {
				AlertHelper.Alert (page, "Total Call Time", 
					string.Format ("You burned through that whole list in {0} minutes and {1} seconds!", min, seconds),
					"OK"
				);
			} else {
				AlertHelper.Alert (page, "Yes Call + Booking Time", 
					string.Format ("You did a Yes Call, recorded the appointment AND texted your prospect in {0} minutes and {1} seconds!", min, seconds),
					"OK"
				);
			}
		}

		public static bool IsTimerRunning(){
			if (StartTime.Days == 0 && StartTime.Hours == 0 && StartTime.Minutes == 0 && StartTime.Seconds == 0 && StartTime.Milliseconds == 0) {
				return false;
			} else {
				return true;
			}
		}

		public static void StartTimer(){
			StartTime = DateTime.Now.TimeOfDay;
			Debug.WriteLine ("Started timer at {0}", StartTime);
		}

		public static TimeSpan StopTimer(){
			var timediff = DateTime.Now.TimeOfDay.Subtract(StartTime);
			Debug.WriteLine ("Stopping timer at {0}, call time was {1}", DateTime.Now.TimeOfDay, timediff);
			StartTime = new TimeSpan(0,0,0,0,0);
			return timediff;
		}
	}
}

