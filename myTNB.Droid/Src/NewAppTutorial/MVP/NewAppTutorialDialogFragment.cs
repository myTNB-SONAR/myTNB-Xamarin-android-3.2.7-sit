using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using AndroidX.Fragment.App;
using AndroidX.ViewPager.Widget;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Billing.MVP;
using myTNB_Android.Src.ManageAccess.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP;
using myTNB_Android.Src.NewAppTutorial.Adapter;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;

namespace myTNB_Android.Src.NewAppTutorial.MVP
{
    public class NewAppTutorialDialogFragment : DialogFragment, View.IOnTouchListener, ViewPager.IPageTransformer
    {

        private Android.App.Activity mContext;
        private ViewPager pager;
        private NewAppTutorialPagerAdapter adapter;
        private LinearLayout swipeDoubleTapLayout;
        private LinearLayout swipeTopDoubleTapLayout;
        private LinearLayout indicator;
        private TextView txtDoubleTapDismiss;
        private LinearLayout indicatorTopContainer;
        private TextView txtTopDoubleTapDismiss;
        private List<NewAppModel> NewAppTutorialList = new List<NewAppModel>();
        private GestureDetector mGeatureDetector;
        private Fragment mFragment;
        private ISharedPreferences mPref;
        private bool IndicationShowTop = false;


