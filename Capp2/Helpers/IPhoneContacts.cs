using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Capp2
{
	public interface IPhoneContacts
	{
		Task<List<ContactData>> GetProfilePicPerPerson (List<ContactData> contacts);
		bool SaveContactToDevice (string firstName, string lastName, string phone, string aff);
		Task<bool> SendSMS (string number, string message, string name, string ConfirmOrBOM, bool AutoCall =  false, 
			string TodayOrTomorrow = null);
		Task SendSMS (string number);
		Task Share (string message);

		Task PresentNativeAddContactView ();
	}
}

