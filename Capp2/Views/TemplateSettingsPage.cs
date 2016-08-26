using System;
using System.ServiceModel.Channels;
using Xamarin.Forms;
using System.ComponentModel;
using Acr.UserDialogs;
using System.Diagnostics;
using Capp2.Helpers;

namespace Capp2
{
	public class TemplateSettingsPage:ContentPage
	{
		Label MainLabel, EmptyLabel;
		Editor SMSEntry, LocEntry, DailyEmailEntry;
		StackLayout SettingsList1, SettingsList2, DailyEmailTemplate;
		bool SMSEntryShown = false, LocEntrySHown = false, EmailShown = false;
		Button RestoreButton;

		public TemplateSettingsPage ()
		{
			var scroller = createUI ();
			AdHelper.AddGreenURLOrangeTitleBannerToStack (scroller.Content as StackLayout);
			Content = scroller;
		}
		void WarnUserNotToDeleteMeetingAndTimePlaceholders(){
			AlertHelper.Alert("Warning", string.Format("Please don't delete the <meeting here> and <date here> parts. " +
			                                           "{0} looks for those to automatically input your meetup details"), 
			                  Values.APPNAME);
		}
		ScrollView createUI(){
			this.BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER);
			BindingContext = new SettingsViewModel();

			RestoreButton = new Button { 
				Command = new Command(() => { }),
				Text = "Restore Default",
				BackgroundColor = Color.FromHex(Values.GOOGLEBLUE),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
			};

			EmptyLabel = new Label{
				Text = "     "
			};
			MainLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				Text = "Message Templates",
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center
			};

			SMSEntry = new Editor ();
			SMSEntry.SetBinding (Editor.TextProperty, new Xamarin.Forms.Binding(){Path="BOMTemplateSettings"});
			SMSEntry.Completed += (sender, e) => {
				CheckIfUserChangedNameMeetingAndTimeStringTags(SMSEntry.Text, false);
			};
			SMSEntry.HorizontalOptions = LayoutOptions.FillAndExpand;

			LocEntry = new Editor ();
			LocEntry.SetBinding (Editor.TextProperty, new Xamarin.Forms.Binding(){Path="BOMLocationSettings"});
			LocEntry.Completed += (sender, e) => {
				if(!LocationEntryIsValid()){
					LocEntry.Focus();
				}
			};
			LocEntry.HorizontalOptions = LayoutOptions.FillAndExpand;

			DailyEmailEntry = new Editor ();
			DailyEmailEntry.SetBinding<SettingsViewModel> (Editor.TextProperty, vm => vm.DailyEmailTemplateSettings);
			DailyEmailEntry.HorizontalOptions = LayoutOptions.FillAndExpand;
			DailyEmailTemplate = new StackLayout{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {

					UIBuilder.CreateSetting ("", "Daily Report", 
						new TapGestureRecognizer {Command = new Command (() => 
							{
								UIAnimationHelper.ShrinkUnshrinkElement(DailyEmailTemplate);
								ShowOrHideEmailTemplate(DailyEmailEntry, DailyEmailTemplate, 1);
							}
						)}, true),
					UIBuilder.CreateSeparator (Color.Gray, 0.3),
				}
			};
			SettingsList1 = new StackLayout{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					
					UIBuilder.CreateSetting ("", "Confirm Message", 
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
					UIBuilder.CreateSetting ("", "Usual Meetup Area", 
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
								EmptyLabel, 
								UIBuilder.CreateModalXPopper (new Command(() => {
									//if(LocationEntryIsValid()){
										CheckIfUserChangedNameMeetingAndTimeStringTags(SMSEntry.Text, true);
									//}

								}), "Message Templates")
								//, MainLabel, 
							}
						},
						UIBuilder.CreateEmptyStackSpace(),
						UIBuilder.CreateSeparator (Color.Gray, 0.3),
						new StackLayout {
							Orientation = StackOrientation.Vertical,
							HorizontalOptions = LayoutOptions.Fill,
							VerticalOptions = LayoutOptions.FillAndExpand, 
							Padding = new Thickness(5),
							Children = {
								SettingsList2, SettingsList1,// DailyEmailTemplate
								UIBuilder.CreateTextTemplateSetting(
									new Xamarin.Forms.Binding(){Path="DailyEmailTemplateSettings"}, 
									BindingContext as SettingsViewModel,
									"Daily Report Template", ""
								),
							}
						}
					}
				}
			};
		}
		bool LocationEntryIsValid() {
			if (string.IsNullOrWhiteSpace(LocEntry.Text)) {
				this.DisplayAlert("Error", "We can't leave the location box blank. Please fill it in", "OK");
				return false;
			}
			return true;
		}
		void CheckIfUserChangedNameMeetingAndTimeStringTags(string input, bool poppage){
			if (!string.IsNullOrEmpty (input)) {
				if (input.ToLower ().Contains ("<meeting here>") && input.ToLower ().Contains ("<date here>")
				    && input.ToLower().Contains(Values.NAMETEMPLATE)) {
					if (poppage)
						App.NavPage.Navigation.PopModalAsync ();
				} else {
					this.DisplayAlert ("Oops!", "Please don't touch the '"+Values.NAMETEMPLATE
					                   +"', '<meeting here>' and '<date here>' tags", "OK");
				}
			} else {
				if(poppage) App.NavPage.Navigation.PopModalAsync ();
				//this.DisplayAlert("Bad Entry", "We can't leave any info blank. Please fill in all the fields", "OK");
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
				if (LocationEntryIsValid()) { 
					parent.Children.Remove(entry);
					LocEntrySHown = false;
					Debug.WriteLine((BindingContext as SettingsViewModel).BOMLocationSettings);
				}
			}
		}
		void ShowOrHideEmailTemplate(Editor entry, StackLayout parent, int indexToInsertAt){
			if (!EmailShown) {
				entry.Focus ();
				parent.Children.Insert (indexToInsertAt, entry);
				EmailShown = true;
				entry.Text = Settings/*(BindingContext as SettingsViewModel)*/.DailyEmailTemplateSettings;
				Debug.WriteLine (entry.Text);
			}else{
				parent.Children.Remove (entry);
				EmailShown = false;
				Debug.WriteLine (Settings/*(BindingContext as SettingsViewModel)*/.DailyEmailTemplateSettings);
			}
		}
	}
}

