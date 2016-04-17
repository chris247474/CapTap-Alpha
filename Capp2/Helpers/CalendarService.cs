using System;
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

        public static async void CheckIfMeetingsTomorrowConfirmSentSendIfNot(bool showMessages)
        {
            SettingsViewModel vm;
            vm = new SettingsViewModel();
            List<ContactData> peopleTomorrow = PeopleForTomorrow();

            if (!string.Equals(DateTime.Today.Day.ToString(), vm.DateRemindedSettings))
            {
                if (showMessages) UserDialogs.Instance.InfoToast("Tomorrows appointments not yet confirmed", "Tomorrows appointments not yet confirmed", 4000);
                try
                {
                    if (peopleTomorrow != null && peopleTomorrow.Count != 0)
                    {
                        if (showMessages) UserDialogs.Instance.InfoToast("Going thorugh tomorrows meetings", "", 4000);
                        for (int c = 0; c < peopleTomorrow.Count; c++)
                        {
                            var person = peopleTomorrow.ElementAt(c);
                            var meeting = await GetAppointmentByID(person.NextMeetingID);
                            DependencyService.Get<IPhoneContacts>().SendSMS(person.Number,
                                App.contactFuncs.ConnectStrings(new string[] {
                                    "Hi",
                                    person.Name,
                                    vm.MeetingConfirmSettings,
                                    meeting.Start.Hour.ToString (),
                                    ":",
                                    meeting.Start.Minute.ToString ()
                                }), person.Name, Values.CONFIRM, Values.TOMORROW);
                        }
                        vm.DateRemindedSettings = DateTime.Today.Day.ToString();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("" + e.Message);
                }

            }
            else {
                if (showMessages) UserDialogs.Instance.WarnToast("DateRemidned Settings already checked", "DateRemidned Settings already checked", 4000);
            }
        }
        public static async void CheckIfMeetingsTodayConfirmSentSendIfNot(bool showMessages)
        {//testing
            SettingsViewModel vm;
            vm = new SettingsViewModel();
            List<ContactData> peopleToday = PeopleForToday();

            if (!string.Equals(DateTime.Today.Day.ToString(), vm.DateRemindedForTodaySettings/*make new settings field*/))
            {
                if (showMessages) UserDialogs.Instance.InfoToast("Todays appointments not yet confirmed", "Todays appointments not yet confirmed", 4000);
                try
                {
                    if (peopleToday != null && peopleToday.Count != 0)
                    {
                        if (showMessages) UserDialogs.Instance.InfoToast("Going thorugh Todays meetings", "", 4000);
                        for (int c = 0; c < peopleToday.Count; c++)
                        {
                            var person = peopleToday.ElementAt(c);
                            var meeting = await GetAppointmentByID(person.NextMeetingID);
                            DependencyService.Get<IPhoneContacts>().SendSMS(person.Number,
                                App.contactFuncs.ConnectStrings(new string[] {
                                    "Hi",
                                    person.Name,
                                    vm.MeetingTodayConfirmSettings,//make new settings field
									meeting.Start.Hour.ToString (),
                                    ":",
                                    meeting.Start.Minute.ToString ()
                                }), person.Name, Values.CONFIRM, Values.TODAY);
                        }
                        vm.DateRemindedForTodaySettings = DateTime.Today.Day.ToString();//make new settings field
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("" + e.Message);
                }

            }
            else {
                if (showMessages) UserDialogs.Instance.WarnToast("DateRemidnedForToday Settings already checked", "DateRemidnedForToday Settings already checked", 4000);
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
                    ce = await GetAppointmentByID(ID);
                }
                catch (Exception)
                {
                    ce = new CalendarEvent
                    {
                        Name = eventName,
                        Description = description,
                        Start = startDate,
                        End = startDate.AddHours(Values.MEETINGLENGTH)
                    };
                }

                try
                {
                    await CrossCalendars.Current.AddOrUpdateEventAsync(PrimaryCalendar, ce);
                    Debug.WriteLine("ADDED CALENDAREVENT " + ce.ExternalID);
                    return ce.ExternalID;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[CreateAppointment()] " + ex.Message);
                }
                return null;
            }
            return null;
        }
        public static async Task<bool> ReschedAppointment(string ID, DateTime startDate)
        {
            if (CalendarExists)
            {
                Debug.WriteLine("STRING ID " + ID);
                Debug.WriteLine("ABOUT TO GETAPPOINTMENT");
                CalendarEvent ce = await GetAppointmentByID(ID);
                Debug.WriteLine("GOT APPOINTMENT");

                ce.Start = startDate;
                ce.End = startDate.AddHours(Values.MEETINGLENGTH);
                await CrossCalendars.Current.AddOrUpdateEventAsync(PrimaryCalendar, ce);
                return true;
            }
            return false;
        }
    }


}




