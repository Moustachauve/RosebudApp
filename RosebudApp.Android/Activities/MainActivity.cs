using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using RosebudAppAndroid.Adapters;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Model;
using Newtonsoft.Json;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using RosebudAppAndroid.Fragments;
using System;
using Android.Support.V4.Content;
using Android.Support.Design.Widget;
using Android.Widget;
using Android.App;

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "Rosebud", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
    public class MainActivity : AppCompatActivity
    {
        //private FeedAdapter feedAdapter;
        //private RecyclerView feedRecyclerView;
        //private SwipeRefreshLayout feedPullToRefresh;
        //private SwipeRefreshLayout feedPullToRefreshEmpty;
        private IMenuItem searchMenu;
        DrawerLayout drawerLayout;
        NavigationView navigationView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_24dp);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.drawer_navigation_view);

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                SelectDrawerItem(e.MenuItem);
                drawerLayout.CloseDrawers();
            };
            SelectDrawerItem(navigationView.Menu.GetItem(0));
        }

        private void SelectDrawerItem(IMenuItem menuItem)
        {
            Fragment fragment = null;

            switch (menuItem.ItemId)
            {
                case Resource.Id.menu_drawer_agency:
                    fragment = new FeedListFragment();
                    break;
                case Resource.Id.menu_drawer_settings:
                    Intent intent = new Intent(this, typeof(SettingsActivity));
                    StartActivity(intent);
                    return;
                default:
                    fragment = new FeedListFragment();
                    break;
            }

            FragmentManager.BeginTransaction().Replace(Resource.Id.content, fragment).Commit();
            menuItem.SetChecked(true);
            Title = menuItem.TitleFormatted.ToString();

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            searchMenu = menu.FindItem(Resource.Id.action_search);
            //searchMenu.SetVisible(feedAdapter != null);

            var searchViewJava = MenuItemCompat.GetActionView(searchMenu);
            SearchViewCompat searchView = searchViewJava.JavaCast<SearchViewCompat>();

            searchView.QueryTextChange += (sender, args) =>
            {
                //feedAdapter.Filter = args.NewText;
            };

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(GravityCompat.Start);
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}


