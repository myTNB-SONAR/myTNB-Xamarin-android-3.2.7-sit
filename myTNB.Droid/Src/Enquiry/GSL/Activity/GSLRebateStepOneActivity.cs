using System;
using Android.App;
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
{
    [Activity(Label = "GSL Rebate Step One", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class GSLRebateStepOneActivity : BaseToolbarAppCompatActivity
    {
        const string PAGE_ID = "GSLRebeateFirstStep";

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.UpdatePersonalDetailStepOneView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }
    }
}
