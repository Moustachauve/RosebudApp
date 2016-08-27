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
using RosebudAppCore.Utils;
using Android.Support.V7.App;

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "StopTimesActivity")]
    public class StopTimesActivity : AppCompatActivity, ILocationServiceListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();
            Dependency.LocationService.AddOnLocationChangedListener(this);
        }

        protected override void OnPause()
        {
            Dependency.LocationService.RemoveOnLocationChangedListener(this);
            base.OnPause();
        }

        public void OnLocationChanged(RosebudAppCore.Model.Location location)
        {
            Toast.MakeText(this, "Testify", ToastLength.Short).Show();
            //throw new NotImplementedException();
        }
    }
}