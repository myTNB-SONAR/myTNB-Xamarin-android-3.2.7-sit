using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.CompoundView
{
    public class ProfileMenuItemSingleContentComponent : RelativeLayout
    {
        private TextView itemTitle;
        private LinearLayout itemContainer;

        public ProfileMenuItemSingleContentComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemSingleContentComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemSingleContentComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemSingleContentComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ProfileMenuItemContentNoActionLayout, this);
            itemContainer = FindViewById<LinearLayout>(Resource.Id.itemContainer);
            itemTitle = FindViewById<TextView>(Resource.Id.itemTitle);

            TextViewUtils.SetMuseoSans500Typeface(itemTitle);
            TextViewUtils.SetTextSize14(itemTitle);
        }

        public void SetTitle(string title)
        {
            itemTitle.Text = title;
        }

        public void SetItemActionCall(Action action)
        {
            itemContainer.Click += delegate
            {
                action();
            };
        }
    }
}
