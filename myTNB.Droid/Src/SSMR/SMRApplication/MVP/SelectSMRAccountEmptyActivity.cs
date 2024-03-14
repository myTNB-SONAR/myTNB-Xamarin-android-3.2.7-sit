
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
using myTNB.Android.Src.AddAccount.Adapter;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.MultipleAccountPayment.Adapter;
using myTNB.Android.Src.MultipleAccountPayment.Model;
using myTNB.Android.Src.SSMR.SMRApplication.Adapter;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "Self Meter Reading"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.Dashboard")]
    public class SelectSMRAccountEmptyActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.smr_submitted_title)]
        TextView SMRMainTitle;

        public override int ResourceId()
        {
            return Resource.Layout.SelectSMRAccountEmptyLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
                // Create your application here
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                TextViewUtils.SetMuseoSans300Typeface(SMRMainTitle);
                TextViewUtils.SetTextSize14(SMRMainTitle);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}