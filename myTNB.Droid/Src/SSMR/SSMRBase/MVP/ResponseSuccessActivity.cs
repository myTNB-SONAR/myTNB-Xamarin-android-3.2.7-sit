﻿
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
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB_Android.Src.SSMR.SSMRBase.MVP
{
    [Activity(Label = "ResponseSuccessActivity", Theme = "@style/Theme.BillRelated")]
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

            btnBackToHomeSuccess.Text = "Back to My Usage";
            btnTrackApplication.Text = "View Reading History";

            Bundle extras = Intent.Extras;

            if (extras != null && extras.ContainsKey("SUBMIT_RESULT"))
            {
                SMRSubmitResponseData response = JsonConvert.DeserializeObject<SMRSubmitResponseData>(extras.GetString("SUBMIT_RESULT"));
                txtTitleInfo.Text = response.DisplayTitle;
                txtMessageInfo.Text = response.DisplayMessage;
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
