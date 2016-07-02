using System;
using System.Collections.Generic;
using Plugin.Contacts.Abstractions;
using System.Linq;
using System.Diagnostics;
using Plugin.Calendars.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Capp2
{
	public class ContactViewModel:BaseViewModel
	{
		ContactData contact;
		string _fname, _lname;
		//string Separator = Values.FORMATSEPARATOR;
		//public static string StaticSeparator = Values.FORMATSEPARATOR;

		public ContactViewModel(ContactData contactParam){
			contact = contactParam;
			_fname = contact.FirstName;
			_lname = contact.LastName;
		}


		public string FirstName { 
			get { 
				return contact.FirstName;
			} 
			set { 
				SetObservableProperty (ref _fname, value);
				contact.FirstName = _fname;
				contact.Name = this.FirstName + " " + contact.LastName;

				GenerateInitials ();
				App.Database.UpdateItem (contact);
			} 
		} 

		public string LastName { 
			get { 
				return contact.LastName;
			} 
			set { 
				SetObservableProperty (ref _lname, value);
				contact.LastName = _lname;
				contact.Name = this.FirstName + " " + contact.LastName;

				GenerateInitials ();
				App.Database.UpdateItem (contact);
			} 
		} 

		public void GenerateInitials(){
			var firstinitial = contact.FirstName.ToCharArray()[0];
			var secondinitial = contact.LastName.ToCharArray()[0];

			if(contact.HasDefaultImage_Small){
				Debug.WriteLine("{0} has default image");
				contact.Initials = firstinitial.ToString()+secondinitial.ToString();
			}else{
				contact.Initials = string.Empty;
			}
		}

		public string[] GetNamelists() { 
			return SeparateMultipleValuesFromContactFieldIntoArray (contact.Playlist);
		} 

		public string[] GetNumbers(){
			var numbers = SeparateMultipleValuesFromContactFieldIntoArray (contact.Number);
			/*for (int c = 0;c < numbers.Length;c++) {
				numbers[c] = App.contactFuncs.MakeDBContactCallable (numbers[c], false);
			}*/
			return numbers;
		}

		public void AddNumbers(string[] numbers, bool singlesave = true){
			for (int c = 0; c < numbers.Length; c++) {
				if(!string.IsNullOrWhiteSpace(numbers[c])){
					contact.Number += Values.FORMATSEPARATOR + /*PhoneUtil.ToNumber_Custom*/(numbers [c]);
				}
			}

			if (singlesave) {
				App.Database.UpdateItem (contact); 
			}
		}
		public ContactData RemoveNamelist(string namelist){
			contact.Playlist = contact.Playlist.Replace (FormatNamelist (namelist), Values.FORMATSEPARATOR);
			Debug.WriteLine ("removed {0} from {1}'s namelists. result: {2}", namelist, contact.Name, contact.Playlist);
			return contact;
		}
		public ContactData AddNamelist(string[] namelists, bool save = true){
			for (int c = 0; c < namelists.Length; c++) {
				if(!string.IsNullOrWhiteSpace(namelists[c])){
					if (contact.Playlist.EndsWith (FormatNamelist(namelists [c]))) {
						//already part of this namelist
					} else if (contact.Playlist.EndsWith (Values.FORMATSEPARATOR)) {
						Debug.WriteLine ("previous namelist was added: ends with separator");
						contact.Playlist += namelists [c] + Values.FORMATSEPARATOR;
					} else {
						Debug.WriteLine ("No previous namelist added");
						contact.Playlist += Values.FORMATSEPARATOR+ namelists [c]+Values.FORMATSEPARATOR;
					}
					Debug.WriteLine ("Added a namelist to {0}: {1}", contact.Name, contact.Playlist);
				}
			}

			if (save) {
				App.Database.UpdateItem (contact);
			}

			return contact;
		}
		public static ContactData AddNamelist(ContactData person, string[] namelists, bool save = true){
			for (int c = 0; c < namelists.Length; c++) {
				if(!string.IsNullOrWhiteSpace(namelists[c])){
					if (person.Playlist.EndsWith (Values.FORMATSEPARATOR + namelists[c] + Values.FORMATSEPARATOR)) {
						//already part of this namelist
					} else if (person.Playlist.EndsWith (Values.FORMATSEPARATOR)) {
						Debug.WriteLine ("previous namelist was added: ends with separator");
						person.Playlist += namelists [c] + Values.FORMATSEPARATOR;
					} else {
						Debug.WriteLine ("No previous namelist added");
						person.Playlist += Values.FORMATSEPARATOR+ namelists [c]+Values.FORMATSEPARATOR;
					}
					Debug.WriteLine ("Added a namelist to {0}: {1}", person.Name, person.Playlist);
				}
			}

			if (save) {
				App.Database.UpdateItem (person);
			}

			return person;
		}
		public static string FormatNamelist(string namelist){
			return Values.FORMATSEPARATOR + namelist + Values.FORMATSEPARATOR;
		}

		 
		void NullCheck(){
			if (contact == null) {
				throw new NullReferenceException ("ContactViewModel has null contact reference, initialize it first");
			}
		}

		public string[] SeparateMultipleValuesFromContactFieldIntoArray(string formatted){
			return formatted.Split (new string[]{Values.FORMATSEPARATOR}, StringSplitOptions.None);
		}

		List<string> SeparateMultipleValuesFromContactFieldIntoList(string formatted){
			return formatted.Split (new string[]{Values.FORMATSEPARATOR}, StringSplitOptions.None).ToList();
		}
			
	}
}

