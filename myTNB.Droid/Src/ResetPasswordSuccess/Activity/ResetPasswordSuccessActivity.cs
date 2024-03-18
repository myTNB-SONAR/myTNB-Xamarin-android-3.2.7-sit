using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Login.Activity;
using myTNB.AndroidApp.Src.ResetPasswordSuccess.MVP;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Runtime;

namespace myTNB.AndroidApp.Src.ResetPasswordSuccess.Activity
{
    [Activity(NoHistory = true
              , Icon = "@drawable/ic_launcher"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.ResetPasswordSuccess")]
    public class ResetPasswordSuccessActivity : BaseActivityCustom, ResetPasswordSuccessContract.IView
    {

        private ResetPasswordSuccessContract.IUserActionsListener userActionsListener;
        private ResetPasswordSuccessPresenter mPresenter;

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.btnLogin)]
        Button btnLogin;

        [BindView(Resource.Id.btnClose)]
        ImageView btnClose;

        const string PAGE_ID = "PasswordResetSuccess";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                // Create your application here
                mPresenter = new ResetPasswordSuccessPresenter(this);

                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);
                TextViewUtils.SetMuseoSans300Typeface(txtContentInfo);
                TextViewUtils.SetMuseoSans500Typeface(btnLogin);
                TextViewUtils.SetTextSize14(txtContentInfo);
                TextViewUtils.SetTextSize16(btnLogin, txtTitleInfo);
                txtTitleInfo.Text = GetLabelByLanguage("title");
                txtContentInfo.Text = GetLabelByLanguage("resetSuccessMessage");
                btnLogin.Text = GetLabelCommonByLanguage("login");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ResetPasswordSuccessView;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Reset Password Success");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(ResetPasswordSuccessContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowBackActivity()
        {
            Intent LoginIntent = new Intent(this, typeof(LoginActivity));
            LoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(LoginIntent);
        }

        public void ShowLoginActivity()
        {
            Intent LoginIntent = new Intent(this, typeof(LoginActivity));
            LoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(LoginIntent);
        }

        [OnClick(Resource.Id.btnLogin)]
        void OnLogin(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnLogin();
            }
        }

        [OnClick(Resource.Id.btnClose)]
        void OnClose(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnClose();
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }
    }
}