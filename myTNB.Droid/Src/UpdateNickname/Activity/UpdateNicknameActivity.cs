using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.UpdateNickname.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.UpdateNickname.Activity
{
    [Activity(Label = "@string/update_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.UpdateMobile")]
    public class UpdateNicknameActivity : BaseToolbarAppCompatActivity, UpdateNicknameContract.IView
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
        private LoadingOverlay loadingOverlay;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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
            //if (progress != null && progress.IsShowing)
            //{
            //    progress.Dismiss();
            //}

            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
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
            txtInputLayoutAccountNickname.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
        }

        public void ShowProgressDialog()
        {
            //if (progress != null && !progress.IsShowing)
            //{
            //    progress.Show();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
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

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_cancelled_exception_btn_retry), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_api_exception_btn_retry), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_unknown_exception_btn_retry), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
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
            txtInputLayoutAccountNickname.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            txtInputLayoutAccountNickname.Error = GetString(Resource.String.invalid_charac);
        }


        public void ClearError()
        {
            txtInputLayoutAccountNickname.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Update Account Nickname Screen");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}