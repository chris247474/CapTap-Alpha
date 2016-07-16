using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Capp2.Helpers;

namespace Capp2
{
	public static class StartupHelper
	{
		public static void SetInstallDateForStatsPageReference()
		{
			if (Settings.InstallDateSettings == DateTime.MinValue)
			{
				Settings.InstallDateSettings = DateTime.Today.Date;
				Debug.WriteLine("Install Date: {0}", Settings.InstallDateSettings);
			}
			else {
				Debug.WriteLine("Install Date already set");
			}
		}
		public static async Task CheckForMeetingsTodayTomorrowThenSendSMSToConfirm()
		{
			try
			{
				//returns true, then device calendar has at least one calendar account
				if (await Util.DeviceCalendarExistsAndInit())
				{
					await Task.Delay(500);
					Debug.WriteLine("checking meetings today");
					App.CalendarHelper.CheckMeetingsTodayTomorrowConfirmSentSendIfNot();
				}
			}
			catch (Exception e) { Debug.WriteLine("Calendar error {0}", e.Message); }
			Settings.IsFirstRunSettings = false;
		}
		public static void SetAsPremium(bool isPremium)
		{
			//if not premium, ads will be shown
			Settings.IsPremiumSettings = isPremium;
		}

		static bool TimeToAskForEmail() {
			var timetoask = (Settings.AskAgainSettings && !Settings.IsFirstRunSettings &&
					DateTime.Today.Date >= Settings.InstallDateSettings.Date.AddDays(
				                 Values.XDAYSTOWAITAFTERINSTALLINGTOASKFOREMAIL) &&
							 Settings.InstallDateSettings.Date > DateTime.MinValue.Date);
			Debug.WriteLine("Time To ask for email: {0}", timetoask);
			return timetoask;
		}

		public static async Task StoreUserEmail()
		{
			Debug.WriteLine("AskAgain: {0}, FirstRun: {1}", Settings.AskAgainSettings, Settings.IsFirstRunSettings);
			if (TimeToAskForEmail())
			{
				var itsgreat = await UserDialogs.Instance.ConfirmAsync(
																   "Would you like to know more about apps that can double your productivity?",
																	string.Format("Liking {0} so far?", Values.APPNAME),
																   "Take a look", "not now");
				if (itsgreat)
				{
					var email = await Util.GetUserInputSingleLinePromptDialogueWithoutCancel("Let's keep in touch!", "Great!",
																							 "Please enter your email",
																							  InputType.Email);
					if (string.IsNullOrWhiteSpace(email))
					{
						email = (await UserDialogs.Instance.PromptAsync("Sorry, didn't seem to get that", "Blank text", "OK", null
																		, "Please enter your email", InputType.Email)).Text;
						if (string.IsNullOrWhiteSpace(email))
						{
							Settings.AskAgainSettings = true;
						}
					}
					else {
						Settings.EmailSettings = email;
						Settings.AskAgainSettings = false;
					}
				}
				else {
					var askagain = await UserDialogs.Instance.ActionSheetAsync("Not a good time?", null, null, new string[] {Values.NEXTTIME,
																												Values.DONTREMIND});
					Settings.AskAgainSettings = string.Equals(askagain, Values.NEXTTIME) ? true : false;
				}
			}
		}
	}
}

