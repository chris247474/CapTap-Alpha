using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Diagnostics;
using System.Linq;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

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
				        select x).ToList ();
				App.lastIndex = list.Count+1;
			} else {
				try{
					list = (from x in (database.Table<ContactData> ().Where(x => x.Playlist == playlist).OrderBy (x => x.LastName))
						select x).ToList ();
					App.lastIndex = list.Count+1;
				}catch(Exception e){
					Debug.WriteLine (e.Message+" --------------------------- EMPTYEXCEPTION");
					UserDialogs.Instance.Alert ("No contacts yet", "This namelist is still empty", "I'll add some in a bit");
					list = null;
				}
			}
			foreach(ContactData c in list){
				c.Number = App.contactFuncs.MakeDBContactCallable (c.Number, false);
			}
			return list;
		}

		//public static Func<DataContext, string, IQueryable<ContactData>>
		//getCustomers= CompiledQuery.Compile((DataContext db, string strCustCode)=>  database.Table<ContactData> ().OrderBy (x => x.LastName));

		public async Task<List<Grouping<string, ContactData>>> GetGroupedItemsFasterAsync (string playlist){
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
		}
		public List<Grouping<string, ContactData>> GetGroupedItems (string playlist) {
			try{
				if (playlist == Values.ALLPLAYLISTPARAM) {
					//dont choose which playlist, display all contacts including those marked as different playlists because "All" will be understood as a separate namelist
					list = (from x in (database.Table<ContactData> ().OrderBy (x => x.LastName))
						select x).ToList ();
					App.lastIndex = list.Count+1;
				}else if(playlist == Values.TODAYSCALLS){
					//all contacts regardless of playlist, marked for callback today
					list = (from x in (database.Table<ContactData> ().Where (c => c.NextCall == DateTime.Now.Date).OrderBy (x => x.LastName))
						select x).ToList ();
					App.lastIndex = list.Count+1;
				} else {
					try{
						list = (from x in (database.Table<ContactData> ().Where(x => x.Playlist == playlist).OrderBy (x => x.LastName))
							select x).ToList ();
						App.lastIndex = list.Count+1;
					}catch(Exception e){
						Debug.WriteLine (e.Message+" --------------------------- EMPTYEXCEPTION");
						UserDialogs.Instance.Alert ("No contacts yet", "This namelist is still empty", "I'll add some in a bit");
						list = null;
					}
				}

				for(int c = 0;c < list.Count;c++){
					list.ElementAt (c).Number = App.contactFuncs.MakeDBContactCallable(list.ElementAt (c).Number, false);
				}

				var groupedData =
					list.OrderBy(p => p.LastName)
						.GroupBy(p => p.LastName[0].ToString())
						.Select(p => new Grouping<string, ContactData>(p))
						.ToList();

				return groupedData;
			}catch(Exception){
				UserDialogs.Instance.ErrorToast ("Error", "Couldn't load call list");
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
				//Debug.WriteLine("SaveItem(): "+item.Name+" Number: "+item.Number+" ExternalID:"+item.NextMeetingID+" Playlist:"+item.Playlist); 
				//item = App.AzureDB.ConvertToAzureDBItemThenSave (item);

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
				Debug.WriteLine("UpdateItem(): "+item.Name+" Number: "+item.Number+" ExternalID:"+item.NextMeetingID+" Playlist:"+item.Playlist); 
				//App.AzureDB.ConvertThenUpdateToAzureDB(item);

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
            List<Playlist> list = (from i in (database.Table<Playlist>().OrderBy(i => i.PlaylistName)) select i).ToList();
            foreach (Playlist c in list)
            {
                ctr++;
                Debug.WriteLine(c.PlaylistName);
            }
            App.lastIndex = ctr + 1;
            Debug.WriteLine(App.lastIndex + ": lastIndex");
            return list;
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
    }
}

