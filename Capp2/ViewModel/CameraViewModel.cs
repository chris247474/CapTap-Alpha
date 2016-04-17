using System.Threading.Tasks;

using Xamarin.Forms;

using XLabs.Forms.Mvvm;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace Capp2
{
	public class CameraViewModel : ViewModel
	{
		private readonly TaskScheduler _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		private IMediaPicker _Mediapicker;
		private ImageSource _ImageSource;
		private string _VideoInfo;
		private Command _TakePictureCommand;
		private Command _SelectPictureCommand;
		private Command _SelectVideoCommand;
		private string _Status;
		public MediaFile mediaFileForTesseract{ get; set;}

		public CameraViewModel ()
		{
			Setup ();
		}

		public ImageSource ImageSource
		{
			get { return _ImageSource; }
			set { SetProperty (ref _ImageSource, value); }
		}

		public string VideoInfo
		{
			get { return _VideoInfo; }
			set { SetProperty (ref _VideoInfo, value); }
		}

		public Command TakePictureCommand
		{
			get {
				return _TakePictureCommand ?? (_TakePictureCommand =
					new Command (async () => await TakePicture (), () => true));
			}
		}

		public Command SelectPictureCommand
		{
			get {
				return _SelectPictureCommand ?? (_SelectPictureCommand =
					new Command (async () => await SelectPicture (), () => true));
			}
		}

		public Command SelectVideoCommand
		{
			get {
				return _SelectVideoCommand ?? (_SelectVideoCommand =
					new Command (async () => await SelectVideo (), () => true));
			}
		}

		public string Status
		{
			get { return _Status; }
			set { SetProperty (ref _Status, value); }
		}

		private void Setup()
		{
			if (_Mediapicker == null) {
				var device = Resolver.Resolve<IDevice>();
				_Mediapicker = DependencyService.Get<IMediaPicker>() ?? device.MediaPicker;
			}
		}

		public async Task<MediaFile> TakePicture()
		{
			Setup ();

			ImageSource = null;

			return await _Mediapicker.TakePhotoAsync (new CameraMediaStorageOptions {
				DefaultCamera = CameraDevice.Front, MaxPixelDimension = 400
			}).ContinueWith (t => {
				if (t.IsFaulted)
				{
					Status = t.Exception.InnerException.ToString();
				}
				else if (t.IsCanceled)
				{
					Status = "Canceled";
				}
				else
				{
					var mediaFile = t.Result;
					ImageSource = ImageSource.FromStream(() => mediaFile.Source);

					return mediaFile;
				}

				return null;
			}, _scheduler);
		}

		public async Task SelectPicture()
		{
			Setup ();

			ImageSource = null;

			try
			{
				var mediaFile = await _Mediapicker.SelectPhotoAsync(new CameraMediaStorageOptions
					{
						DefaultCamera = CameraDevice.Front,
						MaxPixelDimension = 400
					});

				VideoInfo = mediaFile.Path;
				ImageSource = ImageSource.FromStream(() => mediaFile.Source);

				//send mediaFile for loadContactsFromPic()
				mediaFileForTesseract = mediaFile;
			}
			catch (System.Exception ex)
			{
				Status = ex.Message;
			}
		}

		public async Task SelectVideo()
		{
			Setup ();

			VideoInfo = "Selecting video";

			try
			{
				var mediaFile =  await _Mediapicker.SelectVideoAsync(new VideoMediaStorageOptions());

				VideoInfo = mediaFile != null
					? string.Format("Your video size {0} MB", ConvertBytesToMegabytes(mediaFile.Source.Length))
					: "No video was selected";
			}
			catch (System.Exception ex)
			{
				if (ex is TaskCanceledException) {
					VideoInfo = "Selecting video cancelled";
				} else {
					VideoInfo = ex.Message;
				}
			}
		}

		private static double ConvertBytesToMegabytes(long bytes)
		{
			double rtn_value = (bytes / 1024f) / 1024f;

			return rtn_value;
		}
	}
}

