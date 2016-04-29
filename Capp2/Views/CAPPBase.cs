using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Capp2
{
	public class CAPPBase:GradientContentPage, INotifyPropertyChanged
	{
		public ListView listView{ get; set;}
		public string playlist;

		public CAPPBase ()
		{
		}

		public void refresh ()
		{
			listView.ItemsSource = App.Database.GetGroupedItems(this.playlist);
		}
	}
}

