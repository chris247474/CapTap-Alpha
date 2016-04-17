using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using Plugin.Contacts;
using System.Threading.Tasks;
using Plugin.Contacts.Abstractions;
using System.Linq;
using System.Collections.ObjectModel;
using Acr.UserDialogs;
//using OpenCV.Core;
using XLabs.Forms.Controls;
using System.ComponentModel;

namespace Capp2
{
	public class CAPPCloudSync : ContentPage, INotifyPropertyChanged
	{
		ContactDataItemAzure personCalled;
		public ListView listView{ get; set;}
		string playlist;
		public PlaylistItemAzure playlistChosen{ get; set;}
		string[] import;
		bool calling;
		CameraViewModel cameraOps = null;
		int contactsCount = 0;
		Label lblContactsCount = null;
		Button cmdAutocall;
		ToolbarItem EditTBI, DeleteTBI, MoveToTBI;
		string title;
		//bool IsEditing;

		bool AutoCallContinue;
		int AutoCallCounter;
		List<ContactDataItemAzure> AutoCallList;

		public CAPPCloudSync (PlaylistItemAzure playlistChosen)
		{
			this.playlistChosen = playlistChosen;
			this.playlist = this.playlistChosen.PlaylistName;

			try{
				CreateUI (this.playlistChosen);
			}catch(Exception e){
				Debug.WriteLine ("CAPPCloudSync Constructor error: "+e.Message);
			}
		}
		async void CreateUI(PlaylistItemAzure playlistChosen){
			title = playlistChosen.PlaylistName + " Contacts";


			SubscribeForAutoCallListeners ();
			SubscribeForEditingListener ();

			this.Title = title;
			cameraOps = new CameraViewModel();

			SearchBar searchBar = new SearchBar {
				Placeholder = "Enter someone's name",
			};
			searchBar.TextChanged += (sender, e) => {
				FilterCAPPContacts(searchBar.Text, playlist);
			};
			lblContactsCount = new Label{
				Text = (await App.AzureDB.GetItems (playlist)).Count.ToString()+" Contacts",
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				HeightRequest = 18
			};
			Debug.WriteLine ("Contacts in playlist: "+(await App.AzureDB.GetItems (playlist)).Count);

			cmdAutocall = new Button { Text = "START CALLING", BackgroundColor = Color.Green };
			cmdAutocall.Clicked += (sender, e) => {
				if(calling){
					SetupNotAutoCalling ();
				}else{
					SetupAutoCalling ();
					autoCall(this.playlist);
				}
			};
			cmdAutocall.BackgroundColor = Color.Green;

			import = new string[]{ "Take a pic of a namelist", "Enter manually", "Load from an image" };
			var AddTBI = new ToolbarItem("Add", "", async () =>
				{
					var importResult = await this.DisplayActionSheet("Import Contacts", "OK", "Cancel", import);
					try{
						if(importResult == "Enter manually"){
							Navigation.PushAsync(new AddContactPageCloudSync (this));
						}else if(importResult == "Take a pic of a namelist"){
							var mediaFile = await cameraOps.TakePicture ();
							//App.contactFuncs.loadContactsFromPic(playlist, mediaFile.Source, this, false);	//test on camera
						}else if(importResult == "Load from an image"){
							GetImageImportToDB();
						}

					}catch(Exception){}
				});
			this.ToolbarItems.Add (AddTBI);

			MoveToTBI = new ToolbarItem("Move To", "", async () =>
				{
					var enumerableList = await App.AzureDB.GetItems (this.playlist);
					var contactsArr = enumerableList.ToArray ();
					var saveList = new List<ContactDataItemAzure>();

					var MoveToResult = await this.DisplayActionSheet("Move to Namelist", "OK", "Cancel", await PlaylistsIntoStringArr ());
					Debug.WriteLine ("MoveResult {0}, contactsArr.Length {1}",MoveToResult, contactsArr.Length);
					if(!string.IsNullOrWhiteSpace (MoveToResult)){
						Debug.WriteLine ("ABOUT TO LOOP");

						for(int c = 0;c < contactsArr.Length;c++){
							Debug.WriteLine ("Top of {0} loop: {1}", contactsArr[c].Name, contactsArr[c].IsSelected.ToString ());
							if(contactsArr[c].IsSelected){
								contactsArr[c].Playlist = MoveToResult;
								saveList.Add (contactsArr[c]);
								Debug.WriteLine (contactsArr[c].Name +" is being moved to "+MoveToResult);
							}
						}
						Debug.WriteLine ("ABOUT TO SAVE {0} CONTACTS", saveList.Count);
						App.AzureDB.UpdateAll (saveList.AsEnumerable ());
						DeselectAll (saveList);
						refresh ();
					}
				});
			DeleteTBI = new ToolbarItem("Delete", "", async () =>
				{
					var enumerableList = await App.AzureDB.GetItems (this.playlist);
					var contactsArr = enumerableList.ToArray ();
					var saveList = new List<ContactDataItemAzure>();

					for(int c = 0;c < contactsArr.Length;c++){
						if(contactsArr[c].IsSelected){
							contactsArr[c].Playlist = Values.ALLPLAYLISTPARAM;
							saveList.Add (contactsArr[c]);
						}
					}
					App.AzureDB.UpdateAll (saveList.AsEnumerable ());
					DeselectAll (saveList);
					refresh();
				});
			EditTBI = new ToolbarItem("Edit", "", async () =>
				{
					if(string.Equals(EditTBI.Text, "Edit")){
						cmdAutocall.IsEnabled = false;
						cmdAutocall.BackgroundColor = Color.Gray;
						App.IsEditing = true;
						EditTBI.Text = "Done";
						this.Title = "Tap a name to select it";
						Debug.WriteLine ("EDITING");
						MessagingCenter.Send(this, Values.ISEDITING);
						ToolbarItems.Remove (AddTBI);
						ToolbarItems.Insert(0, MoveToTBI);
						if(!string.Equals (this.playlist, Values.ALLPLAYLISTPARAM) && !string.Equals (this.playlist, Values.TODAYSCALLS)) ToolbarItems.Insert (1, DeleteTBI);

					}else{
						cmdAutocall.IsEnabled = true;
						cmdAutocall.BackgroundColor = Color.Green;
						EditTBI.Text = "Edit";
						this.Title = title;
						App.IsEditing = false;
						MessagingCenter.Send(this, Values.DONEEDITING);
						DeselectAll (await App.AzureDB.GetItems(this.playlist));
						if(!string.Equals (this.playlist, Values.ALLPLAYLISTPARAM) && !string.Equals (this.playlist, Values.TODAYSCALLS)) ToolbarItems.Remove (DeleteTBI);
						ToolbarItems.Remove (MoveToTBI);
						ToolbarItems.Insert (0, AddTBI);
					}
				});
			this.ToolbarItems.Add (EditTBI);

			BindingContext = new ObservableCollection<Grouping<string, ContactDataItemAzure>>(await App.AzureDB.GetGroupedItems (playlist));

			listView = new ListView ()
			{
				//ItemsSource = App.AzureDB.GetGroupedItems (playlist),//DONT DELETE THIS COMMENT
				ItemTemplate = new DataTemplate(() => {
					return new ContactViewCellCloudSync (this);
				}),
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Binding("Key"),
				HasUnevenRows = true,
				GroupShortNameBinding = new Binding ("Key"),//doesnt work android
				GroupHeaderTemplate = new DataTemplate (() => {
					return new HeaderCell ();
				})
			};

			refresh ();

			listView.ItemSelected += (sender, e) => {
				// has been set to null, do not 'process' tapped event
				if (e.SelectedItem == null)
					return; 

				personCalled = (ContactDataItemAzure)e.SelectedItem;

				if(!App.IsEditing){

					Navigation.PushAsync(new EditContactPageCloudSync(personCalled, this)); 

					// de-select the row
					((ListView)sender).SelectedItem = null; 
				}else{
					//personCalled.IsSelected = true;
					//Debug.WriteLine (personCalled.Name+"' selected value is "+personCalled.IsSelected.ToString ());


					((ListView)sender).SelectedItem = null; 
				}
			};

			Content =  new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness (7, 3, 7, 7),
				Children = 
				{
					searchBar, new StackLayout
					{
						Padding = new Thickness(7,0,7,0),
						Children = {cmdAutocall}
					}, lblContactsCount,listView
				}
				};
		}
		public async Task<string[]> PlaylistsIntoStringArr(){
			PlaylistItemAzure[] list = (await App.AzurePlaylistDB.GetPlaylistItems ()).ToArray ();
			List<string> finalList = new List<string> ();

			for(int c = 0;c < list.Length;c++){
				if(!string.Equals (list[c].PlaylistName, Values.ALLPLAYLISTPARAM) && !string.Equals(list[c].PlaylistName, Values.TODAYSCALLS)){
					finalList.Add (list [c].PlaylistName);
				}
			}
			return finalList.ToArray ();
		}
		public async void GetImageImportToDB(){
			try{
				await cameraOps.SelectPicture ();
				var mediaFile = cameraOps.mediaFileForTesseract;
				// uncomment App.contactFuncs.loadContactsFromPic(this.playlist, mediaFile.Source, this, true);
			}catch(Exception e){
				UserDialogs.Instance.WarnToast ("Oops something happened! Ok let's try that one more time");
			}
		}
		public void DeselectAll(IEnumerable<ContactDataItemAzure> list){
			ContactDataItemAzure[] arr = list.ToArray ();
			for(int c = 0;c < arr.Length;c++){
				arr [c].IsSelected = false;
			}
			App.AzureDB.UpdateAll (arr.AsEnumerable ());
			refresh ();
		}
		public void EnableAll(IEnumerable<ContactDataItemAzure> list){
			ContactDataItemAzure[] arr = list.ToArray ();
			for(int c = 0;c < arr.Length;c++){
				arr [c].IsSelected = true;
			}
			App.AzureDB.UpdateAll (arr.AsEnumerable ());
			refresh ();
		}
		public void SetupNotAutoCalling(){
			cmdAutocall.Text = "START CALLING";
			calling = false;
			AutoCallContinue = false;
			cmdAutocall.BackgroundColor = Color.Green;
		}
		public void SetupAutoCalling(){
			cmdAutocall.Text = "STOP CALLING";
			calling = true;

			cmdAutocall.BackgroundColor = Color.Red;
		}
		public void SubscribeForAutoCallListeners(){
			MessagingCenter.Subscribe<TextTemplatePage>(this, Values.DONEWITHCALL, (args) =>{ 
				Navigation.PopModalAsync ();//exception on lollipop
				Navigation.PopModalAsync ();

				Debug.WriteLine ("CALL FINISHED");
				AutoCallContinue = true;
				Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
				StartContinueAutoCall ();
			});
			MessagingCenter.Subscribe<DatePage>(this, Values.DONEWITHNOCALL, (args) =>{ 
				Navigation.PopModalAsync ();

				Debug.WriteLine ("NO-CALL FINISHED");
				AutoCallContinue = true;
				Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
				StartContinueAutoCall ();
			});
		}
		public void SubscribeForEditingListener(){
			MessagingCenter.Subscribe<CAPP> (this, Values.ISEDITING, (args) => { 
				Debug.WriteLine ("ISEDITING MESSAGE RECEIVED");
				listView.ItemTemplate = new DataTemplate(() => {
					return new ContactViewCellCloudSync (this);
				});
			});
			MessagingCenter.Subscribe<CAPP> (this, Values.DONEEDITING, (args) => { 
				Debug.WriteLine ("DONEEDITING MESSAGE RECEIVED");
				listView.ItemTemplate = new DataTemplate(() => {
					return new ContactViewCellCloudSync (this);
				});
			});
		}
		public async Task<ListView> BuildGroupedSearchableListView(string playlist, ListView listView){
			//listView.ItemsSource = App.AzureDB.GetGroupedItems (playlist);
			listView.ItemsSource = (await App.AzureDB.GetGroupedItems (playlist));
			listView.ItemTemplate = new DataTemplate (() => {
				return new ContactViewCellCloudSync (this);
			});
			listView.IsGroupingEnabled = true;
			listView.GroupDisplayBinding = new Binding ("Key");
			listView.HasUnevenRows = true;
			listView.GroupShortNameBinding = new Binding ("Key");
			listView.GroupHeaderTemplate = new DataTemplate (() => {
				return new HeaderCell ();
			});
			return listView;
		}


