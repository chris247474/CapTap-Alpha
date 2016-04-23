using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Capp2
{
	public interface IColorConverter
	{
		/*public static Color StringToColor(IList<string> color)
		{
			for(var i = 0; i < color.Count(); i++)
			{
				//Regex to get the color code
				color[i] = Regex.Replace(color[i], @"^\d.\d+]", "");
			}

			var a = double.Parse(color[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
			var r = double.Parse(color[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
			var g = double.Parse(color[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
			var b = double.Parse(color[3], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

			return Color.FromRgba(r, g, b, a);
		}*/
		Color ColorFromHex (string hex);
	}
}

