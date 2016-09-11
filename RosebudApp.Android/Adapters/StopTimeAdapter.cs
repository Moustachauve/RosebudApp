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

        public int GetNearestTimeSectionPosition(DateTime time)
        {
            HeaderTimeItem closestHeader = null;
            int smallestDifference = int.MaxValue;

            foreach (InternalItem item in AllItems)
            {
                HeaderTimeItem header = item as HeaderTimeItem;
                if(header == null)
                {
                    continue;
                }

                int headerHour = header.Hour;
                if(headerHour >= 24)
                {
                    headerHour -= 24;
                } 

                int difference = Math.Abs(header.Hour - time.Hour);
                if(difference < smallestDifference)
                {
                    smallestDifference = difference;
                    closestHeader = header;
                }
            }

            if(closestHeader != null)
            {
                return closestHeader.SectionFirstPosition;
            }
            else
            {
                return 0;
            }
        }

        protected override List<InternalItem> CreateInternalItems(List<StopTime> items)
        {
            List<InternalItem> internalItems = new List<InternalItem>();
            Dictionary<string, HeaderTimeItem> sections = new Dictionary<string, HeaderTimeItem>();

            int position = 0;

            foreach (var item in items)
            {
                string hour = item.departure_time.Split(':')[0];

                HeaderTimeItem section = null;

                if (!sections.ContainsKey(hour))
                {
                    section = new HeaderTimeItem();
                    section.Title = TimeFormatter.FormatHoursMinutes(hour + ":00:00");
                    section.Hour = int.Parse(hour);
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

        public class HeaderTimeItem : HeaderItem
        {
            public int Hour { get; set; }
        }
    }
}
