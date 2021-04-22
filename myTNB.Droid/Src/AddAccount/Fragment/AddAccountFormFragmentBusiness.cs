using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.MVP;
using myTNB_Android.Src.Barcode.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;

namespace myTNB_Android.Src.AddAccount.Fragment
{
    public class AddAccountFormFragmentBusiness : AndroidX.Fragment.App.Fragment , AddAccountContract.IView, View.IOnTouchListener
    {
        private static string TAG = "AddAccountForm";
        private bool isOwner = true;
        private bool hasRights = false;
        private AccountType selectedAccountType;
        private readonly int SELECT_ACCOUNT_TYPE_REQ_CODE = 2011;

        Button addAccount;

        private Snackbar mSnackBar;
        private MaterialDialog dialogWhereMyAccountNo;

        private AddAccountPresenter mPresenter;
        private AddAccountContract.IUserActionsListener userActionsListener;

        LinearLayout skip_add_acc;

        LinearLayout rootView;

        [BindView(Resource.Id.account_no_edittext)]
        EditText edtAccountNo;

        [BindView(Resource.Id.account_label_edittext)]
        EditText edtAccountLabel;

        [BindView(Resource.Id.add_roc_account_number)]
        EditText edtRocNo;

        [BindView(Resource.Id.owner_mother_maiden_name_edittext)]
        EditText edtOwnerMotherName;

        [BindView(Resource.Id.account_no_layout)]
        TextInputLayout textInputLayoutAccountNo;

        [BindView(Resource.Id.account_label_layout)]
        TextInputLayout textInputLayoutAccountLabel;

        [BindView(Resource.Id.roc_label_layout)]
        TextInputLayout textInputLayoutRocNo;

        [BindView(Resource.Id.owner_mother_maiden_name_layout)]
        TextInputLayout textInputLayoutMotherMaidenName;

        [BindView(Resource.Id.txtAccountType)]
        TextView txtAccountType;

        [BindView(Resource.Id.btnWhereIsMyAccountNo)]
        TextView btnWhereIsMyAccountNo;

        [BindView(Resource.Id.selector_account_type)]
        TextView accountType;

        [BindView(Resource.Id.txtTitle)]
        TextView txtTitle;

        [BindView(Resource.Id.txtTitleROC)]
        TextView txtTitleROC;

        [BindView(Resource.Id.txtTitlePremise)]
        TextView txtTitlePremise;

        [BindView(Resource.Id.txtSkipAcc)]
        TextView txtSkipAcc;

        private bool isClicked = false;

        private InputFilterFormField mFormField;

        private bool fromRegisterPage = false;

        public void ClearText()
        {
            edtAccountNo.Text = "";
            edtAccountLabel.Text = "";
            edtRocNo.Text = "";
            edtAccountNo.ClearFocus();
            edtAccountLabel.ClearFocus();
            edtRocNo.ClearFocus();
        }

        public void HideAddingAccountProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return this.IsAdded && this.IsVisible;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //isOwner = Arguments.GetBoolean("isOwner");
            hasRights = Arguments.GetBoolean("hasRights");
            mPresenter = new AddAccountPresenter(this);

