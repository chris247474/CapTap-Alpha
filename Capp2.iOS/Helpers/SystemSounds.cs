using System;
using Xamarin.Forms;
using AudioToolbox;
using Foundation;

[assembly: Dependency(typeof(Capp2.iOS.SystemSounds))]

namespace Capp2.iOS
{
	public class SystemSounds:ISystemSounds
	{
		NSUrl url;
		SystemSound systemSound;

		string tapSoundFile = "Sounds/tap.aif";

		public SystemSounds ()
		{
			Console.WriteLine ("enum Tap string value is {0}", Capp2.SystemSounds.Tap.ToString());
		}

		public void Play(Capp2.SystemSounds soundtype){
			systemSound = new SystemSound ((uint)soundtype);
			systemSound.PlaySystemSound ();
		}
	}
}

