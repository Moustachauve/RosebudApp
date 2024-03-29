﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Sothree.Slidinguppanel;
using RosebudAppAndroid.Adapters;
using RosebudAppCore;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Model;
using Newtonsoft.Json;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using RosebudAppAndroid.Fragments;
using System;
using Android.Support.V7.Widget;
using Android.Gms.Common;

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "TripDetailsActivity")]
    public class TripDetailsActivity : AppCompatActivity, IOnMapReadyCallback, View.IOnClickListener
    {
        const int REQUEST_GOOGLE_PLAY_SERVICES = 42;
        const int REQUEST_LOCATION_ID = 0;
        readonly string[] PermissionsLocation =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        Route routeInfo;
        Trip tripInfo;
        string stopId;
        StopTimeTripAdapter stopAdapter;
        RecyclerView stopRecyclerView;
        TextView emptyView;
        LinearLayout slidingContainer;
        SlidingUpPanelLayout slidingLayout;

        ToolbarCompat toolbar;

        LinearLayout dragView;
        TextView lblRouteShortName;
        TextView lblRouteLongName;
        TextView lblTripHeadsign;

        GoogleMap map;
        bool isMapLoaded = false;
        bool isMapAvailable;
        List<Marker> markers = new List<Marker>();
        LatLngBounds mapBounds;
        int mapBoundPadding;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.trip_details);

            NetworkStatusFragment networkFragment = (NetworkStatusFragment)SupportFragmentManager.FindFragmentById(Resource.Id.network_fragment);
            toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            toolbar.SetNavigationOnClickListener(this);

            Window.AddFlags(WindowManagerFlags.TranslucentStatus);

            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.trip_map);
            slidingContainer = FindViewById<LinearLayout>(Resource.Id.sliding_container);
            slidingLayout = FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_layout);
            stopRecyclerView = FindViewById<RecyclerView>(Resource.Id.stop_recyclerview);
            emptyView = FindViewById<TextView>(Resource.Id.empty_view);
            dragView = FindViewById<LinearLayout>(Resource.Id.drag_view);
            lblRouteShortName = FindViewById<TextView>(Resource.Id.lbl_route_short_name);
            lblRouteLongName = FindViewById<TextView>(Resource.Id.lbl_route_long_name);
            lblTripHeadsign = FindViewById<TextView>(Resource.Id.lbl_trip_headsign);

            mapBoundPadding = 30 * (int)Resources.DisplayMetrics.Density;

            if (IsGooglePlayServiceAvailable())
            {
                isMapAvailable = true;
                mapFragment.GetMapAsync(this);
            }
            else
            {
                isMapAvailable = false;
                int paddingPixel = 25 * (int)Resources.DisplayMetrics.Density;
                int paddingPixelTop = 120 * (int)Resources.DisplayMetrics.Density;
                mapFragment.View.SetPadding(paddingPixel, paddingPixelTop, paddingPixel, paddingPixel);
            }

            slidingLayout.AnchorPoint = 0.4f;
            slidingLayout.SetPanelState(SlidingUpPanelLayout.PanelState.Anchored);
            slidingLayout.CoveredFadeColor = Color.Transparent;
            slidingLayout.ClipPanel = false;

            slidingLayout.Post(delegate
            {
                toolbar.SetPadding(0, GetStatusBarHeight(), 0, 0);
                toolbar.LayoutParameters.Height += GetStatusBarHeight();

                var layoutParams = new SlidingUpPanelLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                layoutParams.SetMargins(0, GetStatusBarHeight(), 0, 0);
                slidingContainer.LayoutParameters = layoutParams;
            });


            routeInfo = JsonConvert.DeserializeObject<Route>(Intent.GetStringExtra("routeInfos"));
            tripInfo = JsonConvert.DeserializeObject<Trip>(Intent.GetStringExtra("tripInfos"));
            stopId = Intent.GetStringExtra("stopId");

            ShowRouteInfo();
            SetRouteColor();

            toolbar.Post(async () =>
            {
                await LoadStops();
            });
            

            slidingLayout.PanelStateChanged += delegate (object sender, SlidingUpPanelLayout.PanelStateChangedEventArgs args)
            {
                UpdateStatusBar(args.P2);
                if (args.P2 != SlidingUpPanelLayout.PanelState.Dragging && args.P2 != SlidingUpPanelLayout.PanelState.Expanded)
                {
                    UpdateMapZoom();
                }
            };

            slidingLayout.PanelSlide += SlidingLayout_PanelSlide;

            networkFragment.RetryLastRequest += async (object sender, EventArgs args) =>
            {
                await LoadStops();
            };
        }

        void SlidingLayout_PanelSlide(object sender, SlidingUpPanelLayout.PanelSlideEventArgs e)
        {
            if (slidingContainer.Top > 200)
            {
                UpdateMapPadding();
            }
        }

        public override void OnBackPressed()
        {
            if (slidingLayout != null &&
                (slidingLayout.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded || slidingLayout.GetPanelState() == SlidingUpPanelLayout.PanelState.Anchored)
                && emptyView.Visibility != ViewStates.Visible)
            {
                slidingLayout.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case REQUEST_LOCATION_ID:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            ShowMyPosition();
                        }
                        else
                        {
                            //Permission Denied :(
                            //Disabling location functionality
                            var snack = Snackbar.Make(lblTripHeadsign, "Location permission is denied.", Snackbar.LengthShort);
                            snack.Show();
                        }
                    }
                    break;
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.UiSettings.MapToolbarEnabled = false;
            map.CameraChange += OnCameraChange;
            isMapLoaded = true;

            UpdateMapZoom();
            UpdateStatusBar(slidingLayout.GetPanelState());

            if ((int)Build.VERSION.SdkInt < 23)
            {
                ShowMyPosition();
            }
            else
            {
                RequestLocationPermission();
            }
        }

        async Task waitForMap()
        {
            while (!isMapLoaded)
            {
                await Task.Delay(200);
            }

            return;
        }

        async Task LoadStops(bool overrideCache = false)
        {
            var details = await StopAccessor.GetStopsForTrip(routeInfo.feed_id, routeInfo.route_id, tripInfo.trip_id, overrideCache);

            if (details == null)
            {
                slidingLayout.SetPanelState(SlidingUpPanelLayout.PanelState.Expanded);
                stopRecyclerView.Visibility = ViewStates.Gone;
                emptyView.Visibility = ViewStates.Visible;
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                //slidingLayout.TouchEnabled = false;
                return;
            }

            emptyView.Visibility = ViewStates.Gone;
            stopRecyclerView.Visibility = ViewStates.Visible;

            if (stopAdapter == null)
            {
                stopAdapter = new StopTimeTripAdapter(this, details.stops, routeInfo);
                //stopAdapter.ItemClick += OnItemClicked;
                stopRecyclerView.SetAdapter(stopAdapter);
                stopRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            }
            else
            {
                stopAdapter.ReplaceItems(details.stops);
            }

            if (isMapAvailable)
            {
                showTripOnMap(details);
            }

            stopRecyclerView.Post(() =>
            {
                if(!string.IsNullOrWhiteSpace(stopId))
                {
                    int position = stopAdapter.GetPositionByStopId(stopId);
                    ((LinearLayoutManager)stopRecyclerView.GetLayoutManager()).ScrollToPositionWithOffset(position, 50);
                }
            });
        }

        async void showTripOnMap(TripDetails details)
        {
            if (details.shape == null || details.shape.Count <= 0)
            {
                View viewNoMap = FindViewById(Resource.Id.view_no_map);
                viewNoMap.Visibility = ViewStates.Visible;
                UpdateMapPadding();
                return;
            }

            PolylineOptions tripLine = new PolylineOptions();
            LatLngBounds.Builder boundsBuilder = new LatLngBounds.Builder();
            Color lineColor;

            tripLine.InvokeWidth(6 * (int)Resources.DisplayMetrics.Density);
            if (!string.IsNullOrWhiteSpace(routeInfo.route_color))
            {
                lineColor = Color.ParseColor(ColorHelper.FormatColor(routeInfo.route_color));
            }
            else
            {
                lineColor = new Color(ContextCompat.GetColor(this, Resource.Color.default_item_color));
            }

            tripLine.InvokeColor(lineColor);

            foreach (ShapePoint point in details.shape)
            {
                LatLng coord = new LatLng(point.shape_pt_lat, point.shape_pt_lon);
                tripLine.Add(coord);
                boundsBuilder.Include(coord);
            }

            mapBounds = boundsBuilder.Build();

            var markerIcon = BitmapDescriptorFactory.FromBitmap(getMarkerBitmapFromView(lineColor));

            //We need our map before continuing!
            await waitForMap();

            foreach (var stop in details.stops)
            {
                LatLng coord = new LatLng(stop.stop_lat, stop.stop_lon);
                MarkerOptions stopMarker = new MarkerOptions();
                stopMarker.SetPosition(coord);
                stopMarker.SetTitle(stop.stop_name);
                stopMarker.SetIcon(markerIcon);
                stopMarker.Anchor(0.5f, 0.5f);
                markers.Add(map.AddMarker(stopMarker));
            }

            CameraUpdate cameraPosition = CameraUpdateFactory.NewLatLngBounds(mapBounds, mapBoundPadding);

            map.AddPolyline(tripLine);
            map.MoveCamera(cameraPosition);

            UpdateMapZoom();
        }

        void ShowRouteInfo()
        {
            lblRouteShortName.Text = routeInfo.route_short_name;
            lblTripHeadsign.Text = tripInfo.trip_headsign;

            if (!string.IsNullOrWhiteSpace(routeInfo.route_long_name))
            {
                lblRouteLongName.Text = routeInfo.route_long_name;
            }
            else
            {
                lblRouteLongName.Text = routeInfo.route_desc;
            }

        }

        void SetRouteColor()
        {
            Color mainColor;

            if (!string.IsNullOrWhiteSpace(routeInfo.route_color))
            {
                mainColor = Color.ParseColor(ColorHelper.FormatColor(routeInfo.route_color));
            }
            else
            {
                mainColor = new Color(ContextCompat.GetColor(this, Resource.Color.default_item_color));
            }

            Color contrastColor = ColorHelper.ContrastColor(mainColor);

            dragView.SetBackgroundColor(mainColor);
            lblRouteShortName.SetTextColor(contrastColor);
            lblRouteLongName.SetTextColor(contrastColor);
            lblTripHeadsign.SetTextColor(contrastColor);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.SetStatusBarColor(ColorHelper.DarkenColor(mainColor));
            }
        }

        public void RequestLocationPermission()
        {
            string permission = Manifest.Permission.AccessFineLocation;
            if (CheckSelfPermission(permission) == Permission.Granted)
            {
                ShowMyPosition();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                Snackbar.Make(lblTripHeadsign, "Localisation désactivé", Snackbar.LengthLong)
                        .SetAction("Activer", v => RequestPermissions(PermissionsLocation, REQUEST_LOCATION_ID))
                        .Show();
                return;
            }

            RequestPermissions(PermissionsLocation, REQUEST_LOCATION_ID);
        }

        void ShowMyPosition()
        {
            map.UiSettings.MyLocationButtonEnabled = true;
            map.MyLocationEnabled = true;
        }

        Bitmap getMarkerBitmapFromView(Color color)
        {
            View customMarkerView = ((LayoutInflater)GetSystemService(Context.LayoutInflaterService)).Inflate(Resource.Layout.map_marker, null);

            View view = customMarkerView.FindViewById(Resource.Id.map_marker_view);
            GradientDrawable shape = (GradientDrawable)view.Background.Mutate();
            shape.SetStroke(3 * (int)Resources.DisplayMetrics.Density, color);

            customMarkerView.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
            customMarkerView.Layout(0, 0, customMarkerView.MeasuredWidth, customMarkerView.MeasuredHeight);
            customMarkerView.BuildDrawingCache();
            Bitmap returnedBitmap = Bitmap.CreateBitmap(customMarkerView.MeasuredWidth, customMarkerView.MeasuredHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(returnedBitmap);
            canvas.DrawColor(Color.White, PorterDuff.Mode.SrcIn);
            Drawable drawable = customMarkerView.Background;
            if (drawable != null)
                drawable.Draw(canvas);
            customMarkerView.Draw(canvas);
            return returnedBitmap;
        }

        void OnCameraChange(object sender, GoogleMap.CameraChangeEventArgs args)
        {
            if (args.Position.Zoom < 12.5)
            {
                markers.ForEach(m => m.Visible = false);
            }
            else
            {
                markers.ForEach(m => m.Visible = true);
            }
        }

        int GetStatusBarHeight()
        {
            Rect displayRect = new Rect();
            Window.DecorView.GetWindowVisibleDisplayFrame(displayRect);
            return displayRect.Top;
        }

        void UpdateMapPadding()
        {
            if (!isMapLoaded)
            {
                return;
            }

            Point size = new Point();
            WindowManager.DefaultDisplay.GetSize(size);

            int paddingTop = toolbar.Height - 25;
            int paddingBottom = size.Y - slidingContainer.Top - 210;
            map.SetPadding(0, paddingTop, 0, paddingBottom);
        }

        void UpdateMapZoom()
        {
            UpdateMapPadding();

            if (!isMapLoaded || mapBounds == null)
            {
                return;
            }

            CameraUpdate cameraPosition = CameraUpdateFactory.NewLatLngBounds(mapBounds, mapBoundPadding);
            map.AnimateCamera(cameraPosition);
        }

        void UpdateStatusBar(SlidingUpPanelLayout.PanelState slidingPanelState)
        {
            if (slidingPanelState == SlidingUpPanelLayout.PanelState.Expanded)
            {
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                var layoutParams = new SlidingUpPanelLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                layoutParams.SetMargins(0, 0, 0, 0);
                slidingContainer.LayoutParameters = layoutParams;
            }
            else
            {
                Window.AddFlags(WindowManagerFlags.TranslucentStatus);

                if (slidingPanelState != SlidingUpPanelLayout.PanelState.Dragging)
                {
                    var layoutParams = new SlidingUpPanelLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                    layoutParams.SetMargins(0, GetStatusBarHeight(), 0, 0);
                    slidingContainer.LayoutParameters = layoutParams;
                }
            }
        }

        bool IsGooglePlayServiceAvailable()
        {
            GoogleApiAvailability googleAPI = GoogleApiAvailability.Instance;
            int result = googleAPI.IsGooglePlayServicesAvailable(this);
            if (result != ConnectionResult.Success)
            {
                if (googleAPI.IsUserResolvableError(result))
                {
                    googleAPI.GetErrorDialog(this, result,
                            REQUEST_GOOGLE_PLAY_SERVICES).Show();
                }

                return false;
            }

            return true;
        }

        public void OnClick(View v)
        {
            //Back Button in nav bar
            OnBackPressed();
        }
    }
}

