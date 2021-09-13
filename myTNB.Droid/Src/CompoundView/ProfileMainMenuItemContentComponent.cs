using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ProfileMainMenuItemContentComponent : RelativeLayout
    {
        private TextView itemTitle, itemValue, itemValue2, itemVerifyLabel;
        private LinearLayout itemActionContainer, itemContainer;
        private ImageView itemAction, itemIcon;

        public ProfileMainMenuItemContentComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuItemContentComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuItemContentComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuItemContentComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ProfileMainMenuItemContentLayout, this);
            itemTitle = FindViewById<TextView>(Resource.Id.itemTitle);
            itemValue = FindViewById<TextView>(Resource.Id.itemValue);
            itemValue2 = FindViewById<TextView>(Resource.Id.itemValue2);
            itemAction = FindViewById<ImageView>(Resource.Id.itemAction);
            itemVerifyLabel = FindViewById<TextView>(Resource.Id.infoLabel);
            itemActionContainer = FindViewById<LinearLayout>(Resource.Id.itemActionContainer);
            itemContainer = FindViewById<LinearLayout>(Resource.Id.itemContainer);


            TextViewUtils.SetMuseoSans300Typeface(itemValue, itemValue2);
            TextViewUtils.SetMuseoSans500Typeface(itemTitle);
            TextViewUtils.SetTextSize14(itemTitle);
            TextViewUtils.SetTextSize12(itemValue, itemValue2);
            

        }

        public void SetFlagID(bool FlagID)
        {
            if (FlagID)
            {
                itemVerifyLabel.Visibility = Android.Views.ViewStates.Visible;
                //itemVerifyLabel.SetBackgroundResource(Resource.Drawable.icons_verify_email);
                itemVerifyLabel.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icons_verify_email, 0);
            }
            else
            {
                itemVerifyLabel.Visibility = Android.Views.ViewStates.Gone;
            }
        }

        public void SetTitle(string title)
        {
            itemTitle.Text = title;
        }

        public void SetValue(string value)
        {
            itemValue.Text = value;
        }

        public void SetValue2(string value2)
        {
            itemValue2.Text = value2;
        }

        public void SetItemActionVisibility(bool isVisible)
        {
            itemActionContainer.Visibility = isVisible ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
        }

        public void SetItemActionCall(Action action)
        {
            itemActionContainer.Click += delegate
            {
                action();
            };
        }

    }
}
