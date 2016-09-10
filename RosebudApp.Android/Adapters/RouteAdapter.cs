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
using Android.Support.V4.Graphics;
using Android.Support.V7.Widget;

namespace RosebudAppAndroid.Adapters
{
	public class RouteAdapter : SearchableRecyclerAdapter<Route>
	{
		public RouteAdapter(Context context, List<Route> routes) : base(context, routes)
		{
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View view = Inflater.Inflate(Resource.Layout.route_listitem, parent, false);
			return new RouteAdapter.RouteViewHolder(view, OnClick);
		}

		protected override List<Route> ApplyFilter()
		{
			if (string.IsNullOrWhiteSpace(Filter))
				return new List<Route>(AllItems);
			else
				return AllItems.Where(r => r.route_short_name.ContainsInsensitive(Filter) 
			                      || r.route_long_name.ContainsInsensitive(Filter))
					           .ToList();
		}

		public class RouteViewHolder : BaseViewHolder
		{
			public RouteViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
			{
			}

			public override void BindData(Route item, int position)
			{
				TextView lblRouteShortName = view.FindViewById<TextView>(Resource.Id.lbl_route_short_name);
				TextView lblRouteLongName = view.FindViewById<TextView>(Resource.Id.lbl_route_long_name);

				lblRouteShortName.Text = item.route_short_name;

                if(!string.IsNullOrWhiteSpace(item.route_long_name))
                {
                    lblRouteLongName.Text = item.route_long_name;
                }
                else
                {
                    lblRouteLongName.Text = item.route_desc;
                }

				if (!string.IsNullOrWhiteSpace(item.route_color))
				{
					lblRouteShortName.SetBackgroundColor(Color.ParseColor(ColorHelper.FormatColor(item.route_color)));
					lblRouteShortName.SetTextColor(ColorHelper.ContrastColor(item.route_color));
				}
			}
		}
	}
}
