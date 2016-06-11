using Android.App;
using Android.Widget;
using Android.OS;
using MyTransit.Core;
using Android.Views;
using Android.Content;
using Newtonsoft.Json;

namespace MyTransit.Android
{
	[Activity(Label = "MyTransit", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		private FeedAdapter feedAdapter;
		private ListView feedListView;
		private ProgressBar feedProgressBar;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			feedListView = FindViewById<ListView>(Resource.Id.feed_listview);
			feedProgressBar = FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);

			LoadFeeds();

			feedListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
			{
				Feed clickedFeed = feedAdapter[args.Position];
				Intent detailsIntent = new Intent(this, typeof(FeedDetailsActivity));
				detailsIntent.PutExtra("feedInfos", JsonConvert.SerializeObject(clickedFeed));
				StartActivity(detailsIntent);
			};
		}

		private async void LoadFeeds()
		{
			feedProgressBar.Visibility = ViewStates.Visible;
			var feeds = await FeedAccessor.GetAllFeeds();

			if (feedAdapter == null)
			{
				feedAdapter = new FeedAdapter(this, feeds);
				feedListView.Adapter = new FeedAdapter(this, feeds);
			}
			else
				feedAdapter.ReplaceData(feeds);
			
			feedProgressBar.Visibility = ViewStates.Gone;
		}
	}
}


