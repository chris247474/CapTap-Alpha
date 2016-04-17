using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Net;

namespace Capp2.Droid
{
	public class SendSMSActivity:Activity {  
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle); 
		}  

		public void sendSMS(string number)  
		{  
			StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.FromParts("sms", number, null)));  
		}  

		public void sendSMS2(string number) 
	      { 
			Android.Net.Uri uri = Android.Net.Uri.Parse("smsto:"+number); 
			Intent it = new Intent(Intent.ActionSendto, uri); 
			it.PutExtra("sms_body", "Here you can set the SMS text to be sent"); 
	         StartActivity(it); 
	      }  
	}
}

