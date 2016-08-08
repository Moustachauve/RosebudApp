
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
		protected List<TItem> DisplayedItems { get; set; }

		private string filter;
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

		public TItem this[int i] { get { return DisplayedItems[i]; } }

		public event EventHandler<int> ItemClick;

		public BaseRecyclerAdapter(Context context, List<TItem> items) {
			Context = context;
			Inflater = LayoutInflater.From(context);

            if(items == null)
            {
                AllItems = new List<TItem>();
            }
            else
            {
                AllItems = new List<TItem>(items);
            }
			
			DisplayedItems = new List<TItem>(AllItems);
		}

		protected void OnClick(int position)
		{
            ItemClick?.Invoke(this, position);
        }

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			TItem currentItem = this[position];

			((BaseViewHolder)holder).BindData(currentItem);
		}

		protected abstract List<TItem> ApplyFilter();

		public void AddItem(TItem item) {
			AllItems.Add(item);
			AnimateTo(ApplyFilter());
		}

		public void ClearItems()
		{
			int numberElements = ItemCount;
			AllItems = new List<TItem>();
			DisplayedItems = new List<TItem>();
			NotifyItemRangeRemoved(0, numberElements);
		}

		public void ReplaceItems(List<TItem> items)
		{
            if(items == null)
            {
                AllItems = new List<TItem>();
            }
            else
            {
                AllItems = new List<TItem>(items);
            }
            AnimateTo(ApplyFilter());
		}

		protected TItem RemoveItem(int position)
		{
			TItem item = DisplayedItems[position];
			DisplayedItems.Remove(item);
			NotifyItemRemoved(position);
			return item;
		}

		protected void InsertItem(int position, TItem item)
		{
			DisplayedItems.Insert(position, item);
			NotifyItemInserted(position);
		}

		public void MoveItem(int fromPosition, int toPosition)
		{
			TItem item = DisplayedItems[fromPosition];
			DisplayedItems.Remove(item);
			DisplayedItems.Insert(toPosition, item);
			NotifyItemMoved(fromPosition, toPosition);
		}

		protected void AnimateTo(List<TItem> items)
		{
			ApplyAndAnimateRemovals(items);
			ApplyAndAnimateAdditions(items);
			ApplyAndAnimateMovedItems(items);
		}

		private void ApplyAndAnimateRemovals(List<TItem> items)
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

		private void ApplyAndAnimateAdditions(List<TItem> items)
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

		private void ApplyAndAnimateMovedItems(List<TItem> items)
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

		public abstract class BaseViewHolder : RecyclerView.ViewHolder
		{
			protected readonly View view;

			public BaseViewHolder(View itemView, Action<int> listener) : base(itemView)
			{
				view = itemView;
				itemView.Click += (sender, e) => listener(base.AdapterPosition);
			}

			public abstract void BindData(TItem item);
		}

	}
}

