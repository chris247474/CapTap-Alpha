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
using Capp2.Helpers;
using System.Globalization;

namespace Capp2
{
	public class CAPP:CAPPBase
	{
		ContactData personCalled;
		string[] import;
		bool calling, AutoCallContinue;
		CameraViewModel cameraOps = null;
		int contactsCount = 0;
		Label lblContactsCount = null;
		public ToolbarItem EditTBI, DeleteTBI, MoveToTBI, AddTBI;
		string title;
		StackLayout stack = new StackLayout(), AutoCallStack = new StackLayout();
        List<Grouping<string, ContactData>> PreLoadedGroupedList;
		SearchBar searchBar;
		List<ContactData> AutoCallList;
		List<ContactData> PreloadedList;
		ScrollView scroller = new ScrollView();
		int AutoCallCounter;
		public bool AutoCalling;

		public CAPP (string playlistChosen, bool showtip = true)
		{
			Init (playlistChosen);

			SubscribeToMessagingCenterListeners ();

			CreateUIElements ();

			CreateLayouts ();

			PlayCappLoadAnimation ();

			ShowTipsIfFirstRun (showtip);

			AdHelper.AddGreenURLOrangeTitleBannerToStack (scroller.Content as StackLayout);
        }

		void Init(string playlistChosen){
			App.CapPage = this;
			App.CurrentNamelist = playlistChosen;
			PreLoadedGroupedList = App.Database.GetGroupedItems(playlistChosen);
			PreloadedList = App.Database.GetItems (playlistChosen);
			title = playlistChosen + " Contacts";
			this.playlist = playlistChosen;
			this.Title = title;
			cameraOps = new CameraViewModel();
		}

		async Task ShowTipsIfFirstRun(bool showtip){
			if (App.InTutorialMode && /*TutorialHelper.PrevPageIsPlaylistPage() &&*/ TutorialHelper.WelcomeTipDone) {
				//do not proceed, tutorial is already running
			}else if(!TutorialHelper.WelcomeTipDone){
				//start tutorial if not shown before
				if (!Settings.TutorialShownSettings) {
					Debug.WriteLine ("Tutorial shown: {0}", Settings.TutorialShownSettings);
					App.InTutorialMode = true;
					Settings.TutorialShownSettings = true;
					Debug.WriteLine ("Tutorial shown: {0}", Settings.TutorialShownSettings);

					if (showtip) {
						Debug.WriteLine ("created new Capp as modal");

						CappModal cappmodal = new CappModal (Values.ALLPLAYLISTPARAM, "", 
							PreLoadedGroupedList, PreloadedList.ToList<ContactData> (), 
							                      new List<ContactData> (), true);
						await TutorialHelper.ShowTip_Welcome (cappmodal, "Welcome to CapTap!!!", Color.FromHex (Values.CAPPTUTORIALCOLOR_Orange));
					}
				} else {
					Debug.WriteLine ("Tutorial already shown");
				}
			}
		}

		void PlayCappLoadAnimation(){
			UIAnimationHelper.FlyIn(searchBar, 600, App.AppJustLaunched);
			UIAnimationHelper.FlyIn(listView, 600, App.AppJustLaunched);
		}
		void CreateLayouts(){
			scroller = new ScrollView {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = //stack
					new StackLayout {
					Orientation = StackOrientation.Vertical,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Children = {
						UIBuilder.CreateEmptyStackSpace (),
						UIBuilder.CreateEmptyStackSpace (),
						new StackLayout{
							Padding = new Thickness(0, 10, 0, 0),
							Children = { searchBar }
						}, listView
					}
				},
			};

			if (Device.OS == TargetPlatform.iOS) {
				stack = new StackLayout()
				{
					BackgroundColor = Color.Transparent,//White,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Orientation = StackOrientation.Vertical,
					Children =
						{
							scroller
						}
					};
			} else if (Device.OS == TargetPlatform.Android) {
				stack = new StackLayout()
				{
					BackgroundColor = Color.Transparent,//White,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Orientation = StackOrientation.Vertical,
					Padding = new Thickness(7, 3, 7, 7),
					Children =
					{
						scroller
					}
					};
			}

			Content = UIBuilder.AddFloatingActionButtonToViewWrapWithRelativeLayout(stack, "", 
				new Command (async () =>
				{
					//CallHelper.AutoCall(this.playlist);
					AutoCall();
				}), Color.FromHex (Values.GOOGLEBLUE), Color.FromHex (Values.PURPLE));
		}

