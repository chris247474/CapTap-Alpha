using System;
using Android.App;
using Android.OS;
using Android.Content;
using Android.Content.PM;

namespace Capp2.Droid
{
	[Activity (Label = "CapTap", Icon = "@drawable/icon", MainLauncher = true, NoHistory = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, Theme = "@style/Theme.Splash"/*"@android:style/Theme.Holo.Light"*/)]

	public class SplashScreen:Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			var intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
			//Finish();
		}
	}
}

