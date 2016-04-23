using System;
using Capp2;
using Capp2.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Globalization;
using CoreAnimation;
using CoreGraphics;

[assembly: ExportRenderer(typeof(GradientContentPage), typeof(GradientContentPageRenderer))]

namespace Capp2.iOS
{
	public class GradientContentPageRenderer: PageRenderer
	{
		public override void ViewWillAppear(bool animated)
		{
			
			base.ViewWillAppear(animated);

			if(App.OnAppStart)
			{
				App.Width = View.Bounds.Width.ToString(CultureInfo.InvariantCulture);
				App.Height = View.Bounds.Height.ToString(CultureInfo.InvariantCulture);
				App.OnAppStart = false;
			}

			var gradientLayer = new CAGradientLayer {
				//This line had to be changed to be able to draw the background correctly
				Frame = new CGRect{ Width = nfloat.Parse (App.Width), Height = nfloat.Parse (App.Height) },
				Colors = new [] { App.StartColor.ToCGColor (), App.EndColor.ToCGColor () },
				StartPoint = new CGPoint (0, 0),
				EndPoint = new CGPoint (1, 1)
			};

			//This is needed to get every background redrawn if the color changes on runtime
			if(View.Layer.Sublayers[0].GetType() == typeof(CAGradientLayer))
			{
				View.Layer.ReplaceSublayer(View.Layer.Sublayers[0], gradientLayer);
			}
			else
			{
				View.Layer.InsertSublayer(gradientLayer, 0);
			}

		}
	}
}

