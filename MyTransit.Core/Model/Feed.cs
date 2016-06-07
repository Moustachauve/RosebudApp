using System;
namespace MyTransit.Core
{
	public class Feed
	{
		public int feed_id { get; set; }
		public string short_name { get; set; }
		public string long_name { get; set; }
		public DateTime last_update { get; set; }
	}
}

