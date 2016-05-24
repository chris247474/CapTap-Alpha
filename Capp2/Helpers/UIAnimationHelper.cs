using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Capp2
{
	public static class UIAnimationHelper
	{
		public static async Task StartPressMeEffectOnView(View view, double scale = 1.3, uint duration = 1000){
			while (true) {
				await UIAnimationHelper.ZoomUnZoomElement (view, scale, duration, true);
				await Task.Delay (1000);
			}
		}

		public static async Task SwitchLabelText(Label lbl, string newtext, uint duration = 250){
			await lbl.FadeTo (0, duration/2, Easing.CubicInOut);
			lbl.Text = newtext;
			lbl.FadeTo (1, duration/2, Easing.CubicInOut);
			ZoomUnZoomElement (lbl, 2);
		}
		public static async Task ZoomUnZoomElement(View elem, double scaleMult = 1.3, uint duration = 250, bool pressme = false){
			var scale = elem.Scale;
			if (pressme) {
				await elem.ScaleTo(scale*scaleMult, duration/2, Easing.SinInOut);
				await elem.ScaleTo(scale, duration/2, Easing.SinInOut);
			} else {
				await elem.ScaleTo(scale*scaleMult, duration/2, Easing.CubicInOut);
				await elem.ScaleTo(scale, duration/2, Easing.CubicInOut);
			}
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
		public static async Task FlyFromLeft(View elem, UInt32 duration = 500, bool bounce = false){
			var width = Application.Current.MainPage.Width;

			var storyboard = new Animation ();
			Animation enterLeft = null;
			/*var rotation = new Animation (callback: d => elem.Rotation = d, 
				start:    button.Rotation, 
				end:      button.Rotation + 360, 
				easing:   Easing.SpringOut);


			var exitRight = new Animation (callback: d => button.TranslationX = d,
				start:    0,
				end:      width,
				easing:   Easing.SpringIn);*/

			if (bounce) {
				enterLeft = new Animation (callback: d => elem.TranslationX = d,
					start:    -width,
					end:      0,
					easing:   Easing.BounceOut);
			} else {
				enterLeft = new Animation (callback: d => elem.TranslationX = d,
					start:    -width,
					end:      0,
					easing:   Easing.CubicInOut);
			}

			storyboard.Add (0, 1, enterLeft);
			storyboard.Commit (elem, "FlyLeft", length: duration);
		}
	}
}

