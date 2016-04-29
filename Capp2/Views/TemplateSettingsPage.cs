using System;
using System.ServiceModel.Channels;
using Xamarin.Forms;
using System.ComponentModel;
using Acr.UserDialogs;
using System.Diagnostics;

namespace Capp2
{
	public class TemplateSettingsPage:ContentPage
	{
		Label MainLabel, EmptyLabel;
		Image DoneImage;
		Editor SMSEntry, LocEntry;
		StackLayout SettingsList1, SettingsList2;
		bool SMSEntryShown = false, LocEntrySHown = false;

		public TemplateSettingsPage ()
		{
			WarnUserNotToDeleteMeetingAndTimePlaceholders ();
			Content = createUI ();
		}
		void WarnUserNotToDeleteMeetingAndTimePlaceholders(){
			DisplayAlert ("Warning", "Please don't delete the <meeting here> and <date here> parts. CapTap looks for those to automatically input your meetup details", "OK");
		}
		ScrollView createUI(){
			this.BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER);
			BindingContext = new SettingsViewModel();

			DoneImage = UIBuilder.CreateTappableImage ("clear-Small.png", LayoutOptions.Start, Aspect.AspectFit, new Command(() => {
				UIAnimationHelper.ShrinkUnshrinkElement(DoneImage);
				CheckIfUserChangedMeetingAndTimeStringTags(SMSEntry.Text, true);
			}));

			EmptyLabel = new Label{
				Text = "     "
			};
			MainLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				Text = "Text Templates",
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center
			};

			SMSEntry = new Editor ();
			SMSEntry.SetBinding (Editor.TextProperty, new Xamarin.Forms.Binding(){Path="BOMTemplateSettings"});
			SMSEntry.Completed += (sender, e) => {
				CheckIfUserChangedMeetingAndTimeStringTags(SMSEntry.Text, false);
			};

			LocEntry = new Editor ();
			LocEntry.SetBinding (Editor.TextProperty, new Xamarin.Forms.Binding(){Path="BOMLocationSettings"});

			SettingsList1 = new StackLayout{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					
					UIBuilder.CreateSetting ("", "BOM Text Template", 
						new TapGestureRecognizer {Command = new Command (() => 
						{
								UIAnimationHelper.ShrinkUnshrinkElement(SettingsList1);
								ShowOrHideTextTemplate(SMSEntry, SettingsList1, 1);
						}
						)}, true),
					UIBuilder.CreateSeparator (Color.Gray, 0.3),
				}
			};
			SettingsList2 = new StackLayout{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					UIBuilder.CreateSetting ("", "Meeting Place Text Template", 
						new TapGestureRecognizer {Command = new Command (() => 
							{
								UIAnimationHelper.ShrinkUnshrinkElement(SettingsList2);
								ShowOrHideLocTemplate(LocEntry, SettingsList2, 1);
							}
						)}, true),
					UIBuilder.CreateSeparator (Color.Gray, 0.3),
				}
			};
			return new ScrollView {
				Content = new StackLayout{
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					BackgroundColor = Color.White,
					Children = { 
						UIBuilder.CreateEmptyStackSpace(),
						UIBuilder.CreateEmptyStackSpace(),
						new StackLayout{
							Orientation = StackOrientation.Horizontal,
							Children = {
								EmptyLabel, DoneImage, MainLabel, 
							}
						},
						UIBuilder.CreateEmptyStackSpace(),
						UIBuilder.CreateSeparator (Color.Gray, 0.3),
						new StackLayout {
							Orientation = StackOrientation.Vertical,
							HorizontalOptions = LayoutOptions.Fill,
							VerticalOptions = LayoutOptions.FillAndExpand, 
							Children = {
								SettingsList2, SettingsList1, 
							}
						}
					}
				}
			};
		}
		void CheckIfUserChangedMeetingAndTimeStringTags(string input, bool poppage){
			if (!string.IsNullOrEmpty (input)) {
				if (input.ToLower ().Contains ("<meeting here>") && input.ToLower ().Contains ("<date here>")) {
					if (poppage)
						App.NavPage.Navigation.PopModalAsync ();
				} else {
					this.DisplayAlert ("Oops!", "Please don't touch the '<meeting here>' and '<date here>' tags", "OK");
				}
			} else {
				if(poppage) App.NavPage.Navigation.PopModalAsync ();
			}
		}
		void ShowOrHideTextTemplate(Editor entry, StackLayout parent, int indexToInsertAt){
			if (!SMSEntryShown) {
				entry.Focus ();
				parent.Children.Insert (indexToInsertAt, entry);
				SMSEntryShown = true;
				entry.Text = (this.BindingContext as SettingsViewModel).BOMTemplateSettings;
				Debug.WriteLine (entry.Text);
			}else{
				parent.Children.Remove (entry);
				SMSEntryShown = false;
				(this.BindingContext as SettingsViewModel).BOMTemplateSettings = (this.BindingContext as SettingsViewModel).BOMTemplateSettings.Replace ("Hi (name will auto fill), ", "");
				Debug.WriteLine ((this.BindingContext as SettingsViewModel).BOMTemplateSettings);
			}
		}
		void ShowOrHideLocTemplate(Editor entry, StackLayout parent, int indexToInsertAt){
			if (!LocEntrySHown) {
				entry.Focus ();
				parent.Children.Insert (indexToInsertAt, entry);
				LocEntrySHown = true;
				entry.Text = (BindingContext as SettingsViewModel).BOMLocationSettings;
				Debug.WriteLine (entry.Text);
			}else{
				parent.Children.Remove (entry);
				LocEntrySHown = false;
				Debug.WriteLine ((BindingContext as SettingsViewModel).BOMLocationSettings);
			}
		}
	}
}

