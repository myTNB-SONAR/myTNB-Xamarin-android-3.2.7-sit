
using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;

namespace myTNB_Android.Src.myTNBMenu.Activity
{
    [Activity(Label = ""
        ,ScreenOrientation = ScreenOrientation.Portrait
        ,Theme = "@style/Theme.Dashboard")]
    public class DashboardHomeActivity : BaseToolbarAppCompatActivity
    {
        public override int ResourceId()
        {
            return Resource.Layout.DashboardHomeView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return false;
        }

        public override bool ShowBackArrowIndicator()
        {
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
        }
    }
}
