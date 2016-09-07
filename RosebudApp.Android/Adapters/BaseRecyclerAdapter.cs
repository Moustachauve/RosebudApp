
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
using System.Runtime.CompilerServices;

namespace RosebudAppAndroid
{
    public abstract class BaseRecyclerAdapter<TItem> : RecyclerView.Adapter
    {
        protected readonly Context Context;
        protected readonly LayoutInflater Inflater;
        protected List<TItem> AllItems { get; set; }

        public override int ItemCount { get { return AllItems.Count; } }

        public virtual TItem this[int i] { get { return AllItems[i]; } }

        public event EventHandler<int> ItemClick;

        public BaseRecyclerAdapter(Context context, List<TItem> items)
        {
            Context = context;
            Inflater = LayoutInflater.From(context);

            if (items == null)
            {
                AllItems = new List<TItem>();
            }
            else
            {
                AllItems = new List<TItem>(items);
            }
        }

        protected void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TItem currentItem = this[position];

            ((BaseViewHolder)holder).BindData(currentItem, position);
        }

        public virtual void AddItem(TItem item)
        {
            AllItems.Add(item);
            AnimateTo(AllItems);
        }

        public virtual void ClearItems()
        {
            int numberElements = ItemCount;
            AllItems = new List<TItem>();
            NotifyItemRangeRemoved(0, numberElements);
        }

        public virtual void ReplaceItems(List<TItem> items)
        {
            if (items == null)
            {
                AllItems = new List<TItem>();
            }
            else
            {
                AllItems = new List<TItem>(items);
            }
            AnimateTo(AllItems);
        }

        protected virtual TItem RemoveItem(int position)
        {
            TItem item = AllItems[position];
            NotifyItemRemoved(position);
            return item;
        }

        protected virtual void InsertItem(int position, TItem item)
        {
            AllItems.Insert(position, item);
            NotifyItemInserted(position);
        }

        public virtual void MoveItem(int fromPosition, int toPosition)
        {
            TItem item = AllItems[fromPosition];
            AllItems.Remove(item);
            AllItems.Insert(toPosition, item);
            NotifyItemMoved(fromPosition, toPosition);
        }

        protected void AnimateTo(List<TItem> items)
        {
            ApplyAndAnimateRemovals(items);
            ApplyAndAnimateAdditions(items);
            ApplyAndAnimateMovedItems(items);
        }

        protected virtual void ApplyAndAnimateRemovals(List<TItem> items)
        {
            for (int i = AllItems.Count - 1; i >= 0; i--)
            {
                TItem item = AllItems[i];
                if (!items.Contains(item))
                {
                    RemoveItem(i);
                }
            }
        }

        protected virtual void ApplyAndAnimateAdditions(List<TItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                TItem item = items[i];
                if (!AllItems.Contains(item))
                {
                    InsertItem(i, item);
                }
            }
        }

        protected virtual void ApplyAndAnimateMovedItems(List<TItem> items)
        {
            for (int toPosition = items.Count - 1; toPosition >= 0; toPosition--)
            {
                TItem item = items[toPosition];
                int fromPosition = AllItems.IndexOf(item);
                if (fromPosition >= 0 && fromPosition != toPosition)
                {
                    MoveItem(fromPosition, toPosition);
                }
            }
        }

        public abstract class BaseViewHolder : RecyclerView.ViewHolder
        {
            protected readonly View view;

            public BaseViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                view = itemView;
                if (listener != null)
                {
                    itemView.Click += (sender, e) => listener(base.AdapterPosition);
                }
            }

            public abstract void BindData(TItem item, int position);
        }

    }
}

