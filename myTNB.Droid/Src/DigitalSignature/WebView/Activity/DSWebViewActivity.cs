using Android.App;
using Android.Content.PM;
using Android.OS;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.DigitalSignature.WebView.MVP;
using myTNB_Android.Src.Utils;
using Android.Webkit;
using myTNB.Mobile;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB;
using myTNB.Mobile.AWS.Models.DS.Identification;
using Newtonsoft.Json;
using Android.Widget;
using Android.Util;
using Android.Net.Http;
using myTNB_Android.Src.myTNBMenu.Activity;
using Android.Content;

namespace myTNB_Android.Src.DigitalSignature.WebView.Activity
{
    [Activity(Label = "DS WebView", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class DSWebViewActivity : BaseActivityCustom, DSWebViewContract.IView
    {
        private const string PAGE_ID = "DSWebView";

        private DSWebViewContract.IUserActionsListener userActionsListener;
        private Android.Webkit.WebView micrositeWebView;

        GetEKYCIdentificationModel _identificationModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                Bundle extras = Intent.Extras;
                if ((extras != null) && extras.ContainsKey(DigitalSignatureConstants.DS_IDENTIFICATION_MODEL))
                {
                    _identificationModel = JsonConvert.DeserializeObject<GetEKYCIdentificationModel>(extras.GetString(DigitalSignatureConstants.DS_IDENTIFICATION_MODEL));
                }

                _ = new DSWebViewPresenter(this);
                this.userActionsListener?.OnInitialize();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.DS_WEBVIEW, LanguageConstants.DSWebView.TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            micrositeWebView = FindViewById<Android.Webkit.WebView>(Resource.Id.digitalSignatureWebView);

            PrepareWebView();
        }

        public void SetPresenter(DSWebViewContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DSWebView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override void OnBackPressed()
        {
            ShowBackPopUpConfirmation();
        }

        private void ShowBackPopUpConfirmation()
        {
            RunOnUiThread(() =>
            {
                MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_ICON_TWO_BUTTON)
                   .SetHeaderImage(Resource.Drawable.ic_display_validation_success)
                   .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.DS_WEBVIEW, LanguageConstants.DSWebView.CONFIRM_POPUP_TITLE))
                   .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.DS_WEBVIEW, LanguageConstants.DSWebView.CONFIRM_POPUP_MSG))
                   .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.DS_WEBVIEW, LanguageConstants.DSWebView.STAY))
                   .SetSecondaryCTALabel(Utility.GetLocalizedLabel(LanguageConstants.DS_WEBVIEW, LanguageConstants.DSWebView.LEAVE))
                   .SetCTAaction(() => { })
                   .SetSecondaryCTAaction(() => LeaveOnClick())
                   .Build();
                marketingTooltip.Show();
            });
        }

        private void LeaveOnClick()
        {
            SetResult(Result.Canceled);
            Finish();
        }

        private void PrepareWebView()
        {
            UserEntity user = UserEntity.GetActive();
            string myTNBAccountName = user?.DisplayName ?? string.Empty;
            string signature = SSOManager.Instance.GetDSSignature(myTNBAccountName
                , AccessTokenCache.Instance.GetAccessToken(this)
                , user?.DeviceId ?? string.Empty
                , DeviceIdUtils.GetAppVersionName().Replace("v", string.Empty)
                , 16
                , (LanguageUtil.GetAppLanguage() == "MS" ? LanguageManager.Language.MS : LanguageManager.Language.EN).ToString()
                , TextViewUtils.FontInfo ?? "N"
                , user?.UserID
                , _identificationModel?.IdentificationType
                , _identificationModel?.IdentificationNo);

            string ssoURL = string.Format(AWSConstants.Domains.DSSSO, signature);

            micrositeWebView.SetWebChromeClient(new WebChromeClient());
            micrositeWebView.SetWebViewClient(new MyTNBWebViewClient(this));
            micrositeWebView.Settings.JavaScriptEnabled = true;

            micrositeWebView.LoadUrl(ssoURL);
        }

        internal void OnShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            HomeMenuUtils.ResetAll();
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        public class MyTNBWebViewClient : WebViewClient
        {
            private DSWebViewActivity mActivity;
            private ProgressBar progressBar;

            public MyTNBWebViewClient(DSWebViewActivity mActivity)
            {
                this.mActivity = mActivity;
            }

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
            {
                bool shouldOverride = false;
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    Log.Debug("[DEBUG]", "MyTNBWebViewClient url: " + url.ToString());
                    if (url.Contains("mytnbapp://action=backToApp"))
                    {
                        mActivity.OnBackPressed();
                        shouldOverride = true;
                    }
                    else if (url.Contains("mytnbapp://action=backToHome"))
                    {
                        //mActivity.OnShowDashboard();
                        shouldOverride = true;
                    }

                    //Update for X button
                    if (url.ToString().Contains("BillDelivery/Success"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = true;
                        //mActivity.OnTag(true);
                    }
                    else if (url.ToString().Contains("Home/Error"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = true;
                        //mActivity.OnTag(true, true);
                    }
                    else if (url.ToString().Contains("Feedback/Rate")
                        || url.ToString().Contains("FeedbackRating/Success"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = false;
                        //mActivity.OnTagRatingDynatrace(url.ToString().Contains("FeedbackRating/Success"));
                    }
                    //Log.Debug("[DEBUG]", "MyTNBWebViewClient ShouldBackToHome: " + mActivity.ShouldBackToHome);
                }
                return shouldOverride;
            }

            public override void OnPageStarted(Android.Webkit.WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                try
                {
                    base.OnPageStarted(view, url, favicon);
                    Log.Debug("[DEBUG]", "OnPageStarted url: " + url.ToString());
                    //Update for X button
                    if (url.ToString().Contains("BillDelivery/Success"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = true;
                        //mActivity.OnTag(true);
                    }
                    else if (url.ToString().Contains("Home/Error"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = true;
                        //mActivity.OnTag(true, true);
                    }
                    else if (url.ToString().Contains("Feedback/Rate")
                       || url.ToString().Contains("FeedbackRating/Success"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = false;
                        //mActivity.OnTagRatingDynatrace(url.ToString().Contains("FeedbackRating/Success"));
                        //mActivity.SetShareFeedbackTitle();
                    }
                    //Log.Debug("[DEBUG]", "OnPageStarted ShouldBackToHome: " + mActivity.ShouldBackToHome);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                try
                {
                    Log.Debug("[DEBUG]", "OnPageFinished url: " + url.ToString());
                    //Update for X button
                    if (url.ToString().Contains("BillDelivery/Success"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = true;
                        //mActivity.OnTag(true);
                    }
                    else if (url.ToString().Contains("Home/Error"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = true;
                        //mActivity.OnTag(true, true);
                    }
                    else if (url.ToString().Contains("Feedback/Rate")
                       || url.ToString().Contains("FeedbackRating/Success"))
                    {
                        //mActivity.ShouldBackToHome = true;
                        //mActivity.IsDBR = false;
                        //mActivity.OnTagRatingDynatrace(url.ToString().Contains("FeedbackRating/Success"));
                    }
                    //Log.Debug("[DEBUG]", "OnPageFinished ShouldBackToHome: " + mActivity.ShouldBackToHome);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override bool OnRenderProcessGone(Android.Webkit.WebView view, RenderProcessGoneDetail detail)
            {
                return true;
            }

            public override void OnReceivedError(Android.Webkit.WebView view, ClientError errorCode, string description, string failingUrl)
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

            public override void OnReceivedSslError(Android.Webkit.WebView view, SslErrorHandler handler, SslError error)
            {
                handler.Proceed();
            }

            public override void OnLoadResource(Android.Webkit.WebView view, string url)
            {
                if (!ConnectionUtils.HasInternetConnection(mActivity))
                {
                    view.StopLoading();
                }
            }

            public static void ShowErrorMessage(string failingUrl)
            {
                //if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                //{
                //    mErrorMessageSnackBar.Dismiss();
                //}

                //mErrorMessageSnackBar = Snackbar.Make(mainView
                //    , Utility.GetLocalizedErrorLabel("noDataConnectionMessage")
                //    , Snackbar.LengthIndefinite)
                //    .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain")
                //        , delegate
                //        {
                //            mErrorMessageSnackBar.Dismiss();
                //        });
                //View v = mErrorMessageSnackBar.View;
                //TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                //tv.SetMaxLines(5);
                //mErrorMessageSnackBar.Show();
            }
        }
    }
}