            if (Arguments != null)
            {
                Bundle bundle = this.Arguments;
                fromRegisterPage = bundle.GetBoolean("fromRegisterPage", true);
            }
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mainView = inflater.Inflate(Resource.Layout.AddAccountTypeViewNewBusiness, container, false);
            try
            {
                rootView = mainView.FindViewById<LinearLayout>(Resource.Id.rootView);
                edtAccountNo = mainView.FindViewById<EditText>(Resource.Id.account_no_edittext);
                edtAccountLabel = mainView.FindViewById<EditText>(Resource.Id.account_label_edittext);
                edtRocNo = mainView.FindViewById<EditText>(Resource.Id.add_roc_account_number);
                edtOwnerMotherName = mainView.FindViewById<EditText>(Resource.Id.owner_mother_maiden_name_edittext);
                skip_add_acc = rootView.FindViewById<LinearLayout>(Resource.Id.layoutSkipAcc);

                textInputLayoutAccountNo = mainView.FindViewById<TextInputLayout>(Resource.Id.account_no_layout);
                textInputLayoutAccountLabel = mainView.FindViewById<TextInputLayout>(Resource.Id.account_label_layout);
                textInputLayoutMotherMaidenName = mainView.FindViewById<TextInputLayout>(Resource.Id.owner_mother_maiden_name_layout);
                textInputLayoutRocNo = mainView.FindViewById<TextInputLayout>(Resource.Id.roc_label_layout);
                txtAccountType = mainView.FindViewById<TextView>(Resource.Id.txtAccountType);
                txtSkipAcc = mainView.FindViewById<TextView>(Resource.Id.txtSkipAcc);
                txtTitle = mainView.FindViewById<TextView>(Resource.Id.txtTitle);
                txtTitlePremise = mainView.FindViewById<TextView>(Resource.Id.txtTitlePremise);
                txtTitleROC = mainView.FindViewById<TextView>(Resource.Id.txtTitleROC);
                accountType = mainView.FindViewById<TextView>(Resource.Id.selector_account_type);
                btnWhereIsMyAccountNo = mainView.FindViewById<TextView>(Resource.Id.btnWhereIsMyAccountNo);

                txtAccountType.Text = Utility.GetLocalizedLabel("AddAccount", "PremisesHint").ToUpper();
                textInputLayoutAccountNo.Hint = Utility.GetLocalizedLabel("AddAccount", "accNumber");
                textInputLayoutAccountLabel.Hint = Utility.GetLocalizedLabel("Common","acctNickname");
                textInputLayoutRocNo.Hint = Utility.GetLocalizedLabel("AddAccount", "rocNumberOptional").ToUpper();
                //txtTitlePremise.Hint = Utility.GetLocalizedLabel("AddAccount", "AccHeaderText");
                txtTitlePremise.Text = Utility.GetLocalizedLabel("AddAccount", "AccHeaderText");
                txtTitle.Text = Utility.GetLocalizedLabel("AddAccount", "ROCHeaderText");
                txtTitleROC.Text = Utility.GetLocalizedLabel("AddAccount", "ROCDetailsText");
                btnWhereIsMyAccountNo.Hint = Utility.GetLocalizedLabel("AddAccount", "WhereAccNo");
               
                txtAccountType.TextSize = TextViewUtils.GetFontSize(18);
                txtTitlePremise.TextSize = TextViewUtils.GetFontSize(18);
                txtTitle.TextSize = TextViewUtils.GetFontSize(18);
                txtTitleROC.TextSize = TextViewUtils.GetFontSize(18);
                btnWhereIsMyAccountNo.TextSize = TextViewUtils.GetFontSize(18);
                accountType.TextSize = TextViewUtils.GetFontSize(14);
                txtSkipAcc.TextSize = TextViewUtils.GetFontSize(14);

                TextViewUtils.SetMuseoSans300Typeface(edtAccountLabel
                    , edtAccountNo
                    , edtRocNo
                    , edtOwnerMotherName);

                TextViewUtils.SetMuseoSans300Typeface(textInputLayoutAccountNo
                    , textInputLayoutAccountLabel
                    , textInputLayoutMotherMaidenName
                    , textInputLayoutRocNo);

                TextViewUtils.SetMuseoSans300Typeface(txtAccountType, accountType, txtTitle, accountType, txtTitlePremise, btnWhereIsMyAccountNo);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleROC, txtSkipAcc);

                if (fromRegisterPage)
                {
                    fromRegisterPage = true;
                    skip_add_acc.Visibility = ViewStates.Visible;
                }
                else
                {
                    skip_add_acc.Visibility = ViewStates.Gone;
                }

                skip_add_acc.Click += delegate
                {
                    Bundle bundle = new Bundle();
                    bundle.PutBoolean("fromRegister", true);
                    ((AddAccountActivity)Activity).nextFragment(this, bundle);
                };

                addAccount = rootView.FindViewById<Button>(Resource.Id.btnAddAnotherAccount);
                TextViewUtils.SetMuseoSans500Typeface(addAccount);
                addAccount.Text = Utility.GetLocalizedLabel("Common", "next");
                addAccount.Click += delegate
                {
                    CallValidateAccountService();
                };

