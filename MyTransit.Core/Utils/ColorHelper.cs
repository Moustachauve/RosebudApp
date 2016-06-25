using System;
using Android.Graphics;

namespace MyTransit.Core
{
	public static class ColorHelper
	{
		public static string FormatColor(string color)
		{
			if (color[0] != '#')
				color = "#" + color;

			return color;
		}

		public static Color ContrastColor(string color)
		{
			return ContrastColor(Color.ParseColor(FormatColor(color)));
		}

		public static Color ContrastColor(Color color)
		{
			// Counting the perceptive luminance - human eye favors green color... 
			double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

			if (a < 0.5)
				return Color.Black;
			else
				return Color.White;
		}
	}
}

