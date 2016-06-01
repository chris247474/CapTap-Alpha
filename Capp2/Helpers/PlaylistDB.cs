using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Capp2
{
	public class PlaylistDB
	{
		static object locker = new object ();
		SQLite.SQLiteConnection database; 

		public PlaylistDB ()
		{
			database = DependencyService.Get<ISQLite> ().GetConnectionPlaylists ();
			database.CreateTable<Playlist>(); 
		}
		public IEnumerable<Playlist> GetItems () {
			Debug.WriteLine ("in playlist GetItems()");
			int ctr = 0;
			List<Playlist> list = (from i in (database.Table<Playlist>().OrderBy(i => i.PlaylistName)) select i).ToList<Playlist>(); 
			foreach (Playlist c in list){
				ctr++; 
				Debug.WriteLine (c.PlaylistName);
			}
			App.lastIndex = ctr+1;
			Debug.WriteLine (App.lastIndex+": lastIndex");
			return list;
		}
		/*public Playlist GetItem (int id)
		{
			Debug.WriteLine("entered plyalist GetItem()-----------------------------------------------------------------------------");
			lock (locker) {
				return database.Table<Playlist> ().FirstOrDefault (x => x.ID == id);
			}
		}*/
		public int DeleteItem(Playlist item)
		{
			if(string.Equals(Values.ALLPLAYLISTPARAM, item.PlaylistName) || string.Equals(Values.TODAYSCALLS, item.PlaylistName)){
				Debug.WriteLine ("PlaylistDB.DeleteItem Can't delete this namelist: "+item.PlaylistName);
				return 0;
			}{
				return DeleteItem (item.ID);
			}

		}
		public int DeleteItem(int id)
		{
			lock (locker) {
				return database.Delete<Playlist> (id);
			}
		}
		public int SaveItem (Playlist item) 
		{
			Debug.WriteLine("playlist entered SaveItem()-----------------------------------------------------------------------------");
			lock (locker) {
				return database.Insert(item);
			}
		}
		public int UpdateItem (Playlist item) 
		{
			Debug.WriteLine("Playlist UpdateItem(): "+item.PlaylistName); 
			lock (locker) {
				return database.Update(item);
			}
		}
	}
}

