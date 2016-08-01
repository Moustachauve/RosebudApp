using System;
namespace MyTransitCore.Model
{
	public class Route
	{
		public string route_id { get; set; }
		public int feed_id { get; set; }
		public string route_short_name { get; set; }
		public string route_long_name { get; set; }
		public string route_desc { get; set; }
		public RouteType route_type { get; set; }
		public string route_url { get; set; }
		public string route_color { get; set; }
		public string route_text_color { get; set; }
	}

	public enum RouteType
	{
		StreetLevelRail = 0,
		UndergroundRail = 1,
		Train = 2,
		Bus = 3,
		Ferry = 4,
		CableCar = 5,
		SuspendedCableCar = 6,
		Funicular = 7
	}
}

