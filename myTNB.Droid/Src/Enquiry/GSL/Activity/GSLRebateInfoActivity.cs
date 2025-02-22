﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.CompoundView;
using myTNB.AndroidApp.Src.Enquiry.GSL.Fragment;
using myTNB.AndroidApp.Src.Enquiry.GSL.MVP;
using myTNB.AndroidApp.Src.Feedback_Prelogin_NewIC.Activity;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using static myTNB.AndroidApp.Src.CompoundView.ExpandableTextViewComponent;

namespace myTNB.AndroidApp.Src.Enquiry.GSL.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.AdjustPan,
        Theme = "@style/Theme.Enquiry")]
    public class GSLRebateInfoActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.gslInfoViewList)]
        LinearLayout gslInfoViewList;

        [BindView(Resource.Id.gslInfoBtnBack)]
        Button gslInfoBtnBack;

        [BindView(Resource.Id.gslInfoBtnProceed)]
        Button gslInfoBtnProceed;

        List<GSLInfo> gslInfoContentList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetUpViews();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.GSLRebateInfoView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        private void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.GSL, LanguageConstants.Gsl.TITLE));

            TextViewUtils.SetMuseoSans500Typeface(gslInfoBtnBack, gslInfoBtnProceed);
            TextViewUtils.SetTextSize16(gslInfoBtnBack, gslInfoBtnProceed);

            gslInfoBtnBack.Text = Utility.GetLocalizedLabel(LanguageConstants.GSL, LanguageConstants.Gsl.BACK_BTN_TITLE);
            gslInfoBtnProceed.Text = Utility.GetLocalizedLabel(LanguageConstants.GSL, LanguageConstants.Gsl.PROCEED_BTN_TITLE);

            gslInfoContentList = new List<GSLInfo>(GetGSLInfoFromSelector());
            RenderContent();
        }

        private void RenderContent()
        {
            try
            {
                if (gslInfoContentList.Count > 0)
                {
                    gslInfoContentList.ForEach(info =>
                    {
                        GSLInfoItemListComponent infoItemList = new GSLInfoItemListComponent(this, this);
                        infoItemList.SetTitle(info.title);
                        infoItemList.SetDescription(info.description);

                        if (info.expandCollapseList != null && info.expandCollapseList.Count > 0)
                        {
                            info.expandCollapseList.ForEach(item =>
                            {
                                ExpandableTextViewComponent component = new ExpandableTextViewComponent(this, this);
                                component.SetExpandableType(ExpandableTextViewType.GSL_INFO);
                                component.SetExpandableGSLTitle(item.title);
                                component.SetExpandableGSLContent(item.description);
                                component.RequestLayout();
                                infoItemList.AddContent(component);
                            });
                        }
                        gslInfoViewList.AddView(infoItemList);
                    });
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private List<GSLInfo> GetGSLInfoFromSelector()
        {
            List<GSLInfo> gslInfoList = new List<GSLInfo>();
            var localContent = LanguageManager.Instance.GetSelectorsByPage<GSLInfo>(EnquiryConstants.GSL_INFO_SELECTOR);
            if (localContent.ContainsKey(EnquiryConstants.GSL_INFO))
            {
                gslInfoList = localContent[EnquiryConstants.GSL_INFO];
            }
            return gslInfoList;
        }

        [OnClick(Resource.Id.gslInfoBtnBack)]
        public void ButtonBackOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Finish();
            }
        }

        [OnClick(Resource.Id.gslInfoBtnProceed)]
        public void ButtonProceedOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                var submitEnquiryIntent = new Intent(this, typeof(FeedbackPreloginNewICActivity));
                StartActivity(submitEnquiryIntent);
            }
        }
    }
}
