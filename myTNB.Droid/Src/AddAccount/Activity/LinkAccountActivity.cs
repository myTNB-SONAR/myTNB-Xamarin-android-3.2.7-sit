using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using Android.Support.V7.Widget;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.MVP;
using Android.Util;
using Android.Support.Design.Widget;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.AddAccount.Adapter;
using Newtonsoft.Json;
using Refit;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.SummaryDashBoard.Models;
using System.Runtime;

namespace myTNB_Android.Src.AddAccount.Activity
{
    [Activity(Label = "Add Electricity Account"
        , ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.LinkAccount")]
    public class LinkAccountActivity : BaseToolbarAppCompatActivity, LinkAccountContract.IView
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

        private LinkAccountPresenter mPresenter;
        private LinkAccountContract.IUserActionsListener userActionsListener;
        private Button done;

        private bool fromDashboard = false;

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
            //if (this.mGetAccountsProgressDialog != null && !this.mGetAccountsProgressDialog.IsShowing)
            //{
            //    this.mGetAccountsProgressDialog.Show();
            //}
            try {
            if (IsActive()) {
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

        public void HideGetAccountsProgressDialog()
        {
            //if (this.mGetAccountsProgressDialog != null && this.mGetAccountsProgressDialog.IsShowing)
            //{
            //    this.mGetAccountsProgressDialog.Dismiss();
            //}
            try {
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

        public void ShowAccountList(List<Account> response)
        {
            try {
            if (response != null)
            { 
            if (response.Count > 0)
            {
                    if(response.Count == 1)
                    {
                        textNoOfAcoount.Text = response.Count + " electricity supply account found!";
                    }
                    else {
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
                                accountAddress = item.AccountStAddress ,
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
                textNoOfAcoount.Text = "No Accounts Found!";
                labelAccountLabel.Visibility = ViewStates.Gone;
                mNoAccountFoundDialog = new AlertDialog.Builder(this)
                .SetTitle("Sorry")
                .SetMessage("We could not find any supply accounts for you. Please use Add Account button below, to add accounts")
                .SetPositiveButton("Ok", (senderAlert, args) => {
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
            try {
            NewAccount item = accountList[position];
            mDeleteDialog = new AlertDialog.Builder(this)
              .SetTitle("Remove Account")
              .SetMessage("Are you sure! Remove " + item.accountNumber + " from the list ?")
              .SetPositiveButton("Remove", (senderAlert, args) => {
                  accountList.Remove(item);
                  adapter = new AccountListAdapter(this, accountList);
                  accountListRecyclerView.SetAdapter(adapter);
                  adapter.ItemClick += OnItemClick;
                  adapter.NotifyDataSetChanged();
                  int totalAccountAdded = adapter.GetAccountList().Count() + additionalAdapter.GetAccountList().Count();
                  if (accountList != null && totalAccountAdded < Constants.ADD_ACCOUNT_LIMIT)
                  {
                      done.Visibility = ViewStates.Visible;
                  }
                  mDeleteDialog.Dismiss();
              })

             .SetNegativeButton("Cancel", (senderAlert, args) => {
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
            try {
            NewAccount item = additionalAccountList[position];
            mDeleteDialog = new AlertDialog.Builder(this)
              .SetTitle("Remove Account")
              .SetMessage("Are you sure! Remove " + item.accountNumber + " from the list ?")
              .SetPositiveButton("Remove", (senderAlert, args) => {
                  additionalAccountList.Remove(item);
                  additionalAdapter = new AdditionalAccountListAdapter(this, additionalAccountList);
                  additionalAccountListRecyclerView.SetAdapter(additionalAdapter);
                  additionalAdapter.AdditionalItemClick += OnAdditionalItemClick;
                  additionalAdapter.NotifyDataSetChanged();
                  if(additionalAccountList.Count == 0)
                  {
                      textAdditionalAcoount.Visibility = ViewStates.Gone;
                  }
                  int totalAccountAdded = adapter.GetAccountList().Count + additionalAdapter.GetAccountList().Count();
                  if (accountList != null && totalAccountAdded < Constants.ADD_ACCOUNT_LIMIT)
                  {
                      done.Visibility = ViewStates.Visible;
                  }
                  mDeleteDialog.Dismiss();
              })

             .SetNegativeButton("Cancel", (senderAlert, args) => {
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
            RunOnUiThread(() => StartActivityForResult(typeof(AddAccountActivity), ADD_ACCOUNT_REQUEST_CODE));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
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

            TextViewUtils.SetMuseoSans500Typeface(textNoOfAcoount);
            TextViewUtils.SetMuseoSans300Typeface(labelAccountLabel);

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
            List<CustomerBillingAccount> savedAccounts = CustomerBillingAccount.List();
            int numberOfAccounts = savedAccounts.Count();
            for(int i=0; i < numberOfAccounts; i++)
            {
                currentLinkedAccounts += savedAccounts[i].AccNum;
                if(i != numberOfAccounts-1)
                {
                    currentLinkedAccounts += ",";
                }

            }
                //userActionsListener.GetAccounts(userID, apiID);
            userActionsListener.GetAccountByIC(Constants.APP_CONFIG.API_KEY_ID, currentLinkedAccounts, email, idNumber);

            done = FindViewById<Button>(Resource.Id.btnAddAnotherAccount);
            done.Click += delegate
            {
                ShowAddAnotherAccountScreen();

            };

            Button confirm = FindViewById<Button>(Resource.Id.btnConfirm);
            confirm.Click += delegate
            {
                int totalAccountAdded = adapter.GetAccountList().Count() + additionalAdapter.GetAccountList().Count();
                if (adapter.ItemCount == 0 && additionalAdapter.ItemCount == 0)
                {
                    //ShowNoAccountAddedError("No account added. Please click Add Another account button to add electricity account.");
                    this.userActionsListener.OnConfirm(accountList);
                }
                else if(totalAccountAdded > Constants.ADD_ACCOUNT_LIMIT)
                {
                    string errorLimit = GetString(Resource.String.add_account_link_account_limit_wildcard, Constants.ADD_ACCOUNT_LIMIT.ToString());
                    ShowErrorMessage(errorLimit);
                }
                else
                {
                    CallAddMultileAccountsService();
                }


                // TODO : START ACTIVITY DASHBOARD

                //this.userActionsListener.OnConfirm(accountList);
                
            };

            TextViewUtils.SetMuseoSans500Typeface(done, confirm);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("Link Account", "OnResume");
            //userActionsListener.Start();

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
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return GetString(Resource.String.add_electricity_account_title);
        }

        public void ClearAdapter()
        {
            // CLEARS ADAPTER
        }

        public void ShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        public void ShowBCRMAccountList(List<BCRMAccount> response)
        {
            try {
            if (response != null)
            {
                ACCOUNT_COUNT = CustomerBillingAccount.List().Count() + 1;
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
                    textNoOfAcoount.Text = "No Accounts Found!";
                    ShowAddAnotherAccountScreen();
                }
            }
            else
            {
                textNoOfAcoount.Text = "No Accounts Found!";
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
            try {
            List<NewAccount> newList = adapter.GetAccountList();
            List<NewAccount> additionalList = additionalAdapter.GetAccountList();
            //if (ValidateLinkAccountList(newList) && ValidateLinkAccountList(additionalList) && ValidateAccountNames(newList, additionalList) && ValidateAddtionalAccountNames(additionalList, newList))
            if (ValidateAccountNames(newList, additionalList) && ValidateAddtionalAccountNames(additionalList, newList))
            {
                string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                string userID = UserEntity.GetActive().UserID; //"20225235-290c-484a-a633-607cb51b15e6";
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
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool ValidateLinkAccountList(List<NewAccount> list)
        {
            bool flag = true;
            try {

            foreach (NewAccount item in list)
            {
                if(item.accountLabel.Equals(EG_ACCOUNT_LABEL) || item.accountLabel.Contains("eg.") || item.accountLabel.Contains("(") || item.accountLabel.Contains(")") || String.IsNullOrEmpty(item.accountLabel))
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
            try {

            List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
            int index = 0;
            int additioanlIndex = 0;
            int currentItemIndex = 0;
            foreach (NewAccount item in list)
            {
                if (!string.IsNullOrEmpty(item.accountLabel))
                {
                    if (item.accountLabel.Equals(EG_ACCOUNT_LABEL) || item.accountLabel.Contains("eg.") || item.accountLabel.Contains("(") || item.accountLabel.Contains(")") || String.IsNullOrEmpty(item.accountLabel))
                    {
                        flag = false;
                        if (currentItemIndex < list.Count())
                        {
                            AccountListViewHolder vh = (AccountListViewHolder)accountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                            if (vh != null)
                            {
                                vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
                            }
                        }
                        break;
                    }

                    if (!Utility.isAlphaNumeric(item.accountLabel))
                    {
                        flag = false;
                        AccountListViewHolder vh = (AccountListViewHolder)accountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                        if (vh != null)
                        {
                            vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.invalid_charac);
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
                                    vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
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
                    if(itemOccurance > 1)
                    {
                        flag = false;
                        
                        if (currentItemIndex < list.Count())
                        {
                            AccountListViewHolder vh = (AccountListViewHolder)accountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                            if (vh != null)
                            {
                                vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
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
                                    vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
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

                        if (item.accountLabel.Equals(EG_ACCOUNT_LABEL) || item.accountLabel.Contains("eg.") || item.accountLabel.Contains("(") || item.accountLabel.Contains(")") || String.IsNullOrEmpty(item.accountLabel))
                        {
                            flag = false;
                            if (currentItemIndex < aditionallist.Count())
                            {
                                AdditionalAccountViewHolder vh = (AdditionalAccountViewHolder)additionalAccountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                                if (vh != null)
                                {
                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                                    vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
                                }
                            }
                            break;
                        }

                        if (!Utility.isAlphaNumeric(item.accountLabel))
                        {
                            flag = false;
                            AdditionalAccountViewHolder vh = (AdditionalAccountViewHolder)additionalAccountListRecyclerView.FindViewHolderForAdapterPosition(currentItemIndex);
                            if (vh != null)
                            {
                                vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.invalid_charac);
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
                                        vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
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
                                    vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
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
                                        vh.textInputLayoutAccountLabel.Error = GetString(Resource.String.add_account_duplicate_account_nickname);
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

            } catch(Exception e) {
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
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try {
            //base.OnActivityResult(requestCode, resultCode, data);
            if(requestCode == ADD_ACCOUNT_REQUEST_CODE)
            {
                if(resultCode == Result.Ok)
                {
                    NewAccount account = JsonConvert.DeserializeObject<NewAccount>(data.Extras.GetString(Constants.SELECTED_ACCOUNT));
                    if(account != null)
                    {
                        bool alreadyAdded = false;
                        foreach(NewAccount item in accountList)
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
                            if (adapter.ItemCount == 0  && additionalAccountList.Count() > 0) {
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
                        done.Visibility = ViewStates.Gone;
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

            try {
            int ctr = 0;
            bool hasAlreadyExistingSelected = CustomerBillingAccount.HasSelected();

            List<NewAccount> finalAccountList = new List<NewAccount>();
            foreach(Models.AddAccount item in response.Data)
            {
                foreach(NewAccount newAccount in accountList)
                {
                    if (newAccount.accountNumber.Equals(item.accountNumber))
                    {
                        newAccount.accountAddress = item.accountStAddress;
                        newAccount.ownerName = item.accountOwnerName;
                        newAccount.smartMeterCode = item.smartMeterCode == null ? "0" : item.smartMeterCode;
                        finalAccountList.Add(newAccount);
                    }
                }

                foreach(NewAccount extraAccount in additionalAccountList)
                {
                    if (extraAccount.accountNumber.Equals(item.accountNumber))
                    {
                        extraAccount.accountAddress = item.accountStAddress;
                        extraAccount.ownerName = item.accountOwnerName;
                        extraAccount.smartMeterCode = item.smartMeterCode == null ? "0" : item.smartMeterCode;
                        finalAccountList.Add(extraAccount);
                    }
                }
            }
            //finalAccountList.AddRange(accountList);
            //finalAccountList.AddRange(additionalAccountList);



            /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/

            //List<CustomerBillingAccount> customerBiilingAccounts = new List<CustomerBillingAccount>();
            //CustomerBillingAccount.RemoveSelected();
            //int i = 0;
            //if (SummaryDashBoardAccountEntity.GetAllItems().Count() < Constants.SUMMARY_DASHBOARD_PAGE_COUNT)
            //{
            //    i = Constants.SUMMARY_DASHBOARD_PAGE_COUNT - SummaryDashBoardAccountEntity.GetAllItems().Count();
            //}


            //List<NewAccount> ReAccountList = (from item in finalAccountList where item.accountCategoryId == "2" select item).ToList();

            //List<NewAccount> NormalAccountList = (from item in finalAccountList where item.accountCategoryId != "2" select item).ToList();
            //bool isREAccountSelected = false;
            //if (ReAccountList != null && ReAccountList.Count() > 0) {
            //    foreach (NewAccount newAccount in ReAccountList)
            //    {
            //        bool isSelected = ctr == 0 ? true : false;
            //        CustomerBillingAccount.InsertOrReplace(newAccount, isSelected);
            //        isREAccountSelected = true;
            //        if (ctr > 0) {
            //            customerBiilingAccounts.Add(CustomerBillingAccount.FindByAccNum(newAccount.accountNumber));    
            //        }
            //        ctr++;
            //    }
            //    customerBiilingAccounts.Add(CustomerBillingAccount.GetSelected());
            //}


            //if (NormalAccountList != null && NormalAccountList.Count() > 0)
            //{
            //    if (!isREAccountSelected) {
            //        i =  (customerBiilingAccounts.Count() > 0 && customerBiilingAccounts.Count() >= i) ? 0 : i;
            //    }

            //    foreach (NewAccount newAccount in NormalAccountList)
            //    {
                    
            //            bool isSelected = (ctr == 0 && !isREAccountSelected) ? true : false;
            //            CustomerBillingAccount.InsertOrReplace(newAccount, isSelected);    
                     

            //        if (ctr > 0 && i > 1 && ctr < i)
            //        {
            //            customerBiilingAccounts.Add(CustomerBillingAccount.FindByAccNum(newAccount.accountNumber));
            //        }
            //        ctr++;
            //    }
            //    if (!isREAccountSelected)
            //    {
            //        customerBiilingAccounts.Add(CustomerBillingAccount.GetSelected());
            //    }
            //}
            //if (ConnectionUtils.HasInternetConnection(this))
            //{
            //    userActionsListener.InsertingInSummarydashBoard(customerBiilingAccounts);
            //}

            /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/



            foreach (NewAccount newAccount in finalAccountList)
            {
                //if (hasAlreadyExistingSelected)
                //{
                //    CustomerBillingAccount.InsertOrReplace(newAccount, false);
                //}
                //else
                //{
                    //bool isSelected = ctr == 0 ? true : false;
                CustomerBillingAccount.InsertOrReplace(newAccount, false);
                //}

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
            .SetAction("Close", delegate {
                mSnackBar.Dismiss();
                //CallAddMultileAccountsService();
            });
            View v = mSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            mSnackBar.Show();
        }

        public void ShowAddingAccountProgressDialog()
        {
            try {
            if (IsActive()) {
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
            try {
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

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.add_account_link_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.add_account_link_cancelled_exception_btn_retry), delegate {

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
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.add_account_link_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.add_account_link_api_exception_btn_retry), delegate {

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
        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.add_account_link_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.add_account_link_unknown_exception_btn_retry), delegate {

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
        }

        public void RetryGetCusterByIc()
        {
            //Get apiId and userId from the bundle
            string email = UserEntity.GetActive().UserID;
            string idNumber = UserEntity.GetActive().IdentificationNo; //"919191919"; // Get IC number from registration;
            //userActionsListener.GetAccounts(userID, apiID);
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
                .PositiveText(GetString(Resource.String.manage_cards_btn_ok))
                .OnPositive((dialog, which) => NavigateToDashboard())
                .Cancelable(false)
                .Build();
            bcrmDownDialog.Show();
        }


        public void NavigateToDashboard(){
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
    }
}