		public async void refresh ()
		{
			await App.AzureDB.SyncAsync ();
			listView.ItemsSource = await App.AzureDB.GetGroupedItems (playlist); 
			//contactsCount = list.Count;
			lblContactsCount.Text = (await App.AzureDB.GetItems (playlist)).Count.ToString()/*contactsCount.ToString()*/+" Contacts";
		}
		public async Task<bool> WaitableRefresh ()
		{
			await App.AzureDB.SyncAsync ();
			var list = await App.AzureDB.GetGroupedItems (playlist); 
			listView.ItemsSource = list;
			//contactsCount = list.Count;
			lblContactsCount.Text = list.Count.ToString()/*contactsCount.ToString()*/+" Contacts";

			return true;
		}

		public async void FilterCAPPContacts(string filter, string playlist)
		{
			listView.BeginRefresh ();

			if (string.IsNullOrWhiteSpace (filter)) {
				listView = await BuildGroupedSearchableListView (playlist, listView);
			} else {
				listView.ItemsSource = (await App.AzureDB.GetItems (this.playlist))
					.Where (x => x.Name.ToLower ().Contains (filter.ToLower () ) );
				listView.ItemTemplate = new DataTemplate (() => {
					return new ContactViewCellCloudSync (this);
				});
				listView.IsGroupingEnabled = false;
				listView.HasUnevenRows = false;
			}

			listView.EndRefresh ();
		}

