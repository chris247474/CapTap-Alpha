using System;
using System.Linq;
using Foundation;
using UIKit;

namespace Capp2.iOS
{
	public static class iOSNavigationHelper
	{
		public static bool IsUINavigationViewController(NSObject view)
		{
			if (view.GetType() == new CustomNavigationRenderer().GetType())
				return true;
			//if (view.GetType().IsSubclassOf(new UINavigationController().GetType()))
			//	return true;
			return false;
		}

		public static UINavigationController GetUINavigationController(UIViewController controller)
		{
			if (controller != null)
			{
				Console.WriteLine("controller is not null");
				if (IsUINavigationViewController(controller))
				{
					Console.WriteLine("Found uinavigationcontroller");
					return (controller as UINavigationController);
				}

				if (controller.ChildViewControllers.Count() != 0)
				{
					var count = controller.ChildViewControllers.Count();

					for (int c = 0; c < count; c++)
					{
						Console.WriteLine(
							"local iteration {0}: current controller has {1} children", c, count);
						var child = GetUINavigationController(controller.ChildViewControllers[c]);
						if (child == null)
						{
							Console.WriteLine("No children left on current controller. Moving back up");
						}
						else if (IsUINavigationViewController(child))
						{
							Console.WriteLine("returning customnavigationrenderer");
							return (child as UINavigationController);
						}
					}
				}
			}

			return null;
		}
	}
}

