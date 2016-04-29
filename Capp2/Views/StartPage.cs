using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using Capp2.Helpers;

namespace Capp2
{
	public class StartPage:MasterDetailPage
	{
		StackLayout StatsSettings = new StackLayout(), TemplateSettings = new StackLayout(), 
		FeedbackSettings = new StackLayout(), DefaultNamelistSettings = new StackLayout();

		public StartPage ()
		{
			this.Title = "Namelists";

			//this.BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER);
			App.NavPage = new NavigationPage(new PlaylistPage{StartColor = App.StartColor, EndColor = App.EndColor}){
				BarBackgroundColor = Color.FromHex (Values.GOOGLEBLUE),//STACKVIEWSDARKERPURPLE),//Values.PURPLE),
				BarTextColor = Color.White//FromHex (Values.GOOGLEBLUE),//STACKVIEWSORANGE),//BACKGROUNDLIGHTSILVER),

			};
			App.NavPage.Navigation.PopAsync ();
			App.NavPage.Navigation.PushAsync (new CAPP (App.DefaultNamelist));
			this.Detail = App.NavPage;
			this.Master = new ContentPage { 
				//BackgroundColor = Color.FromHex (Values.BACKGROUNDLIGHTSILVER),
				Title = "Options", 
				Icon = "ic_menu_white_24dp.png",
				Content = createUI ()
			};
		}
		ScrollView createUI(){
			CreateSettings ();

			return new ScrollView {
				Content = new StackLayout{
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.Fill,
					VerticalOptions = LayoutOptions.StartAndExpand,
					Padding = new Thickness(20),
					Children = { 
						UIBuilder.CreateSettingsHeader("Settings"),

						StatsSettings,
						UIBuilder.CreateSeparator (Color.Gray, 0.3),

						TemplateSettings,
						UIBuilder.CreateSeparator (Color.Gray, 0.3),

						FeedbackSettings,
						UIBuilder.CreateSeparator (Color.Gray, 0.3),

						DefaultNamelistSettings,
						UIBuilder.CreateSeparator (Color.Gray, 0.3),
					}
				}
			};
		}
		void CreateSettings(){
			StatsSettings = UIBuilder.CreateSetting ("trending-Small.png", "\tMy CAPP Stats", 
				new TapGestureRecognizer{Command = new Command(() =>
					{
						UIAnimationHelper.ZoomUnZoomElement(StatsSettings); 
						UIAnimationHelper.ShrinkUnshrinkElement(TemplateSettings);
						UIAnimationHelper.ShrinkUnshrinkElement(FeedbackSettings);
						UIAnimationHelper.ShrinkUnshrinkElement(DefaultNamelistSettings);
					}
				)});
			DefaultNamelistSettings = UIBuilder.CreateSetting ("", "\tStarting Namelist", 
				new TapGestureRecognizer{Command = new Command(async () =>
					{
						UIAnimationHelper.ZoomUnZoomElement(DefaultNamelistSettings); 
						UIAnimationHelper.ShrinkUnshrinkElement(TemplateSettings);
						UIAnimationHelper.ShrinkUnshrinkElement(FeedbackSettings);
						UIAnimationHelper.ShrinkUnshrinkElement(StatsSettings);

						await Util.ChooseNewDefaultNamelist(App.Database.GetPlaylistNames());
					}
				)});
			TemplateSettings = UIBuilder.CreateSetting ("", "\tText Templates", 
				new TapGestureRecognizer {Command = new Command (() => {
					UIAnimationHelper.ShrinkUnshrinkElement(StatsSettings); 
					UIAnimationHelper.ZoomUnZoomElement(TemplateSettings);
					UIAnimationHelper.ShrinkUnshrinkElement(FeedbackSettings);
					UIAnimationHelper.ShrinkUnshrinkElement(DefaultNamelistSettings);
					App.NavPage.Navigation.PushModalAsync (new TemplateSettingsPage ());
				}
				)});
			FeedbackSettings = UIBuilder.CreateSetting ("", "\tContact Team CapTap", 
				new TapGestureRecognizer {Command = new Command (() => {
					UIAnimationHelper.ShrinkUnshrinkElement(StatsSettings); 
					UIAnimationHelper.ShrinkUnshrinkElement(TemplateSettings);
					UIAnimationHelper.ZoomUnZoomElement(FeedbackSettings);
					UIAnimationHelper.ShrinkUnshrinkElement(DefaultNamelistSettings);
					if (Device.OS == TargetPlatform.Android){
						//Device.OpenUri (new Uri ("mailto:captapuserfeedback@gmail.com"));
					}
					else if (Device.OS == TargetPlatform.iOS) {
						DependencyService.Get<IEmailService> ().SendEmail ();
					}})});
		}
	}
}

