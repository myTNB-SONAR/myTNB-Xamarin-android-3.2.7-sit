using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Net.Http;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DigitalBill.MVP;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
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
        private static Snackbar mErrorMessageSnackBar;

        private static FrameLayout mainView;

        WebView tncWebView;

        const string PAGE_ID = "ManageDigitalBillLanding";

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
            //mainView = rootView.FindViewById<FrameLayout>(Resource.Id.rootView);

            try
            {
                mPresenter = new DigitalBillPresenter(this);

                tncWebView = FindViewById<WebView>(Resource.Id.tncWebView);
                tncWebView.Settings.JavaScriptEnabled = (true);
                tncWebView.SetWebViewClient(new MyTNBWebViewClient(this));
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
                    "<input type='button' value='Back to DBR' class='remove_this' onclick='mytnbapp://action=startDigitalBilling'>"+
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
        public void OnManageBillDelivery()
        {
            ShowProgressDialog();
            try
            {
                CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
                AccountData selectedAccountData = AccountData.Copy(customerAccount, true);
                Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
                intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
                StartActivity(intent);
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            HideProgressDialog();
        }
        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public class MyTNBWebViewClient : WebViewClient
        {
            public DigitalBillActivity mActivity;
            public ProgressBar progressBar;
            private bool isRedirected = false;

            public MyTNBWebViewClient(DigitalBillActivity mActivity)
            {
                this.mActivity = mActivity;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {

                    if (url.ToLower().Contains("mytnbapp://action=startDigitalBilling"))
                    {
                        mActivity.OnManageBillDelivery();
                        
                    }
                    else
                    {
                        view.LoadUrl(url);
                    }
                }
                return true;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                try
                {
                   // if (ConnectionUtils.HasInternetConnection(mActivity))
                    {
                        base.OnPageStarted(view, url, favicon);
                        //progressBar.Visibility = ViewStates.Visible;
                    }
                   // else
                    {
                       // ShowErrorMessage(url);
                    }


                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                try
                {
                   
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override bool OnRenderProcessGone(WebView view, RenderProcessGoneDetail detail)
            {
                return true;
            }

            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                try
                {
                    string message = "Please check your internet connection.";
                    if (ConnectionUtils.HasInternetConnection(mActivity))
                    {
                        switch (errorCode)
                        {
                            case ClientError.FileNotFound:
                                message = "File Not Found."; break;
                            case ClientError.Authentication:
                                message = "Authetication Error."; break;
                            case ClientError.FailedSslHandshake:
                                message = "SSL Handshake Failed."; break;
                            case ClientError.Unknown:
                                message = "Unkown Error."; break;
                        }
                        ShowErrorMessage(failingUrl);
                    }
                    else
                    {
                        ShowErrorMessage(failingUrl);
                    }

                   // if (!ConnectionUtils.HasInternetConnection(mActivity))
                   // {
                   //     mWebView.StopLoading();
                    //}
                   // else
                   // {
                   //     mWebView.LoadUrl("");
                   // }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
            {
                //base.OnReceivedSslError(view, handler, error);
                handler.Proceed();
            }

            public override void OnLoadResource(WebView view, string url)
            {
                //base.OnLoadResource(view, url);
                if (!ConnectionUtils.HasInternetConnection(mActivity))
                {
                    view.StopLoading();
                }
            }
            public static void ShowErrorMessage(string failingUrl)
            {
                if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                {
                    mErrorMessageSnackBar.Dismiss();
                }

                mErrorMessageSnackBar = Snackbar.Make(mainView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain"), delegate
                {
                    if (!failingUrl.ToLower().Contains("statusreceipt.aspx") || !failingUrl.ToLower().Contains("paystatusreceipt"))
                    {
                        //mWebView.LoadUrl(failingUrl);
                    }
                    mErrorMessageSnackBar.Dismiss();
                });
                View v = mErrorMessageSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);

                mErrorMessageSnackBar.Show();
            }

        }

    }
}