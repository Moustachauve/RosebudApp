using RosebudAppCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RosebudAppCore.Utils
{
    public interface ILocationService
    {
        void RemoveOnLocationChangedListener(ILocationServiceListener listener);
        void AddOnLocationChangedListener(ILocationServiceListener listener);
    }

    public interface ILocationServiceListener
    {
        void OnLocationChanged(Location location);
    }
}
