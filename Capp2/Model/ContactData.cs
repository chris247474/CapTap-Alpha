using System;
using SQLite;
using Plugin.Calendars.Abstractions;
using System.Diagnostics;
using System.ComponentModel;
using Xamarin.Forms;
using System.Runtime.CompilerServices;

namespace Capp2
{
	[Table ("Contacts")]
	public class ContactData
	{
		[PrimaryKey, AutoIncrement, Column("ID"), NotNull]
		public int ID { get; set; }

		[Column("Name"), NotNull]
		public string Name { get; set;}

		[Column("FirstName"), NotNull]
		public string FirstName { get; set; }

		[Column("LastName"), NotNull]
		public string LastName { get; set;}

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

		[Column("OldPlaylist")]
		public string OldPlaylist { get; set; } = Values.TODAYSCALLSUNDEFINED;

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
		public string PicStringBase64 { get; set;} = UIBuilder.ChooseRandomProfilePicBackground(App.ProfileBackground);//"profile-blue.png";

		[Column("LargePic")]
		public string LargePic{ get; set;} = UIBuilder.ChooseRandomProfilePicBackground(App.ProfileBackground);

		[Column("IsConfirmedTomorrow")]
		public bool IsConfirmedTomorrow{ get; set;} = false;

		[Column("IsConfirmedToday")]
		public bool IsConfirmedToday{ get; set;} = false;

		[Column("Initials")]
		public string Initials{ get; set;}

		public bool IsAppointed{
			get{ return (Appointed.Date == DateTime.MinValue) ? false : true; }
		}

		public bool IsSetForNextCall{
			get{ return (NextCall.Date == DateTime.MinValue) ? false : true; }
		}

		public bool ShouldCallToday{
			get{ return (NextCall.Date == DateTime.Today.Date) ? true : false; }
		}

		bool _usesDefaultImage;
		public bool HasDefaultImage_Large{
			get{ 
				_usesDefaultImage = false;
				/*for (int c = 0; c < App.ProfileBackground.Length; c++) {
					if (string.Equals (LargePic, App.ProfileBackground [c])) {
						Debug.WriteLine ("LargePic: {0}, placeholder {1}: {2}", LargePic, c, App.ProfileBackground[c]);
						_usesDefaultImage = true;
					}
				}*/
				if(LargePic.Contains("profile-")){
					_usesDefaultImage = true;
				}
				return _usesDefaultImage;
			}
		}

		public bool HasDefaultImage_Small{
			get{ 
				_usesDefaultImage = false;
				/*for (int c = 0; c < App.ProfileBackground.Length; c++) {
					if (string.Equals (PicStringBase64, App.ProfileBackground [c])) {
						Debug.WriteLine ("SmallPic: {0}, placeholder {1}: {2}", PicStringBase64, c, App.ProfileBackground[c]);
						_usesDefaultImage = true;
					}
				}*/
				if(PicStringBase64.Contains("profile-")){
					_usesDefaultImage = true;
				}
				return _usesDefaultImage;
			}
		}
	}
}

