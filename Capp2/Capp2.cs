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
		public static bool firstRun = false;
		public static int lastIndex{ get; set;}
		public static bool IsEditing{ get; set;}
		public static NavigationPage NavPage;
		public static StartPage MasterDetailPage;
		public static TabbedPage StartTabbedPage;
        public static CAPP CapPage { get; set; }
		public static EditContactPage EditPage{ get; set;}
		public static Color StartColor;
		public static Color EndColor;
		public static string Width;
		public static string Height;
		public static bool /*OnAppStart,*/ InTutorialMode = false, UsingSearch = false;
		public static TimeSpan SingleCallTimeEllapsed;
		public static string DefaultNamelist, CurrentNamelist;
		public static int DeviceImageCtr{ get; set;}
		public static bool AppJustLaunched;
		public static SettingsViewModel SettingsHelper = new SettingsViewModel ();
		public static bool ImageImportingDone = false;
		public static string[] ProfileBackground;
		//public ResourceDictionary Resources = new ResourceDictionary ();
		public static ContactData CurrentContact = null;
		public static CalendarService CalendarHelper = new CalendarService();

		public static DB Database {
			get { 
				if (database == null) {
					database = new DB ();
				}
				return database; 
			}
		}

		public App ()
		{			
			PrepareAppData ();
			MainPage = //new StartPage ();
				new TabbedStartPage ();
		}

		public async Task PrepareAppData(){
			AppJustLaunched = true;
			IsEditing = false;
			DefaultNamelist = Settings.DefaultNamelistSettings;

			ProfileBackground = new string[]{ 
				"profile-orange.png",
				"profile-orange.png",
				"profile-blue.png",
				"profile-green.png",
				"profile-purpleblue.png",
			};

			StartupHelper.SetInstallDateForStatsPageReference (); 

			//SetupGradientBackground ();

			StartupHelper.SetAsPremium (true);

			await Util.loadDeviceContactsIntoDBSingleTransaction (false);

			MessagingCenter.Subscribe<string>(this, Values.DONECONFIRMINGMEETINGS, async (args) =>
			{
				Debug.WriteLine("DONECONFIRMINGMEETINGS");
				await StartupHelper.StoreUserEmail();
			});

			StartupHelper.CheckForMeetingsTodayTomorrowThenSendSMSToConfirm ();
		}

		/*void SetupGradientBackground(){
			if(Device.OS == TargetPlatform.iOS)
			{
				OnAppStart = true;
			}
			StartColor = Color.White;//FromHex (Values.GOOGLEBLUE);//BACKGROUNDPURPLEGRADIENT); 
			EndColor = Color.White;//FromHex (Values.BACKGROUNDDARKPURPLEGRADIENT);
		}*/


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
