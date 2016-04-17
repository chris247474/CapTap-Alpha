// To add offline sync support: add the NuGet package WindowsAzure.MobileServices.SQLiteStore
// to all projects in the solution and uncomment the symbol definition OFFLINE_SYNC_ENABLED
// For Xamarin.iOS, also edit AppDelegate.cs and uncomment the call to SQLitePCL.CurrentPlatform.Init()
// For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342 
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.WindowsAzure.MobileServices;
using Acr.UserDialogs;

//#if OFFLINE_SYNC_ENABLED
//using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
//using Microsoft.WindowsAzure.MobileServices.Sync;
//#endif

namespace Capp2
{
	public partial class AzureDataService
	{
		static AzureDataService defaultInstance = new AzureDataService();
		MobileServiceClient client;
		bool isOffline = false;

		IMobileServiceSyncTable<ContactDataItemAzure> postsTable;
		//IMobileServiceSyncTable<PlaylistItemAzure> playlistTable;

		public AzureDataService()
		{
			this.client = new MobileServiceClient(
				Values.ApplicationURL);

			var store = new MobileServiceSQLiteStore("local1.db");
			store.DefineTable<ContactDataItemAzure>();
			store.DefineTable<PlaylistItemAzure> ();

			//Initializes the SyncContext using the default IMobileServiceSyncHandler.
			this.client.SyncContext.InitializeAsync(store);

			this.postsTable = client.GetSyncTable<ContactDataItemAzure>();
			//this.playlistTable = client.GetSyncTable<PlaylistItemAzure>();

		}

		public static AzureDataService DefaultManager
		{
			get
			{
				return defaultInstance;
			}
			private set
			{
				defaultInstance = value;
			}
		}

		public MobileServiceClient CurrentClient
		{
			get { return client; }
		}

