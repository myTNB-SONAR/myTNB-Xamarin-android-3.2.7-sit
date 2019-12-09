﻿using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ProfileMenuItemContentComponent : RelativeLayout
    {
        private TextView itemTitle, itemValue, itemAction;
        private LinearLayout itemActionContainer;

        public ProfileMenuItemContentComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemContentComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemContentComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemContentComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
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
            itemAction.Click += delegate
            {
                action();
            };
        }
    }
}
