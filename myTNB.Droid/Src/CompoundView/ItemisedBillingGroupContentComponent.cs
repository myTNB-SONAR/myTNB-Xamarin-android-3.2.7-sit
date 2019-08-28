using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;

namespace myTNB_Android.Src.CompoundView
{
    public class ItemisedBillingGroupContentComponent : LinearLayout
    {
        private Context mContext;
        public ItemisedBillingGroupContentComponent(Context context) : base(context)
        {
            mContext = context;
            Init(context);
        }

        public ItemisedBillingGroupContentComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init(context);
        }

        public ItemisedBillingGroupContentComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init(context);
        }

        public void Init(Context context)
        {
            Inflate(context,Resource.Layout.ItemisedBillingGroupContentLayout,this);
        }

        public void SetDateHistoryType(string dateHistoryType)
        {
            TextView dateHistoryTypeView = FindViewById<TextView>(Resource.Id.itemisedBillingItemTitle);
            dateHistoryTypeView.Text = dateHistoryType;
        }

        public void SetPaidVia(string paidVia)
        {
            TextView paidViaView = FindViewById<TextView>(Resource.Id.itemisedBillingItemSubTitle);
            paidViaView.Text = paidVia;
        }

        public void SetAmount(string amount)
        { 
            TextView amountView = FindViewById<TextView>(Resource.Id.itemisedBillingItemAmount);
            amountView.Text = amount;
        }

        public void ShowSeparator(bool isShown)
        {
            View viewSeparator = FindViewById<View>(Resource.Id.itemisedBillingGroupContentSeparator);
            viewSeparator.Visibility = isShown ? ViewStates.Visible : ViewStates.Gone;
        }

        public void SetShowBillingDetailsListener(IOnClickListener onClickBillingDetailsListener)
        {
            ImageView imageRightArrow = FindViewById<ImageView>(Resource.Id.itemisedBillingRightArrow);
            if (onClickBillingDetailsListener != null)
            {
                this.SetOnClickListener(onClickBillingDetailsListener);
                imageRightArrow.Visibility = ViewStates.Visible;
            }
            else
            {
                this.SetOnClickListener(null);
                imageRightArrow.Visibility = ViewStates.Gone;
            }
        }
    }
}
