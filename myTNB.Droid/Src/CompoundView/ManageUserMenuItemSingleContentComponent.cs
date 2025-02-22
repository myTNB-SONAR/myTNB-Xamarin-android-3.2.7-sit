﻿using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.CompoundView
{
    public class ManageUserMenuItemSingleContentComponent : RelativeLayout
    {
        private TextView itemTitle;
        private ImageView supplyIcon;
        private CheckBox itemAction;
        private LinearLayout itemContainer, itemActionContainer;

        public ManageUserMenuItemSingleContentComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ManageUserMenuItemSingleContentComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ManageUserMenuItemSingleContentComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ManageUserMenuItemSingleContentComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ManageUserMenuItemSingleLayout, this);
            itemContainer = FindViewById<LinearLayout>(Resource.Id.itemContainer);
            itemTitle = FindViewById<TextView>(Resource.Id.itemTitle);
            itemAction = FindViewById<CheckBox>(Resource.Id.itemAction);
            itemActionContainer = FindViewById<LinearLayout>(Resource.Id.itemActionContainer);

            TextViewUtils.SetMuseoSans500Typeface(itemTitle);
        }

        public void SetTitle(string title)
        {
            itemTitle.Text = title;
        }

        public void SetItemActionVisibility(bool isVisible)
        {
            itemActionContainer.Visibility = isVisible ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
        }


        public void SetItemActionCall(Action action)
        {
            itemContainer.Click += delegate
            {
                action();
            };
        }

        public void AddSeparator()
        {
            View separatorView = new View(Context);
            separatorView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, 3);
            separatorView.SetBackgroundColor(Color.ParseColor("#e4e4e4"));
            itemContainer.AddView(separatorView);
        }
    }
}
