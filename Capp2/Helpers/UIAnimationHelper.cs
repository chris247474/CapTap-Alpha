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
		public static async Task FlyIn(View elem, UInt32 duration = 500, bool delay = false){
			var scale = elem.Scale;
			elem.Scale *= 10;
			elem.Opacity = 0;
			if (delay)
				await Task.Delay (800);
			elem.FadeTo (1, duration, Easing.CubicInOut);
			await elem.ScaleTo(scale, duration, Easing.CubicInOut);
		}
		public static async Task ShrinkUnshrinkElement(View elem){
			var scale = elem.Scale;
			await elem.ScaleTo(scale*.85, 125, Easing.CubicInOut);
			elem.ScaleTo(scale, 125, Easing.CubicInOut);
		}
		public static async Task FlyDown(View elem, UInt32 duration = 500){
			var height = Application.Current.MainPage.Height;

			var storyboard = new Animation ();
			/*var rotation = new Animation (callback: d => elem.Rotation = d, 
				start:    button.Rotation, 
				end:      button.Rotation + 360, 
				easing:   Easing.SpringOut);


			var exitRight = new Animation (callback: d => button.TranslationX = d,
				start:    0,
				end:      width,
				easing:   Easing.SpringIn);*/

			var enterTop = new Animation (callback: d => elem.TranslationY = d,
				start:    -height,
				end:      0,
				easing:   Easing.CubicInOut);

			storyboard.Add (0, 1, enterTop);

			storyboard.Commit (elem, "FlyDown", length: duration);
		}
		public static async Task FlyFromLeft(View elem, UInt32 duration = 500){
			var width = Application.Current.MainPage.Width;

			var storyboard = new Animation ();
			/*var rotation = new Animation (callback: d => elem.Rotation = d, 
				start:    button.Rotation, 
				end:      button.Rotation + 360, 
				easing:   Easing.SpringOut);


			var exitRight = new Animation (callback: d => button.TranslationX = d,
				start:    0,
				end:      width,
				easing:   Easing.SpringIn);*/

			var enterLeft = new Animation (callback: d => elem.TranslationX = d,
				start:    -width,
				end:      0,
				easing:   Easing.CubicInOut);

			storyboard.Add (0, 1, enterLeft);
			storyboard.Commit (elem, "FlyLeft", length: duration);
		}
	}
}

