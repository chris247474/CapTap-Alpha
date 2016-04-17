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


namespace Capp2
{
	public class App : Application
	{
		public static DB database;
		public static PlaylistDB playlists;
		public static Util contactFuncs;
		public static bool firstRun = false;
		public static int lastIndex{ get; set;}
		public static bool IsEditing{ get; set;}
		public static NavigationPage NavPage;

		public App ()
		{			
			PrepareAppData ();
			MainPage = new StartPage();
		}

		public async void PrepareAppData(){
			IsEditing = false;
			contactFuncs = new Util ();

			await contactFuncs.loadDeviceContactsIntoDBSingleTransaction (false);

			//returns true, then device calendar has at least one calendar account
			if (await App.contactFuncs.DeviceCalendarExistsAndInit()) {
				DependencyService.Get<ICalendar>().CheckIfMeetingsTomorrowConfirmSentSendIfNot(false);//notifications replace each other, instead of stacking in KitKat API 19
				DependencyService.Get<ICalendar>().CheckIfMeetingsTodayConfirmSentSendIfNot(true);
			}
		}
		public static DB Database {
			get { 
				if (database == null) {
					database = new DB ();
				}
				return database; 
			}
		}
		public static PlaylistDB Playlists {
			get { 
				if (playlists == null) {
					playlists = new PlaylistDB ();
				}
				return playlists; 
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
