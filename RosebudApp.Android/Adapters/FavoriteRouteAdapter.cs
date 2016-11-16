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
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.App;
using Android.Support.V7.App;
using Android.Support.Design.Widget;

namespace RosebudAppAndroid.Adapters
{
    public class FavoriteRouteAdapter : SearchableRecyclerAdapter<FavoriteRoute>
    {
        private bool isLocationVisible = true;
        public bool IsLocationVisible
        {
            get { return isLocationVisible; }
            set
            {
                if (value == isLocationVisible)
                    return;

                isLocationVisible = value;
                NotifyDataSetChanged();
            }
        }

        protected int NextViewPagerId;

        Android.Support.V4.App.FragmentManager FragmentManager;

        public FavoriteRouteAdapter(Context context, List<FavoriteRoute> routesWithStopLocation, Android.Support.V4.App.FragmentManager fragmentManager) : base(context, routesWithStopLocation)
        {
            FragmentManager = fragmentManager;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Inflater.Inflate(Resource.Layout.route_stop_location_listitem, parent, false);

            FavoriteRouteViewHolder viewHolder = new FavoriteRouteViewHolder(view, OnClick, this);

            return viewHolder;
        }

        protected override List<FavoriteRoute> ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(Filter))
                return new List<FavoriteRoute>(AllItems);
            else
                return AllItems.Where(r => r.Route.route_short_name.ContainsInsensitive(Filter)
                                      || r.Route.route_long_name.ContainsInsensitive(Filter))
                                      .ToList();
        }

        protected override List<FavoriteRoute> ApplySort()
        {
            AlphanumComparator naturalComparator = new AlphanumComparator();
            DistanceComparator distanceComparator = new DistanceComparator();
            TimeComparator timeComparator = new TimeComparator();

            return AllItems.OrderBy(r => r.Directions, distanceComparator)
                           .ThenBy(r => r.Directions, timeComparator)
                           .ThenBy(r => r.Route.route_short_name, naturalComparator)
                           .ToList();
        }

        public void UpdateItem(FavoriteRoute route)
        {
            for (int i = 0; i < AllItems.Count; i++)
            {
                if (AllItems[i].Route.route_id != route.Route.route_id)
                    continue;

                AllItems[i].Route = route.Route;
                AllItems[i].Directions = route.Directions;
                
                NotifyItemChanged(i);
                
                AllItems = ApplySort();
                AnimateTo(AllItems);

                break;
            }
        }

        public class FavoriteRouteViewHolder : BaseViewHolder
        {
            TextView lblRouteShortName;
            ViewPager viewPager;
            FavoriteRouteDirectionPagerAdapter pagerAdapter;
            TabLayout tabLayout;
            Android.Support.V4.App.FragmentManager fragmentManager;

            /* TextView lblRouteTimeNextBus;
             TextView lblRouteClosestStop;
             TextView lblRouteClosestStopDistance;
             TextView lblRouteNextDeparture;
             TextView lblRouteNextDepartureTimeUnit;
             ProgressBar progressTimeRoute;*/

            FavoriteRoute currentItem;

            FavoriteRouteAdapter Adapter;

            public FavoriteRouteViewHolder(View itemView, Action<int> listener, FavoriteRouteAdapter adapter) : base(itemView, listener)
            {
                Adapter = adapter;
                fragmentManager = adapter.FragmentManager;

                lblRouteShortName = view.FindViewById<TextView>(Resource.Id.lbl_route_short_name);
                viewPager = view.FindViewById<ViewPager>(Resource.Id.direction_view_pager);
                tabLayout = view.FindViewById<TabLayout>(Resource.Id.direction_tab_layout);

                viewPager.Id = adapter.NextViewPagerId;
                adapter.NextViewPagerId++;

                /*progressTimeRoute = view.FindViewById<ProgressBar>(Resource.Id.progress_time_route);
                lblRouteTimeNextBus = view.FindViewById<TextView>(Resource.Id.lbl_route_time_next_bus);
                lblRouteClosestStop = view.FindViewById<TextView>(Resource.Id.lbl_route_closest_stop);
                lblRouteClosestStopDistance = view.FindViewById<TextView>(Resource.Id.lbl_route_closest_stop_distance);
                lblRouteNextDeparture = view.FindViewById<TextView>(Resource.Id.lbl_route_next_departure);
                lblRouteNextDepartureTimeUnit = view.FindViewById<TextView>(Resource.Id.lbl_route_next_departure_time_unit);*/
            }

            public override void BindData(FavoriteRoute item, int position)
            {
                currentItem = item;
                lblRouteShortName.Text = item.Route.route_short_name;

                SetViewPager();

                SetColor();

                //SetRouteLongName();
                //
                //SetRouteLocationInfo();
            }

            private void SetViewPager()
            {
                if (pagerAdapter == null)
                {
                    pagerAdapter = new FavoriteRouteDirectionPagerAdapter(fragmentManager, currentItem);
                    viewPager.Adapter = pagerAdapter;
                    tabLayout.SetupWithViewPager(viewPager);
                }
                else
                {
                    pagerAdapter.NotifyDataSetChanged();
                    pagerAdapter.ReplaceItem(currentItem);
                }
            }

            private void SetColor()
            {
                Color routeColor;
                if (!string.IsNullOrWhiteSpace(currentItem.Route.route_color))
                {
                    routeColor = Color.ParseColor(ColorHelper.FormatColor(currentItem.Route.route_color));
                }
                else
                {
                    routeColor = new Color(ContextCompat.GetColor(Adapter.Context, Resource.Color.default_item_color));
                }

                lblRouteShortName.SetBackgroundColor(routeColor);
                lblRouteShortName.SetTextColor(ColorHelper.ContrastColor(routeColor));

            }

            /*
            private void SetRouteLongName()
            {
                if (!string.IsNullOrWhiteSpace(currentItem.Route.route_long_name))
                {
                    lblRouteLongName.Text = currentItem.Route.route_long_name;
                }
                else
                {
                    lblRouteLongName.Text = currentItem.Route.route_desc;
                }
            }

            private void SetColor()
            {
                Color routeColor;
                if (!string.IsNullOrWhiteSpace(currentItem.Route.route_color))
                {
                    routeColor = Color.ParseColor(ColorHelper.FormatColor(currentItem.Route.route_color));
                }
                else
                {
                    routeColor = new Color(ContextCompat.GetColor(Adapter.Context, Resource.Color.default_item_color));
                }

                lblRouteShortName.SetBackgroundColor(routeColor);
                lblRouteShortName.SetTextColor(ColorHelper.ContrastColor(routeColor));

            }

            private void SetRouteLocationInfo()
            {
                if (!Adapter.IsLocationVisible)
                {
                    HideLocation();
                    return;
                }

                lblRouteNextDeparture.Visibility = ViewStates.Visible;

                if (currentItem.Directions == null)
                {
                    if (Dependency.NetworkStatusMonitor.State == NetworkState.Disconnected)
                    {
                        HideLocation();
                    }
                    else
                    {
                        progressTimeRoute.Visibility = ViewStates.Visible;
                        lblRouteClosestStop.Visibility = ViewStates.Gone;
                        lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                        lblRouteTimeNextBus.Visibility = ViewStates.Gone;
                        lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    if (currentItem.Directions.Stop == null)
                    {
                        lblRouteClosestStop.Text = Adapter.Context.Resources.GetString(Resource.String.route_next_departure_no_service);
                        lblRouteTimeNextBus.Text = "--";
                        lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                        lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lblRouteClosestStop.Text = currentItem.Directions.Stop.stop_name;
                        lblRouteClosestStopDistance.Text = RouteLocationHelper.FormatDistance(currentItem.Directions.DistanceInMeter);

                        SetRemainingTime();

                        lblRouteClosestStopDistance.Visibility = ViewStates.Visible;
                    }

                    progressTimeRoute.Visibility = ViewStates.Gone;
                    lblRouteClosestStop.Visibility = ViewStates.Visible;
                    lblRouteTimeNextBus.Visibility = ViewStates.Visible;
                    lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Visible;
                }
            }

            void HideLocation()
            {
                progressTimeRoute.Visibility = ViewStates.Gone;
                lblRouteNextDeparture.Visibility = ViewStates.Gone;
                lblRouteClosestStop.Visibility = ViewStates.Gone;
                lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                lblRouteTimeNextBus.Visibility = ViewStates.Gone;
                lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
            }

            void SetRemainingTime()
            {
                if (currentItem.Directions.NextDepartureTime == DateTime.MinValue)
                {
                    lblRouteTimeNextBus.Text = "--";
                    lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
                    return;
                }

                TimeSpan difference = currentItem.Directions.NextDepartureTime - DateTime.Now;

                int timeDisplayed = 0;

                if (difference.TotalMinutes < 120)
                {
                    timeDisplayed = (int)Math.Round(difference.TotalMinutes);

                    if (timeDisplayed > 1)
                        lblRouteNextDepartureTimeUnit.Text = Adapter.Context.Resources.GetString(Resource.String.minutes);
                    else
                        lblRouteNextDepartureTimeUnit.Text = Adapter.Context.Resources.GetString(Resource.String.minute);
                }
                else
                {
                    timeDisplayed = (int)Math.Round(difference.TotalHours);

                    if (timeDisplayed > 1)
                        lblRouteNextDepartureTimeUnit.Text = Adapter.Context.Resources.GetString(Resource.String.hours);
                    else
                        lblRouteNextDepartureTimeUnit.Text = Adapter.Context.Resources.GetString(Resource.String.hour);
                }

                lblRouteTimeNextBus.Text = timeDisplayed.ToString();
            }*/
        }
    }
}
