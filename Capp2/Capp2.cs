using System;

using Xamarin.Forms;
using Acr.UserDialogs;
using System.Threading.Tasks;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;
using XLabs.Ioc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Plugin.Calendars.Abstractions;
using Plugin.Calendars;
using Plugin.LocalNotifications;
using Plugin.Contacts.Abstractions;
using Capp2.Helpers;
using System.Globalization;

namespace Capp2
{
	public class App : Application
	{
		public static DB database;
		public static Util contactFuncs;
		public static bool firstRun = false;
		public static int lastIndex{ get; set;}
		public static bool IsEditing{ get; set;}
		public static NavigationPage NavPage;
		public static StartPage MasterDetailPage;
        public static bool AutoCallStatus { get; set; }
        public static CAPP CapPage { get; set; }
		public static Color StartColor;
		public static Color EndColor;
		public static string Width;
		public static string Height;
		public static bool OnAppStart, InTutorialMode = false, UsingSearch = false;
		public static TimeSpan SingleCallTimeEllapsed;
		public static string DefaultNamelist;
		public static int DeviceImageCtr{ get; set;}
		public static bool AppJustLaunched;
		public static SettingsViewModel SettingsHelper = new SettingsViewModel ();
		public static bool ImageImportingDone = false;
		public static bool MadeForNuskin = false;
		public static string[] ProfileBackground;

		public App ()
		{			
			PrepareAppData ();
			MainPage = new StartPage ();
		}

		public async Task PrepareAppData(){
			AppJustLaunched = true;
			IsEditing = false;
            AutoCallStatus = false;
			contactFuncs = new Util ();
			DefaultNamelist = Settings.DefaultNamelistSettings;

			ProfileBackground = new string[]{ 
				"profile-orange.png",
				"profile-red.png",
				"profile-orange.png",
				"profile-green.png",
				"profile-orange.png",
				"profile-blue.png",
				"profile-orange.png",
				"profile-red.png",
				"profile-purple.png",
				"profile-orange.png",
				"profile-red.png",
				"profile-orange.png",
			};

			SetInstallDateForStatsPageReference (); 

			SetupGradientBackground ();

			await contactFuncs.loadDeviceContactsIntoDBSingleTransaction (false);
			CheckForMeetingsTodayTomorrowThenSendSMSToConfirm ();
		}

		public void SetInstallDateForStatsPageReference(){
			if (Settings.InstallDateSettings == DateTime.MinValue) {
				Settings.InstallDateSettings = DateTime.Today.Date;
				Debug.WriteLine ("Install Date: {0}", Settings.InstallDateSettings);
			} else {
				Debug.WriteLine ("Install Date already set");
			}
		}

		public static async Task CheckForMeetingsTodayTomorrowThenSendSMSToConfirm(){
			try {
				//returns true, then device calendar has at least one calendar account
				if (await App.contactFuncs.DeviceCalendarExistsAndInit())
				{
					await Task.Delay(10000);
					await CalendarService.CheckMeetingsTodayTomorrowConfirmSentSendIfNot();

				}
			} catch(Exception e){ Debug.WriteLine("Calendar error {0}", e.Message);}
		}
		void SetupGradientBackground(){
			if(Device.OS == TargetPlatform.iOS)
			{
				OnAppStart = true;
			}
			StartColor = Color.White;//FromHex (Values.GOOGLEBLUE);//BACKGROUNDPURPLEGRADIENT); 
			EndColor = Color.White;//FromHex (Values.BACKGROUNDDARKPURPLEGRADIENT);
		}
		public static DB Database {
			get { 
				if (database == null) {
					database = new DB ();
				}
				return database; 
			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}

}
