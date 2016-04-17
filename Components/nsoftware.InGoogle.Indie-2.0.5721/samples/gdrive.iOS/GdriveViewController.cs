﻿using System;
using System.Collections;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.IO;
using nsoftware.InGoogle;

namespace nsoftware.GdriveDemo
{
	partial class GdriveViewController : UIViewController
	{

		Gdrive gdrive;
		public string authToken;

		class TableSource : UITableViewSource
		{
			protected ArrayList tableItems;
			string cellIdentifier = "TableCell";

			public TableSource (ArrayList items)
			{
				tableItems = items;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				if (tableItems == null)
					return 0;
				else
					return tableItems.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
				// if there are no cells to reuse, create a new one
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
				cell.TextLabel.Text = tableItems [indexPath.Row].ToString ();
				return cell;
			}
		}

		public GdriveViewController (IntPtr handle) : base (handle)
		{
			gdrive = new Gdrive (this);

			gdrive.OnSSLServerAuthentication += (s, e) => {
				e.Accept = true;
			};
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			var g = new UITapGestureRecognizer (() => View.EndEditing (true));
			g.CancelsTouchesInView = false; //for iOS5
			View.AddGestureRecognizer (g);

			btnList.Enabled = false;
			btnDelete.Enabled = false;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == "authSegue") {
				AuthViewController destController = (AuthViewController)segue.DestinationViewController;
				destController.sendingController = this;
			}
		}

		void RefreshList ()
		{
			try {
				ArrayList lstDocuments = new ArrayList ();

				gdrive.ResourceIndex = -1; //Clear the Document properties.
				gdrive.ListResources ();

				for (int i = 0; i < gdrive.ResourceCount; i++) {
					gdrive.ResourceIndex = i;
					lstDocuments.Add (gdrive.ResourceTitle);
				}

				tblDocuments.Source = new TableSource (lstDocuments);
				tblDocuments.ReloadData ();

			} catch (Exception ex) {
				new UIAlertView ("Error!", ex.Message, null, "OK", null).Show ();
			}

		}

		partial void btnList_TouchUpInside (UIButton sender)
		{
			try {

				gdrive.Authorization = authToken;
				RefreshList ();

				btnDownload.Enabled = true;
				btnDelete.Enabled = true;
			} catch (Exception ex) {
				new UIAlertView ("Error!", ex.Message, null, "OK", null).Show ();
			}

		}

		partial void btnDelete_TouchUpInside (UIButton sender)
		{
			try {
				if (tblDocuments.IndexPathForSelectedRow.Row < 0) {
					new UIAlertView ("Alert!", "Please select a document.", null, "OK", null).Show ();
					return;
				}

				gdrive.ResourceIndex = tblDocuments.IndexPathForSelectedRow.Row;
				gdrive.DeleteResource ();

				RefreshList ();
			} catch (Exception ex) {
				new UIAlertView ("Error!", ex.Message, null, "OK", null).Show ();
			}

		}

		partial void btnDownload_TouchUpInside (UIButton sender)
		{
			if (tblDocuments.IndexPathForSelectedRow.Row < 0) {
				new UIAlertView ("Alert!", "Please select a document.", null, "OK", null).Show ();
				return;
			}

			string docsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments);

			gdrive.ResourceIndex = tblDocuments.IndexPathForSelectedRow.Row;
			txtFilePath.Text = gdrive.ResourceOriginalName;

			if (txtFilePath.Text == "")
				new UIAlertView ("Filename Empty:", "Please select another document.", null, "OK", null).Show ();
			else {
				gdrive.LocalFile = Path.Combine (docsPath, txtFilePath.Text);
				try {
					gdrive.DownloadFile ("");
					new UIAlertView ("Download:", "Downloading " + gdrive.ResourceOriginalName, null, "OK", null).Show ();
				} catch (Exception ex) {
					new UIAlertView ("Error!", ex.Message, null, "OK", null).Show ();
				}
			}
		}

		public void authorizeGdrive ()
		{
			gdrive.Authorization = authToken;

			RefreshList ();
			btnList.Enabled = true;
			btnDelete.Enabled = true;

		}

	}
}
