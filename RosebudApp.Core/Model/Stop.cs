﻿using RosebudAppCore.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace RosebudAppCore.Model
{
    public class Stop
    {
        public string stop_code { get; set; }
        public string stop_name { get; set; }
        public string stop_desc { get; set; }
        public double stop_lat { get; set; }
        public double stop_lon { get; set; }
        public string zone_id { get; set; }
        public string stop_url { get; set; }
        public string location_type { get; set; }
        public WheelchairAccessibility wheelchair_boarding { get; set; }
    }
}
