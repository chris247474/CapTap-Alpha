using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Capp2
{
	public class CAPPBase:/*GradientContentPage,*/ContentPage//, INotifyPropertyChanged
	{
		public ListView listView{ get; set;}
		public string playlist;
		bool contactsSelected = false;

		public CAPPBase ()
		{
		}

		protected void SelectAll(IEnumerable<ContactData> list, bool refresh = true, bool IsModal = false)
		{
			Debug.WriteLine("In SelectAll()");
			var contacts = list.ToArray();
			var count = contacts.Length;
			for (int c = 0; c < count; c++)
			{
				contacts[c].IsSelected = true;
			}
			App.Database.UpdateAll(list);
			if (refresh)
			{
				//if (IsModal) App.CapModal.refresh();
				//else App.CapPage.refresh();
				this.refresh();
			}
			contactsSelected = true;
		}

		protected void SelectDeselectAll(IEnumerable<ContactData> list, bool refresh = true, bool IsModal = false)
		{
			if (contactsSelected) DeselectAll(list, null, refresh, IsModal);
			else SelectAll(list, refresh, IsModal);
		}

		protected async Task<int> DeselectAll(IEnumerable<ContactData> list, CAPPBase capp, bool refresh = true
										   , bool IsModal = false)
		{
			ContactData[] arr = list.ToArray();
			for (int c = 0; c < arr.Length; c++)
			{
				arr[c].IsSelected = false;
			}
			var updateResult = App.Database.UpdateAll(arr.AsEnumerable());
			if (refresh)
			{
				//if (IsModal) App.CapModal.refresh();
				//else App.CapPage.refresh();
				this.refresh();
			}
			contactsSelected = false;
			return updateResult;
		}

		protected async Task ShowSelectAllFAB(RelativeLayout layout, FAB.Forms.FloatingActionButton fab, Image img,
		                                     double xAdjustment = 0, double yAdjustment = 0)
		{
			fab.Opacity = 0;
			img.Opacity = 0;
			//xamarin forms bug? When calling SHowSelectAllFAB a 2nd time, 
			//fab is added 41 pts to the left even without any code telling it to do so, so one workaround is to 
			//add to layout then remove it from layout to 'skip' that situation
			layout.Children.Add(fab,
								Constraint.RelativeToParent(parent =>
			                                                (parent.Width - fab.Width) - 37 + xAdjustment),
								Constraint.RelativeToParent(parent =>
			                                                (parent.Height - fab.Height) - 100 + yAdjustment)
							   );
			layout.Children.Remove(fab);
			layout.Children.Add(fab,
								Constraint.RelativeToParent(parent =>
			                                                (parent.Width - fab.Width) - 37+ xAdjustment),
								Constraint.RelativeToParent(parent =>
			                                                (parent.Height - fab.Height) - 100+yAdjustment)
							   );

			layout.Children.Add(img,
								Constraint.RelativeToParent(parent =>
															fab.X + (fab.Width * 0.25)),
								Constraint.RelativeToParent(parent =>
															fab.Y + (fab.Height * 0.25)),
								Constraint.RelativeToParent(parent => fab.Width * 0.5),
								Constraint.RelativeToParent(parent => fab.Height * 0.5)
							   );

			Debug.WriteLine("layout: {0}, {1}", layout.Width, layout.Height);
			Debug.WriteLine("fab: {0}, {1}", fab.Width, fab.Height);
			Debug.WriteLine("fab calculation on layout: {0}, {1}",
							(layout.Width - fab.Width) - 37, (layout.Height - fab.Height) - 100);
			Debug.WriteLine("actual fab on layout {0}, {1}", fab.X, fab.Y);
			fab.FadeTo(1, 250, Easing.Linear);
			img.FadeTo(1, 250, Easing.Linear);
			img.TranslateTo(0, -fab.Height, 300, Easing.CubicInOut);
			await fab.TranslateTo(0, -fab.Height, 300, Easing.CubicInOut);
			Debug.WriteLine(
				"Moved:X SelectAllFAB X: {0}, SelectAllFAB Y: {1} - total width, height: {2}, {3}",
				fab.X, fab.Y, layout.Width, layout.Height);
		}

		protected async Task HideSelectAllFAB(RelativeLayout layout, FAB.Forms.FloatingActionButton fab, Image img)
		{
			fab.TranslateTo(0, +fab.Height, 300, Easing.CubicInOut);
			img.TranslateTo(0, +fab.Height, 300, Easing.CubicInOut);
			Debug.WriteLine("HideSelectAllFAB X: {0}, HideSelectAllFAB Y: {1}", fab.X, fab.Y);
			img.FadeTo(0, 200, Easing.Linear);
			await fab.FadeTo(0, 200, Easing.Linear);
			layout.Children.Remove(fab);
			layout.Children.Remove(img);
			Debug.WriteLine("FAB removed from layout - HideSelectAllFAB X: {0}, HideSelectAllFAB Y: {1}", fab.X, fab.Y);
		}

		public void refresh ()
		{
			listView.ItemsSource = App.Database.GetGroupedItems(this.playlist);

		}
	}
}

