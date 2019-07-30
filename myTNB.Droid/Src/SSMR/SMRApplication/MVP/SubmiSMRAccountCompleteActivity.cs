
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
using myTNB_Android.Src.Utils;

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

        [BindView(Resource.Id.btnTryAgain)]
        Button btnTryAgain;

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

            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfo,refNumberLabel,appliedOnDateLabel,refNumberValue,appliedOnDateValue);
        }
    }
}
