using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Capp2
{
	public class StartPage:MasterDetailPage
	{
		Label MainLabel;

		public StartPage ()
		{
			this.Title = "Namelists";

			this.BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER);
			App.NavPage = new NavigationPage(new PlaylistPage()){
				BarBackgroundColor = Color.FromHex (Values.PURPLE),
				BarTextColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER),
			};
			this.Detail = App.NavPage;
			this.Master = new ContentPage { 
				BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER),
				Title = "Namelists", 
				Icon = "ic_menu_white_24dp.png",
				Content = createUI ()
			};
		}
		ScrollView createUI(){

			MainLabel = new Label{ 
				Text = ""
			};

			return new ScrollView {
				Content = new StackLayout{
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.Fill,
					VerticalOptions = LayoutOptions.StartAndExpand,
					BackgroundColor = Color.White,
					Children = { 
						/*UIBuilder.CreateSetting ("", "Settings", 
							new TapGestureRecognizer{Command = new Command(() =>
								Navigation.PushAsync (new SettingsPage())
							)}),
						UIBuilder.CreateSeparator (Color.Gray, 0.3),*/
						UIBuilder.CreateSetting ("ic_trending_up_light_blue_500_24dp.png", "My CAPP Stats", 
							new TapGestureRecognizer{Command = new Command(() =>
								{}
							)}),
						UIBuilder.CreateSeparator (Color.Gray, 0.3),
					}
				}
			};
		}
	}
}

