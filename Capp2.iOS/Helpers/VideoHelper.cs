using System;
using Capp2.iOS;
using MediaPlayer;
using Foundation;
using UIKit;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(VideoHelper))]
namespace Capp2.iOS
{
	public class VideoHelper:IVideoHelper
	{
		MPMoviePlayerController moviePlayer;

		public VideoHelper ()
		{
		}

		public async Task PlayVideo(string videofile){
			if (string.IsNullOrWhiteSpace (videofile)) {
				throw new ArgumentNullException ("The video file string cannot be null empty or whitespace");
			}

			moviePlayer = new MPMoviePlayerController (NSUrl.FromFilename (videofile));

			UIApplication.SharedApplication.KeyWindow.AddSubview (moviePlayer.View);
			moviePlayer.SetFullscreen (true, true);
			moviePlayer.Play ();
		}
	}
}

