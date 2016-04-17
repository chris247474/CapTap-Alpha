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
	[Register ("GdriveViewController")]
	partial class GdriveViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnAuthorize { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnDelete { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnDownload { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnList { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView tblDocuments { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtFilePath { get; set; }

		[Action ("btnDelete_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnDelete_TouchUpInside (UIButton sender);

		[Action ("btnDownload_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnDownload_TouchUpInside (UIButton sender);

		[Action ("btnList_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnList_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnAuthorize != null) {
				btnAuthorize.Dispose ();
				btnAuthorize = null;
			}
			if (btnDelete != null) {
				btnDelete.Dispose ();
				btnDelete = null;
			}
			if (btnDownload != null) {
				btnDownload.Dispose ();
				btnDownload = null;
			}
			if (btnList != null) {
				btnList.Dispose ();
				btnList = null;
			}
			if (tblDocuments != null) {
				tblDocuments.Dispose ();
				tblDocuments = null;
			}
			if (txtFilePath != null) {
				txtFilePath.Dispose ();
				txtFilePath = null;
			}
		}
	}
}
