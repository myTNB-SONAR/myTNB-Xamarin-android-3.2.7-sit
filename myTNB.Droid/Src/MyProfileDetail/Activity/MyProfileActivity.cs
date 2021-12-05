using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageSupplyAccount.Activity;
using myTNB_Android.Src.MyProfileDetail.Adapter;
using myTNB_Android.Src.MyProfileDetail.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ProfileMenu;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.NotificationSettings.Activity;
using myTNB_Android.Src.UpdateID.Activity;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using myTNB_Android.Src.UpdateNameFull.Activity;
using myTNB_Android.Src.UpdatePassword.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.MyAccount.Activity
{
    [Activity(Label = "MyTNB Profile"
      //, NoHistory = false
      //, Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class MyProfileActivity : BaseActivityCustom, ProfileDetailContract.IView
    {
        [BindView(Resource.Id.profileMenuRootContent)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.rootView)]
        LinearLayout profileMenuLinear;

        [BindView(Resource.Id.profileMenuItemsContent)]
        LinearLayout profileMenuItemsContent;

        ProfileDetailPresenter mPresenter;
        private ProfileMenuItemContentComponent fullName, referenceNumber, email, mobileNumber, password;
        private bool mobileNoUpdated = false;

        const string PAGE_ID = "Tnb_Profile";

        private bool fromIDFlag = false;

        private bool fromEmailVerify = false;

       // private bool fromDashboard = false;

        ISharedPreferences mPref;

        private int APP_LANGUAGE_REQUEST = 32766;

        ProfileDetailContract.IUserActionsListener userActionsListener;


        public override int ResourceId()
        {
            return Resource.Layout.ProfileDetailPage;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {

                //if (Intent.HasExtra("fromDashboard"))
                //{
                //    fromDashboard = Intent.Extras.GetBoolean("fromDashboard", false);
                //}

                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                
                UserEntity user = UserEntity.GetActive();
                if(string.IsNullOrEmpty(user.IdentificationNo))
                {
                    fromIDFlag = true;
                }

                //var sharedpref_data = UserSessions.GetCheckEmailVerified(this.mPref);
                //bool isUpdatePersonalDetail = bool.Parse(sharedpref_data);
                //if (isUpdatePersonalDetail)
                //{
                //    fromEmailVerify = true;
                //}

                ProfileDetailItemComponent myTNBProfileItem = GetMyTNBAccountItems();
                profileMenuItemsContent.AddView(myTNBProfileItem);
                ProfileDetailItemComponent MyTNBPasswordItem = GetPasswordItem();
                profileMenuItemsContent.AddView(MyTNBPasswordItem);


                PopulateActiveAccountDetails();
                mPresenter = new ProfileDetailPresenter(this);
                //this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]

        private ProfileDetailItemComponent GetMyTNBAccountItems()
        {

            ProfileDetailItemComponent myTNBAccountItem = new ProfileDetailItemComponent(this);

            List<View> myTNBAccountItems = new List<View>();

            fullName = new ProfileMenuItemContentComponent(this);
            fullName.SetTitle(GetLabelCommonByLanguage("name").ToUpper());
            fullName.SetValue("");
            fullName.SetItemActionVisibility(true);
            fullName.SetItemActionTitle(GetLabelCommonByLanguage("update"));
            fullName.SetItemActionCall(UpdateName);
            myTNBAccountItems.Add(fullName);

            referenceNumber = new ProfileMenuItemContentComponent(this);
            referenceNumber.SetTitle(Utility.GetLocalizedLabel("Tnb_Profile", "lblID").ToUpper());
            referenceNumber.SetValue("");
            referenceNumber.SetFlagID(fromIDFlag);
            referenceNumber.SetItemActionTitle(GetLabelCommonByLanguage("update"));
            referenceNumber.SetItemActionCall(UpdateICNumber);
            myTNBAccountItems.Add(referenceNumber);

            email = new ProfileMenuItemContentComponent(this);
            email.SetTitle(GetLabelCommonByLanguage("email").ToUpper());
            email.SetValue("");
            email.SetItemActionVisibility(true);
            email.SetFlagEmailVerify(fromEmailVerify);
            email.SetItemActionTitle(GetLabelCommonByLanguage("verify"));
            email.SetItemActionCall(ShowEmailResendSuccess);
            myTNBAccountItems.Add(email);

            mobileNumber = new ProfileMenuItemContentComponent(this);
            mobileNumber.SetTitle(GetLabelCommonByLanguage("mobileNumber").ToUpper());
            mobileNumber.SetValue("");
            mobileNumber.SetItemActionVisibility(true);
            mobileNumber.SetItemActionTitle(GetLabelCommonByLanguage("update"));
            mobileNumber.SetItemActionCall(UpdateMobileNumber);
            myTNBAccountItems.Add(mobileNumber);

            /*password = new ProfileMenuItemContentComponent(this);
            password.SetTitle(GetLabelCommonByLanguage("password").ToUpper());
            password.SetValue("");
            password.SetItemActionVisibility(true);
            password.SetItemActionTitle(GetLabelCommonByLanguage("update"));
            password.SetItemActionCall(UpdatePassword);
            myTNBAccountItems.Add(password);*/

            myTNBAccountItem.AddComponentView(myTNBAccountItems);
            return myTNBAccountItem;
        }

        private ProfileDetailItemComponent GetPasswordItem()
        {

            ProfileDetailItemComponent passItem = new ProfileDetailItemComponent(this);

            List<View> passItems = new List<View>();

            ProfileMenuItemSingleContentComponent password = new ProfileMenuItemSingleContentComponent(this);
            password.SetTitle(Utility.GetLocalizedLabel("Tnb_Profile", "passwordchange")); 
            password.SetItemActionCall(UpdatePassword);
            passItems.Add(password);

            passItem.AddComponentView(passItems);
            return passItem;
        }

        private void UpdateName()
        {
            if (!this.GetIsClicked())
            {
                try
                {
                    Intent updateName = new Intent(this, typeof(UpdateNameFullActivity));
                    StartActivityForResult(updateName, Constants.UPDATE_NAME_REQUEST);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void UpdateICNumber()
        {
            if (!this.GetIsClicked())
            {
                try
                {
                    Intent updateICNo = new Intent(this, typeof(UpdateIDActivity));
                    StartActivityForResult(updateICNo, Constants.UPDATE_IC_REQUEST);
                   
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void UpdateMobileNumber()
        {
            if (!this.GetIsClicked())
            {
                try
                {
                    Intent updateMobileNo = new Intent(this, typeof(UpdateMobileActivity));
                    StartActivityForResult(updateMobileNo, Constants.UPDATE_MOBILE_NO_REQUEST);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void UpdatePassword()
        {
            if (!this.GetIsClicked())
            {
                try
                {
                    Intent updateMobileNo = new Intent(this, typeof(UpdatePasswordActivity));
                    StartActivityForResult(updateMobileNo, Constants.UPDATE_PASSWORD_REQUEST);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void PopulateActiveAccountDetails()
        {
            UserEntity user = UserEntity.GetActive();
            fullName.SetValue(user.DisplayName);
            try
            {
                string maskedNo = !string.IsNullOrEmpty(user?.IdentificationNo) ? user.IdentificationNo : "";

                if (!string.IsNullOrEmpty(maskedNo) && maskedNo.Count() > 4)
                {
                    string lastDigit = maskedNo.Substring(maskedNo.Length - 4);

                    maskedNo = GetString(Resource.String.my_account_ic_no_mask) + " " + lastDigit;
                }

                referenceNumber.SetValue(maskedNo);

                email.SetValue(user.Email);
                mobileNumber.SetValue(user.MobileNo);
                //password.SetValue(GetString(Resource.String.my_account_dummy_password));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmailResendSuccess()
        {

            Utility.ShowEmailVerificationDialog(this, () =>
            {
                //ShowProgressDialog();
                UserEntity user = UserEntity.GetActive();
                string email = user?.Email;
                this.userActionsListener.ResendEmailVerify(Constants.APP_CONFIG.API_KEY_ID, email);
                //ShowEmailUpdateSuccess();
            });

        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
        }
        

        private void ShowMobileUpdateSuccess(string newPhone)
        {
            try
            {
                mobileNumber.SetValue(newPhone); 
                Snackbar updatePhoneSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedLabel("Tnb_Profile", "mobileNumberVerified"), Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = updatePhoneSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updatePhoneSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowPasswordUpdateSuccess() 
        {
            
            try
            {
                Snackbar updatePassWordBar = Snackbar.Make(rootView, Utility.GetLocalizedLabel("Tnb_Profile", "passwordUpdateSuccess"), Snackbar.LengthIndefinite)
                            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = updatePassWordBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updatePassWordBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }
        private void ShowNameUpdateSuccess()
        {
            try
            {

                Snackbar updateNameBar = Snackbar.Make(rootView, Utility.GetLocalizedLabel("Tnb_Profile", "toast_SuccessUpdateName"), Snackbar.LengthIndefinite)
                            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = updateNameBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                TextViewUtils.SetTextSize14(tv);
                updateNameBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }
        public void ShowEmailUpdateSuccess(string message)
        {
            try
            {
                string Email = "";
                UserEntity userEntity = UserEntity.GetActive();
                Email = userEntity.Email;
                Snackbar updateEmailBar = Snackbar.Make(rootView, string.Format(Utility.GetLocalizedLabel("Tnb_Profile", "toast_email_send"), Email), Snackbar.LengthIndefinite)
                            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View v = updateEmailBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                TextViewUtils.SetTextSize14(tv);
                updateEmailBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowIDUpdateSuccess()
        {
            try
            {
                Snackbar updateIdBar =
                Snackbar.Make(rootView, Utility.GetLocalizedLabel("Tnb_Profile", "IDUpdateSuccess"), Snackbar.LengthIndefinite)
                           .SetAction(Utility.GetLocalizedCommonLabel("close"),
                             (view) =>
                             {
                                 // EMPTY WILL CLOSE SNACKBAR
                             }
                            );
                View snackbarView = updateIdBar.View;
                TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
                textView.SetMaxLines(4);
                TextViewUtils.SetTextSize14(textView);
                updateIdBar.Show();
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }
        

        private Snackbar mSnackBar;
        public void ShowError(string errorMessage)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
                mSnackBar.Show();
            }
            else
            {
                mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mSnackBar.Dismiss(); }
                );
                View v = mSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mSnackBar.Show();
            }
            this.SetIsClicked(false);
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            try
            {
                if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
                {
                    mCancelledExceptionSnackBar.Dismiss();
                }

                mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {

                    mCancelledExceptionSnackBar.Dismiss();
                }
                );
                View v = mCancelledExceptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mCancelledExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            try
            {
                if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
                {
                    mApiExcecptionSnackBar.Dismiss();
                }

                mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {

                    mApiExcecptionSnackBar.Dismiss();
                }
                );
                View v = mApiExcecptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mApiExcecptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            try
            {
                if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
                {
                    mUknownExceptionSnackBar.Dismiss();

                }

                mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {

                    mUknownExceptionSnackBar.Dismiss();
                }
                );
                View v = mUknownExceptionSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mUknownExceptionSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
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

        public void ShowGetCodeProgressDialog()
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

        private Snackbar mCodeCancelledExceptionSnackBar;
        public void ShowRetryOptionsCodeCancelledException(System.OperationCanceledException operationCanceledException)
        {
            UserEntity user = UserEntity.GetActive();

            if (mCodeCancelledExceptionSnackBar != null && mCodeCancelledExceptionSnackBar.IsShown)
            {
                mCodeCancelledExceptionSnackBar.Dismiss();
            }

            mCodeCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCodeCancelledExceptionSnackBar.Dismiss();
                string email = user?.Email;
                this.userActionsListener.ResendEmailVerify(Constants.APP_CONFIG.API_KEY_ID, email);
            }
            );
            View v = mCodeCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCodeCancelledExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        private Snackbar mCodeApiExceptionSnackBar;
        public void ShowRetryOptionsCodeApiException(ApiException apiException)
        {

            UserEntity user = UserEntity.GetActive();
            
            if (mCodeApiExceptionSnackBar != null && mCodeApiExceptionSnackBar.IsShown)
            {
                mCodeApiExceptionSnackBar.Dismiss();
            }

            mCodeApiExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCodeApiExceptionSnackBar.Dismiss();
                string email = user?.Email;
                this.userActionsListener.ResendEmailVerify(Constants.APP_CONFIG.API_KEY_ID, email);
            }
            );
            View v = mCodeApiExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCodeApiExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        private Snackbar mCodeUnknownException;
        public void ShowRetryOptionsCodeUnknownException(Exception exception)
        {
            UserEntity user = UserEntity.GetActive();

            if (mCodeUnknownException != null && mCodeUnknownException.IsShown)
            {
                mCodeUnknownException.Dismiss();
            }

            mCodeUnknownException = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCodeUnknownException.Dismiss();
                string email = user?.Email;
                this.userActionsListener.ResendEmailVerify(Constants.APP_CONFIG.API_KEY_ID, email);
            }
            );
            View v = mCodeUnknownException.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCodeUnknownException.Show();
            this.SetIsClicked(false);
        }

        public void HideGetCodeProgressDialog()
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

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Tnb_Profile");
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

        public void SetPresenter(ProfileDetailContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                //this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == Constants.UPDATE_MOBILE_NO_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        UserEntity userEntity = UserEntity.GetActive();
                        ShowMobileUpdateSuccess(userEntity.MobileNo);
                        MyTNBAccountManagement.GetInstance().SetIsUpdatedMobile(true);
                    }
                }
                else if (requestCode == Constants.UPDATE_PASSWORD_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsPasswordUpdated(true);
                        {
                            ShowPasswordUpdateSuccess();
                            MyTNBAccountManagement.GetInstance().SetIsPasswordUpdated(false);
                        }
                    }
                }
                else if (requestCode == Constants.UPDATE_NAME_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsNameUpdated(true);
                        {
                            ShowNameUpdateSuccess();
                            MyTNBAccountManagement.GetInstance().SetIsNameUpdated(false);
                        }
                    }
                }
                else if (requestCode == Constants.UPDATE_EMAIL_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsEmailUpdated(true);
                        {
                            UserEntity userEntity = UserEntity.GetActive();
                            ShowEmailUpdateSuccess(userEntity.Email);
                            MyTNBAccountManagement.GetInstance().SetIsEmailUpdated(false);
                        }
                    }
                }
                else if (requestCode == Constants.UPDATE_IC_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsIDUpdated(true);
                        {
                            ShowIDUpdateSuccess();
                            MyTNBAccountManagement.GetInstance().SetIsIDUpdated(false);
                            UserEntity user = UserEntity.GetActive();
                            if (!string.IsNullOrEmpty(user.IdentificationNo))
                            {
                                fromIDFlag = false;
                                referenceNumber.SetFlagID(fromIDFlag);
                            }
                        }
                    }
                }
                
                PopulateActiveAccountDetails();
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
    }
}
