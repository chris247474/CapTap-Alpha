using System;
using Xamarin.Forms;
using System.ServiceModel.Channels;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capp2
{
	public class AddContactPage:ContentPage
	{
		CAPP capp;
		ToolbarItem addTB;
		ContactData AddContact;
		string playlist;

		public AddContactPage (CAPP capp, ContactData contactDuplicate = null)
		{
			this.capp = capp;
			this.playlist = capp.playlistChosen.PlaylistName;

			this.Title = "Adding to "+this.playlist+" namelist";

			NavigationPage.SetHasNavigationBar (this, true);

			var firstNameLabel = new Label { Text = "First Name" };
			var firstNameEntry = new Entry ();

			var lastNameLabel = new Label { Text = "Last Name" };
			var lastNameEntry = new Entry ();

			var numberLabel = new Label { Text = "Number" };
			var numberEntry = new Entry ();
			if(string.IsNullOrWhiteSpace (numberEntry.Text)){
				numberEntry.Text = "09";
			}

			var affLabel = new Label { Text = "Affiliation" };
			var affEntry = new Entry ();

			addTB = new ToolbarItem ("Done", "", async () => {
				firstNameEntry.Text = firstNameEntry.Text.Trim ();
				lastNameEntry.Text = lastNameEntry.Text.Trim ();
				//numberEntry.Text = numberEntry.Text.Trim ();
				if(!string.IsNullOrWhiteSpace (affEntry.Text)) affEntry.Text = affEntry.Text.Trim ();

				//speech functions for fun?
				if (string.IsNullOrWhiteSpace (firstNameEntry.Text) || string.IsNullOrEmpty (firstNameEntry.Text)) {
					await DisplayAlert ("Hey!!!", "I don't think ' ' counts as a First Name, do you? ", "Alright, sorry CappTap...");
				} else if (string.IsNullOrWhiteSpace (lastNameEntry.Text) || string.IsNullOrEmpty (lastNameEntry.Text)) {
					await DisplayAlert ("Hey!!!", "How would you like if you didn't have a family? I need a last name! ", "Alright, sorry CappTap...");
				} else if (string.IsNullOrWhiteSpace (numberEntry.Text) || string.IsNullOrEmpty (numberEntry.Text)) {
					await DisplayAlert ("Hey!!!", "We can't call your contact if he/she doesn't have a number, now can we...? ", "Alright, sorry CappTap...");
				} else if (PhoneUtil.ToNumber (numberEntry.Text) == null || numberEntry.Text.Contains (" ")) {
					await DisplayAlert ("Hey!!!", "Please only enter numbers like 09163334444", "Alright, sorry CappTap...");
				} else {
					AddContact = new ContactData {
						FirstName = firstNameEntry.Text,
						LastName = lastNameEntry.Text,
						ID = App.lastIndex++,
						Number = numberEntry.Text,
						Playlist = this.playlist,
						Aff = affEntry.Text,
					};

					//check if added contact details already exists in playlist
					if(!App.contactFuncs.duplicateExists (AddContact, App.Database.GetItems (this.playlist).ToArray ()/*.ToList ()*/)){
						if(App.contactFuncs.saveContactToDB (false, AddContact, this.playlist)){
							DependencyService.Get<IPhoneContacts> ().SaveContactToDevice(AddContact.FirstName, AddContact.LastName, AddContact.Number, AddContact.Aff);
							//await App.AzureDB.SaveTaskAsync (new ContactDataItemAzure{FirstName = AddContact.FirstName, LastName = AddContact.LastName, Number = AddContact.Number}, true);
						}
					}else{
						DisplayAlert ("Duplicate contact?", AddContact.FirstName+" "+AddContact.LastName + "'s name already exists in this namelist. Please don't save a person twice in one namelist", 
						"OK");
					}

					firstNameEntry.Text = "";
					lastNameEntry.Text = "";
					capp.refresh ();
					this.Navigation.PopAsync ();
				}
			});
			this.ToolbarItems.Add (addTB);

			var cancelButton = new Button { Text = "Cancel" };
			cancelButton.Clicked += (sender, e) => {
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
					//cancelButton
				}
			};
		}
	}
}

