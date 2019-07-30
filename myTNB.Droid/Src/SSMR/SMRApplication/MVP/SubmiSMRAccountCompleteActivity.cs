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
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "SubmiSMRAccountCompleteActivity", Theme = "@style/Theme.Dashboard")]
    public class SubmiSMRAccountCompleteActivity : BaseToolbarAppCompatActivity
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

        public override int ResourceId()
        {
            return Resource.Layout.SubmitSMRAccountSuccessView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            string jsonResponse = Intent.GetStringExtra("SUBMIT_RESULT");
            SMRregistrationSubmitResponse response = JsonConvert.DeserializeObject<SMRregistrationSubmitResponse>(jsonResponse);
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfo,refNumberLabel,appliedOnDateLabel,refNumberValue,appliedOnDateValue);

            txtTitleInfo.Text = response.Data.DisplayTitle;
            txtMessageInfo.Text = response.Data.DisplayMessage;

            refNumberValue.Text = response.Data.AccountDetailsData.ApplicationID;
            appliedOnDateValue.Text = response.Data.AccountDetailsData.AppliedOn;
        }

        [OnClick(Resource.Id.btnBackToHomeSuccess)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(DashboardHomeActivity));
            StartActivity(intent);
        }

        [OnClick(Resource.Id.btnTrackApplication)]
        void OnTrackApplication(object sender, EventArgs eventArgs)
        {
            Finish();
        }
    }
}
