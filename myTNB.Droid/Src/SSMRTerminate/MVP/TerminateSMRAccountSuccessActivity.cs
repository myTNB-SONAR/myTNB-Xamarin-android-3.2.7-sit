
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    [Activity(Label = "TerminateSMRAccountSuccessActivity",
        ScreenOrientation = ScreenOrientation.Portrait,
        Theme = "@style/Theme.BillRelated")]
    public class TerminateSMRAccountSuccessActivity : BaseAppCompatActivity
    {
        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtMessageInfo)]
        TextView txtMessageInfo;

        [BindView(Resource.Id.refNumberLabel)]
        TextView refNumberLabel;

        [BindView(Resource.Id.appliedOnDateLabel)]
        TextView appliedOnDateLabel;

        [BindView(Resource.Id.refNumberValue)]
        TextView refNumberValue;

        [BindView(Resource.Id.appliedOnDateValue)]
        TextView appliedOnDateValue;

        [BindView(Resource.Id.btnTrackApplication)]
        Button btnTrackApplication;

        [BindView(Resource.Id.btnBackToHomeSuccess)]
        Button btnBackToHomeSuccess;

        private string SMR_ACTION = "";

        private AccountData selectedAccount;

        public override int ResourceId()
        {
            return Resource.Layout.TerminateSMRAccountSuccessView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, btnTrackApplication, btnBackToHomeSuccess);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfo, refNumberLabel, appliedOnDateLabel, refNumberValue, appliedOnDateValue);

            txtTitleInfo.TextSize = TextViewUtils.GetFontSize(16f);
            txtMessageInfo.TextSize = TextViewUtils.GetFontSize(12f);
            refNumberLabel.TextSize = TextViewUtils.GetFontSize(10f);
            appliedOnDateLabel.TextSize = TextViewUtils.GetFontSize(10f);
            refNumberValue.TextSize = TextViewUtils.GetFontSize(14f);
            appliedOnDateValue.TextSize = TextViewUtils.GetFontSize(14f);
            btnTrackApplication.TextSize = TextViewUtils.GetFontSize(16f);
            btnBackToHomeSuccess.TextSize = TextViewUtils.GetFontSize(16f);

            txtTitleInfo.Text = Utility.GetLocalizedLabel("Status", "ssmrApplySuccessTitle");
            refNumberLabel.Text = Utility.GetLocalizedLabel("Status", "ssmrApplyReferenceTitle").ToUpper();
            appliedOnDateLabel.Text = Utility.GetLocalizedLabel("Status", "ssmrApplyDateTitle").ToUpper();
            btnBackToHomeSuccess.Text = Utility.GetLocalizedLabel("Status", "ssmrBackToUsage");
            btnTrackApplication.Text = Utility.GetLocalizedLabel("Status", "ssmrTrackApplication");

            Bundle extras = Intent.Extras;

            if (extras != null && extras.ContainsKey("SUBMIT_RESULT"))
            {
                SMRregistrationSubmitResponse response = JsonConvert.DeserializeObject<SMRregistrationSubmitResponse>(extras.GetString("SUBMIT_RESULT"));
                txtTitleInfo.Text = response.Data.DisplayTitle;
                txtMessageInfo.Text = response.Data.DisplayMessage;

                refNumberValue.Text = response.Data.AccountDetailsData.ServiceReqNo;
                appliedOnDateValue.Text = response.Data.AccountDetailsData.AppliedOn;
            }

            btnBackToHomeSuccess.Visibility = ViewStates.Gone;
            btnTrackApplication.Text = Utility.GetLocalizedLabel("Common", "backToHome");

            if (extras != null && extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
            }

            if (extras != null && extras.ContainsKey("SMR_ACTION"))
            {
                SMR_ACTION = extras.GetString("SMR_ACTION");
            }
        }

        [OnClick(Resource.Id.btnBackToHomeSuccess)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            if (selectedAccount != null)
            {
                CustomerBillingAccount.RemoveSelected();
                CustomerBillingAccount.SetSelected(selectedAccount.AccountNum);
            }
            SetResult(Result.Ok);
            Finish();
        }

        [OnClick(Resource.Id.btnTrackApplication)]
        void OnTryAgain(object sender, EventArgs eventArgs)
        {
            // SetResult(Result.Canceled);
            // Finish();
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                if (SMR_ACTION == Constants.SMR_ENABLE_FLAG)
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "Apply SMR Success");
                }
                else if (SMR_ACTION == Constants.SMR_DISABLE_FLAG)
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "SMR Termination Success");
                }
                else
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "Apply SMR Success");
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }
    }
}
