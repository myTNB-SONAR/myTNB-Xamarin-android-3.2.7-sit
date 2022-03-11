
using Android.App;
using Android.Content.PM;
using Android.OS;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.DigitalSignature.WebView.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.DigitalSignature.WebView.Activity
{
    [Activity(Label = "DS WebView", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class DSWebViewActivity : BaseActivityCustom, DSWebViewContract.IView
    {
        private const string PAGE_ID = "DSWebView";

        private DSWebViewContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _ = new DSWebViewPresenter(this);
            this.userActionsListener?.OnInitialize();
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.DS_WEBVIEW, LanguageConstants.DSWebView.TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
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
            SetResult(Result.Canceled);
            Finish();
        }
    }
}
