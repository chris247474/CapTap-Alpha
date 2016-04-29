using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Capp2
{
	public static class CappBuilder
	{
		public static void AddToolBarItem(Page page, string name, string icon, Command AddCommand){
			ToolbarItem AddTo = new ToolbarItem{Text = name, Icon = icon, Command = AddCommand};
			page.ToolbarItems.Add (AddTo);
		}

		public static StackLayout CreateCAPPContactList(Page page, SearchBar searchBar, ListView listView){
			Debug.WriteLine ("Entered CreateCAPPContactList()");
			StackLayout stack = null;

			if (Device.OS == TargetPlatform.iOS) {
				Debug.WriteLine ("Device is iOS");
				stack = new StackLayout()
				{
					BackgroundColor = Color.Transparent,
					Orientation = StackOrientation.Vertical,
					Children =
						{
							searchBar,
							listView
						}
					};
			} else if (Device.OS == TargetPlatform.Android) {
				stack = new StackLayout()
				{
					BackgroundColor = Color.Transparent,
					Orientation = StackOrientation.Vertical,
					Padding = new Thickness(7, 3, 7, 7),
					Children =
						{
							searchBar,
							listView
						}
					};
			}
			Debug.WriteLine ("returning stack for capp sheet");
			return stack;
		}

		public static ListView CreateGroupedListView<T>(Page page, List<T> itemssource, CappModalViewCell itemtemplate, Command OnItemSelected){
			ListView listView;

			listView = new ListView ()
			{
				BackgroundColor = Color.Transparent,
				ItemsSource = itemssource,
				SeparatorColor = Color.Transparent,
				ItemTemplate = new DataTemplate(() => {
					return itemtemplate;
				}),
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Binding("Key"),
				HasUnevenRows = true,
				GroupShortNameBinding = new Binding ("Key"),//doesnt work android, works iOS
				GroupHeaderTemplate = new DataTemplate (() => {
					return new HeaderCell ();
				})
			};

			listView.ItemSelected += (sender, e) => {
				OnItemSelected.Execute(null);

				((ListView)sender).SelectedItem = null;
			};

			return listView;
		}

		public static SearchBar CreateSearchBar(string placeholder, Command searchCommand){
			SearchBar searchBar = new SearchBar {
				BackgroundColor = Color.Transparent,
				Placeholder = placeholder,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			searchBar.TextChanged += (sender, e) => {searchCommand.Execute(null);};
			return searchBar;
		}

		public static Label CreateListViewItemCounter(string countLabel, double heightRequest = 18){
			Label counter = new Label{
				Text = countLabel,
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				HeightRequest = heightRequest
			};
			counter.GestureRecognizers.Add (new TapGestureRecognizer{ Command = new Command (() => {
				UIAnimationHelper.ZoomUnZoomElement(counter);
			}) });
		
			return counter;
		}
	}
}

