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
using Android.Util;

namespace RosebudAppAndroid.Views
{
    public class ClickableLinearLayout : LinearLayout
    {
        public ClickableLinearLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return false;
        }

    }
}