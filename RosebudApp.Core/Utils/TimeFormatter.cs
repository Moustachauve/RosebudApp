using System;
using System.Globalization;
namespace RosebudAppCore
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

        /// <summary>
        /// Seconds -> "HH:MM"
        /// </summary>
        public static string FormatHoursMinutes(double seconds)
		{
			TimeSpan time = TimeSpan.FromSeconds(seconds);
			return FormatHoursMinutes(time);
		}

        /// <summary>
        /// time -> "HH:MM"
        /// </summary>
        public static string FormatHoursMinutes(TimeSpan time)
		{
			return time.ToString(@"hh\:mm");
		}

        /// <summary>
        /// datetime -> "yyyyMMdd"
        /// </summary>
        public static string ToShortDateApi(DateTime date)
		{
			return date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
		}

        /// <summary>
        /// Take a datetime and format it to properly display it in an abreviated format (Ex: Mon. 13 Sept.)
        /// </summary>
		public static string ToAbrevShortDate(DateTime date)
		{
			return date.ToString("ddd d MMM", CultureInfo.InvariantCulture);
		}

        /// <summary>
        /// Take a datetime and format it to properly display it in full format (Ex: Monday 13 September)
        /// </summary>
        public static string ToFullShortDate(DateTime date)
		{
			return date.ToString("dddd dd MMMM", CultureInfo.InvariantCulture);
		}
	}
}

