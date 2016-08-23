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
        private const string PREFERENCE_KEY_CELLULARE_DATA = "use_cellular_data";

        private Context Context { get; set; }

        public DateTime SelectedDatetime { get; set; } = DateTime.Today;

        public bool UseCellularData
        {
            get
            {
                ISharedPreferences preferences = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(Context);
                bool returnValue = preferences.GetBoolean(PREFERENCE_KEY_CELLULARE_DATA, true);
                return returnValue;
            }

            set
            {
                ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(Context);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutBoolean(PREFERENCE_KEY_CELLULARE_DATA, value);
                editor.Commit();
            }
        }

        public PreferenceManager(Context context)
        {
            Context = context;
        }
    }
}