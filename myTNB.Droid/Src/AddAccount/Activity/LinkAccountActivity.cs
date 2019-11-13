﻿using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace myTNB_Android.Src.AddAccount.Activity
{
    [Activity(Label = "Add Electricity Account"
        , ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.LinkAccount")]
    public class LinkAccountActivity : BaseActivityCustom, LinkAccountContract.IView
    {

        RecyclerView.LayoutManager layoutManager;
        RecyclerView.LayoutManager layoutManager2;

        private readonly int ADD_ACCOUNT_REQUEST_CODE = 4129;
        private readonly string EG_ACCOUNT_LABEL = "Account ";
        private int ACCOUNT_COUNT = 0;
        private int TOTAL_NO_OF_ACCOUNTS_TO_ADD = 0;

        AccountListAdapter adapter;
        AdditionalAccountListAdapter additionalAdapter;

        List<NewAccount> accountList = new List<NewAccount>();
        List<NewAccount> additionalAccountList = new List<NewAccount>();

        private AlertDialog mDeleteDialog;
        private AlertDialog mNoAccountFoundDialog;

        private AlertDialog mGetAccountsProgressDialog;
        private LoadingOverlay loadingOverlay;
        private MaterialDialog mAddAccountProgressDialog;
        private Snackbar mSnackBar;
        Snackbar mErrorMessageSnackBar;

        [BindView(Resource.Id.account_list_recycler_view)]
        RecyclerView accountListRecyclerView;

        [BindView(Resource.Id.additional_account_list_recycler_view)]
        RecyclerView additionalAccountListRecyclerView;

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.text_no_of_account)]
        TextView textNoOfAcoount;

        [BindView(Resource.Id.label_your_account)]
        TextView labelAccountLabel;

        [BindView(Resource.Id.text_additional_accounts)]
        TextView textAdditionalAcoount;

        [BindView(Resource.Id.layout_additional_accounts)]
        LinearLayout layoutAdditionalAccounts;

        [BindView(Resource.Id.no_account_layout)]
        LinearLayout NoAccountLayout;

        [BindView(Resource.Id.btnAddAnotherAccount)]
        Button btnAddAnotherAccount;

        [BindView(Resource.Id.btnConfirm)]
        Button btnConfirm;

        private LinkAccountPresenter mPresenter;
        private LinkAccountContract.IUserActionsListener userActionsListener;

        private bool fromDashboard = false;
        const string PAGE_ID = "AddAccount";

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.AccountListView;
        }

        public void SetPresenter(LinkAccountContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowGetAccountsProgressDialog()
        {
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

        public void HideGetAccountsProgressDialog()
        {
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

        public void ShowAccountList(List<Account> response)
        {
            try
            {
                if (response != null)
                {
                    if (response.Count > 0)
                    {
                        if (response.Count == 1)
                        {
                            textNoOfAcoount.Text = response.Count + " electricity supply account found!";
                        }
                        else
                        {
                            textNoOfAcoount.Text = response.Count + " electricity supply accounts found!";
                        }

                        labelAccountLabel.Visibility = ViewStates.Visible;
                        for (int i = 0; i < response.Count; i++)
                        {
                            Account item = response[i];
                            if (item != null)
                            {
                                NewAccount accountDetails = new NewAccount()
                                {
                                    isOwner = item.IsOwned,
                                    accountNumber = item.AccountNumber,
                                    accountLabel = item.AccDesc,
                                    accountAddress = item.AccountStAddress,
                                    amCurrentChg = item.AmCurrentChg,
                                    icNum = item.IcNum,
                                    isRegistered = item.IsRegistered,
                                    isPaid = item.IsPaid,
                                    type = item.Type,
                                    userAccountId = item.UserAccountID,
                                    ownerName = item.OwnerName,
                                    accountTypeId = item.AccountTypeId,
                                    accountCategoryId = item.AccountCategoryId

                                };
                                accountList.Add(accountDetails);
                            }
                        }
                        adapter = new AccountListAdapter(this, accountList);
                        accountListRecyclerView.SetAdapter(adapter);
                        adapter.ItemClick += OnItemClick;
                        adapter.NotifyDataSetChanged();
                    }
                }
                else
                {
                    textNoOfAcoount.Text = GetLabelByLanguage("noAccountsTitle");
                    labelAccountLabel.Visibility = ViewStates.Gone;
                    mNoAccountFoundDialog = new AlertDialog.Builder(this)
                    .SetTitle("Sorry")
                    .SetMessage("We could not find any supply accounts for you. Please use Add Account button below, to add accounts")
                    .SetPositiveButton("Ok", (senderAlert, args) =>
                    {
                        mNoAccountFoundDialog.Dismiss();
                    })
                    .SetCancelable(true)
                    .Create();
                    if (!mNoAccountFoundDialog.IsShowing)
                    {
                        mNoAccountFoundDialog.Show();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                NewAccount item = accountList[position];
                mDeleteDialog = new AlertDialog.Builder(this)
                  .SetTitle("Remove Account")
                  .SetMessage("Are you sure! Remove " + item.accountNumber + " from the list ?")
                  .SetPositiveButton("Remove", (senderAlert, args) =>
                  {
                      accountList.Remove(item);
                      adapter = new AccountListAdapter(this, accountList);
                      accountListRecyclerView.SetAdapter(adapter);
                      adapter.ItemClick += OnItemClick;
                      adapter.NotifyDataSetChanged();
                      int totalAccountAdded = adapter.GetAccountList().Count() + additionalAdapter.GetAccountList().Count();
                      if (accountList != null && totalAccountAdded < Constants.ADD_ACCOUNT_LIMIT)
                      {
                          btnAddAnotherAccount.Visibility = ViewStates.Visible;
                      }
                      mDeleteDialog.Dismiss();
                  })

                 .SetNegativeButton("Cancel", (senderAlert, args) =>
                 {
                     mDeleteDialog.Dismiss();
                 })
                  .SetCancelable(false)
                  .Create();
                if (!mDeleteDialog.IsShowing)
                {
                    mDeleteDialog.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnAdditionalItemClick(object sender, int position)
        {
            try
            {
                NewAccount item = additionalAccountList[position];
                mDeleteDialog = new AlertDialog.Builder(this)
                  .SetTitle("Remove Account")
                  .SetMessage("Are you sure! Remove " + item.accountNumber + " from the list ?")
                  .SetPositiveButton("Remove", (senderAlert, args) =>
                  {
                      additionalAccountList.Remove(item);
                      additionalAdapter = new AdditionalAccountListAdapter(this, additionalAccountList);
                      additionalAccountListRecyclerView.SetAdapter(additionalAdapter);
                      additionalAdapter.AdditionalItemClick += OnAdditionalItemClick;
                      additionalAdapter.NotifyDataSetChanged();
                      if (additionalAccountList.Count == 0)
                      {
                          textAdditionalAcoount.Visibility = ViewStates.Gone;
                      }
                      int totalAccountAdded = adapter.GetAccountList().Count + additionalAdapter.GetAccountList().Count();
                      if (accountList != null && totalAccountAdded < Constants.ADD_ACCOUNT_LIMIT)
                      {
                          btnAddAnotherAccount.Visibility = ViewStates.Visible;
                      }
                      mDeleteDialog.Dismiss();
                  })

                 .SetNegativeButton("Cancel", (senderAlert, args) =>
                 {
                     mDeleteDialog.Dismiss();
                 })
                  .SetCancelable(false)
                  .Create();
                if (!mDeleteDialog.IsShowing)
                {
                    mDeleteDialog.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAddAnotherAccountScreen()
        {
            AddAccountUtils.SetAccountList(accountList, additionalAccountList);
            RunOnUiThread(() => StartActivityForResult(typeof(AddAccountActivity), ADD_ACCOUNT_REQUEST_CODE));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new LinkAccountPresenter(this);

                mGetAccountsProgressDialog = new AlertDialog.Builder(this)
                   .SetTitle("Loading..")
                   .SetMessage("Please wait while we are fetching account details")
                   .SetCancelable(false)
                   .Create();

                if (Intent.HasExtra("fromDashboard"))
                {
                    fromDashboard = Intent.Extras.GetBoolean("fromDashboard", false);
                }

                TextViewUtils.SetMuseoSans500Typeface(textNoOfAcoount, btnAddAnotherAccount, btnConfirm);
                TextViewUtils.SetMuseoSans300Typeface(labelAccountLabel);

                textNoOfAcoount.Text = GetLabelByLanguage("noAccountsTitle");
                labelAccountLabel.Text = GetLabelByLanguage("noAcctFoundMsg");
                btnAddAnotherAccount.Text = GetLabelByLanguage("addAnotherAcct");
                btnConfirm.Text = GetLabelCommonByLanguage("confirm");

                adapter = new AccountListAdapter(this, accountList);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                accountListRecyclerView.SetLayoutManager(layoutManager);
                accountListRecyclerView.SetAdapter(adapter);

                additionalAdapter = new AdditionalAccountListAdapter(this, additionalAccountList);
                layoutManager2 = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                additionalAccountListRecyclerView.SetLayoutManager(layoutManager2);
                additionalAccountListRecyclerView.SetAdapter(additionalAdapter);


                //Get apiId and userId from the bundle
                string email = UserEntity.GetActive().UserID;
                string idNumber = UserEntity.GetActive().IdentificationNo; // Get IC number from registration;
                string currentLinkedAccounts = "";
                List<CustomerBillingAccount> savedAccounts = new List<CustomerBillingAccount>();
                savedAccounts = CustomerBillingAccount.List();
                int numberOfAccounts = savedAccounts.Count();
                for (int i = 0; i < numberOfAccounts; i++)
                {
                    currentLinkedAccounts += savedAccounts[i].AccNum;
                    if (i != numberOfAccounts - 1)
                    {
                        currentLinkedAccounts += ",";
                    }

                }
                userActionsListener.GetAccountByIC(Constants.APP_CONFIG.API_KEY_ID, currentLinkedAccounts, email, idNumber);

                btnAddAnotherAccount.Click += delegate
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        ShowAddAnotherAccountScreen();
                    }

                };

                btnConfirm.Click += delegate
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        int totalAccountAdded = adapter.GetAccountList().Count() + additionalAdapter.GetAccountList().Count();
                        if (adapter.ItemCount == 0 && additionalAdapter.ItemCount == 0)
                        {
                            this.userActionsListener.OnConfirm(accountList);
                        }
                        else if (totalAccountAdded > Constants.ADD_ACCOUNT_LIMIT)
                        {
                            string errorLimit = GetString(Resource.String.add_account_link_account_limit_wildcard, Constants.ADD_ACCOUNT_LIMIT.ToString());
                            ShowErrorMessage(errorLimit);
                        }
                        else
                        {
                            CallAddMultileAccountsService();
                        }
                    }
                };

                AddAccountUtils.ClearList();
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Link Accounts Staging");
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

        public void ShowNoAccountAddedError(string message)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return Utility.GetLocalizedLabel("AddAccount", "title");
        }

        public void ClearAdapter()
        {
            // CLEARS ADAPTER
        }

        public void ShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        public void ShowBCRMAccountList(List<BCRMAccount> response)
        {
            try
            {
                if (response != null)
                {
                    ACCOUNT_COUNT = CustomerBillingAccount.List().Count() + 1;
                    if (response.Count > 0)
                    {
                        textNoOfAcoount.Text = response.Count + GetLabelByLanguage("supplyAcctCount");

                        labelAccountLabel.Visibility = ViewStates.Visible;
                        for (int i = 0; i < response.Count; i++)
                        {
                            BCRMAccount item = response[i];
                            if (item != null)
                            {
                                NewAccount accountDetails = new NewAccount()
                                {
                                    isOwner = item.isOwned,
                                    accountNumber = item.accNum,
                                    accountLabel = EG_ACCOUNT_LABEL + ACCOUNT_COUNT,
                                    accountAddress = item.accountStAddress,
                                    ownerName = "",
                                    accountTypeId = item.accountTypeId,
                                    accountCategoryId = item.accountCategoryId,
                                    amCurrentChg = "0",
                                    icNum = item.icNum,
                                    isRegistered = false,
                                    isPaid = false,
                                    type = "1", //
                                    userAccountId = item.accNum,
                                };
                                ACCOUNT_COUNT += 1;
                                accountList.Add(accountDetails);
                            }
                        }
                        adapter = new AccountListAdapter(this, accountList);
                        accountListRecyclerView.SetAdapter(adapter);
                        adapter.ItemClick += OnItemClick;
                        adapter.NotifyDataSetChanged();
                    }
                    else
                    {
                        textNoOfAcoount.Text = GetLabelByLanguage("noAccountsTitle");
                        ShowAddAnotherAccountScreen();
                    }
                }
                else
                {
                    textNoOfAcoount.Text = GetLabelByLanguage("noAccountsTitle");
                    ShowAddAnotherAccountScreen();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CallAddMultileAccountsService()
        {
            try
            {
                List<NewAccount> newList = adapter.GetAccountList();
                List<NewAccount> additionalList = additionalAdapter.GetAccountList();
                if (ValidateAccountNames(newList, additionalList) && ValidateAddtionalAccountNames(additionalList, newList))
                {
                    string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                    string userID = UserEntity.GetActive().UserID;
                    string email = UserEntity.GetActive().Email;
                    List<Models.AddAccount> accounts = new List<Models.AddAccount>();
                    foreach (NewAccount item in newList)
                    {
                        Models.AddAccount account = new Models.AddAccount();
                        account.accountNumber = item.accountNumber;
                        account.accountNickName = item.accountLabel;
                        account.accountStAddress = item.accountAddress;
                        account.icNum = item.icNum;
                        account.isOwned = item.isOwner;
                        account.accountTypeId = item.type;
                        account.accountCategoryId = item.accountCategoryId;
                        accounts.Add(account);
                    }
                    foreach (NewAccount item in additionalList)
                    {
                        Models.AddAccount account = new Models.AddAccount();
                        account.accountNumber = item.accountNumber;
                        account.accountNickName = item.accountLabel;
                        account.accountStAddress = item.accountAddress;
                        account.icNum = item.icNum;
                        account.isOwned = item.isOwner;
                        account.accountTypeId = item.type;
                        account.accountCategoryId = item.accountCategoryId;
                        accounts.Add(account);
                    }
                    TOTAL_NO_OF_ACCOUNTS_TO_ADD = accounts.Count;
                    this.userActionsListener.AddMultipleAccounts(apiKeyID, userID, email, accounts);
                }
                else
                {
                    ShowErrorMessage("Please enter valid account label.");
                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool ValidateLinkAccountList(List<NewAccount> list)
        {
            bool flag = true;
            try
            {

                foreach (NewAccount item in list)
                {
                    if (item.accountLabel.Equals(EG_ACCOUNT_LABEL) || item.accountLabel.Contains("eg.") || item.accountLabel.Contains("(") || item.accountLabel.Contains(")") || String.IsNullOrEmpty(item.accountLabel))
                    {
                        flag = false;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return flag;
        }

        public bool ValidateAccountNames(List<NewAccount> list, List<NewAccount> addtionalList)
        {

            bool flag = true;
            try
            {

                List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
                int index = 0;
                int additioanlIndex = 0;
                int currentItemIndex = 0;
                foreach (NewAccount item in list)
                {
                    if (!string.IsNullOrEmpty(item.accountLabel))
                    {
                        if (String.IsNullOrEmpty(item.accountLabel))
                        {
                            flag = false;
                            if (currentItemIndex < list.Count())
                            {
                                AccountListViewHolder vh = (AccountListViewHolder)accountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                                if (vh != null)
                                {
                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                    vh.textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
                                }
                            }
                            break;
                        }

                        foreach (CustomerBillingAccount savedItem in accounts)
                        {
                            if (savedItem.AccDesc.ToLower().Trim().Equals(item.accountLabel.ToString().ToLower().Trim()))
                            {
                                flag = false;
                                if (index < list.Count())
                                {
                                    AccountListViewHolder vh = (AccountListViewHolder)accountListRecyclerView.FindViewHolderForAdapterPosition(index);
                                    if (vh != null)
                                    {
                                        vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                        vh.textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
                                    }
                                }
                                break;
                            }
                            index++;
                        }

                        int itemOccurance = 0;
                        foreach (NewAccount addiAccount in list)
                        {
                            if (addiAccount.accountLabel.ToLower().Trim().Equals(item.accountLabel.ToString().ToLower().Trim()))
                            {
                                itemOccurance++;
                            }
                        }
                        if (itemOccurance > 1)
                        {
                            flag = false;

                            if (currentItemIndex < list.Count())
                            {
                                AccountListViewHolder vh = (AccountListViewHolder)accountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                                if (vh != null)
                                {
                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                    vh.textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
                                }
                            }
                            break;
                        }

                        foreach (NewAccount addiAccount in addtionalList)
                        {
                            if (addiAccount.accountLabel.ToLower().Trim().Equals(item.accountLabel.ToString().ToLower().Trim()))
                            {
                                flag = false;
                                if (additioanlIndex < list.Count())
                                {
                                    AccountListViewHolder vh = (AccountListViewHolder)accountListRecyclerView.FindViewHolderForAdapterPosition(additioanlIndex);
                                    if (vh != null)
                                    {
                                        vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                        vh.textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
                                    }
                                }
                                break;
                            }
                            additioanlIndex++;
                        }
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                    currentItemIndex++;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return flag;
        }

        public bool ValidateAddtionalAccountNames(List<NewAccount> aditionallist, List<NewAccount> newlist)
        {
            bool flag = true;
            try
            {
                List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
                int index = 0;
                int newlistIndex = 0;
                int currentItemIndex = 0;
                foreach (NewAccount item in aditionallist)
                {
                    if (!string.IsNullOrEmpty(item.accountLabel))
                    {

                        if (String.IsNullOrEmpty(item.accountLabel))
                        {
                            flag = false;
                            if (currentItemIndex < aditionallist.Count())
                            {
                                AdditionalAccountViewHolder vh = (AdditionalAccountViewHolder)additionalAccountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                                if (vh != null)
                                {
                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                    vh.textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
                                }
                            }
                            break;
                        }

                        foreach (CustomerBillingAccount savedItem in accounts)
                        {
                            if (savedItem.AccDesc.ToLower().Trim().Equals(item.accountLabel.ToString().ToLower().Trim()))
                            {
                                flag = false;
                                if (index < aditionallist.Count())
                                {
                                    AdditionalAccountViewHolder vh = (AdditionalAccountViewHolder)additionalAccountListRecyclerView.FindViewHolderForAdapterPosition(index);
                                    if (vh != null)
                                    {
                                        vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                        vh.textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
                                    }
                                }
                                break;
                            }
                            index++;
                        }

                        int itemOccurance = 0;
                        foreach (NewAccount addiAccount in aditionallist)
                        {
                            if (addiAccount.accountLabel.ToLower().Trim().Equals(item.accountLabel.ToString().ToLower().Trim()))
                            {
                                itemOccurance++;
                            }
                        }
                        if (itemOccurance > 1)
                        {
                            flag = false;
                            if (currentItemIndex < aditionallist.Count())
                            {
                                AdditionalAccountViewHolder vh = (AdditionalAccountViewHolder)additionalAccountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                                if (vh != null)
                                {
                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                    vh.textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
                                }
                            }
                            break;
                        }

                        foreach (NewAccount addiAccount in newlist)
                        {
                            if (addiAccount.accountLabel.ToLower().Trim().Equals(item.accountLabel.ToString().ToLower().Trim()))
                            {
                                flag = false;
                                if (newlistIndex < newlist.Count())
                                {
                                    AccountListViewHolder vh = (AccountListViewHolder)accountListRecyclerView.FindViewHolderForAdapterPosition(newlistIndex);
                                    if (vh != null)
                                    {
                                        vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                        vh.textInputLayoutAccountLabel.Error = Utility.GetLocalizedErrorLabel("duplicateNickname");
                                    }
                                }
                                break;
                            }
                            newlistIndex++;
                        }
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                    currentItemIndex++;
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return flag;
        }

        public void ShowErrorMessage()
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, "Something went wrong! Please try again later", Snackbar.LengthIndefinite)
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }

        public void ShowErrorMessage(string message)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == ADD_ACCOUNT_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        NewAccount account = JsonConvert.DeserializeObject<NewAccount>(data.Extras.GetString(Constants.SELECTED_ACCOUNT));
                        if (account != null)
                        {
                            bool alreadyAdded = false;
                            foreach (NewAccount item in accountList)
                            {
                                if (item.accountNumber.Equals(account.accountNumber))
                                {
                                    alreadyAdded = true;
                                }
                            }
                            foreach (NewAccount item in additionalAccountList)
                            {
                                if (item.accountNumber.Equals(account.accountNumber))
                                {
                                    alreadyAdded = true;
                                }
                            }

                            if (!alreadyAdded)
                            {
                                additionalAccountList.Add(account);
                                if (adapter.ItemCount == 0 && additionalAccountList.Count() > 0)
                                {
                                    NoAccountLayout.Visibility = ViewStates.Gone;
                                    textNoOfAcoount.Visibility = ViewStates.Gone;
                                    labelAccountLabel.Visibility = ViewStates.Gone;
                                }
                                additionalAdapter = new AdditionalAccountListAdapter(this, additionalAccountList);
                                additionalAccountListRecyclerView.SetAdapter(additionalAdapter);
                                additionalAdapter.AdditionalItemClick += OnAdditionalItemClick;
                                additionalAdapter.NotifyDataSetChanged();
                                textAdditionalAcoount.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                ShowErrorMessage("Account already added!!");
                            }
                        }

                        //Hide add account button if account list count >= 10
                        int totalAccountAdded = adapter.GetAccountList().Count() + additionalAdapter.GetAccountList().Count();
                        if (accountList != null && totalAccountAdded >= Constants.ADD_ACCOUNT_LIMIT)
                        {
                            btnAddAnotherAccount.Visibility = ViewStates.Gone;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAddAccountSuccess(AddMultipleAccountResponse.Response response)
        {

            try
            {
                int ctr = 0;
                bool hasAlreadyExistingSelected = CustomerBillingAccount.HasSelected();

                List<NewAccount> finalAccountList = new List<NewAccount>();
                foreach (Models.AddAccount item in response.Data)
                {
                    foreach (NewAccount newAccount in accountList)
                    {
                        if (newAccount.accountNumber.Equals(item.accountNumber))
                        {
                            newAccount.accountAddress = item.accountStAddress;
                            newAccount.ownerName = item.accountOwnerName;
                            newAccount.smartMeterCode = item.smartMeterCode == null ? "0" : item.smartMeterCode;
                            newAccount.isOwned = item.isOwned;
                            newAccount.IsTaggedSMR = item.IsTaggedSMR == "true" ? true : false;
                            finalAccountList.Add(newAccount);
                        }
                    }

                    foreach (NewAccount extraAccount in additionalAccountList)
                    {
                        if (extraAccount.accountNumber.Equals(item.accountNumber))
                        {
                            extraAccount.accountAddress = item.accountStAddress;
                            extraAccount.ownerName = item.accountOwnerName;
                            extraAccount.smartMeterCode = item.smartMeterCode == null ? "0" : item.smartMeterCode;
                            extraAccount.isOwned = item.isOwned;
                            extraAccount.IsTaggedSMR = item.IsTaggedSMR == "true" ? true : false;
                            finalAccountList.Add(extraAccount);
                        }
                    }
                }

                foreach (NewAccount newAccount in finalAccountList)
                {
                    CustomerBillingAccount.InsertOrReplace(newAccount, false);

                    ctr++;
                }

                SummaryDashBoardAccountEntity.RemoveAll();

                Intent sucessIntent = new Intent(this, typeof(AddAccountSuccessActivity));
                sucessIntent.PutExtra("Accounts", JsonConvert.SerializeObject(finalAccountList));
                StartActivity(sucessIntent);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAddAccountFail(string errorMessage)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();

            }

            mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
            .SetAction("Close", delegate
            {
                mSnackBar.Dismiss();
            });
            View v = mSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            mSnackBar.Show();
            this.SetIsClicked(false);
        }

        public void ShowAddingAccountProgressDialog()
        {
            try
            {
                if (IsActive())
                {
                    mAddAccountProgressDialog = new MaterialDialog.Builder(this)
                        .CustomView(Resource.Layout.CustomDialogLayout, false)
                        .Cancelable(true)
                        .Build();

                    View view = mAddAccountProgressDialog.View;
                    if (view != null)
                    {
                        string title = TOTAL_NO_OF_ACCOUNTS_TO_ADD == 1 ? "Adding 1 Account" : string.Format("Adding {0} Accounts", TOTAL_NO_OF_ACCOUNTS_TO_ADD);
                        string message = TOTAL_NO_OF_ACCOUNTS_TO_ADD == 1 ? "This may take a while as we add your TNB account." : "This may take a while as we add your TNB accounts.";
                        TextView titleText = view.FindViewById<TextView>(Resource.Id.txtTitle);
                        TextView infoText = view.FindViewById<TextView>(Resource.Id.txtMessage);
                        titleText.Text = title;
                        infoText.Text = message;
                        if (titleText != null && infoText != null)
                        {
                            TextViewUtils.SetMuseoSans500Typeface(titleText);
                            TextViewUtils.SetMuseoSans300Typeface(infoText);
                        }
                    }


                    if (mAddAccountProgressDialog != null && !mAddAccountProgressDialog.IsShowing)
                    {
                        mAddAccountProgressDialog.Show();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideAddingAccountProgressDialog()
        {
            try
            {
                if (IsActive())
                {
                    if (mAddAccountProgressDialog != null && mAddAccountProgressDialog.IsShowing)
                    {
                        mAddAccountProgressDialog.Dismiss();
                    }
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

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
                RetryGetCusterByIc();
            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            mCancelledExceptionSnackBar.Show();
            this.SetIsClicked(false);
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
                RetryGetCusterByIc();
            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            mApiExcecptionSnackBar.Show();
            this.SetIsClicked(false);
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
                RetryGetCusterByIc();
            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        public void RetryGetCusterByIc()
        {
            //Get apiId and userId from the bundle
            string email = UserEntity.GetActive().UserID;
            string idNumber = UserEntity.GetActive().IdentificationNo;  // Get IC number from registration;
            userActionsListener.GetAccountByIC(Constants.APP_CONFIG.API_KEY_ID, "", email, idNumber);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if (fromDashboard)
                ShowDashboard();
            else
                this.Finish();
        }


        public void ShowBCRMDownException(String msg)
        {
            MaterialDialog bcrmDownDialog;
            bcrmDownDialog = new MaterialDialog.Builder(this)
                .Title("Error")
                .Content(msg)
                .PositiveText(Utility.GetLocalizedCommonLabel("ok"))
                .OnPositive((dialog, which) => NavigateToDashboard())
                .Cancelable(false)
                .Build();
            bcrmDownDialog.Show();
        }


        public void NavigateToDashboard()
        {
            if (fromDashboard)
                ShowDashboard();
            else
                this.Finish();
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

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
