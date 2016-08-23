
using Android.App;
using Android.Content;
using Android.Net;
using RosebudAppAndroid.Utils;
using RosebudAppCore.Utils;
using System;

namespace RosebudAppAndroid.Utils
{
    public class NetworkStatusMonitor : INetworkStatusMonitor
    {
        public event EventHandler<NetworkState> StateChanged;

        private NetworkState previousState = NetworkState.Unknown;

        public NetworkState State
        {
            get
            {
                UpdateState();
                return previousState;
            }
        }

        public bool CanConnect
        {
            get
            {
                return CouldConnect(State);
            }
        }

        public void UpdateState()
        {
            var state = NetworkState.Unknown;
            var connectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
            var activeNetworkInfo = connectivityManager.ActiveNetworkInfo;

            if (activeNetworkInfo != null && activeNetworkInfo.IsConnectedOrConnecting)
            {
                state = activeNetworkInfo.Type == ConnectivityType.Wifi ? NetworkState.ConnectedWifi : NetworkState.ConnectedData;
            }
            else
            {
                state = NetworkState.Disconnected;
            }

            if (state != previousState)
            {
                previousState = state;
                StateChanged?.Invoke(this, state);
            }
        }

        public bool CouldConnect(NetworkState state)
        {
            if (state == NetworkState.Disconnected)
            {
                return false;
            }

            if (state == NetworkState.ConnectedData && !Dependency.PreferenceManager.UseCellularData)
            {
                return false;
            }

            return true;
        }
    }
}