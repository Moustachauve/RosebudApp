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
using Android.Net;
using RosebudAppCore.Utils;
using RosebudAppAndroid.Fragments;

namespace RosebudAppAndroid.Activities
{
    public abstract class AbstractHttpActivity : AppCompatActivity
    {
		//not in use
        protected ConnectivityManager ConnectivityManager;
        protected NetworkStatusFragment networkStatusFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //TODO: Retry reload if request failed and internet become available after that
            //ConnectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }
    }
}