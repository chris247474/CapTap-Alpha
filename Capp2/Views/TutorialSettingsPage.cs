using System;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace Capp2
{
	public class TutorialSettingsPage : ContentPage
	{
		RelativeLayout layout = new RelativeLayout();

		public TutorialSettingsPage ()
		{

			CreateView ();
		}

		async Task CreateView(){
			layout = new RelativeLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			Content = layout;

			layout = await TutorialHelper.ShowExtraTips (this, Color.FromHex (Values.CAPPTUTORIALCOLOR_Orange), 
				"A few extra tips...", false);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			GC.Collect();
		}
	}
}
