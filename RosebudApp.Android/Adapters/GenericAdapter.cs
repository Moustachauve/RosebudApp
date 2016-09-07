﻿
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

namespace RosebudAppAndroid.Adapters
{
    public abstract class GenericAdapter<T> : BaseAdapter<T>
    {
        protected Context context;
        protected LayoutInflater inflater;

        protected List<T> allItems;
        protected List<T> displayedItems;

        public override T this[int position] { get { return displayedItems[position]; } }
        public override int Count { get { return displayedItems.Count; } }

        string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                if (value.ToLower() == filter)
                    return;

                filter = value.ToLower();
                displayedItems = ApplyFilter();
                NotifyDataSetChanged();
            }
        }


        public GenericAdapter(Context context, List<T> items)
        {
            inflater = LayoutInflater.FromContext(context);
            allItems = items;
            displayedItems = allItems;
        }

        public override long GetItemId(int position) { return position; }


        public void ReplaceItems(List<T> routes)
        {
            this.allItems = routes;
            displayedItems = ApplyFilter();
            NotifyDataSetChanged();
        }

        protected abstract List<T> ApplyFilter();
    }
}

