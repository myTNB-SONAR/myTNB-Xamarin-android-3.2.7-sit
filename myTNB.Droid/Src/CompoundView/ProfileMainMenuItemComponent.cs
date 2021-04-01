using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ProfileMainMenuComponent : LinearLayout
    {
        private TextView itemHeaderTitle;
        private LinearLayout profileItemContent;
        public ProfileMainMenuComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ProfileMenuItemLayout, this);
            itemHeaderTitle = FindViewById<TextView>(Resource.Id.profileItemHeader);
            profileItemContent = FindViewById<LinearLayout>(Resource.Id.profileItemContent);

            TextViewUtils.SetMuseoSans500Typeface(itemHeaderTitle);
            itemHeaderTitle.TextSize = TextViewUtils.GetFontSize(14);
        }

        public void SetHeaderTitle(string title)
        {
            itemHeaderTitle.Text = title;
        }

        public void HideHeaderTitle()
        {
            itemHeaderTitle.Visibility = ViewStates.Gone;
        }

        public void AddComponentView(View view)
        {
            profileItemContent.AddView(view);
        }

        public void AddComponentView(List<View> viewList)
        {
            profileItemContent.RemoveAllViews();
            viewList.ForEach(view =>
            {
                profileItemContent.AddView(view);
                AddSeparator();
            });
        }

        public void AddSeparator()
        {
            View separatorView = new View(Context);
            separatorView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, 30);
            separatorView.SetBackgroundColor(Color.ParseColor("#F9F9F9"));
            profileItemContent.AddView(separatorView);
        }
    }
}
