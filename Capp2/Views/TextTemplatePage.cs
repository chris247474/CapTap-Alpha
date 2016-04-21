﻿using System;
using Xamarin.Forms;
using System.Diagnostics;
using XLabs.Platform.Services;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Capp2.Helpers;

namespace Capp2
{
	public class TextTemplatePage:ContentPage
	{
		public string BOMTextTemplate{ get; set;}
		public string MeetupLoc{get;set;}
		public string MeetupDate{get;set;}
		public string Smiley{ get; set;}
		public bool useSmiley{get;set;}
		Editor SMSEntry;
		Button cmdSMS;
		Label lbl;
		string Name;
		bool AutoCall;
		ContactData person;

		/*public TextTemplatePage (string name, string meetupLoc, string meetupDate, bool smiley)
		{
			if(smiley) Smiley = ":)";
			BOMTextTemplate = " we can meet at "+meetupLoc+". We introduce our guests to the speaker then all go up together. " +
				"See you "+meetupDate+" "+Smiley+". Please reply if recieved ";

			Content = createUI ();
		}*/
		public TextTemplatePage (ContactData person, bool autocall)
		{
			AutoCall = autocall;
			this.Name = person.FirstName;
			this.person = person;

			Content = createUI ();
		}
		StackLayout createUI(){
			BackgroundColor = Color.White;
			BindingContext = new SettingsViewModel();
			
			SMSEntry = new Editor ();
			(BindingContext as SettingsViewModel).BOMTemplateSettings = string.Format ("Hi {0}, {1}", person.FirstName, (BindingContext as SettingsViewModel).BOMTemplateSettings);

			SMSEntry.SetBinding<SettingsViewModel> (Editor.TextProperty, vm => vm.BOMTemplateSettings);

			cmdSMS = new Button {Text = "Send"};
			cmdSMS.Clicked += async (sender, e) => {
				await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number, SMSEntry.Text, person.Name, Values.BOM);
				(BindingContext as SettingsViewModel).BOMTemplateSettings = (BindingContext as SettingsViewModel).BOMTemplateSettings.Replace("Hi " + person.FirstName + ", ", "");

				if(AutoCall){
					Debug.WriteLine ("SENDING DONEWITHCALL MESSAGE");
					if(Device.OS == TargetPlatform.Android) MessagingCenter.Send(this, Values.DONEWITHCALL);//iOS sends 'DONEWITHCALL' in IPhoneContacts impl, to leave room for iOS required manual user SMS
				}else{
                    try {
						NavigationHelper.ClearModals(this);
                    } catch (IndexOutOfRangeException ex) {
                        Debug.WriteLine("Error popping datepage and texttemplate modals possibly due to using 'await Navigation.PopModalAsync()': {0} ", ex.Message);
                    }
				}
            };

			lbl = new Label{ 
				Text = "Send Text",
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Start,
				HorizontalTextAlignment = TextAlignment.Center,
			};
            Label emptyLabel = new Label();
            emptyLabel.HeightRequest = 100;

			return new StackLayout{
				VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness(20),
				Children = {
					emptyLabel, lbl, new StackLayout{
						VerticalOptions = LayoutOptions.CenterAndExpand,
						Children = { SMSEntry,cmdSMS }
					}
				}
			};
		}
		protected override void OnDisappearing(){
			Debug.WriteLine ("OnDisappearing");
			if(Device.OS == TargetPlatform.iOS) App.NavPage.BarBackgroundColor = Color.FromHex (Values.GOOGLEBLUE);
			else App.NavPage.BarBackgroundColor = Color.FromHex (Values.PURPLE);
		}

	}
}

