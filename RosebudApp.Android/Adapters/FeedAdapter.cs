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
using RosebudAppCore.DataAccessor;
using RosebudAppAndroid.Adapters.Events;

namespace RosebudAppAndroid.Adapters
{
    public class FeedAdapter : SearchableRecyclerAdapter<Feed>
    {
        public event EventHandler<ItemCheckChangedEventArgs> ItemFavoriteClick;

        public FeedAdapter(Context context, List<Feed> feeds) : base(context, feeds)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Inflater.Inflate(Resource.Layout.feed_listitem, parent, false);
            FeedViewHolder viewHolder = new FeedViewHolder(view, OnClick);
            viewHolder.FavoriteChange += OnFavoriteClick;
            return viewHolder;
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

        protected override List<Feed> ApplySort()
        {
            return AllItems.OrderByDescending(f => FavoriteFeedAccessor.IsFeedFavorite(f))
                .ThenBy(f => f.agency_name).ToList();
        }

        protected void OnFavoriteClick(object sender, ItemCheckChangedEventArgs e)
        {
            AnimateTo(ApplySort());
            ApplyFilter();
            ItemFavoriteClick?.Invoke(this, e);
        }

        public class FeedViewHolder : BaseViewHolder
        {
            public EventHandler<ItemCheckChangedEventArgs> FavoriteChange;
            TextView lblAgencyName;
            CheckBox chkFavorite;
            Feed currentItem;

            public FeedViewHolder(View itemView, Action<int> clickListener) : base(itemView, clickListener)
            {
                lblAgencyName = view.FindViewById<TextView>(Resource.Id.lbl_agency_name);
                chkFavorite = view.FindViewById<CheckBox>(Resource.Id.chk_favorite);
                chkFavorite.CheckedChange += ChkFavoriteCheckedChange;
            }

            public override void BindData(Feed item, int position)
            {
                this.currentItem = item;
                lblAgencyName.Text = item.agency_name;
                chkFavorite.Checked = FavoriteFeedAccessor.IsFeedFavorite(item);
            }

            private async void ChkFavoriteCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (FavoriteFeedAccessor.IsFeedFavorite(currentItem) == e.IsChecked)
                    return;

                await FavoriteFeedAccessor.SetFavoriteForFeed(e.IsChecked, currentItem);

                FavoriteChange?.Invoke(this, new ItemCheckChangedEventArgs(AdapterPosition, e.IsChecked));
            }
        }
    }
}
