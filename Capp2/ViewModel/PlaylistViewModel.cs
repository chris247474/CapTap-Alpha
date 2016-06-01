using System;
using XLabs.Forms.Mvvm;
using Xamarin.Forms;

namespace Capp2
{
	public class PlaylistViewModel: ViewModel
	{
		private ImageSource _ImageSource;
		private Color _TextColor;

		public PlaylistViewModel ()
		{
		}

		public ImageSource ImageSource
		{
			get { return _ImageSource; }
			set { SetProperty (ref _ImageSource, value); }
		}

		public Color TextColor
		{
			get { return _TextColor; }
			set { SetProperty (ref _TextColor, value); }
		}
	}
}

