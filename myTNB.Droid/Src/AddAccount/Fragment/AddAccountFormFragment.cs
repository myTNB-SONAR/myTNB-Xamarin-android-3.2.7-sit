using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.MVP;
using myTNB_Android.Src.Barcode.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using Refit;
using System;

namespace myTNB_Android.Src.AddAccount.Fragment
{
    public class AddAccountFormFragment : Android.App.Fragment, AddAccountContract.IView
    {
        private static string TAG = "AddAccountForm";
        private bool isOwner = false;
        private bool hasRights = false;
        private AccountType selectedAccountType;
        private readonly int SELECT_ACCOUNT_TYPE_REQ_CODE = 2011;

        LinearLayout ownerDetailsLayout;
        Button addAccount;

        private LoadingOverlay loadingOverlay;
        private Snackbar mSnackBar;
        private MaterialDialog dialogWhereMyAccountNo;

        private AddAccountPresenter mPresenter;
        private AddAccountContract.IUserActionsListener userActionsListener;

        LinearLayout rootView;

        [BindView(Resource.Id.account_no_edittext)]
        EditText edtAccountNo;

        [BindView(Resource.Id.account_label_edittext)]
        EditText edtAccountLabel;

        [BindView(Resource.Id.owner_ic_no_edittext)]
        EditText edtOwnersIC;

        [BindView(Resource.Id.owner_mother_maiden_name_edittext)]
        EditText edtOwnerMotherName;

        [BindView(Resource.Id.account_no_layout)]
        TextInputLayout textInputLayoutAccountNo;

        [BindView(Resource.Id.account_label_layout)]
        TextInputLayout textInputLayoutAccountLabel;

        [BindView(Resource.Id.owner_ic_no_layout)]
        TextInputLayout textInputLayoutOwnerIC;

        [BindView(Resource.Id.owner_mother_maiden_name_layout)]
        TextInputLayout textInputLayoutMotherMaidenName;

        [BindView(Resource.Id.scan)]
        ImageButton scan;

        [BindView(Resource.Id.txtAccountType)]
        TextView txtAccountType;

        [BindView(Resource.Id.btnWhereIsMyAccountNo)]
        TextView btnWhereIsMyAccountNo;

        [BindView(Resource.Id.selector_account_type)]
        TextView accountType;

        private bool isClicked = false;

        public void ClearText()
        {
            edtAccountNo.Text = "";
            edtAccountLabel.Text = "";
            edtOwnersIC.Text = "";
            edtOwnerMotherName.Text = "";
        }

