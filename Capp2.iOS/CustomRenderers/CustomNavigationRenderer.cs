using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Capp2.iOS;
using System.Collections.Generic;
using UIKit;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]

namespace Capp2.iOS
{
	public class CustomNavigationRenderer : NavigationRenderer
	{
		public override void PushViewController(UIKit.UIViewController viewController, bool animated)
		{
			base.PushViewController(viewController, animated);
			//NavigationBar.Translucent = true;
			//NavigationBar.BarTintColor = UIColor.White;
			//NavigationBar.ShadowImage = new UIImage ();
			//NavigationBar.BackgroundColor = UIColor.Clear;

			UIStringAttributes myTextAttrib = new UIStringAttributes();
			myTextAttrib.Font = UIFont.SystemFontOfSize(15);
			NavigationBar.TitleTextAttributes = myTextAttrib;

			//this.HidesBarsOnSwipe = true;
			//this.NavigationBarHidden = true;

			//NavigationBar.SetBackgroundImage (new UIImage (), UIBarMetrics.Default);//completely see through navbar
			//NavigationBar.TintColor = UIColor.White;

			/*var list = new List<UIBarButtonItem>();

			/*foreach (var item in TopViewController.NavigationItem.RightBarButtonItems)
			{
				if(string.IsNullOrEmpty(item.Title))
				{
					continue;
				}


				if (item.Title.ToLower() == "done")
				{
					var newItem = new UIBarButtonItem(UIBarButtonSystemItem.Done)
					{
						Action = item.Action,
						Target = item.Target
					};

					list.Add(newItem);
				}
				if (item.Title.ToLower() == "edit")
				{
					var newItem = new UIBarButtonItem(UIBarButtonSystemItem.Edit)
					{
						Action = item.Action,
						Target = item.Target
					};

					list.Add(newItem);
				}

				if (item.Title.ToLower() == "camera")
				{
					var newItem = new UIBarButtonItem(UIBarButtonSystemItem.Camera)
					{
						Action = item.Action,
						Target = item.Target
					};

					list.Add(newItem);
				}

				TopViewController.NavigationItem.RightBarButtonItems = list.ToArray();
			}*/
		}

		public override void ViewDidLayoutSubviews ()
		{
			//base.ViewDidLayoutSubviews ();
		}
		UIColor ColorFromHex (string hexValue, float alpha = 1.0f)
		{
			var colorString = hexValue.Replace ("#", "");
			if (alpha > 1.0f) {
				alpha = 1.0f;
			} else if (alpha < 0.0f) {
				alpha = 0.0f;
			}

			float red, green, blue;

			switch (colorString.Length) 
			{
			case 3 : // #RGB
				{
					red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
					green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
					blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
					return UIColor.FromRGBA(red, green, blue, alpha);
				}
			case 6 : // #RRGGBB
				{
					red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
					green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
					blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;

					return UIColor.FromRGBA(red, green, blue, alpha);
				}   

			default :
				throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

			}
		}
	}

}

