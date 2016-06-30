using System;
using System.Globalization;
namespace MyTransit.Core
{
	public static class TimeFormatter
	{
		//TODO: Use datetime instead of timespan

		/// <summary>
		/// Parse a string representing a time with the format "HH:MM:SS". Hours can be over 24h
		/// </summary>
		public static TimeSpan StringToTimeSpan(string time)
		{
			string[] values = time.Split(':');
			return new TimeSpan(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
		}

		/// <summary>
		/// Formats a time in receive as "HH:MM:SS" to "HH:MM" where hours can be over 24h.
		/// </summary>
		public static string FormatHoursMinutes(string time)
		{
			return FormatHoursMinutes(StringToTimeSpan(time));
		}

		public static string FormatHoursMinutes(double seconds)
		{
			TimeSpan time = TimeSpan.FromSeconds(seconds);
			return FormatHoursMinutes(time);
		}

		public static string FormatHoursMinutes(TimeSpan time)
		{
			return time.ToString(@"hh\:mm");
		}

		public static string ToShortDateApi(DateTime date)
		{
			return date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
		}

		public static string ToAbrevShortDate(DateTime date)
		{
			return date.ToString("ddd d MMM", CultureInfo.InvariantCulture);
		}

		public static string ToFullShortDate(DateTime date)
		{
			return date.ToString("dddd dd MMMM", CultureInfo.InvariantCulture);
		}
	}
}

