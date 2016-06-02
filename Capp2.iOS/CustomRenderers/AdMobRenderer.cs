﻿using System;
using Xamarin.Forms.Platform.iOS;
using Capp2;
using Capp2.iOS;
using Xamarin.Forms;
using Google.MobileAds;
using UIKit;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobRenderer))]

namespace Capp2.iOS
{
	
	public class AdMobRenderer : ViewRenderer
	{
		const string AdmobID = "ca-app-pub-6161089310557130/2498669604";

		BannerView adView;
		bool viewOnScreen;

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
		{
			Console.WriteLine ("OnElementChanged AdMobRenderer");
			base.OnElementChanged(e);

			if (e.NewElement == null)
				return;

			if (e.OldElement == null)
			{
				Console.WriteLine ("in e.OldElement condition"); 

				adView = new BannerView(AdSizeCons.SmartBannerPortrait)
				{
					AdUnitID = AdmobID,
					RootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController,//UIApplication.SharedApplication.Windows[0].RootViewController
				};

				Console.WriteLine ("adView initialized");

				adView.AdReceived += (sender, args) => 
				{
					if (!viewOnScreen) this.AddSubview(adView);
					viewOnScreen = true;
				};

				Console.WriteLine ("adView AdReceived");

				Request request = Request.GetDefaultRequest();

				Console.WriteLine ("Got Request");

				adView.LoadRequest(request);
				//request.testDevices = @[ kGADSimulatorID ];

				Console.WriteLine ("loadedrequest");

				base.SetNativeControl(adView);
				Console.WriteLine ("native control set");

			}
		}
	}
}

