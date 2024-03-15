using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.CompoundView
{
    public class ProfileDetailItemComponent : LinearLayout
    {
        //private TextView itemHeaderTitle;
        private LinearLayout profileItemContent;
        public ProfileDetailItemComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ProfileDetailItemComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ProfileDetailItemComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ProfileDetailItemComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ProfileDetailItemLayout, this);
            //itemHeaderTitle = FindViewById<TextView>(Resource.Id.profileItemHeader);
            profileItemContent = FindViewById<LinearLayout>(Resource.Id.profileItemContentDetail);

            //TextViewUtils.SetMuseoSans500Typeface(itemHeaderTitle);
        }

        /*public void SetHeaderTitle(string title)
        {
            itemHeaderTitle.Text = title;
        }*/

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
            separatorView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, 3);
            separatorView.SetBackgroundColor(Color.ParseColor("#e4e4e4"));
            profileItemContent.AddView(separatorView);
        }
    }
}
