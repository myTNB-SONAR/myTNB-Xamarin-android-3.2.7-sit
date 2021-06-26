
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

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "SubmiSMRAccountFailedActivity", Theme = "@style/Theme.Dashboard")]
    public class SubmiSMRAccountFailedActivity : BaseToolbarAppCompatActivity
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
            return Resource.Layout.SubmitSMRAccountFailedView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            // Create your application here
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfoError);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfoError);
            TextViewUtils.SetTextSize12(txtMessageInfoError);
            TextViewUtils.SetTextSize16(btnBackToHomeFailed, btnTryAgainFailed, txtTitleInfoError);
        }

        [OnClick(Resource.Id.btnBackToHomeFailed)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent intent = new Intent(this, typeof(DashboardHomeActivity));
                StartActivity(intent);
            }
        }

        [OnClick(Resource.Id.btnTryAgainFailed)]
        void OnTryAgain(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
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
