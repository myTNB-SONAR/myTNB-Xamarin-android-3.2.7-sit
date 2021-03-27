using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;


using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.UpdateNickname.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.UpdateNickname.Activity
{
    [Activity(Label = "@string/update_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class UpdateNicknameActivity : BaseActivityCustom, UpdateNicknameContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtInputLayoutAccountNickname)]
        TextInputLayout txtInputLayoutAccountNickname;

        [BindView(Resource.Id.txtAccountNickname)]
        EditText txtAccountNickname;

        [BindView(Resource.Id.btnSave)]
        Button btnSave;

        AccountData accountData;

        UpdateNicknameContract.IUserActionsListener userActionsListener;
        UpdateNicknamePresenter mPresenter;

        MaterialDialog progress;
        const string PAGE_ID = "UpdateNickname";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_UpdateMobileLarge : Resource.Style.Theme_UpdateMobile);
            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        //accountData = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
                        accountData = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                }


                progress = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.update_account_progress_title))
                    .Content(GetString(Resource.String.update_account_progress_content))
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                // Create your application here
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutAccountNickname);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountNickname);
                TextViewUtils.SetMuseoSans500Typeface(btnSave);

                txtInputLayoutAccountNickname.Hint = GetLabelCommonByLanguage("acctNickname");
                btnSave.Text = GetLabelCommonByLanguage("save");

                txtAccountNickname.AddTextChangedListener(new InputFilterFormField(txtAccountNickname, txtInputLayoutAccountNickname));

                if (accountData != null && !string.IsNullOrEmpty(accountData.AccountNickName))
                {
                    txtAccountNickname.Text = accountData.AccountName;
                }


                mPresenter = new UpdateNicknamePresenter(this, accountData);
                this.userActionsListener.Start();



                txtAccountNickname.TextChanged += TxtAccountNickname_TextChanged;

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }


        [Preserve]
        private void TxtAccountNickname_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                string newAccountNickName = txtAccountNickname.Text;
                this.userActionsListener.OnVerifyNickName(accountData.AccountNum, newAccountNickName);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.btnSave)]
        void OnClickSave(object sender, EventArgs eventArgs)
        {
            try
            {
                string newNickName = txtAccountNickname.Text;
                string oldNickname = accountData.AccountNickName;
                string accountNo = accountData.AccountNum;

                this.userActionsListener.OnUpdateNickName(accountNo, oldNickname, newNickName);
            }
            catch (Exception e)
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
            return Resource.Layout.UpdateAccountNicknameView;
        }

        public void SetPresenter(UpdateNicknameContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowEmptyNickNameError()
        {
            txtInputLayoutAccountNickname.Error = GetString(Resource.String.update_account_empty_nickname_error);
        }

        public void ShowSameNickNameError()
        {
            txtInputLayoutAccountNickname.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
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

        public void ShowResponseError(string error)
        {
            txtInputLayoutAccountNickname.Error = error;
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();

        }

        public void ShowSuccessUpdateNickname(string newNickName)
        {
            txtAccountNickname.Text = newNickName;
            Intent intent = new Intent();
            accountData.AccountNickName = newNickName;
            intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            SetResult(Result.Ok, intent);
            Finish();
        }

        public void ShowNickname(string nickname)
        {
            txtAccountNickname.Text = nickname;
            txtAccountNickname.SetSelection(nickname.Length);
        }

        public void EnableSaveButton()
        {
            btnSave.Enabled = true;
            btnSave.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);

        }

        public void DisableSaveButton()
        {
            btnSave.Enabled = false;
            btnSave.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }


        public void ShowEnterValidAccountName()
        {
            txtInputLayoutAccountNickname.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
           
            txtInputLayoutAccountNickname.Error = GetString(Resource.String.invalid_charac);
        }


        public void ClearError()
        {
            txtInputLayoutAccountNickname.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
            
            txtInputLayoutAccountNickname.Error = "";
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

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                FirebaseAnalyticsUtils.SetScreenName(this, "Update Account Nickname");
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
