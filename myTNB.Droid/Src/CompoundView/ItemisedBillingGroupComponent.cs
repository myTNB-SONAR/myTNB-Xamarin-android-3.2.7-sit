using System;
using Android.Content;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;

namespace myTNB_Android.Src.CompoundView
{
    public class ItemisedBillingGroupComponent : RelativeLayout
    {
        [BindView(Resource.Id.itemisedBillingGroupContent)]
        LinearLayout itemisedBillingGroupContent;

        public ItemisedBillingGroupComponent(Context context) : base(context)
        {
            Init(context);
        }

        public ItemisedBillingGroupComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context);
        }

        public ItemisedBillingGroupComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context);
        }

        public ItemisedBillingGroupComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(context);
        }

        public void Init(Context context)
        {
            Inflate(context,Resource.Layout.ItemisedBillingHistoryGroupingLayout,this);
        }

        public void AddContent(LinearLayout contentLayout)
        {
            LinearLayout linearLayout =  FindViewById<LinearLayout>(Resource.Id.itemisedBillingGroupContent);
            linearLayout.AddView(contentLayout);
        }

        public void SetBackground()
        {
            SetBackgroundResource(Resource.Color.white);
        }

        public void SetMonthYearLabel(string monthYear)
        {
            TextView monthYearLabel = FindViewById<TextView>(Resource.Id.itemisedBillingGroupLabel);
            monthYearLabel.Text = monthYear;
        }

        public void ShowSeparator(bool isShown)
        {
            View viewSeparator = FindViewById<View>(Resource.Id.itemisedBillingGroupSeparator);
            viewSeparator.Visibility = isShown ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}
