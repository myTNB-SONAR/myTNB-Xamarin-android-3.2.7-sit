using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using AndroidSwipeLayout;
using AndroidSwipeLayout.Util;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.AndroidApp.Src.AddAccount.Activity;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.MyAccount.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.XDetailRegistrationForm.Models;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.RegisterValidation.MVP;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Login.Activity;

namespace myTNB.AndroidApp.Src.RegisterValidation.Activity
{
    [Activity(Label = "@string/app_name"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class EmailRegisterPopupActivity : BaseActivityCustom, EmelPopupContract.IView
    {

        [BindView(Resource.Id.txtAccCreated)]
        TextView txtAccCreated;

        [BindView(Resource.Id.txtVerifyEmail)]
        TextView txtVerifyEmail;

        [BindView(Resource.Id.txtVerifyNotification)]
        TextView txtVerifyNotification;

        [BindView(Resource.Id.btnContinue)]
        Button btnContinue;

        EmelPopupContract.IUserActionsListener userActionsListener;
        EmelPopupContract mPresenter;

        const string PAGE_ID = "Register";

        public string emailCredentials;

        public override int ResourceId()
        {
            return Resource.Layout.EmailPopup;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {

                    if (extras.ContainsKey(Constants.APP_CONFIG.API_KEY_ID))
                    {
                        emailCredentials = extras.GetString(Constants.APP_CONFIG.API_KEY_ID);
                    }
                }

                UserEntity userEntity = UserEntity.GetActive();
                var email = emailCredentials;
                string data;
                data = Utility.GetLocalizedLabel("RegisterSuccess", "emailVerifiedLinkSent");
                TextViewUtils.SetMuseoSans300Typeface(
                    txtVerifyNotification);
                TextViewUtils.SetMuseoSans500Typeface(txtAccCreated,
                    txtVerifyEmail);

                TextViewUtils.SetTextSize18(btnContinue);
                TextViewUtils.SetTextSize16(txtAccCreated, txtVerifyEmail);
                TextViewUtils.SetTextSize14(txtVerifyNotification);

                txtAccCreated.Text = Utility.GetLocalizedLabel("RegisterSuccess", "acccreated");
                txtVerifyEmail.Text = Utility.GetLocalizedLabel("RegisterSuccess", "emailverify");
                //txtAccCreated.Text = GetLabelByLanguage("acccreated");
                //txtVerifyEmail.Text = GetLabelByLanguage("emailverify");
                //txtVerifyNotification.Text = GetLabelByLanguage("emailNotiFirstHalf") + email + GetLabelByLanguage("emailNotiSecondHalf");
                //txtVerifyNotification.Text = string.Format(Utility.GetLocalizedLabel("Register", "emailVerifiedLinkSent"), email);
                string temp = string.Format(data, email);
                txtVerifyNotification.TextFormatted = GetFormattedText(temp);
                btnContinue.Text = Utility.GetLocalizedLabel("RegisterSuccess", "backLogin");
                //btnContinue.Text = GetLabelCommonByLanguage("continue");

                btnContinue.Click += OnClickAddAccount;

                //mPresenter = new MyAccountPresenter(this);
                //this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]

        [OnClick(Resource.Id.btnContinue)]
        void OnClickAddAccount(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                ShowLoginActivity();
            }
        }

        public void ShowLoginActivity()
        {
            //Intent LinkAccountIntent = new Intent(this, typeof(LinkAccountActivity));
            //LinkAccountIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            //LinkAccountIntent.PutExtra("fromDashboard", true);
            //LinkAccountIntent.PutExtra("fromRegisterPage", true);
            //StartActivity(LinkAccountIntent);

            Intent LoginIntent = new Intent(this, typeof(LoginActivity));
            LoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(LoginIntent);
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

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "");
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

        public void HideShowProgressDialog()
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void SetPresenter(EmelPopupContract.IUserActionsListener userActionListener)
        {
            throw new NotImplementedException();
        }

        public override bool ShowCustomToolbarTitle()
        {
            return false;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }
    }
}
