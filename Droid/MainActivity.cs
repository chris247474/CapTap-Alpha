using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using System.Collections.Generic;
using Tesseract.Droid;
using Acr.UserDialogs;
using Android.Content.Res;
using Android.Graphics;
using Xamarin.Forms;
using System.Threading.Tasks;
using Java.Util.Logging;
using XLabs.Platform.Services.Media;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using Java.IO;
using System.IO;
using Android.Util;
using Java.Text;
using Java.Util;
//using OpenCV.Android;
using Plugin.Calendars;
using Plugin.Calendars.Abstractions;
using Xamarin.Forms.Platform.Android;

namespace Capp2.Droid
{
	[Activity (Label = "CapTap"/*, Icon = "@drawable/icon", MainLauncher = true*/, 
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, Theme = "@style/CapTheme"/*"@android:style/Theme.Holo.Light"*/)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity//FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;
			FormsAppCompatActivity.TabLayoutResource = Resource.Layout.tabs;

			base.OnCreate (bundle);

            //implement corresponding Init on iOS
			#region Resolver Init
			SimpleContainer container = new SimpleContainer();
			container.Register<IDevice>(t => AndroidDevice.CurrentDevice);
			container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
			container.Register<INetwork>(t => t.Resolve<IDevice>().Network);
			container.Register<IPhoneService>(t => t.Resolve<IDevice> ().PhoneService);//for sms

            try
            {
				Resolver.SetResolver(container.GetResolver());
			}catch(Exception){
				//ResetResolver prevents crashing when you tap local notifications that bring you back to the app
				Resolver.ResetResolver (container.GetResolver ());
			}
            //implement corresponding Init on iOS
            #endregion

            global::Xamarin.Forms.Forms.Init (this, bundle);
			UserDialogs.Init(this);//no need to init in iOS
			LoadApplication (new App ());
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar> (Resource.Id.toolbar);//error with KitKat
			toolbar.SetZ (12);
			toolbar.SoundEffectsEnabled = true;
			SetSupportActionBar (toolbar);
		}
		public void trySaveToDevice(){
			try{
				PhoneContacts phone = new PhoneContacts ();
				System.Console.WriteLine ("SAVECONTACTTODEVICE result "+phone.SaveContactToDevice ("CHRISTOPHER", "REPHOTSIRHC", "09163247357", "SECRET FILES").ToString ());
			}catch(Exception e){
				System.Console.WriteLine ("SAVETODEVICE error: "+e.Message);
			}
		}
		public void trySMS(){
			PhoneService phone = new PhoneService ();
			if (PhoneService.Manager.IsSmsCapable) {
				try {
					System.Console.WriteLine ("PHONE CAN SEND SMS: ABOUT TO ATTEMPT SMS");
					phone.SendSMS ("09163247357", "TESTING AUTO TEXT");	
				} catch (Exception) {
					System.Console.WriteLine ("CANT SEND SMS IN EMULATOR");
				}
			} else {
				System.Console.WriteLine ("PHONE CANT SEND SMS IN EMULATOR");
			}
		}

	}
}

