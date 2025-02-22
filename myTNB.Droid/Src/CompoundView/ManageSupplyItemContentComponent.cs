﻿using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.CompoundView
{
    public class ManageSupplyItemContentComponent : RelativeLayout
    {
        private TextView itemTitle, itemValue, itemAction;
        private LinearLayout itemActionContainer;

        public ManageSupplyItemContentComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ManageSupplyItemContentComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ManageSupplyItemContentComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ManageSupplyItemContentComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ProfileMenuItemContentLayout, this);
            itemTitle = FindViewById<TextView>(Resource.Id.itemTitle);
            itemValue = FindViewById<TextView>(Resource.Id.itemValue);
            itemAction = FindViewById<TextView>(Resource.Id.itemAction);
            itemActionContainer = FindViewById<LinearLayout>(Resource.Id.itemActionContainer);

            TextViewUtils.SetMuseoSans300Typeface(itemTitle, itemValue);
            TextViewUtils.SetMuseoSans500Typeface(itemAction);
        }

        public void SetTitle(string title)
        {
            itemTitle.Text = title;
        }

        public void SetValue(string value)
        {
            itemValue.Text = value;
        }

       

        public void SetItemActionVisibility(bool isVisible)
        {
            itemActionContainer.Visibility = isVisible ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
        }

        public void SetItemActionTitle(string actionTitle)
        {
            itemAction.Text = actionTitle;
        }

        public void SetItemActionCall(Action action)
        {
            itemActionContainer.Click += delegate
            {
                action();
            };
        }

        public void EnableActionCall(bool isEnable)
        {
            itemActionContainer.Enabled = isEnable;
            if (isEnable)
            {
                itemAction.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Context, Resource.Color.powerBlue)));
            }
            else
            {
                itemAction.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Context, Resource.Color.silverChalice)));
            }
        }
    }
}
