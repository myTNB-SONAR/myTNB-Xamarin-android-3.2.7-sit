using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB.AndroidApp.Src.AppLaunch.Activity;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.PreLogin.Activity;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Runtime;

namespace myTNB.AndroidApp.Src.LogoutEnd.Activity
{
    [Activity(Label = "@string/logout_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Logout")]
    public class LogoutEndActivity : BaseAppCompatActivity
    {

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.btnBackToHome)]
        Button btnBackToHome;

        private ISharedPreferences mSharedPref;

        public override int ResourceId()
        {
            return Resource.Layout.LogoutEndView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            TextViewUtils.SetMuseoSans300Typeface(txtContentInfo);
            TextViewUtils.SetMuseoSans500Typeface(btnBackToHome, txtTitleInfo);
            txtTitleInfo.Text = Utility.GetLocalizedLabel("Logout", "logoutTitle");
            txtContentInfo.Text = Utility.GetLocalizedLabel("Logout", "message");
            btnBackToHome.Text = Utility.GetLocalizedLabel("Logout", "loginAgain");
            TextViewUtils.SetTextSize14(txtContentInfo);
            TextViewUtils.SetTextSize16(btnBackToHome, txtTitleInfo);
            mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            UserSessions.SavePhoneVerified(mSharedPref, false);
        }

        [OnClick(Resource.Id.btnBackToHome)]
        void OnBackToHome(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                LaunchViewActivity.MAKE_INITIAL_CALL = true;
                Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
                PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                AppInfoManager.Instance.Clear();
                StartActivity(PreLoginIntent);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            LaunchViewActivity.MAKE_INITIAL_CALL = true;
            Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
            PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            AppInfoManager.Instance.Clear();
            StartActivity(PreLoginIntent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Log Out");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}