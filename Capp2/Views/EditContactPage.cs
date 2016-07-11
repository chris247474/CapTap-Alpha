using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Collections;
using XLabs.Forms.Controls;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using FFImageLoading.Transformations;

namespace Capp2
{
	public class EditContactPage:ContentPage
	{
		StackLayout EditContactStack = new StackLayout(), DetailStack = new StackLayout(), 
			MainStack = new StackLayout();
		string[] playlistArr;
		Label numberLabel, number2Label, number3Label, number4Label, number5Label, firstNameLabel, lastNameLabel, affLabel, playlistLabel;
		Entry numberEntry, number2Entry, number3Entry, number4Entry, number5Entry, firstNameEntry, lastNameEntry, affEntry;
		Picker playlistPicker;
		Image phoneImg = new Image(), messageImg = new Image(),
			dome = new Image();
		CachedImage backgroundImage = new CachedImage ();
		CircleImage ContactPic = new CircleImage ();
		BoxView shader;
		DetailsView details;
		RelativeLayout relativeLayout = new RelativeLayout ();
		bool viewmode = true;
		ToolbarItem EditTBI;

		public EditContactPage (ContactData contact, CAPP page)  
		{
			NavigationPage.SetHasNavigationBar (this, true);
			App.EditPage = this;
			ShowPage (contact, page);
		}
		async void ShowPage(ContactData contact, CAPP page){
			await InitUI (contact);

			CreateLayouts (contact);

			this.Content = UIBuilder.AddFloatingActionButtonToRelativeLayout(relativeLayout, "Edit.png", 
				new Command(() => {
					//SwitchEditViewMode(page, contact);
					Navigation.PushAsync(new AddEditContactNativePage(contact));
				}), Color.FromHex(Values.GOOGLEBLUE), Color.FromHex(Values.PURPLE)/*, "Checkmark.png"*/);

			UIAnimationHelper.FlyDown (relativeLayout);
		}
		void CreateLayouts(ContactData contact){
			
			relativeLayout = new RelativeLayout ();

			relativeLayout.Children.Add (
				backgroundImage,
				Constraint.Constant (0),
				Constraint.Constant (0),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Height * .4;
				})
			);

