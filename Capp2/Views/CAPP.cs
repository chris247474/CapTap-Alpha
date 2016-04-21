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
using XLabs.Forms.Controls;
using System.ComponentModel;
using FAB.Forms;

namespace Capp2
{
	public class CAPP:ContentPage, INotifyPropertyChanged
	{
		ContactData personCalled;
		public ListView listView{ get; set;}
		string playlist;
		public Playlist playlistChosen{ get; set;}
		string[] import;
		bool calling;
		CameraViewModel cameraOps = null;
		int contactsCount = 0;
		Label lblContactsCount = null;
		Button cmdAutocall;
		ToolbarItem EditTBI, DeleteTBI, MoveToTBI, AddTBI;
		string title;
        StackLayout stack = new StackLayout();
        List<Grouping<string, ContactData>> PreLoadedGroupedList;
		SearchBar searchBar;
        
        bool AutoCallContinue;
		int AutoCallCounter;
		List<ContactData> AutoCallList;

		public CAPP (Playlist playlistChosen)
		{
            App.CapPage = this;
            PreLoadedGroupedList = App.Database.GetGroupedItems(playlistChosen.PlaylistName);
            UserDialogs.Instance.HideLoading();
            title = playlistChosen.PlaylistName + " Contacts";
			this.playlistChosen = playlistChosen;
			this.playlist = this.playlistChosen.PlaylistName;
			this.BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER);
			if (Device.OS == TargetPlatform.iOS)
				App.NavPage.BackgroundColor = Color.FromHex (Values.GOOGLEBLUE);

			SubscribeForAutoCallListeners ();
			SubscribeForEditingListener ();

			this.Title = title;
			cameraOps = new CameraViewModel();

