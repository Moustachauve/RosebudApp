using System;
using System.Collections.Generic;
using System.Text;

namespace RosebudAppCore.Utils
{
    public interface INetworkStatusMonitor
    {
        event EventHandler<NetworkState> StateChanged;
        NetworkState State { get; }
        bool CanConnect { get; }
        void UpdateState();
    }

    public enum NetworkState
    {
        Unknown,
        ConnectedWifi,
        ConnectedData,
        Disconnected
    }

}
