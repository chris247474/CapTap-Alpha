using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Capp2.iOS;
using System.Collections.Generic;
using UIKit;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]

namespace Capp2.iOS
{
	public class CustomNavigationRenderer : NavigationRenderer
	{

		public override void PushViewController(UIKit.UIViewController viewController, bool animated)
		{
			base.PushViewController(viewController, animated);

			var list = new List<UIBarButtonItem>();

			foreach (var item in TopViewController.NavigationItem.RightBarButtonItems)
			{
				if(string.IsNullOrEmpty(item.Title))
				{
					continue;
				}

				if (item.Title.ToLower() == "add")
				{
					var newItem = new UIBarButtonItem(UIBarButtonSystemItem.Add)
					{
						Action = item.Action,
						Target = item.Target
					};

					list.Add(newItem);
				}
				if (item.Title.ToLower() == "done")
				{
					var newItem = new UIBarButtonItem(UIBarButtonSystemItem.Done)
					{
						Action = item.Action,
						Target = item.Target
					};

					list.Add(newItem);
				}
				if (item.Title.ToLower() == "edit")
				{
					var newItem = new UIBarButtonItem(UIBarButtonSystemItem.Edit)
					{
						Action = item.Action,
						Target = item.Target
					};

					list.Add(newItem);
				}
				/*if (item.Title.ToLower() == "menu")
				{
					var newItem = new UIBarButtonItem(UIBarButtonSystemItem.)
					{
						Action = item.Action,
						Target = item.Target
					};

					list.Add(newItem);
				}*/

				if (item.Title.ToLower() == "camera")
				{
					var newItem = new UIBarButtonItem(UIBarButtonSystemItem.Camera)
					{
						Action = item.Action,
						Target = item.Target
					};

					list.Add(newItem);
				}

				TopViewController.NavigationItem.RightBarButtonItems = list.ToArray();
			}
		}
	}

}

