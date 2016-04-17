using System;
using Newtonsoft.Json;

namespace Capp2
{
	public class ContactDataItemAzure
	{
		string id, name, firstname, lastname, affiliation, number, number2, number3, number4, number5, playlist, nextmeetingid;
		DateTime called, appointed, presented, purchased, nextcall;
		bool isselected;

		[JsonProperty(PropertyName = "id")]
		public string ID 
		{
			get { return id; }
			set { id = value;}
		}

		[JsonProperty(PropertyName = "Name")]
		public string Name { get{return name; } set{name = value; } }

		[JsonProperty(PropertyName = "FirstName")]
		public string FirstName { get{return firstname; } set{firstname = value; } }

		[JsonProperty(PropertyName = "LastName")]
		public string LastName { get{return lastname; } set{ lastname = value; } }

		[JsonProperty(PropertyName = "Affiliation")]
		public string Aff { get{return affiliation; } set{ affiliation = value; } }

		[JsonProperty(PropertyName = "Number")]
		public string Number { get{return number; } set{ number = value; } }

		[JsonProperty(PropertyName = "Number2")]
		public string Number2 { get{return number2; } set{ number2 = value; } }

		[JsonProperty(PropertyName = "Number3")]
		public string Number3 { get{return number3; } set{ number3 = value; } }

		[JsonProperty(PropertyName = "Number4")]
		public string Number4 {get{return number4; } set{ number4 = value; } }

		[JsonProperty(PropertyName = "Number5")]
		public string Number5 { get{return number5; } set{ number5 = value; } }

		[JsonProperty(PropertyName = "Playlist")]
		public string Playlist { get{return playlist; } set{ playlist = value; } }

		[JsonProperty(PropertyName = "Called")]
		public DateTime Called{ get{return called; } set{ called = value; }}
		[JsonProperty(PropertyName = "Appointed")]
		public DateTime Appointed{ get{return appointed; } set{ appointed = value; }}
		[JsonProperty(PropertyName = "Presented")]
		public DateTime Presented{ get{return presented; } set{ presented = value; }}
		[JsonProperty(PropertyName = "Purchased")]
		public DateTime Purchased{ get{return purchased; } set{ purchased = value; }}

		[JsonProperty(PropertyName = "NextMeetingID")]
		public string NextMeetingID{ get{return nextmeetingid; } set{ nextmeetingid = value; }}

		[JsonProperty(PropertyName = "NextCall")]
		public DateTime NextCall{ get{return nextcall; } set{ nextcall = value; }}

		[JsonProperty(PropertyName = "IsSelected")]
		public bool IsSelected{ get{return isselected; } set{ isselected = value; }}


		//[Microsoft.WindowsAzure.MobileServices.Version]
		//public string Version { get; set; }
	}
}

