using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyTransit.Core;

namespace MyTransit
{
	public class FeedAdapter : BaseAdapter<Feed>
	{
		LayoutInflater inflater;
		private List<Feed> feeds;

		public override Feed this[int position] { get { return feeds[position]; } }
		public override int Count { get { return feeds.Count; } }

		public FeedAdapter(Context context, List<Feed> feeds)
		{
			this.inflater = LayoutInflater.FromContext(context);
			this.feeds = feeds;
		}

		public override long GetItemId(int position) { return position; }

		public override View GetView(int position, View convertView, ViewGroup parent)
		{

			if (convertView == null)
			{
				convertView = inflater.Inflate(Resource.Layout.feed_listitem, parent, false);
			}

			Feed currentFeed = this[position];

			TextView lblAgencyName = convertView.FindViewById<TextView>(Resource.Id.lbl_agency_name);

			lblAgencyName.Text = currentFeed.agency_name;

			return convertView;
		}

		public void ReplaceData(List<Feed> feeds)
		{
			this.feeds = feeds;
			NotifyDataSetChanged();
		}
	}
}