		async Task AddContacts(){
			import = Util.ImportChoices(playlist);
			var importResult = await this.DisplayActionSheet("Where do you want to add contacts from?", "Cancel", null, import);
			try{
				if (importResult == Values.IMPORTCHOICEMANUAL)
				{
					//await Navigation.PushModalAsync(new AddContactPage(this));
					await Navigation.PushAsync(new AddEditContactNativePage());
				}
				else if (importResult == Values.IMPORTCHOICEGDRIVE)
				{
					UserDialogs.Instance.InfoToast("Not yet implemented", null, 2000);
				}else if(string.Equals(importResult, "Cancel")){
					//do nothing
				}else
				{
					Debug.WriteLine("Importing from {0}", importResult);
					var list = App.Database.GetItems(importResult).ToList<ContactData>();
					if(list.Count == 0){
						AlertHelper.Alert(this, "Empty Namelist", importResult+" has no contacts", "OK");
					}else{
						await Navigation.PushModalAsync(new CappModal(importResult, 
							this.playlist, App.Database.GetGroupedItems(importResult), 
							list, App.Database.GetItems(this.playlist).ToList<ContactData>()));
					}
				}
			}catch(Exception){}
		}

		async void CreateUIElements(){
			this.BackgroundColor = Color.Transparent;
			if (Device.OS == TargetPlatform.iOS)
				App.NavPage.BackgroundColor = Color.FromHex (Values.GOOGLEBLUE);
			
			searchBar = new SearchBar {
				BackgroundColor = Color.Transparent,
				Placeholder = "Search for a name or number",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				FontSize = 15
				//CancelButtonColor = Color.FromHex(Values.GOOGLEBLUE),
			};
			searchBar.TextChanged += (sender, e) => {
				FilterCAPPContacts(searchBar.Text, playlist, PreLoadedGroupedList, PreloadedList);
			};
			searchBar.Focused += (object sender, FocusEventArgs e) => {
				Debug.WriteLine("SearchBar focused");
				//ReBuildGroupedSearchableListView(playlist, PreLoadedGroupedList, ListViewCachingStrategy.RecycleElement);
			};
			lblContactsCount = new Label{
				Text = App.Database.GetItems (playlist).Count.ToString()+" Contacts",
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				HeightRequest = 18
			};
			lblContactsCount.GestureRecognizers.Add (new TapGestureRecognizer{ Command = new Command (() => {
				UIAnimationHelper.ZoomUnZoomElement(lblContactsCount);
			}) });

			CreateTBIs ();

			CreateListView (ListViewCachingStrategy.RecycleElement);//Retain
		}

		public async Task AutoCall(){
			if (App.IsEditing) {
				await UpdateUI_EnableAutoCallingAfterEditing ();
			}

			PrepareForAutoCall ();

			if (App.Database.GetItems (playlist).Count () == 0) {
				AlertHelper.Alert (string.Format ("{0} has no contacts to call", playlist), "");
			} else {
				if (string.Equals (App.SettingsHelper.BOMLocationSettings, "<meetup here>")) {
					App.SettingsHelper.BOMLocationSettings = await Util.GetUserInputSingleLinePromptDialogue ("You haven't entered a meetup place in your meetup templates (go to Settings)", 
						"Enter a default meetup location, then try again", "<meetup here>");

					//DoAutoCall ();
					CheckToResumeCallingIfPlaylistCalledBeforeThenCall(playlist);
				} else {
					//DoAutoCall ();
					CheckToResumeCallingIfPlaylistCalledBeforeThenCall(playlist);
				}
			}
		}

