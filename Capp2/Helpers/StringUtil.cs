using System;

namespace Capp2
{
	public static class StringUtil
	{
		public static string[] Split(string raw, string separator){
			return raw.Split(new string[]{separator}, StringSplitOptions.None);
		}
	}
}

