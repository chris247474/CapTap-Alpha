using System;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Plugin.Calendars.Abstractions;
using System.Threading.Tasks;
using Capp2.Helpers;

namespace Capp2
{
	public class PlaylistPage:GradientContentPage
	{
		public ListView listView{ get; set;}
		public Playlist playlistSelected;
        StackLayout stack = new StackLayout();
		SearchBar searchBar;

        public PlaylistPage()
        {
            this.Title = "Namelists";
			this.BackgroundColor = Color.Transparent;

            foreach (Playlist p in App.Database.GetPlaylistItems()) {
                Debug.WriteLine("playlist: {0}", p.PlaylistName);
            }

            searchBar = new SearchBar {
				BackgroundColor = Color.Transparent,
                Placeholder = "Search",
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            searchBar.TextChanged += (sender, e) => {
                FilterPlaylists(searchBar.Text);
            };

			CreateListView ();

            if (Device.OS == TargetPlatform.iOS) {
                stack = new StackLayout
                {
					BackgroundColor = Color.Transparent,
                    Children = {
						UIBuilder.CreateEmptyStackSpace(),
						UIBuilder.CreateEmptyStackSpace(),
						new StackLayout{
							Padding = new Thickness(0, 10, 0, 0),
							Children = {searchBar}
						},
                        new StackLayout{
							VerticalOptions = LayoutOptions.Start,
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
						UIBuilder.CreateEmptyStackSpace(),
						UIBuilder.CreateEmptyStackSpace(),
						searchBar,
                        new StackLayout{
							VerticalOptions = LayoutOptions.Start,
                            Padding = new Thickness(7,0,0,0),
                            Children = {listView}
	                    }
	                }
                };
            }

			AdHelper.AddGreenURLOrangeTitleBannerToStack (stack);

			Content = UIBuilder.AddFloatingActionButtonToViewWrapWithRelativeLayout(stack, "Plus.png", new Command (async () =>
				{
					Util.AddNamelist(this);
				}), Color.FromHex (Values.GOOGLEBLUE), Color.FromHex (Values.PURPLE));

			listView.Opacity = 0;
		}
			
		public void CreateListView(){
			listView = new ListView {
				BackgroundColor = Color.Transparent,
				ItemsSource = App.Database.GetPlaylistItems(),
				SeparatorColor = Color.Transparent,//FromHex("#E1E1E1"),//this.BackgroundColor,
				ItemTemplate = new DataTemplate(() =>
					{
						return new PlaylistViewCell(this);
					}),
				VerticalOptions = LayoutOptions.Start,
			};
			listView.ItemSelected += (sender, e) => {
				// has been set to null, do not 'process' tapped event
				if (e.SelectedItem == null)
					return;
				
				this.playlistSelected = (Playlist)e.SelectedItem;

				//if(Settings.IsFirstRunSettings){
				//	Navigation.PushAsync(App.CapPage);
				//}else{
					Navigation.PushAsync(new CAPP(playlistSelected.PlaylistName));
				//}

				// de-select the row
				((ListView)sender).SelectedItem = null;

				listView.Opacity = 0;
			};
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			listView.FadeTo (1, 125, Easing.CubicIn);
			UIAnimationHelper.FlyFromLeft (listView, 400);

			if (App.InTutorialMode && !TutorialHelper.HowToMakeNamelistDone) {
				Debug.WriteLine ("About to launch HowTomakeANamelistTIp");

				TutorialHelper.HowToMakeANamelist(this, "Let's setup your first namelist.\nPlease tap the '+' button down there",
					Color.FromHex(Values.CAPPTUTORIALCOLOR_Purple));
			}
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
			this.Height = this.RenderHeight * 2;
			Label playlistLabel = new Label();
			playlistLabel.SetBinding(Label.TextProperty, "PlaylistName");//"Name" binds directly to the ContactData.Name property
			playlistLabel.HorizontalTextAlignment = TextAlignment.Start;
			playlistLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
			playlistLabel.HeightRequest = playlistLabel.Height * 2;
			//playlistLabel.FontFamily = Device.OnPlatform ("SFCompact", "sans-serif-black", null);
			playlistLabel.FontSize = playlistLabel.FontSize * 1.2;
			playlistLabel.TextColor = Color.FromHex ("#797979");

			Image nextImage = new Image{ 
				Source = FileImageSource.FromFile ("NextArrowBlue.png"),
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				HeightRequest = playlistLabel.FontSize *1.3,
				WidthRequest = playlistLabel.FontSize *1.3,
			};
			Image WarmCold = new Image{ 
				Source = "",//FileImageSource.FromFile ("people.png"),
				Aspect = Aspect.Fill,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				//HeightRequest = playlistLabel.FontSize *1.5,
				//WidthRequest = playlistLabel.FontSize *1.5,
			};
			WarmCold.SetBinding (Image.SourceProperty, "Icon");

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
				Padding = new Thickness (0, 5, 5, 15),
				Children = { 
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.StartAndExpand,
						Children = {
							UIBuilder.CreateDataTemplateLabelEmptySpace(),
							WarmCold,
							UIBuilder.CreateDataTemplateLabelEmptySpace(),
							playlistLabel,

						}
					},
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.End,
						VerticalOptions = LayoutOptions.Center,
						Children = {nextImage, UIBuilder.CreateDataTemplateLabelEmptySpace(),}
					}
				}
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

