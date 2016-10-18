using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RosebudAppCore.Utils;
using Android.Locations;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Content.Res;
using System.Threading.Tasks;

namespace RosebudAppAndroid.Utils
{
    public class LocationService : Java.Lang.Object, ILocationService, GoogleApiClient.IConnectionCallbacks, Android.Gms.Location.ILocationListener, GoogleApiClient.IOnConnectionFailedListener
    {
        const long UPDATE_INTERVAL = 30000;
        const long FASTEST_UPDATE_INTERVAL = UPDATE_INTERVAL / 2;
        const long DISPLACEMENT = 10;

        Context Context;

        GoogleApiClient GoogleApiClient;
        LocationRequest LocationRequest;

        private Location lastKnownLocation;
        public RosebudAppCore.Model.Location LastKnownLocation
        {
            get
            {
                if (lastKnownLocation == null)
                    return null;

                return new RosebudAppCore.Model.Location(lastKnownLocation.Latitude, lastKnownLocation.Longitude);
            }
        }

        List<ILocationServiceListener> LocationListeners;

        bool RequestingLocationUpdates;

        public string LastUpdateTime { get; private set; }

        public LocationService(Context context)
        {
            Context = context;
            LocationRequest = new LocationRequest();
            LocationListeners = new List<ILocationServiceListener>();
            BuildGoogleApiClient();
        }

        protected void CreateLocationRequest()
        {
            LocationRequest = new LocationRequest();
            LocationRequest.SetInterval(UPDATE_INTERVAL);
            LocationRequest.SetFastestInterval(FASTEST_UPDATE_INTERVAL);
            LocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            LocationRequest.SetSmallestDisplacement(DISPLACEMENT);
        }

        protected void BuildGoogleApiClient()
        {
            GoogleApiClient = new GoogleApiClient.Builder(Context)
                .AddConnectionCallbacks(this)
                .AddApi(LocationServices.API)
                .AddOnConnectionFailedListener(this)
                .Build();

            //ConnectGoogleApi();

            CreateLocationRequest();
        }

        public void ConnectGoogleApi()
        {
            GoogleApiClient.Connect();
        }

        private async Task StartLocationUpdates()
        {
            RequestingLocationUpdates = true;

            if (GoogleApiClient.IsConnected && RequestingLocationUpdates)
            {
                var result = await LocationServices.FusedLocationApi.RequestLocationUpdates(GoogleApiClient, LocationRequest, this);
            }
        }

        private async Task StopLocationUpdates()
        {
            RequestingLocationUpdates = false;

            if (GoogleApiClient.IsConnected)
                await LocationServices.FusedLocationApi.RemoveLocationUpdates(GoogleApiClient, this);
        }

        public async void OnConnected(Bundle connectionHint)
        {
            if (lastKnownLocation == null)
            {
                lastKnownLocation = LocationServices.FusedLocationApi.GetLastLocation(GoogleApiClient);

                if (lastKnownLocation == null)
                {
                    var result = await LocationServices.FusedLocationApi.RequestLocationUpdates(GoogleApiClient, LocationRequest, this);
                    Toast.MakeText(Context, "Allo", ToastLength.Long);
                }

                LastUpdateTime = DateTime.Now.TimeOfDay.ToString();
            }

            if (RequestingLocationUpdates)
            {
                await StartLocationUpdates();
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            GoogleApiClient.Connect();
        }

        public void OnLocationChanged(Location location)
        {
            lastKnownLocation = location;
            LastUpdateTime = DateTime.Now.TimeOfDay.ToString();
            NotifyListeners(location);
        }

        public async void AddOnLocationChangedListener(ILocationServiceListener listener)
        {
            if (!LocationListeners.Contains(listener))
                LocationListeners.Add(listener);

            if (!RequestingLocationUpdates)
                await StartLocationUpdates();
        }

        public async void RemoveOnLocationChangedListener(ILocationServiceListener listener)
        {
            LocationListeners.Remove(listener);

            if (RequestingLocationUpdates && LocationListeners.Count <= 0)
                await StopLocationUpdates();
        }

        private void NotifyListeners(Location location)
        {
            if (LocationListeners.Count <= 0)
                return;

            foreach (var listener in LocationListeners)
            {
                listener.OnLocationChanged(new RosebudAppCore.Model.Location(location.Latitude, location.Longitude));
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }

        public int CalculateDistance(double latA, double lonA, double latB, double lonB)
        {
            Location locationA = new Location("point A");

            locationA.Latitude = latA;
            locationA.Longitude = lonA;

            Location locationB = new Location("point B");

            locationB.Latitude = latB;
            locationB.Longitude = lonB;

            return (int)locationA.DistanceTo(locationB);
        }
    }
}