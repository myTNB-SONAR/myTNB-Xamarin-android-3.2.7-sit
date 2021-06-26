﻿using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageSupplyAccount.Activity;
using myTNB_Android.Src.MyAccount.Adapter;
using myTNB_Android.Src.MyAccount.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyAccount.Activity
{
    [Activity(Label = "@string/my_account_activity_title"
        //, Icon = "@drawable/Logo"
        //, MainLauncher = true
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.MyAccount")]
    public class MyAccountActivity : BaseActivityCustom, MyAccountContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

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

        MaterialDialog accountRetrieverDialog;

        const string PAGE_ID = "MyAccount";

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
                // Create your application here
                TextViewUtils.SetMuseoSans300Typeface(
                    txtMyAccountNoAccountContent);
                TextViewUtils.SetMuseoSans500Typeface(btnAddAnotherAccount,
                    btnLogout,
                    btnAddAccount);

                TextViewUtils.SetTextSize12(txtMyAccountNoAccountContent);
                TextViewUtils.SetTextSize14(txtMyAccountNoAccountTitle);
                TextViewUtils.SetTextSize16(btnAddAccount, btnAddAnotherAccount);
                TextViewUtils.SetTextSize18(txtTnBSupplyAccountTitle);

                txtTnBSupplyAccountTitle.Text = GetLabelByLanguage("accountSectionTitle");
                btnAddAnotherAccount.Text = GetLabelCommonByLanguage("addAnotherAcct");
                btnLogout.Text = GetLabelByLanguage("logout");
                txtMyAccountNoAccountTitle.Text = GetLabelByLanguage("noAccounts");
                txtMyAccountNoAccountContent.Text = GetLabelByLanguage("addAccountMessage");
                btnAddAccount.Text = Utility.GetLocalizedLabel("AddAccount", "addAccountCTATitle");

                adapter = new MyAccountAdapter(this, false);
                listView.Adapter = adapter;
                listView.SetNoScroll();
                listView.ItemClick += ListView_ItemClick;

                mPresenter = new MyAccountPresenter(this);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                CustomerBillingAccount customerBillingAccount = adapter.GetItemObject(e.Position);
                ShowManageSupplyAccount(AccountData.Copy(customerBillingAccount, false), e.Position);
            }
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

        [OnClick(Resource.Id.btnAddAccount)]
        void OnClickAddAccount(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                ShowAddAccount();
            }
        }

        [OnClick(Resource.Id.btnAddAnotherAccount)]
        void OnClickAddAnotherAccount(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                ShowAddAccount();
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
                Intent addAccountIntent = new Intent(this, typeof(LinkAccountActivity));
                StartActivity(addAccountIntent);
            }
            catch (Exception e)
            {
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
                Snackbar removeSupplySnackbar = Snackbar.Make(rootView, GetString(Resource.String.my_account_successful_remove_supply_account), Snackbar.LengthIndefinite)
                           .SetAction(Utility.GetLocalizedCommonLabel("close"),
                            (view) =>
                            {

                                // EMPTY WILL CLOSE SNACKBAR
                            }
                           );
                View v = removeSupplySnackbar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                removeSupplySnackbar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearAccountsAdapter()
        {
            adapter.Clear();
        }

        public void ShowAccountRemovedSuccess()
        {
            try
            {
                Snackbar updatePassWordBar = Snackbar.Make(rootView, GetLabelByLanguage("accountDeleteSuccess"), Snackbar.LengthIndefinite)
                            .SetAction(GetLabelCommonByLanguage("close"),
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
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
