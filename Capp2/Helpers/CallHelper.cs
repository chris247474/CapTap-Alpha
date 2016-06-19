using System;
using System.Diagnostics;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Acr.UserDialogs;
using System.Linq;

namespace Capp2
{
	public static class CallHelper
	{
		static TimeSpan StartTime = new TimeSpan(0,0,0,0,0);

		public static List<ContactData> AutoCallList;
		public static bool AutoCallContinue, calling;
		public static int AutoCallCounter;

		public static string GetNumbers(ContactData contact){
			string numbers = string.Empty;
			numbers += string.Format("\n{0}",contact.Number);
			numbers += string.Format("\n{0}",contact.Number2);
			numbers += string.Format("\n{0}",contact.Number3);
			numbers += string.Format("\n{0}",contact.Number4);
			numbers += string.Format("\n{0}",contact.Number5);
			return numbers;
		}

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

		public static async Task SendDoneMessageAsync(){
			Device.OnPlatform (
				() => {
					Debug.WriteLine("Sending iOSDONEWITHCALL");
					MessagingCenter.Send ("", Values.iOSDONEWITHCALL);
				}
				, () => {
					Debug.WriteLine("Sending DONEWITHCALL");
					MessagingCenter.Send ("", Values.DONEWITHCALL);
				}
			);
		}

		/*public static async Task AutoCall(string playlist){
			if (App.Database.GetItems (playlist).Count () == 0) {
				await AlertHelper.Alert (string.Format ("{0} has no contacts to call", playlist), "");
			} else {
				if (string.Equals (App.SettingsHelper.BOMLocationSettings, "<meetup here>")) {
					App.SettingsHelper.BOMLocationSettings = await Util.GetUserInputSingleLinePromptDialogue ("You haven't entered a meetup place in your meetup templates (go to Settings)", 
						"Enter a default meetup location, then try again", "<meetup here>");

					//DoAutoCall (playlist);
					await CheckToResumeCallingIfPlaylistCalledBeforeThenCall(playlist);
				} else {
					//DoAutoCall (playlist);
					await CheckToResumeCallingIfPlaylistCalledBeforeThenCall(playlist);
				}
			}
		}*/
		public static async Task CheckToResumeCallingIfPlaylistCalledBeforeThenCall(string playlist){
			if (CanResume (playlist)) {
				var resume = await UserDialogs.Instance.ConfirmAsync ("Resume calling from where we left off?", 
					             "We've called this list before", "Resume", "Start Over");
				if (resume) {
					//call from last index
					var lastindexcalled = GetPlaylistItemThenNullCheck (playlist).LastIndexCalled;
					Debug.WriteLine ("Setting AutoCallCounter from {0} to {1}", AutoCallCounter, lastindexcalled);
					AutoCallCounter = lastindexcalled;
				} else {
					//start from first number in list
					await DoAutoCall (playlist);
				}
			} else {
				//list hasn't been called before, start from first number
				Debug.WriteLine("List {0} hasn't been called before, startin from index 0", playlist);
				await DoAutoCall (playlist);
			}
		}
		public static async Task DoAutoCall(string playlist){
			if(calling){
				SetupNotAutoCalling (playlist);
			}else{
				SetupAutoCalling ();
				await autoCall(playlist);
			}
		}

