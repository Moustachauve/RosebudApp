using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using RosebudAppCore;
using System.Linq;
using RosebudAppAndroid;
using Android.App;
using RosebudAppCore.Model;
using RosebudAppCore.Utils;
using Android.Support.V7.Widget;

namespace RosebudAppAndroid.Adapters
{
	public class FeedAdapter : SearchableRecyclerAdapter<Feed>
	{
        public FeedAdapter(Context context, List<Feed> feeds) : base(context, feeds)
		{
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View view = Inflater.Inflate(Resource.Layout.feed_listitem, parent, false);
			return new FeedAdapter.FeedViewHolder(view, OnClick);
		}

		protected override List<Feed> ApplyFilter()
        {
			if (string.IsNullOrWhiteSpace(Filter))
				return new List<Feed>(AllItems);
            else
				return AllItems.Where(feed => feed.agency_name.ContainsInsensitive(Filter)
				                      || feed.keywords.ContainsInsensitive(Filter))
					           .ToList();
        }

		public class FeedViewHolder : BaseViewHolder
		{
			public FeedViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
			{
			}

			public override void BindData(Feed item, int position)
			{
				TextView lblAgencyName = view.FindViewById<TextView>(Resource.Id.lbl_agency_name);

				lblAgencyName.Text = item.agency_name;
			}
		}

	}
}
