using System;
using System.Globalization;
namespace MyTransit.Core.Utils
{
	public static class StringExtensions
	{
		public static bool ContainsInsensitive(this string source, string toCheck) {
			CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;
			CompareOptions options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
			return compareInfo.IndexOf(source, toCheck, options) >= 0;
		}
	}
}

