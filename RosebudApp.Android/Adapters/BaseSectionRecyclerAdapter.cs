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
using Com.Tonicartos.Superslim;

namespace RosebudAppAndroid.Adapters
{
    public abstract class BaseSectionRecyclerAdapter<TItem> : BaseRecyclerAdapter<BaseSectionRecyclerAdapter<TItem>.InternalItem>
    {
        protected const int VIEW_TYPE_HEADER = 0x01;
        protected const int VIEW_TYPE_CONTENT = 0x00;
        protected const int LINEAR = 0;

        private int headerDisplay = 18; //18 = Start aligned and sticky
        public int HeaderDisplay
        {
            get { return headerDisplay; }
            set
            {
                headerDisplay = value;
                NotifyHeaderChanges();
            }
        }

        private bool marginsFixed = true;
        public bool MarginsFixed
        {
            get { return marginsFixed; }
            set
            {
                marginsFixed = value;
                NotifyHeaderChanges();
            }
        }

        public BaseSectionRecyclerAdapter(Context context, List<TItem> items) : base(context, null)
        {
            ReplaceItems(CreateInternalItems(items));
        }

        protected abstract List<InternalItem> CreateInternalItems(List<TItem> items);
        protected abstract BaseViewHolder CreateHeaderView(ViewGroup parent);
        protected abstract BaseViewHolder CreateItemView(ViewGroup parent);

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == VIEW_TYPE_HEADER)
            {
                return CreateHeaderView(parent);
            }
            else
            {
                return CreateItemView(parent);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);

            InternalItem item = this[position];

            GridSLM.LayoutParams parameters = GridSLM.LayoutParams.From(holder.ItemView.LayoutParameters);

            if (item is HeaderItem)
            {
                //parameters.HeaderDisplay = headerDisplay;
                if (parameters.IsHeaderInline || (marginsFixed && !parameters.IsHeaderOverlay))
                {
                    parameters.Width = ViewGroup.LayoutParams.MatchParent;
                }
                else
                {
                    parameters.Width = ViewGroup.LayoutParams.WrapContent;
                }

                parameters.HeaderEndMarginIsAuto = !marginsFixed;
                parameters.HeaderStartMarginIsAuto = !marginsFixed;
            }
            else if(item is ObjectItem)
            {
                ObjectItem objectItem = item as ObjectItem;
                if(objectItem.IsLast)
                {
                    parameters.BottomMargin = 10 * (int)Context.Resources.DisplayMetrics.Density;
                }
            }

            parameters.SetSlm(item.SectionManager == LINEAR ? LinearSLM.Id : GridSLM.Id);
            parameters.ColumnWidth = Context.Resources.GetDimensionPixelSize(Resource.Dimension.grid_column_width);
            parameters.FirstPosition = item.SectionFirstPosition;
            holder.ItemView.LayoutParameters = parameters;
        }

        public override int GetItemViewType(int position)
        {
            return this[position] is HeaderItem ? VIEW_TYPE_HEADER : VIEW_TYPE_CONTENT;
        }

        public void ReplaceItems(List<TItem> items)
        {
            List<InternalItem> internalItems = CreateInternalItems(items);
            base.ReplaceItems(internalItems);
        }

        private void NotifyHeaderChanges()
        {
            for (int i = 0; i < AllItems.Count; i++)
            {
                if (AllItems[i] is HeaderItem)
                {
                    NotifyItemChanged(i);
                }
            }
        }

        public abstract class InternalItem
        {
            public int SectionManager { get; set; }
            public int SectionFirstPosition { get; set; }
        }

        public class HeaderItem : InternalItem
        {
            public string Title { get; set; }
        }

        public class ObjectItem : InternalItem
        {
            public TItem Item { get; set; }
            public bool IsLast { get; set; }
        }
    }
}