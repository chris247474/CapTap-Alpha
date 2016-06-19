using Capp2.iOS.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using Foundation;

[assembly: Dependency(typeof(NativeDialer))]
namespace Capp2.iOS.Helpers
{
    class NativeDialer : IDialer
    {
		public async Task<bool> Dial(string number)//needs async or shows error
		{
			return UIKit.UIApplication.SharedApplication.OpenUrl(
				new NSUrl("tel:" + number));
		}
    }
}
