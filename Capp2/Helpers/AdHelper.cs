using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Capp2.Helpers;

namespace Capp2
{
	public static class AdHelper
	{
		public static bool IsUserPremiumAccount(){
			return Settings.IsPremiumSettings;
			//return true;
		}

		public static async Task AddGreenURLOrangeTitleBannerToStack(StackLayout stack, int BannerHeight = 50, 
			int BannerWidth = 320, AdMobView banner = null)
		{
			if (!IsUserPremiumAccount ()) {
				//show ads
				if (stack != null) {
					if (banner != null) {
						stack.Children.Add (banner);
					} else {
						stack.Children.Add (await CreateBannerAd (BannerHeight, BannerWidth));
					}
				}
			}
		}

		public static async Task<AdMobView> CreateBannerAd(int BannerHeight = 50, 
			int BannerWidth = 320)
		{
			if (!IsUserPremiumAccount ()) {
				await Task.Delay (2000);
				return new AdMobView {
					HeightRequest = BannerHeight,
					WidthRequest = BannerWidth,
				};
			} else {
				return new AdMobView{ HeightRequest = 0, WidthRequest = 0 };
			}
		}

		/*public static async Task AddDefaultNativeToStack(StackLayout stack, int BannerHeight = 50, 
			int BannerWidth = 320)
		{
			
		}*/
	}
}

