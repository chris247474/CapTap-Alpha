using System;
using System.ServiceModel.Channels;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Diagnostics;
using XLabs.Forms.Controls;
using System.Linq;
using System.Collections.Generic;

namespace Capp2
{
	public class CappModal:CAPPBase
	{
		SearchBar searchBar = null;
		//ListView listView = null;
		List<Grouping<string, ContactData>> groupedlist = new List<Grouping<string, ContactData>> ();
		List<ContactData> list, namelisttoaddto;
		//string playlist, 
		string playlistToAddTo;
		StackLayout stack = new StackLayout();
		Button AddTo = new Button ();

		public CappModal (string playlist, string playlistToAddTo, List<Grouping<string, ContactData>> groupedlist, List<ContactData> list, List<ContactData> NamelistToAddTo)
		{
			Debug.WriteLine ("Entered CappModal");
			this.playlist = playlist;
			this.playlistToAddTo = playlistToAddTo;
			this.groupedlist = groupedlist;
			this.list = list;
			this.namelisttoaddto = NamelistToAddTo;
			BindingContext = new ObservableCollection<Grouping<string, ContactData>>(groupedlist);

			CreateUIElements ();

			stack = CappBuilder.CreateCAPPContactList (this, 
				searchBar, 
				listView
			);

			Debug.WriteLine ("Created SearchBar and ListView");

			Content = new StackLayout{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(5),
				Children = {
					UIBuilder.CreateEmptyStackSpace(),
					UIBuilder.CreateModalXPopper("clear-Small.png", new Command(() => {
						App.Database.DeselectAll(this.list, this);
						Navigation.PopModalAsync();
					})),
					AddTo,
					stack
				}
			};

			//for some reason, all the contacts are initially shown w the same name. this will 'refresh' the list
			ReBuildGroupedSearchableListView(playlist, groupedlist, stack);
			Debug.WriteLine ("Finished constructing CappModal");
		}

		void CreateUIElements(){
			AddTo = new Button { 
				Text = "Add To '"+playlist+"'", 
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

				await App.Database.DeselectAll(this.namelisttoaddto, App.CapPage);//uncheck checkmarks that get copied to namelist

				await AlertHelper.Alert(this, "Copied!",
					string.Format("Moved {0} contacts from {1} to {2}", selectedItems.Length, playlist, playlistToAddTo)
				);

				Navigation.PopModalAsync();
			};


			searchBar = CappBuilder.CreateSearchBar ("Search", new Command (() => {
				FilterCAPPContacts(searchBar.Text, playlist, groupedlist, stack);
			}));

			listView = CappBuilder.CreateGroupedListView (this, groupedlist, new CappModalViewCell (playlist), new Command (() => {
				//dont do anything
			}));
		}

		void FilterCAPPContacts(string filter, string playlist, List<Grouping<string, ContactData>> groupedList, 
			StackLayout stack)
		{
			Debug.WriteLine("Filter called!");
			if (string.IsNullOrWhiteSpace(filter))
			{
				ReBuildGroupedSearchableListView(playlist, groupedList, stack);
			}
			else {
				listView.BeginRefresh();

				listView.IsGroupingEnabled = false;
				listView.GroupDisplayBinding = new Xamarin.Forms.Binding(".");
				listView.GroupShortNameBinding = new Xamarin.Forms.Binding(".");
				listView.GroupHeaderTemplate = null;
				listView.ItemsSource = Util.FilterNameNumberOrg(App.Database.GetItems(this.playlist), filter);

				listView.EndRefresh();
			}
		}
		public void ReBuildGroupedSearchableListView(string playlist, List<Grouping<string, ContactData>> groupedList, 
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
				/*listView.ItemSelected += (sender, e) => {
					// has been set to null, do not 'process' tapped event
					if (e.SelectedItem == null)
						return;
						
					((ListView)sender).SelectedItem = null;
				};*/
				stack.Children.Add(listView);
				this.IsBusy = false;
			}
			catch (Exception e) {
				Debug.WriteLine("ReBuildGroupedSearchableListView() error: {0}", e.Message);
			}
		}
	}



	public class CappModalViewCell:ViewCell
	{
		Label nameLabel;
		CheckBox checkbox;
		ContactData personCalled;

		public CappModalViewCell(string playlist){
			CreateUIElements ();
			View = CreateView (playlist);
		}

		View CreateView(string playlist){
			return UIBuilder.AddElementToObjectDependingOniOSAndAndroidListViewShortNameBinding (
				new StackLayout{
					HorizontalOptions = LayoutOptions.StartAndExpand,
					Children = {nameLabel}
				},
				new StackLayout{
					HorizontalOptions = LayoutOptions.End,
					Children = {checkbox}
				}
			);
		}
		void CreateUIElements(){
			this.Height = 56;
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
				Debug.WriteLine (personCalled.Name+"' selected value is "+personCalled.IsSelected.ToString ());
				App.Database.UpdateItem(personCalled);

				//var person = (from x in (App.Database.GetItems (Values.ALLPLAYLISTPARAM).Where(x => x.Name == personCalled.Name))
				//	select x);
				//Debug.WriteLine (person.ElementAtOrDefault (0).Name+"' selected value is "+person.ElementAtOrDefault (0).IsSelected.ToString ()); 
			};

		}
	}
}

