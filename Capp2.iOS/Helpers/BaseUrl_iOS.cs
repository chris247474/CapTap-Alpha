using System;
using Capp2.iOS;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl_iOS))]
namespace Capp2.iOS
{
	public class BaseUrl_iOS : IBaseUrl
	{
		public string Get()
		{
			return NSBundle.MainBundle.BundlePath;
		}
	}
}
