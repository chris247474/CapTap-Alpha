// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace nsoftware.GdriveDemo
{
	[Register ("AuthViewController")]
	partial class AuthViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnDone { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtAuthURL { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtDeviceCode { get; set; }

		[Action ("btnDone_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnDone_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnDone != null) {
				btnDone.Dispose ();
				btnDone = null;
			}
			if (txtAuthURL != null) {
				txtAuthURL.Dispose ();
				txtAuthURL = null;
			}
			if (txtDeviceCode != null) {
				txtDeviceCode.Dispose ();
				txtDeviceCode = null;
			}
		}
	}
}