		public void call(ContactDataItemAzure contact, bool autocall){
			Debug.WriteLine ("Calling "+contact.Name+" autocall: "+autocall.ToString ());
			var dialer = DependencyService.Get<IDialer> ();
			if (dialer != null) {
				//if (await dialer.Dial (Phone.ToNumber (person.Number))) {
				if (dialer.Dial (contact.Number).Result) {
					contact.Called = DateTime.Now;
					App.AzureDB.UpdateItem (contact);
					Navigation.PushModalAsync (new DatePageCloudSync (Values.APPOINTED, contact, autocall)); 
				} 
			}else
				throw new Exception ("dialer return null in CAPP.call()");
		}

		public void autoCall(string playlist){
			PrepareForAutoCall ();
			StartContinueAutoCall ();
		}
		async void PrepareForAutoCall(){
			AutoCallCounter = 0;
			AutoCallContinue = true;
			AutoCallList = await App.AzureDB.GetItems(playlist);//.ToList (); 
		}
		void StartContinueAutoCall(){
			if(AutoCallContinue && (AutoCallCounter < AutoCallList.Count)){
				Debug.WriteLine ("ITERATION "+AutoCallCounter+" IN AUTOCALL, "+AutoCallList.Count+" Numbers in list");
				AutoCallContinue = false;
				Debug.WriteLine ("CONTINUE SET TO FALSE, WAITING FOR DONEWITHCALL MESSAGE");
				call (AutoCallList.ElementAt (AutoCallCounter), true);
				Debug.WriteLine ("AutoCallCounter after call is "+AutoCallCounter);
				AutoCallCounter++;
			}
			Debug.WriteLine ("EXITING WHILE CONTINUE");

			if(AutoCallCounter >= AutoCallList.Count){
				SetupNotAutoCalling ();
			}
		}
	}
	/*public class Grouping<S, T> : ObservableCollection<T>
	{
		private readonly S _key;

		public Grouping(IGrouping<S, T> group)
			: base(group)
		{
			_key = group.Key;
		}

		public S Key
		{
			get { return _key; }
		}
	}

	public class HeaderCell : ViewCell 
	{ 
		public HeaderCell() 
		{ 
			this.Height = 27; 
			var title = new Label 
			{ 
				FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.White,
				VerticalOptions = LayoutOptions.Center 
			}; 
			title.SetBinding(Label.TextProperty, "Key"); 
			View = new StackLayout 
			{ 
				HorizontalOptions = LayoutOptions.FillAndExpand, 
				HeightRequest = 25, 
				//BackgroundColor = Color.FromRgb(52, 152, 218), 
				Padding = 5, 
				Orientation = StackOrientation.Horizontal, 
				Children = { title } 
			}; 
		} 
	}*/
}


