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
	public class PlaylistPage:ContentPage//MasterDetailPage
	{
		public ListView listView{ get; set;}
		public Playlist playlistSelected;
        StackLayout stack = new StackLayout();

        public PlaylistPage()
        {
            this.Title = "Namelists";
            this.BackgroundColor = Color.White;

            foreach (Playlist p in App.Database.GetPlaylistItems()) {
                Debug.WriteLine("playlist: {0}", p.PlaylistName);
            }

            var searchBar = new SearchBar {
                Placeholder = "Enter a namelist",
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            searchBar.TextChanged += (sender, e) => {
                FilterPlaylists(searchBar.Text);
            };

            if (Device.OS == TargetPlatform.iOS)
            {
                Debug.WriteLine("Adding add tbi temporarily");
                this.ToolbarItems.Add(new ToolbarItem("Add", "", async () =>
                {
                    var result = await UserDialogs.Instance.PromptAsync("Please enter a name for this list:", "New namelist", "OK", "Cancel");
                    if (string.IsNullOrWhiteSpace(result.Text) || string.IsNullOrEmpty(result.Text))
                    {
                    }
                    else {
                        App.Database.SavePlaylistItem(new Playlist { PlaylistName = result.Text });
                        refresh();
                    }
                }));
            }

            listView = new ListView {
                ItemsSource = App.Database.GetPlaylistItems(),
                SeparatorColor = this.BackgroundColor,
                ItemTemplate = new DataTemplate(() =>
                    {
                        return new PlaylistViewCell(this);
                    })
            };
            listView.ItemSelected += (sender, e) => {
                this.playlistSelected = (Playlist)e.SelectedItem;

                // has been set to null, do not 'process' tapped event
                if (e.SelectedItem == null)
                    return;

                //load contacts based on type of playlist (warm, cold, semi warm whatever playlist is tapped)
                Navigation.PushAsync(new CAPP(playlistSelected));

                // de-select the row
                ((ListView)sender).SelectedItem = null;
            };

            if (Device.OS == TargetPlatform.iOS) {
                stack = new StackLayout
                {
                    //Padding = new Thickness(10),
                    Children = {
                        searchBar,
                        new StackLayout{
                            Padding = new Thickness(7,0,0,0),
                            Children = {listView}
                    }
                }
                };
            } else if (Device.OS == TargetPlatform.Android) {
                stack = new StackLayout
                {
                    Padding = new Thickness(10),
                    Children = {
                        searchBar,
                        new StackLayout{
                            Padding = new Thickness(7,0,0,0),
                            Children = {listView}
                    }
                }
                };
            }

			Content = UIBuilder.AddFloatingActionButtonToStackLayout(stack, "ic_add_white_24dp.png", new Command (async () =>
				{
					var result = await UserDialogs.Instance.PromptAsync("Please enter a name for this list:", "New namelist", "OK", "Cancel");
					if(string.IsNullOrWhiteSpace(result.Text) || string.IsNullOrEmpty(result.Text)){
					}else {
						App.Database.SavePlaylistItem(new Playlist{PlaylistName = result.Text});
						refresh();
					}
				}), Color.FromHex (Values.GOOGLEBLUE), Color.FromHex (Values.PURPLE));
		}

		public void refresh ()
		{
			listView.ItemsSource = App.Database.GetPlaylistItems();
		}

		public void FilterPlaylists(string filter)
		{
			listView.BeginRefresh ();

			if (string.IsNullOrWhiteSpace (filter)) {
				refresh();
			} else {
				listView.ItemsSource = App.Database.GetPlaylistItems()
                    .Where (x => x.PlaylistName.ToLower ()
						.Contains (filter.ToLower ()));
			}

			listView.EndRefresh ();
		}
	}

	public class PlaylistViewCell:ViewCell
	{
		public PlaylistViewCell (PlaylistPage page)
		{
			Label playlistLabel = new Label();
			playlistLabel.SetBinding(Label.TextProperty, "PlaylistName");//"Name" binds directly to the ContactData.Name property

			var EditAction = new MenuItem { Text = "Edit" };
			EditAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			EditAction.Clicked += async (sender, e) => {
				var mi = ((MenuItem)sender);
				await EditPlaylistName((Playlist)mi.BindingContext);
				page.refresh ();
			};

			var DeleteAction = new MenuItem { Text = "Delete" , IsDestructive = true};
			DeleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			DeleteAction.Clicked += (sender, e) => {
				var mi = ((MenuItem)sender);
				DeletePlaylist((Playlist)mi.BindingContext);
				page.refresh ();
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
		async Task<bool> EditPlaylistName(Playlist playlist){
            string oldPlaylistName = playlist.PlaylistName;
			if (string.Equals (Values.ALLPLAYLISTPARAM, playlist.PlaylistName) || string.Equals (Values.TODAYSCALLS, playlist.PlaylistName)) {
				UserDialogs.Instance.InfoToast ("Sorry, we can't edit an essential namelist");
			} else {
				var result = await UserDialogs.Instance.PromptAsync("Enter a new name for this namelist:", "", "OK", "Cancel");
				if (string.IsNullOrWhiteSpace (result.Text) || string.IsNullOrEmpty (result.Text)) {
				} else {
					playlist.PlaylistName = result.Text;
					App.Database.UpdatePlaylistItem(playlist);//how to update all contacts playlist property? data bind?
                    await UpdatePlaylistContentsToNewName(oldPlaylistName, result.Text);
                    return true;
				}
			}
			return false;
		}
        async Task UpdatePlaylistContentsToNewName(string oldPlaylistName, string newPlaylistName) {
            List<ContactData> contactsToMove = new List<ContactData>();
            try {
                var contacts = App.Database.GetItems(oldPlaylistName).ToArray();

                for (int c = 0; c < contacts.Length; c++)
                {
                    contacts[c].Playlist = newPlaylistName;
                    contactsToMove.Add(contacts[c]);
                }

                App.Database.UpdateAll(contactsToMove.AsEnumerable());
            } catch (Exception e) {
                Debug.WriteLine("UpdatePlaylistContentsToNewName error: {0}", e.Message);
                UserDialogs.Instance.ShowError("Something went wrong! Pls try again"); }
        }
		void DeletePlaylist(Playlist playlist){
			if (string.Equals (Values.ALLPLAYLISTPARAM, playlist.PlaylistName) || string.Equals (Values.TODAYSCALLS, playlist.PlaylistName)) {
				UserDialogs.Instance.InfoToast ("Sorry, we can't delete an essential namelist");
			} else {
				App.Database.DeletePlaylistItem (playlist);
			}
		}
	}
}