                btnWhereIsMyAccountNo = rootView.FindViewById<TextView>(Resource.Id.btnWhereIsMyAccountNo);
                btnWhereIsMyAccountNo.Text = Utility.GetLocalizedLabel("AddAccount", "whereIsMyAccountTitle");
                TextViewUtils.SetMuseoSans500Typeface(btnWhereIsMyAccountNo);
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
                Individual.Id = "2";
                Individual.Type = Utility.GetLocalizedLabel("AddAccount", "business");
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
                edtRocNo.TextChanged += TextChange;

                edtAccountNo.AddTextChangedListener(new InputFilterFormField(edtAccountNo, textInputLayoutAccountNo));
                edtAccountLabel.AddTextChangedListener(new InputFilterFormField(edtAccountLabel, textInputLayoutAccountLabel));
                mFormField = new InputFilterFormField(edtRocNo, textInputLayoutRocNo);
                edtRocNo.AddTextChangedListener(mFormField);

                edtAccountLabel.FocusChange += (sender, e) =>
                {
                    textInputLayoutAccountLabel.HelperText = "";
                    string accountLabel = edtAccountLabel.Text.Trim();
                    if (e.HasFocus)
                    {
                        textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutFeedbackCount);
                        textInputLayoutAccountLabel.HelperText = Utility.GetLocalizedHintLabel("nickname");
                    }
                    try
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            try
                            {
                                ViewGroup.MarginLayoutParams lp3 = (ViewGroup.MarginLayoutParams)txtAccountType.LayoutParameters;
                                lp3.TopMargin = (int)DPUtils.ConvertDPToPx(8f);
                                txtAccountType.LayoutParameters = lp3;
                                txtAccountType.RequestLayout();
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        });
                    }
                    catch (Exception exp)
                    {
                        Utility.LoggingNonFatalError(exp);
                    }
                };

                if (Android.OS.Build.Manufacturer.ToLower() == "samsung")
                {
                    edtAccountNo.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                    edtAccountLabel.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                    edtRocNo.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                    edtOwnerMotherName.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                }

                edtAccountNo.SetOnTouchListener(this);

                this.userActionsListener.Start();

                ClearText();
                ClearAllErrorFields();

                textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);               
                textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                textInputLayoutRocNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);

                edtAccountNo.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_account_no, 0, Resource.Drawable.scan, 0);
                edtAccountLabel.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_name, 0, 0, 0);
                edtRocNo.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_account_no, 0, 0, 0);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return rootView;
        }

        public void ClearAllErrorFields()
        {
            if (!string.IsNullOrEmpty(textInputLayoutAccountNo.Error))
            {
                textInputLayoutAccountNo.Error = null;
                textInputLayoutAccountNo.ErrorEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutAccountLabel.HelperText))
            {
                textInputLayoutAccountLabel.HelperText = null;
                textInputLayoutAccountLabel.HelperTextEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutRocNo.Error))
            {
                textInputLayoutRocNo.Error = null;
                textInputLayoutRocNo.ErrorEnabled = false;
            }
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
            string ic_no = edtRocNo.Text;
            string accountName = edtAccountLabel.Text.ToString();
            this.userActionsListener.CheckRequiredFields(accountNo, accountName, isOwner, ic_no);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == Constants.BARCODE_REQUEST_CODE)
                {
                    if (resultCode == (int) Result.Ok)
                    {

                        string barcodeResultText = data.GetStringExtra(Constants.BARCODE_RESULT);
                        edtAccountNo.Text = barcodeResultText;

                    }
                }
                else if (requestCode == SELECT_ACCOUNT_TYPE_REQ_CODE)
                {

                    if (resultCode == (int) Result.Ok)
                    {
                        selectedAccountType = JsonConvert.DeserializeObject<AccountType>(data.GetStringExtra("selectedAccountType"));
                        if (selectedAccountType != null)
                        {
                            accountType.Text = selectedAccountType.Type;
                            if (selectedAccountType.Id.Equals("1"))
                            {
                                Bundle bundle = new Bundle();
                                bundle.PutBoolean("isResidential", true);
                                ((AddAccountActivity)Activity).nextFragment(this, bundle);
                            }
                            else
                            {
                                edtOwnerMotherName.Visibility = ViewStates.Gone;
                                edtRocNo.RemoveTextChangedListener(mFormField);
                                textInputLayoutRocNo.Hint = Utility.GetLocalizedLabel("AddAccount", "rocNumberOptional");
                                mFormField = new InputFilterFormField(edtRocNo, textInputLayoutRocNo);
                                edtRocNo.AddTextChangedListener(mFormField);
                                if (edtRocNo.HasFocus)
                                {
                                    edtRocNo.RequestFocus();
                                }
                                else
                                {
                                    edtRocNo.RequestFocus();
                                    edtRocNo.ClearFocus();
                                }
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
                string tnbAccountHolderICNum = edtRocNo.Text;
                string tnbAccountContractNum = edtRocNo.Text;
                string type = "2";
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
                string icNumber = edtRocNo.Text;
                string type = selectedAccountType.Id;
                bool owner;
                string suppliedMotherName = edtOwnerMotherName.Text;
                string accountLabel = edtAccountLabel.Text;

                if(icNumber.Equals(""))
                {
                    owner = false;
                }
                else 
                {
                    owner = true;
                }

                if (!IsAccountAlreadyRegistered(accountNum) && !AddAccountUtils.IsFoundAccountList(accountNum))
                {
                    this.userActionsListener.ValidateAccount(apiKeyID, accountNum, type, icNumber, suppliedMotherName, owner, accountLabel);
                }
                else
                {
                    MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(Utility.GetLocalizedErrorLabel("error_duplicateAccountTitle"))
                        .SetMessage(Utility.GetLocalizedErrorLabel("error_duplicateAccountMessageNew"))
                        .SetContentGravity(GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                        .Build().Show();
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
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyAccountNickNameError()
        {
            textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutAccountLabel.HelperText = "Invalid Account NickName";          
        }

        public void ShowEmptyAccountNumberError()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                        textInputLayoutAccountNo.Error = "Invalid Account Number";
                        textInputLayoutAccountNo.RequestLayout();
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }          
        }

        public void ShowEmptyMothersMaidenNameError()
        {
            textInputLayoutRocNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutRocNo.Error = "Invalid Mother's Maiden Name";
        }

        public void ShowEmptyOwnerIcNumberError()
        {
            textInputLayoutRocNo.Error = "Invalid Owner's IC Number";
            textInputLayoutRocNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
        }

        public void ShowInvalidAccountNumberError()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);

                        if (textInputLayoutAccountNo.Error != Utility.GetLocalizedErrorLabel("accountLengthNew"))
                        {
                            textInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("accountLengthNew");  // fix bouncing issue
                        }
                        
                        textInputLayoutAccountNo.RequestLayout();
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
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

            string msg = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            
            mUknownExceptionSnackBar = Snackbar.Make(rootView, msg, Snackbar.LengthIndefinite)
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
            textInputLayoutAccountLabel.HelperText = this.Activity.GetString(Resource.String.invalid_charac);
        }

        public void RemoveNameErrorMessage()
        {
            textInputLayoutAccountLabel.HelperText = "";
            
        }

        public void RemoveNumberErrorMessage()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                        textInputLayoutAccountNo.Error = "";
                        textInputLayoutAccountNo.RequestLayout();
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowSameAccountNameError()
        {
            textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);

            if (textInputLayoutAccountLabel.HelperText != Utility.GetLocalizedErrorLabel("duplicateNickname"))
            {
                textInputLayoutAccountLabel.HelperText = Utility.GetLocalizedErrorLabel("duplicateNickname");  // fix bouncing issue
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            const int DRAWABLE_LEFT = 0;
            const int DRAWABLE_TOP = 1;
            const int DRAWABLE_RIGHT = 2;
            const int DRAWABLE_BOTTOM = 3;
            if (v is EditText)
            {
                EditText eTxtView = v as EditText;
                if (eTxtView.Id == Resource.Id.account_no_edittext)
                {
                    edtAccountNo.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, Resource.Drawable.scan, 0);
                    if (e.RawX >= (edtAccountNo.Right - edtAccountNo.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                    {
                        if (!isClicked)
                        {
                            isClicked = true;
                            Intent barcodeIntent = new Intent(Activity, typeof(BarcodeActivity));
                            StartActivityForResult(barcodeIntent, Constants.BARCODE_REQUEST_CODE);
                        }

                        return true;
                    }
                }
            }
            return false;
        }

        public void GovermentDialog()
        {
            throw new NotImplementedException();
        }
    }
}
