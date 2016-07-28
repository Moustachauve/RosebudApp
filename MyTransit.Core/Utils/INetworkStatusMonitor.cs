using System;
using System.Collections.Generic;
using System.Text;

namespace MyTransit.Core.Utils
{
    public interface INetworkStatusMonitor
    {
        event EventHandler<NetworkState> StateChanged;
        NetworkState State { get; }
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
