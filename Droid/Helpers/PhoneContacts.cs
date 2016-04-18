using System;
using Xamarin.Forms;
using Capp2.Droid;
using System.Collections.Generic;
using Android.Provider;
using Android.Content;
using Android.Widget;
using Android.Telephony;
using System.Threading.Tasks;

[assembly: Dependency(typeof(PhoneContacts))]
namespace Capp2.Droid
{
    public class PhoneContacts:IPhoneContacts
	{
		public PhoneContacts(){
		}
		public async Task SendSMS(string number, string message, string name, string ConfirmOrBOM, string TodayOrTomorrow = null){
            System.Console.WriteLine("ENTERED PhoneContacts.SendSMS()");
            AndroidReminderService notifier = new AndroidReminderService ();
			try{
				SmsManager smsManager = SmsManager.Default;
				smsManager.SendTextMessage (number, null, message, null, null);
				//Toast.MakeText(MainApplication.Context, "Confirmation text sent to "+name, ToastLength.Short).Show();
				if(string.Equals(ConfirmOrBOM, Values.BOM)){
					notifier.Remind (DateTime.Now.AddMilliseconds (0), "Text Confirmation", "BOM Confirmation texted to "+name);
				}else{
					if(string.Equals (TodayOrTomorrow, Values.TODAY)){
						notifier.Remind (DateTime.Now.AddMilliseconds (0), "Confirming "+name, "Texted "+name+" for later");
					}else{
						notifier.Remind (DateTime.Now.AddMilliseconds (0), "Confirming "+name, "Texted "+name+" for tomorrow");
					}
				}
				//return true;
			}catch(Exception){
				if (string.Equals (ConfirmOrBOM, Values.BOM)) {
					notifier.Remind (DateTime.Now.AddMilliseconds (0), "Text Confirmation Failed", "BOM Confirmation failed sending to " + name);
				}else{
					
					if(string.Equals (TodayOrTomorrow, Values.TODAY)){
						notifier.Remind (DateTime.Now.AddMilliseconds (0), "Couldn't confirm "+name+" for later's meeting", "SMS failed to send");
					}else{
						notifier.Remind (DateTime.Now.AddMilliseconds (0), "Couldn't confirm "+name+" for tomorrow's meeting", "SMS failed to send");
					}
				}
				//return false;
			}

		}
		/*public bool SendSMS(string number, string message){
			AndroidReminderService notifier = new AndroidReminderService ();
			try{
				SmsManager smsManager = SmsManager.Default;
				smsManager.SendTextMessage (number, null, message, null, null);
				//Toast.MakeText(MainApplication.Context, "Confirmation text sent to "+name, ToastLength.Short).Show();
				notifier.Remind (DateTime.Now.AddMilliseconds (0), "Text Confirmation", "BOM Confirmation texted to "+name);
				return true;
			}catch(Exception e){
				notifier.Remind (DateTime.Now.AddMilliseconds (0), "Text Confirmation Failed", "BOM Confirmation failed sending to "+name);
				return false;
			}

		}*/
		public bool SaveContactToDevice(string firstName, string lastName, string phone, string aff){
			System.Console.WriteLine ("IN SAVECONTACTTODEVICE");

			List<ContentProviderOperation> ops = new List<ContentProviderOperation>();

			ContentProviderOperation.Builder builder =
				ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri);
			builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null);
			builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
			ops.Add(builder.Build());

			//Name
			builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
			builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
			builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
				ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
			builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, lastName);
			builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.GivenName, firstName);
			ops.Add(builder.Build());

			//Number
			builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
			builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
			builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
				ContactsContract.CommonDataKinds.Phone.ContentItemType);
			builder.WithValue(ContactsContract.CommonDataKinds.Phone.Number, phone);
			builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
				ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom);
			builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label, "Work");
			ops.Add(builder.Build());

			//Company
			builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
			builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
			builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
				ContactsContract.CommonDataKinds.Organization.ContentItemType);
			builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Data, aff);
			builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Type,
				ContactsContract.CommonDataKinds.Organization.InterfaceConsts.TypeCustom);
			builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Label, "Work");
			ops.Add(builder.Build());

			//Add the new contact
			ContentProviderResult[] res;
			try
			{
				res = MainApplication.Context.ContentResolver.ApplyBatch(ContactsContract.Authority, ops);

				Toast.MakeText(MainApplication.Context, "Contact saved: "+firstName+" "+lastName, ToastLength.Short).Show();
			}
			catch(Exception e)
			{
				Toast.MakeText(MainApplication.Context, "Contact not saved: "+firstName+" "+lastName+": "+e.Message, ToastLength.Long).Show();
			}
			return true;
		}
		
	}
}



