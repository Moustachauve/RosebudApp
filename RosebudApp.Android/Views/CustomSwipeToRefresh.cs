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
    [Register("ca.cgagnier.rosebudapp.views.CustomSwipeToRefresh")]
    public class CustomSwipeToRefresh : SwipeRefreshLayout
    {

        private int mTouchSlop;
        private float mPrevX;

        public CustomSwipeToRefresh(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mTouchSlop = ViewConfiguration.Get(context).ScaledTouchSlop;
            //this.OnInterceptTouchEvent
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (!Enabled)
                return false;

            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    mPrevX = MotionEvent.Obtain(ev).GetX();
                    break;

                case MotionEventActions.Move:
                    float eventX = ev.GetX();
                    float xDiff = Math.Abs(eventX - mPrevX);

                    if (xDiff > mTouchSlop)
                    {
                        return false;
                    }
                    break;
            }
            return base.OnInterceptTouchEvent(ev);
        }
    }
}