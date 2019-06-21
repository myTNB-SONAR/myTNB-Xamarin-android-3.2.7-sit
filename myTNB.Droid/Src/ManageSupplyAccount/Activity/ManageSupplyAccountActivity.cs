using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.ManageSupplyAccount.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.UpdateNickname.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.ManageSupplyAccount.Activity
{
    [Activity(Label = "@string/manage_supply_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.ManageSupplyAccount")]
    public class ManageSupplyAccountActivity : BaseToolbarAppCompatActivity, ManageSupplyAccountContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtAccountNumber)]
        TextView txtAccountNumber;

        [BindView(Resource.Id.txtAccountAddress)]
        TextView txtAccountAddress;

        [BindView(Resource.Id.txtInputLayoutNickName)]
        TextInputLayout txtInputLayoutNickName;

        [BindView(Resource.Id.txtNickName)]
        EditText txtNickName;

        [BindView(Resource.Id.btnTextUpdateNickName)]
        TextView btnTextUpdateNickName;

        [BindView(Resource.Id.btnRemoveAccount)]
        Button btnRemoveAccount;


        AccountData accountData;
        int position;

        ManageSupplyAccountContract.IUserActionsListener userActionsListener;
        ManageSupplyAccountPresenter mPresenter;

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
                    position = extras.GetInt(Constants.SELECTED_ACCOUNT_POSITION);
                }


                progress = new MaterialDialog.Builder(this)
                    .Title(Resource.String.manage_supply_account_remove_progress_title)
                    .Content(Resource.String.manage_supply_account_remove_progress_content)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNickName);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountAddress, txtNickName);
                TextViewUtils.SetMuseoSans500Typeface(txtAccountNumber, btnTextUpdateNickName);
                TextViewUtils.SetMuseoSans500Typeface(btnRemoveAccount);

                txtAccountNumber.Text = accountData.AccountNum;
                txtAccountAddress.Text = accountData.AddStreet;

                txtNickName.Text = accountData.AccountNickName;


                mPresenter = new ManageSupplyAccountPresenter(this, accountData);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnTextUpdateNickName)]
        void OnClickUpdateNickname(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnUpdateNickname();
        }
        AlertDialog removeDialog;
        [OnClick(Resource.Id.btnRemoveAccount)]
        void OnClickRemoveAccount(object sender, EventArgs eventArgs)
        {
            try
            {
                if (removeDialog != null && removeDialog.IsShowing)
                {
                    removeDialog.Dismiss();
                }

                removeDialog = new AlertDialog.Builder(this)

                    .SetTitle(Resource.String.manage_supply_account_remove_dialog_title)
                    .SetMessage(GetString(Resource.String.manage_supply_account_remove_dialog_content_wildcard, accountData.AccountNickName, accountData.AccountNum))
                    .SetNegativeButton(Resource.String.manage_cards_btn_cancel,
                    delegate
                    {
                        removeDialog.Dismiss();
                    })
                    .SetPositiveButton(Resource.String.manage_cards_btn_ok,
                    delegate
                    {
                        this.userActionsListener.OnRemoveAccount(accountData);
                    })
                    .Show()
                    ;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //int titleId = Resources.GetIdentifier("alertTitle", "id", "android");
            //TextView txtTitle = removeDialog.FindViewById<TextView>(titleId);
            //txtTitle.SetTextSize(ComplexUnitType.Sp ,17);

        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ManageSupplyAccountsView;
        }

        public void SetPresenter(ManageSupplyAccountContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowUpdateNickname()
        {
            Intent updateNickName = new Intent(this, typeof(UpdateNicknameActivity));
            updateNickName.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            StartActivityForResult(updateNickName, Constants.UPDATE_NICKNAME_REQUEST);
        }

        public void ShowSuccessRemovedAccount()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            result.PutExtra(Constants.SELECTED_ACCOUNT_POSITION, position);
            result.PutExtra(Constants.ACCOUNT_REMOVED_FLAG, true);
            SetResult(Result.Ok, result);
            Finish();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
        }

        public void ShowUpdateSuccessNickname(AccountData accountData)
        {
            try
            {
                this.accountData = accountData;
                txtNickName.Text = accountData.AccountNickName;

                Snackbar.Make(rootView, GetString(Resource.String.manage_supply_account_update_nickname_success), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.manage_supply_account_btn_close),
                 (view) =>
                 {

                 // EMPTY WILL CLOSE SNACKBAR
             }
                ).Show();
                SetResult(Result.Ok);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRemoveProgress()
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

        public void HideRemoveProgress()
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

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.manage_supply_account_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.manage_supply_account_cancelled_exception_btn_close), delegate
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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.manage_supply_account_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.manage_supply_account_api_exception_btn_close), delegate
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

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.manage_supply_account_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.manage_supply_account_unknown_exception_btn_close), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public void ShowErrorMessageResponse(string error)
        {
            Snackbar.Make(rootView, error, Snackbar.LengthIndefinite)
                        .SetAction(GetString(Resource.String.manage_supply_account_btn_close),
                         (view) =>
                         {

                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        ).Show();
        }

        public void ShowNickname(string nickname)
        {
            txtNickName.Text = nickname;
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