        public NewAppTutorialDialogFragment(Android.App.Activity ctx, Fragment fragment, ISharedPreferences pref, List<NewAppModel> list, bool mIndicationShowTop = false)
        {
            this.mContext = ctx;
            if (list != null && list.Count > 0)
            {
                this.NewAppTutorialList = list;
            }
            this.mFragment = fragment;
            this.mPref = pref;
            this.IndicationShowTop = mIndicationShowTop;

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
            View rootView = inflater.Inflate(Resource.Layout.NewAppTutorialLayout, container, false);

            try
            {
                pager = rootView.FindViewById<ViewPager>(Resource.Id.viewPager);
                indicator = rootView.FindViewById<LinearLayout>(Resource.Id.indicatorContainer);
                indicatorTopContainer = rootView.FindViewById<LinearLayout>(Resource.Id.indicatorTopContainer);
                swipeDoubleTapLayout = rootView.FindViewById<LinearLayout>(Resource.Id.swipeDoubleTapLayout);
                swipeTopDoubleTapLayout = rootView.FindViewById<LinearLayout>(Resource.Id.swipeTopDoubleTapLayout);
                txtDoubleTapDismiss = rootView.FindViewById<TextView>(Resource.Id.txtDoubleTapDismiss);
                txtTopDoubleTapDismiss = rootView.FindViewById<TextView>(Resource.Id.txtTopDoubleTapDismiss);
                TextViewUtils.SetMuseoSans300Typeface(txtDoubleTapDismiss, txtTopDoubleTapDismiss);
                TextViewUtils.SetTextSize12(txtDoubleTapDismiss, txtTopDoubleTapDismiss);

                if (NewAppTutorialList.Count > 1)
                {
                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        txtDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "swipeText"), FromHtmlOptions.ModeLegacy);
                        txtTopDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "swipeText"), FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "swipeText"));
                        txtTopDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "swipeText"));
                    }
                }
                else
                {
                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        txtDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "skip"), FromHtmlOptions.ModeLegacy);
                        txtTopDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "skip"), FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "skip"));
                        txtTopDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "skip"));
                    }
                //     txtDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedCommonLabel("tutorialSwipeTextNew"), FromHtmlOptions.ModeLegacy);
                //     txtTopDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedCommonLabel("tutorialSwipeTextNew"), FromHtmlOptions.ModeLegacy);
                // }
                // else
                // {
                //     txtDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedCommonLabel("tutorialSwipeTextNew"));
                //     txtTopDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedCommonLabel("tutorialSwipeTextNew"));
                }

               

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
                    else if (this.mFragment is RewardMenuFragment)
                    {
                        ((RewardMenuFragment)this.mFragment).StopScrolling();
                    }
                    else if (this.mFragment is WhatsNewMenuFragment)
                    {
                        ((WhatsNewMenuFragment)this.mFragment).StopScrolling();
                    }
                }
                else if (this.mContext is ManageAccessActivity)
                {
                    if (NewAppTutorialList.Count > 0)
                    {
                        int ItemCount = NewAppTutorialList[1].ItemCount;
                        if (ItemCount > 0)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                            if (((ManageAccessActivity)this.mContext).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((ManageAccessActivity)this.mContext).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    ((ManageAccessActivity)this.mContext).HomeMenuCustomScrolling(topHeight / 2);
                                }
                                else
                                {
                                    ((ManageAccessActivity)this.mContext).HomeMenuCustomScrolling(0);
                                }
                            }
                            else
                            {
                                ((ManageAccessActivity)this.mContext).HomeMenuCustomScrolling(0);
                            }
                        }
                        else
                        {
                            ((ManageAccessActivity)this.mContext).HomeMenuCustomScrolling(0);
                        }
                    }
                    else
                    {
                        ((ManageAccessActivity)this.mContext).HomeMenuCustomScrolling(0);
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
                            int middleHeight = ((SSMRMeterHistoryActivity)this.mContext).GetSMRTopViewHeight();
                            int checkPoint = (int)DPUtils.ConvertDPToPx(60f);
                            if (DisplayMode == "NONSMR")
                            {
                                checkPoint = (int)DPUtils.ConvertDPToPx(40f);
                            }

                            if (ItemCount == 1)
                            {
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
                        if (IndicationShowTop)
                        {
                            swipeTopDoubleTapLayout.Visibility = ViewStates.Visible;
                            swipeDoubleTapLayout.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            swipeTopDoubleTapLayout.Visibility = ViewStates.Gone;
                            swipeDoubleTapLayout.Visibility = ViewStates.Visible;
                        }
                        txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                        txtTopDoubleTapDismiss.Visibility = ViewStates.Visible;
                        indicator.Visibility = ViewStates.Visible;
                        indicatorTopContainer.Visibility = ViewStates.Visible;
                    }
                    else if(this.mContext is DashboardHomeActivity)
                    {
                        swipeTopDoubleTapLayout.Visibility = ViewStates.Gone;
                        swipeDoubleTapLayout.Visibility = ViewStates.Visible;

                        txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                        txtDoubleTapDismiss.Text = Utility.GetLocalizedLabel("Tutorial", "skip");
                        indicator.Visibility = ViewStates.Visible;
                        indicatorTopContainer.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                        txtTopDoubleTapDismiss.Visibility = ViewStates.Visible;
                        indicatorTopContainer.Visibility = ViewStates.Visible;
                        indicator.Visibility = ViewStates.Gone;

                        if (IndicationShowTop)
                        {
                            swipeTopDoubleTapLayout.Visibility = ViewStates.Visible;
                            swipeDoubleTapLayout.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            swipeTopDoubleTapLayout.Visibility = ViewStates.Gone;
                            swipeDoubleTapLayout.Visibility = ViewStates.Visible;
                        }
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
                                txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                                txtTopDoubleTapDismiss.Visibility = ViewStates.Gone;
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
                    else if (this.mFragment != null && this.mFragment is HomeMenuFragment)
                    {
                        if ((NewAppTutorialList.Count == 4 && !NewAppTutorialList[0].NeedHelpHide) || (NewAppTutorialList.Count == 3 && NewAppTutorialList[0].NeedHelpHide))
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    int middleHeight = (int)DPUtils.ConvertDPToPx(275f);
                                    int checkPoint = (int)DPUtils.ConvertDPToPx(230f);

                                    if (((topHeight / 2 + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                                    {
                                        txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                                        txtTopDoubleTapDismiss.Visibility = ViewStates.Gone;
                                    }
                                }
                            }
                        }
                    }
                    else if (this.mContext != null && this.mContext is ApplicationStatusLandingActivity)
                    {
                        if (NewAppTutorialList != null && NewAppTutorialList.Count > 1)
                        {
                            swipeTopDoubleTapLayout.Visibility = ViewStates.Gone;
                            swipeDoubleTapLayout.Visibility = ViewStates.Visible;
                        }
                    }

                    else if (this.mContext != null && this.mContext is ApplicationStatusDetailActivity)
                    {
                        if (NewAppTutorialList != null && NewAppTutorialList.Count > 1)
                        {

                            //RelativeLayout.LayoutParams parameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
                            //parameters.AddRule(LayoutRules.AlignParentTop);
                            //swipeDoubleTapLayout.LayoutParameters = parameters;


                            swipeTopDoubleTapLayout.Visibility = ViewStates.Visible;
                            swipeDoubleTapLayout.Visibility = ViewStates.Gone;
                        }
                    }



                    if (NewAppTutorialList.Count > 1)
                    {
                        for (int i = 0; i < NewAppTutorialList.Count; i++)
                        {
                            ImageView image = new ImageView(mContext);
                            image.Id = i;
                            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            layoutParams.RightMargin = 12;
                            layoutParams.LeftMargin = 12;
                            image.LayoutParameters = layoutParams;
                            if (i == 0)
                            {
                                image.SetImageResource(Resource.Drawable.white_circle_active);
                                txtDoubleTapDismiss.Visibility = TextViewUtils.IsLargeFonts ? ViewStates.Gone : ViewStates.Visible;
                                txtTopDoubleTapDismiss.Visibility = TextViewUtils.IsLargeFonts ? ViewStates.Gone : ViewStates.Visible;
                            }
                            else
                            {
                                image.SetImageResource(Resource.Drawable.white_circle);
                            }
                            indicator.AddView(image, i);

                            ImageView imageTop = new ImageView(mContext);
                            imageTop.Id = i;
                            LinearLayout.LayoutParams layoutTopParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            layoutTopParams.RightMargin = 12;
                            layoutTopParams.LeftMargin = 12;
                            imageTop.LayoutParameters = layoutTopParams;
                            if (i == 0)
                            {
                                imageTop.SetImageResource(Resource.Drawable.white_circle_active);
                            }
                            else
                            {
                                imageTop.SetImageResource(Resource.Drawable.white_circle);
                            }
                            indicatorTopContainer.AddView(imageTop, i);
                        }
                    }

                    adapter = new NewAppTutorialPagerAdapter(mContext, mFragment, mPref, this, NewAppTutorialList);
                    pager.Adapter = adapter;

                    if (NewAppTutorialList.Count > 1)
                    {
                        pager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) =>
                        {
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

                                ImageView selectedTopDot = (ImageView)indicatorTopContainer.GetChildAt(i);
                                if (e.Position == i)
                                {
                                    selectedTopDot.SetImageResource(Resource.Drawable.white_circle_active);
                                }
                                else
                                {
                                    selectedTopDot.SetImageResource(Resource.Drawable.white_circle);
                                }
                            }

                            if (this.mContext != null && this.mContext is ApplicationStatusLandingActivity)
                            {
                                if (e.Position == 0)
                                {
                                    swipeTopDoubleTapLayout.Visibility = ViewStates.Gone;
                                    swipeDoubleTapLayout.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    swipeTopDoubleTapLayout.Visibility = ViewStates.Visible;
                                    swipeDoubleTapLayout.Visibility = ViewStates.Gone;
                                }
                            }

                            if (e.Position == NewAppTutorialList.Count - 1)
                 {
                                txtDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "skip"));
                                txtTopDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "skip"));
                                //txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                                //txtTopDoubleTapDismiss.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                txtDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "swipeText"));
                                txtTopDoubleTapDismiss.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Tutorial", "swipeText"));

                                //txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                                //txtTopDoubleTapDismiss.Visibility = ViewStates.Visible;
                            }
                            if (this.mContext != null && this.mContext is ApplicationStatusLandingActivity)
                            {
                                txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                                txtTopDoubleTapDismiss.Visibility = ViewStates.Visible;
                            }
                            if (this.mContext != null && this.mContext is ApplicationStatusDetailActivity)
                            {
                                txtDoubleTapDismiss.Visibility = ViewStates.Visible;
                                txtTopDoubleTapDismiss.Visibility = ViewStates.Visible;
                            }
                            if (this.mFragment != null)
                            {
                                if (this.mFragment is HomeMenuFragment)
                                {
                                    if (NewAppTutorialList.Count > 0)
                                    {
                                        if (e.Position == NewAppTutorialList.Count - 1 && !NewAppTutorialList[0].NeedHelpHide)
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
                                                    if (NewAppTutorialList.Count == 4 && e.Position == 2)
                                                    {
                                                        var h1 = 65f;
                                                        topHeight = (int)DPUtils.ConvertDPToPx(h1);
                                                    }
                                                    int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                                    int halfScroll = topHeight / 2;

                                                    if (diffHeight < halfScroll)
                                                    {
                                                        ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(topHeight / 2);

                                                        if (NewAppTutorialList.Count == 4 && e.Position == 0)
                                                        {
                                                            int middleHeight = (int)DPUtils.ConvertDPToPx(275f);
                                                            int checkPoint = (int)DPUtils.ConvertDPToPx(230f);

                                                            if (((topHeight / 2 + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                                                            {
                                                                txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                                                                txtTopDoubleTapDismiss.Visibility = ViewStates.Gone;
                                                            }
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
                                                    int middleHeight = (int)DPUtils.ConvertDPToPx(208f);

                                                    if (ItemCount == 0)
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
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
                                                        middleHeight = (int)DPUtils.ConvertDPToPx(130f);
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
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
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
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
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                                        {
                                                            int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(20f);
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
                                                        txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                                                        txtTopDoubleTapDismiss.Visibility = ViewStates.Gone;
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
                                                    int middleHeight = (int)DPUtils.ConvertDPToPx(208f);

                                                    if (DisplayMode == "Extra")
                                                    {
                                                        topHeight = (int)DPUtils.ConvertDPToPx(405f);
                                                    }

                                                    if (ItemCount == 0)
                                                    {
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
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
                                                        middleHeight = (int)DPUtils.ConvertDPToPx(130f);
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
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
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
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
                                                        if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                                        {
                                                            int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(20f);
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
                                                    txtDoubleTapDismiss.Visibility = ViewStates.Gone;
                                                    txtTopDoubleTapDismiss.Visibility = ViewStates.Gone;
                                                }
                                                else
                                                {
                                                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                                                }
                                                txtDoubleTapDismiss.Visibility = TextViewUtils.IsLargeFonts ? ViewStates.Gone : ViewStates.Visible;
                                                txtTopDoubleTapDismiss.Visibility = TextViewUtils.IsLargeFonts ? ViewStates.Gone : ViewStates.Visible;
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
                    swipeTopDoubleTapLayout.Visibility = ViewStates.Gone;
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

            SetStyle(DialogFragment.StyleNormal, Android.Resource.Style.ThemeTranslucentNoTitleBar);
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
            AndroidX.Fragment.App.Fragment mFragment;
            Android.App.Activity mActivity;
            ISharedPreferences mPref;
            List<NewAppModel> NewAppTutorialList = new List<NewAppModel>();

            public DialogTapDetector(DialogFragment dialog, AndroidX.Fragment.App.Fragment fragment, Android.App.Activity activity, ISharedPreferences pref, List<NewAppModel> list)
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
            public override bool OnSingleTapUp(MotionEvent e)
            {
                this.mDialog.DismissAllowingStateLoss();
                NewAppTutorialUtils.CloseNewAppTutorial();
                if (this.mFragment != null)
                {
                    if (this.mFragment is HomeMenuFragment)
                    {
                        ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                        UserSessions.DoHomeTutorialShown(this.mPref);
                        ((HomeMenuFragment)this.mFragment).RestartHomeMenu();
                    }
                    else if (this.mFragment is ItemisedBillingMenuFragment)
                    {
                        ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                        if (NewAppTutorialList.Count == 2)
                        {
                            UserSessions.DoItemizedBillingRETutorialShown(this.mPref);
                        }
                        else
                        {
                            UserSessions.DoItemizedBillingNMSMTutorialShown(this.mPref);
                        }
                    }
                    else if (this.mFragment is DashboardChartFragment)
                    {
                        ((DashboardChartFragment)this.mFragment).DashboardCustomScrolling(0);
                        ((DashboardChartFragment)this.mFragment).ShowBottomSheet();
                        UserSessions.DoSMRDashboardTutorialShown(this.mPref);
                    }
                    else if (this.mFragment is RewardMenuFragment)
                    {
                        ((RewardMenuFragment)this.mFragment).StopScrolling();
                        UserSessions.DoRewardsShown(this.mPref);
                    }
                    else if (this.mFragment is WhatsNewMenuFragment)
                    {
                        ((WhatsNewMenuFragment)this.mFragment).StopScrolling();
                        UserSessions.DoWhatsNewShown(this.mPref);
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
                    else if (this.mActivity is RewardDetailActivity)
                    {
                        UserSessions.DoRewardsDetailShown(this.mPref);
                    }
                    else if (this.mActivity is ApplicationStatusLandingActivity)
                    {
                        UserSessions.DoApplicationStatusShown(this.mPref);
                    }
                    else if (this.mActivity is ApplicationStatusDetailActivity)
                    {
                        UserSessions.DoApplicationDetailShown(this.mPref);
                    }
                    else if (this.mActivity is DashboardHomeActivity)
                    {
                        UserSessions.DoManageAccessIconTutorialShown(this.mPref);
                        //((DashboardHomeActivity)mActivity).ShowCommercialDialog();
                    }
                    else if (this.mActivity is ManageAccessActivity)
                    {
                        UserSessions.DoManageAccessPageTutorialShown(this.mPref);
                    }
                }
                return true;
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
                        UserSessions.DoHomeTutorialShown(this.mPref);
                        ((HomeMenuFragment)this.mFragment).RestartHomeMenu();
                    }
                    else if (this.mFragment is ItemisedBillingMenuFragment)
                    {
                        ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                        if (NewAppTutorialList.Count == 2)
                        {
                            UserSessions.DoItemizedBillingRETutorialShown(this.mPref);
                        }
                        else
                        {
                            UserSessions.DoItemizedBillingNMSMTutorialShown(this.mPref);
                        }
                    }
                    else if (this.mFragment is DashboardChartFragment)
                    {
                        ((DashboardChartFragment)this.mFragment).DashboardCustomScrolling(0);
                        ((DashboardChartFragment)this.mFragment).ShowBottomSheet();
                        UserSessions.DoSMRDashboardTutorialShown(this.mPref);
                    }
                    else if (this.mFragment is RewardMenuFragment)
                    {
                        ((RewardMenuFragment)this.mFragment).StopScrolling();
                        UserSessions.DoRewardsShown(this.mPref);
                    }
                    else if (this.mFragment is WhatsNewMenuFragment)
                    {
                        ((WhatsNewMenuFragment)this.mFragment).StopScrolling();
                        UserSessions.DoWhatsNewShown(this.mPref);
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
                    else if (this.mActivity is RewardDetailActivity)
                    {
                        UserSessions.DoRewardsDetailShown(this.mPref);
                    }
                    else if (this.mActivity is DashboardHomeActivity)
                    {
                        UserSessions.DoManageAccessIconTutorialShown(this.mPref);
                    }
                    else if (this.mActivity is ManageAccessActivity)
                    {
                        UserSessions.DoManageAccessPageTutorialShown(this.mPref);
                    }
                }
                return true;
            }

            public override bool OnDoubleTapEvent(MotionEvent e)
            {
                return true;
            }
            public override bool OnSingleTapConfirmed(MotionEvent e)
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