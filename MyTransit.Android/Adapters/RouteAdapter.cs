using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyTransit.Core;
using Android.Graphics;
using System.Linq;
using MyTransit.Core.Model;
using MyTransit.Core.Utils;
using Android.Support.V4.Graphics;

namespace MyTransit.Android.Adapters
{
	public class RouteAdapter : GenericAdapter<Route>
	{
		public RouteAdapter(Context context, List<Route> routes) : base(context, routes)
		{
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{

			if (convertView == null)
			{
				convertView = inflater.Inflate(Resource.Layout.route_listitem, parent, false);
			}

			Route currentRoute = this[position];

			TextView lblRouteShortName = convertView.FindViewById<TextView>(Resource.Id.lbl_route_short_name);
			TextView lblRouteLongName = convertView.FindViewById<TextView>(Resource.Id.lbl_route_long_name);

			lblRouteShortName.Text = currentRoute.route_short_name;
			lblRouteLongName.Text = currentRoute.route_long_name;

            if (!string.IsNullOrWhiteSpace(currentRoute.route_color))
            {
				lblRouteShortName.SetBackgroundColor(Color.ParseColor(ColorHelper.FormatColor(currentRoute.route_color)));
				lblRouteShortName.SetTextColor(ColorHelper.ContrastColor(currentRoute.route_color));
            }

            return convertView;
		}

		protected override List<Route> ApplyFilter()
		{
			if (string.IsNullOrWhiteSpace(Filter))
				return new List<Route>(allItems);
			else
				return allItems.Where(r => r.route_short_name.ContainsInsensitive(Filter) 
			                      || r.route_long_name.ContainsInsensitive(Filter))
					           .ToList();
		}

	}
}
