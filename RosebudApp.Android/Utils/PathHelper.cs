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

namespace RosebudAppAndroid.Utils
{
    public class PathHelper : IPathHelper
    {
        Context Context { get; set; }

        public string TempFolderPath
        {
            get
            {
                if (Context.ExternalCacheDir == null)
                    return null;

                return Context.ExternalCacheDir.AbsolutePath;
            }
        }

        public string PermanentFolderPath
        {
            get
            {
                if (Context.FilesDir == null)
                    return null;

                return Context.FilesDir.AbsolutePath;
            }
        }


        public PathHelper(Context context)
        {
            Context = context;
        }
    }
}