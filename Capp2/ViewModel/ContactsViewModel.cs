using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace Capp2
{
	public class ContactsViewModel:BaseViewModel
	{
		string namelist;
		public bool AllContactsSelected { get; set; } = false;
		public bool ContactsAlreadyDeselected { get; set; } = false;

		string _contactscount;
		public string ContactsCount { 
			get {
				return _contactscount;
			} 
			set {
				_contactscount = value;
			}
		}

		void UpdateContactCount() { 
			var numberofcontacts = Contacts.Count;
			_contactscount = (numberofcontacts > 1) ? numberofcontacts + " Contacts" : numberofcontacts + " Contact";
		}

		string _contactscount;
		public string ContactsCount { 
			get {
				return _contactscount;
			} 
			set {
				_contactscount = value;
			}
		}

		void UpdateContactCount() { 
			var numberofcontacts = Contacts.Count;
			_contactscount = (numberofcontacts > 1) ? numberofcontacts + " Contacts" : numberofcontacts + " Contact";
		}

		ObservableCollection<ContactData> _list;
		public ObservableCollection<ContactData> Contacts {
			get {
				return _list;
			}
			set {
				SetProperty(ref _list, value, nameof(_list));
				UpdateContactCount();
				_groupedlist = Group(_list);
			}
		}

		ObservableCollection<Grouping<string, ContactData>> _groupedlist;
		public ObservableCollection<Grouping<string, ContactData>> GroupedContacts
		{
			get
			{
				return _groupedlist;
			}
			set
			{
				SetProperty(ref _groupedlist, value, nameof(_groupedlist));
				_list = App.Database.GetObservableItems(namelist);
			}
		}
<<<<<<< Updated upstream
		public void Refresh() {
			Contacts = App.Database.GetObservableItems(namelist);
			App.CapPage.listView.ItemsSource = GroupedContacts;
=======
		public void Refresh() { 
			Contacts = App.Database.GetObservableItems(namelist);
>>>>>>> Stashed changes
		}
		public ContactsViewModel(string namelist)
		{
			this.namelist = namelist;
			Contacts = App.Database.GetObservableItems(namelist);
		}
		public ContactsViewModel(string namelist, ObservableCollection<Grouping<string, ContactData>> groupedcontacts, 
		                        ObservableCollection<ContactData> contacts)
		{
			this.namelist = namelist;
			_list = contacts;
			_groupedlist = groupedcontacts;
		}

		public ObservableCollection<Grouping<string, ContactData>> Group(ObservableCollection<ContactData> list) {
			Debug.WriteLine("in ContactsViewModel.Group");
			var group = list.OrderBy(p => p.FirstName)
						.GroupBy(p => p.FirstName[0].ToString())
						.Select(p => new Grouping<string, ContactData>(p));
			//var observableGroup = new ObservableCollection<Grouping<string, ContactData>>();
			/*for (int c = 0; c < group.Length; c++) {
				observableGroup.Add(group[c]);
			}*/
			return new ObservableCollection<Grouping<string, ContactData>>(group);
		}

		ContactData FindContact(int ID)
		{
			var arr = Contacts.ToArray();
			for (int c = 0; c < arr.Length; c++)
			{
				if (arr[c].ID == ID) 
					return arr[c];
			}
			return null;
		}

		public void AddContactToGroupedContactsThenReorder(ContactData contact) {//Grouping class doesn't reflect changes
			var templist = _list;
			templist.Add(contact);
			GroupedContacts = Group(templist);
		}

		public ContactData FindObservableContact(int ID)
		{
			foreach (ContactData contact in Contacts) {
				if (contact.ID == ID) return contact;
			}
			return null;
		}

		public ContactData FindObservableGroupedContact(int ID)
		{
			int GroupCtr = GroupedContacts.Count;
			int ContactInGroupCtr = 0;
			ContactData person;

			for (int c = 0; c < GroupCtr; c++)
			{
				var contactGroup = GroupedContacts.ElementAt(c);
				ContactInGroupCtr = contactGroup.Count;
				for (int n = 0; n < ContactInGroupCtr; n++)
				{
					person = contactGroup.ElementAt(n);
					if (person.ID == ID) return person;
				}
			}
			return null;
		}

		public void MarkForTodaysCalls(ContactData contact, bool yescall, DateTime date)
		{
			var matchingContact = FindObservableContact(contact.ID);
			if (matchingContact != null)
			{
				if (yescall)
				{
					matchingContact.OldPlaylist = Values.TODAYSCALLSUNDEFINED;
					Contacts.Remove(matchingContact);
					foreach (Grouping<string, ContactData> grouping in GroupedContacts) {
						var result = grouping.Remove(matchingContact);
						Debug.WriteLine("MarkForTodaysCalls Found a match: {0}", result);
						if (result) return;//doesn't reflect in UI
					}
				}
				else {
					matchingContact.NextCall = date.Date;
					matchingContact.OldPlaylist = Values.CALLTODAY;
					App.Database.UpdateItem(matchingContact);
					Debug.WriteLine("Will follow up {0} on {1}. date.Date param: {2}. OldPlaylist is {3}",
						matchingContact.Name, matchingContact.NextCall, date.Date, matchingContact.OldPlaylist);
				}
			}
			else throw new NullReferenceException("contact doesn't exist in VM");
		}

		void SaveContact(ContactData contact) {
		}
		public string[] PlaylistsIntoStringArr()
		{
			Playlist[] list = App.Database.GetPlaylistItems().ToArray();
			List<string> finalList = new List<string>();

			for (int c = 0; c < list.Length; c++)
			{
				if (!string.Equals(list[c].PlaylistName, Values.ALLPLAYLISTPARAM) &&
				   !string.Equals(list[c].PlaylistName, Values.TODAYSCALLS))
				{
					finalList.Add(list[c].PlaylistName);
				}
			}
			return finalList.ToArray();
		}

		public async Task RemoveAllSelectedContactsFromNamelist(bool save = true) {
			int GroupCtr = GroupedContacts.Count;
<<<<<<< Updated upstream
			//int ContactInGroupCtr;
=======
			int ContactInGroupCtr = 0;
>>>>>>> Stashed changes
			ContactData person;
			List<ContactData> contactsToUpdateInDB = new List<ContactData>();

			try {
<<<<<<< Updated upstream
				for (int c = 0; c < GroupedContacts.Count; c++)
				{
					var contactGroup = GroupedContacts.ElementAt(c);
					//ContactInGroupCtr = contactGroup.Count;
					for (int n = 0; n < contactGroup.Count; n++)
=======
				for (int c = 0; c < GroupCtr; c++)
				{
					var contactGroup = GroupedContacts.ElementAt(c);
					ContactInGroupCtr = contactGroup.Count;
					for (int n = 0; n < ContactInGroupCtr; n++)
>>>>>>> Stashed changes
					{
						person = contactGroup.ElementAt(n);
						if (person.IsSelected)
						{
<<<<<<< Updated upstream
							Debug.WriteLine("Removing namelist {0} from {1}: number {2}/{5} in group#{3}/{4}", 
							                this.namelist, person.Name, n, c, GroupedContacts.Count, contactGroup.Count);
							RemoveNamelistFromContact(person, this.namelist);
							person.IsSelected = false;//no need for separate deselect call
							//contactGroup.Remove(person);
							//Contacts.Remove(person);
							//n--;//adjust size so loop avoids crashing and processes the rest of the collections for deletion	
							contactsToUpdateInDB.Add(person);
						}
						else {
							Debug.WriteLine("{0} is not selected", person.Name);
						}
					}
					/*if (contactGroup.Count < 1) { 
						GroupedContacts.Remove(contactGroup);
						c--;
					}*/
				}
				ContactsAlreadyDeselected = true;
				//Refresh();

=======
							Debug.WriteLine("Removing namelist {0} from {1}", this.namelist, person.Name);
							RemoveNamelistFromContact(person, this.namelist);
							person.IsSelected = false;//no need for separate deselect call
							contactGroup.Remove(person);
							Contacts.Remove(person);
							n--;//adjust size so loop avoids crashing and processes the rest of the collections for deletion	
							contactsToUpdateInDB.Add(person);
						}
					}
				}
>>>>>>> Stashed changes
			} catch (ArgumentOutOfRangeException ie) {
				Debug.WriteLine("ContactsViewModel.RemoveAllSelectedContactsFromNamelist expected error:D {0}", ie.Message);
			}

			if (save) App.Database.UpdateAll(contactsToUpdateInDB);
			contactsToUpdateInDB = null;
			person = null;
		}
<<<<<<< Updated upstream

		void RemoveNamelistFromContact(ContactData contact, string namelistToRemove) { 
			contact.Playlist = contact.Playlist.Replace(FormatNamelist(namelistToRemove), 
			                                            Values.FORMATSEPARATOR);
			Debug.WriteLine("removed {0} from {1}'s namelists. result: {2}", namelistToRemove,
							 contact.Name, contact.Playlist);
		}

=======

		void RemoveNamelistFromContact(ContactData contact, string namelistToRemove) { 
			contact.Playlist = contact.Playlist.Replace(FormatNamelist(namelistToRemove), 
			                                            Values.FORMATSEPARATOR);
			Debug.WriteLine("removed {0} from {1}'s namelists. result: {2}", namelistToRemove,
							 contact.Name, contact.Playlist);
		}

>>>>>>> Stashed changes
		public static ObservableCollection<ContactData> FilterNameNumberOrg(
			ObservableCollection<ContactData> list, string filter)
		{
			var results = new ObservableCollection<ContactData>();
				
			if (list == null)
			{
				Debug.WriteLine("input to filter by array is null");
				return null;
			}

			try
			{
				/*return list
					.Where(x => x.Name.ToLower().Contains(filter.ToLower())
					       || x.Number.ToLower().Contains(filter.ToLower())).ToList();*/
				var arr = list.ToArray();
				for (int c = 0; c < arr.Length; c++) {
					if (arr[c].Name.ToLower().Contains(filter.ToLower()) || 
					    arr[c].Number.ToLower().Contains(filter.ToLower())) 
					{
						results.Add(arr[c]);
					}
				}
				return results;
			}
			catch (Exception e)
			{
				Debug.WriteLine("filterbyArray error: {0}", e.Message);
			}
			return null;
		}

		public async Task CopySelectedItemsToNamelistChosenByUser() {
			var MoveToResult = await UserDialogs.Instance.ActionSheetAsync(
				"Move to Namelist", "Cancel", null, PlaylistsIntoStringArr());
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
			var contactsToSave = new List<ContactData>();

			if (!string.Equals(MoveToResult, "Cancel"))
			{
				if (!string.IsNullOrWhiteSpace(MoveToResult))
				{
					Debug.WriteLine("ABOUT TO LOOP");

					foreach (ContactData contact in Contacts)
					{
						if (contact.IsSelected)
						{
							AddNamelistsToContact(contact, new string[] { MoveToResult }, false);
							Debug.WriteLine(contact.Name + " is being copied to " + MoveToResult);
							contact.IsSelected = false;
							contactsToSave.Add(contact);
						}
					}
					App.Database.UpdateAll(contactsToSave);
					UserDialogs.Instance.ShowSuccess("Copied!", 2000);
				}
				else {
					UserDialogs.Instance.WarnToast("Oops! You didn't choose a new namelist. Please try again", null, 2000);
				}
			}
<<<<<<< Updated upstream
			ContactsAlreadyDeselected = true;
=======
>>>>>>> Stashed changes
		}
		public ContactData AddNamelistsToContactThenReturn(ContactData contact, string[] namelists, bool save = true)
		{
			for (int c = 0; c < namelists.Length; c++)
			{
				if (!string.IsNullOrWhiteSpace(namelists[c]))
				{
					if (contact.Playlist.EndsWith(FormatNamelist(namelists[c])))
					{
						//already part of this namelist
					}
					else if (contact.Playlist.EndsWith(Values.FORMATSEPARATOR))
					{
						Debug.WriteLine("previous namelist was added: ends with separator");
						contact.Playlist += namelists[c] + Values.FORMATSEPARATOR;
					}
					else {
						Debug.WriteLine("No previous namelist added");
						contact.Playlist += Values.FORMATSEPARATOR + namelists[c] + Values.FORMATSEPARATOR;
					}
					Debug.WriteLine("Added a namelist to {0}: {1}", contact.Name, contact.Playlist);
				}
			}
			if (save)
			{
				App.Database.UpdateItem(contact);
			}
			return contact;
		}
		public void AddNamelistsToContact(ContactData contact, string[] namelists, bool save = true)
		{
			for (int c = 0; c < namelists.Length; c++)
			{
				if (!string.IsNullOrWhiteSpace(namelists[c]))
				{
					if (contact.Playlist.EndsWith(FormatNamelist(namelists[c])))
					{
						//already part of this namelist
					}
					else if (contact.Playlist.EndsWith(Values.FORMATSEPARATOR))
					{
						Debug.WriteLine("previous namelist was added: ends with separator");
						contact.Playlist += namelists[c] + Values.FORMATSEPARATOR;
					}
					else {
						Debug.WriteLine("No previous namelist added");
						contact.Playlist += Values.FORMATSEPARATOR + namelists[c] + Values.FORMATSEPARATOR;
					}
					Debug.WriteLine("Added a namelist to {0}: {1}", contact.Name, contact.Playlist);
				}
			}
			if (save)
			{
				App.Database.UpdateItem(contact);
			}
			//return contact;
		}
		public static string FormatNamelist(string namelist)
		{
			return Values.FORMATSEPARATOR + namelist + Values.FORMATSEPARATOR;
		}

		public static string AddContactsToNamelist(string namelistData, string[] namelists)//, bool save = true)
		{
			for (int c = 0; c < namelists.Length; c++)
			{
				if (!string.IsNullOrWhiteSpace(namelists[c]))
				{
					if (namelistData.Contains(Values.FORMATSEPARATOR + namelists[c] + Values.FORMATSEPARATOR))
					{
						//already part of this namelist
					}
					else if (namelistData.EndsWith(Values.FORMATSEPARATOR))
					{
						Debug.WriteLine("previous namelist was added: ends with separator");
						namelistData += namelists[c] + Values.FORMATSEPARATOR;
					}
					else {
						Debug.WriteLine("No previous namelist added");
						namelistData += Values.FORMATSEPARATOR + namelists[c] + Values.FORMATSEPARATOR;
					}
					Debug.WriteLine("returning new namelist string {0}", namelistData);
				}
			}

			/*if (save)
			{
				App.Database.UpdateItem(person);
			}*/
			return namelistData;
		}

		public async Task AddContacts()
		{
			var import = Util.ImportChoices(namelist);
			var importResult = await UserDialogs.Instance.ActionSheetAsync(
				"Where do you want to add contacts from?", "Cancel", null, import);
			try
			{
				if (importResult == Values.IMPORTCHOICEMANUAL)
				{
<<<<<<< Updated upstream
					//await App.NavPage.Navigation.PushAsync(new AddEditContactNativePage());
					//await App.NavPage.Navigation.PopAsync(false);
					await AddEditContactNativePage.OpenNativeContactsUI();
=======
					await App.NavPage.Navigation.PushAsync(new AddEditContactNativePage());
					await App.NavPage.Navigation.PopAsync(false);
					//await AddEditContactNativePage.OpenNativeContactsUI();
>>>>>>> Stashed changes
				}
				else if (importResult == Values.IMPORTCHOICEGDRIVE)
				{
					UserDialogs.Instance.InfoToast("Not yet implemented", null, 2000);
				}
				else if (string.Equals(importResult, "Cancel"))
				{
					//do nothing
				}
				else
				{
					Debug.WriteLine("Importing from {0}", importResult);
					var list = App.Database.GetObservableItems(importResult);//App.Database.GetItems(importResult).ToList<ContactData>();
					if (list.Count == 0)
					{
						AlertHelper.Alert("Empty Namelist", importResult + " has no contacts", "OK");
					}
					else {
<<<<<<< Updated upstream
						await App.NavPage.Navigation.PushModalAsync(new CappModal(importResult,
						    this.namelist, Group(list), list/*, Contacts*/));
=======
						//await App.NavPage.Navigation.PushModalAsync(new CappModal(importResult,
						//    this.namelist, Group(list), list/*, Contacts*/));
>>>>>>> Stashed changes
					}
				}
			}
			catch (Exception) { }
		}

		public void DeselectAll(bool modal = false)
		{
<<<<<<< Updated upstream
			/*Debug.WriteLine("Sending DESELECTALLMESSAGE");
			if(modal) MessagingCenter.Send("", Values.DESELECTALLMESSAGECAPPMODAL);
			else MessagingCenter.Send("", Values.DESELECTALLMESSAGE);*/
=======
>>>>>>> Stashed changes
			int GroupCtr = GroupedContacts.Count;
			int ContactInGroupCtr = 0;
			var contactsToUpdate = new List<ContactData>();
			ContactData person;

			for (int c = 0; c < GroupCtr; c++)
			{
				var contactGroup = GroupedContacts.ElementAt(c);
				ContactInGroupCtr = contactGroup.Count;
				for (int n = 0; n < ContactInGroupCtr; n++)
				{
					person = contactGroup.ElementAt(n);
					Debug.WriteLine("DeSelectAll Looping {0}", person.Name);
					person.IsSelected = false;
					Debug.WriteLine("{0} IsSelected: {1}", person.Name, person.IsSelected);
					contactsToUpdate.Add(person);
				}
			}

<<<<<<< Updated upstream
			AllContactsSelected = false;
			//if (updateDB) App.Database.UpdateAll(contactsToUpdate);
=======
			allSelected = false;
			if (updateDB) App.Database.UpdateAll(contactsToUpdate);
				 //UngroupListButRetainOrder(GroupedContacts));
>>>>>>> Stashed changes
		}

		public void SelectAll(bool modal = false)
		{
			//if(modal) MessagingCenter.Send("", Values.SELECTALLMESSAGECAPPMODAL);
			//else MessagingCenter.Send("", Values.SELECTALLMESSAGE);
			int GroupCtr = GroupedContacts.Count;
			int ContactInGroupCtr = 0;
			var contactsToUpdate = new List<ContactData>();
			ContactData person;

			for (int c = 0; c < GroupCtr; c++)
			{
				var contactGroup = GroupedContacts.ElementAt(c);
				ContactInGroupCtr = contactGroup.Count;
				for (int n = 0; n < ContactInGroupCtr; n++)
				{
					person = contactGroup.ElementAt(n);
					Debug.WriteLine("SelectAll Looping {0}", person.Name);
					person.IsSelected = true;
					Debug.WriteLine("{0} IsSelected: {1}", person.Name, person.IsSelected);
					contactsToUpdate.Add(person);
				}
			}

			AllContactsSelected = true;
			//if (updateDB) App.Database.UpdateAll(contactsToUpdate);
		}

		public ObservableCollection<ContactData> UngroupListButRetainOrder(
			ObservableCollection<Grouping<string, ContactData>> group)
		{
<<<<<<< Updated upstream
			ObservableCollection<ContactData> UngroupedOrderedList = 
				new ObservableCollection<ContactData>();
			int GroupCtr = group.Count;
			int ContactInGroupCtr = 0;

			if (group == null)
				throw new NullReferenceException("UngroupListButRetainOrder has null param");

			for (int c = 0; c < GroupCtr; c++)
			{
				var contactGroup = group.ElementAt(c);
				ContactInGroupCtr = contactGroup.Count;
				for (int n = 0; n < ContactInGroupCtr; n++)
				{
					Debug.WriteLine("Adding {0} to UngroupedOrderedList", contactGroup.ElementAt(n));
					UngroupedOrderedList.Add(contactGroup.ElementAt(n));
				}
			}

=======
			int GroupCtr = GroupedContacts.Count;
			int ContactInGroupCtr = 0;
			var contactsToUpdate = new List<ContactData>();
			ContactData person;

			for (int c = 0; c < GroupCtr; c++)
			{
				var contactGroup = GroupedContacts.ElementAt(c);
				ContactInGroupCtr = contactGroup.Count;
				for (int n = 0; n < ContactInGroupCtr; n++)
				{
					person = contactGroup.ElementAt(n);
					Debug.WriteLine("SelectAll Looping {0}", person.Name);
					person.IsSelected = true;
					Debug.WriteLine("{0} IsSelected: {1}", person.Name, person.IsSelected);
					contactsToUpdate.Add(person);
				}
			}

			allSelected = true;
			if (updateDB) App.Database.UpdateAll(contactsToUpdate);
				//UngroupListButRetainOrder(GroupedContacts));
		}

		public ObservableCollection<ContactData> UngroupListButRetainOrder(
			ObservableCollection<Grouping<string, ContactData>> group)
		{
			ObservableCollection<ContactData> UngroupedOrderedList = 
				new ObservableCollection<ContactData>();
			int GroupCtr = group.Count;
			int ContactInGroupCtr = 0;

			if (group == null)
				throw new NullReferenceException("UngroupListButRetainOrder has null param");

			for (int c = 0; c < GroupCtr; c++)
			{
				var contactGroup = group.ElementAt(c);
				ContactInGroupCtr = contactGroup.Count;
				for (int n = 0; n < ContactInGroupCtr; n++)
				{
					Debug.WriteLine("Adding {0} to UngroupedOrderedList", contactGroup.ElementAt(n));
					UngroupedOrderedList.Add(contactGroup.ElementAt(n));
				}
			}

>>>>>>> Stashed changes
			return UngroupedOrderedList;
		}

		public void SelectDeselectAll(bool modal = false)
		{
			if (AllContactsSelected) DeselectAll(modal);
			else SelectAll(modal);
		}

		public ObservableCollection<ContactData> GetSelectedItems()
		{
			/*var selectedItems = new ObservableCollection<ContactData>();

			var arr = Contacts.ToArray();
			for (int c = 0; c < arr.Length; c++)
			{
				if (arr[c].IsSelected)
				{
					selectedItems.Add(arr[c]);
				}
			}
			return selectedItems;*/
			return new ObservableCollection<ContactData>(
				UngroupListButRetainOrder(GroupedContacts).Where(contact => contact.IsSelected == true));
		}
	}
}

