using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class NotificationDetailCTAComponent : LinearLayout
    {
        Context mContext;
        public NotificationDetailCTAComponent(Context context) : base(context)
        {
            mContext = context;
            InitializeViews(context);
        }

        public NotificationDetailCTAComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            InitializeViews(context);
        }

        public NotificationDetailCTAComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            InitializeViews(context);
        }

        public NotificationDetailCTAComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            mContext = context;
            InitializeViews(context);
        }

        public void InitializeViews(Context context)
        {
            Inflate(context, Resource.Layout.NotificationDetailCTALayout, this);
        }

        public void SetCTAButton(List<NotificationDetailModel.NotificationCTA> ctaList)
        {
            if (ctaList.Count == 1)
            {
                Button primaryBtn = FindViewById<Button>(Resource.Id.btnPrimary);
                TextViewUtils.SetTextSize16(primaryBtn);

                LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)primaryBtn.LayoutParameters;
                layoutParams.Weight = 1f;
                primaryBtn.LayoutParameters = layoutParams;
                primaryBtn.Text = ctaList[0].label;
                if (ctaList[0].isSolidBackground)
                {
                    primaryBtn.SetTextColor(new Color(ContextCompat.GetColor(Context, Resource.Color.white)));
                    primaryBtn.Background = ContextCompat.GetDrawable(mContext, Resource.Drawable.green_button_background);
                    if (ctaList[0].isEnabled)
                    {
                        primaryBtn.Enabled = true;
                        primaryBtn.Background = ContextCompat.GetDrawable(mContext, Resource.Drawable.green_button_background);
                    }
                    else
                    {
                        primaryBtn.Enabled = false;
                        primaryBtn.Background = ContextCompat.GetDrawable(mContext, Resource.Drawable.silver_chalice_button_background);
                    }
                }
                else
                {
                    primaryBtn.SetTextColor(new Color(ContextCompat.GetColor(Context, Resource.Color.freshGreen)));
                    primaryBtn.Background = ContextCompat.GetDrawable(mContext, Resource.Drawable.light_button_background);
                }
                primaryBtn.Click += delegate
                {
                    ctaList[0].action();
                };
                TextViewUtils.SetMuseoSans500Typeface(primaryBtn);
            }
            else
            {
                Button primaryBtn = FindViewById<Button>(Resource.Id.btnPrimary);
                TextViewUtils.SetTextSize16(primaryBtn);
                LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)primaryBtn.LayoutParameters;
                layoutParams.Weight = 0.5f;
                primaryBtn.LayoutParameters = layoutParams;
                primaryBtn.Text = ctaList[0].label;

                primaryBtn.Click += delegate
                {
                    ctaList[0].action();
                };

                Button secondaryBtn = FindViewById<Button>(Resource.Id.btnSecondary);
                TextViewUtils.SetTextSize16(secondaryBtn);
                layoutParams = (LinearLayout.LayoutParams)secondaryBtn.LayoutParameters;
                layoutParams.Weight = 0.5f;
                secondaryBtn.LayoutParameters = layoutParams;
                secondaryBtn.Text = ctaList[1].label;

                secondaryBtn.Click += delegate
                {
                    ctaList[1].action();
                };

                TextViewUtils.SetMuseoSans500Typeface(primaryBtn, secondaryBtn);
                secondaryBtn.Enabled = ctaList[1].isEnabled;
                if (ctaList[1].isEnabled)
                {
                    secondaryBtn.Background = ContextCompat.GetDrawable(mContext, Resource.Drawable.green_button_background);
                }
                else
                {
                    secondaryBtn.Background = ContextCompat.GetDrawable(mContext, Resource.Drawable.silver_chalice_button_background);
                }
            }
        }
    }
}
