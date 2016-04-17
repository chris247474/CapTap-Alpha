using System;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Capp2
{
	public class AzureListViewTest : ContentPage
	{
		ListView listView;
		List<ContactDataItemAzure> list = new List<ContactDataItemAzure>();
		public AzureListViewTest ()
		{
			GetAzureData ();

		}
		public async void GetAzureData(){//DB records getter must be in same thread as listview initializer. ugh. 18 hrs... Now to replace local DB with Azure DB
			//this.list =;

			if (this.list == null)
				Debug.WriteLine ("list is null in GetAzureData");
			else
				Debug.WriteLine ("list is not null in GetAzureData");

			var arr = this.list.ToArray ();
			for (int c = 0; c < arr.Length; c++) {
				Debug.WriteLine ("From Azure in GetAzureData: "+c+". "+arr[c].Name + " " + arr[c].Number);
			}

			Debug.WriteLine ("Assigning AzureDB.GetItems as listView.ItemSource");
			listView = new ListView{
				ItemsSource =  await App.AzureDB.GetGroupedItems (Values.ALLPLAYLISTPARAM),//new List<ContactDataItemAzure>{new ContactDataItemAzure {Name = "Test1"}, new ContactDataItemAzure {Name = "Test2"}},
				ItemTemplate = new DataTemplate(() => {
					return new TestViewCell ();
				}),
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Binding("Key"),
				HasUnevenRows = true,
				GroupShortNameBinding = new Binding ("Key"),//doesnt work android
				GroupHeaderTemplate = new DataTemplate (() => {
					return new HeaderCell ();
				})
			};

			this.Content = new StackLayout { 
				Orientation = StackOrientation.Vertical,
				Children = {
					new Label { Text = "Hello ContentPage"}, listView
				}
			};
		}
	}
	class TestViewCell:ViewCell{
		Label nameLabel;
		public TestViewCell(){
			nameLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalTextAlignment = TextAlignment.Start
			};
			nameLabel.SetBinding(Label.TextProperty, new Xamarin.Forms.Binding(){Path="Name"});

			View = createView ();
		}
		StackLayout createView(){
			return new StackLayout{
				Orientation = StackOrientation.Horizontal,
				Children = {nameLabel}
			};
		}
	}
}


