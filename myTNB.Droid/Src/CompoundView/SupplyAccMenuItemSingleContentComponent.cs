using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class SupplyAccMenuItemSingleContentComponent : RelativeLayout
    {
        private TextView itemTitle;
        private ImageView supplyIcon, itemAction;
        private LinearLayout itemContainer, itemActionContainer, ContainerAction;

        public SupplyAccMenuItemSingleContentComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public SupplyAccMenuItemSingleContentComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public SupplyAccMenuItemSingleContentComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public SupplyAccMenuItemSingleContentComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ProfileMainMenuItemContentActionLayout, this);
            itemContainer = FindViewById<LinearLayout>(Resource.Id.itemContainer);
            itemTitle = FindViewById<TextView>(Resource.Id.itemTitle);
            supplyIcon = FindViewById<ImageView>(Resource.Id.img_profile);
            itemAction = FindViewById<ImageView>(Resource.Id.itemAction);
            itemActionContainer = FindViewById<LinearLayout>(Resource.Id.itemActionContainer);
            ContainerAction = FindViewById<LinearLayout>(Resource.Id.ContainerAction);

            TextViewUtils.SetMuseoSans500Typeface(itemTitle);
            TextViewUtils.SetTextSize14(itemTitle);
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
                supplyIcon.SetBackgroundResource(Resource.Drawable.user_access);

            }
            else
            {
                supplyIcon.SetBackgroundResource(Resource.Drawable.autopay_yellow);
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

        public void AddSeparator()
        {
            View separatorView = new View(Context);
            separatorView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, 3);
            separatorView.SetBackgroundColor(Color.ParseColor("#e4e4e4"));
            itemContainer.AddView(separatorView);
        }
    }
}
