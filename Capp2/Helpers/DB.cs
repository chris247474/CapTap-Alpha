using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;
using System.Linq;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Capp2.Helpers;

namespace Capp2
{
	public class DB
	{ 
		static object locker = new object ();
		SQLite.SQLiteConnection database; 
		List<ContactData> list;
		ContactViewModel contactViewModel;

		public DB () 
		{
			database = DependencyService.Get<ISQLite> ().GetConnectionCAPP ();
			database.CreateTable<ContactData>();
            database.CreateTable<Playlist>();
		}
		public List<ContactData> GetItems (string playlist) {
			Debug.WriteLine ("Fetching list {0}", playlist);
			if (playlist == "All") {
				//dont choose which playlist, display all contacts including those marked as different playlists because "All" will be understood as a separate namelist
				list = (from x in (database.Table<ContactData> ().OrderBy (x => x.FirstName))
					select x).ToList<ContactData> ();
				App.lastIndex = list.Count+1;
			} else if(playlist == Values.TODAYSCALLS){
				//all contacts regardless of playlist, marked for callback today
				list = (from x in (database.Table<ContactData> ().
					Where (c => c.OldPlaylist == Values.CALLTODAY && c.NextCall == DateTime.Today.Date).
				                   OrderBy (x => x.FirstName))
					select x).ToList<ContactData> (); 
				App.lastIndex = list.Count+1;
			}else {
				try{
					//get list of namelists, compare to playlist param. 
					var matchingNamelist = FindMatchingNamelist(playlist);
					list = (from x in (database.Table<ContactData> ().Where(x => x.Playlist.Contains(matchingNamelist))
					                   .OrderBy (x => x.FirstName))
						select x).ToList<ContactData> ();
					App.lastIndex = list.Count+1;
				}catch(Exception e){
					Debug.WriteLine (e.Message+" --------------------------- EMPTYEXCEPTION");
					UserDialogs.Instance.Alert ("No contacts yet", "This namelist is still empty", "I'll add some in a bit");
					list = null;
				}
			}

			if (list == null) {
				list = new ContactData[]{}.ToList();
			}
			var listarr = list.ToArray();

			for(int c = 0;c < listarr.Length;c++){
				var firstinitial = listarr[c].FirstName[0];
				var secondinitial = listarr[c].LastName[0];
				if(listarr[c].HasDefaultImage_Small){
					listarr[c].Initials = firstinitial.ToString()+secondinitial.ToString();
				}else{
					listarr[c].Initials = string.Empty;
				}

				/*Need to do this before populating every list - for some reason, string ContactData.Number doesn't
				 keep the callable format after being stored in the db.
				 Additionally, searching the namelist only returns literal/exact matches and not similar matches,
				 ex: searching for 09163247357 will not show 0916(324)7357 to the user if we don't call Util.MakeDBContactCallable
				 ,because formats are lost while storing into SQLite*/
				listarr[c].Number = App.contactFuncs.MakeDBContactCallable (listarr[c].Number, false);
				if (!string.IsNullOrWhiteSpace(listarr[c].Number2))
				{
					listarr[c].Number2 = App.contactFuncs.MakeDBContactCallable(listarr[c].Number2, false);
					if (!string.IsNullOrWhiteSpace(listarr[c].Number3))
					{
						listarr[c].Number3 = App.contactFuncs.MakeDBContactCallable(listarr[c].Number3, false);
						if (!string.IsNullOrWhiteSpace(listarr[c].Number4))
						{
							listarr[c].Number4 = App.contactFuncs.MakeDBContactCallable(listarr[c].Number4, false);
							if (!string.IsNullOrWhiteSpace(listarr[c].Number5))
							{
								listarr[c].Number5 = App.contactFuncs.MakeDBContactCallable(listarr[c].Number5, false);
							}
						}
					}
				}
				//Debug.WriteLine ("{0}'s number is {1}", listarr[c].Name, listarr[c].Number);
			}
			return list;
		}

		string FindMatchingNamelist(string playlist){
			var playlists = GetPlaylistNames(); 
			for(int c = 0;c < playlists.Length;c++){
				if(string.Equals(playlists[c], playlist)){
					Debug.WriteLine ("FindMatchingNamelist returning {0}", playlist);
					return FormatNamelist(playlist);
				}
			}
			return "hypomonstrososquipedalophobia";//doesnt matter what string goes here as long as it doesnt match any namelist
		}
		string FormatNamelist(string namelist){
			return Values.FORMATSEPARATOR + namelist + Values.FORMATSEPARATOR;
		}

