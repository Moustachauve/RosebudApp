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
using Android.Support.Design.Widget;
using RosebudApp.AndroidMaterialCalendarBinding;
using Android.Support.V7.App;
using RosebudAppCore.Utils;
using System.Threading.Tasks;
using RosebudAppCore;
using Android.Views.Animations;

namespace RosebudAppAndroid.Activities
{
    public abstract class CalendarToolBarActivity : AppCompatActivity
    {
        protected int ActivityLayout;
        protected AppBarLayout appBarLayout;
        protected TextView lblToolbarTitle;
        protected TextView lblToolbarDate;
        protected MaterialCalendarView calendarView;
        protected ImageView icoDropdownDatePicker;

        protected bool isCalendarExpanded;
        bool isArrowPointingUp;
        int previousToolbarOffset = int.MinValue;
        
        float currentCalendarArrowRotation = 360f;

        public EventHandler<DateTime> SelectedDateChange;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(ActivityLayout);

            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.app_bar_layout);
            lblToolbarTitle = FindViewById<TextView>(Resource.Id.lbl_toolbar_title);
            lblToolbarDate = FindViewById<TextView>(Resource.Id.lbl_toolbar_date);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.my_awesome_toolbar);
            SetSupportActionBar(toolbar);

            var btnDatePicker = FindViewById<RelativeLayout>(Resource.Id.btn_date_picker);
            icoDropdownDatePicker = FindViewById<ImageView>(Resource.Id.ico_dropdown_calendar);
            calendarView = FindViewById<MaterialCalendarView>(Resource.Id.calendar_view);
            SetCalendarSelectedDate(Dependency.PreferenceManager.SelectedDatetime);

            appBarLayout.OffsetChanged += AppBarLayoutOffsetChanged;
            toolbar.NavigationClick += ToolbarNavigationClick;
            btnDatePicker.Click += BtnDatePickerClick;
            calendarView.DateChanged += CalendarViewDateChanged;
            SelectedDateChange += OnSelectedDateChanged;
        }

        protected override void OnResume()
        {
            base.OnResume();

            int currentYear = calendarView.SelectedDate.Calendar.Get(Java.Util.CalendarField.Year);
            int currentMonth = calendarView.SelectedDate.Calendar.Get(Java.Util.CalendarField.Month);
            int currentDay = calendarView.SelectedDate.Calendar.Get(Java.Util.CalendarField.DayOfMonth);

            DateTime currentDate = new DateTime(currentYear, currentMonth + 1, currentDay);

            if (currentDate != Dependency.PreferenceManager.SelectedDatetime)
            {
                SwitchDate(Dependency.PreferenceManager.SelectedDatetime);
            }
        }

        protected void SwitchDate(int year, int month, int day)
        {
            SwitchDate(new DateTime(year, month, day));
        }

        protected void SwitchDate(DateTime date)
        {
            Dependency.PreferenceManager.SelectedDatetime = date;
            lblToolbarDate.Text = TimeFormatter.ToFullShortDate(date);

            SelectedDateChange?.Invoke(this, date);
        }

        protected void SetCalendarSelectedDate(DateTime selectedDate)
        {
            CalendarDay calendarDay = new CalendarDay(selectedDate.Year, selectedDate.Month - 1, selectedDate.Day);
            calendarView.SetDateSelected(calendarDay, true);
        }

        private void CalendarViewDateChanged(object sender, DateSelectedEventArgs e)
        {
            ToggleDatePicker();
            CalendarDay calendarDay = e.P1;

            int year = calendarDay.Calendar.Get(Java.Util.CalendarField.Year);
            int month = calendarDay.Calendar.Get(Java.Util.CalendarField.Month);
            int day = calendarDay.Calendar.Get(Java.Util.CalendarField.DayOfMonth);

            SwitchDate(year, month + 1, day);
        }

        private void RotateArrow()
        {
            RotateAnimation animation = ArrowRotateAnimation.GetAnimation(currentCalendarArrowRotation, 180);
            icoDropdownDatePicker.StartAnimation(animation);

            currentCalendarArrowRotation = (currentCalendarArrowRotation + 180f) % 360f;
        }

        private void ToggleDatePicker()
        {
            isCalendarExpanded = !isCalendarExpanded;
            appBarLayout.SetExpanded(isCalendarExpanded);
        }

        private void AppBarLayoutOffsetChanged(object sender, AppBarLayout.OffsetChangedEventArgs e)
        {
            if (e.VerticalOffset == previousToolbarOffset)
                return;

            if (Math.Abs(e.VerticalOffset) == appBarLayout.TotalScrollRange)
            {
                //Collapsed
                if(isCalendarExpanded)
                {
                    if(isArrowPointingUp)
                    {
                        RotateArrow();
                        isArrowPointingUp = false;
                    }
                    
                    isCalendarExpanded = false;
                }
            }
            else if(e.VerticalOffset == 0)
            {
                //Expanded
                if (!isCalendarExpanded)
                {
                    if (!isArrowPointingUp)
                    {
                        RotateArrow();
                        isArrowPointingUp = true;
                    }

                    isCalendarExpanded = true;
                }
            }
            else
            {
                //Between
                if(isCalendarExpanded && !isArrowPointingUp)
                {
                    RotateArrow();
                    isArrowPointingUp = true;
                }
                else if(!isCalendarExpanded && isArrowPointingUp)
                {
                    RotateArrow();
                    isArrowPointingUp = false;
                }
            }

            previousToolbarOffset = e.VerticalOffset;
        }

        private void BtnDatePickerClick(object sender, EventArgs e)
        {
            ToggleDatePicker();
        }

        private void ToolbarNavigationClick(object sender, Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs e)
        {
            OnBackPressed();
        }

        protected abstract void OnSelectedDateChanged(object sender, DateTime selectedDate);
    }
}