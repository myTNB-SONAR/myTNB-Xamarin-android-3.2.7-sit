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
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.UpdateNameFull.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.UpdateNameFull.Activity
{
    [Activity(Label = "@string/update_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class UpdateNameFullActivity : BaseActivityCustom, UpdateNicknameContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtInputLayoutNameFull)]
        TextInputLayout txtInputLayoutNameFull;

        [BindView(Resource.Id.txtNameFull)]
        EditText txtNameFull;

        [BindView(Resource.Id.btnSave)]
        Button btnSave;

        UpdateNicknameContract.IUserActionsListener userActionsListener;
        UpdateNicknamePresenter mPresenter;

        Snackbar mUpdateFullName;

        MaterialDialog progress;
        const string PAGE_ID = "UpdateName";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                UserEntity user = UserEntity.GetActive();
                progress = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.update_account_progress_title))
                    .Content(GetString(Resource.String.update_account_progress_content))
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                // Create your application here
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNameFull);
                TextViewUtils.SetMuseoSans300Typeface(txtNameFull);
                TextViewUtils.SetMuseoSans500Typeface(btnSave);

                txtInputLayoutNameFull.Hint = GetLabelCommonByLanguage("name");
                btnSave.Text = GetLabelCommonByLanguage("save");
                txtNameFull.Text = user.DisplayName;

                txtNameFull.AddTextChangedListener(new InputFilterFormField(txtNameFull, txtInputLayoutNameFull));

                mPresenter = new UpdateNicknamePresenter(this);
                this.userActionsListener.Start();

                txtNameFull.TextChanged += TxtNameFull_TextChanged;

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }


        [Preserve]
        private void TxtNameFull_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                string newName = txtNameFull.Text;
                this.userActionsListener.OnVerifyName(newName);
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
                string newName = txtNameFull.Text;
                this.userActionsListener.OnUpdateName(newName);
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
            return Resource.Layout.UpdateNameFullView;
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
            txtInputLayoutNameFull.Error = GetString(Resource.String.update_account_empty_nickname_error);
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

        public void ShowResponseError(string errorMessage)
        {
            if (mUpdateFullName != null && mUpdateFullName.IsShown)
            {
                mUpdateFullName.Dismiss();
            }

            mUpdateFullName = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mUpdateFullName.Dismiss(); }
            );
            View v = mUpdateFullName.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mUpdateFullName.Show();
            this.SetIsClicked(false);
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

        public void ShowSuccessUpdateName()
        {
            SetResult(Result.Ok);
            Finish();
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
            txtInputLayoutNameFull.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            txtInputLayoutNameFull.Error = GetString(Resource.String.invalid_charac);
        }


        public void ClearError()
        {
            txtInputLayoutNameFull.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            txtInputLayoutNameFull.Error = "";
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
