﻿using System;
using System.Collections.Generic;
using RosebudAppCore.Model;
namespace RosebudAppCore.Comparators
{
    public class TimeComparator : IComparer<StopLocation>
    {
        const int EQUAL = 0;
        const int GREATER = 1;
        const int LESSER = -1;

        public TimeComparator()
        {
        }

        public int Compare(StopLocation x, StopLocation y)
        {
            bool xHasLocation = HasLocation(x);
            bool yHasLocation = HasLocation(y);

            if (!xHasLocation && !yHasLocation)
                return EQUAL;

            if (!xHasLocation && yHasLocation)
                return GREATER;

            if (xHasLocation && !yHasLocation)
                return LESSER;

            return DateTime.Compare(x.NextDepartureTime, y.NextDepartureTime);
        }

        private bool HasLocation(StopLocation stopLocation)
        {
            return stopLocation != null && stopLocation.NextDepartureTime > DateTime.MinValue;
        }
    }
}
