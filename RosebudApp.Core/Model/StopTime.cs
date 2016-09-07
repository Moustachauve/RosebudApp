using RosebudAppCore.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace RosebudAppCore.Model
{
    public class StopTime
    {
        public string trip_id{ get; set; }
        public string arrival_time{ get; set; }
        public string departure_time{ get; set; }
        public string stop_headsign { get; set; }
        public string pickup_type { get; set; }
        public string drop_off_type { get; set; }
        public WheelchairAccessibility wheelchair_accessible{ get; set; }
        public BikesAllowed bikes_allowed{ get; set; }
    }
}
