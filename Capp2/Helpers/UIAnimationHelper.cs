using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Capp2
{
	public static class UIAnimationHelper
	{
		public static async Task ZoomUnZoomElement(View elem){
			var scale = elem.Scale;
			await elem.ScaleTo(scale*1.3, 125, Easing.CubicInOut);
			await elem.ScaleTo(scale, 125, Easing.CubicInOut);
		}
		public static async Task ShrinkUnshrinkElement(View elem){
			var scale = elem.Scale;
			await elem.ScaleTo(scale*.85, 125, Easing.CubicInOut);
			elem.ScaleTo(scale, 125, Easing.CubicInOut);

		}
	}
}

