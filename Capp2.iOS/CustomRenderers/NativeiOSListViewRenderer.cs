using System;
using Capp2;
using Capp2.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NativeListView), typeof(NativeiOSListViewRenderer))]

namespace Capp2.iOS
{
	public class NativeiOSListViewRenderer : ListViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// Unsubscribe
			}

			if (e.NewElement != null)
			{
				//Control.Source = new NativeiOSListViewSource(e.NewElement as NativeListView);
			}
		}

		/*public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			NativeiOSListViewCell cell = tableView.DequeueReusableCell(cellIdentifier) as NativeiOSListViewCell;

			// if there are no cells to reuse, create a new one
			if (cell == null)
			{
				cell = new NativeiOSListViewCell(cellIdentifier);
			}

			if (String.IsNullOrWhiteSpace(tableItems[indexPath.Row].ImageFilename))
			{
				cell.UpdateCell(tableItems[indexPath.Row].Name
				  , tableItems[indexPath.Row].Category
				  , null);
			}
			else {
				cell.UpdateCell(tableItems[indexPath.Row].Name
				  , tableItems[indexPath.Row].Category
				  , UIImage.FromFile("Images/" + tableItems[indexPath.Row].ImageFilename + ".jpg"));
			}

			return cell;
		}*/
	}
}

