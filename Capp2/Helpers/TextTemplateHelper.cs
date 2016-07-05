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

			//Settings.BOMTemplateSettings = string.Format ("Hi {0}, {1}", person.FirstName, Settings.BOMTemplateSettings);
			Settings.BOMTemplateSettings = PlaceNameLocationAndDatesIntoTemplateText (Settings.BOMTemplateSettings, person);
			Debug.WriteLine("Inserting message into SMS: {0}", Settings.BOMTemplateSettings);

			await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number, Settings.BOMTemplateSettings, 
				person.Name, Values.BOM, AutoCall);
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

		public static string PlaceNameLocationAndDatesIntoTemplateText(string template, ContactData person){
			template = template.Replace(Values.NAMETEMPLATE, person.FirstName);
			template = template.Replace (Values.MEETINGTEMPLATE, Settings.LocSettings);
			return template.Replace (Values.DATETEMPLATE, string.Format("{0}, {1}", 
				person.Appointed.ToString("M", CultureInfo.CurrentCulture), 
				person.Appointed.ToString("t", CultureInfo.CurrentCulture)));
		}

		public static string PlaceNameAndDatesIntoConfirmText(string template, ContactData person){
			template = template.Replace(Values.NAMETEMPLATE, person.FirstName);
			return template.Replace ("<time here>", string.Format("{0}", 
				person.Appointed.ToString("t", CultureInfo.CurrentCulture)));
		}

		public static async Task PrepareConfirmTomorrowsMeetingsTemplateThenSendText(ContactData person){
			if (!person.IsConfirmedTomorrow) {
				Debug.WriteLine ("Meeting confirm tomorrow text: {0}", Settings.MeetingConfirmDefault);

				string messageToSend = Settings.MeetingConfirmDefault;

				messageToSend = string.Format ("Hi {0}, {1}", person.FirstName,  
					messageToSend);

				messageToSend = 
					PlaceNameAndDatesIntoConfirmText (messageToSend, person);
				Debug.WriteLine ("inserting values into message: {0}", messageToSend);

				await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number, 
					messageToSend, person.Name, Values.CONFIRM, false, Values.TOMORROW);

				messageToSend = string.Empty;

				person.IsConfirmedTomorrow = true;
				App.Database.UpdateItem (person);
			}
		}

		public static async Task PrepareConfirmTodaysMeetingsTemplateThenSendText(ContactData person){
			if (!person.IsConfirmedToday) {
				Debug.WriteLine ("Confirm Today default: {0}", Settings.MeetingTodayConfirmDefault);
				var messageToSend = Settings.MeetingTodayConfirmDefault;

				messageToSend = string.Format ("Hi {0}, {1}", person.FirstName,  
					messageToSend);

				messageToSend = 
					PlaceNameAndDatesIntoConfirmText (messageToSend, person);
				Debug.WriteLine ("inserting values into message: {0}", messageToSend);

				await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number, 
					messageToSend, person.Name, Values.CONFIRM, false, Values.TODAY);

				messageToSend = string.Empty;
				Debug.WriteLine ("Reset messageToSend: {0}", messageToSend);

				person.IsConfirmedToday = true;
				App.Database.UpdateItem (person);
			}
		}
	}
}
