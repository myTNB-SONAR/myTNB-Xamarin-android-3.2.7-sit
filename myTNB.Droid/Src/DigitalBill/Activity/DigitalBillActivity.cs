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
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.DigitalBill.MVP;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Constants = myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.DigitalBill.Activity
{
    [Activity(Label = "@string/terms_conditions_activity_title",
        ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.TnC")]
    public class DigitalBillActivity : BaseActivityCustom, DigitalBillContract.IView
    {
        private DigitalBillPresenter mPresenter;
        private DigitalBillContract.IUserActionsListener userActionsListener;
        private static Snackbar mErrorMessageSnackBar;
        public AccountData mSelectedAccountData;
        private static FrameLayout mainView;
        private GetBillRenderingResponse BillRendering;

        private WebView micrositeWebView;

        private const string PAGE_ID = "ManageDigitalBillLanding";
        private const string SELECTED_ACCOUNT_KEY = ".selectedAccount";
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
            catch (System.Exception e)
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
                Bundle extras = Intent.Extras;

                if ((extras != null) && extras.ContainsKey(SELECTED_ACCOUNT_KEY))
                {
                    mSelectedAccountData = JsonConvert.DeserializeObject<AccountData>(extras.GetString(SELECTED_ACCOUNT_KEY));
                }
                if ((extras != null) && extras.ContainsKey("billrenderingresponse"))
                {
                    BillRendering = JsonConvert.DeserializeObject<GetBillRenderingResponse>(extras.GetString("billrenderingresponse"));
                }

                mPresenter = new DigitalBillPresenter(this);

                micrositeWebView = FindViewById<WebView>(Resource.Id.tncWebView);
                micrositeWebView.Settings.JavaScriptEnabled = true;
                micrositeWebView.SetWebViewClient(new MyTNBWebViewClient(this));
                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                ShowDigitalBill(true);

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetDefaultData()
        {
            try
            {
                UserEntity user = UserEntity.GetActive();
                string accnum = mSelectedAccountData.AccountNum;
                string myTNBAccountName = user?.DisplayName ?? string.Empty;
                string signature = SSOManager.Instance.GetSignature(myTNBAccountName
                    , AccessTokenCache.Instance.GetAccessToken(this)
                    , user.DeviceId ?? string.Empty
                    , DeviceIdUtils.GetAppVersionName().Replace("v", string.Empty)
                    , 16
                    , (LanguageUtil.GetAppLanguage() == "MS"
                        ? LanguageManager.Language.MS
                        : LanguageManager.Language.EN).ToString()
                    , TextViewUtils.FontSelected ?? "N"
                    , BillRendering.Content.OriginURL
                    , BillRendering.Content.RedirectURL
                    , accnum);

                string ssoURL = string.Format(AWSConstants.Domains.SSO, signature);
                micrositeWebView.LoadUrl(ssoURL);
            }
            catch (System.Exception e)
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
            catch (System.Exception e)
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
            catch (System.Exception e)
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

        internal void OnManageBillDelivery()
        {
            ShowProgressDialog();
            try
            {
                CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
                AccountData selectedAccountData = AccountData.Copy(customerAccount, true);
                Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
                intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
                intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(BillRendering));
                intent.PutExtra("_isOwner", JsonConvert.SerializeObject(DBRUtility.Instance.IsCADBREligible(selectedAccountData.AccountNum)));
                StartActivity(intent);
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            HideProgressDialog();
        }

        internal void OnShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
            HomeMenuUtils.ResetAll();
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (System.Exception e)
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
            catch (System.Exception e)
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
                bool shouldOverride = false;
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    if (url.ToLower().Contains("mytnbapp://action=backToApp"))
                    {
                        mActivity.OnBackPressed();
                        shouldOverride = true;
                    }
                    else if (url.ToLower().Contains("mytnbapp://action=backToHome"))
                    {
                        mActivity.OnShowDashboard();
                        shouldOverride = true;
                    }
                }
                return shouldOverride;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                try
                {
                    base.OnPageStarted(view, url, favicon);
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
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
            {
                handler.Proceed();
            }

            public override void OnLoadResource(WebView view, string url)
            {
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

                mErrorMessageSnackBar = Snackbar.Make(mainView
                    , Utility.GetLocalizedErrorLabel("noDataConnectionMessage")
                    , Snackbar.LengthIndefinite)
                    .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain")
                        , delegate
                    {
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