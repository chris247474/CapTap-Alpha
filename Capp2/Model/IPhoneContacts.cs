using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Capp2
{
	public interface IPhoneContacts
	{
		//Task<bool> LoadDeviceContactsIntoDBAsync();
		//bool SaveDeviceContactsToDB(IEnumerable<ContactData> contacts);
		//IEnumerable<ContactData> FormatContactsForDB(IEnumerable<ContactData> raw);
		//IEnumerable<ContactData> GetAllPhoneContacts();
		bool SaveContactToDevice (string firstName, string lastName, string phone, string aff);
		Task SendSMS (string number, string message, string name, string ConfirmOrBOM, string TodayOrTomorrow = null);
	}
}

