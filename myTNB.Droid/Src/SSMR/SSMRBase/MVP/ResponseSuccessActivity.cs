
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
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.Api;
using myTNB.AndroidApp.Src.SSMRMeterHistory.MVP;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using static myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB.AndroidApp.Src.SSMR.SSMRBase.MVP
{
    [Activity(Label = "ResponseSuccessActivity", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.BillRelated")]
    public class ResponseSuccessActivity : BaseAppCompatActivity
    {
        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtMessageInfo)]
        TextView txtMessageInfo;

        [BindView(Resource.Id.btnTrackApplication)]
        Button btnTrackApplication;

        [BindView(Resource.Id.btnBackToHomeSuccess)]
        Button btnBackToHomeSuccess;


        public override int ResourceId()
        {
            return Resource.Layout.SSMRResponseSuccessLayout;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, btnTrackApplication, btnBackToHomeSuccess);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfo);
            TextViewUtils.SetTextSize12(txtMessageInfo);
            TextViewUtils.SetTextSize16(txtTitleInfo, btnTrackApplication, btnBackToHomeSuccess);

            btnBackToHomeSuccess.Text = "Back to My Usage";
            btnTrackApplication.Text = "View Reading History";

            Bundle extras = Intent.Extras;

            if (extras != null && extras.ContainsKey("SUBMIT_RESULT"))
            {
                SMRSubmitResponseData response = JsonConvert.DeserializeObject<SMRSubmitResponseData>(extras.GetString("SUBMIT_RESULT"));
                txtTitleInfo.Text = response.DisplayTitle;
                txtMessageInfo.Text = response.DisplayMessage;
            }

            btnBackToHomeSuccess.Text = Utility.GetLocalizedLabel("Status", "ssmrBackToUsage");
            btnTrackApplication.Text = Utility.GetLocalizedLabel("Status", "ssmrViewReadHistory");
        }

        [OnClick(Resource.Id.btnBackToHomeSuccess)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(DashboardIntent);
            }
        }

        [OnClick(Resource.Id.btnTrackApplication)]
        void OnTryAgain(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (SMRPopUpUtils.GetFromUsageFlag())
                {
                    SMRPopUpUtils.SetFromUsageSubmitSuccessfulFlag(true);
                }
                Intent intent = new Intent(this, typeof(SSMRMeterHistoryActivity));
                StartActivity(intent);
                Finish();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}
