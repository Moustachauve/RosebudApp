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
using MyTransit.Android.Adapters;
using MyTransit.Core;
using MyTransit.Core.DataAccessor;
using MyTransit.Core.Model;
using Newtonsoft.Json;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;

namespace MyTransit.Android.Activities
{
	[Activity(Label = "TripDetailsActivity")]
	public class TripDetailsActivity : AppCompatActivity, IOnMapReadyCallback
	{
		const int RequestLocationId = 0;
		private readonly string[] PermissionsLocation =
		{
			Manifest.Permission.AccessCoarseLocation,
			Manifest.Permission.AccessFineLocation
		};

		private Route routeInfo;
		private Trip tripInfo;
		private StopAdapter stopAdapter;
		private ListView stopListView;
		LinearLayout slidingContainer;
		SlidingUpPanelLayout slidingLayout;

		private TextView lblRouteShortName;
		private TextView lblRouteLongName;
		private TextView lblTripHeadsign;

		private GoogleMap map;
		private bool isMapLoaded = false;
		private List<Marker> markers = new List<Marker>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.trip_details);

			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
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
			lblRouteShortName = FindViewById<TextView>(Resource.Id.lbl_route_short_name);
			lblRouteLongName = FindViewById<TextView>(Resource.Id.lbl_route_long_name);
			lblTripHeadsign = FindViewById<TextView>(Resource.Id.lbl_trip_headsign);

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
				if (args.P2 == SlidingUpPanelLayout.PanelState.Expanded)
				{
					Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
				}
				else
				{
					Window.AddFlags(WindowManagerFlags.TranslucentStatus);
				}
			};
		}

		public override void OnBackPressed()
		{
			if (slidingLayout != null &&
				(slidingLayout.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded || slidingLayout.GetPanelState() == SlidingUpPanelLayout.PanelState.Anchored))
			{
				slidingLayout.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
			}
			else {
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

			if ((int)Build.VERSION.SdkInt < 23)
			{
				ShowMyPosition();
			}
			else {
				RequestLocationPermission();
			}
		}

		private async Task waitForMap()
		{
			while (!isMapLoaded)
			{
				await Task.Delay(200);
			}

			return;
		}

		private async void LoadStops()
		{
			var details = await StopAccessor.GetTripsForRoute(routeInfo.feed_id, routeInfo.route_id, tripInfo.trip_id);

			if (stopAdapter == null)
			{
				stopAdapter = new StopAdapter(this, details.stops);
				stopListView.Adapter = stopAdapter;
				InvalidateOptionsMenu();
			}
			else {
				stopAdapter.ReplaceItems(details.stops);
			}

			showTripOnMap(details);

			//stopListView.Post(() =>
			//{
			//	stopListView.SmoothScrollToPositionFromTop(stopAdapter.GetPositionOfNextTrip(), 50);
			//});
		}

		private async void showTripOnMap(TripDetails details)
		{
			PolylineOptions tripLine = new PolylineOptions();
			LatLngBounds.Builder boundsBuilder = new LatLngBounds.Builder();
			Color lineColor;

			tripLine.InvokeWidth(6 * (int)Resources.DisplayMetrics.Density);
			if (!string.IsNullOrWhiteSpace(routeInfo.route_color))
			{
				lineColor = Color.ParseColor(ColorHelper.FormatColor(routeInfo.route_color));
			}
			else {
				lineColor = new Color(ContextCompat.GetColor(this, Resource.Color.default_item_color));
			}

			tripLine.InvokeColor(lineColor);

			foreach (ShapePoint point in details.shape)
			{
				LatLng coord = new LatLng(point.shape_pt_lat, point.shape_pt_lon);
				tripLine.Add(coord);
				boundsBuilder.Include(coord);
			}

			LatLngBounds mapBounds = boundsBuilder.Build();
			int padding = 50 * (int)Resources.DisplayMetrics.Density;

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

			CameraUpdate cameraPosition = CameraUpdateFactory.NewLatLngBounds(mapBounds, padding);

			map.AddPolyline(tripLine);
			map.MoveCamera(cameraPosition);
		}

		private void showRouteInfo()
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

		private void ShowMyPosition()
		{
			map.UiSettings.MyLocationButtonEnabled = true;
			map.MyLocationEnabled = true;
		}

		private Bitmap getMarkerBitmapFromView(Color color)
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

		private void OnCameraChange(object sender, GoogleMap.CameraChangeEventArgs args)
		{
			if (args.Position.Zoom < 12.5)
			{
				markers.ForEach(m => m.Visible = false);
			}
			else {
				markers.ForEach(m => m.Visible = true);
			}
		}

		public int GetStatusBarHeight()
		{
			Rect displayRect = new Rect();
			Window.DecorView.GetWindowVisibleDisplayFrame(displayRect);
			return displayRect.Top;
		}
	}
}

