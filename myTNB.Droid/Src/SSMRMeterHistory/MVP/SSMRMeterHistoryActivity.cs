using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;




using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.SSMRMeterHistory.Adapter;
using myTNB_Android.Src.SSMRTerminate.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
    [Activity(Label = "@string/ssmr_meter_history_activity_title"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.SSMRMeterHistoryStyle")]
    public class SSMRMeterHistoryActivity : BaseActivityCustom, SSMRMeterHistoryContract.IView
    {
        private string SMR_ACTION_KEY;
        private SMRActivityInfoResponse smrResponse;
        private AccountData selectedAccount;
        private SMRAccount selectedEligibleAccount;
        private string selectedAccountNickName;
        private bool IsFromUsage = false;
        private List<SSMRMeterHistoryMenuModel> ssmrMeterHistoryMenuList = new List<SSMRMeterHistoryMenuModel>();
        public readonly static int SSMR_METER_HISTORY_ACTIVITY_CODE = 8796;
        public readonly static int SSMR_SUBMIT_METER_ACTIVITY_CODE = 8797;
        public readonly static int SSMR_SELECT_ACCOUNT_ACTIVITY_CODE = 8798;
        private SSMRMeterHistoryContract.IPresenter mPresenter;
        private string selectedAccountNumber;
        private List<SMRAccount> smrAccountList = new List<SMRAccount>();
        const string PAGE_ID = "SSMRReadingHistory";
        ISharedPreferences mPref;
        private bool isTutorialShown = false;
        private bool isSMR = false;
        private bool noMeterAccess = false;
        private bool IsTenant = false;

        SSMRMeterHistoryMenuAdapter meterHistoryMenuAdapter;

        [BindView(Resource.Id.smr_submitted_img)]
        ImageView SMRMainImg;

        [BindView(Resource.Id.smr_submitted_title)]
        TextView SMRMainTitle;

        [BindView(Resource.Id.smr_submitted_content)]
        TextView SMRMainContent;

        [BindView(Resource.Id.smr_content_history_header)]
        TextView SMRListHeader;

        [BindView(Resource.Id.btnSubmitMeter)]
        Button btnSubmitMeter;

        [BindView(Resource.Id.btnEnableSubmitMeter)]
        Button btnEnableSubmitMeter;

        [BindView(Resource.Id.btnDisableSubmitMeter)]
        Button btnDisableSubmitMeter;

        [BindView(Resource.Id.smr_history_recyclerview)]
        RecyclerView mSMRRecyclerView;

        [BindView(Resource.Id.empty_smr_history_container)]
        LinearLayout EmptySMRHistoryContainer;

        [BindView(Resource.Id.empty_smr_history_message)]
        TextView EmptySMRHistoryMessage;

        [BindView(Resource.Id.smr_message_title)]
        TextView SMRMessageTitle;

        [BindView(Resource.Id.txtSelectedAccountTitle)]
        TextView SMRAccountTitle;

        [BindView(Resource.Id.selector_smr_account)]
        TextView SMRAccountSelected;

        [BindView(Resource.Id.smrActionContainer)]
        LinearLayout SMRActionContainer;

        [BindView(Resource.Id.nonSMRActionContainer)]
        LinearLayout NonSMRActionContainer;

        [BindView(Resource.Id.bottomLayout)]
        LinearLayout bottomLayout;

        [BindView(Resource.Id.txtMeterAccessTitle)]
        TextView txtMeterAccessTitle;

        [BindView(Resource.Id.btnNo)]
        Button btnNo;

        [BindView(Resource.Id.btnYes)]
        Button btnYes;

        [BindView(Resource.Id.meterLookLabelContainer)]
        LinearLayout meterLookLabelContainer;

        [BindView(Resource.Id.meterLookLabel)]
        TextView meterLookLabel;

        [BindView(Resource.Id.readingHistoryList)]
        LinearLayout ReadingHistoryListContainer;

        [BindView(Resource.Id.disableSMRBtnContainer)]
        LinearLayout DisableSMRBtnContainer;

        [BindView(Resource.Id.layout_content_nestedscroll)]
        NestedScrollView NestedScrollViewContent;

        [BindView(Resource.Id.smrReadingHistoryDetailContent)]
        LinearLayout smrReadingHistoryDetailContent;

        [BindView(Resource.Id.accountListRefreshContainer)]
        LinearLayout smrAccountListRefreshContainer;

        [BindView(Resource.Id.refreshMsg)]
        TextView refreshMsg;

        [BindView(Resource.Id.btnRefresh)]
        Button btnRefresh;

        [BindView(Resource.Id.selectAccountContainer)]
        LinearLayout selectAccountContainer;


        public override int ResourceId()
        {
            return Resource.Layout.SSMRMeterHistoryLayout;
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
                isSMR = false;
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);

                TextViewUtils.SetMuseoSans500Typeface(SMRMainTitle, SMRListHeader, SMRMessageTitle, btnSubmitMeter
                    , btnEnableSubmitMeter, btnDisableSubmitMeter, btnRefresh);
                TextViewUtils.SetMuseoSans300Typeface(SMRMainContent, SMRAccountTitle, SMRAccountSelected, EmptySMRHistoryMessage, refreshMsg);
                TextViewUtils.SetMuseoSans500Typeface(txtMeterAccessTitle, btnNo, btnYes, meterLookLabel);
                TextViewUtils.SetTextSize9(SMRAccountTitle);
                TextViewUtils.SetTextSize12(meterLookLabel);
                TextViewUtils.SetTextSize14(SMRMainTitle, SMRMainContent, EmptySMRHistoryMessage);
                TextViewUtils.SetTextSize16(SMRMessageTitle, SMRAccountSelected, txtMeterAccessTitle, SMRListHeader
                    , refreshMsg, btnEnableSubmitMeter, btnRefresh, btnNo, btnYes, btnSubmitMeter, btnDisableSubmitMeter);

                SMRMessageTitle.Text = GetLabelByLanguage("subTitle");
                SMRAccountTitle.Text = GetLabelCommonByLanguage("account").ToUpper();
                SMRAccountSelected.Text = GetLabelCommonByLanguage("selectAccount");
                SMRListHeader.Text = GetLabelByLanguage("headerTitle");
                btnEnableSubmitMeter.Text = GetLabelByLanguage("enableSSMRCTA");
                btnDisableSubmitMeter.Text = GetLabelByLanguage("disableSSMRCTA");
                EmptySMRHistoryMessage.Text = GetLabelByLanguage("noHistoryData");
                refreshMsg.Text = GetLabelCommonByLanguage("refreshDescription");
                btnRefresh.Text = GetLabelCommonByLanguage("refreshNow");
                txtMeterAccessTitle.Text = GetLabelByLanguage("doYouHaveMeterAccess");
                btnNo.Text = GetLabelCommonByLanguage("no");
                btnYes.Text = GetLabelCommonByLanguage("yes");
                meterLookLabel.Text = GetLabelByLanguage("whereIsMyMeter");
                btnSubmitMeter.Text = Utility.GetLocalizedLabel("SSMRSubmitMeterReading", "title");
                mPresenter = new SSMRMeterHistoryPresenter(this);
                mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                mSMRRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
                smrAccountList = this.mPresenter.GetEligibleSMRAccountList();

                if (smrAccountList != null && smrAccountList.Count > 0)
                {
                    foreach (SMRAccount account in smrAccountList)
                    {
                        account.accountSelected = false;
                    }

                    Bundle extras = Intent.Extras;
                    //If has selected account - means coming from inner dashboard
                    if (extras != null)
                    {
                        if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                        {
                            selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                            selectedAccountNumber = selectedAccount.AccountNum;
                            selectedAccountNickName = selectedAccount.AccountNickName;
                            CustomerBillingAccount.RemoveSelected();
                            CustomerBillingAccount.SetSelected(selectedAccount.AccountNum);
                            SMRAccount smrSelectedAccount = smrAccountList.Find(account => account.accountNumber == selectedAccountNumber);
                            if (extras.ContainsKey(Constants.SMR_RESPONSE_KEY))
                            {
                                IsTenant = smrSelectedAccount.IsTenant;
                                smrResponse = JsonConvert.DeserializeObject<SMRActivityInfoResponse>(extras.GetString(Constants.SMR_RESPONSE_KEY));
                                UpdateUIForSMR(smrResponse);
                                this.mPresenter.CheckIsBtnSubmitHide(smrResponse);
                                isSMR = true;
                                isTutorialShown = true;
                            }

                            if (extras.ContainsKey("fromNotificationDetails"))
                            {
                                IsTenant = smrSelectedAccount.IsTenant;
                                UpdateUIForNonSMR();
                                isSMR = false;
                                isTutorialShown = true;
                            }
                        }
                        else
                        {
                            SMRAccount smrSelectedAccount = smrAccountList.Find(account =>
                            {
                                return account.isTaggedSMR;
                            });

                            if (smrSelectedAccount == null)
                            {
                                smrSelectedAccount = smrAccountList[0];
                            }

                            selectedAccountNumber = smrSelectedAccount.accountNumber;
                            CustomerBillingAccount.RemoveSelected();
                            CustomerBillingAccount.SetSelected(smrSelectedAccount.accountNumber);
                            if (smrSelectedAccount != null)
                            {
                                selectedAccountNickName = smrSelectedAccount.accountName;
                                IsTenant = smrSelectedAccount.IsTenant;
                                if (smrSelectedAccount.isTaggedSMR)
                                {
                                    isSMR = true;
                                    this.mPresenter.GetSSMRAccountStatus(smrSelectedAccount.accountNumber);
                                }
                                else
                                {
                                    UpdateUIForNonSMR();
                                    isSMR = false;
                                    isTutorialShown = true;
                                }
                            }
                            else
                            {
                                ShowNonSMRVisible(true, false);
                            }
                        }

                        if (extras.ContainsKey("fromUsage"))
                        {
                            IsFromUsage = extras.GetBoolean("fromUsage");
                        }
                        else
                        {
                            IsFromUsage = false;
                        }
                    }
                    //Else from HomeScreen
                    else
                    {
                        IsFromUsage = false;
                        SMRAccount smrSelectedAccount = smrAccountList.Find(account =>
                        {
                            return account.isTaggedSMR;
                        });

                        if (smrSelectedAccount == null)
                        {
                            smrSelectedAccount = smrAccountList[0];
                        }

                        selectedAccountNumber = smrSelectedAccount.accountNumber;
                        CustomerBillingAccount.RemoveSelected();
                        CustomerBillingAccount.SetSelected(smrSelectedAccount.accountNumber);
                        if (smrSelectedAccount != null)
                        {
                            selectedAccountNickName = smrSelectedAccount.accountName;
                            IsTenant = smrSelectedAccount.IsTenant;
                            if (smrSelectedAccount.isTaggedSMR)
                            {
                                isSMR = true;
                                this.mPresenter.GetSSMRAccountStatus(smrSelectedAccount.accountNumber);
                            }
                            else
                            {
                                UpdateUIForNonSMR();
                                isSMR = false;
                                isTutorialShown = true;
                            }
                        }
                        else
                        {
                            ShowNonSMRVisible(true, false);
                        }
                    }
                }
                else
                {
                    IsFromUsage = false;
                    ShowNonSMRVisible(true, false);
                }

                if (DownTimeEntity.IsBCRMDown())
                {
                    // this.SetIsClicked(false);
                    //btnDisableSubmitMeter.Enabled = false;
                    btnDisableSubmitMeter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                    btnDisableSubmitMeter.SetTextColor(Android.Graphics.Color.White);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowNonSMRVisible(bool isNonSMRAccount, bool hasNoSMREligibleAccount)
        {
            //Non-SMR visibility
            NonSMRActionContainer.Visibility = isNonSMRAccount ? ViewStates.Visible : ViewStates.Gone;
            bottomLayout.Visibility = isNonSMRAccount ? ViewStates.Visible : ViewStates.Gone;
            //SMR UI visiblility
            SMRActionContainer.Visibility = isNonSMRAccount ? ViewStates.Gone : ViewStates.Visible;
            ReadingHistoryListContainer.Visibility = isNonSMRAccount ? ViewStates.Gone : ViewStates.Visible;
            DisableSMRBtnContainer.Visibility = isNonSMRAccount ? ViewStates.Gone : ViewStates.Visible;

            //Checking for no tagged SMR
            NonSMRActionContainer.Visibility = hasNoSMREligibleAccount ? ViewStates.Visible : ViewStates.Gone;
            bottomLayout.Visibility = hasNoSMREligibleAccount ? ViewStates.Visible : ViewStates.Gone;

            if (MyTNBAccountManagement.GetInstance().IsSMROpenToTenant() && IsTenant)
            {
                DisableSMRBtnContainer.Visibility = ViewStates.Gone;
                NonSMRActionContainer.Visibility = ViewStates.Gone;
            }

            btnNo.Enabled = true;
            btnNo.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
            btnNo.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);

            btnYes.Enabled = true;
            btnYes.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
            btnYes.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);

            btnEnableSubmitMeter.Enabled = false;
            btnEnableSubmitMeter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);

            noMeterAccess = false;
        }

        [OnClick(Resource.Id.btnNo)]
        void OnSelectNo(object sender, EventArgs eventArgs)
        {
            btnNo.Enabled = true;
            btnNo.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.white));
            btnNo.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);

            btnYes.Enabled = true;
            btnYes.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
            btnYes.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);

            if (DownTimeEntity.IsBCRMDown())
            {
                // this.SetIsClicked(false);
                btnEnableSubmitMeter.Enabled = true;
                btnEnableSubmitMeter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            }
            else
            {
                btnEnableSubmitMeter.Enabled = true;
                btnEnableSubmitMeter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
           

            noMeterAccess = true;
        }

        [OnClick(Resource.Id.btnYes)]
        void OnSelectYes(object sender, EventArgs eventArgs)
        {
            btnNo.Enabled = true;
            btnNo.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
            btnNo.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);

            btnYes.Enabled = true;
            btnYes.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.white));
            btnYes.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);

            if (DownTimeEntity.IsBCRMDown())
            {
                // this.SetIsClicked(false);
                btnEnableSubmitMeter.Enabled = true;
                btnEnableSubmitMeter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            }
            else
            {
                btnEnableSubmitMeter.Enabled = true;
                btnEnableSubmitMeter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
           
            noMeterAccess = false;
        }

        [OnClick(Resource.Id.meterLookLabelContainer)]
        void OnMeterLookPopupClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                    .SetHeaderImage(Resource.Drawable.img_start_smr)
                    .SetTitle(GetLabelByLanguage("whereIsMyMeterTitle"))
                    .SetMessage(GetLabelByLanguage("whereIsMyMeterMessage"))
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                    .SetCTAaction(() => { this.SetIsClicked(false); })
                    .Build().Show();
            }
        }

        private void UpdateUIForNonSMR()
        {
            ShowRefreshScreen(false);
            SMRAccountSelected.Text = selectedAccountNickName;
            ShowNonSMRVisible(true, true);

        }

        protected override void OnStart()
        {
            base.OnStart();
            NestedScrollViewContent.Parent.RequestChildFocus(NestedScrollViewContent, NestedScrollViewContent);
        }

        public void UpdateUIForSMR(SMRActivityInfoResponse activityInfoResponse)
        {
            ShowRefreshScreen(false);
            SMRAccountSelected.Text = selectedAccountNickName;
            ShowNonSMRVisible(false, false);
            smrResponse = activityInfoResponse;

            if (activityInfoResponse.Response.Data.MeterReadingHistory.Count > 0)
            {
                SSMRMeterHistoryAdapter adapter = new SSMRMeterHistoryAdapter(activityInfoResponse.Response.Data.MeterReadingHistory);
                mSMRRecyclerView.SetAdapter(adapter);
                mSMRRecyclerView.Visibility = ViewStates.Visible;
                EmptySMRHistoryContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                EmptySMRHistoryContainer.Visibility = ViewStates.Visible;
                mSMRRecyclerView.Visibility = ViewStates.Gone;
            }

            if (activityInfoResponse.Response.Data.DashboardCTAType == Constants.SMR_SUBMIT_METER_KEY && activityInfoResponse.Response.Data.isCurrentPeriodSubmitted == "false"
                && activityInfoResponse.Response.Data.isDashboardCTADisabled == "false")
            {
                btnSubmitMeter.Visibility = ViewStates.Visible;
                if (DownTimeEntity.IsBCRMDown())
                {
                    // this.SetIsClicked(false);
                    //btnSubmitMeter.Enabled = false;
                    btnSubmitMeter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                }
            }
            else
            {
                btnSubmitMeter.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrEmpty(activityInfoResponse.Response.Data.HistoryViewTitle))
            {
                SMRMainTitle.TextFormatted = GetFormattedText(activityInfoResponse.Response.Data.HistoryViewTitle);
                SMRMainTitle.Visibility = ViewStates.Visible;
            }
            else
            {
                SMRMainTitle.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrEmpty(activityInfoResponse.Response.Data.HistoryViewMessage))
            {
                SMRMainContent.TextFormatted = GetFormattedText(activityInfoResponse.Response.Data.HistoryViewMessage);
                SMRMainContent.Visibility = ViewStates.Visible;
            }
            else
            {
                SMRMainContent.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrEmpty(activityInfoResponse.Response.Data.DashboardCTAText))
            {
                btnSubmitMeter.Text = activityInfoResponse.Response.Data.DashboardCTAText;
            }
        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == SSMR_METER_HISTORY_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    isTutorialShown = false;
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    SetResult(Result.Ok);
                    Finish();
                }
            }
            if (requestCode == SSMR_SELECT_ACCOUNT_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    selectedAccountNumber = data.GetStringExtra("SELECTED_ACCOUNT_NUMBER");
                    foreach (SMRAccount account in smrAccountList)
                    {
                        if (account.accountNumber == selectedAccountNumber)
                        {
                            selectedEligibleAccount = account;
                            account.accountSelected = true;
                        }
                        else
                        {
                            account.accountSelected = false;
                        }
                    }
                    selectedAccountNickName = selectedEligibleAccount.accountName;
                    CustomerBillingAccount.RemoveSelected();
                    CustomerBillingAccount.SetSelected(selectedEligibleAccount.accountNumber);
                    IsTenant = selectedEligibleAccount.IsTenant;
                    if (selectedEligibleAccount.isTaggedSMR)
                    {
                        isTutorialShown = false;
                        isSMR = true;
                        this.mPresenter.GetSSMRAccountStatus(selectedAccountNumber);
                    }
                    else
                    {
                        isSMR = false;
                        UpdateUIForNonSMR();
                    }
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_ssmr_more:
                    break;
            }
            return base.OnOptionsItemSelected(item);
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

        public void HideProgressDialog()
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

        [OnClick(Resource.Id.btnSubmitMeter)]
        internal void OnSubmitMeter(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (DownTimeEntity.IsBCRMDown())
                {
                    // this.SetIsClicked(false);
                    //OnBCRMDownTimeErrorMessage();
                    DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_RS_SYSTEM);
                    OnBCRMDownTimeErrorMessageV2(bcrmEntity);
                }
                else
                {
                    if (smrResponse != null && smrResponse.Response != null && smrResponse.Response.Data != null &&
                                       smrResponse.Response.Data.SMRMROValidateRegisterDetails != null && smrResponse.Response.Data.SMRMROValidateRegisterDetails.Count > 0)
                    {
                        AccountData accountData = new AccountData();
                        SMRAccount eligibleAccount = smrAccountList.Find(account => { return account.accountNumber == selectedAccountNumber; });
                        accountData.AccountNum = selectedAccountNumber;

                        Intent ssmr_submit_meter_activity = new Intent(this, typeof(SubmitMeterReadingActivity));
                        ssmr_submit_meter_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                        ssmr_submit_meter_activity.PutExtra(Constants.SMR_RESPONSE_KEY, JsonConvert.SerializeObject(smrResponse));
                        StartActivityForResult(ssmr_submit_meter_activity, SSMR_SUBMIT_METER_ACTIVITY_CODE);
                    }
                    else
                    {
                        ShowEmptyMeterValidationPopup();
                        this.SetIsClicked(false);
                    }
                }
               
            }
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

        [OnClick(Resource.Id.selectAccountContainer)]
        void OnSelectAccount(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (smrAccountList != null && smrAccountList.Count > 0)
                {
                    this.mPresenter.CheckSMRAccountEligibility(smrAccountList);
                }
                else
                {
                    Intent intent = new Intent(this, typeof(SelectSMRAccountActivity));
                    StartActivityForResult(intent, SSMR_SELECT_ACCOUNT_ACTIVITY_CODE);
                }
            }
        }

        [OnClick(Resource.Id.btnDisableSubmitMeter)]
        void OnDisableSubmitMeter(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                if (DownTimeEntity.IsBCRMDown())
                {
                    // this.SetIsClicked(false);
                    //OnBCRMDownTimeErrorMessage();
                    DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_RS_SYSTEM);
                    OnBCRMDownTimeErrorMessageV2(bcrmEntity);
                }
                else
                {
                    this.SetIsClicked(true);
                    AccountData accountData = new AccountData();
                    SMRAccount eligibleAccount = smrAccountList.Find(account => { return account.accountNumber == selectedAccountNumber; });
                    accountData.AccountNum = selectedAccountNumber;
                    accountData.AddStreet = eligibleAccount.accountAddress;
                    accountData.AccountNickName = eligibleAccount.accountName;
                    SMR_ACTION_KEY = Constants.SMR_DISABLE_FLAG;
                    Intent SSMRTerminateActivity = new Intent(this, typeof(SSMRTerminateActivity));
                    SSMRTerminateActivity.PutExtra("SMR_ACTION", SMR_ACTION_KEY);
                    SSMRTerminateActivity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                    StartActivity(SSMRTerminateActivity);
                }
               
            }
        }

        public override void OnBackPressed()
        {
            if (IsFromUsage)
            {
                SetResult(Result.Ok);
                Finish();
            }
            else
            {
                Finish();
            }
        }

        [OnClick(Resource.Id.btnEnableSubmitMeter)]
        void OnEnableSubmitMeter(object sender, EventArgs eventArgs)
        {
            if (DownTimeEntity.IsBCRMDown())
            {
                // this.SetIsClicked(false);
                //OnBCRMDownTimeErrorMessage();
                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_RS_SYSTEM);
                OnBCRMDownTimeErrorMessageV2(bcrmEntity);
            }
            else
            {
                if (noMeterAccess)
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(Utility.GetLocalizedErrorLabel("noMeterAccessErrorTitle"))
                            .SetMessage(Utility.GetLocalizedErrorLabel("noMeterAccessErrorMessage"))
                            .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                            .SetCTAaction(() => { this.SetIsClicked(false); })
                            .Build().Show();
                    }
                }
                else
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        AccountData accountData = new AccountData();
                        SMRAccount eligibleAccount = smrAccountList.Find(account => { return account.accountNumber == selectedAccountNumber; });
                        accountData.AccountNum = selectedAccountNumber;
                        accountData.AddStreet = eligibleAccount.accountAddress;
                        accountData.AccountNickName = eligibleAccount.accountName;
                        SMR_ACTION_KEY = Constants.SMR_ENABLE_FLAG;
                        this.mPresenter.GetCARegisteredContactInfoAsync(accountData);
                    }
                }

            }
           
        }

        public void ShowSMREligibleAccountList(List<SMRAccount> smrEligibleAccountList)
        {
            Intent intent = new Intent(this, typeof(SelectSMRAccountActivity));
            intent.PutExtra("SMR_ELIGIBLE_ACCOUNT_LIST", JsonConvert.SerializeObject(smrEligibleAccountList));
            StartActivityForResult(intent, SSMR_SELECT_ACCOUNT_ACTIVITY_CODE);
        }

        public void ShowRefreshScreen(bool isShow)
        {
            this.SetIsClicked(false);
            NestedScrollViewContent.Visibility = isShow ? ViewStates.Gone : ViewStates.Visible;
            smrAccountListRefreshContainer.Visibility = isShow ? ViewStates.Visible : ViewStates.Gone;
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            this.mPresenter.GetSSMRAccountStatus(selectedAccountNumber);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Meter Reading History");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            Handler h = new Handler();
            Action myAction = () =>
            {
                NewAppTutorialUtils.ForceCloseNewAppTutorial();
                if (isTutorialShown)
                {
                    OnShowSMRMeterReadingDialog();
                }
            };
            h.PostDelayed(myAction, 50);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void ShowEnableDisableSMR(CAContactDetailsModel contactDetailsModel)
        {
            AccountData accountData = new AccountData();
            SMRAccount eligibleAccount = smrAccountList.Find(account => { return account.accountNumber == selectedAccountNumber; });
            accountData.AccountNum = selectedAccountNumber;
            accountData.AddStreet = eligibleAccount.accountAddress;
            accountData.AccountNickName = eligibleAccount.accountName;

            Intent SSMRTerminateActivity = new Intent(this, typeof(SSMRTerminateActivity));
            SSMRTerminateActivity.PutExtra("SMR_ACTION", SMR_ACTION_KEY);
            SSMRTerminateActivity.PutExtra("SMR_CONTACT_DETAILS", JsonConvert.SerializeObject(contactDetailsModel));
            SSMRTerminateActivity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            StartActivity(SSMRTerminateActivity);
        }

        public void EnableButton()
        {
            this.SetIsClicked(false);
        }

        public void ShowContactNotAvailableTooltip(string title, string content, string cta)
        {
            ContactNotAvailableModel notavailableTooltipModel = new ContactNotAvailableModel();

            if (string.IsNullOrEmpty(title))
            {
                notavailableTooltipModel.Title = GetString(Resource.String.smr_contact_not_available_title);
            }
            else
            {
                notavailableTooltipModel.Title = title;
            }

            if (string.IsNullOrEmpty(content))
            {
                notavailableTooltipModel.Content = GetString(Resource.String.smr_contact_not_available_message);
            }
            else
            {
                notavailableTooltipModel.Content = content;
            }

            if (string.IsNullOrEmpty(cta))
            {
                notavailableTooltipModel.CTA = GetString(Resource.String.smr_contact_not_available_btn);
            }
            else
            {
                notavailableTooltipModel.CTA = cta;
            }

            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(notavailableTooltipModel.Title)
                .SetMessage(notavailableTooltipModel.Content)
                .SetCTALabel(notavailableTooltipModel.CTA)
                .Build().Show();
        }

        public string GetSMRActionKey()
        {
            return SMR_ACTION_KEY;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
        public void OnShowSMRMeterReadingDialog()
        {
            if (!UserSessions.HasSMRMeterHistoryTutorialShown(this.mPref))
            {
                isTutorialShown = true;
                NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.mPresenter.OnGeneraNewAppTutorialList(isSMR));
            }
        }

        public void MeterHistoryCustomScrolling(int yPosition)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        NestedScrollViewContent.ScrollTo(0, yPosition);
                        NestedScrollViewContent.RequestLayout();
                    }
                    catch (System.Exception er)
                    {
                        Utility.LoggingNonFatalError(er);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StopScrolling()
        {
            try
            {
                NestedScrollViewContent.SmoothScrollBy(0, 0);
                NestedScrollViewContent.ScrollTo(0, 0);
                NestedScrollViewContent.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool CheckIsScrollable()
        {
            View child = (View)NestedScrollViewContent.GetChildAt(0);

            return NestedScrollViewContent.Height < child.Height + NestedScrollViewContent.PaddingTop + NestedScrollViewContent.PaddingBottom;
        }

        private void ShowEmptyMeterValidationPopup()
        {
            try
            {
                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(SMRPopUpUtils.GetTitle())
                .SetMessage(SMRPopUpUtils.GetMessage())
                .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                .Build().Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public int GetSMRTopViewHeight()
        {
            int height = 0;

            try
            {
                LinearLayout.LayoutParams smrTitleLayout = SMRMessageTitle.LayoutParameters as LinearLayout.LayoutParams;
                height += SMRMessageTitle.Height + smrTitleLayout.TopMargin;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            if (selectAccountContainer.Visibility == ViewStates.Visible)
            {
                try
                {
                    LinearLayout.LayoutParams selectAccountContainerLayout = selectAccountContainer.LayoutParameters as LinearLayout.LayoutParams;
                    height += selectAccountContainer.Height + selectAccountContainerLayout.TopMargin + selectAccountContainerLayout.BottomMargin;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            if (SMRActionContainer.Visibility == ViewStates.Visible)
            {
                try
                {
                    height += SMRActionContainer.Height;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            if (NonSMRActionContainer.Visibility == ViewStates.Visible)
            {
                try
                {
                    height -= (int)DPUtils.ConvertDPToPx(16f);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            return height;
        }

        public void OnBCRMDownTimeErrorMessageV2(DownTimeEntity bcrmEntity)
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_WITH_FLOATING_IMAGE_ONE_BUTTON)
           .SetHeaderImage(Resource.Drawable.maintenance_bcrm_new)
           .SetTitle(bcrmEntity.DowntimeTextMessage)
           .SetMessage(bcrmEntity.DowntimeMessage)
           .SetCTALabel(Utility.GetLocalizedCommonLabel("close"))
           //.SetCTAaction(() => { isBCMRDownDialogShow = false; })
           .Build()
           .Show();
        }
    }
}
