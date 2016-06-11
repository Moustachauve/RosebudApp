using System;
namespace MyTransit.Core
{
	public class Feed
	{
		public int feed_id { get; set; }
		public DateTime last_update { get; set; }
		public string agency_name { get; set; }
		public string agency_url { get; set; }
		public string agency_timezone { get; set; }
		public string agency_lang { get; set; }
		public string agency_phone { get; set; }
		public string agency_fare_url { get; set; }
		public string agency_email { get; set; }
	}
}

