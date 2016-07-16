using System;
using Capp2.Android;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl_Android))]
namespace Capp2.Android
{
	public class BaseUrl_Android : IBaseUrl
	{
		public string Get()
		{
			return "file:///android_asset/";
		}
	}
}
