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
using Android.Support.V4.View;
using Android.Util;

namespace RosebudAppAndroid.Views
{
    [Register("ca.cgagnier.rosebudapp.views.ClickableViewPager")]
    public class ClickableViewPager : ViewPager
    {

        public ClickableViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Clickable = false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return false;
        }

    }
}