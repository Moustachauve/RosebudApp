using System.Collections.Generic;
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

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "TripDetailsActivity")]
    public class TripDetailsActivity : AppCompatActivity, IOnMapReadyCallback
    {
        const int RequestLocationId = 0;
        readonly string[] PermissionsLocation =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        Route routeInfo;
        Trip tripInfo;
        StopAdapter stopAdapter;
        ListView stopListView;
        TextView emptyView;
        LinearLayout slidingContainer;
        SlidingUpPanelLayout slidingLayout;

        ToolbarCompat toolbar;

        TextView lblRouteShortName;
        TextView lblRouteLongName;
        TextView lblTripHeadsign;

        GoogleMap map;
        bool isMapLoaded = false;
        List<Marker> markers = new List<Marker>();
        LatLngBounds mapBounds;
        int mapBoundPadding;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.trip_details);

            NetworkStatusFragment networkFragment = (NetworkStatusFragment)FragmentManager.FindFragmentById(Resource.Id.network_fragment);
            toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            toolbar.NavigationClick += delegate
            {
                OnBackPressed();
            };

            Window.AddFlags(WindowManagerFlags.TranslucentStatus);

            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.trip_map);
            slidingContainer = FindViewById<LinearLayout>(Resource.Id.sliding_container);
            slidingLayout = FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_layout);
            stopListView = FindViewById<ListView>(Resource.Id.stop_listview);
            emptyView = FindViewById<TextView>(Resource.Id.empty_view);
            lblRouteShortName = FindViewById<TextView>(Resource.Id.lbl_route_short_name);
            lblRouteLongName = FindViewById<TextView>(Resource.Id.lbl_route_long_name);
            lblTripHeadsign = FindViewById<TextView>(Resource.Id.lbl_trip_headsign);

            mapBoundPadding = 30 * (int)Resources.DisplayMetrics.Density;
            mapFragment.GetMapAsync(this);

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

            showRouteInfo();
            LoadStops();

            slidingLayout.PanelStateChanged += delegate (object sender, SlidingUpPanelLayout.PanelStateChangedEventArgs args)
            {
                UpdateStatusBar(args.P2);
                if (args.P2 != SlidingUpPanelLayout.PanelState.Dragging && args.P2 != SlidingUpPanelLayout.PanelState.Expanded)
                {
                    UpdateMapZoom();
                }
            };

            slidingLayout.PanelSlide += SlidingLayout_PanelSlide;

            networkFragment.RetryLastRequest += (object sender, EventArgs args) =>
            {
                LoadStops();
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
                case RequestLocationId:
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

        async void LoadStops(bool overrideCache = false)
        {
            var details = await StopAccessor.GetStopsForTrip(routeInfo.feed_id, routeInfo.route_id, tripInfo.trip_id, overrideCache);

            if (details == null)
            {
                slidingLayout.SetPanelState(SlidingUpPanelLayout.PanelState.Expanded);
                stopListView.Visibility = ViewStates.Gone;
                emptyView.Visibility = ViewStates.Visible;
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                //slidingLayout.TouchEnabled = false;
                return;
            }

            emptyView.Visibility = ViewStates.Gone;
            stopListView.Visibility = ViewStates.Visible;

            if (stopAdapter == null)
            {
                if (details != null)
                {
                    stopAdapter = new StopAdapter(this, details.stops);
                    stopListView.Adapter = stopAdapter;
                }
                InvalidateOptionsMenu();
            }
            else
            {
                if (details == null)
                {
                    stopAdapter.ReplaceItems(new List<Stop>());
                }
                else
                {
                    stopAdapter.ReplaceItems(details.stops);
                }
            }

            showTripOnMap(details);

            //stopListView.Post(() =>
            //{
            //	stopListView.SmoothScrollToPositionFromTop(stopAdapter.GetPositionOfNextTrip(), 50);
            //});
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

        void showRouteInfo()
        {
            lblRouteShortName.Text = routeInfo.route_short_name;
            lblRouteLongName.Text = routeInfo.route_long_name;
            lblTripHeadsign.Text = tripInfo.trip_headsign;

            if (!string.IsNullOrWhiteSpace(routeInfo.route_color))
            {
                lblRouteShortName.SetBackgroundColor(Color.ParseColor(ColorHelper.FormatColor(routeInfo.route_color)));
                lblRouteShortName.SetTextColor(ColorHelper.ContrastColor(routeInfo.route_color));
            }
        }

        public void RequestLocationPermission()
        {
            string permission = Manifest.Permission.AccessFineLocation;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                ShowMyPosition();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                Snackbar.Make(lblTripHeadsign, "Localisation désactivé", Snackbar.LengthLong)
                        .SetAction("Activer", v => RequestPermissions(PermissionsLocation, RequestLocationId))
                        .Show();
                return;
            }

            RequestPermissions(PermissionsLocation, RequestLocationId);
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
    }
}

