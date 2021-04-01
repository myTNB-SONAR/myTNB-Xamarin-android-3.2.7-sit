using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ProfileMainMenuItemSingleContentComponent : RelativeLayout
    {
        private TextView itemTitle;
        private LinearLayout itemActionContainer, itemContainer, ContainerAction;
        private ImageView imgIcon, itemAction;

        public ProfileMainMenuItemSingleContentComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuItemSingleContentComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuItemSingleContentComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ProfileMainMenuItemSingleContentComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ProfileMainMenuItemContentActionLayout, this);
            imgIcon = FindViewById<ImageView>(Resource.Id.img_profile);
            itemTitle = FindViewById<TextView>(Resource.Id.itemTitle);
            itemAction = FindViewById<ImageView>(Resource.Id.itemAction);
            itemActionContainer = FindViewById<LinearLayout>(Resource.Id.itemActionContainer);
            itemContainer = FindViewById<LinearLayout>(Resource.Id.itemContainer);
            ContainerAction = FindViewById<LinearLayout>(Resource.Id.ContainerAction);

            TextViewUtils.SetMuseoSans500Typeface(itemTitle);
            itemTitle.TextSize = TextViewUtils.GetFontSize(14);
        }

        public void SetTitle(string title)
        {
            itemTitle.Text = title;
        }

        public void SetIcon(int code)
        {
            itemAction.SetBackgroundResource(Resource.Drawable.expand_right_arrow);

            if (code == 1)
            {
                imgIcon.SetBackgroundResource(Resource.Drawable.payment_methods);

            }
            else if (code == 2)
            {
                imgIcon.SetBackgroundResource(Resource.Drawable.electricity_accounts);

            }
            else if (code == 3)
            {
                imgIcon.SetBackgroundResource(Resource.Drawable.app_settings);

            }
            else
            {
                imgIcon.SetBackgroundResource(Resource.Drawable.learn_more_tnb);
            }

        }

        public void SetItemActionVisibility(bool isVisible)
        {
            itemActionContainer.Visibility = isVisible ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
        }

        public void SetItemActionCall(Action action)
        {
            ContainerAction.Click += delegate
            {
                action();
            };
        }

       /* public void EnableActionCall(bool isEnable)
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
        }*/
    }
}
