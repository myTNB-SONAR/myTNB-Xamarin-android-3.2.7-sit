
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
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    [Activity(Label = "TerminateSMRAccountSuccessActivity", Theme = "@style/Theme.BillRelated")]
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

            btnBackToHomeSuccess.Text = "Back to My Usage";
            btnTrackApplication.Text = "Track Application";

            Bundle extras = Intent.Extras;

            if (extras != null && extras.ContainsKey("SUBMIT_RESULT"))
            {
                SMRregistrationSubmitResponse response = JsonConvert.DeserializeObject<SMRregistrationSubmitResponse>(extras.GetString("SUBMIT_RESULT"));
                txtTitleInfo.Text = response.Data.DisplayTitle;
                txtMessageInfo.Text = response.Data.DisplayMessage;

                refNumberValue.Text = response.Data.AccountDetailsData.ApplicationID;
                appliedOnDateValue.Text = response.Data.AccountDetailsData.AppliedOn;
            }
        }

        [OnClick(Resource.Id.btnBackToHomeSuccess)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            SetResult(Result.Ok);
            Finish();
        }

        [OnClick(Resource.Id.btnTrackApplication)]
        void OnTryAgain(object sender, EventArgs eventArgs)
        {
            SetResult(Result.Canceled);
            Finish();
        }
    }
}
