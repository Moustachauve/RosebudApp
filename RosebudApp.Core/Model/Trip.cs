using RosebudAppCore.Model.Enum;
using System;
using System.Collections.Generic;

namespace RosebudAppCore.Model
{
	public class Trip
	{
		public string trip_id { get; set; }
		public string trip_headsign { get; set; }
		public string trip_short_name { get; set; }
		public TripDirection direction_id { get; set; }
		public string block_id { get; set; }
		public string shape_id { get; set; }
		public int? headway_secs { get; set; }
		public WheelchairAccessibility wheelchair_accessible { get; set; }
		public BikesAllowed bikes_allowed { get; set; }
		public string note_fr { get; set; }
		public string note_en { get; set; }

		public string start_time { get; set; }
		public string end_time { get; set; }
	}
}

