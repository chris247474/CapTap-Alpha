﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Calendars;
using Plugin.Calendars.Abstractions;
using Acr.UserDialogs;
using Xamarin.Forms;
using System.Diagnostics;

namespace Capp2.Helpers
{

    public static class CalendarService
    {
        public static Calendar PrimaryCalendar { get; set; }
        static bool CalendarExists { get; set; }

		static async Task<bool> CheckToday(bool alerted){
			UserDialogs.Instance.HideLoading ();

			var peopleToday = PeopleForToday().ToArray();
				try
				{	
					if (peopleToday != null && peopleToday.Length != 0)
					{
						Debug.WriteLine("Going thorugh Todays meetings");
						UserDialogs.Instance.HideLoading ();
						for (int c = 0; c < peopleToday.Length; c++)
						{
							var person = peopleToday[c];
							await Task.Delay(1000);
							await TextTemplateHelper.PrepareConfirmTodaysMeetingsTemplateThenSendText(person);
						}
					}
					
				}
				catch (Exception e)
				{
					Debug.WriteLine("" + e.Message);
					
				}

			return alerted;
		}

		static async Task<bool> CheckTomorrow(bool alerted){
			UserDialogs.Instance.HideLoading ();
			var peopleTomorrow = PeopleForTomorrow().ToArray();
				try
				{
					//
					if (peopleTomorrow != null && peopleTomorrow.Length != 0)
					{
							
						Debug.WriteLine("Going thorugh tomorrows meetings: {0}", peopleTomorrow.Length);
						for (int c = 0; c < peopleTomorrow.Length; c++)
						{
							var person = peopleTomorrow[c];
							await Task.Delay(1000);
							await TextTemplateHelper.PrepareConfirmTomorrowsMeetingsTemplateThenSendText(person);
						}

					}else{
						Debug.WriteLine("No meetings tomorrow");
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine("" + e.Message);
				}
			return alerted;
		}

		public static async Task CheckMeetingsTodayTomorrowConfirmSentSendIfNot()
        {
			bool alerted = false;
			Debug.WriteLine ("first run? {0}", Settings.IsFirstRunSettings);
			if (!Settings.IsFirstRunSettings) {
				UserDialogs.Instance.ShowLoading ("Checking today's calendar to confirm meetings");
				alerted = await CheckToday(alerted);
				await Task.Delay (3000);
				UserDialogs.Instance.ShowLoading ("Checking tomorrow's calendar to confirm meetings");
				await CheckTomorrow (alerted);
			}
        }

        public static void NotifyUserForTomorrowsAppointments(int hour, int seconds/*, string number = "", string message = "", string name = ""*/)
        {
            var notifier = DependencyService.Get<IReminderService>();
            string people = "";

            List<ContactData> peopleTomorrow = PeopleForTomorrow();

            if (peopleTomorrow != null && peopleTomorrow.Count != 0)
            {
                Debug.WriteLine("peopleTomorrow Count: " + peopleTomorrow.Count);
                foreach (ContactData c in peopleTomorrow)
                {
                    people += c.Name + "\n";
                }

                Debug.WriteLine("DateTime.Now.Hour: {0}, DateTime.Today.Hour: {1}", DateTime.Now.Hour, DateTime.Today.Hour);
                if (DateTime.Now.Hour < hour)
                {
                    notifier.Remind(DateTime.Today.AddHours(hour), "Dont forget tomorrow's meetings", people/*, number, name*/);
                }
                else {
                    notifier.Remind(DateTime.Now.AddMilliseconds(5000), "Dont forget tomorrow's meetings", people/*, name*/);
                }

            }
            else {
                notifier.Remind(DateTime.Now.AddMilliseconds(5000), "Message from Daniel Laogan", "Why don't you have meetings tomorrow????");
            }

        }
        public static List<ContactData> PeopleForTomorrow()
        {
            if (CalendarExists)
            {
                IList<CalendarEvent> meetingsTomorrow = CrossCalendars.Current.GetEventsAsync(PrimaryCalendar, DateTime.Today.AddDays(1), DateTime.Today.AddDays(1).AddHours(23)).Result;
                List<ContactData> peopleTomorrow = new List<ContactData>();

                if (meetingsTomorrow == null || meetingsTomorrow.Count == 0)
                {
                    return null;
                }
                else {
                    foreach (CalendarEvent e in meetingsTomorrow)
                    {
                        foreach (ContactData c in App.Database.GetItems(Values.ALLPLAYLISTPARAM))
                        {
                            if (e.ExternalID == c.NextMeetingID)
                            {
                                peopleTomorrow.Add(c);
                            }
                        }
                    }
                }

                return peopleTomorrow;
            }
            return null;
        }
        public static List<ContactData> PeopleForToday()
        {
            if (CalendarExists)
            {
                IList<CalendarEvent> meetingsTomorrow = CrossCalendars.Current.GetEventsAsync(PrimaryCalendar, DateTime.Today.AddDays(0), DateTime.Today.AddDays(0).AddHours(23)).Result;
                List<ContactData> peopleTomorrow = new List<ContactData>();

                if (meetingsTomorrow == null || meetingsTomorrow.Count == 0)
                {
                    return null;
                }
                else {
                    foreach (CalendarEvent e in meetingsTomorrow)
                    {
                        foreach (ContactData c in App.Database.GetItems(Values.ALLPLAYLISTPARAM))
                        {
                            if (e.ExternalID == c.NextMeetingID)
                            {
                                peopleTomorrow.Add(c);
                            }
                        }
                    }
                }

                return peopleTomorrow;
            }
            return null;
        }
        public static IList<CalendarEvent> GetAppointmentsTomorrow()
        {//use for (1)auto text confirming appointments, (2) for showing appointment notif reminders for user
            if (CalendarExists)
            {
                return CrossCalendars.Current.GetEventsAsync(PrimaryCalendar, DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(23)).Result;
            }
            return null;
        }
        public static async Task<bool> InitCalendar()
        {
            Debug.WriteLine("ENTERED INITCALENDAR");

            IList<Calendar> calendars = GetCalendars();
            Debug.WriteLine("CALENDAR NAME" + calendars.ElementAt(0).Name);
            try {
                Debug.WriteLine("CALENDAR NAME 2" + calendars.ElementAt(1).Name);
                Debug.WriteLine("CALENDAR NAME 3" + calendars.ElementAt(2).Name);
                Debug.WriteLine("CALENDAR NAME 4" + calendars.ElementAt(3).Name);
            } catch (Exception) {
                Debug.WriteLine("No secondary calendar");
            }
            Calendar[] calArray = new Calendar[calendars.Count];
            calendars.CopyTo(calArray, 0);
            PrimaryCalendar = calArray[0];
            CalendarExists = true;

            return true;
        }
        public static IList<Calendar> GetCalendars()
        {
            return CrossCalendars.Current.GetCalendarsAsync().Result;
        }
        public static IList<CalendarEvent> GetAppointments()
        {
            if (CalendarExists)
            {
                return CrossCalendars.Current.GetEventsAsync(PrimaryCalendar, DateTime.Now, DateTime.MaxValue).Result;
            }
            return null;
        }
        public static async Task<CalendarEvent> GetAppointmentByID(string ID)
        {
            if (CalendarExists)
            {
                CalendarEvent ce = await CrossCalendars.Current.GetEventByIdAsync(ID);
                if (ce == null)
                {
                    throw new KeyNotFoundException("No matching CalendarEvent for ExternalID: " + ID);
                }
                return ce;
            }
            return null;
        }
        public static async Task<bool> ReschedAppointment(string ID, string name, string description, DateTime startDate)
        {
            if (CalendarExists)
            {
                CalendarEvent ce = await GetAppointmentByID(ID);
                ce.Name = name;
                ce.Description = description;
                ce.Start = startDate;
                ce.End = startDate.AddHours(Values.MEETINGLENGTH);
                await CrossCalendars.Current.AddOrUpdateEventAsync(PrimaryCalendar, ce);
                return true;
            }
            return false;
        }
        public static async Task<bool> CancelAppointment(string ID)
        {
            if (CalendarExists)
            {

                await CrossCalendars.Current.DeleteEventAsync(PrimaryCalendar, await GetAppointmentByID(ID));
                return true;
            }
            return false;
        }
        public static async Task<string> CreateAppointment(string ID, string eventName, string description, DateTime startDate)
        {
            if (CalendarExists)
            {
				CalendarEvent ce;

                try
                {
					if(await ReschedAppointment (ID, startDate)){
						Debug.WriteLine("Rescheduled: {0}", ID);
						return ID;//return NextMeetingID if a previous meeting event was found
					}else{
						Debug.WriteLine("No previous meeting found, creating one");

						ce = new CalendarEvent {
							Name = eventName,
							Description = description,
							Start = startDate,
							End = startDate.AddHours (Values.MEETINGLENGTH)
						};
						await CrossCalendars.Current.AddOrUpdateEventAsync(PrimaryCalendar, ce);
						Debug.WriteLine("ADDED CALENDAREVENT " + ce.ExternalID);
						return ce.ExternalID;//return a new one if no previous meeting yet

					}
                }
                catch (Exception e)
                {
					Debug.WriteLine ("Creating event error: {0}", e.Message);
                }

                /*try  
                { 
                    await CrossCalendars.Current.AddOrUpdateEventAsync(PrimaryCalendar, ce);
                    Debug.WriteLine("ADDED CALENDAREVENT " + ce.ExternalID);
                    return ce.ExternalID;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[CreateAppointment()] " + ex.Message);
                }
                return null;*/
            } 
            return null;
        }
        public static async Task<bool> ReschedAppointment(string ID, DateTime startDate)
        {
			try{
				if (CalendarExists)
				{
					Debug.WriteLine("STRING ID " + ID);
					Debug.WriteLine("ABOUT TO GETAPPOINTMENT");
					CalendarEvent ce = await GetAppointmentByID(ID);
					Debug.WriteLine("GOT APPOINTMENT");

					ce.Start = startDate;
					ce.End = startDate.AddHours(Values.MEETINGLENGTH);
					await CrossCalendars.Current.AddOrUpdateEventAsync(PrimaryCalendar, ce);
					UserDialogs.Instance.ShowSuccess("I rescheduled your appointment in your calendar!");

					return true;
				}
			}catch(Exception e){
				Debug.WriteLine ("ReschedAppointment error: {0}", e.Message);
			}
            return false;
        }
    }


}




//var meeting = await GetAppointmentByID(person.NextMeetingID);
/*await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number,
                                App.contactFuncs.ConnectStrings(new string[] {
                                    "Hi",
                                    person.Name,
                                    vm.MeetingConfirmSettings,
                                    meeting.Start.Hour.ToString (),
                                    ":",
                                    meeting.Start.Minute.ToString ()
                                }), person.Name, Values.CONFIRM, Values.TOMORROW);*/