using System;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Plugin.Calendars.Abstractions;
using System.Threading.Tasks;

namespace Capp2
{
	public class PlaylistPageCloudSync : ContentPage
	{
		public ListView listView{ get; set;}
		public PlaylistItemAzure playlistSelected;

		public PlaylistPageCloudSync ()
		{
			CreateUI ();
		}

		async void CreateUI(){
			

			this.Title = "Namelists";

			this.ToolbarItems.Add (new ToolbarItem("Add", "", async () =>
				{
					var result = await UserDialogs.Instance.PromptAsync("Please enter a name for this list:", "New namelist", "Ok", "Cancel");
					if(string.IsNullOrWhiteSpace(result.Text) || string.IsNullOrEmpty(result.Text)){
					}else {
						App.AzurePlaylistDB.SaveItem(new PlaylistItemAzure{PlaylistName = result.Text}, true);
						await WaitableRefresh();
					}
				}));
			Debug.WriteLine ("PlaylistPagCloudSync here1");
			var searchBar = new SearchBar {
				Placeholder = "Enter a namelist",
			};
			searchBar.TextChanged += (sender, e) => {
				FilterPlaylists(searchBar.Text);
			};

			//just to initialize the database to avoid nullreferenceexceptions
			//PlaylistDB playlists = App.AzurePlaylistDB;

			Debug.WriteLine ("PlaylistPagCloudSync here2");
			listView = new ListView{
				ItemsSource = (await App.AzurePlaylistDB.GetPlaylistItems()),
				ItemTemplate = new DataTemplate(() => 
					{
						return new PlaylistViewCellCloudSync(this);
					})
			};
			Debug.WriteLine ("PlaylistPagCloudSync here3");
			listView.ItemSelected += (sender, e) => {
				this.playlistSelected = (PlaylistItemAzure)e.SelectedItem;

				// has been set to null, do not 'process' tapped event
				if (e.SelectedItem == null)
					return; 

				//load contacts based on type of playlist (warm, cold, semi warm whatever playlist is tapped)
				Navigation.PushAsync(new CAPPCloudSync(playlistSelected));

				// de-select the row
				((ListView)sender).SelectedItem = null; 
			};
			Debug.WriteLine ("PlaylistPagCloudSync here4");
			Content = new StackLayout {
				VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness(10),
				Children = {
					searchBar, 
					new StackLayout{
						Padding = new Thickness(7,0,0,0),
						Children = {listView}
					}
				}
			};
			Debug.WriteLine ("End of PlaylistPagCloudSync Constructor");
		}
		public async Task<bool> WaitableRefresh ()
		{
			listView.ItemsSource = await App.AzurePlaylistDB.GetPlaylistItems ();
			return true;
		}
		public async void refresh ()
		{
			listView.ItemsSource = await App.AzurePlaylistDB.GetPlaylistItems ();
		}

		public async void FilterPlaylists(string filter)
		{
			listView.BeginRefresh ();

			if (string.IsNullOrWhiteSpace (filter)) {
				refresh();
			} else {
				listView.ItemsSource = (await App.AzurePlaylistDB.GetPlaylistItems())
					.Where (x => x.PlaylistName.ToLower ()
						.Contains (filter.ToLower ()));
			}

			listView.EndRefresh ();
		}
	}

	public class PlaylistViewCellCloudSync:ViewCell
	{
		public PlaylistViewCellCloudSync (PlaylistPageCloudSync page)
		{
			Label playlistLabel = new Label();
			playlistLabel.SetBinding(Label.TextProperty, "PlaylistName");//"Name" binds directly to the ContactData.Name property

			var EditAction = new MenuItem { Text = "Edit" };
			EditAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			EditAction.Clicked += async (sender, e) => {
				var mi = ((MenuItem)sender);
				await EditPlaylistName((PlaylistItemAzure)mi.BindingContext);
				await page.WaitableRefresh ();
			};

			var DeleteAction = new MenuItem { Text = "Delete" , IsDestructive = true};
			DeleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			DeleteAction.Clicked += async (sender, e) => {
				var mi = ((MenuItem)sender);
				DeletePlaylist((PlaylistItemAzure)mi.BindingContext);
				await page.WaitableRefresh ();
			};

			View = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness (15, 5, 5, 15),
				Children = { playlistLabel }
			};

			ContextActions.Add(EditAction);
			ContextActions.Add(DeleteAction);
		}
		async Task<bool> EditPlaylistName(PlaylistItemAzure playlist){
			if (string.Equals (Values.ALLPLAYLISTPARAM, playlist.PlaylistName) || string.Equals (Values.TODAYSCALLS, playlist.PlaylistName)) {
				UserDialogs.Instance.InfoToast ("Sorry, we can't edit an essential namelist");
			} else {
				var result = await UserDialogs.Instance.PromptAsync("Enter a new name for this namelist:", "", "OK", "Cancel");
				if (string.IsNullOrWhiteSpace (result.Text) || string.IsNullOrEmpty (result.Text)) {
				} else {
					playlist.PlaylistName = result.Text;
					await App.AzurePlaylistDB.UpdateItem(playlist);//how to update all contacts playlist property? data bind?
					return true;
				}
			}
			return false;
		}
		async void DeletePlaylist(PlaylistItemAzure playlist){
			if (string.Equals (Values.ALLPLAYLISTPARAM, playlist.PlaylistName) || string.Equals (Values.TODAYSCALLS, playlist.PlaylistName)) {
				UserDialogs.Instance.InfoToast ("Sorry, we can't delete an essential namelist");
			} else {
				await App.AzurePlaylistDB.DeleteItem (playlist);
			}
		}
	}
}


