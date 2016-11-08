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
using Android.Support.V4.Widget;
using Android.Util;

namespace RosebudAppAndroid.Views
{
    [Register("ca.cgagnier.rosebudapp.views.LockableNestedScrollView")]
    public class LockableNestedScrollView : NestedScrollView
    {
        public bool Locked { get; set; }

        public LockableNestedScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (Locked)
                return false;
            else
                return base.OnInterceptTouchEvent(ev);
        }
    }
}