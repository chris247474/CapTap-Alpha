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
		StackLayout stack = new StackLayout();
		ToolbarItem addTB;
		string[] playlistArr;
		Label numberLabel, number2Label, number3Label, number4Label, number5Label, firstNameLabel, lastNameLabel, affLabel, playlistLabel;
		Entry numberEntry, number2Entry, number3Entry, number4Entry, number5Entry, firstNameEntry, lastNameEntry, affEntry;
		Picker playlistPicker;
		Button MarkAutoCall = new Button ();
		Image ContactPic = new Image();

		public EditContactPage (ContactData contact, CAPP page)  
		{
			NavigationPage.SetHasNavigationBar (this, true);

			BindingContext = contact;

			ContactPic = UIBuilder.CreateTappableCircleImage ("", LayoutOptions.CenterAndExpand, 
				Aspect.AspectFit, new Command (() => {
					//UIAnimationHelper.ShrinkUnshrinkElement(ContactPic);
				}
			));
			ContactPic.HeightRequest = 222;
			ContactPic.WidthRequest = 222;
			ContactPic.SetBinding(Image.SourceProperty, new Xamarin.Forms.Binding{Path = "LargePic"});

			firstNameLabel = new Label { Text = "First Name" };
			firstNameEntry = new Entry ();
			firstNameEntry.Text = contact.FirstName;

			lastNameLabel = new Label { Text = "Last Name" };
			lastNameEntry = new Entry ();
			lastNameEntry.Text = contact.LastName;

			numberLabel = new Label { Text = "Number" };
			numberEntry = new Entry ();
			numberEntry.Text = contact.Number;

			number2Label = new Label { Text = "Number" };
			number2Entry = new Entry ();
			number2Entry.Text = contact.Number2;

			number3Label = new Label { Text = "Number" };
			number3Entry = new Entry ();
			number3Entry.Text = contact.Number3;

			number4Label = new Label { Text = "Number" };
			number4Entry = new Entry ();
			number4Entry.Text = contact.Number4;

			number5Label = new Label { Text = "Number" };
			number5Entry = new Entry ();
			number5Entry.Text = contact.Number5;

			affLabel = new Label { Text = "Affiliation" };
			affEntry = new Entry ();
			if (contact.Aff != null)
				affEntry.Text = contact.Aff;
			else
				affEntry.Text = " ";

			playlistLabel = new Label { Text = "Choose a Namelist" };
			playlistPicker = new Picker
			{
				Title = "Choose a Namelist",
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
			//this.ToolbarItems.Add (addTB);

			var cancelButton = new Button { Text = "Cancel" };
			cancelButton.Clicked += (sender, e) => {
				page.refresh();
				this.Navigation.PopAsync();
			};

			MarkAutoCall = new Button{
				Text = "Use for Calls",
			};
			MarkAutoCall.Clicked += (object sender, EventArgs e) => {
				
			};

			Content = UIBuilder.AddFloatingActionButtonToStackLayout(CreateEditContactLayout (contact), "CheckmarkWhite500.png", 
				new Command(() => {
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
				}), Color.FromHex(Values.GOOGLEBLUE), Color.FromHex(Values.PURPLE));

			UIAnimationHelper.FlyDown (stack);
		}

		void SaveNewContactInfo(){
		}

		View CreateEditContactLayout(ContactData contact){
			if(Number5Exists(contact)){
				stack = new StackLayout {
					VerticalOptions = LayoutOptions.StartAndExpand,
					Padding = new Thickness(20),
					Children = {
						firstNameLabel, firstNameEntry,
						lastNameLabel, lastNameEntry,
						UIBuilder.CreateEmptyStackSpace(),
						numberLabel, numberEntry,
						number2Label, number2Entry,
						number3Label, number3Entry,
						number4Label, number4Entry,
						number5Label, number5Entry,
						UIBuilder.CreateEmptyStackSpace(),
						affLabel, affEntry,
						UIBuilder.CreateEmptyStackSpace(),
						playlistLabel,playlistPicker,
					}
				};
			}else if(!Number5Exists(contact) && Number4Exists(contact)){
				stack = new StackLayout {
					VerticalOptions = LayoutOptions.StartAndExpand,
					Padding = new Thickness(20),
					Children = {
						firstNameLabel, firstNameEntry,
						lastNameLabel, lastNameEntry,
						UIBuilder.CreateEmptyStackSpace(),
						numberLabel, numberEntry,
						number2Label, number2Entry,
						number3Label, number3Entry,
						number4Label, number4Entry,
						UIBuilder.CreateEmptyStackSpace(),
						affLabel, affEntry,
						UIBuilder.CreateEmptyStackSpace(),
						playlistLabel,playlistPicker,
					}
				};
			}else if(!Number5Exists(contact) && !Number4Exists(contact) && Number3Exists(contact)){
				stack = new StackLayout {
					VerticalOptions = LayoutOptions.StartAndExpand,
					Padding = new Thickness(20),
					Children = {
						firstNameLabel, firstNameEntry,
						lastNameLabel, lastNameEntry,
						UIBuilder.CreateEmptyStackSpace(),
						numberLabel, numberEntry,
						number2Label, number2Entry,
						number3Label, number3Entry,
						UIBuilder.CreateEmptyStackSpace(),
						affLabel, affEntry,
						UIBuilder.CreateEmptyStackSpace(),
						playlistLabel,playlistPicker,
					}
				};
			}else if(!Number5Exists(contact) && !Number4Exists(contact) && !Number3Exists(contact) && Number2Exists(contact)){
				stack = new StackLayout {
					VerticalOptions = LayoutOptions.StartAndExpand,
					Padding = new Thickness(20),
					Children = {
						firstNameLabel, firstNameEntry,
						lastNameLabel, lastNameEntry,
						UIBuilder.CreateEmptyStackSpace(),
						numberLabel, numberEntry,
						number2Label, number2Entry,
						UIBuilder.CreateEmptyStackSpace(),
						affLabel, affEntry,
						UIBuilder.CreateEmptyStackSpace(),
						playlistLabel,playlistPicker,
					}
				};
			}else {
				stack = new StackLayout {
					VerticalOptions = LayoutOptions.StartAndExpand,
					Padding = new Thickness(20),
					Children = {
						firstNameLabel, firstNameEntry,
						lastNameLabel, lastNameEntry,
						UIBuilder.CreateEmptyStackSpace(),
						numberLabel, numberEntry,
						UIBuilder.CreateEmptyStackSpace(),
						affLabel, affEntry,
						UIBuilder.CreateEmptyStackSpace(),
						playlistLabel,playlistPicker,
					}
				};
			}

			//insert call, message icons
			stack.Children.Insert(0, new StackLayout{
				Orientation = StackOrientation.Horizontal,
				//HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness(0, 20),
				Children = {
					new StackLayout{
						HorizontalOptions = LayoutOptions.StartAndExpand,
						Children = {
							UIBuilder.CreateTappableImage("Phone", LayoutOptions.Fill, Aspect.AspectFit,
								new Command(async () => {
									await CallHelper.call(contact, false);
								}), firstNameLabel.FontSize * 1.5)
						}
					},
					new StackLayout{
						HorizontalOptions = LayoutOptions.End,
						Children = {
							UIBuilder.CreateTappableImage("Message-green-100", LayoutOptions.Fill, Aspect.AspectFit,
								new Command(async () => {
									DependencyService.Get<IPhoneContacts>().SendSMS(await CallHelper.HandleMutlipleNumbers(contact));
								}), firstNameLabel.FontSize * 1.5)
						}
					},
				}
			});

			stack.Children.Insert (0, UIBuilder.CreateEmptyStackSpace ());
			stack.Children.Insert (0, ContactPic);
			stack.Children.Insert (0, UIBuilder.CreateEmptyStackSpace ());
			stack.Children.Insert (0, UIBuilder.CreateEmptyStackSpace ());
			stack.HorizontalOptions = LayoutOptions.CenterAndExpand;


			return new ScrollView{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = stack
			};
		}

		bool Number2Exists(ContactData contact){
			if (string.IsNullOrWhiteSpace (contact.Number2)) {
				return false;
			} else {
				return true;
			}
		}
		bool Number3Exists(ContactData contact){
			if (string.IsNullOrWhiteSpace (contact.Number3)) {
				return false;
			} else {
				return true;
			}
		}
		bool Number4Exists(ContactData contact){
			if (string.IsNullOrWhiteSpace (contact.Number4)) {
				return false;
			} else {
				return true;
			}
		}
		bool Number5Exists(ContactData contact){
			if (string.IsNullOrWhiteSpace (contact.Number5)) {
				return false;
			} else {
				return true;
			}
		}
	}
}

