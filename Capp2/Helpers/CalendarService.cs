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

    public /* static*/ class CalendarService
    {
        public  /*static*/ Calendar PrimaryCalendar { get; set; }
        /*static*/ bool CalendarExists { get; set; }
		/*static*/ bool alerted = false;

		int peopleCounter;
		ContactData[] peopleToday, peopleTomorrow;
		bool confirmingToday;

		readonly int PauseBetweenConfirm = 3000;

		public CalendarService(){
			MessagingCenter.Subscribe<string>(this, Values.DONEWITHCONFIRMTEXT, async (args) =>{ 
				await SendConfirm(confirmingToday);
			});

			MessagingCenter.Subscribe<string>(this, Values.DONECONFIRMINGTODAYSMEETINGS, async (args) =>{ 
				Debug.WriteLine ("Checking tomorrow's calendar to confirm meetings");
				await CheckTomorrow (alerted);
			});
		}
		async Task SendConfirm(bool confirmingtoday = true){
			int totalPeople;
			if (confirmingtoday) {
				totalPeople = peopleToday.Length;
				Debug.WriteLine ("SendConfirm: todays meetings");
				if (peopleCounter < totalPeople) {
					Debug.WriteLine ("before sending text: peopleCounter {0}, unconfirmed peopleToday {1}", peopleCounter, totalPeople);
					await Task.Delay (PauseBetweenConfirm);
					//UserDialogs.Instance.HideLoading();
					await TextTemplateHelper.PrepareConfirmTodaysMeetingsTemplateThenSendText (peopleToday [peopleCounter]);
					//UserDialogs.Instance.ShowLoading("Confirming appointments...");
					peopleCounter++;
					Debug.WriteLine ("after sending text: peopleCounter {0}, peopleToday {1}", peopleCounter, totalPeople);
				} else if (peopleCounter >= totalPeople) {
					//UserDialogs.Instance.HideLoading();
					Debug.WriteLine ("peopleCounter {0} > unconfirmed totalPeopleToday {1}", peopleCounter, totalPeople);
					MessagingCenter.Send ("", Values.DONECONFIRMINGTODAYSMEETINGS);
					peopleCounter = 0;
					Debug.WriteLine ("DONECONFIRMINGTODAYSMEETINGS sent, peopleCounter is {0}", peopleCounter);
				}
			} else {
				totalPeople = peopleTomorrow.Length;
				Debug.WriteLine ("SendConfirm: tomorrows meetings");
				if (peopleCounter < totalPeople)
				{
					Debug.WriteLine("before sending text: peopleCounter {0}, unconfirmed peopleTomorrow {1}", peopleCounter, totalPeople);
					await Task.Delay(PauseBetweenConfirm);
					//UserDialogs.Instance.HideLoading();
					await TextTemplateHelper.PrepareConfirmTomorrowsMeetingsTemplateThenSendText(peopleTomorrow[peopleCounter]);
					//UserDialogs.Instance.ShowLoading("Confirming appointments...");
					peopleCounter++;
					Debug.WriteLine("after sending text: peopleCounter {0}, unconfirmed peopleTomorrow {1}", peopleCounter, totalPeople);
				}
				else {
					//UserDialogs.Instance.HideLoading();
				}
			}
		}
		 async Task<bool> CheckToday(bool alerted){
			confirmingToday = true;
			try
			{	
				peopleToday = PeopleForToday().Where(person => person.IsConfirmedToday == false).ToArray();
				if (peopleToday != null && peopleToday.Length != 0)
				{
					Debug.WriteLine("Going thorugh Todays meetings");

					peopleCounter = 0;
					await SendConfirm(confirmingToday);
				}else{
					await CheckTomorrow(alerted);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("CheckToday error: " + e.Message);
				//try {UserDialogs.Instance.HideLoading(); } catch { }
			}

			return alerted;
		}

		 async Task<bool> CheckTomorrow(bool alerted){
			confirmingToday = false;
			try
			{
				peopleTomorrow = PeopleForTomorrow().Where(person => person.IsConfirmedToday == false).ToArray();
				if (peopleTomorrow != null && peopleTomorrow.Length != 0)
				{
					Debug.WriteLine("Going thorugh tomorrows meetings: {0}", peopleTomorrow.Length);
					peopleCounter = 0;
					await SendConfirm(confirmingToday);

				}else{
					Debug.WriteLine("No meetings tomorrow");
					//UserDialogs.Instance.HideLoading();
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("CheckTomorrow error: " + e.Message);
				//try { UserDialogs.Instance.HideLoading(); } catch { }
			}
			return alerted;
		}

		public  /*static*/ async Task CheckMeetingsTodayTomorrowConfirmSentSendIfNot()
        {
			//Debug.WriteLine ("first run? {0}", Settings.IsFirstRunSettings);
			if (!Settings.IsFirstRunSettings) {
				//UserDialogs.Instance.ShowLoading ("Checking calendar to confirm meetings");
				alerted = await CheckToday(alerted);
			}
        }

        public  /*static*/ void NotifyUserForTomorrowsAppointments(int hour, int seconds/*, string number = "", string message = "", string name = ""*/)
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
        public  /*static*/ List<ContactData> PeopleForTomorrow()
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
        public  /*static*/ List<ContactData> PeopleForToday()
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
        public  /*static*/ IList<CalendarEvent> GetAppointmentsTomorrow()
        {//use for (1)auto text confirming appointments, (2) for showing appointment notif reminders for user
            if (CalendarExists)
            {
                return CrossCalendars.Current.GetEventsAsync(PrimaryCalendar, DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(23)).Result;
            }
            return null;
        }
        public  /*static*/ async Task<bool> InitCalendar()
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
        public  /*static*/ IList<Calendar> GetCalendars()
        {
            return CrossCalendars.Current.GetCalendarsAsync().Result;
        }
        public  /*static*/ IList<CalendarEvent> GetAppointments()
        {
            if (CalendarExists)
            {
                return CrossCalendars.Current.GetEventsAsync(PrimaryCalendar, DateTime.Now, DateTime.MaxValue).Result;
            }
            return null;
        }
        public  /*static*/ async Task<CalendarEvent> GetAppointmentByID(string ID)
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
        public  /*static*/ async Task<bool> ReschedAppointment(string ID, string name, string description, DateTime startDate)
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
        public  /*static*/ async Task<bool> CancelAppointment(string ID)
        {
            if (CalendarExists)
            {

                await CrossCalendars.Current.DeleteEventAsync(PrimaryCalendar, await GetAppointmentByID(ID));
                return true;
            }
            return false;
        }
        public async Task<string> CreateAppointment(string ID, string eventName, string description, DateTime startDate)
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
            } 
            return null;
        }

		public async Task<string> CreateReschedAppointment(ContactData contact, string description, DateTime startDate)
		{
			if (CalendarExists)
			{
				CalendarEvent ce;
				try
				{
					if (await ReschedAppointment(contact.NextMeetingID, startDate))
					{
						contact.IsConfirmedToday = false;
						contact.IsConfirmedTomorrow = false;
						Debug.WriteLine("Rescheduled: {0} confirmedtoday {1} confirmedtomorrow {2}", 
						                contact.NextMeetingID, contact.IsConfirmedToday, contact.IsConfirmedTomorrow);
						return contact.NextMeetingID;//return NextMeetingID if a previous meeting event was found
					}
					else {
						Debug.WriteLine("No previous meeting found, creating one");

						ce = new CalendarEvent
						{
							Name = contact.Name,
							Description = description,
							Start = startDate,
							End = startDate.AddHours(Values.MEETINGLENGTH)
						};
						await CrossCalendars.Current.AddOrUpdateEventAsync(PrimaryCalendar, ce);
						Debug.WriteLine("ADDED CALENDAREVENT " + ce.ExternalID);
						return ce.ExternalID;//return a new one if no previous meeting yet
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine("Creating event error: {0}", e.Message);
				}
			}
			return null;
		}

        public  /*static*/ async Task<bool> ReschedAppointment(string ID, DateTime startDate)
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