using Android.Content;
using Android.OS;


using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.PromotionsMenu
{
    public class PromotionsMenuFragment : BaseFragment
    {

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.webView)]
        private WebView webView;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        Weblink weblink;

        public static PromotionsMenuFragment NewInstance(Weblink weblink)
        {
            PromotionsMenuFragment fragment = new PromotionsMenuFragment();
            Bundle arguments = new Bundle();
            arguments.PutString(Constants.PROMOTIONS_LINK, JsonConvert.SerializeObject(weblink));
            fragment.Arguments = arguments;
            return fragment;
        }

        public override int ResourceId()
        {
            return Resource.Layout.PromotionsMenuView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            weblink = JsonConvert.DeserializeObject<Weblink>(Arguments.GetString(Constants.PROMOTIONS_LINK));

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();

        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            webView.Settings.JavaScriptEnabled = true;
            webView.SetWebViewClient(new PromotionsWebViewClient(this, mProgressBar, weblink, webView));
            webView.LoadUrl(weblink.Url);
        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardHomeActivity)
                {
                    var activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                    activity.UnsetToolbarBackground();
                }
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            base.OnAttach(context);
        }
        private Snackbar mErrorNoInternet;
        public void ShowErrorMessageNoInternet(string failingUrl)
        {
            try
            {
                if (mErrorNoInternet != null && mErrorNoInternet.IsShown)
                {
                    mErrorNoInternet.Dismiss();
                }

                mErrorNoInternet = Snackbar.Make(rootView, GetString(Resource.String.promotions_menu_snackbar_error_no_internet), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.promotions_menu_snackbar_error_btn), delegate
                {
                    webView.LoadUrl(failingUrl);
                    mErrorNoInternet.Dismiss();
                });
                View v = mErrorNoInternet.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);

                mErrorNoInternet.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();
        }

        class PromotionsWebViewClient : WebViewClient
        {

            private ProgressBar progressBar;
            private Weblink webLink;
            private PromotionsMenuFragment fragment;
            private WebView webView;

            public PromotionsWebViewClient(PromotionsMenuFragment fragment, ProgressBar progressBar, Weblink webLink, WebView webView)
            {
                this.fragment = fragment;
                this.progressBar = progressBar;
                this.webLink = webLink;
                this.webView = webView;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (fragment != null && fragment.Activity != null)
                {
                    if (ConnectionUtils.HasInternetConnection(fragment.Activity))
                    {
                        view.LoadUrl(url);
                    }
                    else
                    {
                        fragment.ShowErrorMessageNoInternet(url);
                    }
                    return true;
                }
                return false;

            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                if (fragment != null && fragment.Activity != null)
                {
                    if (ConnectionUtils.HasInternetConnection(fragment.Activity))
                    {
                        base.OnPageStarted(view, url, favicon);
                        progressBar.Visibility = ViewStates.Visible;
                        webView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        fragment.ShowErrorMessageNoInternet(url);
                    }
                }

            }

            public override void OnPageFinished(WebView view, string url)
            {
                if (progressBar != null && webView != null)
                {
                    progressBar.Visibility = ViewStates.Gone;
                    webView.Visibility = ViewStates.Visible;
                }

            }
        }
    }
}