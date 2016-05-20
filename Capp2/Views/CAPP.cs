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
	public class CAPP:CAPPBase
	{
		ContactData personCalled;
		//public ListView listView{ get; set;}
		//public string playlist;
		string[] import;
		bool calling, AutoCallContinue;
		CameraViewModel cameraOps = null;
		int contactsCount = 0, AutoCallCounter;
		Label lblContactsCount = null;
		Button cmdAutocall;
		public ToolbarItem EditTBI, DeleteTBI, MoveToTBI, AddTBI;
		string title;
		StackLayout stack = new StackLayout(), AutoCallStack = new StackLayout();
        List<Grouping<string, ContactData>> PreLoadedGroupedList;
		SearchBar searchBar;
		List<ContactData> AutoCallList;
		IEnumerable<ContactData> PreloadedList;
		ScrollView scroller = new ScrollView();

		public CAPP (string playlistChosen, bool showtip = true)
		{
			Init (playlistChosen);

			SubscribeToMessagingCenterListeners ();
			SubscribeForEditingListener ();

			CreateUIElements ();

			CreateLayouts ();

			PlayCappLoadAnimation ();

			ShowTipsIfFirstRun (showtip);
        }

		void Init(string playlistChosen){
			App.CapPage = this;
			PreLoadedGroupedList = App.Database.GetGroupedItems(playlistChosen);
			PreloadedList = App.Database.GetItems (playlistChosen);
			UserDialogs.Instance.HideLoading();
			title = playlistChosen + " Contacts";
			this.playlist = playlistChosen;
			this.Title = title;
			cameraOps = new CameraViewModel();
		}

		async Task ShowTipsIfFirstRun(bool showtip){
			App.InTutorialMode = true;

			if (showtip) {
				Debug.WriteLine ("created new Capp as modal");

				CappModal cappmodal = new CappModal (Values.ALLPLAYLISTPARAM, "", PreLoadedGroupedList, PreloadedList.ToList(), 
					new List<ContactData>(), true);
				await TutorialHelper.ShowTip_Welcome (cappmodal, "Welcome to CapTap!!!", Color.FromHex (Values.CAPPTUTORIALCOLOR_Orange));
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

			Content = UIBuilder.AddFloatingActionButtonToViewWrapWithRelativeLayout(stack, "", new Command (async () =>
				{
					AutoCall();
				}), Color.FromHex (Values.GOOGLEBLUE), Color.FromHex (Values.PURPLE));
		}

		async void AddContacts(){
			import = Util.ImportChoices(playlist);
			var importResult = await this.DisplayActionSheet("Choose a source to get contacts from", "Cancel", null, import);
			try{
				if (importResult == Values.IMPORTCHOICEMANUAL)
				{
					await Navigation.PushAsync(new AddContactPage(this));
				}
				else if (importResult == Values.IMPORTCHOICEGDRIVE)
				{
					UserDialogs.Instance.InfoToast("Not yet implemented", null, 2000);
				}else if(string.Equals(importResult, "Cancel")){
					//do nothing
				}else
				{
					Debug.WriteLine("Importing from {0}", importResult);
					var list = App.Database.GetItems(importResult).ToList();
					if(list.Count == 0){
						AlertHelper.Alert(this, "Empty Namelist", importResult+" has no contacts", "OK");
					}else{
						await Navigation.PushModalAsync(new CappModal(importResult, this.playlist, App.Database.GetGroupedItems(importResult), 
							list, App.Database.GetItems(this.playlist).ToList()));
						
					}
				}

			}catch(Exception){}


		}

		void CreateUIElements(){
			
			this.BackgroundColor = Color.Transparent;
			if (Device.OS == TargetPlatform.iOS)
				App.NavPage.BackgroundColor = Color.FromHex (Values.GOOGLEBLUE);
			
			searchBar = new SearchBar {
				BackgroundColor = Color.Transparent,
				Placeholder = "Search for a name or number",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			searchBar.TextChanged += (sender, e) => {
				FilterCAPPContacts(searchBar.Text, playlist, PreLoadedGroupedList, PreloadedList);
			};
			lblContactsCount = new Label{
				Text = App.Database.GetItems (playlist).Count().ToString()+" Contacts",
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				HeightRequest = 18
			};
			lblContactsCount.GestureRecognizers.Add (new TapGestureRecognizer{ Command = new Command (() => {
				UIAnimationHelper.ZoomUnZoomElement(lblContactsCount);
			}) });
			cmdAutocall = new Button { Text = "START CALLING", BackgroundColor = Color.Green, TextColor = Color.Black, FontAttributes = FontAttributes.Bold };
			cmdAutocall.Clicked += (sender, e) => {
				UIAnimationHelper.ShrinkUnshrinkElement(cmdAutocall);
				AutoCall();
			};
			cmdAutocall.BackgroundColor = Color.Green;

			CreateTBIs ();
			CreateListView ();
		}

		async Task AutoCall(){
			if (App.Database.GetItems (playlist).Count () == 0) {
				AlertHelper.Alert (string.Format ("{0} has no contacts to call", playlist), "");
			} else {
				if (string.Equals (App.SettingsHelper.BOMLocationSettings, "<meetup here>")) {
					App.SettingsHelper.BOMLocationSettings = await Util.GetUserInputSingleLinePromptDialogue ("You haven't entered a meetup place in your meetup templates (go to Settings)", 
						"Enter a default meetup location, then try again", "<meetup here>");

					DoAutoCall ();
				} else {
					DoAutoCall ();
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

			if (App.InTutorialMode) {
				TutorialHelper.HowToAddNumbers (this, "Now let's add some contacts so we can try out AutoCall\n" +
					"Just tap '+' up there", 
					Color.FromHex (Values.CAPPTUTORIALCOLOR_Orange));
			}
		}

		void CreateListView(){
			BindingContext = new ObservableCollection<Grouping<string, ContactData>>(App.Database.GetGroupedItems (playlist));

			listView = new ListView (/*ListViewCachingStrategy.RecycleElement*/)
			{
				RowHeight = 60,
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
				})
			};
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
			
			import = Util.ImportChoices(playlist);
			AddTBI = new ToolbarItem("Add", "", async () =>
				{
					AddContacts();
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
							App.Database.DeselectAll(saveList, this);
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
					App.Database.DeselectAll (saveList, this);
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
		}

		async Task UpdateUI_DisableAutoCallingBeforeEditing(){
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

		async Task UpdateUI_EnableAutoCallingAfterEditing(){
			cmdAutocall.IsEnabled = true;
			cmdAutocall.BackgroundColor = Color.Green;
			EditTBI.Text = "Edit";
			this.Title = title;
			App.IsEditing = false;
			MessagingCenter.Send(this, Values.DONEEDITING);
			App.Database.DeselectAll (App.Database.GetItems(this.playlist), this);
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



		public void SetupNotAutoCalling(){
			cmdAutocall.Text = "START CALLING";
			calling = false;
			AutoCallContinue = false;
            App.AutoCallStatus = false;
            cmdAutocall.BackgroundColor = Color.Green;

			CallHelper.ShowUserYesCallTime (this, true);
		}

		public void SetupAutoCalling(){
			cmdAutocall.Text = "STOP CALLING";
			calling = true;
            App.AutoCallStatus = true;
            cmdAutocall.BackgroundColor = Color.Red;
		}

		public void SubscribeToMessagingCenterListeners(){
			MessagingCenter.Subscribe<string>(this, Values.DONEWITHCALL, (args) =>{ 
				try{
					NavigationHelper.ClearModals(this);
				}catch(Exception e){
					Debug.WriteLine("Popping Date and Template modals crashed: {0}", e.Message);
				}

				Debug.WriteLine ("CALL FINISHED");
				AutoCallContinue = true;
				Debug.WriteLine ("CONTINUING TO NEXT NUMBER");
				StartContinueAutoCall ();
			});
			MessagingCenter.Subscribe<TextTemplatePage>(this, Values.DONEWITHCALL, (args) =>{ 
				try{
					NavigationHelper.ClearModals(this);
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

			MessagingCenter.Subscribe<string>(this, Values.DONEWAUTOCALLTIP, (args) =>{ 
				TutorialHelper.ShowTip_HowToGoBackToPlaylistPage(this, "Let's start by making a new namelist", 
					Color.FromHex(Values.CAPPTUTORIALCOLOR_Blue));
			});
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			//remove tutorial view
		}

		public void SubscribeForEditingListener(){
			
		}

		public void ReBuildGroupedSearchableListView(string playlist, List<Grouping<string, ContactData>> groupedList){
            try {
                this.IsBusy = true;
				(scroller.Content as StackLayout).Children.Remove((scroller.Content as StackLayout).Children.Last());//RemoveAt(5);//use last instead?
				CreateListView();
				(scroller.Content as StackLayout).Children.Add(listView);
                this.IsBusy = false;
            }
            catch (Exception e) {
                Debug.WriteLine("ReBuildGroupedSearchableListView() error: {0}", e.Message);
            }
		}

		public void refresh (ListView list, string playlist)
		{
			listView.ItemsSource = App.Database.GetGroupedItems(playlist);
			contactsCount = App.Database.GetItems (playlist).Count ();
			lblContactsCount.Text = contactsCount.ToString()+" Contacts";
		}

		public void FilterCAPPContacts(string filter, string playlist, 
			List<Grouping<string, ContactData>> groupedList, IEnumerable<ContactData> PreloadedList)
        {
            Debug.WriteLine("Filter called!");
            if (string.IsNullOrWhiteSpace(filter))
            {
                ReBuildGroupedSearchableListView(playlist, groupedList);
				//UIAnimationHelper.FlyDown (listView, 1000);
            }
            else {
                listView.BeginRefresh();

                listView.IsGroupingEnabled = false;
                listView.GroupDisplayBinding = new Binding(".");
                listView.GroupShortNameBinding = new Binding(".");
                listView.GroupHeaderTemplate = null;
				listView.ItemsSource = Util.FilterNameNumberOrg(PreloadedList, filter);
                
                listView.EndRefresh();
            }
        }

		/*async Task<string> HandleMutlipleNumbers(ContactData contact){
			List<string> list = new List<string> ();

			if (!string.IsNullOrWhiteSpace (contact.Number2)) {
				list.Add (contact.Number);
				list.Add (contact.Number2);
				if (!string.IsNullOrWhiteSpace (contact.Number3)) {
					list.Add (contact.Number3);
				}
				if (!string.IsNullOrWhiteSpace (contact.Number4)) {
					list.Add (contact.Number4);
				}
				if (!string.IsNullOrWhiteSpace (contact.Number5)) {
					list.Add (contact.Number5);
				}

				return await this.DisplayActionSheet ("Which number do we call?", null, null,
					list.ToArray ()
				);
			} 
			return contact.Number;
		}

		public async Task CallContact(ContactData contact, bool autocall){
			//to time call cycle of user
			if (!CallHelper.IsTimerRunning ()) {
				CallHelper.StartTimer ();
				Debug.WriteLine ("Timer not running, calling start timer");
			}

			Debug.WriteLine ("Calling " + contact.Name + " autocall: " + autocall.ToString ());
			var dialer = DependencyService.Get<IDialer> ();
			if (dialer != null) {
				if (await dialer.Dial (await HandleMutlipleNumbers (contact))) {
					contact.Called = DateTime.Now;
					App.Database.UpdateItem (contact);
					Debug.WriteLine ("Delaying 4s");
					await Task.Delay (Values.CALLTOTEXTDELAY);
					Navigation.PushModalAsync (new DatePage (Values.APPOINTED, contact, autocall));
				} 
			} else
				throw new Exception ("dialer return null in CAPP.call()");
		}*/

		/*public async Task call(ContactData contact, bool autocall){
			if (string.Equals (App.SettingsHelper.BOMLocationSettings, "<meetup here>")) {
				App.SettingsHelper.BOMLocationSettings = await Util.GetUserInputSingleLinePromptDialogue ("You haven't entered a meetup place in your meetup templates (go to Settings)", 
					"Enter a default meetup location, then try again", "<meetup here>");

				CallContact (contact, autocall);
			} else {
				CallContact (contact, autocall);
			}
		}*/

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
				CallHelper.call (AutoCallList.ElementAt (AutoCallCounter), true);
				Debug.WriteLine ("AutoCallCounter after call is "+AutoCallCounter);
				AutoCallCounter++;
			}
			Debug.WriteLine ("EXITING WHILE CONTINUE");
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
				BackgroundColor = Color.Transparent,
				FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.Black,
				VerticalOptions = LayoutOptions.Center 
			}; 
			title.SetBinding(Label.TextProperty, "Key"); 
			View = new StackLayout 
			{ 
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand, 
				HeightRequest = 25, 
				//BackgroundColor = Color.FromRgb(52, 152, 218), 
				Padding = 5, 
				Orientation = StackOrientation.Horizontal, 
				Children = { title } 
			}; 
			this.View.BackgroundColor = Color.Transparent;
		} 
	}


}