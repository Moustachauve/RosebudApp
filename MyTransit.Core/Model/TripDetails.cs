using System;
using System.Collections.Generic;

namespace MyTransitCore.Model
{
	public class TripDetails
	{
		public List<Stop> stops { get; set; }
		public List<ShapePoint> shape { get; set; }
	}
}

