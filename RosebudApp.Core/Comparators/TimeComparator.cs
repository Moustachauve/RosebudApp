using System;
using System.Collections.Generic;
using RosebudAppCore.Model;
namespace RosebudAppCore.Comparators
{
    public class TimeComparator : IComparer<List<FavoriteRouteDirection>>
    {
        const int EQUAL = 0;
        const int GREATER = 1;
        const int LESSER = -1;

        public TimeComparator()
        {
        }

        public int Compare(List<FavoriteRouteDirection> x, List<FavoriteRouteDirection> y)
        {
            bool xHasLocation = HasLocation(x);
            bool yHasLocation = HasLocation(y);

            if (!xHasLocation && !yHasLocation)
                return EQUAL;

            if (!xHasLocation && yHasLocation)
                return GREATER;

            if (xHasLocation && !yHasLocation)
                return LESSER;

            return DateTime.Compare(GetSmallestDate(x), GetSmallestDate(y));
        }

        private bool HasLocation(List<FavoriteRouteDirection> directions)
        {
            foreach (var direction in directions)
            {
                if (direction.Stop != null)
                {
                    return true;
                }
            }

            return false;
        }

        private DateTime GetSmallestDate(List<FavoriteRouteDirection> directions)
        {
            if(directions.Count == 1)
            {
                return directions[0].NextDepartureTime;
            }
            else
            {
                if(directions[0].NextDepartureTime > directions[1].NextDepartureTime)
                {
                    return directions[0].NextDepartureTime;
                }
                else
                {
                    return directions[1].NextDepartureTime;
                }
            }
        }

    }
}
