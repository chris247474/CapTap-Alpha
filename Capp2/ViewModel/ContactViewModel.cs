using System;
using System.Collections.Generic;
using Plugin.Contacts.Abstractions;
using System.Linq;
using System.Diagnostics;
using Plugin.Calendars.Abstractions;
using System.ComponentModel;

namespace Capp2
{
	public class ContactViewModel:BaseViewModel
	{
		ContactData contact;

		public ContactViewModel(ContactData contact){
			this.contact = contact;
		}

		public List<string> Numbers { get; set; }

		public List<string> Playlists { get; set; }

		public bool IsAppointed{
			get{ return (contact.Appointed.Date == DateTime.MinValue) ? false : true; }
		}

		public bool IsSetForNextCall{
			get{ return (contact.NextCall.Date == DateTime.MinValue) ? false : true; }
		}

		public bool ShouldCallToday{
			get{ return (contact.NextCall.Date == DateTime.Today.Date) ? true : false; }
		}
	}
}

