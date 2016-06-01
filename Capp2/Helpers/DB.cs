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

		public DB () 
		{
			database = DependencyService.Get<ISQLite> ().GetConnectionCAPP ();
			database.CreateTable<ContactData>();
            database.CreateTable<Playlist>();
		}
		public async Task<IEnumerable<ContactData>> GetItemsAsync(string playlist){
			IEnumerable<ContactData> list = null;
			await Task.Run(() => {
				list = GetItems(playlist);
			});
			return list;
		}
		public IEnumerable<ContactData> GetItems (string playlist) {
			if (playlist == "All") {
				//dont choose which playlist, display all contacts including those marked as different playlists because "All" will be understood as a separate namelist
				list = (from x in (database.Table<ContactData> ().OrderBy (x => x.LastName))
					select x).ToList<ContactData> ();
				App.lastIndex = list.Count+1;
			} else {
				try{
					list = (from x in (database.Table<ContactData> ().Where(x => x.Playlist == playlist).OrderBy (x => x.LastName))
						select x).ToList<ContactData> ();
					App.lastIndex = list.Count+1;
				}catch(Exception e){
					Debug.WriteLine (e.Message+" --------------------------- EMPTYEXCEPTION");
					UserDialogs.Instance.Alert ("No contacts yet", "This namelist is still empty", "I'll add some in a bit");
					list = null;
				}
			}

			var listarr = list.ToArray();

			for(int c = 0;c < listarr.Length;c++){
				var firstinitial = listarr[c].FirstName.ToCharArray()[0];
				var secondinitial = listarr[c].LastName.ToCharArray()[0];
				listarr[c].Initials = firstinitial.ToString()+secondinitial.ToString();


				listarr[c].Number = App.contactFuncs.MakeDBContactCallable (listarr[c].Number, false);
				listarr[c].Number2 = App.contactFuncs.MakeDBContactCallable (listarr[c].Number2, false);
				listarr[c].Number3 = App.contactFuncs.MakeDBContactCallable (listarr[c].Number3, false);
				listarr[c].Number4 = App.contactFuncs.MakeDBContactCallable (listarr[c].Number4, false);
				listarr[c].Number5 = App.contactFuncs.MakeDBContactCallable (listarr[c].Number5, false);
			}

			/*foreach(ContactData c in list){
				c.Number = App.contactFuncs.MakeDBContactCallable (c.Number, false);
				c.Number2 = App.contactFuncs.MakeDBContactCallable (c.Number2, false);
				c.Number3 = App.contactFuncs.MakeDBContactCallable (c.Number3, false);
				c.Number4 = App.contactFuncs.MakeDBContactCallable (c.Number4, false);
				c.Number5 = App.contactFuncs.MakeDBContactCallable (c.Number5, false);
			}*/
			return list;
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

        /*public async Task<List<Grouping<string, ContactData>>> GetGroupedItemsFasterAsync (string playlist){
			List<Grouping<string, ContactData>> groupedList = new List<Grouping<string, ContactData>> ();
			await Task.Run ( async () => {
				groupedList = await GetGroupedItemsFaster (playlist);
			});
			return groupedList;
		}
		public async Task<List<Grouping<string, ContactData>>> GetGroupedItemsFaster (string playlist) {
			List<Grouping<string, ContactData>> groupedData = new List<Grouping<string, ContactData>> ();
			try{
				groupedData =
					(await GetItemsAsync(playlist))//.OrderBy(p => p.LastName)
						.GroupBy(p => p.LastName[0].ToString())
						.Select(p => new Grouping<string, ContactData>(p))
						.ToList();
				return groupedData;
			}catch(Exception){
				UserDialogs.Instance.ErrorToast ("Error", "Couldn't load call list");
				return null;
			}
		}*/
		public List<Grouping<string, ContactData>> GetGroupedItems (string playlist) {
			try{
				if (playlist == Values.ALLPLAYLISTPARAM) {
					//dont choose which playlist, display all contacts including those marked as different playlists because "All" will be understood as a separate namelist
					list = (from x in (database.Table<ContactData> ().OrderBy (x => x.LastName))
						select x).ToList<ContactData> ();
					App.lastIndex = list.Count+1;
				}else if(playlist == Values.TODAYSCALLS){
					//all contacts regardless of playlist, marked for callback today
					list = (from x in (database.Table<ContactData> ().Where (c => c.NextCall == DateTime.Now.Date).OrderBy (x => x.LastName))
						select x).ToList<ContactData> ();
					App.lastIndex = list.Count+1;
				} else {
					try{
						list = (from x in (database.Table<ContactData> ().Where(x => x.Playlist == playlist).OrderBy (x => x.LastName))
							select x).ToList<ContactData> ();
						App.lastIndex = list.Count+1;
					}catch(Exception e){
						Debug.WriteLine (e.Message+" --------------------------- EMPTYEXCEPTION");
						UserDialogs.Instance.Alert ("No contacts yet", "This namelist is still empty", "I'll add some in a bit");
						list = null;
					}
				}

				var listarr = list.ToArray();

				for(int c = 0;c < listarr.Length;c++){
					//list.ElementAt (c).Number = App.contactFuncs.MakeDBContactCallable(list.ElementAt (c).Number, false);
					listarr[c].Number = App.contactFuncs.MakeDBContactCallable(listarr[c].Number, false);
					var firstinitial = listarr[c].FirstName.ToCharArray()[0];
					var secondinitial = listarr[c].LastName.ToCharArray()[0];
					listarr[c].Initials = firstinitial.ToString()+secondinitial.ToString();

				}


				var groupedData =
					list.OrderBy(p => p.LastName)
						.GroupBy(p => p.LastName[0].ToString())
						.Select(p => new Grouping<string, ContactData>(p))
						.ToList<Grouping<string, ContactData>>();

				return groupedData;
			}catch(Exception e){
				Debug.WriteLine ("Couldn't load call list: {0}", e.Message);
				return null;
			}
		}
		/*public Playlist GetItem (int id)
		{
			lock (locker) {
				return database.Table<ContactData> ().FirstOrDefault (x => x.ID == id);
			}
		}*/
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
		public int SaveAll (IEnumerable<ContactData> items) 
		{
			try{
				var itemsArr = items.ToArray ();
				for(int c = 0;c < itemsArr.Length;c++){
					itemsArr[c].Name = itemsArr[c].FirstName + " " + itemsArr[c].LastName;
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
				Debug.WriteLine("Updated {0}: image: {1}", 
					item.Name, item.PicStringBase64);
					
				lock (locker) {
					return database.Update(item);
				}
			}catch(Exception){
				return 0;
			}
		}
		public int UpdateAll(IEnumerable<ContactData> items){
			try{
				var itemsArr = items.ToArray ();
				for(int c = 0;c < itemsArr.Length;c++){
					itemsArr[c].Name = itemsArr[c].FirstName + " " + itemsArr[c].LastName;
				}
				lock (locker) {
					return database.UpdateAll(itemsArr.AsEnumerable ());
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

		public async Task DeselectAll(IEnumerable<ContactData> list, CAPPBase capp){
			ContactData[] arr = list.ToArray ();
			for(int c = 0;c < arr.Length;c++){
				arr [c].IsSelected = false;
			}
			App.Database.UpdateAll (arr.AsEnumerable ());
			capp.refresh ();
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

