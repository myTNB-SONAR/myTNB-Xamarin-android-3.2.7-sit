using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.Adapter;
using myTNB_Android.Src.SSMRTerminate.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
	[Activity(Label = "@string/ssmr_meter_history_activity_title"
		  , ScreenOrientation = ScreenOrientation.Portrait
		  , Theme = "@style/Theme.SSMRMeterHistoryStyle")]
	public class SSMRMeterHistoryActivity : BaseToolbarAppCompatActivity, SSMRMeterHistoryContract.IView
	{
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

        private SMRActivityInfoResponse smrResponse;

        private AccountData selectedAccount;
        private SMRAccount selectedEligibleAccount;
        private string selectedAccountNickName;

        private MaterialDialog SSMRMenuDialog;

        private bool IsFromUsage = false;

        LoadingOverlay loadingOverlay;

        SSMRMeterHistoryMenuAdapter meterHistoryMenuAdapter;

        private List<SSMRMeterHistoryMenuModel> ssmrMeterHistoryMenuList = new List<SSMRMeterHistoryMenuModel>();

        public readonly static int SSMR_METER_HISTORY_ACTIVITY_CODE = 8796;
        public readonly static int SSMR_SUBMIT_METER_ACTIVITY_CODE = 8797;
        public readonly static int SSMR_SELECT_ACCOUNT_ACTIVITY_CODE = 8798;

        private SSMRMeterHistoryContract.IPresenter mPresenter;
        private string selectedAccountNumber;
        private List<SMRAccount> smrAccountList = new List<SMRAccount>();

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
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.bg_smr);

                TextViewUtils.SetMuseoSans500Typeface(SMRMainTitle, SMRListHeader, SMRMessageTitle, btnSubmitMeter, btnEnableSubmitMeter, btnDisableSubmitMeter);
                TextViewUtils.SetMuseoSans300Typeface(SMRMainContent, SMRAccountTitle, SMRAccountSelected, NonSMRNoteContent);

                mPresenter = new SSMRMeterHistoryPresenter(this);
                mSMRRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
                smrAccountList = this.mPresenter.GetEligibleSMRAccountList();
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
                        }
                    }
                    else
                    {
                        SMRAccount smrSelectedAccount = smrAccountList.Find(account => {
                            return account.isTaggedSMR;
                        });
                        selectedAccountNumber = smrSelectedAccount.accountNumber;
                        CustomerBillingAccount.RemoveSelected();
                        CustomerBillingAccount.SetSelected(smrSelectedAccount.accountNumber);
                        if (smrSelectedAccount != null)
                        {
                            selectedAccountNickName = smrSelectedAccount.accountName;
                            this.mPresenter.GetSSMRAccountStatus(smrSelectedAccount.accountNumber);
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
                    selectedAccountNumber = smrSelectedAccount.accountNumber;
                    CustomerBillingAccount.RemoveSelected();
                    CustomerBillingAccount.SetSelected(smrSelectedAccount.accountNumber);
                    if (smrSelectedAccount != null)
                    {
                        selectedAccountNickName = smrSelectedAccount.accountName;
                        this.mPresenter.GetSSMRAccountStatus(smrSelectedAccount.accountNumber);
                    }
                    else
                    {
                        ShowNonSMRVisible(true,false);
                    }
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

            SSMRMeterHistoryAdapter adapter = new SSMRMeterHistoryAdapter(activityInfoResponse.Response.Data.MeterReadingHistory);
            mSMRRecyclerView.SetAdapter(adapter);

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
        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == SSMR_METER_HISTORY_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    SetResult(Result.Ok);
                    Finish();
                }
            }
            if (requestCode == SSMR_SELECT_ACCOUNT_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    selectedAccountNumber = data.GetStringExtra("SELECTED_ACCOUNT_NUMBER");
                    selectedEligibleAccount = smrAccountList.Find(account =>
                    {
                        return account.accountNumber == selectedAccountNumber;
                    });
                    selectedAccountNickName = selectedEligibleAccount.accountName;
                    CustomerBillingAccount.RemoveSelected();
                    CustomerBillingAccount.SetSelected(selectedEligibleAccount.accountNumber);
                    if (selectedEligibleAccount.isTaggedSMR)
                    {
                        this.mPresenter.GetSSMRAccountStatus(selectedAccountNumber);
                    }
                    else
                    {
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

        public void HideProgressDialog()
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

        [OnClick(Resource.Id.btnSubmitMeter)]
        internal void OnSubmitMeter(object sender, EventArgs eventArgs)
        {
            Intent ssmr_submit_meter_activity = new Intent(this, typeof(SubmitMeterReadingActivity));
            ssmr_submit_meter_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            ssmr_submit_meter_activity.PutExtra(Constants.SMR_RESPONSE_KEY, JsonConvert.SerializeObject(smrResponse));
            StartActivityForResult(ssmr_submit_meter_activity, SSMR_SUBMIT_METER_ACTIVITY_CODE);
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
            this.mPresenter.CheckSMRAccountEligibility(smrAccountList);
        }

        [OnClick(Resource.Id.btnDisableSubmitMeter)]
        void OnDisableSubmitMeter(object sender, EventArgs eventArgs)
        {
            AccountData accountData = new AccountData();
            SMRAccount eligibleAccount = smrAccountList.Find(account => { return account.accountNumber == selectedAccountNumber; });
            accountData.AccountNum = selectedAccountNumber;
            accountData.AddStreet = eligibleAccount.accountAddress;
            accountData.AccountNickName = eligibleAccount.accountName;
            Intent SSMRTerminateActivity = new Intent(this, typeof(SSMRTerminateActivity));
            SSMRTerminateActivity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            StartActivityForResult(SSMRTerminateActivity, SSMR_METER_HISTORY_ACTIVITY_CODE);
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

        public void ShowSMREligibleAccountList(List<SMRAccount> smrEligibleAccountList)
        {
            Intent intent = new Intent(this, typeof(SelectSMRAccountActivity));
            intent.PutExtra("SMR_ELIGIBLE_ACCOUNT_LIST", JsonConvert.SerializeObject(smrEligibleAccountList));
            StartActivityForResult(intent, SSMR_SELECT_ACCOUNT_ACTIVITY_CODE);
        }

        public void ShowRefreshScreen(bool isShow)
        {
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Meter Reading History Screen");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
