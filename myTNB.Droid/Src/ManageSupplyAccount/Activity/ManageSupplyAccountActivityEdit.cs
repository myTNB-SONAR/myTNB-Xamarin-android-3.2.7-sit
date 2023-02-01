using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.ManageSupplyAccount.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.UpdateNickname.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;
using System.Collections.Generic;
using myTNB_Android.Src.UpdatePassword.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using Org.BouncyCastle.Crypto.Signers;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.ManageAccess.Activity;
using myTNB_Android.Src.Database.Model;
using Android.Preferences;
using myTNB.Mobile;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.SessionCache;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.ManageSupplyAccount.Activity
{
    [Activity(Label = "@string/manage_supply_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class ManageSupplyAccountActivityEdit : BaseActivityCustom, ManageSupplyAccountContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtAccountNumber)]
        TextView txtAccountNumber;

        [BindView(Resource.Id.txtAccountAddress)]
        TextView txtAccountAddress;

        [BindView(Resource.Id.txtInputLayoutNickName)]
        TextView txtInputLayoutNickName;

        [BindView(Resource.Id.txtNickName)]
        TextView txtNickName;

        [BindView(Resource.Id.btnTextUpdateNickName)]
        TextView btnTextUpdateNickName;

        [BindView(Resource.Id.profileMenuItemsContent)]
        LinearLayout profileMenuItemsContent;

        [BindView(Resource.Id.ManageBill_container)]
        LinearLayout ManageBill_container;

        [BindView(Resource.Id.manageBillTitle)]
        TextView manageBillTitle;

        [BindView(Resource.Id.icon_myaccount_new)]
        LinearLayout icon_myaccount_new;

        [BindView(Resource.Id.btnRemoveAccount)]
        Button btnRemoveAccount;

        [BindView(Resource.Id.txtNewLabel)]
        TextView txtNewLabel;

        [BindView(Resource.Id.infoAddress)]
        TextView infoAddress;

        [BindView(Resource.Id.view3)]
        View view3;

        [BindView(Resource.Id.accountLayout)]
        LinearLayout accountLayout;

        [BindView(Resource.Id.layoutNickName)]
        FrameLayout layoutNickName;


        private IMenu ManageSupplyAccountMenu;
        AccountData accountData;
        int position;

        private ManageSupplyItemContentComponent manageUser;
        ManageSupplyAccountContract.IUserActionsListener userActionsListener;
        ManageSupplyAccountPresenter mPresenter;

        MaterialDialog progress;

        const string PAGE_ID = "ManageAccount";
        private bool _isOwner;
        private GetBillRenderingResponse _billRenderingResponse;
        private GetBillRenderingTenantResponse billRenderingTenantResponse;

        ISharedPreferences mPref;

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

                        position = extras.GetInt(Constants.SELECTED_ACCOUNT_POSITION);
                        _isOwner = accountData.IsOwner && DBRUtility.Instance.IsCAEligible(accountData.AccountNum);
                    }
                    //position = extras.GetInt(Constants.SELECTED_ACCOUNT_POSITION);
                }


                progress = new MaterialDialog.Builder(this)
                    .Title(Resource.String.manage_supply_account_remove_progress_title)
                    .Content(Resource.String.manage_supply_account_remove_progress_content)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();


                ManageSupplyItemComponent manageSupplyItem = GetManageSupply();
                profileMenuItemsContent.AddView(manageSupplyItem);




                //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNickName);
                //TextViewUtils.SetMuseoSans300Typeface(txtAccountAddress, txtNickName);
                //TextViewUtils.SetMuseoSans500Typeface(txtAccountNumber, btnTextUpdateNickName, manageBillTitle);
                //TextViewUtils.SetMuseoSans500Typeface(btnRemoveAccount);

                //TextViewUtils.SetTextSize14(txtAccountAddress, txtNickName, txtAccountNumber, btnTextUpdateNickName, btnRemoveAccount, txtInputLayoutNickName);

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNickName);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountAddress, txtNickName, infoAddress);
                TextViewUtils.SetMuseoSans500Typeface(txtAccountNumber, btnTextUpdateNickName, manageBillTitle, txtNewLabel);
                //txtInputLayoutNickName.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                //    ? Resource.Style.TextInputLayout_TextAppearance_Large
                //    : Resource.Style.TextInputLayout_TextAppearance_Small);
                TextViewUtils.SetMuseoSans500Typeface(btnRemoveAccount, infoAddress);
                TextViewUtils.SetTextSize14(txtAccountNumber, txtAccountAddress, btnTextUpdateNickName, manageBillTitle, infoAddress);
                TextViewUtils.SetTextSize16(btnRemoveAccount, txtNickName);

                txtAccountNumber.Text = accountData.AccountNum;
                //txtAccountAddress.Text = Utility.StringMasking(Utility.Masking.Address, accountData.AddStreet);

                bool isWhiteList = UserSessions.GetWhiteList(PreferenceManager.GetDefaultSharedPreferences(this));
                //if not owner mask the address IRUL
                if (!accountData.IsOwner == true)
                {
                    if (isWhiteList)
                    {
                        txtAccountAddress.Text = accountData.AddStreet;
                        infoAddress.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        if (!accountData.IsHaveAccess == true)
                        {
                            //txtAccountAddress.Text = Utility.StringSpaceMasking(Utility.Masking.Address, accountData.AddStreet);
                            txtAccountAddress.Text = accountData.AddStreet;
                            infoAddress.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            txtAccountAddress.Text = accountData.AddStreet;
                            infoAddress.Visibility = ViewStates.Gone;
                        }
                    }

                }
                else
                {

                    txtAccountAddress.Text = accountData.AddStreet;
                    infoAddress.Visibility = ViewStates.Gone;
                }

                txtNickName.Text = accountData.AccountNickName;

                txtInputLayoutNickName.Hint = GetLabelCommonByLanguage("acctNickname").ToUpper();
                btnTextUpdateNickName.Text = GetLabelCommonByLanguage("update");
                btnRemoveAccount.Text = Utility.GetLocalizedLabel("ManageAccount", "removeAccount");
                infoAddress.Text = Utility.GetLocalizedLabel("ManageAccount", "dialogAddrress");
                txtNewLabel.Text = Utility.GetLocalizedLabel("Common", "new");

                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                if (UserSessions.HasManageSupplyAccountTutorialShown(this.mPref))
                {
                    icon_myaccount_new.Visibility = ViewStates.Gone;
                }

                //txtNickName.AddTextChangedListener(new InputFilterFormField(txtNickName, txtInputLayoutNickName));
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                // this.SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                //SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);

                mPresenter = new ManageSupplyAccountPresenter(this, accountData, PreferenceManager.GetDefaultSharedPreferences(this));
                this.userActionsListener.Start();
                GetBillRenderingAsync(accountData);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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

        [OnClick(Resource.Id.ManageBill_container)]
        void OnManageBillDelivery(object sender, EventArgs eventArgs)
        {
            DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.ManageElectricityAccount.Manage);
            Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
            intent.PutExtra("isOwner", _isOwner);
            intent.PutExtra("accountNumber", accountData.AccountNum);
            intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(_billRenderingResponse));
            intent.PutExtra("billRenderingTenantResponse", JsonConvert.SerializeObject(billRenderingTenantResponse));
            StartActivity(intent);
        }

        [OnClick(Resource.Id.btnRemoveAccount)]
        void OnClickRemoveAccount(object sender, EventArgs eventArgs)
        {
            try
            {

                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    ShowRemoveAccountDialog(this, () =>
                    {
                        this.userActionsListener.OnRemoveAccount(accountData);
                    });
                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }



        //AlertDialog removeDialog;

        void ShowRemoveAccountDialog(Android.App.Activity context, Action confirmAction, Action cancelAction = null)
        {

            //CustomerBillingAccount account = adapter.GetItemObject(position);
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("ManageAccount", "popupremoveAccountTitle"))
                        //.SetMessage(Utility.GetLocalizedLabel("Common", "updateIdMessage"))
                        .SetMessage(string.Format(Utility.GetLocalizedLabel("ManageAccount", "popupremoveAccountMessage"), accountData.AccountNickName, accountData.AccountNum))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "cancel"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "ok"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();

            //try
            //{
            //    if (removeDialog != null && removeDialog.IsShowing)
            //    {
            //        removeDialog.Dismiss();
            //    }

            //    removeDialog = new AlertDialog.Builder(this)

            //        .SetTitle(GetLabelByLanguage("popupremoveAccountTitle"))
            //        .SetMessage(GetFormattedText(string.Format(GetLabelByLanguage("popupremoveAccountMessage"), accountData.AccountNickName, accountData.AccountNum)))
            //        .SetNegativeButton(GetLabelCommonByLanguage("cancel"),
            //        delegate
            //        {
            //            removeDialog.Dismiss();
            //        })
            //        .SetPositiveButton(GetLabelCommonByLanguage("ok"),
            //        delegate
            //        {
            //            this.userActionsListener.OnRemoveAccount(accountData);
            //        })
            //        .Show()
            //        ;
            //}
            //catch (Exception e)
            //{
            //    Utility.LoggingNonFatalError(e);
            //}
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ManageSupplyAccountsViewEdit;
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

        private ManageSupplyItemComponent GetManageSupply()
        {
            //Context context = Activity.ApplicationContext;

            ManageSupplyItemComponent manageItem = new ManageSupplyItemComponent(this);

            List<View> manageItems = new List<View>();
            bool isWhiteList = UserSessions.GetWhiteList(PreferenceManager.GetDefaultSharedPreferences(this));

            //if (true)
            if ((accountData.IsOwner || isWhiteList) && accountData.AccountTypeId == "1")
            //if (accountData.IsOwner && accountData.AccountTypeId == "1")
            {
                SupplyAccMenuItemSingleContentComponent manageUser = new SupplyAccMenuItemSingleContentComponent(this);
                manageUser.SetTitle(Utility.GetLocalizedLabel("ManageAccount", "manageUserAccess"));
                manageUser.SetIcon(1);
                manageUser.SetItemActionVisibility(true);
                manageUser.SetItemActionCall(ShowManageUser);
                manageItems.Add(manageUser);

                view3.Visibility = ViewStates.Gone;
            }

            //SupplyAccMenuItemSingleContentComponent autoPay = new SupplyAccMenuItemSingleContentComponent(this);
            //autoPay.SetTitle(Utility.GetLocalizedLabel("ManageAccount", "manageAutopay"));
            //autoPay.SetIcon(2);
            //autoPay.SetItemActionVisibility(true);
            //autoPay.SetItemActionCall(ShowManageAutopay);
            //manageItems.Add(autoPay);

            manageItem.AddComponentView(manageItems);
            return manageItem;
        }

        private async void GetBillRenderingAsync(AccountData selectedAccount)
        {
            try
            {
                ShowProgressDialog();
                if (DBRUtility.Instance.IsAccountEligible && DBRUtility.Instance.IsCAEligible(selectedAccount.AccountNum))
                {
                    GetBillRenderingModel getBillRenderingModel = new GetBillRenderingModel();
                    AccountData dbrAccount = selectedAccount;

                    if (!AccessTokenCache.Instance.HasTokenSaved(this))
                    {
                        string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                        AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
                    }
                    _billRenderingResponse = await DBRManager.Instance.GetBillRendering(dbrAccount.AccountNum, AccessTokenCache.Instance.GetAccessToken(this), dbrAccount.IsOwner);

                    List<string> dBRCAs = DBRUtility.Instance.GetCAList();
                    bool tenantAllowOptIn = false;
                    billRenderingTenantResponse = await DBRManager.Instance.GetBillRenderingTenant(dBRCAs, UserEntity.GetActive().UserID, AccessTokenCache.Instance.GetAccessToken(this));

                    //Nullity Check
                    if (_billRenderingResponse != null
                        && _billRenderingResponse.StatusDetail != null
                        && _billRenderingResponse.StatusDetail.IsSuccess
                        && _billRenderingResponse.Content != null
                        && _billRenderingResponse.Content.DBRType != MobileEnums.DBRTypeEnum.None)
                    {
                        if (billRenderingTenantResponse != null
                           && billRenderingTenantResponse.StatusDetail != null
                           && billRenderingTenantResponse.StatusDetail.IsSuccess
                           && billRenderingTenantResponse.Content != null)
                        {
                            bool isOwnerOverRule = billRenderingTenantResponse.Content.Find(x => x.CaNo == selectedAccount.AccountNum).IsOwnerOverRule;
                            bool isOwnerAlreadyOptIn = billRenderingTenantResponse.Content.Find(x => x.CaNo == selectedAccount.AccountNum).IsOwnerAlreadyOptIn;
                            bool isTenantAlreadyOptIn = billRenderingTenantResponse.Content.Find(x => x.CaNo == selectedAccount.AccountNum).IsTenantAlreadyOptIn;

                            if (selectedAccount.AccountHasOwner && !isOwnerOverRule && !isOwnerAlreadyOptIn && !isTenantAlreadyOptIn)
                            {
                                tenantAllowOptIn = true;
                            }
                        }
                        manageBillTitle.Text = Utility.GetLocalizedLabel("ManageAccount", _isOwner || tenantAllowOptIn
                            ? "dbrManageDeliveryMethod"
                            : "dbrViewBillDelivery");

                        ManageBill_container.Visibility = ViewStates.Visible;
                        //if (_isOwner || tenantAllowOptIn == true)
                        //{
                        //    ManageBill_container.Visibility = ViewStates.Visible;
                        //}

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

        private void ShowManageUser()
        {
            try
            {
                this.userActionsListener.ManageAccessUser(accountData);
                //ManageUserActivity();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ManageUserActivity()
        {
            Intent ManageUser = new Intent(this, typeof(ManageAccessActivity));
            ManageUser.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            StartActivityForResult(ManageUser, Constants.UPDATE_NICKNAME_REQUEST);
        }

        private void ShowManageAutopay()
        {
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
                TextViewUtils.SetTextSize14(tv);
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

        MaterialDialog addressInfoDialog;
        //AlertDialog addressInfo;
        [OnClick(Resource.Id.infoAddress)]

        void OnClickAddressInfo(object sender, EventArgs eventArgs)
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                       .SetTitle((string.Format(GetLabelByLanguage("dialogAddrress"))))
                       .SetMessage(string.Format(GetLabelByLanguage("dialogAddrressMessage")))
                       .SetContentGravity(GravityFlags.Left)
                       .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                       .Build().Show();

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
            TextViewUtils.SetTextSize14(textView);
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
            TextViewUtils.SetTextSize14(textView);
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
            TextViewUtils.SetTextSize14(textView);
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
            TextViewUtils.SetTextSize14(textView);
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
            if (accountData.IsOwner && accountData.AccountTypeId == "1")
            {
                int height = accountLayout.Height + (int)DPUtils.ConvertDPToPx(65f);
                return height;
            }
            else
            {
                int height = accountLayout.Height;
                return height;
            }
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
