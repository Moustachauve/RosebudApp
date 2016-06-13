using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyTransit.Core;
using System.Linq;
using MyTransit.Android;
using Android.App;

namespace MyTransit
{
	public class FeedAdapter : BaseAdapter<Feed>
	{
		LayoutInflater inflater;

        private Context context;
        private List<Feed> feeds;
        private List<Feed> filteredFeeds;
        private string filter;

        public override Feed this[int position] { get { return filteredFeeds[position]; } }
		public override int Count { get { return filteredFeeds.Count; } }

        public string Filter
        {
            get { return filter; }
            set
            {
                if (value.ToLower() == filter)
                    return;

                filter = value.ToLower();
                ApplyFilter();
            }
        }

        public FeedAdapter(Context context, List<Feed> feeds)
		{
            this.context = context;
			this.inflater = LayoutInflater.FromContext(context);
			this.feeds = feeds;
            ApplyFilter();
		}

        public override long GetItemId(int position) { return filteredFeeds[position].feed_id; }

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
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(filter))
                filteredFeeds = new List<Feed>(feeds);
            else
                filteredFeeds = feeds.Where(feed => feed.agency_name.ToLower().Contains(filter)).ToList();

            
            NotifyDataSetChanged();
        }
    }
}
