using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ItemisedBillingGroupContentComponent : LinearLayout
    {
        private Context mContext;
        TextView dateHistoryTypeView, paidViaView, amountView;
        private bool isPayment = false;
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
            dateHistoryTypeView = FindViewById<TextView>(Resource.Id.itemisedBillingItemTitle);
            paidViaView = FindViewById<TextView>(Resource.Id.itemisedBillingItemSubTitle);
            amountView = FindViewById<TextView>(Resource.Id.itemisedBillingItemAmount);

            TextViewUtils.SetMuseoSans300Typeface(paidViaView);
            TextViewUtils.SetMuseoSans500Typeface(dateHistoryTypeView, amountView);
        }

        public void IsPayment(bool isPay)
        {
            isPayment = isPay;
        }

        public void SetDateHistoryType(string dateHistoryType)
        {
            dateHistoryTypeView.Text = dateHistoryType;
        }

        public void SetPaidVia(string paidVia)
        {
            paidViaView.Text = paidVia;
        }

        public void SetAmount(string amount)
        { 
            amountView.Text = amount;
            if (isPayment)
            {
                amountView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(mContext, Resource.Color.freshGreen)));
            }
            else
            {
                amountView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(mContext, Resource.Color.tunaGrey)));
            }
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
                imageRightArrow.Visibility = ViewStates.Invisible;
            }
        }
    }
}
