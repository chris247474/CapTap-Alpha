using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Capp2
{
	public class StartPage:MasterDetailPage
	{

		public StartPage ()
		{
			this.Title = "Namelists";

			//this.BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER);
			App.NavPage = new NavigationPage(new PlaylistPage{StartColor = App.StartColor, EndColor = App.EndColor}){
				BarBackgroundColor = Color.FromHex (Values.PURPLE),//STACKVIEWSDARKERPURPLE),//Values.PURPLE),
				BarTextColor = Color.White//FromHex (Values.GOOGLEBLUE),//STACKVIEWSORANGE),//BACKGROUNDLIGHTSILVER),

			};
			this.Detail = App.NavPage;
			this.Master = new ContentPage { 
				//BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER),
				Title = "Options", 
				Icon = "ic_menu_white_24dp.png",
				Content = createUI ()
			};
		}
		ScrollView createUI(){

			return new ScrollView {
				Content = new StackLayout{
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.Fill,
					VerticalOptions = LayoutOptions.StartAndExpand,
					Children = { 
						
						UIBuilder.CreateSetting ("ic_trending_up_light_blue_500_24dp.png", "My CAPP Stats", 
							new TapGestureRecognizer{Command = new Command(() =>
								{}
							)}),
						UIBuilder.CreateSeparator (Color.Gray, 0.3),

						UIBuilder.CreateSetting ("", "Text Templates", 
							new TapGestureRecognizer{Command = new Command(() =>
								App.NavPage.Navigation.PushModalAsync (new TemplateSettingsPage())
							)}),
						UIBuilder.CreateSeparator (Color.Gray, 0.3),

						UIBuilder.CreateSetting("", "Contact Team CapTap", 
							new TapGestureRecognizer{Command = new Command(() => {
								if(Device.OS == TargetPlatform.Android) 
									Device.OpenUri(new Uri("mailto:captapuserfeedback@gmail.com"));
								else if(Device.OS == TargetPlatform.iOS){
									DependencyService.Get<IEmailService>().SendEmail();
								}
							}
						)}),
					}
				}
			};
		}
	}
}