		public int GetTodaysYesCalls(){
			var ListArr = (from x in (database.Table<ContactData> ()) select x).ToArray ();

			List<ContactData> CalledList = new List<ContactData> ();

			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Called.Date == DateTime.Today && ListArr[c].Appointed.Date > DateTime.MinValue) {
					CalledList.Add (ListArr [c]);
				}
			}

			return CalledList.Count;
		}

		public List<ContactData> GetCalledContacts(string playlist){
			ContactData[] ListArr = new ContactData[]{ };
			if (string.Equals (playlist, Values.ALLPLAYLISTPARAM)) {
				ListArr = (from x in (database.Table<ContactData> ()) select x).ToArray ();
			}else{
				ListArr = (from x in (database.Table<ContactData> ().Where(contact => contact.Playlist == playlist)) 
					select x).ToArray ();
			}

			List<ContactData> CalledList = new List<ContactData> ();

			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Called.Date > DateTime.MinValue) {
					CalledList.Add (ListArr [c]);
				}
			}

			return CalledList;
		}

		public List<ContactData> GetAppointedContacts(string playlist){
			ContactData[] ListArr = new ContactData[]{ };
			if (string.Equals (playlist, Values.ALLPLAYLISTPARAM)) {
				ListArr = (from x in (database.Table<ContactData> ()) select x).ToArray ();
			}else{
				ListArr = (from x in (database.Table<ContactData> ().Where(contact => contact.Playlist == playlist)) 
					select x).ToArray ();
			}

			List<ContactData> AppointedList = new List<ContactData> ();
			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Appointed.Date > DateTime.MinValue) {
					AppointedList.Add (ListArr [c]);
				}
			}

			return AppointedList;
		}

		public List<ContactData> GetPresentedContacts(string playlist){
			ContactData[] ListArr = new ContactData[]{ };
			if (string.Equals (playlist, Values.ALLPLAYLISTPARAM)) {
				ListArr = (from x in (database.Table<ContactData> ()) select x).ToArray ();
			}else{
				ListArr = (from x in (database.Table<ContactData> ().Where(contact => contact.Playlist == playlist)) 
					select x).ToArray ();
			}

			List<ContactData> PresentedList = new List<ContactData> ();
			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Presented.Date > DateTime.MinValue) {
					PresentedList.Add (ListArr [c]);
				}
			}

			return PresentedList;
		}

		public List<ContactData> GetPurchasedContacts(string playlist){
			ContactData[] ListArr = new ContactData[]{ };
			if (string.Equals (playlist, Values.ALLPLAYLISTPARAM)) {
				ListArr = (from x in (database.Table<ContactData> ()) select x).ToArray ();
			}else{
				ListArr = (from x in (database.Table<ContactData> ().Where(contact => contact.Playlist == playlist)) 
					select x).ToArray ();
			}

			List<ContactData> PurchasedList = new List<ContactData> ();
			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Purchased.Date > DateTime.MinValue) {
					PurchasedList.Add (ListArr [c]);
				}
			}

			return PurchasedList;
		}

		public ChartData[] GetDailyYesCalls(){
			var ListArr = GetCalledContacts (Values.ALLPLAYLISTPARAM).ToArray();
			var tempChartData = new ChartData ();
			List<ChartData> data = new List<ChartData> ();

			var date = DateTime.Today.Date;
			var installdate = Settings.InstallDateSettings;

			if (ListArr != null && ListArr.Length > 0) {
				while (date.Date != installdate.Date) {
					Debug.WriteLine ("Looped date: {0}", date.Date);

					tempChartData = new ChartData {
						Name = date.Date.ToString ()
					};

					for (int c = 0; c < ListArr.Length; c++) {
						if (ListArr [c].Called.Date == date.Date) {
							tempChartData.Value++;
						}
					}

					data.Add (tempChartData);

					date = date.AddDays (-1);
				}
			} else {
				//no yes calls yet
				return new ChartData[]{ };
			}

			return data.ToArray ();
		}

		public ChartData[] GetCappStats(){
			var ListArr = (from x in (database.Table<ContactData> ()) select x).ToArray ();

			List<ContactData> CalledList = new List<ContactData> ();
			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Called.Date > DateTime.MinValue) {
					CalledList.Add (ListArr [c]);
				}
			}
			List<ContactData> AppointedList = new List<ContactData> ();
			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Appointed.Date > DateTime.MinValue) {
					AppointedList.Add (ListArr [c]);
				}
			}
			List<ContactData> PresentedList = new List<ContactData> ();
			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Presented.Date > DateTime.MinValue) {
					PresentedList.Add (ListArr [c]);
				}
			}
			List<ContactData> PurchasedList = new List<ContactData> ();
			for (int c = 0; c < ListArr.Length; c++) {
				if (ListArr [c].Purchased.Date > DateTime.MinValue) {
					PurchasedList.Add (ListArr [c]);
				}
			}

			ChartData Called = new ChartData{Name = "Not Appointed", Value = CalledList.Count - AppointedList.Count};
			ChartData Appointed = new ChartData{Name = "Appointed", Value = AppointedList.Count - PresentedList.Count};
			ChartData Presented = new ChartData{Name = "Presented", Value = PresentedList.Count - PurchasedList.Count};
			ChartData Purchased = new ChartData{Name = "Purchased", Value = PurchasedList.Count};

			Debug.WriteLine ("Called: {0}, Appointed: {1}, Presented: {2}, Purchased: {3}", 
				CalledList.Count, AppointedList.Count, PresentedList.Count, PurchasedList.Count);

			return new ChartData[]{ Called, Appointed, Presented, Purchased};
		}

		public List<Grouping<string, ContactData>> GetGroupedItems (string playlist) {
			try{
				list = GetItems(playlist);

				var groupedData =
					list.OrderBy(p => p.FirstName)
					    .GroupBy(p => p.FirstName[0].ToString())
						.Select(p => new Grouping<string, ContactData>(p))
						.ToList<Grouping<string, ContactData>>();

				return groupedData;
			}catch(Exception e){
				Debug.WriteLine ("Couldn't load call list: {0}", e.Message);
				return null;
			}
		}
		public Playlist GetPlaylist (string name)
		{
			lock (locker) {
				return database.Table<Playlist> ().FirstOrDefault (x => x.PlaylistName == name);
			}
		}

		public void MarkForTodaysCalls(ContactData contact, bool yescall, DateTime date){
			if (yescall) {
				contact.OldPlaylist = Values.TODAYSCALLSUNDEFINED;
				//App.CapPage.refresh ();//doesnt work for some reason
			} else {
				contact.NextCall = date.Date;
				contact.OldPlaylist = Values.CALLTODAY;
				App.Database.UpdateItem (contact);
				Debug.WriteLine ("Will follow up {0} on {1}. date.Date param: {2}. OldPlaylist is {3}", 
					contact.Name, contact.NextCall, date.Date, contact.OldPlaylist);
			}
		}

		public int DeleteItem(int id)
		{
			lock (locker) {
				return database.Delete<Playlist> (id);
			}
		}
		public int SaveItem (ContactData item) 
		{
			try{
				item.Name = item.FirstName + " " + item.LastName;

				lock (locker) {
					return database.Insert(item);
				}
			}catch(Exception){
				return 0;
			}
		}

		public int SaveAll (IEnumerable<ContactData> items, bool savingFromPicture  = false) 
		{
			try{
				var itemsArr = items.ToArray ();
				if(savingFromPicture){
					for(int c = 0;c < itemsArr.Length;c++){
						itemsArr[c].Name = itemsArr[c].FirstName + " " + itemsArr[c].LastName;
					}
				}
				lock (locker) {
					return database.InsertAll(itemsArr.AsEnumerable ());
				}
			}catch(Exception e){
				Debug.WriteLine ("DB SAVEALL error "+e.Message);
				return 0;
			}
		}
		public int UpdateItem (ContactData item) 
		{
			try{
				item.Name = item.FirstName + " " + item.LastName;
				Debug.WriteLine("Update Item {0}", 
					item.Name);
					
				lock (locker) {
					return database.Update(item);
				}
			}catch(Exception){
				return 0;
			}
		}
		public int UpdateItem (Playlist item) 
		{
			try{
				Debug.WriteLine("Updating Namelist {0}", 
					item.PlaylistName);

				lock (locker) {
					return database.Update(item);
				}
			}catch(Exception){
				return 0;
			}
		}
		public int UpdateAll(IEnumerable<ContactData> items){
			try{
				lock (locker) {
					Debug.WriteLine("in UpdateAll locker");
					return database.UpdateAll(/*itemsArr.AsEnumerable ()*/items.AsEnumerable());
				}
			}catch(Exception e){
				Debug.WriteLine ("DB UpdateAll error "+e.Message);
				return 0;
			}
		}

		public async Task<int> UpdateAllAsync(IEnumerable<ContactData> items){
			try{
				lock (locker) {
					Debug.WriteLine("in UpdateAll locker");
					return database.UpdateAll(items);
				}
			}catch(Exception e){
				Debug.WriteLine ("DB UpdateAll error "+e.Message);
				return 0;
			}
		}

        public IEnumerable<Playlist> GetPlaylistItems()
        {
            Debug.WriteLine("in playlist GetItems()");
            int ctr = 0;
			List<Playlist> list = (from i in (database.Table<Playlist>().OrderBy(i => i.PlaylistName)) select i).ToList<Playlist>();
            foreach (Playlist c in list)
            {
                ctr++;
                Debug.WriteLine(c.PlaylistName);
				if (string.IsNullOrWhiteSpace (c.Icon)) {
					if (string.Equals (c.PlaylistName, Values.ALLPLAYLISTPARAM)) {
						c.Icon = "people.png";
					} else if(string.Equals(c.PlaylistName, Values.TODAYSCALLS)){
						c.Icon = "todo.png";
					}
				}
            }
            App.lastIndex = ctr + 1;
            Debug.WriteLine(App.lastIndex + ": lastIndex");
            return list;
        }
		public string[] GetPlaylistNames(){
			try{
				var playlists = GetPlaylistItems();
				Playlist[] playlistarr = playlists.ToArray();
				var pnames = new string[playlistarr.Length];
				for(int c = 0;c < playlistarr.Length;c++){
					pnames[c] = playlistarr[c].PlaylistName;
				}
				return pnames;
			}catch(Exception e){
				Debug.WriteLine ("GetPlaylistNames() error: {0}", e.Message);
			}
			return null;
		}
		public List<ContactData> GetSelectedItems(string playlist){
			var wholeList = GetItems (playlist).ToArray();
			List<ContactData> selectedItems = new List<ContactData> ();

			for (int c = 0; c < wholeList.Length; c++) {
				if (wholeList [c].IsSelected) {
					selectedItems.Add (wholeList [c]);
				}
			}

			return selectedItems;
		}
        public int DeletePlaylistItem(Playlist item)
        {
            if (string.Equals(Values.ALLPLAYLISTPARAM, item.PlaylistName) || string.Equals(Values.TODAYSCALLS, item.PlaylistName))
            {
                Debug.WriteLine("PlaylistDB.DeleteItem Can't delete this namelist: " + item.PlaylistName);
                return 0;
            }
            {
                return DeletePlaylistItemByID(item.ID);
            }

        }
        public int DeletePlaylistItemByID(int id)
        {
            lock (locker)
            {
                return database.Delete<Playlist>(id);
            }
        }

		public bool PlaylistNameAlreadyExists(string name){
			var playlistarr = GetPlaylistItems ().ToArray();
			for(int x = 0;x < playlistarr.Length;x++){
				if(string.Equals(playlistarr[x].PlaylistName, name)){
					return true;
				}
			}
			return false;
		}

        public int SavePlaylistItem(Playlist item)
        {
            Debug.WriteLine("playlist entered SaveItem()-----------------------------------------------------------------------------");
            lock (locker)
            {
                return database.Insert(item);
            }
        }
        public int UpdatePlaylistItem(Playlist item)
        {
            Debug.WriteLine("Playlist UpdateItem(): " + item.PlaylistName);
            lock (locker)
            {
                return database.Update(item);
            }
        }


		public async Task<int> DeselectAll(IEnumerable<ContactData> list, CAPPBase capp, bool refresh = true){
			ContactData[] arr = list.ToArray ();
			for(int c = 0;c < arr.Length;c++){
				arr [c].IsSelected = false;
			}
			var updateResult = App.Database.UpdateAll (arr.AsEnumerable ());
			if (refresh) {
				App.CapPage.refresh ();
			}
			return updateResult;
		}

		public void EnableAll(IEnumerable<ContactData> list, CAPPBase capp){
			ContactData[] arr = list.ToArray ();
			for(int c = 0;c < arr.Length;c++){
				arr [c].IsSelected = true;
			}
			App.Database.UpdateAll (arr.AsEnumerable ());
			capp.refresh ();
		}
    }
}

