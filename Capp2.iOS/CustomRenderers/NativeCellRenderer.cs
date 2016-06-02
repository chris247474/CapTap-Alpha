﻿using System;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Capp2;
using Capp2.iOS;
using CoreGraphics;
using System.Reflection;
using ObjCRuntime;

[assembly: ExportRenderer (typeof(NativeCell), typeof(NativeCellRenderer))]
namespace Capp2.iOS
{
	public class NativeCellRenderer : ViewCellRenderer
	{
		static NSString rid = new NSString ("NativeCell");

		public override UITableViewCell GetCell (Xamarin.Forms.Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			UITableViewCell cell = base.GetCell(item, reusableCell, tv);


			// Get UIImage with a green color fill
			CGRect rect = new CGRect(0, 0, 1, 1);
			CGSize size = rect.Size;
			UIGraphics.BeginImageContext(size);
			CGContext currentContext = UIGraphics.GetCurrentContext();
			currentContext.SetFillColor(Color.FromHex(Values.YELLOW).ToCGColor());
			currentContext.FillRect(rect);
			var backgroundImage = UIGraphics.GetImageFromCurrentImageContext();
			currentContext.Dispose();


			// This is the assembly full name which may vary by the Xamarin.Forms version installed.
			// NullReferenceException is raised if the full name is not correct.
			var t = Type.GetType("Xamarin.Forms.Platform.iOS.ContextActionsCell, Xamarin.Forms.Platform.iOS, Version=9.6.1.9, Culture=neutral, PublicKeyToken=null");

			// Now change the static field value! "normalBackground" OR "destructiveBackground"
			/*var field = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			for (int c = 0; c < field.Length; c++) {
				Console.WriteLine ("iteration {0}, field.Name: {1}", c, field[c].Name);
				if (string.Equals (field [c].Name, "normalBackground")) {
					Console.WriteLine ("found normalBackground field");
					field[c].SetValue(new object(), backgroundImage);
				}
			}*/
			//Xamarin.Forms >2.1.0.6529: change "normalBackground" to "NormalBackground"
			try{
				var field = t.GetField("NormalBackground", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); 
				field.SetValue(new object(), backgroundImage);
			}catch(NullReferenceException){
				var field = t.GetField("normalBackground", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); 
				field.SetValue(new object(), backgroundImage);
			}

			//set destructive color
			currentContext = UIGraphics.GetCurrentContext();
			currentContext.SetFillColor(Color.FromHex(Values.MaterialDesignOrange/*"#FF5722"*/).ToCGColor());
			currentContext.FillRect(rect);
			backgroundImage = UIGraphics.GetImageFromCurrentImageContext();
			currentContext.Dispose();

			//Xamarin.Forms >2.1.0.6529: change "normalBackground" to "NormalBackground"
			try{
				var field = t.GetField("DestructiveBackground", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); 
				field.SetValue(new object(), backgroundImage);
			}catch(NullReferenceException){
				var field = t.GetField("destructiveBackground", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); 
				field.SetValue(new object(), backgroundImage);
			}

			return cell;
		}
	}
}

