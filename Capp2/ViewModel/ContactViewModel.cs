using System;
using System.Collections.Generic;
using Plugin.Contacts.Abstractions;
using System.Linq;
using System.Diagnostics;
using Plugin.Calendars.Abstractions;
using System.ComponentModel;

namespace Capp2
{
	public class ContactViewModel
	{
		public String Playlist{ get; set;}
		public CAPPDates CAPP{ get; set;}
		public Contact Contact{ get; set;}
		/*public CalendarEvent NextMeeting{ 
			get{
				if (NextMeeting == null) {
					Debug.WriteLine ("[ContactData] NextMeeting field is null, initializing default values");
					return new CalendarEvent {
						Name = "Appointment",
						Description = "Meeting",
						Start = DateTime.Now,
						End = DateTime.Now.AddHours (Values.MEETINGLENGTH)
					};
				}
				return NextMeeting;
			}set{}
		}

		public CalendarEvent GetNextMeeting(){
			return NextMeeting;
		}*/

		public ContactViewModel (string fname, string lname, IQueryable<Plugin.Contacts.Abstractions.Phone> phone, string aff, string playlist)
		{
			//required minimim info
			this.Contact = new Contact{FirstName = fname, LastName = lname, Phones = phone.ToList()};
			this.Playlist = playlist;
		}
		public ContactViewModel (string fname, string lname, string number, string aff, string playlist/*, CAPPDates cappDates*/)
		{
			//required minimim info
			this.Contact = new Contact{FirstName = fname, LastName = lname};
			this.Contact.Phones = new List<Plugin.Contacts.Abstractions.Phone> ();
			this.Contact.Phones.Add (new Plugin.Contacts.Abstractions.Phone ());
			this.Contact.Phones.ElementAtOrDefault (0).Number = number;
			this.Playlist = playlist;
			//this.CAPP = cappDates;
		}
		public ContactViewModel (Contact person, string playlist)
		{
			if (string.IsNullOrEmpty (person.LastName) && person.Phones.Count < 1) {
				throw new ArgumentException ("param passed to ContactViewModel(Contact) is lacking name and phone number properties");
			} else {
				this.Contact = person;
				this.Playlist = playlist;
			}
		}
		public bool Save(){
			if (string.IsNullOrEmpty (this.Contact.LastName) || this.Contact.Phones.Count == 0) {
				throw new Exception ("ERROR SAVING CONTACT: Contact has no lastname or no phone number");
			} else {
				try{
					App.Database.SaveItem (new ContactData
						{
							FirstName = this.Contact.FirstName, 
							LastName = this.Contact.LastName,
							Number = ((Plugin.Contacts.Abstractions.Phone)this.Contact.Phones.ElementAtOrDefault(0)).Number,
							Aff = this.Contact.Organizations.ElementAtOrDefault(0).Name,//not yet tested
							Playlist = this.Playlist,
							Called = this.CAPP.Called,
							Appointed = this.CAPP.Appointed,
							Presented = this.CAPP.Presented,
							Purchased = this.CAPP.Purchased
						});
					return true;
				}catch(Exception e){
					Debug.WriteLine ("ERROR SAVING CONTACT: "+e.Message);
				}
			}
			return false;
		}
		public bool Update(){
			if (string.IsNullOrEmpty (this.Contact.LastName) || this.Contact.Phones.Count == 0) {
				throw new Exception ("ERROR UPDATING CONTACT: Contact has no lastname or no phone number");
			} else {
				try{
					App.Database.UpdateItem (new ContactData
						{
							FirstName = this.Contact.FirstName, 
							LastName = this.Contact.LastName,
							Number = ((Plugin.Contacts.Abstractions.Phone)this.Contact.Phones.ElementAtOrDefault(0)).Number,
							Aff = this.Contact.Organizations.ElementAtOrDefault(0).Name,//not yet tested
							Playlist = this.Playlist,
							Called = this.CAPP.Called,
							Appointed = this.CAPP.Appointed,
							Presented = this.CAPP.Presented,
							Purchased = this.CAPP.Purchased
						});
					return true;
				}catch(Exception e){
					Debug.WriteLine ("ERROR UPDATING CONTACT: "+e.Message);
				}
			}
			return false;
		}

	}
	public class CAPPDates{
		public DateTime Called{ get; set;}
		public DateTime Appointed{ get; set;}
		public DateTime Presented{ get; set;}
		public DateTime Purchased{ get; set;}
	}
}

