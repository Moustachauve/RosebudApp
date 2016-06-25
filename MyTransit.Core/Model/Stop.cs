using System;
namespace MyTransit.Core.Model
{
	public class Stop : StopTime
	{
		public string stop_code { get; set; }
        public string stop_name { get; set; }
        public string stop_desc { get; set; }
        public double stop_lat { get; set; }
        public double stop_lon { get; set; }
        public string zone_id { get; set; }
        public string stop_url { get; set; }
        public string location_type { get; set; }
        public string parent_station { get; set; }
        public string stop_timezone { get; set; }
        public string wheelchair_boarding { get; set; }
	}
}

