using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Capp2
{
	public class NativeListView : ListView
	{
		public static readonly BindableProperty ItemsProperty =
			BindableProperty.Create("Items", typeof(IEnumerable<ContactData>), typeof(NativeListView), new List<ContactData>());

		public IEnumerable<ContactData> Items
		{
			get { return (IEnumerable<ContactData>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

		public void NotifyItemSelected(object item)
		{
			if (ItemSelected != null)
			{
				ItemSelected(this, new SelectedItemChangedEventArgs(item));
			}
		}
	}
}

