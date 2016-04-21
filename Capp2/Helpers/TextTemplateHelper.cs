using System;
using Xamarin.Forms;
using System.ServiceModel.Channels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Capp2
{
	public static class TextTemplateHelper
	{
		public static async Task BookProspectOrMarkForCallBackDate(ContactData person, bool AutoCall){
			var Settings = new SettingsViewModel(); 
			Settings.BOMTemplateSettings = string.Format ("Hi {0}, {1}", person.FirstName, Settings.BOMTemplateSettings);

			await DependencyService.Get<IPhoneContacts>().SendSMS(person.Number, Settings.BOMTemplateSettings, person.Name, Values.BOM);
			Settings.BOMTemplateSettings = Settings.BOMTemplateSettings.Replace("Hi " + person.FirstName + ", ", "");

			if(!AutoCall){
				try {
					NavigationHelper.ClearModals(App.NavPage);
				} catch (IndexOutOfRangeException ex) {
					Debug.WriteLine("Error popping datepage and texttemplate modals possibly due to using 'await Navigation.PopModalAsync()': {0} ", ex.Message);
				}
			}
		}
	}
}

