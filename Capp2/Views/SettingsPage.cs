using System;

using Xamarin.Forms;
using Capp2.Helpers;

namespace Capp2
{
	public class SettingsPage : ContentPage
	{

		StackLayout StatsSettings = new StackLayout(), TemplateSettings = new StackLayout(), 
		FeedbackSettings = new StackLayout(), DefaultNamelistSettings = new StackLayout(), 
		DailyEmailSettings = new StackLayout(), SendYesCallSettings = new StackLayout(), HowToSettings = new StackLayout(),
		SettingsLayout = new StackLayout();

		public SettingsPage ()
		{
			Icon = "settings.png";
			Title = "Settings";
			BackgroundColor = Color.FromHex ("#333333");
			Content = createUI ();

			AdHelper.AddGreenURLOrangeTitleBannerToStack (SettingsLayout);
		}

		ScrollView createUI(){
			CreateSettings ();

			SettingsLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness (20),
				Children = { 
					UIBuilder.CreateSettingsHeader ("Settings"),

					StatsSettings,
					UIBuilder.CreateSeparator (Color.Gray, 0.3),

					TemplateSettings,
					UIBuilder.CreateSeparator (Color.Gray, 0.3),

					FeedbackSettings,
					UIBuilder.CreateSeparator (Color.Gray, 0.3),

					DefaultNamelistSettings,
					UIBuilder.CreateSeparator (Color.Gray, 0.3),

					DailyEmailSettings,
					UIBuilder.CreateSeparator (Color.Gray, 0.3),

					SendYesCallSettings,
					UIBuilder.CreateSeparator (Color.Gray, 0.3),

					HowToSettings,
					UIBuilder.CreateSeparator (Color.Gray, 0.3),

				},
			};

			return new ScrollView {
				Content = SettingsLayout
			};
		}

		void CreateSettings(){
			HowToSettings = UIBuilder.CreateSetting ("help.png", "\tHelp", 
				new TapGestureRecognizer{ Command = new Command(() => {
					UIAnimationHelper.ZoomUnZoomElement(HowToSettings);
					App.NavPage.Navigation.PushModalAsync(new TutorialSettingsPage());
					/*App.NavPage.Navigation.PushModalAsync(new CarouselHelper(IndicatorStyleEnum.Dots, 
						Color.FromHex(Values.CAPPTUTORIALCOLOR_Orange)));*/
				})});
			StatsSettings = UIBuilder.CreateSetting ("trending-Medium.png", "\tMy Call Stats", 
				new TapGestureRecognizer{Command = new Command(() =>
					{
						UIAnimationHelper.ZoomUnZoomElement(StatsSettings); 
						App.NavPage.Navigation.PushModalAsync(new CapStats());
					}
				)});
			SendYesCallSettings = UIBuilder.CreateSetting ("Leaderboard.png", "\tSend Yes Calls Today", 
				new TapGestureRecognizer{Command = new Command(() =>
					{
						UIAnimationHelper.ZoomUnZoomElement(SendYesCallSettings);
						DependencyService.Get<IPhoneContacts>().Share(
							StatsHelper.GetYesCallMessage()
						);
					}
				)});
			DailyEmailSettings = UIBuilder.CreateSetting ("Message-100-yellow.png", "\tDaily Reports", 
				new TapGestureRecognizer{Command = new Command(() =>
					{
						UIAnimationHelper.ZoomUnZoomElement(DailyEmailSettings); 
						//DependencyService.Get<IEmailService>().SendEmail(string.Empty, 
						//Settings.DailyEmailTemplateSettings, "My Daily Progress");
						DependencyService.Get<IPhoneContacts>().Share(//if user select viber, viber doesn't allow editing before sending
							Settings.DailyEmailTemplateSettings
						);
					}
				)});
			DefaultNamelistSettings = UIBuilder.CreateSetting ("FinishFlag.png", "\tStarting Namelist", 
				new TapGestureRecognizer{Command = new Command(async () =>
					{
						UIAnimationHelper.ZoomUnZoomElement(DefaultNamelistSettings); 
						await Util.ChooseNewDefaultNamelist(App.Database.GetPlaylistNames());
					}
				)});
			TemplateSettings = UIBuilder.CreateSetting ("SpeechBubble.png", "\tMessage Templates", 
				new TapGestureRecognizer {Command = new Command (() => {
					UIAnimationHelper.ZoomUnZoomElement(TemplateSettings);
					App.NavPage.Navigation.PushModalAsync (new TemplateSettingsPage ());
				}
				)});
			FeedbackSettings = UIBuilder.CreateSetting ("Feedback.png", string.Format("\tContact Team {0}", Values.APPNAME), 
				new TapGestureRecognizer {Command = new Command (() => {
					UIAnimationHelper.ZoomUnZoomElement(FeedbackSettings);

					if (Device.OS == TargetPlatform.Android){
						//Device.OpenUri (new Uri ("mailto:captapuserfeedback@gmail.com"));
					}
					else if (Device.OS == TargetPlatform.iOS) {
					DependencyService.Get<IEmailService> ().SendEmail ("captapuserfeedback@gmail.com", string.Empty,
					                                                  "ConTap User Feedback");
					}})});
		}
	}
}


