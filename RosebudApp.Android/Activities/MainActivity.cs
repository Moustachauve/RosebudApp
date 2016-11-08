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
using System.Collections.Generic;

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "Rosebud", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
    public class MainActivity : AppCompatActivity
    {
        const string STATE_SELECTED_MENU_POSITION = "state-selected-menu-position";

        IMenuItem searchMenu;
        DrawerLayout drawerLayout;
        NavigationView navigationView;

        int selectedMenuPosition;

        public event EventHandler<string> SearchTextChanged;
        public event EventHandler RetryLastRequest;

        public bool SearchBarVisible
        {
            get { return searchMenu.IsVisible; }
            set
            {
                if (searchMenu == null)
                    return;

                searchMenu.SetVisible(value);
            }
        }

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
            NetworkStatusFragment networkFragment = (NetworkStatusFragment)FragmentManager.FindFragmentById(Resource.Id.network_fragment);

            if (savedInstanceState == null)
            {
                SelectDrawerItem(navigationView.Menu.GetItem(0));
            }
            else
            {
                selectedMenuPosition = savedInstanceState.GetInt(STATE_SELECTED_MENU_POSITION);
                SelectDrawerItem(navigationView.Menu.GetItem(selectedMenuPosition));
                //RestoreSelectedDrawerItem();
            }

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                SelectDrawerItem(e.MenuItem);
                drawerLayout.CloseDrawers();
            };


            networkFragment.RetryLastRequest += (object sender, EventArgs args) =>
           {
               RetryLastRequest?.Invoke(sender, args);
           };
        }

        private void SelectDrawerItem(IMenuItem menuItem)
        {
            Fragment fragment = null;

            switch (menuItem.ItemId)
            {
                case Resource.Id.menu_drawer_settings:
                    Intent intent = new Intent(this, typeof(SettingsActivity));
                    StartActivity(intent);
                    return;
                case Resource.Id.menu_drawer_favorite:
                    fragment = GetFragment(typeof(FavoriteFragment));
                    break;
                case Resource.Id.menu_drawer_agency:
                default:
                    fragment = GetFragment(typeof(FeedListFragment));
                    break;
            }

            SetCurrentFragment(fragment);
            menuItem.SetChecked(true);
            selectedMenuPosition = GetItemPositionInMenu(menuItem);
            Title = menuItem.TitleFormatted.ToString();
        }

        private int GetItemPositionInMenu(IMenuItem menuItem)
        {
            List<IMenuItem> items = new List<IMenuItem>();
            for (int i = 0; i < navigationView.Menu.Size(); i++)
            {
                items.Add(navigationView.Menu.GetItem(i));
            }

            return items.IndexOf(menuItem);
        }

        private Fragment GetFragment(Type fragmentType)
        {
            string fragmentTag = fragmentType.FullName;

            Fragment fragment = FragmentManager.FindFragmentByTag(fragmentTag);

            if (fragment == null)
            {
                fragment = (Fragment)Activator.CreateInstance(fragmentType);
            }

            return fragment;
        }

        private void SetCurrentFragment(Fragment fragment)
        {
            Type fragmentType = fragment.GetType();
            string fragmentTag = fragmentType.FullName;

            FragmentManager.BeginTransaction().Replace(Resource.Id.content, fragment, fragmentTag).Commit();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            searchMenu = menu.FindItem(Resource.Id.action_search);
            SearchBarVisible = false;

            var searchViewJava = MenuItemCompat.GetActionView(searchMenu);
            SearchViewCompat searchView = searchViewJava.JavaCast<SearchViewCompat>();

            searchView.QueryTextChange += (sender, args) =>
            {
                SearchTextChanged?.Invoke(this, args.NewText);
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

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(STATE_SELECTED_MENU_POSITION, selectedMenuPosition);
        }

        protected override void OnResume()
        {
            base.OnResume();
            navigationView.Menu.GetItem(selectedMenuPosition).SetChecked(true);
        }
    }
}


