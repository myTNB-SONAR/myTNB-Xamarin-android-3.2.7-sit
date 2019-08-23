
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB_Android.Src.SSMR.SSMRBase.MVP
{
    [Activity(Label = "ResponseFailedActivity", Theme = "@style/Theme.BillRelated")]
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
        }

        [OnClick(Resource.Id.btnBackToHomeFailed)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        [OnClick(Resource.Id.btnTryAgainFailed)]
        void OnTryAgain(object sender, EventArgs eventArgs)
        {
            SetResult(Result.Canceled);
            Finish();
        }
    }
}
