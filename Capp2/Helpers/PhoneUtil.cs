﻿using System;
using System.Text;
using System.Diagnostics;

namespace Capp2
{
	public static class PhoneUtil
	{
		static bool Contains(this string keyString, char c)
		{
			return keyString.IndexOf(c) >= 0;
		}

		static readonly string[] digits = {
			"ABC", "DEF", "GHI", "JKL", "MNO", "PQRS", "TUV", "WXYZ"
		};

		static int? TranslateToNumber(char c)
		{
			for (int i = 0; i < digits.Length; i++)
			{
				if (digits[i].Contains(c))
					return 2 + i;
			}
			return null;
		}

		public static string ToNumber(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw))
				return null;

			raw = raw.ToUpperInvariant();

			var newNumber = new StringBuilder();
			foreach (char c in raw)
			{
				if (" -0123456789".Contains(c))
					newNumber.Append(c);
				else
				{
					var result = TranslateToNumber(c);
					if (result != null)
						newNumber.Append(result);
					// Bad character?
					else
						return null;
				}
			}
			return newNumber.ToString();
		}

		public static string ToNumber_Custom(string number)
		{
			if (string.IsNullOrWhiteSpace(number))
				return null;

			number = number.ToLower ();

			var newNumber = new StringBuilder();
			foreach (char c in number)
			{
				if ("0123456789".Contains(c))
					newNumber.Append(c);
			}
			Debug.WriteLine ("PhoneUtil.ToNumber() returning {0}", newNumber.ToString());
			return newNumber.ToString();
		}
	}
}

