using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.ForgotPasswordVerificationCodeSuccess.MVP;
using myTNB.AndroidApp.Src.Login.Activity;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Runtime;

namespace myTNB.AndroidApp.Src.ForgotPasswordVerificationCodeSuccess.Activity
{
    [Activity(NoHistory = true
              , Icon = "@drawable/ic_launcher"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.ResetPasswordSuccess")]
    public class ForgotPasswordVerificationCodeSuccessActivity : BaseActivityCustom, ForgotPasswordVerificationCodeSuccessContract.IView
    {

        private ForgotPasswordVerificationCodeSuccessContract.IUserActionsListener userActionsListener;
        private ForgotPasswordVerificationCodeSuccessPresenter mPresenter;
        private string email;

        [BindView(Resource.Id.verifyCodeTxtTitleInfo)]
        TextView verifyCodeTxtTitleInfo;

        [BindView(Resource.Id.verifyCodeTxtContentInfo)]
        TextView verifyCodeTxtContentInfo;

        //[BindView(Resource.Id.verifyCodeTxtContentInfo2)]
        //TextView verifyCodeTxtContentInfo2;


        [BindView(Resource.Id.verifyCodeBtnLogin)]
        Button verifyCodeBtnLogin;

        [BindView(Resource.Id.btnClose)]
        ImageView btnClose;

        const string PAGE_ID = "PasswordResetSuccess";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                // Create your application here
                mPresenter = new ForgotPasswordVerificationCodeSuccessPresenter(this);

                TextViewUtils.SetMuseoSans500Typeface(verifyCodeTxtTitleInfo);
                TextViewUtils.SetMuseoSans300Typeface(verifyCodeTxtContentInfo);
                //TextViewUtils.SetMuseoSans300Typeface(verifyCodeTxtContentInfo2);
                TextViewUtils.SetMuseoSans500Typeface(verifyCodeBtnLogin);
                TextViewUtils.SetTextSize14(verifyCodeTxtContentInfo);
                TextViewUtils.SetTextSize16(verifyCodeTxtTitleInfo, verifyCodeBtnLogin);
                verifyCodeTxtTitleInfo.Text = GetLabelByLanguage("title");
                verifyCodeBtnLogin.Text = GetLabelByLanguage("proceedToLogin");

                Bundle extras = Intent.Extras;

                if (extras.ContainsKey("email"))
                {
                    email = extras.GetString("email");

                    verifyCodeTxtContentInfo.Text = string.Format(GetLabelByLanguage("details"), email);
                }
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
            return Resource.Layout.ForgotPasswordVerificationCodeSuccessView;
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

        public void SetPresenter(ForgotPasswordVerificationCodeSuccessContract.IUserActionsListener userActionListener)
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

        [OnClick(Resource.Id.verifyCodeBtnLogin)]
        void OnLogin(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnLogin();
        }

        [OnClick(Resource.Id.btnClose)]
        void OnClose(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnClose();
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