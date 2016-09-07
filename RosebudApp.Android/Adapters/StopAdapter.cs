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
using Android.Support.V7.Widget;
using RosebudAppCore.Model.Enum;
using Android.Graphics.Drawables;

namespace RosebudAppAndroid.Adapters
{
    public class StopAdapter : BaseRecyclerAdapter<Stop>
    {
        Route RouteInfo;

        public StopAdapter(Context context, List<Stop> stops, Route routeInfo) : base(context, stops)
        {
            RouteInfo = routeInfo;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Inflater.Inflate(Resource.Layout.stop_listitem, parent, false);
            return new StopViewHolder(view, OnClick, Context, this, RouteInfo);
        }

        public class StopViewHolder : BaseViewHolder
        {
            Context Context;
            StopAdapter Adapter;
            Route RouteInfo;

            TextView lblStopName;
            View lineStopTop;
            View lineStopBottom;
            View mapMarker;


            public StopViewHolder(View itemView, Action<int> listener, Context context, StopAdapter adapter, Route routeInfo) : base(itemView, listener)
            {
                Context = context;
                Adapter = adapter;
                RouteInfo = routeInfo;

                lblStopName = view.FindViewById<TextView>(Resource.Id.lbl_stop_name);
                lineStopTop = view.FindViewById<View>(Resource.Id.line_stop_top);
                lineStopBottom = view.FindViewById<View>(Resource.Id.line_stop_bottom);
                mapMarker = view.FindViewById<View>(Resource.Id.map_marker);

                SetLineColor();
            }

            public override void BindData(Stop item, int position)
            {
                lblStopName.Text = item.stop_name;

                lineStopTop.Visibility = position == 0 ? ViewStates.Invisible : ViewStates.Visible;
                lineStopBottom.Visibility = position == Adapter.ItemCount - 1 ? ViewStates.Invisible : ViewStates.Visible;

                if (item.location_type == LocationType.Station)
                {
                    //lblTempType.Text = "O";
                }

                
            }

            private void SetLineColor()
            {
                Color lineColor = new Color(Context.GetColor(Resource.Color.default_item_color));

                if (!string.IsNullOrWhiteSpace(RouteInfo.route_color))
                {
                    lineColor = Color.ParseColor(ColorHelper.FormatColor(RouteInfo.route_color));
                }

                lineStopTop.SetBackgroundColor(lineColor);
                lineStopBottom.SetBackgroundColor(lineColor);

                GradientDrawable shape = (GradientDrawable)mapMarker.Background.Mutate();
                shape.SetStroke(3 * (int)Context.Resources.DisplayMetrics.Density, lineColor);
            }

            protected string formatFrequencyTime(int timeSeconds)
            {
                TimeSpan time = TimeSpan.FromSeconds(timeSeconds);

                return time.Minutes.ToString();
            }
        }

    }
}
