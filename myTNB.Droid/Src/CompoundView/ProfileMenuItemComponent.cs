using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ProfileMenuItemComponent : LinearLayout
    {
        private TextView itemHeaderTitle;
        private LinearLayout profileItemContent;
        public ProfileMenuItemComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public ProfileMenuItemComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.ProfileMenuItemLayout, this);
            itemHeaderTitle = FindViewById<TextView>(Resource.Id.profileItemHeader);
            profileItemContent = FindViewById<LinearLayout>(Resource.Id.profileItemContent);

            TextViewUtils.SetMuseoSans500Typeface(itemHeaderTitle);
            TextViewUtils.SetTextSize16(itemHeaderTitle);

            try
            {
                Configuration configuration = Resources.Configuration;
                configuration.FontScale = configuration.FontScale >= 1.3F ? 1.3f : configuration.FontScale;

                DisplayMetrics metrics = Resources.DisplayMetrics;
                metrics.ScaledDensity = configuration.FontScale * metrics.Density;

                Resources.UpdateConfiguration(configuration, metrics);
            }
            catch (Java.Lang.Exception javaEx)
            {
                Console.WriteLine("[DEBUG] configuration.DensityDpi Java Exception: " + javaEx.Message);
            }
            catch (System.Exception sysEx)
            {
                Console.WriteLine("[DEBUG] configuration.DensityDpi System Exception: " + sysEx.Message);
            }
        }

        public void SetHeaderTitle(string title)
        {
            itemHeaderTitle.Text = title;
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
            separatorView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, 3);
            separatorView.SetBackgroundColor(Color.ParseColor("#e4e4e4"));
            profileItemContent.AddView(separatorView);
        }
    }
}