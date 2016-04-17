using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using nsoftware.InGoogle;

namespace nsoftware.GdriveDemo
{
	partial class AuthViewController : UIViewController
	{
		Oauth oauth;
		String authURL;
		public GdriveViewController sendingController;


		public AuthViewController (IntPtr handle) : base (handle)
		{
			oauth = new Oauth (this);

			oauth.OnSSLServerAuthentication += (s, e) => {
				e.Accept = true;
			};
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var g = new UITapGestureRecognizer (() => View.EndEditing (true));
			g.CancelsTouchesInView = false; //for iOS5
			View.AddGestureRecognizer (g);

			try {
				oauth.ClientProfile = OauthClientProfiles.cfDevice;
				oauth.ClientId = "157623334268-pdch1uarb3180t5hq2s16ash9ei315j0.apps.googleusercontent.com";
				oauth.ClientSecret = "k4NSk71U-p2sU8lB8Qv3G24R";
				oauth.ServerAuthURL = "https://accounts.google.com/o/oauth2/device/code";
				oauth.ServerTokenURL = "https://accounts.google.com/o/oauth2/token";
				oauth.AuthorizationScope = "https://docs.google.com/feeds";	

				authURL = oauth.GetAuthorizationURL();

				txtAuthURL.Text = authURL;
				txtDeviceCode.Text = oauth.Config("DeviceUserCode");


			} catch (Exception ex) {
				new UIAlertView ("Error!", ex.Message, null, "OK", null).Show ();
			}
		}

		partial void btnDone_TouchUpInside (UIButton sender)
		{
			try{
				sendingController.authToken = oauth.GetAuthorization();
				if (sendingController.authToken != ""){
					sendingController.authorizeGdrive();
					DismissViewController(true, null);
				}
				else
					new UIAlertView("Token not retreived!", "Please follow instructions above and/or tap \"Done\" again.", null, "OK", null).Show();
			} catch (Exception ex) {
				new UIAlertView ("Error!", ex.Message, null, "OK", null).Show ();
			}
		}
	}
}