		async Task DoAutoCall(){
			if(calling){
				SetupNotAutoCalling ();
			}else{
				SetupAutoCalling ();
				autoCall(this.playlist);
			}
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			scroller.VerticalOptions = LayoutOptions.FillAndExpand;
			stack.VerticalOptions = LayoutOptions.FillAndExpand;
			listView.VerticalOptions = LayoutOptions.FillAndExpand;

			TutorialHelper.ContinueCAPPTutorialIfNotDone (this);

			if (App.InTutorialMode && TutorialHelper.ReadyForExtraTips && TutorialHelper.HowToAddContactsDone) {
				Debug.WriteLine ("finishing tutorial mode. about to show extra tips");
				App.InTutorialMode = false;
				TutorialHelper.ShowExtraTips (this, Color.FromHex(Values.CAPPTUTORIALCOLOR_Purple));
			}
		}
		void CreateListView(ListViewCachingStrategy cachestrat = ListViewCachingStrategy.RecycleElement){

			BindingContext = new ObservableCollection<Grouping<string, ContactData>>(
				/*App.Database.GetGroupedItems (playlist)*/this.PreLoadedGroupedList);

			listView = new ListView (cachestrat)
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent,
				ItemsSource = PreLoadedGroupedList,
				SeparatorColor = Color.Transparent,
				ItemTemplate = new DataTemplate(() => {
					return new ContactViewCell (this);
				}),
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Binding("Key"),
				HasUnevenRows = true,
				GroupShortNameBinding = new Binding ("Key"),//doesnt work android, works iOS
				GroupHeaderTemplate = new DataTemplate (() => {
					return new HeaderCell ();
				}),
				Header = contactsCount,
				HeaderTemplate = new DataTemplate(() => {
					return new StackLayout{
						Children = {
							lblContactsCount
						}
					};
				}),
			};
			if (cachestrat == ListViewCachingStrategy.RecycleElement) {
				listView.HasUnevenRows = false;
				listView.RowHeight = 70;
			}
			listView.Focused += (object sender, FocusEventArgs e) => {
				scroller.ScrollToAsync(0, listView.Y, true);
			};
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
		}

