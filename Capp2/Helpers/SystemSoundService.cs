using System;
using Xamarin.Forms;

namespace Capp2
{
	public static class SystemSoundService
	{
		static ISystemSounds Sounds = DependencyService.Get<ISystemSounds>();

		public static void Play(Capp2.SystemSounds soundtype){
			Sounds.Play (soundtype);
		}

	}
}

