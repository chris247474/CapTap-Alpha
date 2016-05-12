using System;
using System.Collections.Generic;
using Plugin.Calendars.Abstractions;
using System.Threading.Tasks;

namespace Capp2
{
	public interface ICalendar
	{
		IList<Calendar> GetCalendars();
		IList<CalendarEvent> GetAppointments();
		Task<CalendarEvent> GetAppointmentByID(string ID);
		Task<bool> ReschedAppointment(string ID, string name, string description, DateTime startDate);
		Task<bool> CancelAppointment(string ID);
		Task<bool> ReschedAppointment(string ID, DateTime startDate);
		Task<string> CreateAppointment(string ID, string eventName, string description, DateTime startDate);
		Calendar PrimaryCalendar{ get; set;}
		Task<bool> InitCalendar();
		IList<CalendarEvent> GetAppointmentsTomorrow();
		List<ContactData> PeopleForTomorrow ();
		//void NotifyUserForTomorrowsAppointments(int day = 0, int hour = 0, int min = 0, int seconds = 0);
		void NotifyUserForTomorrowsAppointments(int hour, int seconds);
		void CheckIfMeetingsTomorrowConfirmSentSendIfNot (bool showMessages);
		List<ContactData> PeopleForToday();
		void CheckIfMeetingsTodayConfirmSentSendIfNot(bool showMessages);
	}
}

