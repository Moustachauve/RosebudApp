using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using RosebudAppCore.Model;
using Android.Support.V7.Widget;
using RosebudAppCore;
using Com.Tonicartos.Superslim;

namespace RosebudAppAndroid.Adapters
{
    public class StopTimeAdapter : BaseSectionRecyclerAdapter<StopTime>
    {
        public StopTimeAdapter(Context context, List<StopTime> stopTimes) : base(context, stopTimes)
        {
            MarginsFixed = false;
        }

        protected override List<InternalItem> CreateInternalItems(List<StopTime> items)
        {
            List<InternalItem> internalItems = new List<InternalItem>();
            Dictionary<string, HeaderItem> sections = new Dictionary<string, HeaderItem>();

            int position = 0;

            foreach (var item in items)
            {
                string hour = item.departure_time.Split(':')[0];

                HeaderItem section = null;

                if (!sections.ContainsKey(hour))
                {
                    section = new HeaderItem();
                    section.Title = hour + ":00";
                    section.SectionFirstPosition = position;
                    sections.Add(hour, section);
                    internalItems.Add(section);
                    position++;
                }
                else
                {
                    section = sections[hour];
                }

                ObjectItem newItem = new ObjectItem();
                newItem.Item = item;
                newItem.SectionFirstPosition = section.SectionFirstPosition;
                internalItems.Add(newItem);
                position++;
            }

            foreach (var section in sections.Values)
            {
                if(section.SectionFirstPosition - 1 >= 0)
                {
                    ObjectItem item = internalItems[section.SectionFirstPosition - 1] as ObjectItem;
                    item.IsLast = true;
                }
            }

            return internalItems;
        }

        protected override BaseViewHolder CreateHeaderView(ViewGroup parent)
        {
            View view = Inflater.Inflate(Resource.Layout.header_listitem, parent, false);
            return new HeaderViewHolder(view);
        }

        protected override BaseViewHolder CreateItemView(ViewGroup parent)
        {
            View view = Inflater.Inflate(Resource.Layout.stop_time_listitem, parent, false);
            return new StopTimeViewHolder(view, OnClick);
        }

        public class StopTimeViewHolder : BaseViewHolder
        {

            public StopTimeViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
            {
            }

            public override void BindData(InternalItem item, int position)
            {
                StopTime stopTime = ((ObjectItem)item).Item;
                TextView lblDepartureTime = view.FindViewById<TextView>(Resource.Id.lbl_departure_time);

                lblDepartureTime.Text = TimeFormatter.FormatHoursMinutes(stopTime.departure_time);
            }
        }

        public class HeaderViewHolder : BaseViewHolder
        {

            public HeaderViewHolder(View itemView) : base(itemView, null)
            {
            }

            public override void BindData(InternalItem item, int position)
            {
                string title = ((HeaderItem)item).Title;
                TextView lblHeaderTitle = view.FindViewById<TextView>(Resource.Id.lbl_header_title);

                lblHeaderTitle.Text = title;
            }
        }
    }
}
