using Android.Content;
using Android.Telephony;
using Capp2.Droid;
using System.Linq;
using Xamarin.Forms;

using Uri = Android.Net.Uri;
using System.Threading.Tasks;

[assembly: Dependency(typeof(NativeDialer))]

namespace Capp2.Droid
{
	public class NativeDialer:IDialer
	{
		public static bool IsIntentAvailable(Context context, Intent intent)
		{
			var packageManager = context.PackageManager;

			var list = packageManager.QueryIntentServices (intent, 0)
				.Union (packageManager.QueryIntentActivities (intent, 0));

			if (list.Any ())
				return true;

			var manager = TelephonyManager.FromContext (context);
			return manager.PhoneType != PhoneType.None;
		}

		public async Task<bool> Dial(string number)
		{
			var context = Forms.Context;
			if (context == null)
				return false;

			var intent = new Intent (Intent.ActionCall);
			intent.SetData (Uri.Parse ("tel:" + number));

			if (IsIntentAvailable (context, intent)) {
				context.StartActivity (intent);
				return true;
			}

			return false;
		}
	}
}

