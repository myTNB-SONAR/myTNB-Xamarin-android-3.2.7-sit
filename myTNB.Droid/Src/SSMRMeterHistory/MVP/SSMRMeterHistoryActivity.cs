using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
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
        private MaterialDialog SSMRMenuDialog;
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

        [BindView(Resource.Id.readingHistoryList)]
        LinearLayout ReadingHistoryListContainer;

        [BindView(Resource.Id.disableSMRBtnContainer)]
        LinearLayout DisableSMRBtnContainer;

        [BindView(Resource.Id.non_smr_note_content)]
        TextView NonSMRNoteContent;

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

                TextViewUtils.SetMuseoSans500Typeface(SMRMainTitle, SMRListHeader, SMRMessageTitle, btnSubmitMeter, btnEnableSubmitMeter, btnDisableSubmitMeter, btnRefresh);
                TextViewUtils.SetMuseoSans300Typeface(SMRMainContent, SMRAccountTitle, SMRAccountSelected, NonSMRNoteContent, EmptySMRHistoryMessage, refreshMsg);

                SMRMessageTitle.Text = GetLabelByLanguage("subTitle");
                SMRAccountTitle.Text = GetLabelCommonByLanguage("account").ToUpper();
                SMRAccountSelected.Text = GetLabelCommonByLanguage("selectAccount");
                SMRListHeader.Text = GetLabelByLanguage("headerTitle");
                btnEnableSubmitMeter.Text = GetLabelByLanguage("enableSSMRCTA");
                btnDisableSubmitMeter.Text = GetLabelByLanguage("disableSSMRCTA");
                EmptySMRHistoryMessage.Text = GetLabelByLanguage("noHistoryData");
                refreshMsg.Text = GetLabelCommonByLanguage("refreshDescription");
                btnRefresh.Text = GetLabelCommonByLanguage("refreshNow");
                NonSMRNoteContent.Text = GetLabelByLanguage("enableSSMRDescription");
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
                            if (extras.ContainsKey(Constants.SMR_RESPONSE_KEY))
                            {
                                smrResponse = JsonConvert.DeserializeObject<SMRActivityInfoResponse>(extras.GetString(Constants.SMR_RESPONSE_KEY));
                                UpdateUIForSMR(smrResponse);
                                this.mPresenter.CheckIsBtnSubmitHide(smrResponse);
                                isSMR = true;
                                isTutorialShown = true;
                            }

                            if (extras.ContainsKey("fromNotificationDetails"))
                            {
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
                        SMRAccount smrSelectedAccount = smrAccountList.Find(account => {
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
            //SMR UI visiblility
            SMRActionContainer.Visibility = isNonSMRAccount ? ViewStates.Gone : ViewStates.Visible;
            ReadingHistoryListContainer.Visibility = isNonSMRAccount ? ViewStates.Gone : ViewStates.Visible;
            DisableSMRBtnContainer.Visibility = isNonSMRAccount ? ViewStates.Gone : ViewStates.Visible;

            //Checking for no tagged SMR
            NonSMRActionContainer.Visibility = hasNoSMREligibleAccount ? ViewStates.Visible : ViewStates.Gone;
        }

        private void UpdateUIForNonSMR()
        {
            ShowRefreshScreen(false);
            SMRAccountSelected.Text = selectedAccountNickName;
            ShowNonSMRVisible(true,true);

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
            ShowNonSMRVisible(false,false);
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
                    foreach(SMRAccount account in smrAccountList)
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
                    OnClickSMRMenuMore();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OnClickSMRMenuMore()
        {
            try
            {
                SSMRMenuDialog = new MaterialDialog.Builder(this)
                    .CustomView(Resource.Layout.SSMRMenuListLayout, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

                View dialogView = SSMRMenuDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);
                WindowManagerLayoutParams wlp = SSMRMenuDialog.Window.Attributes;
                wlp.Gravity = GravityFlags.Top;
                wlp.Width = ViewGroup.LayoutParams.MatchParent;
                wlp.Height = ViewGroup.LayoutParams.WrapContent;
                SSMRMenuDialog.Window.Attributes = wlp;

                ImageView btnSMRMenuClose = SSMRMenuDialog.FindViewById<ImageView>(Resource.Id.btnSMRMenuClose);
                RecyclerView mSMRMenuRecyclerView = SSMRMenuDialog.FindViewById<RecyclerView>(Resource.Id.smrMenuList);
                mSMRMenuRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
                if (smrResponse.Response.Data.MeterReadingMenu.Count > 0)
                {
                    ssmrMeterHistoryMenuList.Clear();
                    ssmrMeterHistoryMenuList.AddRange(smrResponse.Response.Data.MeterReadingMenu);
                    meterHistoryMenuAdapter = new SSMRMeterHistoryMenuAdapter(smrResponse.Response.Data.MeterReadingMenu);
                    mSMRMenuRecyclerView.SetAdapter(meterHistoryMenuAdapter);
                    meterHistoryMenuAdapter.ClickChanged += OnClickChanged;
                }

                btnSMRMenuClose.Click += delegate
                {
                    SSMRMenuDialog.Dismiss();
                };

                SSMRMenuDialog.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnClickChanged(object sender, int position)
        {
            try
            {
                if (position != -1)
                {
                    SSMRMeterHistoryMenuModel selectedMenu = ssmrMeterHistoryMenuList[position];
                    if (selectedMenu.MenuId == "1004")
                    {
                        ShowProgressDialog();
                        Intent SSMRTerminateActivity = new Intent(this, typeof(SSMRTerminateActivity));
                        SSMRTerminateActivity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                        StartActivityForResult(SSMRTerminateActivity, SSMR_METER_HISTORY_ACTIVITY_CODE);
                        HideProgressDialog();
                        SSMRMenuDialog.Dismiss();
                    }
                    else
                    {
                        SSMRMenuDialog.Dismiss();
                    }
                }
                else
                {
                    SSMRMenuDialog.Dismiss();
                }
            }
            catch (System.Exception e)
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

        class ClickSpan : ClickableSpan
        {
            public Action<View> Click;
            public Color textColor { get; set; }
            public Typeface typeFace { get; set; }

            public override void OnClick(View widget)
            {
                if (Click != null)
                {
                    Click(widget);
                }
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.Color = textColor;
                ds.SetTypeface(typeFace);
                ds.UnderlineText = false;
            }
        }

        private void ShowEmptyMeterValidationPopup()
        {
            try
            {
                MaterialDialog materialDialog = new MaterialDialog.Builder(this)
                        .CustomView(Resource.Layout.CustomToolTipWithHeaderLayout, false)
                        .Cancelable(false)
                        .CanceledOnTouchOutside(false)
                        .Build();

                View dialogView = materialDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);
                WindowManagerLayoutParams wlp = materialDialog.Window.Attributes;
                wlp.Gravity = GravityFlags.Center;
                wlp.Width = ViewGroup.LayoutParams.MatchParent;
                wlp.Height = ViewGroup.LayoutParams.WrapContent;
                materialDialog.Window.Attributes = wlp;

                TextView tooltipTitle = materialDialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = materialDialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipCTA = materialDialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);

                tooltipCTA.Text = Utility.GetLocalizedCommonLabel("gotIt");
                tooltipCTA.Click += delegate
                {
                    materialDialog.Dismiss();
                };

                tooltipTitle.Text = SMRPopUpUtils.GetTitle();
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(SMRPopUpUtils.GetMessage(), FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(SMRPopUpUtils.GetMessage());
                }

                SpannableString s = new SpannableString(tooltipMessage.TextFormatted);
                var clickableSpan = new ClickSpan()
                {
                    textColor = Resources.GetColor(Resource.Color.powerBlue),
                    typeFace = Typeface.CreateFromAsset(this.Assets, "fonts/" + TextViewUtils.MuseoSans500)
                };
                clickableSpan.Click += v =>
                {
                    if (SMRPopUpUtils.GetMessage() != null && SMRPopUpUtils.GetMessage().Contains("tel:"))
                    {
                        //Lauch FAQ
                        int startIndex = SMRPopUpUtils.GetMessage().LastIndexOf("\"tel") + 1;
                        int lastIndex = SMRPopUpUtils.GetMessage().LastIndexOf("\">") - 1;
                        int lengthOfId = (lastIndex - startIndex) + 1;
                        if (lengthOfId < SMRPopUpUtils.GetMessage().Length)
                        {
                            string phone = SMRPopUpUtils.GetMessage().Substring(startIndex, lengthOfId);
                            if (!string.IsNullOrEmpty(phone))
                            {
                                var uri = Android.Net.Uri.Parse(phone);
                                var intent = new Intent(Intent.ActionDial, uri);
                                StartActivity(intent);
                            }
                        }
                    }
                };
                var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
                int startFAQLink = s.GetSpanStart(urlSpans[0]);
                int endFAQLink = s.GetSpanEnd(urlSpans[0]);
                s.RemoveSpan(urlSpans[0]);
                s.SetSpan(clickableSpan, startFAQLink, endFAQLink, SpanTypes.ExclusiveExclusive);
                tooltipMessage.TextFormatted = s;
                tooltipMessage.MovementMethod = new LinkMovementMethod();
                materialDialog.Show();
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
                    height += NonSMRActionContainer.Height;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            return height;
        }
    }
}
