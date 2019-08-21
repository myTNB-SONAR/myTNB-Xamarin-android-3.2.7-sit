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
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.LogoutEnd.Activity;
using myTNB_Android.Src.ManageCards.Activity;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.ManageSupplyAccount.Activity;
using myTNB_Android.Src.MyAccount.Adapter;
using myTNB_Android.Src.MyAccount.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using myTNB_Android.Src.UpdatePassword.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.MyAccount.Activity
{
    [Activity(Label = "@string/my_account_activity_title"
        //, Icon = "@drawable/Logo"
        //, MainLauncher = true
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.MyAccount")]
    public class MyAccountActivity : BaseToolbarAppCompatActivity, MyAccountContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtMyAccountTitle)]
        TextView txtMyAccountTitle;

        [BindView(Resource.Id.textInputLayoutFullName)]
        TextInputLayout textInputLayoutFullName;
        [BindView(Resource.Id.txtFullName)]
        TextView txtFullName;

        [BindView(Resource.Id.textInputLayoutIcNo)]
        TextInputLayout textInputLayoutIcNo;
        [BindView(Resource.Id.txtIcNo)]
        TextView txtIcNo;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;
        [BindView(Resource.Id.txtEmail)]
        TextView txtEmail;

        [BindView(Resource.Id.txtInputLayoutMobileNo)]
        TextInputLayout txtInputLayoutMobileNo;
        [BindView(Resource.Id.txtMobileNo)]
        TextView txtMobileNo;
        [BindView(Resource.Id.btnTextUpdateMobileNo)]
        TextView btnTextUpdateMobileNo;

        [BindView(Resource.Id.txtInputLayoutPassword)]
        TextInputLayout txtInputLayoutPassword;
        [BindView(Resource.Id.txtPassword)]
        TextView txtPassword;
        [BindView(Resource.Id.btnTextUpdatePassword)]
        TextView btnTextUpdatePassword;

        [BindView(Resource.Id.txtInputLayoutCards)]
        TextInputLayout txtInputLayoutCards;
        [BindView(Resource.Id.txtCards)]
        TextView txtCards;
        [BindView(Resource.Id.btnTextUpdateCards)]
        TextView btnTextUpdateCards;

        [BindView(Resource.Id.txtTnBSupplyAccountTitle)]
        TextView txtTnBSupplyAccountTitle;


        [BindView(Resource.Id.listView)]
        ListView listView;

        [BindView(Resource.Id.btnAddAnotherAccount)]
        Button btnAddAnotherAccount;

        [BindView(Resource.Id.btnLogout)]
        Button btnLogout;

        [BindView(Resource.Id.btnAddAccount)]
        Button btnAddAccount;

        [BindView(Resource.Id.no_account_layout)]
        FrameLayout NoAccountLayout;

        [BindView(Resource.Id.txtMyAccountNoAccountTitle)]
        TextView txtMyAccountNoAccountTitle;

        [BindView(Resource.Id.txtMyAccountNoAccountContent)]
        TextView txtMyAccountNoAccountContent;

        MyAccountAdapter adapter;

        MyAccountContract.IUserActionsListener userActionsListener;
        MyAccountPresenter mPresenter;

        MaterialDialog accountRetrieverDialog, logoutProgressDialog;
        private LoadingOverlay loadingOverlay;

        public override int ResourceId()
        {
            return Resource.Layout.MyAccountView;
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
                accountRetrieverDialog = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.my_account_account_retrieval_progress_title))
                    .Content(GetString(Resource.String.my_account_account_retrieval_progress_content))
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                logoutProgressDialog = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.logout_activity_title))
                    .Content(GetString(Resource.String.logout_app_question))
                    .PositiveText(GetString(Resource.String.manage_cards_btn_ok))
                    .NeutralText(GetString(Resource.String.bill_related_feedback_selection_cancel))
                    .OnPositive((dialog, which) => this.userActionsListener.OnLogout(this.DeviceId()))
                    .OnNeutral((dialog, which) => dialog.Dismiss())
                    .Build();


                // Create your application here
                TextViewUtils.SetMuseoSans300Typeface(textInputLayoutFullName,
                    textInputLayoutIcNo,
                    txtInputLayoutEmail,
                    txtInputLayoutMobileNo,
                    txtInputLayoutCards,
                    txtInputLayoutPassword);
                TextViewUtils.SetMuseoSans300Typeface(txtFullName,
                    txtIcNo,
                    txtEmail,
                    txtMobileNo,
                    txtCards,
                    txtMyAccountNoAccountContent,
                    txtPassword);
                TextViewUtils.SetMuseoSans500Typeface(txtMyAccountTitle,
                    btnAddAnotherAccount,
                    btnLogout,
                    btnAddAccount,
                    txtMyAccountNoAccountTitle,
                    btnTextUpdatePassword,
                    btnTextUpdateMobileNo,
                    btnTextUpdateCards,
                    txtTnBSupplyAccountTitle);

                adapter = new MyAccountAdapter(this, false);
                listView.Adapter = adapter;
                listView.SetNoScroll();
                listView.ItemClick += ListView_ItemClick;

                mPresenter = new MyAccountPresenter(this);
                this.userActionsListener.Start();

                Bundle extras = Intent.Extras;
                if (extras != null && extras.ContainsKey(Constants.FORCE_UPDATE_PHONE_NO))
                {
                    if (extras.GetBoolean(Constants.FORCE_UPDATE_PHONE_NO))
                    {
                        UserEntity entity = UserEntity.GetActive();
                        ShowMobileUpdateSuccess(entity.MobileNo);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            CustomerBillingAccount customerBillingAccount = adapter.GetItemObject(e.Position);
            this.userActionsListener.OnManageSupplyAccount(customerBillingAccount, e.Position);
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

                mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.my_account_cancelled_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.my_account_cancelled_exception_btn_close), delegate
                {

                    mCancelledExceptionSnackBar.Dismiss();
                }
                );
                mCancelledExceptionSnackBar.Show();
            }
            catch (Exception e)
            {
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

                mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.my_account_api_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.my_account_api_exception_btn_close), delegate
                {

                    mApiExcecptionSnackBar.Dismiss();
                }
                );
                mApiExcecptionSnackBar.Show();
            }
            catch (Exception e)
            {
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

                mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.my_account_unknown_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.my_account_unknown_exception_btn_close), delegate
                {

                    mUknownExceptionSnackBar.Dismiss();
                }
                );
                mUknownExceptionSnackBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnTextUpdateMobileNo)]
        void OnClickMobile(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnUpdateMobileNo();
        }

        [OnClick(Resource.Id.btnTextUpdatePassword)]
        void OnClickPassword(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnUpdatePassword();
        }

        [OnClick(Resource.Id.btnTextUpdateCards)]
        void OnClickUpdateCards(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnManageCards();
        }

        [OnClick(Resource.Id.btnAddAccount)]
        void OnClickAddAccount(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnAddAccount();
        }

        [OnClick(Resource.Id.btnAddAnotherAccount)]
        void OnClickAddAnotherAccount(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnAddAccount();
        }

        [OnClick(Resource.Id.btnLogout)]
        void OnClickLogout(object sender, EventArgs eventArgs)
        {
            try
            {
                //this.userActionsListener.OnLogout(this.DeviceId());
                if (IsActive())
                {
                    logoutProgressDialog.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowUpdateMobileNo()
        {
            try
            {
                Intent updateMobileNo = new Intent(this, typeof(UpdateMobileActivity));
                StartActivityForResult(updateMobileNo, Constants.UPDATE_MOBILE_NO_REQUEST);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowUpdatePassword()
        {
            try
            {
                Intent updateMobileNo = new Intent(this, typeof(UpdatePasswordActivity));
                StartActivityForResult(updateMobileNo, Constants.UPDATE_PASSWORD_REQUEST);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowManageCards(List<CreditCardData> cardList)
        {
            try
            {
                Intent manageCard = new Intent(this, typeof(ManageCardsActivity));
                manageCard.PutExtra(Constants.CREDIT_CARD_LIST, JsonConvert.SerializeObject(cardList));
                StartActivityForResult(manageCard, Constants.MANAGE_CARDS_REQUEST);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowManageSupplyAccount(AccountData accountData, int position)
        {
            try
            {
                Intent manageAccount = new Intent(this, typeof(ManageSupplyAccountActivity));
                manageAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                manageAccount.PutExtra(Constants.SELECTED_ACCOUNT_POSITION, position);
                StartActivityForResult(manageAccount, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAddAccount()
        {
            try
            {
                Intent updateMobileNo = new Intent(this, typeof(LinkAccountActivity));
                StartActivity(updateMobileNo);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowLogout()
        {
            try
            {
                ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
                Intent logout = new Intent(this, typeof(LogoutEndActivity));
                StartActivity(logout);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowProgressDialog()
        {
            //if (accountRetrieverDialog != null && accountRetrieverDialog.IsShowing)
            //{
            //    accountRetrieverDialog.Dismiss();
            //}
            //accountRetrieverDialog.Show();
            try
            {
                if (IsActive())
                {
                    if (loadingOverlay != null && loadingOverlay.IsShowing)
                    {
                        loadingOverlay.Dismiss();
                    }

                    loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                    loadingOverlay.Show();
                }
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
                FirebaseAnalyticsUtils.SetScreenName(this, "More -> My Account");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideShowProgressDialog()
        {
            //if (accountRetrieverDialog != null && accountRetrieverDialog.IsShowing)
            //{
            //    accountRetrieverDialog.Dismiss();
            //}
            try
            {
                if (IsActive())
                {
                    if (loadingOverlay != null && loadingOverlay.IsShowing)
                    {
                        loadingOverlay.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(MyAccountContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void ShowAccountList(List<CustomerBillingAccount> accountList)
        {
            try
            {
                adapter.AddAll(accountList);
                adapter.NotifyDataSetChanged();
                listView.SetNoScroll();
                btnAddAnotherAccount.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyAccount()
        {
            try
            {
                listView.EmptyView = NoAccountLayout;
                btnAddAnotherAccount.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowUserData(UserEntity user, int numOfCards)
        {
            txtFullName.Text = user.DisplayName;

            try
            {

                if (user.IdentificationNo.Count() >= 4)
                {
                    string lastDigit = user.IdentificationNo.Substring(user.IdentificationNo.Length - 4);

                    txtIcNo.Text = GetString(Resource.String.my_account_ic_no_mask) + " " + lastDigit;
                }
                else
                {
                    txtIcNo.Text = GetString(Resource.String.my_account_ic_no_mask);
                }

                txtEmail.Text = user.Email;
                txtMobileNo.Text = user.MobileNo;
                txtPassword.Text = GetString(Resource.String.my_account_dummy_password);
                txtCards.Text = string.Format("{0}", numOfCards);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowMobileUpdateSuccess(string newPhone)
        {
            try
            {
                txtMobileNo.Text = newPhone;
                Snackbar updatePhoneSnackBar = Snackbar.Make(rootView, GetString(Resource.String.my_account_successful_update_mobile_no), Snackbar.LengthIndefinite)
                            .SetAction(GetString(Resource.String.my_account_successful_update_mobile_no_btn),
                             (view) =>
                             {

                             // EMPTY WILL CLOSE SNACKBAR
                         }
                            );
                View v = updatePhoneSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updatePhoneSnackBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowPasswordUpdateSuccess()
        {
            try
            {
                Snackbar updatePassWordBar = Snackbar.Make(rootView, GetString(Resource.String.my_account_successful_update_password), Snackbar.LengthIndefinite)
                            .SetAction(GetString(Resource.String.my_account_successful_update_password_btn),
                             (view) =>
                             {

                             // EMPTY WILL CLOSE SNACKBAR
                         }
                            );
                View v = updatePassWordBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updatePassWordBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRemovedCardSuccess(CreditCardData creditCard, int numOfCards)
        {
            try
            {
                string lastDigits = creditCard.LastDigits.Substring(creditCard.LastDigits.Length - 4);
                txtCards.Text = string.Format("{0}", numOfCards);
                Snackbar.Make(rootView, GetString(Resource.String.manage_cards_card_remove_successfully_wildcard, lastDigits), Snackbar.LengthIndefinite)
                           .SetAction(GetString(Resource.String.manage_cards_btn_close),
                            (view) =>
                            {

                            // EMPTY WILL CLOSE SNACKBAR
                        }
                           ).Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowGetCardsProgressDialog()
        {
            //throw new NotImplementedException();
        }

        public void HideGetCardsProgressDialog()
        {
            //throw new NotImplementedException();
        }

        public void ShowRemovedSupplyAccountSuccess(AccountData accountData, int position)
        {
            try
            {
                adapter.Remove(position);
                Snackbar.Make(rootView, GetString(Resource.String.my_account_successful_remove_supply_account), Snackbar.LengthIndefinite)
                           .SetAction(GetString(Resource.String.my_account_successful_remove_supply_account_btn),
                            (view) =>
                            {

                            // EMPTY WILL CLOSE SNACKBAR
                        }
                           ).Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearAccountsAdapter()
        {
            adapter.Clear();
        }

        public void EnableManageCards()
        {
            try
            {
                btnTextUpdateCards.Enabled = true;
                btnTextUpdateCards.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisableManageCards()
        {
            try
            {
                btnTextUpdateCards.Enabled = false;
                btnTextUpdateCards.SetTextColor(Resources.GetColor(Resource.Color.silverChalice));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowLogoutErrorMessage(string message)
        {
            try
            {
                Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
                            .SetAction(GetString(Resource.String.logout_rate_btn_close),
                             (view) =>
                             {

                             // EMPTY WILL CLOSE SNACKBAR
                         }
                            ).Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowLogoutProgressDialog()
        {
            //if (logoutProgressDialog != null && !logoutProgressDialog.IsShowing)
            //{
            //    logoutProgressDialog.Show();
            //}
            try
            {
                if (IsActive())
                {
                    if (loadingOverlay != null && loadingOverlay.IsShowing)
                    {
                        loadingOverlay.Dismiss();
                    }

                    loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                    loadingOverlay.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideLogoutProgressDialog()
        {
            //if (logoutProgressDialog != null && logoutProgressDialog.IsShowing)
            //{
            //    logoutProgressDialog.Dismiss();
            //}
            try
            {
                if (IsActive())
                {
                    if (loadingOverlay != null && loadingOverlay.IsShowing)
                    {
                        loadingOverlay.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAccountRemovedSuccess()
        {
            try
            {
                Snackbar updatePassWordBar = Snackbar.Make(rootView, GetString(Resource.String.manage_supply_account_removed_success), Snackbar.LengthIndefinite)
                            .SetAction(GetString(Resource.String.my_account_successful_update_password_btn),
                             (view) =>
                             {

                             // EMPTY WILL CLOSE SNACKBAR
                         }
                            );

                View v = updatePassWordBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(4);
                updatePassWordBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}