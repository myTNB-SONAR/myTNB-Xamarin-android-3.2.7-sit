using System;
using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Android.Widget;
using myTNB_Android.Src.NotificationDetails.Models;

namespace myTNB_Android.Src.CompoundView
{
    public class NotificationDetailCTAComponent : LinearLayout
    {
        public NotificationDetailCTAComponent(Context context) : base(context)
        {
            InitializeViews(context);
        }

        public NotificationDetailCTAComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitializeViews(context);
        }

        public NotificationDetailCTAComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitializeViews(context);
        }

        public NotificationDetailCTAComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
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
                LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)primaryBtn.LayoutParameters;
                layoutParams.Weight = 1f;
                primaryBtn.LayoutParameters = layoutParams;
                primaryBtn.Text = ctaList[0].label;

                primaryBtn.Click += delegate
                {
                    ctaList[0].action();
                };
            }
            else
            {
                Button primaryBtn = FindViewById<Button>(Resource.Id.btnPrimary);
                LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams)primaryBtn.LayoutParameters;
                layoutParams.Weight = 0.5f;
                primaryBtn.LayoutParameters = layoutParams;
                primaryBtn.Text = ctaList[0].label;

                primaryBtn.Click += delegate
                {
                    ctaList[0].action();
                };

                Button secondaryBtn = FindViewById<Button>(Resource.Id.btnSecondary);
                layoutParams = (LinearLayout.LayoutParams)secondaryBtn.LayoutParameters;
                layoutParams.Weight = 0.5f;
                secondaryBtn.LayoutParameters = layoutParams;
                secondaryBtn.Text = ctaList[1].label;

                secondaryBtn.Click += delegate
                {
                    ctaList[1].action();
                };
            }
        }
    }
}
