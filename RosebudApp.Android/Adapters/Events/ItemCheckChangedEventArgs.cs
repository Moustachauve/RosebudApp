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

namespace RosebudAppAndroid.Adapters.Events
{
    public class ItemCheckChangedEventArgs : EventArgs
    {
        public int Position { get; private set; }
        public bool IsChecked { get; private set; }

        public ItemCheckChangedEventArgs(int position, bool isChecked)
        {
            Position = position;
            IsChecked = isChecked;
        }
    }
}