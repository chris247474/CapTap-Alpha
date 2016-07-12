using System;
using Xamarin.Forms;
using System.ServiceModel.Channels;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XLabs.Forms.Controls;

namespace Capp2
{
	public class AddContactPage:ContentPage
	{
		CAPP capp;
		ContactData AddContact;
		string playlist;
		Label firstNameLabel, lastNameLabel, numberLabel, affLabel;
		Entry firstNameEntry, lastNameEntry, numberEntry, affEntry;
		StackLayout MainStack = new StackLayout();
		CircleImage ContactPic = new CircleImage ();
		BoxView shader;
		DetailsView details;
		Image phoneImg = new Image(), messageImg = new Image(), backgroundImage = new Image(),
		dome = new Image();
		RelativeLayout relativeLayout = new RelativeLayout ();
		double layoutyposition;
		string imageColor;

		public AddContactPage (CAPP capp, ContactData contactDuplicate = null)
		{
			this.capp = capp;
			this.playlist = capp.playlist;

			this.Title = "Adding to "+this.playlist+" namelist";

			NavigationPage.SetHasNavigationBar (this, true);

			CreateUIElements ();
			CreateLayouts ();

			Content = UIBuilder.AddFloatingActionButtonToRelativeLayout(relativeLayout, "Checkmark.png", 
				new Command(() => {
					Add();
				}), Color.FromHex(Values.YELLOW), Color.FromHex(Values.PURPLE));
		}

		void SlideUpUIOnKeyboardFocus(object sender, FocusEventArgs e){
			layoutyposition = relativeLayout.Y;
			relativeLayout.TranslateTo (relativeLayout.X, backgroundImage.Y - (backgroundImage.Height*0.9), 
				400, Easing.SinInOut);
		}
		void SlideBackUIOnKeyboardUnfocus(object sender, FocusEventArgs e){
			relativeLayout.TranslateTo (relativeLayout.X, layoutyposition, 400, Easing.SinInOut);
		}

		void CreateUIElements(){
			imageColor = UIBuilder.ChooseRandomProfilePicBackground (App.ProfileBackground);


			firstNameLabel = new Label { Text = "First Name" };
			firstNameEntry = new Entry ();
			firstNameEntry.IsEnabled = true;
			firstNameEntry.Focused += SlideUpUIOnKeyboardFocus;
			firstNameEntry.Unfocused += SlideBackUIOnKeyboardUnfocus;

			lastNameLabel = new Label { Text = "Last Name" };
			lastNameEntry = new Entry ();
			lastNameEntry.Focused += SlideUpUIOnKeyboardFocus;
			lastNameEntry.Unfocused += SlideBackUIOnKeyboardUnfocus;

			numberLabel = new Label { Text = "Number" };
			numberEntry = new Entry ();
			numberEntry.Focused += SlideUpUIOnKeyboardFocus;
			numberEntry.Unfocused += SlideBackUIOnKeyboardUnfocus;
			//numberEntry.TextColor = Color.White;
			if(string.IsNullOrWhiteSpace (numberEntry.Text)){
				numberEntry.Text = "09";
			}

			affLabel = new Label { Text = "Affiliation" };
			affEntry = new Entry ();
			affEntry.Focused += SlideUpUIOnKeyboardFocus;
			affEntry.Unfocused += SlideBackUIOnKeyboardUnfocus;

			backgroundImage = new Image () {
				Source = imageColor,
				Aspect = Aspect.AspectFill,
				IsOpaque = true,
				Opacity = 0.8,
				//BackgroundColor = Color.FromHex(Values.GOOGLEBLUE),
			};

			dome = new Image () {
				Aspect = Aspect.AspectFill,
				Source = new FileImageSource () { File = "dome.png" }
			};

			shader = new BoxView () {
				Color = Color.Transparent.MultiplyAlpha(0.9),
			};

			ContactPic = UIBuilder.CreateTappableCircleImage (imageColor, 
				LayoutOptions.Fill, Aspect.Fill, new Command(()=>{}));
			ContactPic.BackgroundColor = Color.White;

			MainStack = new StackLayout {
				VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness(20),
				Children = {
					UIBuilder.CreateEmptyStackSpace(),
					UIBuilder.CreateEmptyStackSpace(),

					firstNameLabel, firstNameEntry,
					lastNameLabel, lastNameEntry,
					numberLabel, numberEntry,
					affLabel, affEntry,
				}
			};
		}

		void CreateLayouts(){
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

			relativeLayout.Children.Add (
				shader,
				Constraint.Constant (0),
				Constraint.Constant (0),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Height * .4;
				})
			);

			relativeLayout.Children.Add (
				dome,
				Constraint.Constant (-10),
				Constraint.RelativeToParent ((parent) => {
					return (parent.Height * .4) - 50;
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width + 10;
				}),
				Constraint.Constant (75)
			);

			relativeLayout.Children.Add (
				ContactPic, 
				Constraint.RelativeToParent ((parent) => {
					return ((parent.Width / 2) - (ContactPic.Width / 2));
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Height * .22;
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width * .5;
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width * .5;
				})
			);

			relativeLayout.Children.Add (
				MainStack,
				Constraint.Constant (0),
				Constraint.RelativeToView (dome, (parent, view) => {
					return view.Y + (view.Height);
				}),
				Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				})/*,
				Constraint.Constant (120)*/
			);

			ContactPic.SizeChanged += (sender, e) => {
				relativeLayout.ForceLayout ();
			};

			this.Content = relativeLayout;
		}

		async Task Add(){
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
					PicStringBase64 = imageColor,
					LargePic = imageColor,
				};

				//check if added contact details already exists in playlist
				if(!Util/*App.contactFuncs*/.duplicateExists (AddContact, App.Database.GetItems (this.playlist).ToArray ()/*.ToList ()*/)){
					if(Util/*App.contactFuncs*/.saveContactToDB (false, AddContact, this.playlist)){
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

				this.Navigation.PopModalAsync ();
			}
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			MessagingCenter.Send("", Values.DONEADDINGCONTACT);
		}
	}
}

