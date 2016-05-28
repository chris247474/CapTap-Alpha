using System;
using SQLite;
using Plugin.Calendars.Abstractions;
using System.Diagnostics;
using System.ComponentModel;
using Xamarin.Forms;

namespace Capp2
{
	[Table ("Contacts")]
	public class ContactData
	{
		[PrimaryKey, AutoIncrement, Column("ID"), NotNull]
		public int ID { get; set; }

		[Column("Name"), NotNull]
		public string Name { get; set; }

		[Column("FirstName"), NotNull]
		public string FirstName { get; set; }

		[Column("LastName"), NotNull]
		public string LastName { get; set; }

		[Column("Affiliation")]
		public string Aff { get; set; }

		[Column("Number"), NotNull]
		public string Number { get; set; }

		[Column("Number2")]
		public string Number2 { get; set; }

		[Column("Number3")]
		public string Number3 { get; set; }

		[Column("Number4")]
		public string Number4 { get; set; }

		[Column("Number5")]
		public string Number5 { get; set; }

		[Column("Playlist")]
		public string Playlist { get; set; }

		[Column("Called")]
		public DateTime Called{ get; set;}
		[Column("Appointed")]
		public DateTime Appointed{ get; set;}
		[Column("Presented")]
		public DateTime Presented{ get; set;}
		[Column("Purchased")]
		public DateTime Purchased{ get; set;}

		[Column("NextMeetingID")]
		public string NextMeetingID{ get; set;}

		[Column("NextCall")]
		public DateTime NextCall{ get; set;}

		[Column("IsSelected")]
		public bool IsSelected{ get; set;}

		[Column("AzureID")]
		public string AzureID{ get; set;}

		[Column("PicStringBase64")]
		public string PicStringBase64{ get; set;} = "placeholder-contact-male.png";

		[Column("LargePic")]
		public string LargePic{ get; set;} = "placeholder-contact-male.png";

		[Column("IsConfirmedTomorrow")]
		public bool IsConfirmedTomorrow{ get; set;} = false;

		[Column("IsConfirmedToday")]
		public bool IsConfirmedToday{ get; set;} = false;
	}
}

