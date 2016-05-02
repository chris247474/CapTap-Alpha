using System;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Threading.Tasks;

namespace Capp2
{
	public static class AlertHelper
	{
		public static async Task Alert(Page page, string title, string message, string OkText = "OK",
			string AndroidToastType = Values.INFOTOAST, int AndroidTimeout = 3000)
		{
			if (Device.OS == TargetPlatform.Android) {
				switch (AndroidToastType) {
				case Values.SUCCESSTOAST:
					UserDialogs.Instance.ShowSuccess(message, AndroidTimeout);

					break;
				case Values.INFOTOAST:
					UserDialogs.Instance.InfoToast(title, message, AndroidTimeout);

					break;
				case Values.ERRORTOAST:
					UserDialogs.Instance.ShowError(message, AndroidTimeout);

					break;
				}
			} else if (Device.OS == TargetPlatform.iOS) {
				await page.DisplayAlert (title, message, OkText);
			}
		}

		public static async Task Alert(string title, string message, string OkText = "OK",
			string AndroidToastType = Values.INFOTOAST, int AndroidTimeout = 3000)
		{
			if (Device.OS == TargetPlatform.Android) {
				switch (AndroidToastType) {
				case Values.SUCCESSTOAST:
					UserDialogs.Instance.ShowSuccess(message, AndroidTimeout);

					break;
				case Values.INFOTOAST:
					UserDialogs.Instance.InfoToast(title, message, AndroidTimeout);

					break;
				case Values.ERRORTOAST:
					UserDialogs.Instance.ShowError(message, AndroidTimeout);

					break;
				}
			} else if (Device.OS == TargetPlatform.iOS) {
				await UserDialogs.Instance.AlertAsync (message, title, OkText);
			}
		}
	}
}

