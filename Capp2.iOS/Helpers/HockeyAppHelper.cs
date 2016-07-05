using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(Capp2.iOS.HockeyAppHelper))]

namespace Capp2.iOS
{
	public class HockeyAppHelper:IHockeyAppHelper
	{
		public HockeyAppHelper()
		{
		}

		public void SetupHockeyApp()
		{
			throw new NotImplementedException();
		}
	}
}

