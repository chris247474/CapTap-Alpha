﻿using System;
using System.ServiceModel.Channels;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Diagnostics;
using XLabs.Forms.Controls;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capp2
{
	public class CappModal:CAPPBase
	{
		SearchBar searchBar = null;
		List<Grouping<string, ContactData>> groupedlist = new List<Grouping<string, ContactData>> ();
		List<ContactData> list, namelisttoaddto;
		//string playlist, 
		string playlistToAddTo;
		StackLayout stack = new StackLayout(), contentstack = new StackLayout();
		//Button AddTo = new Button ();
		ContactViewModel contactViewModel;

		public CappModal (string playlist, string playlistToAddTo, List<Grouping<string, ContactData>> groupedlist, 
			List<ContactData> list, List<ContactData> NamelistToAddTo, bool isToolTipHolder = false)
		{
			Debug.WriteLine ("Entered CappModal");
			this.playlist = playlist;
			this.playlistToAddTo = playlistToAddTo;
			this.groupedlist = groupedlist;
			this.list = list;
			this.namelisttoaddto = NamelistToAddTo;
			BindingContext = new ObservableCollection<Grouping<string, ContactData>>(groupedlist);

			CreateUIElements ();
			CreateLayouts (isToolTipHolder);

			//for some reason, all the contacts are initially shown w the same name. this will 'refresh' the list
			ReBuildGroupedSearchableListView(playlist, groupedlist, stack, 
				ListViewCachingStrategy.RecycleElement);//Retain
			SubscribeToMessagingCenter ();
		}

		void CreateLayouts(bool isToolTipHolder){
			stack = CappBuilder.CreateCAPPContactList (this, 
				searchBar, 
				listView
			);

			if (isToolTipHolder) {
				contentstack = new StackLayout{
					Orientation = StackOrientation.Vertical,
					Padding = new Thickness(5),
					Children = {
						UIBuilder.CreateEmptyStackSpace(),
						stack
					}
				};
				Content = UIBuilder.AddFloatingActionButtonToViewWrapWithRelativeLayout(contentstack, 
					"", new Command (async () => {}), Color.FromHex (Values.GOOGLEBLUE), Color.FromHex (Values.PURPLE));
			} else {
				contentstack = new StackLayout{
					Orientation = StackOrientation.Vertical,
					Padding = new Thickness(5),
					Children = {
						UIBuilder.CreateEmptyStackSpace(),
						UIBuilder.CreateModalXPopper(new Command(() => {
							App.Database.DeselectAll(this.list, this);
							MessagingCenter.Send("", Values.DONEADDINGCONTACT);
							Navigation.PopModalAsync();
						})),
						//AddTo,
						stack
					}
				};
				Content = UIBuilder.AddFloatingActionButtonToViewWrapWithRelativeLayout(contentstack, 
					"Checkmark.png", new Command (async () => {
						//ContactViewModel contactsViewModel;
						//add to namelist
						var selectedItems = App.Database.GetSelectedItems(playlist).ToArray();
						for(int c = 0;c < selectedItems.Length;c++){
							//contactsViewModel = new ContactViewModel(selectedItems[c]);
							selectedItems[c] = ContactViewModel.AddNamelist(selectedItems[c], new string[]{this.playlistToAddTo}, false);
							//selectedItems[c].Playlist = this.playlistToAddTo;//Add to Capp playlist where we came from
						}

						//save as new contacts to preserve other namelists that we're copying from? 
						//App.Database.SaveAll(selectedItems);//duplicates
						//var updateResult = await App.Database.UpdateAllAsync(selectedItems);

						var updateResult = await App.Database.DeselectAll(selectedItems.AsEnumerable(), 
							this, false);//uncheck checkmarks

						//avoid interrupting tutorial overlays
						if(App.InTutorialMode){
							Debug.WriteLine("intutorial mode - only refreshing");
							App.CapPage.refresh();
						}else{
							NavigationHelper.PopNavToRootThenOpenToCAPPInPlaylist(this.playlistToAddTo, 1000);
						}

						if(updateResult != 0){
							await AlertHelper.Alert(this, "Copied!",
								string.Format("Moved {0} contacts from {1} to {2}", selectedItems.Length, playlist, playlistToAddTo)
							);
						}else{
							Debug.WriteLine("result of update operation is {0}", updateResult);
							await AlertHelper.Alert(this, "Oops!",
								"Something went wrong. Please try again");
						}

						MessagingCenter.Send("", Values.DONEADDINGCONTACT);
						await Navigation.PopModalAsync();

					}), Color.FromHex (Values.GOOGLEBLUE), Color.FromHex (Values.PURPLE));
			}
		}

		public void SubscribeToMessagingCenter(){
			MessagingCenter.Subscribe<string>(this, Values.UNFOCUSPLAYLISTPAGESEARCHBAR, async (args) =>{ 
				searchBar.Unfocus();
			});
		}

		void CreateUIElements(){
			/*AddTo = new Button { 
				Text = "Add To '"+playlistToAddTo+"'", 
				BackgroundColor = Color.Green, 
				TextColor = Color.Black, 
				FontAttributes = FontAttributes.Bold 
			};
			AddTo.Clicked += async (sender, e) => {
				UIAnimationHelper.ShrinkUnshrinkElement(AddTo);
				//add to namelist
				var selectedItems = App.Database.GetSelectedItems(playlist).ToArray();
				for(int c = 0;c < selectedItems.Length;c++){
					selectedItems[c].Playlist = this.playlistToAddTo;//Add to Capp playlist where we came from
				}
				App.Database.SaveAll(selectedItems);//save as new contacts to preserve other namelists that we're copying from

				await App.Database.DeselectAll(this.list, this);//uncheck checkmarks

				await AlertHelper.Alert(this, "Copied!",
					string.Format("Moved {0} contacts from {1} to {2}", selectedItems.Length, playlist, playlistToAddTo)
				);

				MessagingCenter.Send("", Values.DONEADDINGCONTACT);
				Navigation.PopModalAsync();
			};*/


			searchBar = CappBuilder.CreateSearchBar ("Search", new Command (() => {
				FilterCAPPContacts(searchBar.Text, playlist, groupedlist, stack);
			}));
			//searchBar.Unfocused += (object sender, FocusEventArgs e) => {
			//};
			//searchBar.Focused += (object sender, FocusEventArgs e) => {
				//ReBuildGroupedSearchableListView(playlist, this.groupedlist, stack, ListViewCachingStrategy.RecycleElement);
			//};

			listView = CappBuilder.CreateGroupedListView (this, groupedlist, new CappModalViewCell (playlist), new Command (() => {
				//dont do anything
			}));
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			App.Database.DeselectAll(App.Database.GetItems(Values.ALLPLAYLISTPARAM), App.CapPage);
		}

		void FilterCAPPContacts(string filter, string playlist, List<Grouping<string, ContactData>> groupedList, 
			StackLayout stack)
		{
			//Debug.WriteLine ("Performing grouped filter search");
			//listView.BeginRefresh();

			App.UsingSearch = true;
			if (string.IsNullOrWhiteSpace(filter))
			{
				ReBuildGroupedSearchableListView(playlist, this.groupedlist, stack, 
					ListViewCachingStrategy.RecycleElement);//Retain
			}
			else {
				listView.BeginRefresh();

				listView.IsGroupingEnabled = false;
				listView.GroupDisplayBinding = new Xamarin.Forms.Binding(".");
				listView.GroupShortNameBinding = new Xamarin.Forms.Binding(".");
				listView.GroupHeaderTemplate = null;
				listView.ItemsSource = Util.FilterNameNumberOrg(this.list, filter);

				listView.EndRefresh();
			}
			App.UsingSearch = false;

			//listView.EndRefresh();
		}
		/*public void ReBuildGroupedSearchableListView(string playlist, List<Grouping<string, ContactData>> groupedList, 
			StackLayout stack)
		{
			try {
				this.IsBusy = true;
				stack.Children.RemoveAt(1);
				listView = new ListView()
				{
					BackgroundColor = Color.Transparent,
					ItemsSource = groupedList,
					SeparatorColor = Color.Transparent,
					ItemTemplate = new DataTemplate(() =>
						{
							return new CappModalViewCell(playlist);
						}),
					IsGroupingEnabled = true,
					GroupDisplayBinding = new Xamarin.Forms.Binding("Key"),
					HasUnevenRows = true,
					GroupShortNameBinding = new Xamarin.Forms.Binding("Key"),//doesnt work android, works iOS
					GroupHeaderTemplate = new DataTemplate(() =>
						{
							return new HeaderCell();
						})
				};
				stack.Children.Add(listView);
				this.IsBusy = false;
			}
			catch (Exception e) {
				Debug.WriteLine("ReBuildGroupedSearchableListView() error: {0}", e.Message);
			}
		}*/

		public async Task ReBuildGroupedSearchableListView(string playlist, List<Grouping<string, ContactData>> groupedList,
			StackLayout stack, ListViewCachingStrategy cachestrat = ListViewCachingStrategy.RecycleElement)
		{
			searchBar.Unfocus ();
			//var listViewPosition = stack.Children.IndexOf (listView); 

			try {
				this.IsBusy = true;
				stack.Children.Remove(stack.Children.Last());//RemoveAt(5);//use last instead?

				CreateListView(cachestrat, this.groupedlist);

				stack.Children.Add(listView);
				this.IsBusy = false;
			}
			catch (Exception e) {
				Debug.WriteLine("ReBuildGroupedSearchableListView() error: {0}", e.Message);
			}
		}

		public void refresh (ListView list, string playlist)
		{
			this.groupedlist = App.Database.GetGroupedItems(playlist);
			listView.ItemsSource = this.groupedlist;
		}

		void CreateListView(ListViewCachingStrategy cachestrat, List<Grouping<string, ContactData>> groupedList){
			listView = new ListView(cachestrat)
			{
				BackgroundColor = Color.Transparent,
				ItemsSource = groupedList,
				SeparatorColor = Color.Transparent,
				ItemTemplate = new DataTemplate(() =>
					{
						return new CappModalViewCell(playlist);
					}),
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Xamarin.Forms.Binding("Key"),
				HasUnevenRows = true,
				GroupShortNameBinding = new Xamarin.Forms.Binding("Key"),//doesnt work android, works iOS
				GroupHeaderTemplate = new DataTemplate(() =>
					{
						return new HeaderCell();
					})
			};
			/*if (cachestrat == ListViewCachingStrategy.RecycleElement) {
				listView.HasUnevenRows = false;
				listView.RowHeight = 70;
			}*/
			listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => { 
				if (e.SelectedItem == null)
					return;

				/*var personCalled = (ContactData)e.SelectedItem;
				personCalled.IsSelected = !personCalled.IsSelected;
				refresh();
				App.Database.UpdateItem(personCalled);*/

				((ListView)sender).SelectedItem = null;
			};
		}
	}



	public class CappModalViewCell:ViewCell
	{
		Label nameLabel;
		CheckBox checkbox;
		ContactData personCalled;
		CircleImage ContactPic = null;

		public CappModalViewCell(string playlist){
			//this.Height = 80;
			CreateUIElements ();
			View = CreateView (playlist);
		}

		View CreateView(string playlist){
			var layout = new RelativeLayout ();

			layout.HorizontalOptions = LayoutOptions.FillAndExpand;
			layout.Padding = new Thickness (15, 5, 5, 15);

			layout.Children.Add(
				ContactPic,
				Constraint.RelativeToParent(parent => parent.Width*0.039),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent => parent.Width*0.15)),
				Constraint.RelativeToParent((parent => parent.Height))
			); 

			layout.Children.Add(nameLabel,
				Constraint.RelativeToParent((parent => parent.X + ContactPic.Width*1.5)),
				Constraint.RelativeToParent((parent => parent.Height*0.25))
			);

			layout.Children.Add(checkbox,
				Constraint.RelativeToParent((parent => parent.Width*0.85)),
				Constraint.RelativeToParent((parent => parent.Height*0.3))
			);

			var initials = UIBuilder.CreateInitialsLabel (/*this.RenderHeight * 0.45*/Values.INITIALSFONTSIZE, "Initials");

			UIBuilder.AddInitialsToContactListItem (layout, initials, 0.039, ContactPic, 0.45);

			return layout;
		}
		void CreateUIElements(){
			this.Height = RenderHeight*1.8;

			ContactPic = UIBuilder.CreateTappableCircleImage ("", LayoutOptions.CenterAndExpand, 
				Aspect.AspectFit, new Command (() => {
					
				}
			));
			ContactPic.SetBinding (CircleImage.SourceProperty, "PicStringBase64");

			nameLabel = new Label{
				FontSize = Values.NAMEFONTSIZE,//Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
			};
			nameLabel.SetBinding(Label.TextProperty, "Name");//"Name" links directly to the ContactData.Name property

			checkbox = new CheckBox{ 
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			checkbox.IsEnabled = true;
			checkbox.IsVisible = true;

			checkbox.SetBinding (CheckBox.CheckedProperty, "IsSelected");
			checkbox.CheckedChanged += (sender, e) => {
				personCalled = (sender as CheckBox).Parent.Parent/*.Parent*/.BindingContext as ContactData;
				if(personCalled.IsSelected){
					MessagingCenter.Send("", Values.UNFOCUSPLAYLISTPAGESEARCHBAR);
				}
				Debug.WriteLine (personCalled.Name+"' selected value is "+personCalled.IsSelected.ToString ());
				App.Database.UpdateItem(personCalled);
			};


		}
		/*protected override void OnBindingContextChanged ()
		{
			base.OnBindingContextChanged ();
			var item = BindingContext as ContactData;

		}*/
	}
}

