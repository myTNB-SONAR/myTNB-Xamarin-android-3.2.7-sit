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
using myTNB_Android.Src.SessionCache;
using myTNB_Android.Src.UpdateNickname.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
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

        [BindView(Resource.Id.manageBillTitle)]
        TextView manageBillTitle;

        [BindView(Resource.Id.accountLayout)]
        LinearLayout accountLayout;

        [BindView(Resource.Id.layoutNickName)]
        LinearLayout layoutNickName;

        [BindView(Resource.Id.ManageBill_container)]
        LinearLayout ManageBill_container;

        [BindView(Resource.Id.icon_myaccount_new)]
        LinearLayout icon_myaccount_new;

        [BindView(Resource.Id.txtNewLabel)]
        TextView txtNewLabel;

        ISharedPreferences mPref;

        ManageSupplyAccountContract.IUserActionsListener userActionsListener;
        ManageSupplyAccountPresenter mPresenter;
        MaterialDialog progress;

        const string PAGE_ID = "ManageAccount";

        private AccountData accountData;
        private int position;
        private Snackbar mCancelledExceptionSnackBar;
        private Snackbar mApiExcecptionSnackBar;
        private GetBillRenderingResponse _billRenderingResponse;
        private bool _isOwner;

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
                        accountData = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                    position = extras.GetInt(Constants.SELECTED_ACCOUNT_POSITION);
                    _isOwner = DBRUtility.Instance.IsDBROTTagFromCache
                        ? accountData.IsOwner
                        : DBRUtility.Instance.IsCADBREligible(accountData.AccountNum);
                }

                progress = new MaterialDialog.Builder(this)
                    .Title(Resource.String.manage_supply_account_remove_progress_title)
                    .Content(Resource.String.manage_supply_account_remove_progress_content)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNickName);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountAddress, txtNickName);
                TextViewUtils.SetMuseoSans500Typeface(txtAccountNumber, btnTextUpdateNickName, manageBillTitle, txtNewLabel);
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
                txtNewLabel.Text = Utility.GetLocalizedLabel("Common", "new");
                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                if (UserSessions.HasManageSupplyAccountTutorialShown(this.mPref))
                {
                    icon_myaccount_new.Visibility = ViewStates.Gone;
                }
                txtNickName.AddTextChangedListener(new InputFilterFormField(txtNickName, txtInputLayoutNickName));
                mPresenter = new ManageSupplyAccountPresenter(this, accountData);
                this.userActionsListener.Start();
                GetBillRenderingAsync(accountData);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.ManageBill_container)]
        void OnManageBillDelivery(object sender, EventArgs eventArgs)
        {
            DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.ManageElectricityAccount.Manage);
            Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
            intent.PutExtra("isOwner", _isOwner);
            intent.PutExtra("accountNumber", accountData.AccountNum);
            intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(_billRenderingResponse));
            StartActivity(intent);
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
                NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.OnGeneraNewAppTutorialList(), false);
            };
            h.PostDelayed(myAction, 100);
        }
        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            if (_isOwner)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Tutorial", "manageCAManageBillDeliveryTitle"),
                    ContentMessage = Utility.GetLocalizedLabel("Tutorial", "manageCAManageBillDeliveryMessage"),
                    ItemCount = 0,
                    DisplayMode = "",
                    IsButtonShow = false
                });
            }
            else
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Tutorial", "manageCAViewBillDeliveryTitle"),
                    ContentMessage = Utility.GetLocalizedLabel("Tutorial", "manageCAViewBillDeliveryMessage"),
                    ItemCount = 0,
                    DisplayMode = "",
                    IsButtonShow = false
                });
            }
            return newList;
        }

        private async void GetBillRenderingAsync(AccountData selectedAccount)
        {
            try
            {
                ShowProgressDialog();
                bool isEligible = DBRUtility.Instance.IsAccountDBREligible;
                if (!EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR
                            , EligibilitySessionCache.FeatureProperty.TargetGroup))
                {
                    isEligible = isEligible
                        && AccountTypeCache.Instance.IsAccountEligible(selectedAccount.AccountNum);
                    Console.WriteLine("[DEBUG] Profile IsDBREnabled 0: " + isEligible);
                    if (isEligible)
                    {
                        PostInstallationDetailsResponse installationDetailsResponse = await DBRManager.Instance.PostInstallationDetails(selectedAccount.AccountNum
                            , AccessTokenCache.Instance.GetAccessToken(this));
                        Console.WriteLine("[DEBUG] Profile RateCategory: " + installationDetailsResponse.RateCategory);
                        Console.WriteLine("[DEBUG] Profile IsResidential: " + installationDetailsResponse.IsResidential);
                        if (installationDetailsResponse != null
                            && installationDetailsResponse.StatusDetail != null
                            && installationDetailsResponse.StatusDetail.IsSuccess
                            && installationDetailsResponse.IsResidential)
                        {
                            isEligible = true;
                        }
                        else
                        {
                            isEligible = false;
                        }
                    }
                }
                if (isEligible)
                {
                    GetBillRenderingModel getBillRenderingModel = new GetBillRenderingModel();
                    AccountData dbrAccount = selectedAccount;

                    if (!AccessTokenCache.Instance.HasTokenSaved(this))
                    {
                        string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                        AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
                    }
                    _billRenderingResponse = await DBRManager.Instance.GetBillRendering(dbrAccount.AccountNum, AccessTokenCache.Instance.GetAccessToken(this));

                    //Nullity Check
                    if (_billRenderingResponse != null
                        && _billRenderingResponse.StatusDetail != null
                        && _billRenderingResponse.StatusDetail.IsSuccess
                        && _billRenderingResponse.Content != null
                        && _billRenderingResponse.Content.DBRType != MobileEnums.DBRTypeEnum.None)
                    {
                        manageBillTitle.Text = Utility.GetLocalizedLabel("ManageAccount", _isOwner
                            ? "dbrManageDeliveryMethod"
                            : "dbrViewBillDelivery");
                        ManageBill_container.Visibility = ViewStates.Visible;


                        Handler handler = new Handler();
                        Action myAction = () =>
                        {
                            NewAppTutorialUtils.ForceCloseNewAppTutorial();
                            if (!UserSessions.HasManageSupplyAccountTutorialShown(this.mPref))
                            {
                                OnShowManageSupplyAccountTutorialDialog();
                            }
                        };
                        handler.PostDelayed(myAction, 50);
                    }
                    else
                    {
                        ManageBill_container.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            HideProgressDialog();
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