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

		public ContactViewModel(ContactData contact){
			this.contact = contact;
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
			for (int c = 0;c < numbers.Length;c++) {
				numbers[c] = App.contactFuncs.MakeDBContactCallable (numbers[c], false);
			}
			return numbers;
		}

		public void AddNumbers(string[] numbers, bool singlesave = true){
			for (int c = 0; c < numbers.Length; c++) {
				if(!string.IsNullOrWhiteSpace(numbers[c])){
					contact.Number += ";" + /*PhoneUtil.ToNumber_Custom*/(numbers [c]);
				}
			}

			if (singlesave) {
				App.Database.UpdateItem (contact);
			}
		}

		public void AddNamelist(string[] namelists, bool singlesave = true){
			for (int c = 0; c < namelists.Length; c++) {
				if(!string.IsNullOrWhiteSpace(namelists[c])){
					contact.Playlist += ";"+ namelists [c];
				}
			}

			if (singlesave) {
				App.Database.UpdateItem (contact);
			}
		}

		string[] SeparateMultipleValuesFromContactFieldIntoArray(string formatted){
			return formatted.Split (new char[]{';' });
		}

		List<string> SeparateMultipleValuesFromContactFieldIntoList(string formatted){
			return formatted.Split (new char[]{';' }).ToList();
		}
			
	}
}

