
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
    [Activity(Label = "TerminateSMRAccountFailedActivity",
        ScreenOrientation = ScreenOrientation.Portrait,
        Theme = "@style/Theme.BillRelated")]
    public class TerminateSMRAccountFailedActivity : BaseAppCompatActivity
    {
        [BindView(Resource.Id.txtTitleInfoError)]
        TextView txtTitleInfoError;

        [BindView(Resource.Id.txtMessageInfoError)]
        TextView txtMessageInfoError;


        [BindView(Resource.Id.btnBackToHomeFailed)]
        Button btnBackToHomeFailed;

        [BindView(Resource.Id.btnTryAgainFailed)]
        Button btnTryAgainFailed;

        private AccountData selectedAccount;

        private string SMR_ACTION = "";


        public override int ResourceId()
        {
            return Resource.Layout.TerminateSMRAccountFailedView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfoError, btnBackToHomeFailed, btnTryAgainFailed);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfoError);

            btnBackToHomeFailed.TextSize = TextViewUtils.GetFontSize(16f);
            btnTryAgainFailed.TextSize = TextViewUtils.GetFontSize(16f);
            txtTitleInfoError.TextSize = TextViewUtils.GetFontSize(16f);
            txtMessageInfoError.TextSize = TextViewUtils.GetFontSize(12f);

            btnBackToHomeFailed.Text = Utility.GetLocalizedLabel("Common", "backToHome");
            btnTryAgainFailed.Text = Utility.GetLocalizedLabel("Common", "tryAgain");

            Bundle extras = Intent.Extras;

            if (extras != null && extras.ContainsKey("SUBMIT_RESULT"))
            {
                SMRregistrationSubmitResponse response = JsonConvert.DeserializeObject<SMRregistrationSubmitResponse>(extras.GetString("SUBMIT_RESULT"));
                if (response != null && response.Data != null && response.Data.DisplayTitle != null)
                {
                    txtTitleInfoError.Text = response.Data.DisplayTitle;
                }
                else
                {
                    txtTitleInfoError.Text = Utility.GetLocalizedLabel("Status", "ssmrDiscontinueFailTitle");
                }
                if (response != null && response.Data != null && response.Data.DisplayMessage != null)
                {
                    txtMessageInfoError.Text = response.Data.DisplayMessage;
                }
                else
                {
                    txtMessageInfoError.Text = Utility.GetLocalizedLabel("Status", "ssmrDiscontinueFailMessage");
                }
            }
            else
            {
                txtTitleInfoError.Text = Utility.GetLocalizedLabel("Status", "ssmrDiscontinueFailTitle");
                txtMessageInfoError.Text = Utility.GetLocalizedLabel("Status", "ssmrDiscontinueFailMessage");
            }

            if (extras != null && extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
            }

            if (extras != null && extras.ContainsKey("SMR_ACTION"))
            {
                SMR_ACTION = extras.GetString("SMR_ACTION");
                if (!string.IsNullOrEmpty(SMR_ACTION))
                {
                    if (SMR_ACTION == Constants.SMR_ENABLE_FLAG)
                    {
                        btnBackToHomeFailed.Text = Utility.GetLocalizedLabel("Common", "backToHome");
                        btnTryAgainFailed.Text = Utility.GetLocalizedLabel("Common", "tryAgain");
                    }
                    else if (SMR_ACTION == Constants.SMR_DISABLE_FLAG)
                    {
                        btnBackToHomeFailed.Text = Utility.GetLocalizedLabel("Status", "ssmrBackToReadingHistory");
                        btnTryAgainFailed.Text = Utility.GetLocalizedLabel("Common", "tryAgain");
                    }
                }
            }
        }

        [OnClick(Resource.Id.btnBackToHomeFailed)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (SMR_ACTION == Constants.SMR_ENABLE_FLAG)
                {
                    Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                    DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(DashboardIntent);
                }
                else if (SMR_ACTION == Constants.SMR_DISABLE_FLAG)
                {
                    if (selectedAccount != null)
                    {
                        CustomerBillingAccount.RemoveSelected();
                        CustomerBillingAccount.SetSelected(selectedAccount.AccountNum);
                    }
                    SetResult(Result.Ok);
                    Finish();
                }
                else
                {
                    Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                    DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(DashboardIntent);
                }
            }
        }

        [OnClick(Resource.Id.btnTryAgainFailed)]
        void OnTryAgain(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                OnBackPressed();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                if (SMR_ACTION == Constants.SMR_ENABLE_FLAG)
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "Apply SMR Failed");
                }
                else if (SMR_ACTION == Constants.SMR_DISABLE_FLAG)
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "SMR Termination Failed");
                }
                else
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "Apply SMR Failed");
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
