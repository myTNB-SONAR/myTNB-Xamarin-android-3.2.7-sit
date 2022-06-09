using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.Mobile;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.MVP;
using myTNB_Android.Src.AddAccountDisclaimer.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace myTNB_Android.Src.AddAccount.Activity
{
    [Activity(Label = "Add Electricity Account"
        , ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class LinkAccountActivity : BaseActivityCustom, LinkAccountContract.IView
    {

        RecyclerView.LayoutManager layoutManager;
        RecyclerView.LayoutManager layoutManager2;

        private readonly int ADD_ACCOUNT_REQUEST_CODE = 4129;
        private readonly int FROM_REGISTER_CODE = 4130;
        private readonly string EG_ACCOUNT_LABEL = "Account ";
        private int ACCOUNT_COUNT = 0;
        private int TOTAL_NO_OF_ACCOUNTS_TO_ADD = 0;

        AccountListAdapter adapter;
        AdditionalAccountListAdapter additionalAdapter;

        List<NewAccount> accountList = new List<NewAccount>();
        List<NewAccount> additionalAccountList = new List<NewAccount>();

        private AlertDialog mGetAccountsProgressDialog;
        private MaterialDialog mAddAccountProgressDialog;
        private AlertDialog mDeleteDialog;
        private Snackbar mSnackBar;
        Snackbar mErrorMessageSnackBar;

        [BindView(Resource.Id.account_list_recycler_view)]
        RecyclerView accountListRecyclerView;

        [BindView(Resource.Id.additional_account_list_recycler_view)]
        RecyclerView additionalAccountListRecyclerView;

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.text_no_of_account)]
        TextView textNoOfAccount;

        [BindView(Resource.Id.label_your_account)]
        TextView labelAccountLabel;

        [BindView(Resource.Id.text_additional_accounts)]
        TextView textAdditionalAccount;

        [BindView(Resource.Id.label_additional_accounts)]
        TextView textlabelAdditionalAcount;

        [BindView(Resource.Id.layout_additional_accounts)]
        LinearLayout layoutAdditionalAccounts;

        [BindView(Resource.Id.no_account_layout)]
        LinearLayout NoAccountLayout;

        [BindView(Resource.Id.txtTermsConditions)]
        TextView txtTermsConditions;

        [BindView(Resource.Id.checkboxCondition)]
        CheckBox txtboxcondition;

        [BindView(Resource.Id.btnAddAnotherAccount)]
        Button btnAddAnotherAccount;

        [BindView(Resource.Id.btnConfirm)]
        Button btnConfirm;

        [BindView(Resource.Id.checkboxlayout)]
        LinearLayout checkboxlayout;
        

        private LinkAccountPresenter mPresenter;
        private LinkAccountContract.IUserActionsListener userActionsListener;

        private bool fromDashboard = false;

        private bool fromRegisterPage = false;

        private bool checkedCondition = false;

        const string PAGE_ID = "AddAccount";

        private ISharedPreferences mSharedPref;

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
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
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
                  .SetTitle(GetLabelByLanguage("removeAcct"))
                  .SetPositiveButton(GetLabelCommonByLanguage("ok"), (senderAlert, args) =>
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
                      if (accountList.Count() > 0)
                      {
                          //textNoOfAcoount.Text = accountList.Count() + " " + GetLabelByLanguage("supplyAccountCount");
                          textNoOfAccount.Text = string.Format(Utility.GetLocalizedLabel("AddAccount", "OwnerDetectTitle"), accountList.Count());

                      }
                      
                      TextViewUtils.SetTextSize18(textNoOfAccount);
                      //mDeleteDialog.Dismiss();

                      if (accountList.Count() == 0 && additionalAccountList.Count() > 0)
                      {
                          NoAccountLayout.Visibility = ViewStates.Gone;
                      }
                      else if(accountList.Count() == 0 && additionalAccountList.Count() == 0)
                      {
                          txtboxcondition.Visibility = ViewStates.Gone;
                          txtTermsConditions.Visibility = ViewStates.Gone;
                          textNoOfAccount.Text = string.Format(Utility.GetLocalizedLabel("AddAccount", "noAccountsTitle"));
                          labelAccountLabel.Text = GetLabelByLanguage("noAcctFoundMsg");
                      }

                      mDeleteDialog.Dismiss();
                  })
                 .SetNegativeButton(GetLabelCommonByLanguage("cancel"), (senderAlert, args) =>
                 {
                     mDeleteDialog.Dismiss();
                 })
                  .SetCancelable(false)
                  .Create();

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    mDeleteDialog.SetMessage(Html.FromHtml(string.Format(GetLabelByLanguage("removeAcctMsg"), item.accountLabel, item.accountNumber), FromHtmlOptions.ModeLegacy));
                }
                else
                {
                    mDeleteDialog.SetMessage(Html.FromHtml(string.Format(GetLabelByLanguage("removeAcctMsg"), item.accountLabel, item.accountNumber)));
                }
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
                  .SetTitle(GetLabelByLanguage("removeAcct"))
                  .SetPositiveButton(GetLabelCommonByLanguage("ok"), (senderAlert, args) =>
                  {
                      additionalAccountList.Remove(item);
                      additionalAdapter = new AdditionalAccountListAdapter(this, additionalAccountList);
                      additionalAccountListRecyclerView.SetAdapter(additionalAdapter);
                      additionalAdapter.AdditionalItemClick += OnAdditionalItemClick;
                      additionalAdapter.NotifyDataSetChanged();
                      if (additionalAccountList.Count == 0)
                      {
                          textAdditionalAccount.Visibility = ViewStates.Gone;
                          textlabelAdditionalAcount.Visibility = ViewStates.Gone;
                          textNoOfAccount.Visibility = ViewStates.Visible;
                          labelAccountLabel.Visibility = ViewStates.Visible;
                          NoAccountLayout.Visibility = ViewStates.Visible;
                      }
                      int totalAccountAdded = adapter.GetAccountList().Count + additionalAdapter.GetAccountList().Count();
                      if (accountList != null && totalAccountAdded < Constants.ADD_ACCOUNT_LIMIT)
                      {
                          btnAddAnotherAccount.Visibility = ViewStates.Visible;
                      }
                      if (accountList.Count() == 0 && additionalAccountList.Count() == 0)
                      {
                          DisableConfirmButton();
                          checkboxlayout.Visibility = ViewStates.Gone;
                      }
                      mDeleteDialog.Dismiss();
                  })
                 .SetNegativeButton(GetLabelCommonByLanguage("cancel"), (senderAlert, args) =>
                 {
                     mDeleteDialog.Dismiss();
                 })
                  .SetCancelable(false)
                  .Create();

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    mDeleteDialog.SetMessage(Html.FromHtml(string.Format(GetLabelByLanguage("removeAcctMsg"), item.accountLabel, item.accountNumber), FromHtmlOptions.ModeLegacy));
                }
                else
                {
                    mDeleteDialog.SetMessage(Html.FromHtml(string.Format(GetLabelByLanguage("removeAcctMsg"), item.accountLabel, item.accountNumber)));
                }
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
            Intent nextIntent = new Intent(this, typeof(AddAccountActivity));
            nextIntent.PutExtra("fromRegisterPage", fromRegisterPage);
            RunOnUiThread(() => StartActivityForResult(nextIntent, ADD_ACCOUNT_REQUEST_CODE));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new LinkAccountPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                mGetAccountsProgressDialog = new AlertDialog.Builder(this)
                   .SetTitle("Loading..")
                   .SetMessage("Please wait while we are fetching account details")
                   .SetCancelable(false)
                   .Create();

                if (Intent.HasExtra("fromDashboard"))
                {
                    fromDashboard = Intent.Extras.GetBoolean("fromDashboard", false);
                }
                if (Intent.HasExtra("fromRegisterPage"))
                {
                    fromRegisterPage = Intent.Extras.GetBoolean("fromRegisterPage", false);
                }
               

                mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);

                TextViewUtils.SetMuseoSans500Typeface(textNoOfAccount, btnAddAnotherAccount, btnConfirm, textAdditionalAccount);
                TextViewUtils.SetMuseoSans300Typeface(labelAccountLabel, textlabelAdditionalAcount);

                textNoOfAccount.Text = GetLabelByLanguage("noAccountsTitle");
                labelAccountLabel.Text = GetLabelByLanguage("noAcctFoundMsg");
                btnAddAnotherAccount.Text = GetLabelByLanguage("addAnotherAcct");
                btnConfirm.Text = GetLabelCommonByLanguage("confirm");
                textAdditionalAccount.Text = GetLabelByLanguage("additionalAddAccts");
                textlabelAdditionalAcount.Text = GetLabelByLanguage("labeladditionalAccts");
                txtTermsConditions.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc"));
                StripUnderlinesFromLinks(txtTermsConditions);

                adapter = new AccountListAdapter(this, accountList);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                accountListRecyclerView.SetLayoutManager(layoutManager);
                accountListRecyclerView.SetAdapter(adapter);

                additionalAdapter = new AdditionalAccountListAdapter(this, additionalAccountList);
                layoutManager2 = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                additionalAccountListRecyclerView.SetLayoutManager(layoutManager2);
                additionalAccountListRecyclerView.SetAdapter(additionalAdapter);
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                var boxcondition = new CheckBox(this)
                {
                    ScaleX = 0.8f,
                    ScaleY = 0.8f
                };

                TextViewUtils.SetTextSize18(textAdditionalAccount);
                TextViewUtils.SetTextSize16(textNoOfAccount, labelAccountLabel, btnAddAnotherAccount, btnConfirm);
                TextViewUtils.SetTextSize14(textlabelAdditionalAcount);
                TextViewUtils.SetTextSize12(txtTermsConditions);


                txtboxcondition.CheckedChange += CheckedChange;
                txtboxcondition.Checked = false;
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
                            UserSessions.UpdateEmailflag(PreferenceManager.GetDefaultSharedPreferences(this));
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

                if (ConnectionUtils.HasInternetConnection(this))
                {
                    string ic_no = idNumber.Replace("-", string.Empty);
                    userActionsListener.GetAccountByIC(Constants.APP_CONFIG.API_KEY_ID, currentLinkedAccounts, email, ic_no);
                }
                else
                {
                    ShowNoInternetSnackbar();
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
                if (additionalAccountList.Count > 0)
                {
                    additionalAdapter.Clear();
                    additionalAdapter = new AdditionalAccountListAdapter(this, additionalAccountList);
                    additionalAccountListRecyclerView.SetAdapter(additionalAdapter);
                    additionalAdapter.AdditionalItemClick += OnAdditionalItemClick;
                    additionalAdapter.NotifyDataSetChanged();
                    textAdditionalAccount.Visibility = ViewStates.Visible;
                    checkboxlayout.Visibility = ViewStates.Visible;
                    textlabelAdditionalAcount.Visibility = ViewStates.Visible;
                }

                FirebaseAnalyticsUtils.SetScreenName(this, "Link Accounts Staging");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            this.mSharedPref.Edit().Remove("selectedcountry").Apply();
            base.OnPause();
        }

        public void ShowNoAccountAddedError(string message)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
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

        private void CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (!txtboxcondition.Checked)
            {
                checkedCondition = true;
                txtTermsConditions.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc"));
                StripUnderlinesFromLinks(txtTermsConditions);
            }
            else
            {
                checkedCondition = false;
                txtTermsConditions.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc_checked"));
                StripUnderlinesFromLinks(txtTermsConditions);
            }
            int totalAccountAdded = adapter.GetAccountList().Count() + additionalAdapter.GetAccountList().Count();
            string totalAcc = totalAccountAdded.ToString();
            this.userActionsListener.CheckRequiredFields(totalAcc, txtboxcondition.Checked);
            CheckingEmailAndPhoneNo();
        }

        [OnClick(Resource.Id.txtTermsConditions)]
        void OnTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.NavigateToTermsAndConditions();
            }
        }

        public void StripUnderlinesFromLinks(TextView textView)
        {
            var spannable = new SpannableStringBuilder(textView.TextFormatted);
            var spans = spannable.GetSpans(0, spannable.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
            foreach (URLSpan span in spans)
            {
                var start = spannable.GetSpanStart(span);
                var end = spannable.GetSpanEnd(span);
                spannable.RemoveSpan(span);
                var newSpan = new URLSpanNoUnderline(span.URL);
                spannable.SetSpan(newSpan, start, end, 0);
            }
            textView.TextFormatted = spannable;
        }

        class URLSpanNoUnderline : URLSpan
        {
            public URLSpanNoUnderline(string url) : base(url)
            {
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = false;
            }
        }

        public void ShowTermsAndConditions()
        {
            Intent disclaimer = new Intent(this, typeof(AddAccountDisclaimerActivity));
            StartActivityForResult(disclaimer, 1);
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
                        int total_count = response.Count;
                        string totalacc = total_count.ToString();
                        //textNoOfAcoount.Text = response.Count + " " + Utility.GetLocalizedLabel("Common", "titleNonOwnerAddAcc");
                        textNoOfAccount.Text = string.Format(Utility.GetLocalizedLabel("AddAccount", "OwnerDetectTitle"), totalacc);
                        labelAccountLabel.Text = GetLabelByLanguage("AcctFoundMsg");
                        labelAccountLabel.Visibility = ViewStates.Visible;
                        checkboxlayout.Visibility = ViewStates.Visible;
                        for (int i = 0; i < response.Count; i++)
                        {
                            BCRMAccount item = response[i];
                            if (item != null)
                            {
                                NewAccount accountDetails = new NewAccount()
                                {
                                    isOwner = item.isOwned,
                                    accountNumber = item.accNum,
                                    //accountLabel = EG_ACCOUNT_LABEL + ACCOUNT_COUNT,
                                    accountLabel = "",
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
                        textNoOfAccount.Text = GetLabelByLanguage("noAccountsTitle");
                        labelAccountLabel.Text = GetLabelByLanguage("noAcctFoundMsg");
                        checkboxlayout.Visibility = ViewStates.Gone;
                        ShowAddAnotherAccountScreen();
                    }
                }
                else
                {
                    textNoOfAccount.Text = GetLabelByLanguage("noAccountsTitle");
                    labelAccountLabel.Text = GetLabelByLanguage("noAcctFoundMsg");
                    checkboxlayout.Visibility = ViewStates.Gone;
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
                    string name = UserEntity.GetActive().DisplayName;
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
                        account.BudgetAmount = item.BudgetAmount == null ? "0" : item.BudgetAmount;
                        account.accountTypeId = item.accountTypeId == null ? "1" : item.accountTypeId;
                        account.IsHaveAccess = item.IsHaveAccess;
                        account.IsInManageAccessList = item.IsInManageAccessList;
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
                        account.emailOwner = item.emailOwner;
                        account.mobileNoOwner = item.mobileNoOwner;
                        account.BudgetAmount = item.BudgetAmount == null ? "0" : item.BudgetAmount;
                        account.accountTypeId = item.accountTypeId == null ? "1" : item.accountTypeId;
                        account.IsHaveAccess = item.IsHaveAccess;
                        account.IsInManageAccessList = item.IsInManageAccessList;
                        accounts.Add(account);
                    }
                    this.userActionsListener.AddMultipleAccounts(apiKeyID, userID, email, name, accounts);
                }
                else
                {
                    ShowErrorMessage(Utility.GetLocalizedErrorLabel("emptyNickname"));
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
                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                                        ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                                        : Resource.Style.TextInputLayoutBottomErrorHint);

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
                                        vh.textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                                            ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                                            : Resource.Style.TextInputLayoutBottomErrorHint);
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
                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                                        ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                                        : Resource.Style.TextInputLayoutBottomErrorHint);

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
                                        vh.textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                                            ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                                            : Resource.Style.TextInputLayoutBottomErrorHint);

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
                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                                        ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                                        : Resource.Style.TextInputLayoutBottomErrorHint);

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
                                        vh.textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                                            ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                                            : Resource.Style.TextInputLayoutBottomErrorHint);

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

                                    vh.textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                                        ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                                        : Resource.Style.TextInputLayoutBottomErrorHint);


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
                                        vh.textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                                            ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                                            : Resource.Style.TextInputLayoutBottomErrorHint);

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

            mErrorMessageSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            btn.SetTextColor(Android.Graphics.Color.Yellow);
            mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }

        public void ShowErrorEnterEmailOrNoPhone(string message)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
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

                            if (account.mobileNoOwner.Equals("") && account.emailOwner.Equals("") && account.type.Equals("1"))
                            {
                                account.isNoDetailOwner = true;
                            }
                            account.ISDmobileNo = "+06";
                            account.CountryCheck = false;
                            account.CountryCheckNoPhone = false;
                            account.countryDetail = "";

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
                                    textNoOfAccount.Visibility = ViewStates.Gone;
                                    labelAccountLabel.Visibility = ViewStates.Gone;
                                    txtboxcondition.Visibility = ViewStates.Visible;
                                    txtTermsConditions.Visibility = ViewStates.Visible;
                                }

                                additionalAdapter = new AdditionalAccountListAdapter(this, additionalAccountList);
                                additionalAccountListRecyclerView.SetAdapter(additionalAdapter);
                                additionalAdapter.AdditionalItemClick += OnAdditionalItemClick;
                                additionalAdapter.NotifyDataSetChanged();
                                textAdditionalAccount.Visibility = ViewStates.Visible;
                               // checkboxlayout.Visibility = ViewStates.Visible;
                                textlabelAdditionalAcount.Visibility = ViewStates.Visible;

                            }
                            else
                            {
                                ShowErrorMessage(Utility.GetLocalizedErrorLabel("error_duplicateAccountMessage"));
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

        public async void ShowAddAccountSuccess(List<Models.AddAccount> responseData)
        {

            try
            {
                int ctr = 0;

                List<NewAccount> finalAccountList = new List<NewAccount>();
                foreach (Models.AddAccount item in responseData)
                {
                    foreach (NewAccount newAccount in accountList)
                    {
                        if (newAccount.accountNumber.Equals(item.accountNumber))
                        {
                            newAccount.accountTypeId = item.accountTypeId == null ? "0" : item.accountTypeId;
                            newAccount.accountAddress = item.accountStAddress;
                            newAccount.ownerName = item.accountOwnerName;
                            newAccount.smartMeterCode = item.smartMeterCode == null ? "0" : item.smartMeterCode;
                            newAccount.isOwned = item.isOwned;
                            newAccount.IsError = item.IsError;
                            newAccount.IsTaggedSMR = item.IsTaggedSMR == "true" ? true : false;
                            newAccount.BudgetAmount = item.BudgetAmount == null ? "0" : item.BudgetAmount;
                            newAccount.InstallationType = item.InstallationType == null ? "0" : item.InstallationType;
                            newAccount.CreatedDate = item.CreatedDate;
                            newAccount.IsApplyEBilling = item.IsApplyEBilling;
                            newAccount.IsHaveAccess = item.IsHaveAccess;
                            newAccount.BusinessArea = item.BusinessArea;
                            newAccount.RateCategory = item.RateCategory;
                            newAccount.IsInManageAccessList = item.IsInManageAccessList;
                            finalAccountList.Add(newAccount);

                            //UserSessions.SaveAddress(mSharedPref, false);
                            ///<summary>
                            ///THIS TO SAVE HOUSE NO FOR MASKING ADDRESS
                            ///</summary>
                            //UserSessions.SaveAddress(finalAccountList);
                        }
                    }

                    foreach (NewAccount extraAccount in additionalAccountList)
                    {
                        if (extraAccount.accountNumber.Equals(item.accountNumber))
                        {
                            extraAccount.accountTypeId = item.accountTypeId == null ? "1" : item.accountTypeId;
                            extraAccount.accountAddress = item.accountStAddress;
                            extraAccount.ownerName = item.accountOwnerName;
                            extraAccount.smartMeterCode = item.smartMeterCode == null ? "0" : item.smartMeterCode;
                            extraAccount.isOwned = item.isOwned;
                            extraAccount.IsError = item.IsError;
                            extraAccount.IsTaggedSMR = item.IsTaggedSMR == "true" ? true : false;
                            extraAccount.BudgetAmount = item.BudgetAmount == null ? "0" : item.BudgetAmount;
                            extraAccount.InstallationType = item.InstallationType == null ? "0" : item.InstallationType;
                            extraAccount.CreatedDate = item.CreatedDate;
                            extraAccount.IsApplyEBilling = item.IsApplyEBilling;
                            extraAccount.IsHaveAccess = item.IsHaveAccess;
                            extraAccount.BusinessArea = item.BusinessArea;
                            extraAccount.RateCategory = item.RateCategory;
                            extraAccount.IsInManageAccessList = item.IsInManageAccessList;
                            finalAccountList.Add(extraAccount);
                        }
                    }
                }

                foreach (NewAccount newAccount in finalAccountList)
                {
                    CustomerBillingAccount.InsertOrReplace(newAccount, false);

                    ctr++;
                }

                CustomerBillingAccount.SetCAListForEligibility();

                SummaryDashBoardAccountEntity.RemoveAll();

                _ = await CustomEligibility.Instance.EvaluateEligibility((Context)this, true);

                List<string> newCAList = new List<string>();

                finalAccountList.ForEach(acct =>
                {
                    newCAList.Add(acct.accountNumber);
                });

                UserInfoV4 usrinf = new UserInfoV4();
                usrinf.ses_param1 = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().DisplayName : "";

                _ = Task.Run(async () => await FeatureInfoManager.Instance.SaveFeatureInfo(CustomEligibility.Instance.GetContractAccountList(newCAList),
                    FeatureInfoManager.QueueTopicEnum.addca, usrinf, new DeviceInfoRequest()));

                if (IsActive())
                {
                    HideAddingAccountProgressDialog();
                }

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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
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
                        .Cancelable(false)
                        .Build();

                    View view = mAddAccountProgressDialog.View;
                    if (view != null)
                    {
                        int totalAccountAdded = adapter.GetAccountList().Count() + additionalAdapter.GetAccountList().Count();
                        string totalAcc = totalAccountAdded.ToString();
                        string title = TOTAL_NO_OF_ACCOUNTS_TO_ADD == 1 ?
                            string.Format(Utility.GetLocalizedLabel("AddAccount", "addAccount"), totalAcc) :
                            string.Format(Utility.GetLocalizedLabel("AddAccount", "addAccounts"), totalAcc);
                        string message = TOTAL_NO_OF_ACCOUNTS_TO_ADD == 1 ?
                            Utility.GetLocalizedLabel("AddAccount", "addAccountMsg") :
                            Utility.GetLocalizedLabel("AddAccount", "addAccountsMsg");
                        TextView titleText = view.FindViewById<TextView>(Resource.Id.txtTitle);
                        TextView infoText = view.FindViewById<TextView>(Resource.Id.txtMessage);
                        titleText.Text = title;
                        infoText.Text = message;
                        TextViewUtils.SetTextSize16(titleText, infoText);
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
            string ic_no = idNumber.Replace("-", string.Empty);
            userActionsListener.GetAccountByIC(Constants.APP_CONFIG.API_KEY_ID, "", email, ic_no);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if (fromDashboard)
                ShowDashboard();
            else
                this.Finish();
        }

        public void EnableConfirmButton()
        {
            btnConfirm.Enabled = true;
            btnConfirm.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void DisableConfirmButton()
        {
            btnConfirm.Enabled = false;
            btnConfirm.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void ShowServiceError(string title, string message)
        {
            string dialogTitle = string.IsNullOrEmpty(title) ? Utility.GetLocalizedErrorLabel("defaultErrorTitle") : title;
            string dialogMessage = string.IsNullOrEmpty(message) ? Utility.GetLocalizedErrorLabel("defaultErrorMessage") : message;
            MaterialDialog serviceErrorDialog = new MaterialDialog.Builder(this)
                .Title(dialogTitle)
                .Content(dialogMessage)
                .PositiveText(Utility.GetLocalizedCommonLabel("ok"))
                .OnPositive((dialog, which) => NavigateToDashboard())
                .Cancelable(false)
                .Build();
            serviceErrorDialog.Show();
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

        public void CheckingEmailAndPhoneNo()
        {
            try
            {

                List<NewAccount> additionalList = additionalAdapter.GetAccountList();

                if (additionalList.Count > 0)
                {
                    string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                    string userID = UserEntity.GetActive().UserID;
                    string email = UserEntity.GetActive().Email;
                    List<Models.AddAccount> accounts = new List<Models.AddAccount>();
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
                        account.emailOwner = item.emailOwner;
                        account.mobileNoOwner = item.mobileNoOwner;
                        accounts.Add(account);
                    }
                    int i = 0;
                    TOTAL_NO_OF_ACCOUNTS_TO_ADD = accounts.Count;

                    if (txtboxcondition.Checked)
                    {
                       EnableConfirmButton();
                    }
                    else
                    {
                        DisableConfirmButton();
                    }
                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
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

        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
        }
    }
}