﻿using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Billing.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB_Android.Src.NewAppTutorial.Adapter;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.NewAppTutorial.MVP
{
    public class NewAppTutorialDialogFragment : DialogFragment, View.IOnTouchListener, ViewPager.IPageTransformer
    {

        private Android.App.Activity mContext;
        private ViewPager pager;
        private NewAppTutorialPagerAdapter adapter;
        private LinearLayout swipeDoubleTapLayout;
        private LinearLayout indicator;
        private TextView txtSwipeSeeMore;
        private TextView txtDoubleTapDismiss;
        private List<NewAppModel> NewAppTutorialList = new List<NewAppModel>();
        private GestureDetector mGeatureDetector;
        private Android.App.Fragment mFragment;
        private ISharedPreferences mPref;

        public NewAppTutorialDialogFragment(Android.App.Activity ctx, Android.App.Fragment fragment, ISharedPreferences pref, List<NewAppModel> list)
        {
            this.mContext = ctx;
            if (list != null && list.Count > 0)
            {
                this.NewAppTutorialList = list;
            }
            this.mFragment = fragment;
            this.mPref = pref;
        }

        public override void OnStart()
        {
            base.OnStart();

            if (Dialog != null)
            {
                Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                Dialog.Window.SetDimAmount(0.0f);
                Dialog.SetCancelable(false);
                Dialog.SetCanceledOnTouchOutside(false);
               
                Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            /*Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.SetDimAmount(0.0f);
            Dialog.SetCancelable(false);
            Dialog.SetCanceledOnTouchOutside(false);
            WindowManagerLayoutParams wlp = Dialog.Window.Attributes;
            wlp.Gravity = GravityFlags.Center;
            wlp.Width = ViewGroup.LayoutParams.MatchParent;
            wlp.Height = ViewGroup.LayoutParams.MatchParent;
            Dialog.Window.Attributes = wlp;
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);*/

            View rootView = inflater.Inflate(Resource.Layout.NewAppTutorialLayout, container, false);

            try
            {
                pager = rootView.FindViewById<ViewPager>(Resource.Id.viewPager);
                indicator = rootView.FindViewById<LinearLayout>(Resource.Id.indicatorContainer);
                swipeDoubleTapLayout = rootView.FindViewById<LinearLayout>(Resource.Id.swipeDoubleTapLayout);
                txtSwipeSeeMore = rootView.FindViewById<TextView>(Resource.Id.txtSwipeSeeMore);
                txtDoubleTapDismiss = rootView.FindViewById<TextView>(Resource.Id.txtDoubleTapDismiss);

                TextViewUtils.SetMuseoSans300Typeface(txtSwipeSeeMore, txtDoubleTapDismiss);

                if (this.mFragment != null)
                {
                    if (this.mFragment is HomeMenuFragment)
                    {
                        if (NewAppTutorialList.Count > 0)
                        {
                            int ItemCount = NewAppTutorialList[0].ItemCount;
                            if (ItemCount > 1)
                            {
                                int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                                if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                                {
                                    int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                    int halfScroll = topHeight / 2;

                                    if (diffHeight < halfScroll)
                                    {
                                        ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(topHeight / 2);
                                    }
                                    else
                                    {
                                        ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                                    }
                                }
                                else
                                {
                                    ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                                }
                            }
                            else
                            {
                                ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                            }
                        }
                        else
                        {
                            ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                        }
                    }
                    else if (this.mFragment is ItemisedBillingMenuFragment)
                    {
                        string DisplayMode = NewAppTutorialList[0].DisplayMode;
                        if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(55f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(285f);
                            int checkPoint = (int)DPUtils.ConvertDPToPx(200f);
                            if (DisplayMode == "Extra")
                            {
                                middleHeight = (int)DPUtils.ConvertDPToPx(265f);
                                checkPoint = (int)DPUtils.ConvertDPToPx(180f);
                            }


                            if (((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                            {
                                ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling((int)DPUtils.ConvertDPToPx(15f));
                            }
                            else
                            {
                                ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                            }
                        }
                        else
                        {
                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                        }
                    }
                    else if (this.mFragment is DashboardChartFragment)
                    {
                        if (((DashboardChartFragment)this.mFragment).CheckIsScrollable())
                        {
                            ((DashboardChartFragment)this.mFragment).DashboardCustomScrolling(((DashboardChartFragment)this.mFragment).OnGetEndOfScrollView());
                        }
                        else
                        {
                            ((DashboardChartFragment)this.mFragment).HideBottomSheet();
                            ((DashboardChartFragment)this.mFragment).DashboardCustomScrolling(0);
                        }
                    }
                }
                else
                {
                    if (this.mContext is SSMRMeterHistoryActivity)
                    {
                        if (((SSMRMeterHistoryActivity)mContext).CheckIsScrollable())
                        {
                            string DisplayMode = NewAppTutorialList[0].DisplayMode;
                            int ItemCount = NewAppTutorialList[0].ItemCount;
                            int topHeight = (int)DPUtils.ConvertDPToPx(255f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(245f);
                            int checkPoint = (int)DPUtils.ConvertDPToPx(50f);
                            if (DisplayMode == "NONSMR")
                            {
                                middleHeight = (int)DPUtils.ConvertDPToPx(235f);
                                checkPoint = (int)DPUtils.ConvertDPToPx(40f);
                            }

                            if (ItemCount == 1)
                            {
                                middleHeight = (int)DPUtils.ConvertDPToPx(190f);
                                checkPoint = 0;
                            }

                            if (((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                            {
                                ((SSMRMeterHistoryActivity)mContext).MeterHistoryCustomScrolling((int)DPUtils.ConvertDPToPx(30f));
                            }
                            else
                            {
                                ((SSMRMeterHistoryActivity)mContext).MeterHistoryCustomScrolling(0);
                            }

                        }
                        else
                        {
                            ((SSMRMeterHistoryActivity)mContext).MeterHistoryCustomScrolling(0);
                        }
                    }
                    else if (this.mContext is SubmitMeterReadingActivity)
                    {
                        if (((SubmitMeterReadingActivity)mContext).CheckIsScrollable())
                        {
                            ((SubmitMeterReadingActivity)mContext).SubmitMeterCustomScrolling(0);
                        }
                    }
                }

                if (NewAppTutorialList.Count > 0)
                {
                    if (NewAppTutorialList.Count > 1)
                    {
                        swipeDoubleTapLayout.Visibility = ViewStates.Visible;
                        txtSwipeSeeMore.Visibility = ViewStates.Visible;
                        txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                        indicator.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        swipeDoubleTapLayout.Visibility = ViewStates.Gone;
                    }

                    if (this.mFragment != null && this.mFragment is ItemisedBillingMenuFragment)
                    {
                        string DisplayMode = NewAppTutorialList[0].DisplayMode;
                        if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(55f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(285f);
                            int checkPoint = (int)DPUtils.ConvertDPToPx(200f);
                            if (DisplayMode == "Extra")
                            {
                                middleHeight = (int)DPUtils.ConvertDPToPx(265f);
                                checkPoint = (int)DPUtils.ConvertDPToPx(180f);
                            }

                            if (((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                            {
                                txtSwipeSeeMore.Visibility = ViewStates.Gone;
                                txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                            }
                        }
                        else
                        {
                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                        }
                    }


                    if (NewAppTutorialList.Count > 1)
                    {
                        for (int i = 0; i < NewAppTutorialList.Count; i++)
                        {
                            ImageView image = new ImageView(mContext);
                            image.Id = i;
                            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            layoutParams.RightMargin = 8;
                            layoutParams.LeftMargin = 8;
                            image.LayoutParameters = layoutParams;
                            if (i == 0)
                            {
                                image.SetImageResource(Resource.Drawable.white_circle_active);
                            }
                            else
                            {
                                image.SetImageResource(Resource.Drawable.white_circle);
                            }
                            indicator.AddView(image, i);
                        }
                    }

                    adapter = new NewAppTutorialPagerAdapter(mContext, mFragment, mPref, this, NewAppTutorialList);
                    pager.Adapter = adapter;

                    if (NewAppTutorialList.Count > 1)
                    {
                        pager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) => {
                            for (int i = 0; i < NewAppTutorialList.Count; i++)
                            {
                                ImageView selectedDot = (ImageView)indicator.GetChildAt(i);
                                if (e.Position == i)
                                {
                                    selectedDot.SetImageResource(Resource.Drawable.white_circle_active);
                                }
                                else
                                {
                                    selectedDot.SetImageResource(Resource.Drawable.white_circle);
                                }
                            }

                            if (e.Position == NewAppTutorialList.Count - 1)
                            {
                                txtSwipeSeeMore.Visibility = ViewStates.Gone;
                                txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                txtSwipeSeeMore.Visibility = ViewStates.Visible;
                                txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                            }

                            if (this.mFragment != null)
                            {
                                if (this.mFragment is HomeMenuFragment)
                                {
                                    if (NewAppTutorialList.Count > 0)
                                    {
                                        if (e.Position == NewAppTutorialList.Count - 1)
                                        {
                                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                                            {
                                                ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                            }
                                            else
                                            {
                                                ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                                            }
                                        }
                                        else
                                        {
                                            int ItemCount = NewAppTutorialList[0].ItemCount;
                                            if (ItemCount > 1)
                                            {
                                                int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                                                if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                                                {
                                                    int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                                    int halfScroll = topHeight / 2;

                                                    if (diffHeight < halfScroll)
                                                    {
                                                        ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(topHeight / 2);
                                                    }
                                                    else
                                                    {
                                                        ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                                                    }
                                                }
                                                else
                                                {
                                                    ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                                                }
                                            }
                                            else
                                            {
                                                ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                                            }
                                        }
                                    }
                                }
                                else if (this.mFragment is ItemisedBillingMenuFragment)
                                {
                                    if (NewAppTutorialList.Count >= 2)
                                    {
                                        int ItemCount = NewAppTutorialList[0].ItemCount;
                                        string DisplayMode = NewAppTutorialList[0].DisplayMode;
                                        if (NewAppTutorialList.Count == 2)
                                        {
                                            if (e.Position == NewAppTutorialList.Count - 1)
                                            {
                                                if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                                {
                                                    int topHeight = (int)DPUtils.ConvertDPToPx(375f);
                                                    int middleHeight = (int)DPUtils.ConvertDPToPx(210f);

                                                    if (ItemCount == 0)
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)))
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(((ItemisedBillingMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                                        }
                                                        else
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                        }
                                                    }
                                                    else if (ItemCount == 1)
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)))
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(((ItemisedBillingMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                                        }
                                                        else
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                        }
                                                    }
                                                    else if (ItemCount == 2)
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)))
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(((ItemisedBillingMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                                        }
                                                        else
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)))
                                                        {
                                                            int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)) + (int)DPUtils.ConvertDPToPx(20f);
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(diff);
                                                        }
                                                        else
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                }
                                            }
                                            else
                                            {
                                                if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                                {
                                                    int topHeight = (int)DPUtils.ConvertDPToPx(55f);
                                                    int middleHeight = (int)DPUtils.ConvertDPToPx(285f);
                                                    int checkPoint = (int)DPUtils.ConvertDPToPx(200f);
                                                    if (DisplayMode == "Extra")
                                                    {
                                                        middleHeight = (int)DPUtils.ConvertDPToPx(265f);
                                                        checkPoint = (int)DPUtils.ConvertDPToPx(180f);
                                                    }

                                                    if (((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                                                    {
                                                        ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling((int)DPUtils.ConvertDPToPx(15f));
                                                        txtSwipeSeeMore.Visibility = ViewStates.Gone;
                                                        txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                                                    }
                                                    else
                                                    {
                                                        ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                    }
                                                }
                                                else
                                                {
                                                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (e.Position == NewAppTutorialList.Count - 1)
                                            {
                                                if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                                {
                                                    int topHeight = (int)DPUtils.ConvertDPToPx(430f);
                                                    int middleHeight = (int)DPUtils.ConvertDPToPx(210f);

                                                    if (DisplayMode == "Extra")
                                                    {
                                                        topHeight = (int)DPUtils.ConvertDPToPx(405f);
                                                    }

                                                    if (ItemCount == 0)
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)))
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(((ItemisedBillingMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                                        }
                                                        else
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                        }
                                                    }
                                                    else if (ItemCount == 1)
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)))
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(((ItemisedBillingMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                                        }
                                                        else
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                        }
                                                    }
                                                    else if (ItemCount == 2)
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)))
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(((ItemisedBillingMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                                        }
                                                        else
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)))
                                                        {
                                                            int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(52f)) + (int)DPUtils.ConvertDPToPx(20f);
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(diff);
                                                        }
                                                        else
                                                        {
                                                            ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                }
                                            }
                                            else if (e.Position == 0)
                                            {
                                                int topHeight = (int)DPUtils.ConvertDPToPx(55f);
                                                int middleHeight = (int)DPUtils.ConvertDPToPx(285f);
                                                int checkPoint = (int)DPUtils.ConvertDPToPx(200f);
                                                if (DisplayMode == "Extra")
                                                {
                                                    middleHeight = (int)DPUtils.ConvertDPToPx(265f);
                                                    checkPoint = (int)DPUtils.ConvertDPToPx(180f);
                                                }

                                                if (((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                                                {
                                                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling((int)DPUtils.ConvertDPToPx(15f));
                                                    txtSwipeSeeMore.Visibility = ViewStates.Gone;
                                                    txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                                                }
                                                else
                                                {
                                                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                }
                                            }
                                            else
                                            {
                                                ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                }
                else
                {
                    swipeDoubleTapLayout.Visibility = ViewStates.Gone;
                }

                mGeatureDetector = new GestureDetector(mContext, new DialogTapDetector(this, this.mFragment, this.mContext, this.mPref, this.NewAppTutorialList));
                pager.SetOnTouchListener(this);
                pager.SetPageTransformer(false, this);

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
            return rootView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnPause()
        {
            base.OnPause();
            NewAppTutorialUtils.ForceCloseNewAppTutorial();
            if (this.mFragment != null)
            {
                if (this.mFragment is HomeMenuFragment)
                {
                    ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                }
                else if (this.mFragment is ItemisedBillingMenuFragment)
                {
                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                }
                else if (this.mFragment is DashboardChartFragment)
                {
                    ((DashboardChartFragment)this.mFragment).DashboardCustomScrolling(0);
                    ((DashboardChartFragment)this.mFragment).ShowBottomSheet();
                }
            }
            else
            {
                if (this.mContext is SSMRMeterHistoryActivity)
                {
                    ((SSMRMeterHistoryActivity)mContext).MeterHistoryCustomScrolling(0);
                }
                else if (this.mContext is SubmitMeterReadingActivity)
                {
                    ((SubmitMeterReadingActivity)mContext).SubmitMeterCustomScrolling(0);
                }
            }

        }

        public void CloseDialog()
        {
            this.DismissAllowingStateLoss();
            if (this.mFragment != null)
            {
                if (this.mFragment is HomeMenuFragment)
                {
                    ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                }
                else if (this.mFragment is ItemisedBillingMenuFragment)
                {
                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                }
                else if (this.mFragment is DashboardChartFragment)
                {
                    ((DashboardChartFragment)this.mFragment).DashboardCustomScrolling(0);
                    ((DashboardChartFragment)this.mFragment).ShowBottomSheet();
                }
            }
            else
            {
                if (this.mContext is SSMRMeterHistoryActivity)
                {
                    ((SSMRMeterHistoryActivity)mContext).MeterHistoryCustomScrolling(0);
                }
                else if (this.mContext is SubmitMeterReadingActivity)
                {
                    ((SubmitMeterReadingActivity)mContext).SubmitMeterCustomScrolling(0);
                }
            }
        }

        bool View.IOnTouchListener.OnTouch(View v, MotionEvent e)
        {
            return mGeatureDetector.OnTouchEvent(e);

        }

        void ViewPager.IPageTransformer.TransformPage(View page, float position)
        {
            page.TranslationX = page.Width * -position;

            if (position <= -1.0f || position >= 1.0f)
            {
                page.Alpha = 0.0f;
            }
            else if (position == 0.0f)
            {
                page.Alpha = 1.0f;
            }
            else
            {
                page.Alpha = 1.0f - Math.Abs(position);
            }
        }

        private class DialogTapDetector : GestureDetector.SimpleOnGestureListener
        {
            DialogFragment mDialog;
            Android.App.Fragment mFragment;
            Android.App.Activity mActivity;
            ISharedPreferences mPref;
            List<NewAppModel> NewAppTutorialList = new List<NewAppModel>();

            public DialogTapDetector(DialogFragment dialog, Android.App.Fragment fragment, Android.App.Activity activity, ISharedPreferences pref, List<NewAppModel> list)
            {
                this.mDialog = dialog;
                this.mFragment = fragment;
                this.mActivity = activity;
                this.mPref = pref;
                if (list != null && list.Count > 0)
                {
                    this.NewAppTutorialList = list;
                }
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                this.mDialog.DismissAllowingStateLoss();
                NewAppTutorialUtils.CloseNewAppTutorial();
                if (this.mFragment != null)
                {
                    if (this.mFragment is HomeMenuFragment)
                    {
                        ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                        ((HomeMenuFragment)this.mFragment).RestartHomeMenu();
                        UserSessions.DoHomeTutorialShown(this.mPref);
                    }
                    else if (this.mFragment is ItemisedBillingMenuFragment)
                    {
                        ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                        UserSessions.DoItemizedBillingTutorialShown(this.mPref);

                        // Lin Siong TODO: To implement once iOS implement
                        /*if (NewAppTutorialList.Count == 2)
                        {
                            UserSessions.DoItemizedBillingRETutorialShown(this.mPref);
                        }
                        else
                        {
                            UserSessions.DoItemizedBillingNMSMTutorialShown(this.mPref);
                        }*/
                    }
                    else if (this.mFragment is DashboardChartFragment)
                    {
                        ((DashboardChartFragment)this.mFragment).DashboardCustomScrolling(0);
                        ((DashboardChartFragment)this.mFragment).ShowBottomSheet();
                        UserSessions.DoSMRDashboardTutorialShown(this.mPref);
                    }
                }
                else
                {
                    if (this.mActivity is BillingDetailsActivity)
                    {
                        UserSessions.DoItemizedBillingDetailTutorialShown(this.mPref);
                    }
                    else if (this.mActivity is SSMRMeterHistoryActivity)
                    {
                        ((SSMRMeterHistoryActivity)this.mActivity).MeterHistoryCustomScrolling(0);
                        UserSessions.DoSMRMeterHistoryTutorialShown(this.mPref);
                    }
                    else if (this.mActivity is SubmitMeterReadingActivity)
                    {
                        ((SubmitMeterReadingActivity)mActivity).SubmitMeterCustomScrolling(0);
                        UserSessions.DoSMRSubmitMeterTutorialShown(this.mPref);
                    }
                }
                return true;
            }

            public override bool OnDoubleTapEvent(MotionEvent e)
            {
                return true;
            }

            public override bool OnDown(MotionEvent e)
            {
                return true;
            }


            public override void OnLongPress(MotionEvent e)
            {
                try
                {
                    base.OnLongPress(e);
                }
                catch (Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }

        }

    }
}