		public bool IsOfflineEnabled//add for playlist table?
		{
			get { return postsTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<ContactDataItemAzure>; }
		}
		public async Task<List<Grouping<string, ContactDataItemAzure>>> GetGroupedItems (string playlist) {
			List<Grouping<string, ContactDataItemAzure>> groupedData = new List<Grouping<string, ContactDataItemAzure>> ();

			try{
				groupedData =
					(await GetItems(playlist))//.OrderBy(p => p.LastName)
						.GroupBy(p => p.LastName[0].ToString())
						.Select(p => new Grouping<string, ContactDataItemAzure>(p))
						.ToList();
				return groupedData;
			}catch(Exception){
				UserDialogs.Instance.ErrorToast ("Error", "Couldn't load call list");
				return null; 
			}
		} 
		/*public async Task<List<PlaylistItemAzure>> GetPlaylistItems(bool syncItems = false)
		{
			Debug.WriteLine ("GetPlaylistItems here1");
			isOffline = syncItems;
			List<PlaylistItemAzure> list = new List<PlaylistItemAzure>();
			try
			{
				//#if OFFLINE_SYNC_ENABLED
				if (syncItems)
				{
					//await this.SyncAsync();
				}
				//#endif
				Debug.WriteLine ("GetPlaylistItems here2");

				list = await playlistTable.OrderBy (x => x.PlaylistName)
					.ToListAsync();
				Debug.WriteLine ("Got All Playlist Items");
				return list;
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"Invalid playlist sync operation: {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"playlist Sync error: {0}", e.Message);
			}
			return null;
		}*/
		public async Task<List<ContactDataItemAzure>> GetItems(string playlist, bool syncItems = false)
		{
			isOffline = syncItems;
			List<ContactDataItemAzure> list = new List<ContactDataItemAzure>();
			try
			{
				//#if OFFLINE_SYNC_ENABLED
				if (syncItems)
				{
					//await this.SyncAsync();
				}
				//#endif
				if (playlist == "All") {
					list = await postsTable.OrderBy (ContactDataItemAzure => ContactDataItemAzure.LastName)
						.ToListAsync();
					Debug.WriteLine ("Got All Items");
				}else{
					try{
						list = await postsTable
							.Where(ContactDataItemAzure => ContactDataItemAzure.Playlist == playlist).OrderBy (ContactDataItemAzure => ContactDataItemAzure.LastName)
							.ToListAsync();
					}catch(Exception e){
						Debug.WriteLine ("AzureDataService.GetCOntactDataAsync: "+e.Message);
						UserDialogs.Instance.Alert ("No contacts yet", "This namelist is still empty", "I'll add some in a bit");
						list = null;
					}
				}
				//return App.contactFuncs.MakeDBContactListCallable(list, true);
				return list;
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"Sync error: {0}", e.Message);
			}
			return null;
		}
		public async Task SaveAll(IEnumerable<ContactDataItemAzure> list){
			var arr = list.ToArray ();
			for(int c = 0;c < arr.Length;c++){
				await SaveItem (arr[c], false);
			}

			this.SyncAsync ();
		}
		public async Task UpdateAll(IEnumerable<ContactDataItemAzure> list){
			await SaveAll (list);
		}
		public async Task UpdateItem(ContactDataItemAzure item){
			await SaveItem (item, true);//fails to update if theres a previous transaction. usually calling sync right after UpdateItem syncs it fine
		}
		public async Task UpdateItemAvoidConflict(ContactDataItemAzure item){
			await SaveItem (item, true);
		}
		public async Task UpdateItem(ContactDataItemAzure item, bool singlesync){
			await SaveItem (item, singlesync);
		}
		public async Task SaveItem(ContactDataItemAzure item, bool singlesync)
		{
			try{
				item.Name = item.FirstName +" "+ item.LastName;
				Debug.WriteLine ("About to save/update "+item.Name+" with ID: "+item.ID);

				if (item.ID == null)
				{
					await postsTable.InsertAsync(item);
				}
				else
				{
					await postsTable.UpdateAsync(item);
				}

				if (singlesync)
				{
					await this.SyncAsync();
				}

			}catch(Exception e){
				Debug.WriteLine ("Azure save error: "+e.Message);
			}
		}
		/*public async Task SaveItem(PlaylistItemAzure item, bool singlesync)
		{
			try{
				Debug.WriteLine ("About to save/update "+item.PlaylistName+" with ID: "+item.ID);

				if (item.ID == null)
				{
					await playlistTable.InsertAsync(item);
				}
				else
				{
					await playlistTable.UpdateAsync(item);
				}

				if (singlesync)
				{
					await this.SyncAsync();
				}

			}catch(Exception e){
				Debug.WriteLine ("Azure save error: "+e.Message);
			}
		}*/
		/*public async Task UpdateItem(PlaylistItemAzure item){
			await SaveItem (item, true);//fails to update if theres a previous transaction. usually calling sync right after UpdateItem syncs it fine
		}*/
		/*public async Task DeleteItem(PlaylistItemAzure item){
			try{
				await playlistTable.DeleteAsync (item);
			}catch(Exception e){
				Debug.WriteLine ("Playlist delete error: "+e.Message);
			}
		}*/

		//#if OFFLINE_SYNC_ENABLED
		public async Task SyncAsync()
		{
		ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;
			Debug.WriteLine ("Syncing Azure DB w local");
			try
			{
				await this.client.SyncContext.PushAsync();

				await this.postsTable.PullAsync(
				//The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
				//Use a different query name for each unique query in your program
				"allContactDataItemAzures",
				this.postsTable.CreateQuery());

				/*await this.playlistTable.PullAsync(
					"allPlaylistItemAzures",
					this.playlistTable.CreateQuery());*/
			}
			catch (MobileServicePushFailedException exc)
			{
				if (exc.PushResult != null)
				{
					syncErrors = exc.PushResult.Errors;
				}
			}
			catch(Exception e){
		UserDialogs.Instance.WarnToast ("Please turn on your data to backup new contacts: "+e.Message);
			}

			// Simple error/conflict handling. A real application would handle the various errors like network conditions,
			// server conflicts and others via the IMobileServiceSyncHandler.
			if (syncErrors != null)
			{
				foreach (var error in syncErrors)
				{
					if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
					{
						//Update failed, reverting to server's copy.
						await error.CancelAndUpdateItemAsync(error.Result);
					}
					else
					{
						// Discard local change.
						await error.CancelAndDiscardItemAsync();
					}

					Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
				}
			}
		}
		//#endif
	}
}
