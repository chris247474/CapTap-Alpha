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

			/*var padding = new Thickness (0, Device.OnPlatform (40, 40, 0), 0, 0);

			this.BackgroundColor = Color.FromHex (Values.CAPPTUTORIALCOLOR_Orange);

			var InfoLabel = UIBuilder.CreateTutorialLabel ("Tips and Tricks\nto help you burn through those call lists", 
				NamedSize.Large, FontAttributes.None); 

			var CappTipPage = new ContentPage{
				Padding = padding,
				Content = UIBuilder.CreateTappableImageWithBottomLabel("HowToCappScreenshot.png", "Where'd my CAPP Sheet go?",
					"HowToCapp.mov")
			};
			var TextTemplatesTipPage = new ContentPage{
				Padding = padding,
				Content = UIBuilder.CreateTappableImageWithBottomLabel("HowToUseTextTemplatesScreenshot.png", "Type your meetup texts once, then never again",
					"HowToUseTextTemplates.mov")
			};
			var StatsTipPage = new ContentPage{
				Padding = padding,
				Content = UIBuilder.CreateTappableImageWithBottomLabel("HowToUseStatsScreenshot.png", "See your daily work stats!",
					"HowToUseStats.mov")
			};
			var FeedbackTipPage = new ContentPage{
				Padding = padding,
				Content = UIBuilder.CreateTappableImageWithBottomLabel("HowToFeedbackScreenshot.png", "Suggestions, Feedback, Questions?",
					"HowToFeedback.mov")
			};
			var YesCallsTipPage = new ContentPage{
				Padding = padding,
				Content = UIBuilder.CreateTappableImageWithBottomLabel("HowToSendYesCallsScreenshot.png", "Send your daily yes calls from CapTap!",
					"HowToUseSendYesCalls.mov")
			};
			var StartingNamelistTipPage = new ContentPage{
				Padding = padding,
				Content = UIBuilder.CreateTappableImageWithBottomLabel("HowToSetStartingScreenshot.png", "Always start with this namelist",
					"HowToSetStarting.mov")
			};
			var DailyEmailTipPage = new ContentPage{
				Padding = padding,
				Content = UIBuilder.CreateTappableImageWithBottomLabel("HowToUseDailyEmailScreenshot.png", "Daily Emails",
					"HowToUseDailyEmail.mov")
			};
			
			Children.Add (CappTipPage);
			Children.Add (TextTemplatesTipPage);
			Children.Add (StatsTipPage);
			Children.Add (YesCallsTipPage);
			Children.Add (DailyEmailTipPage);
			Children.Add (StartingNamelistTipPage);
			Children.Add (FeedbackTipPage);*/

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