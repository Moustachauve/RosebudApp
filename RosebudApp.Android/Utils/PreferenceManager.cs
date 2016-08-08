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
    public class PreferenceManager : IPreferenceManager
    {
        public DateTime SelectedDatetime { get; set; } = DateTime.Today;
    }
}