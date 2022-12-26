
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
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyHome.Activity
{
    [Activity(Label = "MyHomeMicrositeActivity"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Dashboard")]
    public class MyHomeMicrositeActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.txtMyHomeHeaderTitle)]
        TextView txtMyHomeHeaderTitle;
        
        [BindView(Resource.Id.txtMyHomeInfoTitle)]
        TextView txtMyHomeInfoTitle;

        [BindView(Resource.Id.txtViewid_FeedbackNewIC)]
        TextView txtViewid_FeedbackNewIC;

        [BindView(Resource.Id.textviewid_subContent_FeedbackNewIC)]
        TextView textviewid_subContent_FeedbackNewIC;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel("ConnectMyPremise", "title"));

            TextViewUtils.SetMuseoSans500Typeface(txtMyHomeInfoTitle, txtViewid_FeedbackNewIC);
            TextViewUtils.SetMuseoSans300Typeface(txtMyHomeHeaderTitle, textviewid_subContent_FeedbackNewIC);

            TextViewUtils.SetTextSize12(textviewid_subContent_FeedbackNewIC);
            TextViewUtils.SetTextSize14(txtMyHomeHeaderTitle, txtViewid_FeedbackNewIC);
            TextViewUtils.SetTextSize16(txtMyHomeInfoTitle);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.MyHomeMicrositeView;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }
    }
}

