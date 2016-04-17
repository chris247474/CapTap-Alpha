using Foundation;
using System;
using UIKit;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;

namespace Capp2.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //implement corresponding Init on Android
            #region Resolver Init
            SimpleContainer container = new SimpleContainer();
            container.Register<IDevice>(t => AppleDevice.CurrentDevice);
            container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
            container.Register<INetwork>(t => t.Resolve<IDevice>().Network);
            container.Register<IPhoneService>(t => t.Resolve<IDevice>().PhoneService);//for sms
            try
            {
                Resolver.SetResolver(container.GetResolver());
            }
            catch (Exception)
            {
                //ResetResolver prevents crashing when you tap local notifications that bring you back to the app
                Resolver.ResetResolver(container.GetResolver());
            }
            //implement corresponding Init on Android
            #endregion

            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}


