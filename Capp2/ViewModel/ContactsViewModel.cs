using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace Capp2
{
	public class ContactsViewModel:BaseViewModel
	{
		string namelist;
		bool allSelected = false;

		ObservableCollection<ContactData> _list;
		public ObservableCollection<ContactData> Contacts {
			get {
				return _list;
			}
			set {
				SetProperty(ref _list, value, nameof(_list));
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
			}
		}

		public ContactsViewModel(string namelist)
		{
			this.namelist = namelist;
			Task.Run(() => {
				_list = App.Database.GetObservableItems(namelist);
				_groupedlist = Group(_list);
			});
		}

		ObservableCollection<Grouping<string, ContactData>> Group(ObservableCollection<ContactData> list) { 
			var group =  list.OrderBy(p => p.FirstName)
						.GroupBy(p => p.FirstName[0].ToString())
						.Select(p => new Grouping<string, ContactData>(p))
			                 .ToArray();
			var observableGroup = new ObservableCollection<Grouping<string, ContactData>>();
			for (int c = 0; c < group.Length; c++) {
				observableGroup.Add(group[c]);
			}
			return observableGroup;
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

		ContactData FindObservableContact(int ID)
		{
			foreach (ContactData contact in Contacts) {
				if (contact.ID == ID) return contact;
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

		async Task AddContacts()
		{
			var import = Util.ImportChoices(namelist);
			var importResult = await UserDialogs.Instance.ActionSheetAsync(
				"Where do you want to add contacts from?", "Cancel", null, import);
			try
			{
				if (importResult == Values.IMPORTCHOICEMANUAL)
				{
					await App.NavPage.Navigation.PushAsync(new AddEditContactNativePage());
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
					var list = App.Database.GetItems(importResult).ToList<ContactData>();
					if (list.Count == 0)
					{
						AlertHelper.Alert("Empty Namelist", importResult + " has no contacts", "OK");
					}
					else {
						await App.NavPage.Navigation.PushModalAsync(new CappModal(importResult,
							this.namelist, App.Database.GetGroupedItems(importResult),
						    list, App.Database.GetItems(this.namelist).ToList<ContactData>()));
					}
				}
			}
			catch (Exception) { }
		}

		public void DeselectAll(bool updateDB = false)
		{
			foreach (ContactData c in Contacts) {
				c.IsSelected = false;
			}
			allSelected = false;
			if(updateDB) App.Database.UpdateAll(Contacts.AsEnumerable());
		}

		public void Deselect(ContactData[] contacts) {//faster to just deselectall?
			
		}

		public void SelectAll(bool updateDB = false)
		{
			foreach (ContactData c in Contacts)
			{
				c.IsSelected = true;
			}
			allSelected = true;
			if(updateDB) App.Database.UpdateAll(Contacts.AsEnumerable());
		}

		public void SelectDeselectAll(bool updateDB = false)
		{
			if (allSelected) DeselectAll(updateDB);
			else SelectAll(updateDB);
		}

		public ObservableCollection<ContactData> GetSelectedItems()
		{
			var selectedItems = new ObservableCollection<ContactData>();
			var arr = Contacts.ToArray();
			for (int c = 0; c < arr.Length; c++)
			{
				if (arr[c].IsSelected)
				{
					selectedItems.Add(arr[c]);
				}
			}
			return selectedItems;
		}
	}
}