        public void HideAddingAccountProgressDialog()
        {
            if (IsActive())
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                    loadingOverlay = new LoadingOverlay(Activity, Resource.Style.LoadingOverlyDialogStyle);
                }
            }
        }

        public bool IsActive()
        {
            return this.IsAdded && this.IsVisible;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            isOwner = Arguments.GetBoolean("isOwner");
            hasRights = Arguments.GetBoolean("hasRights");
            mPresenter = new AddAccountPresenter(this);
            loadingOverlay = new LoadingOverlay(Activity, Resource.Style.LoadingOverlyDialogStyle);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mainView = inflater.Inflate(Resource.Layout.AddAccountFormView, container, false);
            try
            {
                ownerDetailsLayout = mainView.FindViewById<LinearLayout>(Resource.Id.owner_details_layout);
                rootView = mainView.FindViewById<LinearLayout>(Resource.Id.rootView);
                edtAccountNo = mainView.FindViewById<EditText>(Resource.Id.account_no_edittext);
                edtAccountLabel = mainView.FindViewById<EditText>(Resource.Id.account_label_edittext);
                edtOwnersIC = mainView.FindViewById<EditText>(Resource.Id.owner_ic_no_edittext);
                edtOwnerMotherName = mainView.FindViewById<EditText>(Resource.Id.owner_mother_maiden_name_edittext);

                textInputLayoutAccountNo = mainView.FindViewById<TextInputLayout>(Resource.Id.account_no_layout);
                textInputLayoutAccountLabel = mainView.FindViewById<TextInputLayout>(Resource.Id.account_label_layout);
                textInputLayoutMotherMaidenName = mainView.FindViewById<TextInputLayout>(Resource.Id.owner_mother_maiden_name_layout);
                textInputLayoutOwnerIC = mainView.FindViewById<TextInputLayout>(Resource.Id.owner_ic_no_layout);
                txtAccountType = mainView.FindViewById<TextView>(Resource.Id.txtAccountType);

                accountType = mainView.FindViewById<TextView>(Resource.Id.selector_account_type);

                txtAccountType.Text = Utility.GetLocalizedLabel("Common", "accountType").ToUpper();
                textInputLayoutAccountNo.Hint = Utility.GetLocalizedLabel("Common","accountNo");
                textInputLayoutAccountLabel.Hint = Utility.GetLocalizedLabel("Common","acctNickname");
                textInputLayoutOwnerIC.Hint = Utility.GetLocalizedLabel("AddAccount", "ownerICNumber");

                TextViewUtils.SetMuseoSans300Typeface(edtAccountLabel
                    , edtAccountNo
                    , edtOwnersIC
                    , edtOwnerMotherName);

                TextViewUtils.SetMuseoSans300Typeface(textInputLayoutAccountNo
                    , textInputLayoutAccountLabel
                    , textInputLayoutMotherMaidenName
                    , textInputLayoutOwnerIC);

                TextViewUtils.SetMuseoSans300Typeface(txtAccountType, accountType);

                if (isOwner || hasRights)
                {
                    isOwner = true;
                    ownerDetailsLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    ownerDetailsLayout.Visibility = ViewStates.Gone;
                }

                addAccount = rootView.FindViewById<Button>(Resource.Id.btnAddAccount);
                TextViewUtils.SetMuseoSans500Typeface(addAccount);
                addAccount.Text = Utility.GetLocalizedLabel("AddAccount", "addAccountCTATitle");
                addAccount.Click += delegate
                {
                    CallValidateAccountService();
                };

                scan = rootView.FindViewById<ImageButton>(Resource.Id.scan);
                scan.Click += async delegate
                {
                    if (!isClicked)
                    {
                        isClicked = true;
                        Intent barcodeIntent = new Intent(Activity, typeof(BarcodeActivity));
                        StartActivityForResult(barcodeIntent, Constants.BARCODE_REQUEST_CODE);
                    }
                };

                btnWhereIsMyAccountNo = rootView.FindViewById<TextView>(Resource.Id.btnWhereIsMyAccountNo);
                btnWhereIsMyAccountNo.Text = Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountTitle");
                btnWhereIsMyAccountNo.Click += async delegate
                {
                    dialogWhereMyAccountNo = new MaterialDialog.Builder(Activity)
                    .CustomView(Resource.Layout.WhereIsMyAccountView, false)
                    .Cancelable(true)
                    .PositiveText(Utility.GetLocalizedLabel("DashboardHome", "gotIt"))
                    .PositiveColor(Resource.Color.blue)
                    .Build();

                    View view = dialogWhereMyAccountNo.View;
                    if (view != null)
                    {
                        TextView titleText = view.FindViewById<TextView>(Resource.Id.textDialogTitle);
                        TextView infoText = view.FindViewById<TextView>(Resource.Id.textDialogInfo);
                        if (titleText != null && infoText != null)
                        {
                            TextViewUtils.SetMuseoSans500Typeface(titleText);
                            TextViewUtils.SetMuseoSans300Typeface(infoText);

                            titleText.Text = Utility.GetLocalizedLabel("AddAccount","whereIsMyAccountTitle");
                            infoText.Text = Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountDetails");
                        }
                    }
                    dialogWhereMyAccountNo.Show();
                };

                AccountType Individual = new AccountType();
                Individual.Id = "1";
                Individual.Type = Utility.GetLocalizedLabel("AddAccount", "residential");
                Individual.IsSelected = true;
                selectedAccountType = Individual;
                accountType.Text = selectedAccountType.Type;
                accountType.Click += async delegate
                {
                    if (!isClicked)
                    {
                        isClicked = true;
                        Intent accountType = new Intent(Activity, typeof(SelectAccountActivity));
                        accountType.PutExtra("selectedAccountType", JsonConvert.SerializeObject(selectedAccountType));
                        StartActivityForResult(accountType, SELECT_ACCOUNT_TYPE_REQ_CODE);
                    }
                };

                edtAccountNo.TextChanged += TextChange;
                edtAccountLabel.TextChanged += TextChange;
                edtOwnersIC.TextChanged += TextChange;

                edtAccountNo.AddTextChangedListener(new InputFilterFormField(edtAccountNo, textInputLayoutAccountNo));
                edtAccountLabel.AddTextChangedListener(new InputFilterFormField(edtAccountLabel, textInputLayoutAccountLabel));
                edtOwnersIC.AddTextChangedListener(new InputFilterFormField(edtOwnersIC, textInputLayoutOwnerIC));

                edtAccountLabel.FocusChange += (sender, e) =>
                {
                    textInputLayoutAccountLabel.Error = null;
                    string accountLabel = edtAccountLabel.Text.Trim();
                    if (e.HasFocus)
                    {
                        textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutFeedbackCount);
                        textInputLayoutAccountLabel.Error = Utility.GetLocalizedHintLabel("nickname");
                    }
                };

                if (Android.OS.Build.Manufacturer.ToLower() == "samsung")
                {
                    edtAccountNo.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                    edtAccountLabel.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                    edtOwnersIC.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                    edtOwnerMotherName.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                }

                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return rootView;
        }

        public override void OnResume()
        {
            base.OnResume();
            isClicked = false;
        }

        public override void OnPause()
        {
            base.OnPause();
            isClicked = true;
        }

        private bool onLongClick(object sender, View.LongClickEventArgs e)
        {
            // Code to execute on item click.
            return true;
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            string accountNo = edtAccountNo.Text.ToString();
            string ic_no = edtOwnersIC.Text;
            string accountName = edtAccountLabel.Text.ToString();
            this.userActionsListener.CheckRequiredFields(accountNo, accountName, isOwner, ic_no);
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == Constants.BARCODE_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {

                        string barcodeResultText = data.GetStringExtra(Constants.BARCODE_RESULT);
                        edtAccountNo.Text = barcodeResultText;

                    }
                }
                else if (requestCode == SELECT_ACCOUNT_TYPE_REQ_CODE)
                {

                    if (resultCode == Result.Ok)
                    {
                        selectedAccountType = JsonConvert.DeserializeObject<AccountType>(data.GetStringExtra("selectedAccountType"));
                        if (selectedAccountType != null)
                        {
                            accountType.Text = selectedAccountType.Type;
                            if (selectedAccountType.Id.Equals("1"))
                            {
                                edtOwnerMotherName.Visibility = ViewStates.Visible;
                                textInputLayoutOwnerIC.Hint = Utility.GetLocalizedCommonLabel("idNumber");
                            }
                            else
                            {
                                edtOwnerMotherName.Visibility = ViewStates.Gone;
                                textInputLayoutOwnerIC.Hint = Activity.GetString(Resource.String.add_account_form_owners_roc_no);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CallAddAccountService()
        {
            try
            {
                string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                string userID = UserEntity.GetActive().UserID;
                string email = UserEntity.GetActive().Email;
                string tnbBillAccountNum = edtAccountNo.Text;
                string tnbAccountHolderICNum = edtOwnersIC.Text;
                string tnbAccountContractNum = edtOwnersIC.Text;
                string type = "1";
                string des = edtAccountLabel.Text; //This has to be changed after added in label
                bool owner = isOwner;
                string suppliedMotherName = edtOwnerMotherName.Text;
                this.userActionsListener.AddAccount(apiKeyID, userID, email, tnbBillAccountNum, tnbAccountHolderICNum, tnbAccountContractNum, type, des, owner, suppliedMotherName);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CallValidateAccountService()
        {
            try
            {
                string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                string accountNum = edtAccountNo.Text;
                string icNumber = edtOwnersIC.Text;
                string type = selectedAccountType.Id;
                bool owner = isOwner;
                string suppliedMotherName = edtOwnerMotherName.Text;
                string accountLabel = edtAccountLabel.Text;
                if (!IsAccountAlreadyRegistered(accountNum) && !AddAccountUtils.IsFoundAccountList(accountNum))
                {
                    this.userActionsListener.ValidateAccount(apiKeyID, accountNum, type, icNumber, suppliedMotherName, owner, accountLabel);
                }
                else
                {
                    edtAccountNo.Error = Utility.GetLocalizedErrorLabel("error_duplicateAccountMessage");
                    edtAccountNo.RequestFocus();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(AddAccountContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowAddAccountFail(string errorMessage)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();

            }

            mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mSnackBar.Dismiss(); }
            );
            View v = mSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mSnackBar.Show();
        }

        public void ShowAddAccountResponse(ServiceResponse response)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();

            }

            mSnackBar = Snackbar.Make(rootView, response.message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mSnackBar.Dismiss(); }
            );
            View v = mSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mSnackBar.Show();

        }

        public void ShowAddAccountSuccess(string message)
        {
            Activity.StartActivity(typeof(LinkAccountActivity));
        }

        public void ShowAddingAccountProgressDialog()
        {
            if (IsActive())
            {
                if (loadingOverlay != null)
                {
                    loadingOverlay.Show();
                }
            }
        }

        public void ShowEmptyAccountNickNameError()
        {
            textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutAccountLabel.Error = "Invalid Account NickName";
        }

        public void ShowEmptyAccountNumberError()
        {
            textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutAccountNo.Error = "Invalid Account Number";
        }

        public void ShowEmptyMothersMaidenNameError()
        {
            textInputLayoutOwnerIC.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutOwnerIC.Error = "Invalid Mother's Maiden Name";
        }

        public void ShowEmptyOwnerIcNumberError()
        {
            textInputLayoutOwnerIC.Error = "Invalid Owner's IC Number";
            textInputLayoutOwnerIC.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
        }

        public void ShowInvalidAccountNumberError()
        {
            textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("accountLength");
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
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            string msg = "Something went wrong, Please try again.";
            if (IsAdded)
            {
                msg = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, msg, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {
                mUknownExceptionSnackBar.Dismiss();
            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public void ShowValidateAccountSucess(NewAccount account)
        {
            Intent link_activity = new Intent(Activity, typeof(LinkAccountActivity));
            link_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(account));
            ((AddAccountActivity)Activity).SetResult(Result.Ok, link_activity);
            ((AddAccountActivity)Activity).Finish();
        }

        public bool IsAccountAlreadyRegistered(string accNum)
        {
            return (CustomerBillingAccount.FindByAccNum(accNum) != null);
        }

        public void ShowEmptyAccountNameError()
        {
            edtAccountLabel.Error = "Enter account nickname";
        }

        public void EnableAddAccountButton()
        {
            addAccount.Enabled = true;
            addAccount.Background = ContextCompat.GetDrawable(Activity, Resource.Drawable.green_button_background);
        }

        public void DisableAddAccountButton()
        {
            addAccount.Enabled = false;
            addAccount.Background = ContextCompat.GetDrawable(Activity, Resource.Drawable.silver_chalice_button_background);
        }

        public void ShowEnterValidAccountName()
        {
            textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutAccountLabel.Error = this.Activity.GetString(Resource.String.invalid_charac);
        }

        public void RemoveNameErrorMessage()
        {
            textInputLayoutAccountLabel.Error = "";
        }

        public void RemoveNumberErrorMessage()
        {
            textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutAccountNo.Error = "";
        }

        public void ShowSameAccountNameError()
        {
            textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
        }
    }
}
