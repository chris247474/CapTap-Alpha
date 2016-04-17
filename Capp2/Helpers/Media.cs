using System;
using XLabs.Platform.Services.Media;
using XLabs.Ioc;
using XLabs.Platform.Device;
using Xamarin.Forms;

namespace Capp2
{
	public static class Media
	{
		static IMediaPicker mediaPicker = null;
		public static IMediaPicker MediaPicker
		{
			get
			{
				if (mediaPicker == null)
				{
					var device = Resolver.Resolve<IDevice>();
					mediaPicker = DependencyService.Get<IMediaPicker>() ?? device.MediaPicker;
					if (mediaPicker == null) throw new NullReferenceException("MediaPicker DependencyService.Get error");
				}

				return mediaPicker;
			}
		}
	}
}