			if (!contact.HasDefaultImage_Small) { 
				relativeLayout.Children.Add(
				shader,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) =>
				{
					return parent.Width;
				}),
				Constraint.RelativeToParent((parent) =>
				{
					return parent.Height * .4;
				})
			);
			}

			relativeLayout.Children.Add (
				dome,
				Constraint.Constant (-10),
				Constraint.RelativeToParent ((parent) => {
					return (parent.Height * 0.4) - 50;
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width + 10;
				}),
				Constraint.Constant (75)
			);

			if (contact.HasDefaultImage_Large) {
				relativeLayout.Children.Add(
					ContactPic,
					Constraint.RelativeToParent((parent) =>
					{
						return ((parent.Width / 2) - (ContactPic.Width / 2));
					}),
					Constraint.RelativeToParent((parent) =>
					{
						return parent.Height * .22;
					}),
					Constraint.RelativeToParent((parent) =>
					{
						return parent.Width * .5;
					}),
					Constraint.RelativeToParent((parent) =>
					{
						return parent.Width * .5;
					})
				);

				UIBuilder.PlaceInitialsTextOnImage (relativeLayout, details.name.FontSize, ContactPic, contact);
				Debug.WriteLine ("{0} has default image large", contact.Name);
			} else {
				Debug.WriteLine ("{0} has custom image large", contact.Name);
			}

			var detailHeightMultiplier = contact.HasDefaultImage_Large ? 2 : 0.4;
			relativeLayout.Children.Add (
				MainStack,
				Constraint.Constant (0),
				Constraint.RelativeToView (dome, (parent, view) => {
					return view.Y + (view.Height *detailHeightMultiplier);
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				})/*,
				Constraint.Constant (120)*/
			);

			relativeLayout.Children.Add (
				messageImg,
				Constraint.RelativeToParent ((parent) => {
					return parent.Width * .1;
				}),
				Constraint.RelativeToParent ((parent) => {
					return (parent.Height * .45);
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width * .10;
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width * .10;
				})
			);

			relativeLayout.Children.Add (
				phoneImg,
				Constraint.RelativeToParent ((parent) => {
					return parent.Width * .95 - (parent.Width * .15);
				}),
				Constraint.RelativeToParent ((parent) => {
					return (parent.Height * .46);
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width * .09;
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width * .09;
				})
			);

			ContactPic.SizeChanged += (sender, e) => {
				relativeLayout.ForceLayout ();
			};

			this.Content = relativeLayout;
		}

		async Task InitUI(ContactData contact){
			BindingContext = contact;

			EditTBI = new ToolbarItem("More", "", async () =>
				{
					await Navigation.PushAsync(new AddEditContactNativePage(contact));
				});
			
			this.ToolbarItems.Add (EditTBI);

			backgroundImage = new CachedImage () {
				/*FinishCommand = new Command(()=>{
					backgroundImage.FadeTo (0.8, 1000, Easing.Linear);
				}),*/
				//FadeAnimationEnabled = true,
				DownsampleToViewSize = true,
				LoadingPriority = LoadingPriority.Highest,
				CacheDuration = TimeSpan.FromDays(30),
				Aspect = Aspect.AspectFill,
				//IsOpaque = true,
				Opacity = 0.8,
				Transformations = new System.Collections.Generic.List<ITransformation>() {
					new BlurredTransformation(1)
				},
			};
			backgroundImage.SetBinding(CachedImage.SourceProperty, new Xamarin.Forms.Binding{Path = "LargePic"});

			dome = new Image () {
				Aspect = Aspect.AspectFill,
				Source = new FileImageSource () { File = "dome.png" }
			};

			shader = new BoxView () {
				Color = Color.Black.MultiplyAlpha(0.25),
			};

			ContactPic = UIBuilder.CreateTappableCircleImage ("", LayoutOptions.Fill, Aspect.Fill, new Command(()=>{}));
			ContactPic.BackgroundColor = Color.White;
			ContactPic.SetBinding(CircleImage.SourceProperty, new Xamarin.Forms.Binding{Path = "LargePic"});

			firstNameLabel = new Label { Text = "First Name" };
			firstNameLabel.HorizontalOptions = LayoutOptions.Center;
			firstNameEntry = new Entry ();
			firstNameEntry.Text = contact.FirstName;

			lastNameLabel = new Label { Text = "Last Name" };
			lastNameLabel.HorizontalOptions = LayoutOptions.Center;
			lastNameEntry = new Entry ();
			lastNameEntry.Text = contact.LastName;

			numberLabel = new Label { Text = "Number" };
			numberLabel.HorizontalOptions = LayoutOptions.Center;
			numberEntry = new Entry ();
			numberEntry.Text = contact.Number;

			number2Label = new Label { Text = "Number" };
			number2Label.HorizontalOptions = LayoutOptions.Center;
			number2Entry = new Entry ();
			number2Entry.Text = contact.Number2;

			number3Label = new Label { Text = "Number" };
			number3Label.HorizontalOptions = LayoutOptions.Center;
			number3Entry = new Entry ();
			number3Entry.Text = contact.Number3;

			number4Label = new Label { Text = "Number" };
			number4Label.HorizontalOptions = LayoutOptions.Center;
			number4Entry = new Entry ();
			number4Entry.Text = contact.Number4;

			number5Label = new Label { Text = "Number" };
			number5Label.HorizontalOptions = LayoutOptions.Center;
			number5Entry = new Entry ();
			number5Entry.Text = contact.Number5;

			affLabel = new Label { Text = "Affiliation" };
			affLabel.HorizontalOptions = LayoutOptions.Center;
			affEntry = new Entry ();
			affEntry.Text = contact.Aff;
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

			phoneImg = UIBuilder.CreateTappableImage ("Phone-blue-outline", LayoutOptions.Fill, Aspect.AspectFit,
				new Command (async () => {
					await CallHelper.call (contact, false);
				}), firstNameLabel.FontSize * 2);

			messageImg = UIBuilder.CreateTappableImage ("message-blue-outline"/*"Message-green-100"*/, LayoutOptions.Fill, Aspect.AspectFit,
				new Command (async () => {
					DependencyService.Get<IPhoneContacts> ().SendSMS (await CallHelper.HandleMutlipleNumbers (contact));
				}), firstNameLabel.FontSize * 2);

			details = new DetailsView (contact);

			DetailStack = new StackLayout{
				Orientation = StackOrientation.Vertical,
				BackgroundColor = Color.Transparent,
				Children = {
					details
				}
			};
			CreateEditContactLayout (contact);
			MainStack = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				BackgroundColor = Color.Transparent,
				Children = {
					DetailStack
				}
			};
		}

		async Task SwitchEditViewMode(CAPP page, ContactData contact){
			var layoutyposition = relativeLayout.Y;
			var mainstackyposition = MainStack.Y;

			if (viewmode) {
				Debug.WriteLine ("EditContactPage in view mode");
				MainStack.Children.Remove (DetailStack);
				MainStack.Children.Add (EditContactStack);
				viewmode = false;

				await ContactPic.FadeTo (0, 100);
				ContactPic.Source = "";
				ContactPic.FadeTo (1, 200);
				phoneImg.FadeTo (0, 100);
				messageImg.FadeTo (0, 100);
				relativeLayout.TranslateTo (relativeLayout.X, backgroundImage.Y - (backgroundImage.Height*0.5), 800, Easing.SinInOut);
				MainStack.TranslateTo (MainStack.X, -(MainStack.Height*0.37), 400, Easing.SinInOut);

			} else {
				Debug.WriteLine ("EditContactPage in edit mode");
				if (await EditContact (page, contact)) {
					viewmode = true;
					DetailStack.Children.Remove (details);
					details = new DetailsView (contact);
					DetailStack.Children.Add (details);
					MainStack.Children.Remove (EditContactStack);
					MainStack.Children.Add (DetailStack);

					await ContactPic.FadeTo (0, 100);
					ContactPic.Source = contact.LargePic;
					ContactPic.FadeTo (1, 200);
					relativeLayout.TranslateTo (relativeLayout.X, layoutyposition, 800, Easing.SinInOut);
					MainStack.TranslateTo (MainStack.X, (MainStack.Y-(MainStack.Height*0.37))/16, 400, Easing.SinInOut);
					phoneImg.FadeTo (1, 100);
					messageImg.FadeTo (1, 100);
				}
			}
		}

		async Task<bool> EditContact(CAPP page, ContactData contact){
			UserDialogs.Instance.ShowLoading ("Saving changes...");
			Debug.WriteLine ("In EditContact");
			firstNameEntry.Text = firstNameEntry.Text.Trim();
			lastNameEntry.Text = lastNameEntry.Text.Trim();
			affEntry.Text = affEntry.Text.Trim();
			Debug.WriteLine ("Done Trimming");

			if (string.IsNullOrWhiteSpace (firstNameEntry.Text) || string.IsNullOrEmpty (firstNameEntry.Text)) {
				AlertHelper.Alert ("Hey!!!", "I don't think ' ' counts as a First Name, do you? ", "Alright, sorry CappTap...");
			} else if (string.IsNullOrWhiteSpace (lastNameEntry.Text) || string.IsNullOrEmpty (lastNameEntry.Text)) {
				AlertHelper.Alert ("Hey!!!", "How would you like if you didn't have a family? I need a last name! ", "Alright, sorry CappTap...");
			} else if (string.IsNullOrWhiteSpace (numberEntry.Text) || string.IsNullOrEmpty (numberEntry.Text)) {
				AlertHelper.Alert ("Hey!!!", "We can't call your contact if he/she doesn't have a number, now can we...? ", "Alright, sorry CappTap...");
			} else {
				Debug.WriteLine ("Input is fine");
				if (PhoneUtil.ToNumber (numberEntry.Text) == null || numberEntry.Text.Contains (" ")) {
					AlertHelper.Alert ("Hey!!!", "Please only enter numbers like 09163334444", "Alright, sorry CappTap...");
				} else {
					Debug.WriteLine ("Saving");
					contact.FirstName = firstNameEntry.Text;
					contact.LastName = lastNameEntry.Text;
					contact.Aff = affEntry.Text;
					contact.Number = numberEntry.Text;

					Debug.WriteLine ("udpated ID is:" + contact.ID);
					Debug.WriteLine ("current db index is:" + App.lastIndex);
					App.Database.UpdateItem (contact);
					page.refresh ();

					UserDialogs.Instance.HideLoading ();
					return true;
				}
			}
			UserDialogs.Instance.HideLoading ();
			return false;
		}

		void CreateEditContactLayout(ContactData contact){
			if(Number5Exists(contact)){
				EditContactStack = new StackLayout {
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
				EditContactStack = new StackLayout {
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
				EditContactStack = new StackLayout {
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
				EditContactStack = new StackLayout {
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
				EditContactStack = new StackLayout {
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
			EditContactStack.HorizontalOptions = LayoutOptions.CenterAndExpand;
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