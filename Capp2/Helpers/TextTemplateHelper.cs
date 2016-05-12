using System;
using Xamarin.Forms;
using System.ServiceModel.Channels;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Globalization;
using Capp2.Helpers;

namespace Capp2
{
	public static class TextTemplateHelper
	{
		//if on iOS, this bypasses TextTemplatePage due to Apple's restriction for only sending SMS with user input.  
		//So TextTemplatePage becomes redundant. On Android the opposite is true, so it will be shown
		public static async Task BookProspectOrMarkForCallBackDate(ContactData person, bool AutoCall, 
			SettingsViewModel settings = null)
		{

			SettingsViewModel Settings; 
			if (settings == null) {
				Settings = new SettingsViewModel ();//on iOS
			}else {
				Settings = settings;//on Android
			}
			var DefaultTemplateText = Settings.BOMTemplateSettings;

			Settings.BOMTemplateSettings = string.Format ("Hi {0}, {1}", person.FirstName, Settings.BOMTemplateSettings);
			Settings.BOMTemplateSettings = PlaceLocationAndDatesIntoTemplateText (Settings.BOMTemplateSettings, person);

			await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number, Settings.BOMTemplateSettings, person.Name, Values.BOM);
			Settings.BOMTemplateSettings = DefaultTemplateText;

			if (AutoCall) {
				if (Device.OS == TargetPlatform.Android) {
					Debug.WriteLine ("SENDING DONEWITHCALL MESSAGE");
					MessagingCenter.Send ("", Values.DONEWITHCALL);//iOS sends 'DONEWITHCALL' in IPhoneContacts impl, to leave room for iOS required manual user SMS
				}
			} else {
				try {
					
					NavigationHelper.ClearModals (App.NavPage);
					CallHelper.ShowUserYesCallTime(App.CapPage, false);
				} catch (IndexOutOfRangeException ex) {
					Debug.WriteLine ("Error popping datepage and texttemplate modals possibly due to using 'await Navigation.PopModalAsync()': {0} ", ex.Message);
				}
			}

		}

		public static string PlaceLocationAndDatesIntoTemplateText(string template, ContactData person){
			template = template.Replace ("<meeting here>", Settings.LocSettings);
			return template.Replace ("<date here>", string.Format("{0}, {1}", 
				person.Appointed.ToString("M", CultureInfo.CurrentCulture), 
				person.Appointed.ToString("t", CultureInfo.CurrentCulture)));
		}

		public static string PlaceLocationAndDatesIntoConfirmText(string template, ContactData person){
			return template.Replace ("<time here>", string.Format("{0}", 
				person.Appointed.ToString("t", CultureInfo.CurrentCulture)));
		}

		public static async Task PrepareConfirmTomorrowsMeetingsTemplateThenSendText(ContactData person){
			var DefaultTemplateText = Settings.MeetingConfirmDefault;

			string messageToSend = Settings.MeetingConfirmDefault;

			messageToSend = string.Format ("Hi {0}, {1}", person.FirstName,  
				messageToSend);

			messageToSend = 
				PlaceLocationAndDatesIntoConfirmText (messageToSend, person);

			await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number, 
				messageToSend, person.Name, Values.CONFIRM, Values.TOMORROW);

			Settings.MeetingConfirmSettings = DefaultTemplateText;
		}

		public static async Task PrepareConfirmTodaysMeetingsTemplateThenSendText(ContactData person){
			var DefaultTemplateText = Settings.MeetingTodayConfirmDefault;
			var messageToSend = Settings.MeetingTodayConfirmDefault;

			messageToSend = string.Format ("Hi {0}, {1}", person.FirstName,  
				messageToSend);

			messageToSend = 
				PlaceLocationAndDatesIntoConfirmText (messageToSend, person);

			await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number, 
				messageToSend, person.Name, Values.CONFIRM, Values.TODAY);

			Settings.MeetingTodayConfirmSettings = DefaultTemplateText;
		}
	}
}

/*if(!AutoCall){
				try {
					NavigationHelper.ClearModals(App.NavPage);
				} catch (IndexOutOfRangeException ex) {
					Debug.WriteLine("Error popping datepage and texttemplate modals possibly due to using 'await Navigation.PopModalAsync()': {0} ", ex.Message);
				} 
			}*/ 