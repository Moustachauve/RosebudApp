using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using RosebudAppCore;
using Android.Graphics;
using System.Linq;
using RosebudAppCore.Model;
using RosebudAppCore.Utils;
using Java.Security;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Support.V4.Content;

namespace RosebudAppAndroid.Adapters
{
	public class StopTimeTripAdapter : BaseRecyclerAdapter<StopDetails>
	{
        Route RouteInfo;

		public StopTimeTripAdapter(Context context, List<StopDetails> stops, Route routeInfo) : base(context, stops)
		{
            RouteInfo = routeInfo;
		}

        public int GetPositionByStopId(string stopId)
        {
            for (int i = 0; i < AllItems.Count; i++)
            {
                if (AllItems[i].stop_id == stopId)
                    return i;
            }

            return -1;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Inflater.Inflate(Resource.Layout.stop_time_trip_listitem, parent, false);
            return new StopTimeTripViewHolder(view, OnClick, Context, this, RouteInfo);
        }

        public class StopTimeTripViewHolder : BaseViewHolder
        {
            Context Context;
            StopTimeTripAdapter Adapter;
            Route RouteInfo;

            TextView lblStopName;
            TextView lblDepartureTime;
            View lineStopTop;
            View lineStopBottom;


            public StopTimeTripViewHolder(View itemView, Action<int> listener, Context context, StopTimeTripAdapter adapter, Route routeInfo) : base(itemView, null)
            {
                Context = context;
                Adapter = adapter;
                RouteInfo = routeInfo;

                lblStopName = view.FindViewById<TextView>(Resource.Id.lbl_stop_name);
                lineStopTop = view.FindViewById<View>(Resource.Id.line_stop_top);
                lineStopBottom = view.FindViewById<View>(Resource.Id.line_stop_bottom);
                lblDepartureTime = view.FindViewById<TextView>(Resource.Id.lbl_departure_time);

                SetLineColor();
            }

            public override void BindData(StopDetails item, int position)
            {
                lblStopName.Text = item.stop_name;
                lblDepartureTime.Text = TimeFormatter.FormatHoursMinutes(item.departure_time);

                lineStopTop.Visibility = position == 0 ? ViewStates.Invisible : ViewStates.Visible;
                lineStopBottom.Visibility = position == Adapter.ItemCount - 1 ? ViewStates.Invisible : ViewStates.Visible;
            }

            private void SetLineColor()
            {
                Color lineColor;

                if (!string.IsNullOrWhiteSpace(RouteInfo.route_color))
                {
                    lineColor = Color.ParseColor(ColorHelper.FormatColor(RouteInfo.route_color));
                }
                else
                {
                    lineColor = new Color(ContextCompat.GetColor(Context, Resource.Color.default_item_color));
                }

                Color contrastColor = ColorHelper.ContrastColor(lineColor);

                lineStopTop.SetBackgroundColor(lineColor);
                lineStopBottom.SetBackgroundColor(lineColor);
                lblDepartureTime.SetBackgroundColor(lineColor);
                lblDepartureTime.SetTextColor(contrastColor);
            }

        }

    }
}
