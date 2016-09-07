using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace RosebudAppAndroid.Adapters
{
    public abstract class SearchableRecyclerAdapter<TItem> : BaseRecyclerAdapter<TItem>
    {
        protected List<TItem> DisplayedItems { get; set; }

        string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                if (value.ToLower() == filter)
                    return;

                filter = value.ToLower();
                AnimateTo(ApplyFilter());
            }
        }

        public override int ItemCount { get { return DisplayedItems.Count; } }

        public override TItem this[int i] { get { return DisplayedItems[i]; } }

        public SearchableRecyclerAdapter(Context context, List<TItem> items) : base(context, items)
        {
            DisplayedItems = new List<TItem>(AllItems);
        }

        override public void AddItem(TItem item)
        {
            AllItems.Add(item);
            AnimateTo(ApplyFilter());
        }

        override public void ClearItems()
        {
            int numberElements = ItemCount;
            AllItems = new List<TItem>();
            DisplayedItems = new List<TItem>();
            NotifyItemRangeRemoved(0, numberElements);
        }

        override protected TItem RemoveItem(int position)
        {
            TItem item = DisplayedItems[position];
            DisplayedItems.Remove(item);
            NotifyItemRemoved(position);
            return item;
        }

        override protected void InsertItem(int position, TItem item)
        {
            DisplayedItems.Insert(position, item);
            NotifyItemInserted(position);
        }

        override public void MoveItem(int fromPosition, int toPosition)
        {
            TItem item = DisplayedItems[fromPosition];
            DisplayedItems.Remove(item);
            DisplayedItems.Insert(toPosition, item);
            NotifyItemMoved(fromPosition, toPosition);
        }

        override protected void ApplyAndAnimateRemovals(List<TItem> items)
        {
            for (int i = DisplayedItems.Count - 1; i >= 0; i--)
            {
                TItem item = DisplayedItems[i];
                if (!items.Contains(item))
                {
                    RemoveItem(i);
                }
            }
        }

        override protected void ApplyAndAnimateAdditions(List<TItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                TItem item = items[i];
                if (!DisplayedItems.Contains(item))
                {
                    InsertItem(i, item);
                }
            }
        }

        override protected void ApplyAndAnimateMovedItems(List<TItem> items)
        {
            for (int toPosition = items.Count - 1; toPosition >= 0; toPosition--)
            {
                TItem item = items[toPosition];
                int fromPosition = DisplayedItems.IndexOf(item);
                if (fromPosition >= 0 && fromPosition != toPosition)
                {
                    MoveItem(fromPosition, toPosition);
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            throw new NotImplementedException();
        }

        protected abstract List<TItem> ApplyFilter();

    }
}