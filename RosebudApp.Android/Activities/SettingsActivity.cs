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
using Android.Support.V7.App;

namespace RosebudAppAndroid.Activities
{
	[Activity(Label = "SettingsActivity", ParentActivity = typeof(MainActivity))]
	public class SettingsActivity : AppCompatActivity, View.IOnClickListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.settings);
			Title = GetString(Resource.String.activity_settings);

			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.my_awesome_toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
		}

        public void OnClick(View v)
        {
            //Back Button in nav bar
            OnBackPressed();
        }
    }
}