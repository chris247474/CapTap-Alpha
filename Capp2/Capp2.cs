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
        public static bool AutoCallStatus { get; set; }
        public static CAPP CapPage { get; set; }
		public static Color StartColor;
		public static Color EndColor;
		public static string Width;
		public static string Height;
		public static bool OnAppStart;
		public static TimeSpan SingleCallTimeEllapsed;
		public static string DefaultNamelist;

		public App ()
		{			
			PrepareAppData ();
			MainPage = new StartPage();
		}

		public async void PrepareAppData(){
			IsEditing = false;
            AutoCallStatus = false;
			contactFuncs = new Util ();
			DefaultNamelist = Settings.DefaultNamelistSettings;

			SetupGradientBackground ();

			await contactFuncs.loadDeviceContactsIntoDBSingleTransaction (false);

            try {
                //returns true, then device calendar has at least one calendar account
                if (await App.contactFuncs.DeviceCalendarExistsAndInit())
                {
                    CalendarService.CheckIfMeetingsTomorrowConfirmSentSendIfNot(false);//notifications replace each other, instead of stacking in KitKat API 19
                    CalendarService.CheckIfMeetingsTodayConfirmSentSendIfNot(false);
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