		public static async Task autoCall(string playlist){
			try{
				PrepareForAutoCall (playlist);
				await StartContinueAutoCall (playlist);
			}catch(Exception e){
				Debug.WriteLine ("PrepareForAutoCall() error: {0}", e.Message);
				UserDialogs.Instance.WarnToast ("Your phone may have randomly lagged. Please try again");
			}
		}
		public static void PrepareForAutoCall(string playlist){
			AutoCallCounter = 0;
			AutoCallContinue = true;
			AutoCallList = App.Database.GetItems(playlist).ToList<ContactData> ();
		}
		public static void SetupNotAutoCalling(string playlist){
			calling = false;
			AutoCallContinue = false;

			UpdateContactLastCalled (playlist);

			CallHelper.ShowUserYesCallTime (App.CapPage, true);
		}
		public static void UpdateContactLastCalled(string playlist){
			var playlistItem = GetPlaylistItemThenNullCheck (playlist);

			Debug.WriteLine ("assigning {0} as last index called, previous listindexcalled is {1}", 
				AutoCallCounter - 1, playlistItem.LastIndexCalled);

			if (AutoCallCounter < AutoCallList.Count) {
				playlistItem.LastIndexCalled = AutoCallCounter - 1;
				App.Database.UpdateItem (playlistItem);
			}
		}
		public static Playlist GetPlaylistItemThenNullCheck(string playlist){
			var playlistItem = App.Database.GetPlaylist(playlist);
			if (playlistItem == null) {
				throw new NullReferenceException ( 
					string.Format("Attempted to update last contact called, but playlist {0} doesn't exist", playlist));
			}
			return playlistItem;
		}
		public static bool CanResume(string playlist){
			return (GetPlaylistItemThenNullCheck (playlist).LastIndexCalled == 0) ? false : true;
		}
		public static void SetupAutoCalling(){
			calling = true;
		}
		public static async Task StartContinueAutoCall(string playlist){
			if(AutoCallCounter >= AutoCallList.Count){
				SetupNotAutoCalling (playlist);
				Debug.WriteLine ("Autocalling done. AutoCallCounter: {0}, AutoCallListLength: {1}", 
					AutoCallCounter, AutoCallList.Count);
				NavigationHelper.ClearModals (App.CapPage);
				MessagingCenter.Send ("", Values.READYFOREXTRATIPS);
			}
			if (AutoCallContinue && (AutoCallCounter < AutoCallList.Count)) {
				var contactToCall = AutoCallList.ElementAt (AutoCallCounter);
				Debug.WriteLine ("ITERATION " + AutoCallCounter + " IN AUTOCALL, " + AutoCallList.Count + " Numbers in list");
				AutoCallContinue = false;
				Debug.WriteLine ("CONTINUE SET TO FALSE, WAITING FOR DONEWITHCALL MESSAGE");

				//if next call not set and not yet appointed, then call
				if (!contactToCall.IsAppointed && (!contactToCall.IsSetForNextCall || contactToCall.ShouldCallToday)) { 
					Debug.WriteLine ("{0} hasnt been appointed and hasn't been marked for a follow up call", contactToCall.Name);
					await CallHelper.call (contactToCall, true);
					AutoCallCounter++;	
					Debug.WriteLine ("AutoCallCounter after call is " + AutoCallCounter);
				} else {
					if (contactToCall.IsAppointed) {
						await App.CapPage.DisplayAlert ("Skipping...", string.Format ("{0} was already appointed for {1}",
							contactToCall.FirstName, contactToCall.Appointed.ToString ("MMMM dd, yyyy")), "OK");
					} else if (contactToCall.IsSetForNextCall) {
						await App.CapPage.DisplayAlert ("Skipping...", string.Format ("{0} is already scheduled for call on {1}",
							contactToCall.FirstName, contactToCall.NextCall.ToString ("MMMM dd, yyyy")), "OK");
					}
					AutoCallCounter++;	
					Debug.WriteLine ("AutoCallCounter after call is " + AutoCallCounter);
					MessagingCenter.Send ("", Values.DONEWITHCALL);
					Debug.WriteLine ("Sent DONEWITHCALL");
				}
			} else {
				Debug.WriteLine ("AutoCallCounter is {0}, done with list", AutoCallCounter);
			}
			Debug.WriteLine ("EXITING WHILE CONTINUE");
		}

		public static bool IsTimerRunning(){
			if (StartTime.Days == 0 && StartTime.Hours == 0 && StartTime.Minutes == 0 && StartTime.Seconds == 0 && StartTime.Milliseconds == 0) {
				return false;
			} else {
				return true;
			}
		}

		public static void StartTimer(){
			StartTime = new TimeSpan (0);
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
			if (!CallHelper.IsTimerRunning ()){
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

				await CallContact (contact, autocall);
			} else {
				await CallContact (contact, autocall);
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

