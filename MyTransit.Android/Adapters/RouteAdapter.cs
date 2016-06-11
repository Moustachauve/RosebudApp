using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyTransit.Core;

namespace MyTransit
{
	public class RouteAdapter : BaseAdapter<Route>
	{
		LayoutInflater inflater;
		private List<Route> routes;

		public override Route this[int position] { get { return routes[position]; } }
		public override int Count { get { return routes.Count; } }

		public RouteAdapter(Context context, List<Route> routes)
		{
			this.inflater = LayoutInflater.FromContext(context);
			this.routes = routes;
		}

		public override long GetItemId(int position) { return position; }

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

			return convertView;
		}

		public void ReplaceData(List<Route> routes)
		{
			this.routes = routes;
			NotifyDataSetChanged();
		}
	}
}
