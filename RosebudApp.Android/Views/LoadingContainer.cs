using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Support.V4.Widget;

namespace RosebudAppAndroid.Views
{
    [Register("ca.cgagnier.rosebudapp.views.LoadingContainer")]
    public class LoadingContainer : FrameLayout
    {
        SwipeRefreshLayout refreshLayout;
        LockableNestedScrollView refreshScrollView;
        ViewGroup refreshLayoutContent;
        ProgressBar loadingIndicator;
        private bool loading;

        public event EventHandler Refresh;

        public bool Refreshable
        {
            get { return refreshLayout.Enabled; }
            set { refreshLayout.Enabled = value; }
        }

        public bool Refreshing
        {
            get { return refreshLayout.Refreshing; }
            set { refreshLayout.Refreshing = value; }
        }

        public bool Loading
        {
            get { return loading; }
            set
            {
                if (value)
                {
                    loadingIndicator.Visibility = ViewStates.Visible;
                    refreshLayoutContent.Visibility = ViewStates.Gone;
                }
                else
                {
                    loadingIndicator.Visibility = ViewStates.Gone;
                    refreshLayoutContent.Visibility = ViewStates.Visible;
                }

                loading = value;
            }
        }

        public bool ScrollLocked
        {
            get { return refreshScrollView.Locked; }
            set { refreshScrollView.Locked = value; }
        }

        public LoadingContainer(Context context, IAttributeSet attributes) : base(context, attributes)
        {
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);

            LayoutInflater.From(context).Inflate(Resource.Layout.loading_container, this);

            refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.pull_to_refresh);
            refreshScrollView = FindViewById<LockableNestedScrollView>(Resource.Id.pull_to_refresh_scrollview);
            refreshLayoutContent = FindViewById<ViewGroup>(Resource.Id.pull_to_refresh_content);
            loadingIndicator = FindViewById<ProgressBar>(Resource.Id.loading_indicator);

            refreshLayout.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
            refreshLayout.Refresh += OnRefreshRequested;
        }

        private void OnRefreshRequested(object sender, EventArgs e)
        {
            Refresh?.Invoke(this, e);
        }

        public override void AddView(View child, int index, ViewGroup.LayoutParams @params)
        {
            if (refreshLayout == null)
            {
                base.AddView(child, index, @params);
            }
            else
            {
                refreshLayoutContent.AddView(child, index, @params);
            }
        }
    }
}