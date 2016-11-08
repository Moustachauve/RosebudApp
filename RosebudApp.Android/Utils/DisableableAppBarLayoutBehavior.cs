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
using Android.Support.Design.Widget;
using Android.Util;

//Credit: http://stackoverflow.com/questions/30779123/need-to-disable-expand-on-collapsingtoolbarlayout-for-certain-fragments/
namespace RosebudAppAndroid.Utils
{
    [Register("ca.cgagnier.rosebudapp.utils.DisableableAppBarLayoutBehavior")]
    public class DisableableAppBarLayoutBehavior : AppBarLayout.Behavior
    {
        public bool Enabled { get; set; }

        public DisableableAppBarLayoutBehavior() : base()
        {
        }

        public DisableableAppBarLayoutBehavior(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public override bool OnStartNestedScroll(CoordinatorLayout parent, AppBarLayout child, View directTargetChild, View target, int nestedScrollAxes)
        {
            return Enabled && base.OnStartNestedScroll(parent, child, directTargetChild, target, nestedScrollAxes);
        }
    }
}