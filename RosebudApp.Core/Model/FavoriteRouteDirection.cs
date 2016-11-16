using System;
namespace RosebudAppCore.Model
{
    public class FavoriteRouteDirection
    {
        public Stop Stop { get; set; }
        public int DistanceInMeter { get; set; }
        public DateTime NextDepartureTime { get; set; }
    }
}
