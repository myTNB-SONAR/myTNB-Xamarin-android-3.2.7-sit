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
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MyAccount.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.XDetailRegistrationForm.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using myTNB_Android.Src.RegisterValidation.MVP;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.RegisterValidation.Activity
{
    [Activity(Label = "@string/app_name"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.PreLogin")]
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

        public override int ResourceId()
        {
            return Resource.Layout.EmailPopup;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {

                UserEntity userEntity = UserEntity.GetActive();
                var email = userEntity.Email;  

                TextViewUtils.SetMuseoSans300Typeface(
                    txtVerifyNotification);
                TextViewUtils.SetMuseoSans500Typeface(txtAccCreated,
                    txtVerifyEmail);

                txtAccCreated.Text = GetLabelByLanguage("acccreated");
                txtVerifyEmail.Text = GetLabelByLanguage("emailverify");
                txtVerifyNotification.Text = GetLabelByLanguage("emailNotiFirstHalf") + email + GetLabelByLanguage("emailNotiSecondHalf");
                btnContinue.Text = GetLabelCommonByLanguage("continue");

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
                ShowAccountListActivity();
            }
        }

        public void ShowAccountListActivity()
        {
            Intent LinkAccountIntent = new Intent(this, typeof(LinkAccountActivity));
            LinkAccountIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            LinkAccountIntent.PutExtra("fromDashboard", true);
            LinkAccountIntent.PutExtra("fromRegisterPage", true);
            StartActivity(LinkAccountIntent);
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