		void CreateTBIs(){
			AddTBI = new ToolbarItem("Add", "", async () =>
				{
					TutorialHelper.RemoveHowToAddContactsTipIfNeeded(this);
					await AddContacts();
				});
			if (!string.Equals(this.playlist, Values.TODAYSCALLS)) {
				this.ToolbarItems.Add(AddTBI);
			}

			MoveToTBI = new ToolbarItem("Move To", "", async () =>
				{
					var enumerableList = App.Database.GetItems (this.playlist);
					var contactsArr = enumerableList.ToArray ();
					var saveList = new List<ContactData>();

					var MoveToResult = await this.DisplayActionSheet("Move to Namelist", "Cancel", null, PlaylistsIntoStringArr ());
					Debug.WriteLine ("MoveResult {0}, contactsArr.Length {1}",MoveToResult, contactsArr.Length);
					if (!string.Equals(MoveToResult, "Cancel")) {
						if (!string.IsNullOrWhiteSpace(MoveToResult)/* && !string.Equals(MoveToResult, "Cancel")*/)
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
							await App.Database.DeselectAll(saveList, this);
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
			DeleteTBI = new ToolbarItem("Delete", "", async () =>
				{
					var enumerableList = App.Database.GetItems (this.playlist);
					var contactsArr = enumerableList.ToArray ();
					var saveList = new List<ContactData>();

					for(int c = 0;c < contactsArr.Length;c++){
						if(contactsArr[c].IsSelected){
							Debug.WriteLine("Deleting {0} from {1}", contactsArr[c].Name, contactsArr[c].Playlist);
							contactsArr[c].Playlist = Values.ALLPLAYLISTPARAM;
							saveList.Add (contactsArr[c]);
						}
					}
					App.Database.UpdateAll (saveList.AsEnumerable ());
					await App.Database.DeselectAll (saveList, this);
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
			if (!string.Equals (this.playlist, Values.ALLPLAYLISTPARAM) && !string.Equals(this.playlist, Values.TODAYSCALLS)) {
				this.ToolbarItems.Add (EditTBI);
			}
		}

		async Task UpdateUI_DisableAutoCallingBeforeEditing(){
			//cmdAutocall.IsEnabled = false;
			//cmdAutocall.BackgroundColor = Color.Gray;
			App.IsEditing = true;
			EditTBI.Text = "Done";
			//EditTBI.Icon = "";
			this.Title = "Moving to namelist";
			Debug.WriteLine ("EDITING");
			MessagingCenter.Send(this, Values.ISEDITING);
			/*if (Device.OS == TargetPlatform.iOS)*/ ToolbarItems.Remove (AddTBI);
			ToolbarItems.Insert(0, MoveToTBI);
			if(!string.Equals (this.playlist, Values.ALLPLAYLISTPARAM) && !string.Equals (this.playlist, Values.TODAYSCALLS)) 
				ToolbarItems.Insert (1, DeleteTBI);
		}

		async Task UpdateUI_EnableAutoCallingAfterEditing(){
			//cmdAutocall.IsEnabled = true;
			//cmdAutocall.BackgroundColor = Color.Green;
			EditTBI.Text = "Edit";
			this.Title = title;
			App.IsEditing = false;
			MessagingCenter.Send(this, Values.DONEEDITING);
			App.Database.DeselectAll (App.Database.GetItems(this.playlist), this);
			if(!string.Equals (this.playlist, Values.ALLPLAYLISTPARAM) && !string.Equals (this.playlist, Values.TODAYSCALLS)) ToolbarItems.Remove (DeleteTBI);
			ToolbarItems.Remove (MoveToTBI);
			/*if(Device.OS == TargetPlatform.iOS)*/ ToolbarItems.Insert (0, AddTBI);
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

		public void SetupNotAutoCalling(){
			calling = false;
			AutoCallContinue = false;

			UpdateContactLastCalled (playlist);
			CallHelper.ShowUserYesCallTime (this, true);
		}

		public void UpdateContactLastCalled(string playlist){
			var playlistItem = CallHelper.GetPlaylistItemThenNullCheck (playlist);

			Debug.WriteLine ("assigning {0} as last index called, previous listindexcalled is {1}", 
				AutoCallCounter - 1, playlistItem.LastIndexCalled);

			if (AutoCallCounter < AutoCallList.Count) {
				playlistItem.LastIndexCalled = AutoCallCounter - 1;
			} else {
				playlistItem.LastIndexCalled = 0;
			}
			App.Database.UpdateItem (playlistItem);
		}

		public void SetupAutoCalling(){
			calling = true;
		}

		public async Task CheckToResumeCallingIfPlaylistCalledBeforeThenCall(string playlist){
			if (CallHelper.CanResume (playlist)) {
				var resume = await UserDialogs.Instance.ConfirmAsync ("Resume calling from where we left off?", 
					"We've called this list before", "Resume", "Start Over");
				if (resume) {
					//call from last index
					var lastindexcalled = CallHelper.GetPlaylistItemThenNullCheck (playlist).LastIndexCalled;
					Debug.WriteLine ("Setting AutoCallCounter from {0} to {1}", AutoCallCounter, lastindexcalled);
					AutoCallCounter = lastindexcalled;

					DoAutoCall ();
				} else {
					//start from first number in list
					DoAutoCall ();
				}
			} else {
				//list hasn't been called before, start from first number
				Debug.WriteLine("List {0} hasn't been called before, startin from index 0", playlist);
				DoAutoCall ();
			}
		}

		public void SubscribeToMessagingCenterListeners(){
			MessagingCenter.Subscribe<string>(this, Values.READYFOREXTRATIPS, async (args) =>{ 
				TutorialHelper.ReadyForExtraTips = true;
			});
			MessagingCenter.Subscribe<string>(this, Values.DONEADDINGCONTACT, async (args) =>{ 
				if(App.InTutorialMode && TutorialHelper.HowToAddContactsDone && !TutorialHelper.ReadyForAutoCallTipDone){
					if(App.Database.GetItems(playlist).Count() > 0){
						TutorialHelper.ReadyForAutoCallTip(this, Color.FromHex(Values.CAPPTUTORIALCOLOR_Purple));
					}else{
						await Task.Delay(2000);
						await AlertHelper.Alert("You didn't add any contacts. Let's try again", null);
						await AddContacts(); 
					}
				}
			});
			MessagingCenter.Subscribe<string>(this, Values.DONEWITHCALL, async (args) =>{ 
				try{
					NavigationHelper.ClearModals(this);
				}catch(Exception e){
					Debug.WriteLine("Popping Date and Template modals crashed: {0}", e.Message);
				}
				Debug.WriteLine ("CALL FINISHED");
				this.AutoCallContinue = true;
				Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
				StartContinueAutoCall ();
			});
			MessagingCenter.Subscribe<CAPP>(this, Values.DONEWITHSKIPCALL, async (args) =>{ 
				try{
					NavigationHelper.ClearModals(this);
				}catch(Exception e){
					Debug.WriteLine("DONEWITHNOCALL error: {0}",e.Message);
				}
				Debug.WriteLine ("SKIP-CALL FINISHED");
				this.AutoCallContinue = true;
				Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
				this.StartContinueAutoCall ();
			});
			MessagingCenter.Subscribe<TextTemplatePage>(this, Values.DONEWITHCALL, async (args) =>{ 
				try{
					NavigationHelper.ClearModals(this);
				}catch(Exception e){
					Debug.WriteLine("Popping Date and Template modals crashed: {0}", e.Message);
				}
				Debug.WriteLine ("CALL FINISHED");
				this.AutoCallContinue = true;
				Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
				 this.StartContinueAutoCall ();
			});
			MessagingCenter.Subscribe<string>(this, Values.iOSDONEWITHCALL, async (args) => {//if on iOS, DONEWITHCALL will be sent after Message viewcontroller presents SMS dialogue and receives "Send" command from user
				Debug.WriteLine("CALL FINISHED iOSDONEWITHCALL RECEIVED");
				try{
					NavigationHelper.ClearModals(this);
				}catch(Exception e){
					Debug.WriteLine("Popping Date and Template modals crashed: {0}", e.Message);
				}
				this.AutoCallContinue = true;
				Debug.WriteLine("CONTINUING TO NEXT NUMBER");
				 this.StartContinueAutoCall();
            });
            MessagingCenter.Subscribe<DatePage>(this, Values.DONEWITHNOCALL, async (args) =>{ 
				try{
					NavigationHelper.ClearModals(this);
				}catch(Exception e){
					Debug.WriteLine("DONEWITHNOCALL error: {0}",e.Message);
				}
				Debug.WriteLine ("NO-CALL FINISHED");
				this.AutoCallContinue = true;
				Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
				 this.StartContinueAutoCall ();
			});
			/*MessagingCenter.Subscribe<CAPP> (this, Values.ISEDITING, (args) => { 
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
			});*/
			MessagingCenter.Subscribe<string>(this, Values.DONEWAUTOCALLTIP, (args) =>{ 
				TutorialHelper.ShowTip_HowToGoBackToPlaylistPage(this, "Let's start by making a new namelist\nTap 'Namelists' up here", 
					Color.FromHex(Values.CAPPTUTORIALCOLOR_Blue));
			});
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
		}

		public async Task ReBuildGroupedSearchableListView(string playlist, List<Grouping<string, ContactData>> groupedList,
			ListViewCachingStrategy cachestrat = ListViewCachingStrategy.RecycleElement)
		{
			searchBar.Unfocus ();
			var listViewPosition = (scroller.Content as StackLayout).Children.IndexOf (listView); 

            try {
                this.IsBusy = true;
				(scroller.Content as StackLayout).Children.Remove(listView);

				CreateListView(cachestrat);

				(scroller.Content as StackLayout).Children.Insert(/*(scroller.Content as StackLayout).Children.Count - 1*/
					listViewPosition, listView);
                this.IsBusy = false;
            }
            catch (Exception e) {
                Debug.WriteLine("ReBuildGroupedSearchableListView() error: {0}", e.Message);
            }
		}

		public void refresh (ListView list, string playlist)
		{
			PreLoadedGroupedList = App.Database.GetGroupedItems(playlist);
			listView.ItemsSource = PreLoadedGroupedList;
			PreloadedList = App.Database.GetItems (playlist);
			contactsCount = PreloadedList.Count;
			lblContactsCount.Text = contactsCount.ToString()+" Contacts";
		}

		public void FilterCAPPContacts(string filter, string playlist, 
			List<Grouping<string, ContactData>> groupedList, IEnumerable<ContactData> PreloadedList)
        {
			App.UsingSearch = true;
            if (string.IsNullOrWhiteSpace(filter))
            {
				ReBuildGroupedSearchableListView(playlist, PreLoadedGroupedList, 
					ListViewCachingStrategy.RecycleElement);//Retain
            }
            else {
				Debug.WriteLine ("Searching");
                listView.BeginRefresh();

                listView.IsGroupingEnabled = false;
                listView.GroupDisplayBinding = new Binding(".");
                listView.GroupShortNameBinding = new Binding(".");
                listView.GroupHeaderTemplate = null;
				listView.ItemsSource = Util.FilterNameNumberOrg(PreloadedList, filter);
                
                listView.EndRefresh();
           }
			App.UsingSearch = false;
        }

		public void autoCall(string playlist){
			try{
				//PrepareForAutoCall ();
				StartContinueAutoCall ();
			}catch(Exception e){
				Debug.WriteLine ("PrepareForAutoCall() error: {0}", e.Message);
				UserDialogs.Instance.WarnToast ("Your phone may have randomly lagged. Please try again");
			}
		}
		void PrepareForAutoCall(){
			AutoCalling = true;
			AutoCallCounter = 0;
			AutoCallContinue = true;
			AutoCallList = App.Database.GetItems(playlist);
        }
		async Task StartContinueAutoCall(){
			if(AutoCallCounter >= AutoCallList.Count){
				AutoCalling = false;
				SetupNotAutoCalling ();
				Debug.WriteLine ("Autocalling done");
				NavigationHelper.ClearModals (this);
				MessagingCenter.Send ("", Values.READYFOREXTRATIPS);
			}
			if(AutoCallContinue && (AutoCallCounter < AutoCallList.Count)){
				var contactToCall = AutoCallList.ElementAt (AutoCallCounter);
				Debug.WriteLine ("ITERATION "+AutoCallCounter+" IN AUTOCALL, "+AutoCallList.Count+" Numbers in list");
				AutoCallContinue = false;
				Debug.WriteLine ("CONTINUE SET TO FALSE, WAITING FOR DONEWITHCALL MESSAGE");
				
				//if next call not set and not yet appointed, then call
				if (!contactToCall.IsAppointed && (!contactToCall.IsSetForNextCall || contactToCall.ShouldCallToday)) { 
					Debug.WriteLine ("{0} hasnt been appointed and hasn't been marked for a follow up call", contactToCall.Name);
					await CallHelper.call (contactToCall, true);

					AutoCallCounter++;	
					Debug.WriteLine ("AutoCallCounter after call is "+AutoCallCounter);	
				} else {
					string message = string.Empty;
					if (contactToCall.IsAppointed) {
						//await this.DisplayAlert ("Skipping...", , "OK");
						message = string.Format ("{0} was appointed for {1}\n", contactToCall.FirstName,
							contactToCall.Appointed.ToString ("MMMM dd, yyyy"));
					}
					if (contactToCall.IsSetForNextCall) {
						message += string.Format ("{0} is scheduled for call on {1}",
							contactToCall.FirstName, contactToCall.NextCall.ToString ("MMMM dd, yyyy"));
						
					}
					await this.DisplayAlert ("Skipping...", message, "OK");

					AutoCallCounter++;	
					Debug.WriteLine ("AutoCallCounter after call is "+AutoCallCounter);
					MessagingCenter.Send(this, Values.DONEWITHSKIPCALL);
				}
			}
			Debug.WriteLine ("EXITING WHILE CONTINUE");
		}
	}

	public class Grouping<S, T> : ObservableCollection<T>
	{
		private readonly S _key;
		private readonly T _contact;

		public Grouping(IGrouping<S, T> group)
			: base(group)
		{
			_key = group.Key;
		}

		public T Contact{
			get{ return _contact;}
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
			var title = new Label 
			{ 
				BackgroundColor = Color.Transparent,
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			}; 
			title.SetBinding(Label.TextProperty, "Key"); 
			View = new StackLayout 
			{ 
				BackgroundColor = Color.FromHex("E9E9E9"),
				HorizontalOptions = LayoutOptions.FillAndExpand, 
				VerticalOptions = LayoutOptions.Center,
				Padding = new Thickness(5, 0), 
				Orientation = StackOrientation.Horizontal, 
				Children = { title } 
			}; 
			this.View.BackgroundColor = Color.White;
			this.View.Opacity = 0.5;
			//title.HeightRequest = this.Height / 2;
			//this.Height = title.Height*1.5;
		} 
	}


}