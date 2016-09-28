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
using RosebudAppAndroid.Utils;
using RosebudAppAndroid.Cache;

namespace RosebudAppAndroid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class RosebudApp : Application
    {
        public RosebudApp(IntPtr handle, JniHandleOwnership transfer) : base(handle,transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Dependency.CacheRepository = new CacheRepository(ApplicationContext);
            Dependency.NetworkStatusMonitor = new NetworkStatusMonitor();
            Dependency.PreferenceManager = new PreferenceManager(Context);
            Dependency.LocationService = new LocationService(Context);
            Dependency.PathHelper = new PathHelper(ApplicationContext);

            ((LocationService)Dependency.LocationService).ConnectGoogleApi();
        }
    }
}