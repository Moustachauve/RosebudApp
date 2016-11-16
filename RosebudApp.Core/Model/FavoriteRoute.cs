using System.Collections.Generic;

namespace RosebudAppCore.Model
{
    public class FavoriteRoute
    {
        public List<FavoriteRouteDirection> Directions { get; set; }
        public Route Route { get; set; }

        public FavoriteRoute()
        {
            Directions = new List<FavoriteRouteDirection>();
        }
    }
}
