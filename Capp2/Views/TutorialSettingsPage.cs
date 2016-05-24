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
				"Tips and Tricks\nto help you burn through those call lists", false);
		}
	}
}