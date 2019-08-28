using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace myTNB_Android.Src.CompoundView
{
    public class ExpandableTextViewComponent : LinearLayout
    {
        Context mContext;
        public ExpandableTextViewComponent(Context context) : base(context)
        {
            mContext = context;
            Init();
        }

        public ExpandableTextViewComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init();
        }

        public ExpandableTextViewComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init();
        }

        private void Init()
        {
            Inflate(mContext,Resource.Layout.ExpandingTextViewLayout,this);
            CreateItems();
        }

        public void CreateItems()
        {
            LinearLayout expandableContainer = FindViewById<LinearLayout>(Resource.Id.expandedContent);
            SetOnClickListener(new OnExpandListener(expandableContainer));
            expandableContainer.Visibility = ViewStates.Gone;

            LinearLayout item = (LinearLayout)Inflate(mContext,Resource.Layout.MyOtherChargesItemLayout,null);
            TextView textView = item.FindViewById<TextView>(Resource.Id.otherChargeItem);
            TextView textValue = item.FindViewById<TextView>(Resource.Id.otherChargeValue);
            textView.Text = "Test 1";
            textValue.Text = "RM1.00";
            expandableContainer.AddView(item);

            item = (LinearLayout)Inflate(mContext, Resource.Layout.MyOtherChargesItemLayout, null);
            textView = item.FindViewById<TextView>(Resource.Id.otherChargeItem);
            textValue = item.FindViewById<TextView>(Resource.Id.otherChargeValue);
            textView.Text = "Test 2";
            textValue.Text = "RM2.00";
            expandableContainer.AddView(item);
        }

        public class OnExpandListener : Java.Lang.Object, IOnClickListener
        {
            LinearLayout expandedContent;
            bool isVisible = true;
            public OnExpandListener(LinearLayout content)
            {
                expandedContent = content;
            }
            public void OnClick(View v)
            {
                if (!isVisible)
                {
                    expandedContent.Visibility = ViewStates.Gone;
                }
                else
                {
                    expandedContent.Visibility = ViewStates.Visible;
                }
                isVisible = !isVisible;
            }
        }
    }
}
    