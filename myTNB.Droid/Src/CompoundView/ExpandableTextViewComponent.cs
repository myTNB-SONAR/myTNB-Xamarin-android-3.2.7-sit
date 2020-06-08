﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ExpandableTextViewComponent : LinearLayout
    {
        Context mContext;
        Drawable dropdown;
        TextView myApplicationChargesLabel;
        TextView myApplicationChargesValue;
        LinearLayout expandableContainer;

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

            dropdown = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_action_expand_down);

            myApplicationChargesLabel = FindViewById<TextView>(Resource.Id.myApplicationChargesLabel);
            myApplicationChargesValue = FindViewById<TextView>(Resource.Id.myApplicationChargesValue);
            expandableContainer = FindViewById<LinearLayout>(Resource.Id.expandedContent);

            myApplicationChargesLabel.SetCompoundDrawablesWithIntrinsicBounds(null, null, dropdown, null);
            myApplicationChargesLabel.CompoundDrawablePadding = (int) DPUtils.ConvertDPToPx(4f);

            TextViewUtils.SetMuseoSans500Typeface(myApplicationChargesLabel, myApplicationChargesValue);

            SetOnClickListener(new OnExpandListener(this));
            expandableContainer.Visibility = ViewStates.Gone;
            expandableContainer.RemoveAllViews();
        }

        public void SetApplicationChargesLabel(string label)
        {
            myApplicationChargesLabel.Text = label;
        }

        public void OnClickExpand(bool isExpand)
        {
            if (isExpand)
            {
                dropdown = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_action_expand_down_up);
            }
            else
            {
                dropdown = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_action_expand_down);
            }
            myApplicationChargesLabel.SetCompoundDrawablesWithIntrinsicBounds(null, null, dropdown, null);
            myApplicationChargesLabel.CompoundDrawablePadding = (int)DPUtils.ConvertDPToPx(4f);

            expandableContainer.Visibility = isExpand ? ViewStates.Visible : ViewStates.Gone;
        }

        public void SetOtherCharges(double totalAmount, List<ChargeModel> chargeList)
        {
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            myApplicationChargesValue.Text = "RM " + totalAmount.ToString("#,##0.00", currCult);
            chargeList.ForEach(charge =>
            {
                if (charge.Amount > 0.00)
                {
                    LinearLayout item = (LinearLayout)LayoutInflater.From(mContext).Inflate(Resource.Layout.MyOtherChargesItemLayout,this,false);
                    TextView textView = item.FindViewById<TextView>(Resource.Id.otherChargeItem);
                    TextView textValue = item.FindViewById<TextView>(Resource.Id.otherChargeValue);

                    textView.Text = charge.Title;
                    textValue.Text = "RM" + charge.Amount.ToString("#,##0.00", currCult);
                    TextViewUtils.SetMuseoSans300Typeface(textView, textValue);
                    expandableContainer.AddView(item);
                }
            });
            expandableContainer.Invalidate();
            expandableContainer.RequestLayout();
        }

        public class OnExpandListener : Java.Lang.Object, IOnClickListener
        {
            ExpandableTextViewComponent ownerView;
            bool isVisible = true;
            public OnExpandListener(ExpandableTextViewComponent view)
            {
                ownerView = view;
            }
            public void OnClick(View v)
            {
                if (!isVisible)
                {
                    ownerView.OnClickExpand(false);
                }
                else
                {
                    ownerView.OnClickExpand(true);
                }
                isVisible = !isVisible;
            }
        }
    }
}
    