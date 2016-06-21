using System;
namespace MyTransit.Core
{
	public static class TimeFormatter
	{
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
		public static string FormatHoursMinutes(string time) {
			return StringToTimeSpan(time).ToString(@"hh\:mm");
		}
	}
}

