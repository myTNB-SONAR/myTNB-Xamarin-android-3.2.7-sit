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
using static Android.Resource;

namespace myTNB_Android.Src.AddAccount.Fragment
{
    public class AddAccountFormFragmentNonOwner : AndroidX.Fragment.App.Fragment , AddAccountContract.IView, View.IOnTouchListener
    {
        private static string TAG = "AddAccountForm";
        private bool isOwner = false;
        private bool hasRights = false;
        private AccountType selectedAccountType;
        private readonly int SELECT_ACCOUNT_TYPE_REQ_CODE = 2011;

        Button addAccount;

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

        [BindView(Resource.Id.btnWhereIsMyAccountNo)]
        TextView btnWhereIsMyAccountNo;

        [BindView(Resource.Id.txtNonOwnerTitle)]
        TextView txtNonOwnerTitle;

        private bool isClicked = false;

        UserEntity entity = UserEntity.GetActive();

        private InputFilterFormField mFormField;

        public void ClearText()
        {
            edtAccountNo.Text = "";
            edtAccountLabel.Text = "";
            edtAccountNo.ClearFocus();
            edtAccountLabel.ClearFocus();
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
            isOwner = Arguments.GetBoolean("isOwner");
            hasRights = Arguments.GetBoolean("hasRights");
            mPresenter = new AddAccountPresenter(this);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mainView = inflater.Inflate(Resource.Layout.AddAccountFormViewNonOwner, container, false);
            try
            {
                rootView = mainView.FindViewById<LinearLayout>(Resource.Id.rootView);
                edtAccountNo = mainView.FindViewById<EditText>(Resource.Id.account_no_edittext);
                edtAccountLabel = mainView.FindViewById<EditText>(Resource.Id.account_label_edittext);
                edtOwnersIC = mainView.FindViewById<EditText>(Resource.Id.owner_ic_no_edittext);
                edtOwnerMotherName = mainView.FindViewById<EditText>(Resource.Id.owner_mother_maiden_name_edittext);

                textInputLayoutAccountNo = mainView.FindViewById<TextInputLayout>(Resource.Id.account_no_layout);
                textInputLayoutAccountLabel = mainView.FindViewById<TextInputLayout>(Resource.Id.account_label_layout);
                textInputLayoutMotherMaidenName = mainView.FindViewById<TextInputLayout>(Resource.Id.owner_mother_maiden_name_layout);
                textInputLayoutOwnerIC = mainView.FindViewById<TextInputLayout>(Resource.Id.owner_ic_no_layout);
                txtNonOwnerTitle = mainView.FindViewById<TextView>(Resource.Id.txtNonOwnerTitle);

                //accountType = mainView.FindViewById<TextView>(Resource.Id.selector_account_type);

                txtNonOwnerTitle.Text = Utility.GetLocalizedLabel("Common", "titleNonOwnerAddAcc");
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

                TextViewUtils.SetMuseoSans300Typeface(txtNonOwnerTitle);

                addAccount = rootView.FindViewById<Button>(Resource.Id.btnAddAccount);
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
                Individual.Id = "1";
                Individual.Type = Utility.GetLocalizedLabel("AddAccount", "residential");
                Individual.IsSelected = true;
                selectedAccountType = Individual;

                edtAccountNo.TextChanged += TextChange;
                edtAccountLabel.TextChanged += TextChange;

                edtAccountNo.AddTextChangedListener(new InputFilterFormField(edtAccountNo, textInputLayoutAccountNo));
                edtAccountLabel.AddTextChangedListener(new InputFilterFormField(edtAccountLabel, textInputLayoutAccountLabel));
                mFormField = new InputFilterFormField(edtOwnersIC, textInputLayoutOwnerIC);

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
                                ViewGroup.MarginLayoutParams lp3 = (ViewGroup.MarginLayoutParams)txtNonOwnerTitle.LayoutParameters;
                                lp3.TopMargin = (int)DPUtils.ConvertDPToPx(8f);
                                txtNonOwnerTitle.LayoutParameters = lp3;
                                txtNonOwnerTitle.RequestLayout();
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
                    edtOwnersIC.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                    edtOwnerMotherName.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                }

                edtAccountNo.SetOnTouchListener(this);

                this.userActionsListener.Start();

                ClearText();
                ClearAllErrorFields();

                textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);                
                textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);

                edtAccountNo.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_account_no, 0, Resource.Drawable.scan, 0);
                edtAccountLabel.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_name, 0, 0, 0);

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
            if (!string.IsNullOrEmpty(textInputLayoutAccountLabel.Error))
            {
                textInputLayoutAccountLabel.Error = null;
                textInputLayoutAccountLabel.ErrorEnabled = false;
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
            string ic_no = entity.IdentificationNo;
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
                            //accountType.Text = selectedAccountType.Type;
                            if (selectedAccountType.Id.Equals("1"))
                            {
       
                            }
                            else
                            {
                              
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
                string icNumber = entity.IdentificationNo;
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
                    textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                    textInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("error_duplicateAccountMessage");
                    /*MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(Utility.GetLocalizedErrorLabel("error_duplicateAccountTitle"))
                        .SetMessage(Utility.GetLocalizedErrorLabel("error_duplicateAccountMessage"))
                        .SetContentGravity(GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                        .Build().Show();*/
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

            textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            textInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("error_NotExistsAccountMessage");
            /* if (mSnackBar != null && mSnackBar.IsShown)
             {
                 mSnackBar.Dismiss();

             }

             mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
             .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mSnackBar.Dismiss(); }
             );
             View v = mSnackBar.View;
             TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
             tv.SetMaxLines(5);

             mSnackBar.Show();*/
        }

        public void GovermentDialog()
        {
           string data = Utility.GetLocalizedLabel("AddAccount", "GovDialogDetails");
            string temp = string.Format(data);
            MyTNBAppToolTipBuilder.Create(Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                       .SetTitle((string.Format(Utility.GetLocalizedLabel("AddAccount", "GovDialogTitle"))))
                       .SetMessage(temp)
                       .SetContentGravity(GravityFlags.Center)
                       .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                       .Build().Show();
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
            textInputLayoutAccountLabel.Error = "Invalid Account NickName";
            
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
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        textInputLayoutAccountNo.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);

                        if (textInputLayoutAccountNo.Error != Utility.GetLocalizedErrorLabel("accountLength"))
                        {
                            textInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("accountLength");  // fix bouncing issue
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
            textInputLayoutAccountLabel.Error = this.Activity.GetString(Resource.String.invalid_charac);
            
        }

        public void RemoveNameErrorMessage()
        {
            textInputLayoutAccountLabel.Error = "";
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

            if (textInputLayoutAccountLabel.Error != Utility.GetLocalizedErrorLabel("duplicateNickname"))
            {
                textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");  // fix bouncing issue
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
                    if (e.RawX >= (edtAccountNo.Right - edtAccountNo.GetCompoundDrawables()[2].Bounds.Width()))
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
    }
}
