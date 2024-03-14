using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace myTNB.Android.Src.CompoundView
{
    public class ItemisedBillingHeaderView : LinearLayout
    {
        public ItemisedBillingHeaderView(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ItemisedBillingHeaderView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ItemisedBillingHeaderView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ItemisedBillingHeaderLayout, this);
        }
    }
}
