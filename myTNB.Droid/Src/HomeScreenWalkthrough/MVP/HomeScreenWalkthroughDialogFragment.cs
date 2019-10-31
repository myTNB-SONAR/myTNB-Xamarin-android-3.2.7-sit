﻿using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.SSMR.SSMRMeterReadingTooltip.Adapter;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.HomeScreenWalkthrough.MVP
{
    public class HomeScreenWalkthroughDialogFragment : DialogFragment
    {

        private HomeMenuFragment mFragment;
        private HomeMenuContract mContract;
        private HomeMenuPresenter mPresenter;
        private ViewPager pager;
        private SSMRMeterReadingPagerAdapter adapter;
        private LinearLayout indicator;
        //private CheckBox dontShowAgainCheckbox;
        private LinearLayout txtBtnFirst;
        private TextView txtBtnLabel;
        private bool isSinglePhase = false;
        private List<SSMRMeterReadingModel> SSMRMeterReadingModelList = new List<SSMRMeterReadingModel>();

        public HomeScreenWalkthroughDialogFragment(HomeMenuFragment ctx, bool setIsSinglePhase, List<SSMRMeterReadingModel> list)
        {
            this.mFragment = ctx;
            this.isSinglePhase = setIsSinglePhase;
            if (list != null && list.Count > 0)
            {
                this.SSMRMeterReadingModelList = list;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.SetCancelable(false);
            Dialog.SetCanceledOnTouchOutside(false);
            WindowManagerLayoutParams wlp = Dialog.Window.Attributes;
            wlp.Gravity = GravityFlags.Center;
            wlp.Width = ViewGroup.LayoutParams.MatchParent;
            wlp.Height = ViewGroup.LayoutParams.WrapContent;
            Dialog.Window.Attributes = wlp;

            View rootView = inflater.Inflate(Resource.Layout.CustomDialogWithImgViewPageOneButtonLayout, container, false);

            try
            {
                pager = rootView.FindViewById<ViewPager>(Resource.Id.onBoardingSMRViewPager);
                indicator = rootView.FindViewById<LinearLayout>(Resource.Id.indicatorContainer);
                //dontShowAgainCheckbox = rootView.FindViewById<CheckBox>(Resource.Id.dontShowAgainCheckbox);
                txtBtnFirst = rootView.FindViewById<LinearLayout>(Resource.Id.txtBtnFirst);
                txtBtnLabel = rootView.FindViewById<TextView>(Resource.Id.txtBtnLabel);

                TextViewUtils.SetMuseoSans500Typeface(txtBtnLabel);

                if (SSMRMeterReadingModelList.Count > 0)
                {
                    indicator.Visibility = ViewStates.Visible;
                    for (int i = 0; i < SSMRMeterReadingModelList.Count; i++)
                    {
                        ImageView image = new ImageView(mFragment.Context);
                        image.Id = i;
                        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                        layoutParams.RightMargin = 8;
                        layoutParams.LeftMargin = 8;
                        image.LayoutParameters = layoutParams;
                        if (i == 0)
                        {
                            image.SetImageResource(Resource.Drawable.onboarding_circle_active);
                        }
                        else
                        {
                            image.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                        }
                        indicator.AddView(image, i);
                    }

                    if (SSMRMeterReadingModelList.Count <= 1)
                    {
                        indicator.Visibility = ViewStates.Gone;
                        LinearLayout.LayoutParams pagerParam = pager.LayoutParameters as LinearLayout.LayoutParams;
                        pagerParam.Height = (int)DPUtils.ConvertDPToPx(300f);
                        txtBtnLabel.Text = this.Activity.GetString(Resource.String.tooltip_btnLabel);
                    }

                    adapter = new SSMRMeterReadingPagerAdapter(mFragment.Context, SSMRMeterReadingModelList);
                    pager.Adapter = adapter;

                    pager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) => {
                        for (int i = 0; i < SSMRMeterReadingModelList.Count; i++)
                        {
                            ImageView selectedDot = (ImageView)indicator.GetChildAt(i);
                            if (e.Position == i)
                            {
                                selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                            }
                            else
                            {
                                selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                            }
                        }
                    };
                }
                else
                {
                    indicator.Visibility = ViewStates.Gone;
                    LinearLayout.LayoutParams pagerParam = pager.LayoutParameters as LinearLayout.LayoutParams;
                    pagerParam.Height = (int)DPUtils.ConvertDPToPx(300f);
                }

                txtBtnLabel.Text = this.Activity.GetString(Resource.String.tooltip_btnLabel);

                txtBtnFirst.Click += GotIt_Click;

                //if (isSinglePhase)
                //{
                //    dontShowAgainCheckbox.Checked = MyTNBAccountManagement.GetInstance().GetSMRMeterReadingOnePhaseOnboardingShown();
                //}
                //else
                //{
                //    dontShowAgainCheckbox.Checked = MyTNBAccountManagement.GetInstance().GetSMRMeterReadingThreePhaseOnboardingShown();
                //}


            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
            return rootView;
        }

        private void GotIt_Click(object sender, EventArgs e)
        {
            //if (isSinglePhase)
            //{
            //    MyTNBAccountManagement.GetInstance().UpdateIsSMRMeterReadingOnePhaseOnboardingShown(dontShowAgainCheckbox.Checked);
            //}
            //else
            //{
            //    MyTNBAccountManagement.GetInstance().UpdateIsSMRMeterReadingThreePhaseOnboardingShown(dontShowAgainCheckbox.Checked);
            //}
            this.Dismiss();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

    }
}