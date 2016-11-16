using System;
using System.Collections.Generic;
using RosebudAppCore.Model;
namespace RosebudAppCore.Comparators
{
    public class DistanceComparator : IComparer<List<FavoriteRouteDirection>>
    {
        const int EQUAL = 0;
        const int GREATER = 1;
        const int LESSER = -1;

        public DistanceComparator()
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

            return GetSmallestDistance(x) - GetSmallestDistance(y);
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

        private int GetSmallestDistance(List<FavoriteRouteDirection> directions)
        {
            int smallestDistance = int.MaxValue;

            foreach (var direction in directions)
            {
                if(direction.DistanceInMeter < smallestDistance)
                {
                    smallestDistance = direction.DistanceInMeter;
                }
            }

            return smallestDistance;
        }
    }
}
