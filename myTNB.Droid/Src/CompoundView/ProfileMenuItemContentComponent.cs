using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ProfileMenuItemContentComponent : RelativeLayout
    {
        private TextView itemTitle, itemValue, itemAction, itemVerifyLabel, infoVerifyIcon;
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
            itemVerifyLabel = FindViewById<TextView>(Resource.Id.infoLabel);
            infoVerifyIcon = FindViewById<TextView>(Resource.Id.infoVerifyIcon);
            itemActionContainer = FindViewById<LinearLayout>(Resource.Id.itemActionContainer);

            TextViewUtils.SetMuseoSans300Typeface(itemTitle, itemValue);
            TextViewUtils.SetMuseoSans500Typeface(itemAction);
        }

        public void SetTitle(string title)
        {
            itemTitle.Text = title;
        }

        public void SetFlagID(bool FlagID)
        {
            if (FlagID)
            {
                //itemVerifyLabel.Visibility = Android.Views.ViewStates.Visible;
                itemActionContainer.Visibility = Android.Views.ViewStates.Visible;
                //itemVerifyLabel.SetBackgroundResource(Resource.Drawable.icons_verify_email);
                itemAction.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.icons_verify_email, 0, 0, 0);
                itemValue.SetTextColor(Color.ParseColor("#a6a6a6"));
            }
            else
            {
                itemVerifyLabel.Visibility = Android.Views.ViewStates.Gone;
                itemActionContainer.Visibility = Android.Views.ViewStates.Gone;
                itemValue.SetTextColor(Color.ParseColor("#a6a6a6"));
            }
        }

        public void SetFlagEmailVerify(bool FlagID)
        {
            if (FlagID)
            {
                //infoVerifyIcon.Visibility = Android.Views.ViewStates.Visible;
                itemVerifyLabel.Visibility = Android.Views.ViewStates.Gone;
                itemActionContainer.Visibility = Android.Views.ViewStates.Gone;
                itemValue.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icons_email_verified, 0);
            }
            else
            {
                itemAction.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.icons_verify_email, 0, 0, 0);
                //infoVerifyIcon.Visibility = Android.Views.ViewStates.Gone;
            }
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
