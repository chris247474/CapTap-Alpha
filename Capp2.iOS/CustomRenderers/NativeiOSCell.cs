using System;
using UIKit;
using Foundation;

namespace Capp2.iOS
{
	public class NativeiOSCell : UITableViewCell
	{
		UILabel headingLabel, subheadingLabel;
		UIImageView imageView;

		public NativeiOSCell (NSString cellId) : base (UITableViewCellStyle.Default, cellId)
		{
			SelectionStyle = UITableViewCellSelectionStyle.Gray;

			ContentView.BackgroundColor = UIColor.FromRGB (255, 255, 224);

			imageView = new UIImageView ();

			headingLabel = new UILabel () {
				Font = UIFont.FromName ("Cochin-BoldItalic", 22f),
				//TextColor = UIColor.FromRGB (127, 51, 0),
				BackgroundColor = UIColor.Clear
			};

			subheadingLabel = new UILabel () {
				Font = UIFont.FromName ("AmericanTypewriter", 12f),
				//TextColor = UIColor.FromRGB (38, 127, 0),
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			ContentView.Add (headingLabel);
			ContentView.Add (subheadingLabel);
			ContentView.Add (imageView);
		}

		public void UpdateCell (string caption, string subtitle, UIImage image)
		{
			headingLabel.Text = caption;
			subheadingLabel.Text = subtitle;
			imageView.Image = image;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			headingLabel.Frame = new CoreGraphics.CGRect (5, 4, ContentView.Bounds.Width - 63, 25);
			subheadingLabel.Frame = new CoreGraphics.CGRect (100, 18, 100, 20);
			imageView.Frame = new CoreGraphics.CGRect (ContentView.Bounds.Width - 63, 5, 33, 33);
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

