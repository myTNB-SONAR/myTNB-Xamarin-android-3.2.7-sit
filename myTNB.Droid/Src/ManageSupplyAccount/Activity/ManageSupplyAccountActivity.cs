using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using myTNB_Android.Src.ManageSupplyAccount.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.UpdateNickname.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace myTNB_Android.Src.ManageSupplyAccount.Activity
{
    [Activity(Label = "@string/manage_supply_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.ManageSupplyAccount")]
    public class ManageSupplyAccountActivity : BaseActivityCustom, ManageSupplyAccountContract.IView
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

        [BindView(Resource.Id.manageBillTitle)]
        TextView manageBillTitle;

        [BindView(Resource.Id.accountLayout)]
        LinearLayout accountLayout;

        [BindView(Resource.Id.layoutNickName)]
        LinearLayout layoutNickName;

        [BindView(Resource.Id.ManageBill_container)]
        LinearLayout ManageBill_container;

        GetBillRenderingResponse billrenderingresponse;

        ISharedPreferences mPref;

        ManageSupplyAccountContract.IUserActionsListener userActionsListener;
        ManageSupplyAccountPresenter mPresenter;
        bool _isOwner;
        MaterialDialog progress;

        const string PAGE_ID = "ManageAccount";

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

                        if (EligibilitySessionCache.Instance.IsAccountDBREligible && GetEligibleDBRAccount(accountData) == accountData.AccountNum)
                        {
                            ManageBill_container.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            ManageBill_container.Visibility = ViewStates.Gone;
                        }
                    }
                    position = extras.GetInt(Constants.SELECTED_ACCOUNT_POSITION);
                    _isOwner = EligibilitySessionCache.Instance.IsCADBREligible(accountData.AccountNum);
                }


                progress = new MaterialDialog.Builder(this)
                    .Title(Resource.String.manage_supply_account_remove_progress_title)
                    .Content(Resource.String.manage_supply_account_remove_progress_content)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNickName);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountAddress, txtNickName);
                TextViewUtils.SetMuseoSans500Typeface(txtAccountNumber, btnTextUpdateNickName, manageBillTitle);
                txtInputLayoutNickName.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
                TextViewUtils.SetMuseoSans500Typeface(btnRemoveAccount);
                TextViewUtils.SetTextSize14(txtAccountNumber, txtAccountAddress, btnTextUpdateNickName, manageBillTitle);
                TextViewUtils.SetTextSize16(btnRemoveAccount, txtNickName);
                txtAccountNumber.Text = accountData.AccountNum;
                txtAccountAddress.Text = accountData.AddStreet;

                txtNickName.Text = accountData.AccountNickName;

                txtInputLayoutNickName.Hint = GetLabelCommonByLanguage("acctNickname");
                btnTextUpdateNickName.Text = GetLabelCommonByLanguage("update");
                btnRemoveAccount.Text = GetLabelByLanguage("removeAccount");
                manageBillTitle.Text = Utility.GetLocalizedLabel("ManageAccount", "dbrManageDeliveryMethod");
                txtNickName.AddTextChangedListener(new InputFilterFormField(txtNickName, txtInputLayoutNickName));
                mPresenter = new ManageSupplyAccountPresenter(this, accountData);
                this.userActionsListener.Start();
                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                if (EligibilitySessionCache.Instance.IsAccountDBREligible && GetEligibleDBRAccount(accountData) == accountData.AccountNum)
                {
                    Handler h = new Handler();
                    Action myAction = () =>
                    {
                        NewAppTutorialUtils.ForceCloseNewAppTutorial();
                        if (!UserSessions.HasManageSupplyAccountTutorialShown(this.mPref))
                        {
                            OnShowManageSupplyAccountTutorialDialog();
                        }
                    };
                    h.PostDelayed(myAction, 50);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.ManageBill_container)]
        void OnManageBillDelivery(object sender, EventArgs eventArgs)
        {
            if (EligibilitySessionCache.Instance.IsAccountDBREligible && GetEligibleDBRAccount(accountData) == accountData.AccountNum)
                {
                    GetBillRenderingAsync(accountData);
                }
                else
                {
                    Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
                    intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                    intent.PutExtra("ParallelEmail", "ParallelEmail");
                    intent.PutExtra("_isOwner", JsonConvert.SerializeObject(_isOwner));
                StartActivity(intent);
                }
        }
        

        [OnClick(Resource.Id.btnTextUpdateNickName)]
        void OnClickUpdateNickname(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnUpdateNickname();
            }
        }

        [OnClick(Resource.Id.btnRemoveAccount)]
        void OnClickRemoveAccount(object sender, EventArgs eventArgs)
        {
            try
            {
                MyTNBAppToolTipBuilder removeAccountPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                    .SetTitle(GetLabelByLanguage("popupremoveAccountTitle"))
                    .SetMessage(string.Format(GetLabelByLanguage("popupremoveAccountMessage"), accountData.AccountNickName, accountData.AccountNum))
                    .SetCTALabel(GetLabelCommonByLanguage("cancel"))
                    .SetSecondaryCTALabel(GetLabelCommonByLanguage("ok"))
                    .SetSecondaryCTAaction(() => this.userActionsListener.OnRemoveAccount(accountData))
                    .Build();
                removeAccountPopup.Show();
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

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                if (EligibilitySessionCache.Instance.IsAccountDBREligible && GetEligibleDBRAccount(accountData) == accountData.AccountNum)
                {
                    Handler h = new Handler();
                    Action myAction = () =>
                    {
                        NewAppTutorialUtils.ForceCloseNewAppTutorial();
                        if (!UserSessions.HasManageSupplyAccountTutorialShown(this.mPref))
                        {
                            OnShowManageSupplyAccountTutorialDialog();
                        }
                    };
                    h.PostDelayed(myAction, 50);
                }
                FirebaseAnalyticsUtils.SetScreenName(this, "Manage Electricity Account");
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

                Snackbar updateSnackbar = Snackbar.Make(rootView, GetLabelByLanguage("nicknameUpdateSuccess"), Snackbar.LengthIndefinite)
                .SetAction(GetLabelCommonByLanguage("close"),
                 (view) =>
                 {
                     // EMPTY WILL CLOSE SNACKBAR
                 }
               );
                View v = updateSnackbar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                updateSnackbar.Show();
                SetResult(Result.Ok);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRemoveProgress()
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

        public void HideRemoveProgress()
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

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
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
            View snackbarView = mCancelledExceptionSnackBar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
            }
            );
            View snackbarView = mApiExcecptionSnackBar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View snackbarView = mUknownExceptionSnackBar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            mUknownExceptionSnackBar.Show();

        }

        public void ShowErrorMessageResponse(string error)
        {
            Snackbar errorMessageSnackbar =
            Snackbar.Make(rootView, error, Snackbar.LengthIndefinite)
                        .SetAction(Utility.GetLocalizedCommonLabel("close"),
                         (view) =>
                         {
                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        );//.Show();
            View snackbarView = errorMessageSnackbar.View;
            TextView textView = (TextView)snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            textView.SetMaxLines(4);
            errorMessageSnackbar.Show();
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
        public void OnShowManageSupplyAccountTutorialDialog()
        {
            Handler h = new Handler();
            Action myAction = () =>
            {
                NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.OnGeneraNewAppTutorialList(), true);
            };
            h.PostDelayed(myAction, 100);
        }
        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "manageCAManageBillDeliveryTitle"),
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "manageCAManageBillDeliveryMessage"),
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }
        private async void GetBillRenderingAsync(AccountData selectedAccount)
        {
            try
            {
                ShowProgressDialog();
                GetBillRenderingModel getBillRenderingModel = new GetBillRenderingModel();
                AccountData dbrAccount = selectedAccount;
                if (!AccessTokenCache.Instance.HasTokenSaved(this))
                {
                    string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                    AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
                }
                billrenderingresponse = await DBRManager.Instance.GetBillRendering(dbrAccount.AccountNum, AccessTokenCache.Instance.GetAccessToken(this));

                HideProgressDialog();
                //Nullity Check
                if (billrenderingresponse != null
                   && billrenderingresponse.StatusDetail != null
                   && billrenderingresponse.StatusDetail.IsSuccess)
                {
                    Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
                    intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                    intent.PutExtra("billrenderingresponse", JsonConvert.SerializeObject(billrenderingresponse.Content));
                    intent.PutExtra("_isOwner", JsonConvert.SerializeObject(_isOwner));
                    StartActivity(intent);
                }
                else
                {    
                    MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                        .SetTitle(billrenderingresponse.StatusDetail.Title)
                                        .SetMessage(billrenderingresponse.StatusDetail.Message)
                                        .SetCTALabel(billrenderingresponse.StatusDetail.PrimaryCTATitle)
                                        .Build();
                    errorPopup.Show();
                }

            }
            catch (System.Exception e)
            {
                HideProgressDialog();
                Utility.LoggingNonFatalError(e);
            }
        }
        public string GetEligibleDBRAccount(AccountData selectedAccount)
        {
            CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
            List<string> dBRCAs = EligibilitySessionCache.Instance.GetDBRCAs();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            CustomerBillingAccount account = new CustomerBillingAccount();
            string dbraccount = string.Empty;
            if (dBRCAs.Count > 0)
            {
                foreach (var dbrca in dBRCAs)
                {
                    dbraccount = dBRCAs.Where(x => x == selectedAccount.AccountNum).FirstOrDefault();
                    if (dbraccount != null)
                    {
                        return dbraccount;
                    }
                }
            }
            else
            {
                MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(Utility.GetLocalizedLabel("Error", "defaultErrorTitle"))
                                    .SetMessage(Utility.GetLocalizedLabel("Error", "defaultErrorMessage"))
                                    .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                     .Build();
                errorPopup.Show();
            }
            return dbraccount;
        }
        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (System.Exception e)
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
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public int GetAccountLayoutHeight()
        {
            int height = accountLayout.Height;
            return height;
        }
        public int GetLayoutNickNameHeight()
        {
            int height = layoutNickName.Height;
            return height;
        }
        public int GetLayoutManageBillHeight()
        {
            int height = ManageBill_container.Height;
            return height;
        }
        public int GetLayoutManageBillWidth()
        {
            int height = ManageBill_container.Width;
            return height;
        }

    }
}
