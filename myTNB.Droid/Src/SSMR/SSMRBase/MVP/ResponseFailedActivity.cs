
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
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.myTNBMenu.Activity;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB.Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB.Android.Src.SSMR.SSMRBase.MVP
{
    [Activity(Label = "ResponseFailedActivity", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.BillRelated")]
    public class ResponseFailedActivity : BaseAppCompatActivity
    {

        [BindView(Resource.Id.txtTitleInfoError)]
        TextView txtTitleInfoError;

        [BindView(Resource.Id.txtMessageInfoError)]
        TextView txtMessageInfoError;


        [BindView(Resource.Id.btnBackToHomeFailed)]
        Button btnBackToHomeFailed;

        [BindView(Resource.Id.btnTryAgainFailed)]
        Button btnTryAgainFailed;

        public override int ResourceId()
        {
            return Resource.Layout.ResponseFailedLayout;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfoError, btnBackToHomeFailed, btnTryAgainFailed);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfoError);
            TextViewUtils.SetTextSize12(txtMessageInfoError);
            TextViewUtils.SetTextSize16(btnBackToHomeFailed, btnTryAgainFailed, txtTitleInfoError, btnTryAgainFailed);
            btnBackToHomeFailed.Text = "Back to My Usage";
            btnTryAgainFailed.Text = "Try Again";

            Bundle extras = Intent.Extras;

            if (extras != null && extras.ContainsKey("SUBMIT_RESULT"))
            {
                SMRSubmitResponseData response = JsonConvert.DeserializeObject<SMRSubmitResponseData>(extras.GetString("SUBMIT_RESULT"));
                if (response != null && response.DisplayTitle != null)
                {
                    txtTitleInfoError.Text = response.DisplayTitle;
                }
                else
                {
                    txtTitleInfoError.Text = "Please Try Again";
                }
                if (response != null && response.DisplayMessage != null)
                {
                    txtMessageInfoError.Text = response.DisplayMessage;
                }
                else
                {
                    txtMessageInfoError.Text = "It looks like we can't process your application at the moment.";
                }
            }
            else
            {
                txtTitleInfoError.Text = "Please Try Again";
                txtMessageInfoError.Text = "It looks like we can't process your application at the moment.";
            }

            btnBackToHomeFailed.Text = Utility.GetLocalizedLabel("Status", "ssmrBackToUsage");
            btnTryAgainFailed.Text = Utility.GetLocalizedLabel("Common", "tryAgain");
        }

        [OnClick(Resource.Id.btnBackToHomeFailed)]
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

        [OnClick(Resource.Id.btnTryAgainFailed)]
        void OnTryAgain(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                SetResult(Result.Canceled);
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
