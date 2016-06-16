using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyTransit.Core;
using Android.Graphics;

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
                lblRouteShortName.SetBackgroundColor(Color.ParseColor(FormatColor(currentRoute.route_color)));
            }
            if (!string.IsNullOrWhiteSpace(currentRoute.route_text_color))
            {
                lblRouteShortName.SetTextColor(Color.ParseColor(FormatColor(currentRoute.route_text_color)));
            }

            return convertView;
		}

		protected override List<Route> ApplyFilter()
		{
			return allItems;
		}

		private string FormatColor(string color)
        {
            if (color[0] != '#')
                color = "#" + color;

            return color;
        }
	}
}
