using System;
using XLabs.Forms.Mvvm;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using System.Diagnostics;

namespace Capp2
{
	public class PlaylistViewModel: BaseViewModel
	{
		ObservableCollection<Playlist> _list;
		public ObservableCollection<Playlist> Namelists
		{
			get
			{
				return _list;
			}
			set
			{
				SetProperty(ref _list, value, nameof(Namelists));
			}
		}

		public PlaylistViewModel() {
			_list = App.Database.GetObservablePlaylistItems();//App.Database.GetPlaylistItems().ToList();
		}


		public void UpdateNamelistLabel(string oldName, string newName) {
			int pos = -1;
			Playlist item = null;
			if (!string.IsNullOrWhiteSpace(oldName) && !string.IsNullOrWhiteSpace(newName))
			{
				foreach (Playlist p in _list) { 
					if (string.Equals(p.PlaylistName, oldName))
					{
						p.PlaylistName = newName;
						pos = Namelists.IndexOf(p);
						item = p;
						App.Database.UpdateItem(p);
					}
				}
				if (pos >= 0 && item != null) { 
					//Namelists.Remove(item);
					//Namelists.Insert(pos, item);
				}	
			}
			else throw 
				new ArgumentException
				("PlaylistViewModel.UpdateNamelistLabel does not accept null or empty string arguments");
		}

		public bool NamelistAlreadyExists(string name)
		{
			var playlistarr = Namelists.ToArray();
			for (int x = 0; x < playlistarr.Length; x++)
			{
				if (string.Equals(playlistarr[x].PlaylistName, name))
				{
					return true;
				}
			}
			return false;
		}

		public async Task AddNamelist(PlaylistPage page)
		{
			var result = await UserDialogs.Instance.PromptAsync("Please enter a name for this list:",
				"New namelist", "OK");
			if (!string.IsNullOrWhiteSpace(result.Text) && !string.Equals("Cancel", result.Text))
			{
				if (NamelistAlreadyExists(result.Text))
				{
					await AlertHelper.Alert("That namelist already exists", "Let's try again");
					AddNamelist(page);
				}
				else {
					var newplaylist = new Playlist { PlaylistName = result.Text, Icon = await GetNamelistIcon() };
					Namelists.Add(newplaylist);
					App.Database.SavePlaylistItem(newplaylist);
				}
			}
			if (App.InTutorialMode)
			{
				TutorialHelper.OpenNamelist(page, "You made a namelist! Now tap it to select it",
					Color.FromHex(Values.CAPPTUTORIALCOLOR_Green));
			}
		}

		async Task<string> GetNamelistIcon() {
			string icon = string.Empty;

			var warmcold = await UserDialogs.Instance.ActionSheetAsync("What kind of namelist is this?", null, null,
						new string[] { Values.WARM, Values.SEMIWARM, Values.COLD });
			
			if (string.Equals(warmcold, Values.WARM))
			{
				icon = "flame.png";
			}
			else if (string.Equals(warmcold, Values.COLD))
			{
				icon = "snowflake.png";
			}
			else if (string.Equals(warmcold, Values.SEMIWARM))
			{
				icon = "semi.png";
			}

			return icon;
		}

		public void DeleteNamelist(string name) {
			if (!string.IsNullOrWhiteSpace(name))
			{
				var item = FindMatchingNamelist(name);
				if (item != null)
				{
					Namelists.Remove(item);
					App.Database.DeletePlaylistItem(item);
				}
			}
			else throw
				new ArgumentException
				("PlaylistViewModel.UpdateNamelistLabel does not accept null or empty string arguments");
		}
		Playlist FindMatchingNamelist(string name) { 
			var arr = _list.ToArray();
			for (int c = 0; c < arr.Length; c++)
			{
				if (string.Equals(arr[c].PlaylistName, name)) return arr[c];
			}
			return null;
		}
		public string[] GetPlaylistNames()
		{
			try
			{
				Playlist[] playlistarr = Namelists.ToArray();
				var pnames = new string[playlistarr.Length];
				for (int c = 0; c < playlistarr.Length; c++)
				{
					pnames[c] = playlistarr[c].PlaylistName;
				}
				return pnames;
			}
			catch (Exception e)
			{
				Debug.WriteLine("PaylistViewModel.GetPlaylistNames() error: {0}", e.Message);
			}
			return null;
		}
	}
}

