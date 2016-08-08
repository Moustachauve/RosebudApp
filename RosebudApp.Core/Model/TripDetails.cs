using System;
using System.Collections.Generic;

namespace RosebudAppCore.Model
{
	public class TripDetails
	{
		public List<Stop> stops { get; set; }
		public List<ShapePoint> shape { get; set; }
	}
}

