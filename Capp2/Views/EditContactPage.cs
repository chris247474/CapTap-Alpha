using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Collections;

namespace Capp2
{
	public class EditContactPage:ContentPage
	{
		ToolbarItem addTB;
		string[] playlistArr;

		public EditContactPage (ContactData contact, CAPP page)  
		{
			NavigationPage.SetHasNavigationBar (this, true);

			var firstNameLabel = new Label { Text = "First Name" };
			var firstNameEntry = new Entry ();
			firstNameEntry.Text = contact.FirstName;

			var lastNameLabel = new Label { Text = "Last Name" };
			var lastNameEntry = new Entry ();
			lastNameEntry.Text = contact.LastName;

			var numberLabel = new Label { Text = "Number" };
			var numberEntry = new Entry ();
			numberEntry.Text = contact.Number;

			var affLabel = new Label { Text = "Affiliation" };
			var affEntry = new Entry ();
			if (contact.Aff != null)
				affEntry.Text = contact.Aff;
			else
				affEntry.Text = " ";

			Label playlistLabel = new Label { Text = "Choose a Playlist" };
			Picker playlistPicker = new Picker
			{
				Title = "Playlist",
				SelectedIndex = -1
			};

			//populate dropdown box
			foreach(Playlist p in App.Database.GetPlaylistItems())
            {
				playlistPicker.Items.Add(p.PlaylistName);
			}

			//crude workaround for lack of bindable Picker
			playlistArr = new string[playlistPicker.Items.Count];
			int i = 0;
			foreach(Playlist p in App.Database.GetPlaylistItems())
            {
				playlistArr [i] = p.PlaylistName;
				i++;
			}
			i = 0;

			playlistPicker.SelectedIndexChanged += (sender, args) =>
			{
				int x = 0;
				for(x = 0;x < (sender as Picker).SelectedIndex;x++){
				}
				contact.Playlist = playlistArr[x];
			};

			addTB = new ToolbarItem ("Done", "", () => {
				firstNameEntry.Text = firstNameEntry.Text.Trim();
				lastNameEntry.Text = lastNameEntry.Text.Trim();
				//numberEntry.Text = numberEntry.Text.Trim();
				affEntry.Text = affEntry.Text.Trim();

				//speech functions for fun?
				if(string.IsNullOrWhiteSpace(firstNameEntry.Text) || string.IsNullOrEmpty(firstNameEntry.Text)){
					DisplayAlert("Hey!!!", "I don't think ' ' counts as a First Name, do you? ", "Alright, sorry CappTap...");
				}else
					if(string.IsNullOrWhiteSpace(lastNameEntry.Text) || string.IsNullOrEmpty(lastNameEntry.Text)){
						DisplayAlert("Hey!!!", "How would you like if you didn't have a family? I need a last name! ", "Alright, sorry CappTap...");
					}else
						if(string.IsNullOrWhiteSpace(numberEntry.Text) || string.IsNullOrEmpty(numberEntry.Text)){
							DisplayAlert("Hey!!!", "We can't call your contact if he/she doesn't have a number, now can we...? ", "Alright, sorry CappTap...");
						}else
							if(PhoneUtil.ToNumber(numberEntry.Text) == null || numberEntry.Text.Contains(" ")){
								DisplayAlert("Hey!!!", "Please only enter numbers like 09163334444", "Alright, sorry CappTap...");
							}else{

								contact.FirstName = firstNameEntry.Text;
								contact.LastName = lastNameEntry.Text;
								contact.Aff = affEntry.Text;
								contact.Number = numberEntry.Text;
								//contact.Playlist default value is assigned in Database.GetItems(playlist)

								Debug.WriteLine("udpated ID is:"+contact.ID);
								Debug.WriteLine("current db index is:"+App.lastIndex);

								App.Database.UpdateItem(contact);

								firstNameEntry.Text = "";
								lastNameEntry.Text = "";
								page.refresh ();
								this.Navigation.PopAsync();
							}
			});
			this.ToolbarItems.Add (addTB);

			var cancelButton = new Button { Text = "Cancel" };
			cancelButton.Clicked += (sender, e) => {
				page.refresh();
				this.Navigation.PopAsync();
			};

			Content = new StackLayout {
				VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness(20),
				Children = {
					firstNameLabel, firstNameEntry,
					lastNameLabel, lastNameEntry,
					numberLabel, numberEntry,
					affLabel, affEntry,
					playlistLabel,playlistPicker,
					cancelButton
				}
			};
		}
	}
}

