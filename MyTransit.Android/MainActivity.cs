using Android.App;
using Android.OS;
using MyTransit.Core;
using Android.Views;
using Android.Content;
using Newtonsoft.Json;
using Android.Support.V7.App;
using Android.Widget;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using System;
using Android.Support.V4.View;
using Android.Runtime;

namespace MyTransit.Android
{
    [Activity(Label = "MyTransit", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : AppCompatActivity
    {
        private FeedAdapter feedAdapter;
        private ListView feedListView;
        private ProgressBar feedProgressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
            SetSupportActionBar(toolbar);

            feedListView = FindViewById<ListView>(Resource.Id.feed_listview);
            feedProgressBar = FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);

            feedListView.TextFilterEnabled = true;

            LoadFeeds();

            feedListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
                    {
                        Feed clickedFeed = feedAdapter[args.Position];
                        Intent detailsIntent = new Intent(this, typeof(FeedDetailsActivity));
                        detailsIntent.PutExtra("feedInfos", JsonConvert.SerializeObject(clickedFeed));

                        StartActivity(detailsIntent);
                    };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            var searchItem = menu.FindItem(Resource.Id.action_search);
            var test = MenuItemCompat.GetActionView(searchItem);
            SearchViewCompat searchView = test.JavaCast<SearchViewCompat>();

            searchView.QueryTextSubmit += (sender, args) =>
            {
                feedAdapter.Filter = args.Query;
            };

            return true;
        }

        private async void LoadFeeds()
        {
            feedProgressBar.Visibility = ViewStates.Visible;
            var feeds = await FeedAccessor.GetAllFeeds();

            if (feedAdapter == null)
            {
                feedAdapter = new FeedAdapter(this, feeds);
                feedListView.Adapter = feedAdapter;
            }
            else
                feedAdapter.ReplaceData(feeds);

            feedProgressBar.Visibility = ViewStates.Gone;
        }
    }
}


