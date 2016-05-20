using System;
using Capp2.Droid;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(VideoHelper))]
namespace Capp2.Droid
{
	public class VideoHelper:IVideoHelper
	{
		public VideoHelper ()
		{
		}

		public Task PlayVideo(string videofile){
			if (string.IsNullOrWhiteSpace (videofile)) {
				throw new ArgumentNullException ("The video file string cannot be null empty or whitespace");
			}

			throw new NotImplementedException ("not implmnted");
		}
	}
}

