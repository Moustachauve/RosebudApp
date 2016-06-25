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
using Java.Security;

namespace MyTransit.Android.Adapters
{
	public class TripAdapter : GenericAdapter<Trip>
	{
		public TripAdapter(Context context, List<Trip> trips) : base(context, trips)
		{
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{

			if (convertView == null)
			{
				convertView = inflater.Inflate(Resource.Layout.trip_listitem, parent, false);
			}

			Trip currentTrip = this[position];

			TextView lblHeadsign = convertView.FindViewById<TextView>(Resource.Id.lbl_headsign);
			TextView lblFrequency = convertView.FindViewById<TextView>(Resource.Id.lbl_frequency);
			TextView lblStartTime = convertView.FindViewById<TextView>(Resource.Id.lbl_start_time);
			TextView lblEndTime = convertView.FindViewById<TextView>(Resource.Id.lbl_end_time);

			lblHeadsign.Text = currentTrip.trip_headsign;
			lblStartTime.Text = TimeFormatter.FormatHoursMinutes(currentTrip.start_time);
			lblEndTime.Text = TimeFormatter.FormatHoursMinutes(currentTrip.end_time);

			if(currentTrip.headway_secs.HasValue) {
				lblFrequency.Text = string.Format("À toutes les {0} min.", formatFrequencyTime(currentTrip.headway_secs.Value));
			}
			else {
				lblFrequency.Visibility = ViewStates.Gone;
			}

			return convertView;
		}

		protected override List<Trip> ApplyFilter()
		{
			return allItems;
		}

		public int GetPositionOfNextTrip() {

			TimeSpan currentTime = DateTime.Now.TimeOfDay;
			int closestTimePosition = -1;

			long min = long.MaxValue;

			for (int i = 0; i < allItems.Count; i++)
			{
				TimeSpan tripTime = TimeFormatter.StringToTimeSpan(allItems[i].start_time);
				if (tripTime.Days > 0)
					tripTime = new TimeSpan(tripTime.Hours, tripTime.Minutes, tripTime.Seconds);

				long diff = Math.Abs(currentTime.Ticks - tripTime.Ticks);
				if(diff < min) {
					min = diff;
					closestTimePosition = i;
				}
			}

			return closestTimePosition;
		}

		private string formatFrequencyTime(int timeSeconds) {
			TimeSpan time = TimeSpan.FromSeconds(timeSeconds);

			return time.Minutes.ToString();
		}
	}
}
