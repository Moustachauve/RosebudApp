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
using MyTransitCore.Utils;
using MyTransitAndroid.Utils;
using MyTransitAndroid.Cache;

namespace MyTransitAndroid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MyTransitApplication : Application
    {
        public MyTransitApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle,transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Dependency.CacheRepository = new CacheRepository(ApplicationContext);
            Dependency.NetworkStatusMonitor = new NetworkStatusMonitor();
        }
    }
}