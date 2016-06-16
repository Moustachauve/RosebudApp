using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyTransit.Core;
using System.Linq;
using MyTransit.Android;
using Android.App;
using MyTransit.Core.Model;
using MyTransit.Core.Utils;

namespace MyTransit.Android.Adapters
{
	public class FeedAdapter : GenericAdapter<Feed>
	{

        public FeedAdapter(Context context, List<Feed> feeds) : base(context, feeds)
		{
		}

		public override long GetItemId(int position) { return this[position].feed_id; }

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

        protected override List<Feed> ApplyFilter()
        {
			if (string.IsNullOrWhiteSpace(Filter))
				return new List<Feed>(allItems);
            else
				return allItems.Where(feed => feed.agency_name.ContainsInsensitive(Filter)
				                      || feed.keywords.ContainsInsensitive(Filter))
					           .ToList();
        }
    }
}
