using System;
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
		Button AddTo = new Button ();

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
			ReBuildGroupedSearchableListView(playlist, groupedlist, stack);
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

					}), Color.FromHex (Values.GOOGLEBLUE), Color.FromHex (Values.PURPLE));
			}
		}

		public void SubscribeToMessagingCenter(){
			MessagingCenter.Subscribe<string>(this, Values.UNFOCUSPLAYLISTPAGESEARCHBAR, async (args) =>{ 
				searchBar.Unfocus();
			});
		}

		void CreateUIElements(){
			AddTo = new Button { 
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
			};


			searchBar = CappBuilder.CreateSearchBar ("Search", new Command (() => {
				FilterCAPPContacts(searchBar.Text, playlist, groupedlist, stack);
			}));
			//searchBar.Unfocused += (object sender, FocusEventArgs e) => {
			//};
			searchBar.Focused += (object sender, FocusEventArgs e) => {
				ReBuildGroupedSearchableListView(playlist, this.groupedlist, stack, ListViewCachingStrategy.RecycleElement);
			};

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
			App.UsingSearch = true;
			if (string.IsNullOrWhiteSpace(filter))
			{
				//ReBuildGroupedSearchableListView(playlist, groupedList, stack, ListViewCachingStrategy.RecycleElement);
				//searchBar.Unfocus();
				ReBuildGroupedSearchableListView(playlist, this.groupedlist, stack, ListViewCachingStrategy.RetainElement);

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
			StackLayout stack, ListViewCachingStrategy cachestrat = ListViewCachingStrategy.RetainElement)
		{
			searchBar.Unfocus();
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
				RowHeight = 60,
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
			if (cachestrat == ListViewCachingStrategy.RecycleElement) {
				listView.HasUnevenRows = false;
			}
		}
	}



	public class CappModalViewCell:ViewCell
	{
		Label nameLabel;
		CheckBox checkbox;
		ContactData personCalled;
		CircleImage ContactPic = null;

		public CappModalViewCell(string playlist){
			CreateUIElements ();
			View = CreateView (playlist);
		}

		View CreateView(string playlist){
			var layout = new RelativeLayout ();

			var content = UIBuilder.AddElementToObjectDependingOniOSAndAndroidListViewShortNameBinding (
				
				new StackLayout{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.StartAndExpand,
					Children = {ContactPic, nameLabel}
				},
				new StackLayout{
					HorizontalOptions = LayoutOptions.End,
					Children = {checkbox}
				}
			);

			var label = new Label{
				FontSize = nameLabel.FontSize,
				BackgroundColor = Color.Transparent,
				TextColor = Color.White,
				//FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center,
			};
			label.Opacity = label.Opacity / 1.5;
			label.SetBinding (Label.TextProperty, "Initials");

			layout.Children.Add (
				content,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
				heightConstraint: Constraint.RelativeToParent(parent => parent.Height)
			);

			layout.Children.Add(
				label,
				xConstraint: Constraint.RelativeToParent(parent => (ContactPic.Width*0.59)),
				yConstraint: Constraint.RelativeToParent(parent => ContactPic.Height*0.37)
			); 

			return layout;
		}
		void CreateUIElements(){
			//this.Height = 56;
			this.Height = RenderHeight*1.8;

			ContactPic = UIBuilder.CreateTappableCircleImage ("", LayoutOptions.CenterAndExpand, 
				Aspect.AspectFit, new Command (() => {
					
				}
			));
			ContactPic.SetBinding (CircleImage.SourceProperty, "PicStringBase64");

			nameLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
			};
			nameLabel.SetBinding(Label.TextProperty, "Name");//"Name" links directly to the ContactData.Name property

			checkbox = new CheckBox{ 
				HorizontalOptions = LayoutOptions.Center
			};
			checkbox.IsEnabled = true;
			checkbox.IsVisible = true;

			checkbox.SetBinding (CheckBox.CheckedProperty, "IsSelected");
			checkbox.CheckedChanged += (sender, e) => {
				personCalled = (sender as CheckBox).Parent.Parent.Parent.BindingContext as ContactData;
				if(personCalled.IsSelected){
					MessagingCenter.Send("", Values.UNFOCUSPLAYLISTPAGESEARCHBAR);
				}
				Debug.WriteLine (personCalled.Name+"' selected value is "+personCalled.IsSelected.ToString ());
				App.Database.UpdateItem(personCalled);
			};


		}
	}
}