			searchBar = new SearchBar {
				Placeholder = "Enter someone's name",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
			searchBar.TextChanged += (sender, e) => {
                FilterCAPPContacts(searchBar.Text, playlist, PreLoadedGroupedList);
            };
			lblContactsCount = new Label{
				Text = App.Database.GetItems (playlist).Count().ToString()+" Contacts",
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				HeightRequest = 18
			};
			cmdAutocall = new Button { Text = "START CALLING", BackgroundColor = Color.Green, TextColor = Color.Black, FontAttributes = FontAttributes.Bold };
			cmdAutocall.Clicked += (sender, e) => {
				if(calling){
					SetupNotAutoCalling ();
				}else{
					SetupAutoCalling ();
					autoCall(this.playlist);
				}
			};
			cmdAutocall.BackgroundColor = Color.Green;

			import = new string[]{ "Enter manually", "Load from Google Drive" };
			AddTBI = new ToolbarItem("Add", "", async () =>
			{
				var importResult = await this.DisplayActionSheet("Import Contacts", "OK", "Cancel", import);
				try{
                    if (importResult == "Enter manually")
                    {
                        await Navigation.PushAsync(new AddContactPage(this));
                    }
                    else if (importResult == "Load from Google Drive")
                    {
                        UserDialogs.Instance.InfoToast("Not yet implemented", null, 2000);
                    }

				}catch(Exception){}
			});
            if (Device.OS == TargetPlatform.iOS) {
                Debug.WriteLine("Adding add tbi temporarily");
                this.ToolbarItems.Add(AddTBI);
            }
			

			MoveToTBI = new ToolbarItem("Move To", "", async () =>
				{
					
					var enumerableList = App.Database.GetItems (this.playlist);
					var contactsArr = enumerableList.ToArray ();
					var saveList = new List<ContactData>();

					var MoveToResult = await this.DisplayActionSheet("Move to Namelist", "OK", "Cancel", PlaylistsIntoStringArr ());
					Debug.WriteLine ("MoveResult {0}, contactsArr.Length {1}",MoveToResult, contactsArr.Length);
                    if (!string.Equals(MoveToResult, "OK")) {
                        if (!string.IsNullOrWhiteSpace(MoveToResult) && !string.Equals(MoveToResult, "Cancel"))
                        {
                            Debug.WriteLine("ABOUT TO LOOP");

                            for (int c = 0; c < contactsArr.Length; c++)
                            {
                                Debug.WriteLine("Top of {0} loop: {1}", contactsArr[c].Name, contactsArr[c].IsSelected.ToString());
                                if (contactsArr[c].IsSelected)
                                {
                                    contactsArr[c].Playlist = MoveToResult;
                                    saveList.Add(contactsArr[c]);
                                    Debug.WriteLine(contactsArr[c].Name + " is being moved to " + MoveToResult);
                                }
                            }
                            Debug.WriteLine("ABOUT TO SAVE {0} CONTACTS", saveList.Count);
                            App.Database.UpdateAll(saveList.AsEnumerable());
                            DeselectAll(saveList);
                            refresh();

                            UserDialogs.Instance.ShowSuccess("Moved!", 2000);
                        }
                        else {
							UserDialogs.Instance.WarnToast("Oops! You didn't choose a new namelist. Please try again", null, 2000);
                        }
                    }
					ClearSearchBar();

					UpdateUI_EnableAutoCallingAfterEditing();
				});
			DeleteTBI = new ToolbarItem("Delete", "", () =>
				{
					var enumerableList = App.Database.GetItems (this.playlist);
					var contactsArr = enumerableList.ToArray ();
					var saveList = new List<ContactData>();

					for(int c = 0;c < contactsArr.Length;c++){
						if(contactsArr[c].IsSelected){
							contactsArr[c].Playlist = Values.ALLPLAYLISTPARAM;
							saveList.Add (contactsArr[c]);
						}
					}
					App.Database.UpdateAll (saveList.AsEnumerable ());
					DeselectAll (saveList);
					refresh();
				});
			EditTBI = new ToolbarItem("Edit", "", () =>
			{
				if(string.Equals(EditTBI.Text, "Edit")){
					UpdateUI_DisableAutoCallingBeforeEditing();
				}else{
					UpdateUI_EnableAutoCallingAfterEditing();
				}
			});
			this.ToolbarItems.Add (EditTBI);
			
			BindingContext = new ObservableCollection<Grouping<string, ContactData>>(App.Database.GetGroupedItems (playlist));

			listView = new ListView ()
			{
                ItemsSource = PreLoadedGroupedList,
                SeparatorColor = this.BackgroundColor,
                ItemTemplate = new DataTemplate(() => {
					return new ContactViewCell (this);
				}),
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Binding("Key"),
				HasUnevenRows = true,
				GroupShortNameBinding = new Binding ("Key"),//doesnt work android, works iOS
				GroupHeaderTemplate = new DataTemplate (() => {
					return new HeaderCell ();
				})
			};
            

			//refresh ();
			listView.ItemSelected += (sender, e) => {
				// has been set to null, do not 'process' tapped event
				if (e.SelectedItem == null)
					return; 

				personCalled = (ContactData)e.SelectedItem;
				
				if(!App.IsEditing){
					
					Navigation.PushAsync(new EditContactPage(personCalled, this));
					// de-select the row
					((ListView)sender).SelectedItem = null; 
				}else{
					((ListView)sender).SelectedItem = null; 
				}
			};

            if (Device.OS == TargetPlatform.iOS) {
                stack = new StackLayout()
                {
                    BackgroundColor = Color.White,
                    Orientation = StackOrientation.Vertical,
                    //Padding = new Thickness(0, 0, 10, 0),
                    Children =
                {
                    searchBar, new StackLayout
                    {
                        Padding = new Thickness(7,0,7,0),
                        Children = {cmdAutocall}
                    }, lblContactsCount,listView
                }
                };
            } else if (Device.OS == TargetPlatform.Android) {
                stack = new StackLayout()
                {
                    BackgroundColor = Color.White,
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(7, 3, 7, 7),
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

			Content = UIBuilder.AddFloatingActionButtonToStackLayout(stack, "ic_add_white_24dp.png", new Command (async () =>
				{
					import = new string[]{ "Enter manually", "Load from Google Drive" };
					var importResult = await this.DisplayActionSheet("Import Contacts", "OK", "Cancel", import);
					try{
						if(importResult == "Enter manually"){
							await Navigation.PushAsync(new AddContactPage (this));
						}else if(importResult == "Load from Google Drive"){
							UserDialogs.Instance.InfoToast ("Under construction");
						}
					}catch(Exception){}
				}), Color.FromHex (Values.GOOGLEBLUE), Color.FromHex (Values.PURPLE));

            
        }
		void UpdateUI_DisableAutoCallingBeforeEditing(){
			cmdAutocall.IsEnabled = false;
			cmdAutocall.BackgroundColor = Color.Gray;
			App.IsEditing = true;
			EditTBI.Text = "Done";
			//EditTBI.Icon = "";
			this.Title = "Moving to namelist";
			Debug.WriteLine ("EDITING");
			MessagingCenter.Send(this, Values.ISEDITING);
			if (Device.OS == TargetPlatform.iOS) ToolbarItems.Remove (AddTBI);
			ToolbarItems.Insert(0, MoveToTBI);
			if(!string.Equals (this.playlist, Values.ALLPLAYLISTPARAM) && !string.Equals (this.playlist, Values.TODAYSCALLS)) ToolbarItems.Insert (1, DeleteTBI);

		}
		void UpdateUI_EnableAutoCallingAfterEditing(){
			cmdAutocall.IsEnabled = true;
			cmdAutocall.BackgroundColor = Color.Green;
			EditTBI.Text = "Edit";
			this.Title = title;
			App.IsEditing = false;
			MessagingCenter.Send(this, Values.DONEEDITING);
			DeselectAll (App.Database.GetItems(this.playlist));
			if(!string.Equals (this.playlist, Values.ALLPLAYLISTPARAM) && !string.Equals (this.playlist, Values.TODAYSCALLS)) ToolbarItems.Remove (DeleteTBI);
			ToolbarItems.Remove (MoveToTBI);
			if(Device.OS == TargetPlatform.iOS) ToolbarItems.Insert (0, AddTBI);
		}
		void ClearSearchBar(){
			searchBar.Text = string.Empty;
			searchBar.Unfocus();
		}
		public string[] PlaylistsIntoStringArr(){
			Playlist[] list = App.Database.GetPlaylistItems().ToArray ();
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
				App.contactFuncs.loadContactsFromPic(this.playlist, mediaFile.Source, this, true);
			}catch(Exception){
				UserDialogs.Instance.WarnToast ("Oops something happened! Ok let's try that one more time");
			}
		}
		public void DeselectAll(IEnumerable<ContactData> list){
			ContactData[] arr = list.ToArray ();
			for(int c = 0;c < arr.Length;c++){
				arr [c].IsSelected = false;
			}
			App.Database.UpdateAll (arr.AsEnumerable ());
			refresh ();
		}
		public void EnableAll(IEnumerable<ContactData> list){
			ContactData[] arr = list.ToArray ();
			for(int c = 0;c < arr.Length;c++){
				arr [c].IsSelected = true;
			}
			App.Database.UpdateAll (arr.AsEnumerable ());
			refresh ();
		}
		public void SetupNotAutoCalling(){
			cmdAutocall.Text = "START CALLING";
			calling = false;
			AutoCallContinue = false;
            App.AutoCallStatus = false;
            cmdAutocall.BackgroundColor = Color.Green;
		}
		public void SetupAutoCalling(){
			cmdAutocall.Text = "STOP CALLING";
			calling = true;
            App.AutoCallStatus = true;
            cmdAutocall.BackgroundColor = Color.Red;
		}
		public void SubscribeForAutoCallListeners(){
            MessagingCenter.Subscribe<TextTemplatePage>(this, Values.DONEWITHCALL, (args) =>{ 
				try{
					Navigation.PopModalAsync();
					Navigation.PopModalAsync();
				}catch(Exception e){
					Debug.WriteLine("Popping Date and Template modals crashed: {0}", e.Message);
				}

				Debug.WriteLine ("CALL FINISHED");
				AutoCallContinue = true;
				Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
				StartContinueAutoCall ();
			});
			MessagingCenter.Subscribe<string>(this, Values.iOSDONEWITHCALL, (args) => {//if on iOS, DONEWITHCALL will be sent after Message viewcontroller presents SMS dialogue and receives "Send" command from user
				Debug.WriteLine("CALL FINISHED iOSDONEWITHCALL RECEIVED");
				try{
					NavigationHelper.ClearModals(this);
				
	                AutoCallContinue = true;
	                Debug.WriteLine("CONTINUING TO NEXT NUMBER");
	                StartContinueAutoCall();
				}catch(Exception e){
					Debug.WriteLine("Popping Date and Template modals crashed: {0}", e.Message);
				}
            });
            MessagingCenter.Subscribe<DatePage>(this, Values.DONEWITHNOCALL, (args) =>{ 
				try{
					Navigation.PopModalAsync ();

					Debug.WriteLine ("NO-CALL FINISHED");
					AutoCallContinue = true;
					Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
					StartContinueAutoCall ();
				}catch(Exception e){
					Debug.WriteLine("DONEWITHNOCALL error: {0}",e.Message);
				}
			});
		}
		public void SubscribeForEditingListener(){
			MessagingCenter.Subscribe<CAPP> (this, Values.ISEDITING, (args) => { 
				Debug.WriteLine ("ISEDITING MESSAGE RECEIVED");
				listView.ItemTemplate = new DataTemplate(() => {
					return new ContactViewCell (this);
				});
			});
			MessagingCenter.Subscribe<CAPP> (this, Values.DONEEDITING, (args) => { 
				Debug.WriteLine ("DONEEDITING MESSAGE RECEIVED");
				listView.ItemTemplate = new DataTemplate(() => {
					return new ContactViewCell (this);
				});
			});
		}
		public void ReBuildGroupedSearchableListView(string playlist, List<Grouping<string, ContactData>> groupedList){
            try {
                this.IsBusy = true;
                stack.Children.RemoveAt(3);
                listView = new ListView()
                {
                    ItemsSource = groupedList,
                    SeparatorColor = this.BackgroundColor,
                    ItemTemplate = new DataTemplate(() =>
                    {
                        return new ContactViewCell(this);
                    }),
                    IsGroupingEnabled = true,
                    GroupDisplayBinding = new Binding("Key"),
                    HasUnevenRows = true,
                    GroupShortNameBinding = new Binding("Key"),//doesnt work android, works iOS
                    GroupHeaderTemplate = new DataTemplate(() =>
                    {
                        return new HeaderCell();
                    })
                };
                listView.ItemSelected += (sender, e) => {
                    // has been set to null, do not 'process' tapped event
                    if (e.SelectedItem == null)
                        return;

                    personCalled = (ContactData)e.SelectedItem;

                    if (!App.IsEditing)
                    {

                        Navigation.PushAsync(new EditContactPage(personCalled, this));
                        // de-select the row
                        ((ListView)sender).SelectedItem = null;
                    }
                    else {
                        ((ListView)sender).SelectedItem = null;
                    }
                };
                stack.Children.Add(listView);
                this.IsBusy = false;
            }
            catch (Exception e) {
                Debug.WriteLine("ReBuildGroupedSearchableListView() error: {0}", e.Message);
            }
		}
        

		public void refresh ()
		{
			listView.ItemsSource = App.Database.GetGroupedItems(playlist);
			contactsCount = App.Database.GetItems (playlist).Count ();
			lblContactsCount.Text = contactsCount.ToString()+" Contacts";
		}

		public void FilterCAPPContacts(string filter, string playlist, List<Grouping<string, ContactData>> groupedList)
        {
            Debug.WriteLine("Filter called!");
            if (string.IsNullOrWhiteSpace(filter))
            {
                ReBuildGroupedSearchableListView(playlist, groupedList);
            }
            else {
                listView.BeginRefresh();

                listView.IsGroupingEnabled = false;
                listView.GroupDisplayBinding = new Binding(".");
                listView.GroupShortNameBinding = new Binding(".");
                listView.GroupHeaderTemplate = null;
                listView.ItemsSource = Util.FilterNameNumberOrg(App.Database.GetItems(this.playlist), filter);
                
                listView.EndRefresh();
            }
        }
       
		public async Task call(ContactData contact, bool autocall){
			Debug.WriteLine ("Calling "+contact.Name+" autocall: "+autocall.ToString ());
			var dialer = DependencyService.Get<IDialer> ();
			if (dialer != null) {
				if (await dialer.Dial (contact.Number)) {
					contact.Called = DateTime.Now;
					App.Database.UpdateItem (contact);
					Debug.WriteLine ("Delaying 4s");
					/*if (App.AutoCallStatus) {
						await Task.Delay (Values.CALLTOTEXTDELAY);
					}*/
					await Task.Delay (Values.CALLTOTEXTDELAY);
					Navigation.PushModalAsync (new DatePage (Values.APPOINTED, contact, autocall));
				} 
			}else
				throw new Exception ("dialer return null in CAPP.call()");
		}

		public void autoCall(string playlist){
			try{
				PrepareForAutoCall ();
				StartContinueAutoCall ();
			}catch(Exception e){
				Debug.WriteLine ("PrepareForAutoCall() error: {0}", e.Message);
				UserDialogs.Instance.WarnToast ("Your phone may have randomly lagged. Please try again");
			}
		}
		void PrepareForAutoCall(){
			AutoCallCounter = 0;
			AutoCallContinue = true;
			AutoCallList = App.Database.GetItems(playlist).ToList ();
        }
		void StartContinueAutoCall(){
			if(AutoCallCounter >= AutoCallList.Count){
				SetupNotAutoCalling ();

			}
			if(AutoCallContinue && (AutoCallCounter < AutoCallList.Count)){
				Debug.WriteLine ("ITERATION "+AutoCallCounter+" IN AUTOCALL, "+AutoCallList.Count+" Numbers in list");
				AutoCallContinue = false;
				Debug.WriteLine ("CONTINUE SET TO FALSE, WAITING FOR DONEWITHCALL MESSAGE");
				call (AutoCallList.ElementAt (AutoCallCounter), true);
				Debug.WriteLine ("AutoCallCounter after call is "+AutoCallCounter);
				AutoCallCounter++;
			}
			Debug.WriteLine ("EXITING WHILE CONTINUE");
		}

		protected override void OnAppearing(){
			Debug.WriteLine ("Appearing");
			/*if (Device.OS == TargetPlatform.Android) {
				App.NavPage.BarBackgroundColor = Color.FromHex (Values.PURPLE);
			} else if (Device.OS == TargetPlatform.iOS) {
				App.NavPage.BackgroundColor = Color.FromHex (Values.GOOGLEBLUE);
			}*/
		}
		protected override void OnDisappearing(){
			Debug.WriteLine ("OnDisappearing");
			App.NavPage.BarBackgroundColor = Color.FromHex (Values.PURPLE);
		}
	}
	public class Grouping<S, T> : ObservableCollection<T>
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
				TextColor = Color.Black,
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
	}


}