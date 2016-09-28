using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using RosebudAppCore;
using Android.Graphics;
using System.Linq;
using RosebudAppCore.Model;
using RosebudAppCore.Utils;
using Android.Support.V4.Graphics;
using Android.Support.V7.Widget;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Comparators;
using RosebudAppAndroid.Adapters.Events;

namespace RosebudAppAndroid.Adapters
{
	public class RouteAdapter : SearchableRecyclerAdapter<Route>
	{
        public event EventHandler<ItemCheckChangedEventArgs> ItemFavoriteClick;

        public RouteAdapter(Context context, List<Route> routes) : base(context, routes)
		{
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View view = Inflater.Inflate(Resource.Layout.route_listitem, parent, false);

            RouteViewHolder viewHolder = new RouteViewHolder(view, OnClick);
            viewHolder.FavoriteChange += OnFavoriteClick;

            return viewHolder;
        }

		protected override List<Route> ApplyFilter()
		{
			if (string.IsNullOrWhiteSpace(Filter))
				return new List<Route>(AllItems);
			else
				return AllItems.Where(r => r.route_short_name.ContainsInsensitive(Filter) 
			                      || r.route_long_name.ContainsInsensitive(Filter))
					           .ToList();
		}

        protected override List<Route> ApplySort()
        {
            AlphanumComparator naturalComparator = new AlphanumComparator();

            return AllItems.OrderByDescending(r => FavoriteRouteAccessor.IsRouteFavorite(r))
                .ThenBy(r => r.route_short_name, naturalComparator)
                .ToList();
        }

        protected void OnFavoriteClick(object sender, ItemCheckChangedEventArgs e)
        {
            AnimateTo(ApplySort());
            ApplyFilter();
            ItemFavoriteClick?.Invoke(this, e);
        }

        public class RouteViewHolder : BaseViewHolder
		{
            public EventHandler<ItemCheckChangedEventArgs> FavoriteChange;
            TextView lblRouteShortName;
            TextView lblRouteLongName;
            CheckBox chkFavorite;
            Route currentItem;

            public RouteViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
			{
                lblRouteShortName = view.FindViewById<TextView>(Resource.Id.lbl_route_short_name);
                lblRouteLongName = view.FindViewById<TextView>(Resource.Id.lbl_route_long_name);
                chkFavorite = view.FindViewById<CheckBox>(Resource.Id.chk_favorite);

                chkFavorite.CheckedChange += ChkFavoriteCheckedChange;
            }

            public override void BindData(Route item, int position)
            {
                currentItem = item;

                lblRouteShortName.Text = item.route_short_name;
                chkFavorite.Checked = FavoriteRouteAccessor.IsRouteFavorite(item);

                if (!string.IsNullOrWhiteSpace(item.route_long_name))
                {
                    lblRouteLongName.Text = item.route_long_name;
                }
                else
                {
                    lblRouteLongName.Text = item.route_desc;
                }

                if (!string.IsNullOrWhiteSpace(item.route_color))
                {
                    lblRouteShortName.SetBackgroundColor(Color.ParseColor(ColorHelper.FormatColor(item.route_color)));
                    lblRouteShortName.SetTextColor(ColorHelper.ContrastColor(item.route_color));
                }
            }


            private async void ChkFavoriteCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (FavoriteRouteAccessor.IsRouteFavorite(currentItem) == e.IsChecked)
                    return;

                await FavoriteRouteAccessor.SetFavoriteForRoute(e.IsChecked, currentItem);

                FavoriteChange?.Invoke(this, new ItemCheckChangedEventArgs(AdapterPosition, e.IsChecked));
            }
        }
	}
}
