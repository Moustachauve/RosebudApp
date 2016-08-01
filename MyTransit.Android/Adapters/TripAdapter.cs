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
using Android.Support.V7.Widget;

namespace MyTransitAndroid.Adapters
{
	public class TripAdapter : BaseRecyclerAdapter<Trip>
	{
		public TripAdapter(Context context, List<Trip> trips) : base(context, trips)
		{
		}

		public int GetPositionOfNextTrip()
		{
			TimeSpan currentTime = DateTime.Now.TimeOfDay;
			int closestTimePosition = 0;

			long min = long.MaxValue;

			for (int i = 0; i < AllItems.Count; i++)
			{
				TimeSpan tripTime = TimeFormatter.StringToTimeSpan(AllItems[i].start_time);
				if (tripTime.Days > 0)
					tripTime = new TimeSpan(tripTime.Hours, tripTime.Minutes, tripTime.Seconds);

				long diff = Math.Abs(currentTime.Ticks - tripTime.Ticks);
				if (diff < min)
				{
					min = diff;
					closestTimePosition = i;
				}
			}

			return closestTimePosition;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View view = Inflater.Inflate(Resource.Layout.trip_listitem, parent, false);
			return new TripAdapter.TripViewHolder(view, OnClick);
		}

		protected override List<Trip> ApplyFilter()
		{
			return AllItems;
		}

		public class TripViewHolder : BaseViewHolder
		{
			public TripViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
			{
			}

			public override void BindData(Trip item)
			{
				TextView lblHeadsign = view.FindViewById<TextView>(Resource.Id.lbl_headsign);
				TextView lblFrequency = view.FindViewById<TextView>(Resource.Id.lbl_frequency);
				TextView lblStartTime = view.FindViewById<TextView>(Resource.Id.lbl_start_time);
				TextView lblEndTime = view.FindViewById<TextView>(Resource.Id.lbl_end_time);

				lblHeadsign.Text = item.trip_headsign;
				lblStartTime.Text = TimeFormatter.FormatHoursMinutes(item.start_time);
				lblEndTime.Text = TimeFormatter.FormatHoursMinutes(item.end_time);

				if (item.headway_secs.HasValue)
				{
					lblFrequency.Text = string.Format("À toutes les {0} min.", formatFrequencyTime(item.headway_secs.Value));
				}
				else {
					lblFrequency.Visibility = ViewStates.Gone;
				}
			}

			protected string formatFrequencyTime(int timeSeconds)
			{
				TimeSpan time = TimeSpan.FromSeconds(timeSeconds);

				return time.Minutes.ToString();
			}
		}

	}
}
