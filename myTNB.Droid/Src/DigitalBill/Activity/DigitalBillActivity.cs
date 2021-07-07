using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DigitalBill.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.DigitalBill.Activity
{
    [Activity(Label = "@string/terms_conditions_activity_title",
        ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.TnC")]
    public class DigitalBillActivity : BaseActivityCustom, DigitalBillContract.IView
    {
        private DigitalBillPresenter mPresenter;
        private DigitalBillContract.IUserActionsListener userActionsListener;

        //TextView txtTitle;
        //TextView txtVersion;
        //TextView txtTnCHtml;

        WebView tncWebView;

        const string PAGE_ID = "TnC";

        private string mSavedTimeStamp = "0000000";

        [BindView(Resource.Id.progressBar)]
        ProgressBar progressBar;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DigitalBillView;
        }

        public void SetPresenter(DigitalBillContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowDigitalBill(bool success)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        SetDefaultData();
                    }
                    catch (System.Exception er)
                    {
                        Utility.LoggingNonFatalError(er);
                    }
                });
            }
            catch (Exception e)
            {
                progressBar.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                mPresenter = new DigitalBillPresenter(this);

                tncWebView = FindViewById<WebView>(Resource.Id.tncWebView);
                tncWebView.Settings.JavaScriptEnabled = (true);

                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                ShowDigitalBill(true);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetDefaultData()
        {
            try
            {
                string HTMLText = "<html>" + "<body><b>MicroSite</b><br/><br/></body>" +
                               "</html>";
                tncWebView.LoadDataWithBaseURL("", HTMLText, "text/html", "UTF-8", "");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressBar()
        {
            try
            {
                progressBar.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Terms And Conditions");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        //  TODO: AndroidX Temporary Fix for Android 5,5.1 
        //  TODO: AndroidX Due to this: https://github.com/xamarin/AndroidX/issues/131
        public override AssetManager Assets =>
            (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop && Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            ? Resources.Assets : base.Assets;

    }
}