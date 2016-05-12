using System;
using System.Diagnostics;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Acr.UserDialogs;

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

		public static async Task CallContact(ContactData contact, bool autocall){
			//to time call cycle of user
			if (!CallHelper.IsTimerRunning ()) {
				CallHelper.StartTimer ();
				Debug.WriteLine ("Timer not running, calling start timer");
			}

			Debug.WriteLine ("Calling " + contact.Name + " autocall: " + autocall.ToString ());
			var dialer = DependencyService.Get<IDialer> ();
			if (dialer != null) {
				if (await dialer.Dial (await HandleMutlipleNumbers (contact))) {
					contact.Called = DateTime.Now;
					App.Database.UpdateItem (contact);
					Debug.WriteLine ("Delaying 4s");
					await Task.Delay (Values.CALLTOTEXTDELAY);
					App.NavPage.Navigation.PushModalAsync (new DatePage (Values.APPOINTED, contact, autocall));
				} 
			} else
				throw new Exception ("dialer return null in CAPP.call()");
		}

		public static async Task call(ContactData contact, bool autocall){
			if (string.Equals (App.SettingsHelper.BOMLocationSettings, "<meetup here>")) {
				App.SettingsHelper.BOMLocationSettings = await Util.GetUserInputSingleLinePromptDialogue ("You haven't entered a meetup place in your meetup templates (go to Settings)", 
					"Enter a default meetup location, then try again", "<meetup here>");

				CallContact (contact, autocall);
			} else {
				CallContact (contact, autocall);
			}
		}

		public static async Task<string> HandleMutlipleNumbers(ContactData contact){
			List<string> list = new List<string> ();

			if (!string.IsNullOrWhiteSpace (contact.Number2)) {
				list.Add (contact.Number);
				list.Add (contact.Number2);
				if (!string.IsNullOrWhiteSpace (contact.Number3)) {
					list.Add (contact.Number3);
				}
				if (!string.IsNullOrWhiteSpace (contact.Number4)) {
					list.Add (contact.Number4);
				}
				if (!string.IsNullOrWhiteSpace (contact.Number5)) {
					list.Add (contact.Number5);
				}

				return await UserDialogs.Instance.ActionSheetAsync ("Which number do we call?", null, null,
					list.ToArray ()
				);
			} 
			return contact.Number;
		}
	}
}

