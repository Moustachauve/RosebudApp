using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyTransitCore;
using Android.Graphics;
using System.Linq;
using MyTransitCore.Model;
using MyTransitCore.Utils;
using Java.Security;

namespace MyTransitAndroid.Adapters
{
	public class StopAdapter : GenericAdapter<Stop>
	{
		public StopAdapter(Context context, List<Stop> stops) : base(context, stops)
		{
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			if (convertView == null)
			{
				convertView = inflater.Inflate(Resource.Layout.stop_listitem, parent, false);
			}

			Stop currentStop = this[position];

			TextView lblStopName = convertView.FindViewById<TextView>(Resource.Id.lbl_stop_name);
			TextView lblArrivalTime = convertView.FindViewById<TextView>(Resource.Id.lbl_arrival_time);
			TextView lblDepartureTime = convertView.FindViewById<TextView>(Resource.Id.lbl_departure_time);

			lblStopName.Text = currentStop.stop_name;
			lblArrivalTime.Text = TimeFormatter.FormatHoursMinutes(currentStop.arrival_time);
			lblDepartureTime.Text = TimeFormatter.FormatHoursMinutes(currentStop.departure_time);

			if (lblArrivalTime.Text == lblDepartureTime.Text)
			{
				lblArrivalTime.Visibility = ViewStates.Gone;
			}

			return convertView;
		}

		protected override List<Stop> ApplyFilter()
		{
			return allItems;
		}

		public int GetPositionOfNextStop()
		{
			return 0;
		}
	}
}
