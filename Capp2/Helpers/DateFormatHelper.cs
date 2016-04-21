using System;
using System.Globalization;

namespace Capp2
{
	public static class DateFormatHelper
	{
		public static void SetDateAppointedTemplateSetting(ContactData person, SettingsViewModel settings){
			if (string.IsNullOrWhiteSpace (settings.BOMDayTimeSettings) || 
				string.Equals (settings.BOMDayTimeSettings, "datetime_key")) {

				settings.BOMDayTimeSettings = person.Appointed.Date.ToString("m", 
					CultureInfo.CurrentCulture) + ", " + person.Appointed.Date.ToString ("t", 
						CultureInfo.CurrentCulture);
				
			}
		}
	}
}

