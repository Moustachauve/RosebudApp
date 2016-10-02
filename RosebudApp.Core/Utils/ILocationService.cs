using RosebudAppCore.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RosebudAppCore.Utils
{
    public interface ILocationService
    {
        void RemoveOnLocationChangedListener(ILocationServiceListener listener);
        void AddOnLocationChangedListener(ILocationServiceListener listener);
        Location LastKnownLocation { get; }
        int CalculateDistance(double latA, double lonA, double latB, double lonB);
    }

    public interface ILocationServiceListener
    {
        Task OnLocationChanged(Location location);
    }
}
