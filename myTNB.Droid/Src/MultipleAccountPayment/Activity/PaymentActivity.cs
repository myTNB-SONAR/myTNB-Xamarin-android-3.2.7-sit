﻿using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.AppBar;
using myTNB.Mobile;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB.Mobile.API.Managers.Scheduler;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Models.DBR;
using myTNB.AndroidApp.Src.AppointmentScheduler.AppointmentSelect.MVP;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Base.Api;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.DeviceCache;
using myTNB.AndroidApp.Src.ManageBillDelivery.MVP;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Fragment;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Model;
using myTNB.AndroidApp.Src.MyHome;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.SessionCache;
using myTNB.AndroidApp.Src.SummaryDashBoard.Models;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace myTNB.AndroidApp.Src.MultipleAccountPayment.Activity
{
    [Activity(Label = "Select Payment Method"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.MakePayment")]
    public class PaymentActivity : BaseToolbarAppCompatActivity
    {
        AccountData selectedAccount;
        List<MPAccount> accounts;
        string total;
        AndroidX.Fragment.App.Fragment currentFragment;

        public readonly static int SELECT_PAYMENT_ACTIVITY_CODE = 2367;
        private AndroidX.AppCompat.Widget.Toolbar toolbar;
        private AppBarLayout appBarLayout;
        private FrameLayout frameContainer;
        private List<AccountChargeModel> accountChargeList;
        private AndroidX.CoordinatorLayout.Widget.CoordinatorLayout coordinatorLayout;

        // Mark: Application Payment
        private bool IsApplicationPayment;
        private ApplicationPaymentDetail ApplicationPaymentDetail;
        private string ApplicationType = string.Empty;
        private string SearchTerm = string.Empty;
        private string ApplicationSystem = string.Empty;
        private string StatusId = string.Empty;
        private string StatusCode = string.Empty;
        internal GetApplicationStatusDisplay ApplicationDetailDisplay;
        private GetBillRenderingResponse billRenderingResponse;
        //private PostBREligibilityIndicatorsResponse billRenderingTenantResponse;
        public bool paymentReceiptGenerated = false;

        internal bool ShouldBackToHome { set; get; } = false;
        internal bool IsMultiplePayment;
        internal static List<string> CAsWithPaperBillList;

        public override int ResourceId()
        {
            return Resource.Layout.MakePaymentView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override void SetToolBarTitle(string title)
        {
            base.SetToolBarTitle(title);
        }

        public void ShowToolBar()
        {
            if (appBarLayout != null)
            {
                TypedValue tv = new TypedValue();
                int actionBarHeight = 0;
                if (Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, tv, true))
                {
                    actionBarHeight = TypedValue.ComplexToDimensionPixelSize(tv.Data, Resources.DisplayMetrics);
                }

                appBarLayout.Visibility = ViewStates.Visible;
                AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams lp = new AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams(AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams.MatchParent, AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams.MatchParent);
                lp.SetMargins(0, actionBarHeight, 0, 0);

                frameContainer.LayoutParameters = lp;
            }
        }

        public void HideToolBar()
        {
            if (appBarLayout != null)
            {
                appBarLayout.Visibility = ViewStates.Gone;
                AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams lp = new AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams(AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams.MatchParent, AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams.MatchParent);
                lp.SetMargins(0, 0, 0, 0);

                frameContainer.LayoutParameters = lp;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            try
            {
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBar);
                toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
                frameContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
                coordinatorLayout = FindViewById<AndroidX.CoordinatorLayout.Widget.CoordinatorLayout>(Resource.Id.coordinatorLayout);
                Bundle extras = Intent.Extras;
                CAsWithPaperBillList = new List<string>();
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }

                    if (extras.ContainsKey("PAYMENT_ITEMS"))
                    {
                        accounts = DeSerialze<List<MPAccount>>(extras.GetString("PAYMENT_ITEMS"));
                        IsMultiplePayment = accounts != null && accounts.Count > 1;
                    }

                    if (extras.ContainsKey("ACCOUNT_CHARGES_LIST"))
                    {
                        accountChargeList = DeSerialze<List<AccountChargeModel>>(extras.GetString("ACCOUNT_CHARGES_LIST"));
                    }

                    if (extras.ContainsKey("ISAPPLICATIONPAYMENT"))
                    {
                        IsApplicationPayment = Intent.Extras.GetBoolean("ISAPPLICATIONPAYMENT");
                        if (extras.ContainsKey("APPLICATIONPAYMENTDETAIL"))
                        {
                            ApplicationPaymentDetail = DeSerialze<ApplicationPaymentDetail>(extras.GetString("APPLICATIONPAYMENTDETAIL"));
                        }
                        if (extras.ContainsKey("ApplicationDetailDisplay"))
                        {
                            ApplicationDetailDisplay = DeSerialze<GetApplicationStatusDisplay>(extras.GetString("ApplicationDetailDisplay"));
                        }
                        if (extras.ContainsKey("ApplicationType"))
                        {
                            ApplicationType = extras.GetString("ApplicationType");
                        }
                        if (extras.ContainsKey("SearchTerm"))
                        {
                            SearchTerm = extras.GetString("SearchTerm");
                        }
                        if (extras.ContainsKey("ApplicationSystem"))
                        {
                            ApplicationSystem = extras.GetString("ApplicationSystem");
                        }
                        if (extras.ContainsKey("StatusId"))
                        {
                            StatusId = extras.GetString("StatusId");
                        }
                        if (extras.ContainsKey("StatusCode"))
                        {
                            StatusCode = extras.GetString("StatusCode");
                        }
                    }

                    if (extras.ContainsKey("TOTAL"))
                    {
                        total = Intent.Extras.GetString("TOTAL");
                    }
                }
                OnLoadMainFragment();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void NextFragment(AndroidX.Fragment.App.Fragment fragment, Bundle bundle)
        {
            if (fragment is MPSelectPaymentMethodFragment)
            {
                var paymentWebViewFragment = new MPPaymentWebViewFragment();
                paymentWebViewFragment.Arguments = bundle;
                var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.fragment_container, paymentWebViewFragment);
                fragmentTransaction.AddToBackStack(null);
                fragmentTransaction.Commit();
                currentFragment = paymentWebViewFragment;
            }
        }

        public void OnLoadMainFragment()
        {
            if (!IsFinishing && !IsDestroyed)
            {
                AndroidX.Fragment.App.Fragment selectPaymentFragment = new MPSelectPaymentMethodFragment();
                Bundle bundle = new Bundle();
                if (IsApplicationPayment)
                {
                    UpdateApplicationPayment();
                    bundle.PutBoolean("ISAPPLICATIONPAYMENT", IsApplicationPayment);
                    bundle.PutString("APPLICATIONPAYMENTDETAILS", JsonConvert.SerializeObject(ApplicationPaymentDetail));
                    bundle.PutString("ApplicationType", ApplicationType);
                    bundle.PutString("SearchTerm", SearchTerm);
                    bundle.PutString("ApplicationSystem", ApplicationSystem);
                    bundle.PutString("StatusId", StatusId);
                    bundle.PutString("StatusCode", StatusCode);
                }
                else
                {
                    bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                    bundle.PutString("PAYMENT_ITEMS", JsonConvert.SerializeObject(accounts));
                    bundle.PutString("ACCOUNT_CHARGES_LIST", JsonConvert.SerializeObject(accountChargeList));
                }
                bundle.PutString("TOTAL", total);
                selectPaymentFragment.Arguments = bundle;
                var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.fragment_container, selectPaymentFragment);
                fragmentTransaction.Commit();
                currentFragment = selectPaymentFragment;
            }
        }

        private async void UpdateApplicationPayment()
        {
            ApplicationPaymentDetail = await AccountTypeCache.Instance.UpdateApplicationPayment(ApplicationPaymentDetail, this);
        }

        internal void SetPaymentReceiptFlag(bool flag, SummaryDashBordRequest summaryDashBoardRequest)
        {
            try
            {
                paymentReceiptGenerated = flag;
                if (paymentReceiptGenerated)
                {
                    if (ConnectionUtils.HasInternetConnection(this))
                    {
                        SummaryDashBaordUpdate(summaryDashBoardRequest);
                    }
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Payment Methods");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearBackStack()
        {
            try
            {
                var manager = this.SupportFragmentManager;
                if (manager.BackStackEntryCount > 0)
                {
                    manager.PopBackStack(SupportFragmentManager.GetBackStackEntryAt(0).Id, (int)Android.App.PopBackStackFlags.Inclusive);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override void OnBackPressed()
        {
            try
            {
                int count = this.SupportFragmentManager.BackStackEntryCount;
                Log.Debug("OnBackPressed", "fragment stack count :" + count);
                if (count == 0 || paymentReceiptGenerated)
                {
                    if (paymentReceiptGenerated)
                    {
                        SetResult(Result.Ok);
                    }
                    Finish();
                }
                else
                {
                    Log.Debug("MakePaymentActivity", "Current Fragment :" + currentFragment.Class);
                    if (currentFragment is MPPaymentWebViewFragment)
                    {
                        if (ShouldBackToHome)
                        {
                            MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                            HomeMenuUtils.ResetAll();
                            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                            this.StartActivity(DashboardIntent);
                        }
                        else
                        {
                            MyTNBAppToolTipBuilder cancelPaymentPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                                .SetTitle(Utility.GetLocalizedLabel("MakePayment", "abortTitle"))
                                .SetMessage(Utility.GetLocalizedLabel("MakePayment", "abortMessage"))
                                .SetCTALabel(Utility.GetLocalizedCommonLabel("no"))
                                .SetSecondaryCTALabel(Utility.GetLocalizedCommonLabel("yes"))
                                .SetSecondaryCTAaction(() =>
                                {
                                    this.SupportFragmentManager.PopBackStack();
                                    this.SetToolBarTitle(Utility.GetLocalizedLabel("SelectPaymentMethod", "title"));
                                })
                                .Build();
                            cancelPaymentPopup.Show();
                        }
                    }
                    else
                    {
                        this.SupportFragmentManager.PopBackStack();
                    }
                }
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

        public void SummaryDashBaordUpdate(SummaryDashBordRequest summaryDashBoardRequest)
        {
            try
            {
                if (summaryDashBoardRequest != null)
                {
                    if (summaryDashBoardRequest.AccNum != null && summaryDashBoardRequest.AccNum.Count() > 0)
                        SummaryDashBoardApiCall.GetSummaryDetails(summaryDashBoardRequest);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void NavigateBackToMicrosite()
        {
            Intent intent = new Intent();
            intent.PutExtra(MyHomeConstants.IS_PAYMENT_SUCCESSFUL, true);
            SetResult(Result.Ok, intent);
            Finish();
        }

        public override void Finish()
        {

            if (paymentReceiptGenerated)
            {
                SelectAccountsActivity.selectAccountsActivity?.SetResult(Result.Ok);
            }
            else
            {
                SelectAccountsActivity.selectAccountsActivity?.SetResult(Result.Canceled);
            }
            base.Finish();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        private bool GetIsOwnerTag(string ca)
        {
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            int accountIndex = allAccountList.FindIndex(x => x.AccNum == ca);
            if (accountIndex > -1 && allAccountList[accountIndex] != null)
            {
                return allAccountList[accountIndex].isOwned;
            }
            return false;
        }

        public async void GetBillRenderingAsync()
        {
            try
            {
                DynatraceHelper.OnTrack(IsMultiplePayment
                    ? DynatraceConstants.DBR.CTAs.PaymentSuccess.Multiple
                    : DynatraceConstants.DBR.CTAs.PaymentSuccess.Single);
                if (CAsWithPaperBillList != null && CAsWithPaperBillList.Count > 0)
                {
                    ShowProgressDialog();

                    string dbrAccount = CAsWithPaperBillList[0];

                    if (!AccessTokenCache.Instance.HasTokenSaved(this))
                    {
                        string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                        AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
                    }
                    billRenderingResponse = await DBRManager.Instance.GetBillRendering(dbrAccount, AccessTokenCache.Instance.GetAccessToken(this));

                    //Nullity Check
                    if (billRenderingResponse != null
                       && billRenderingResponse.StatusDetail != null
                       && billRenderingResponse.StatusDetail.IsSuccess
                       && billRenderingResponse.Content != null
                       && billRenderingResponse.Content.DBRType != MobileEnums.DBRTypeEnum.None)
                    {
                        //billRenderingTenantResponse = await DBRManager.Instance.PostBREligibilityIndicators(CAsWithPaperBillList, UserEntity.GetActive().UserID, AccessTokenCache.Instance.GetAccessToken(this));
                        Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
                        intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(billRenderingResponse));
                        //intent.PutExtra("billRenderingTenantResponse", JsonConvert.SerializeObject(billRenderingTenantResponse));
                        intent.PutExtra("accountNumber", dbrAccount);
                        StartActivity(intent);
                    }
                    else
                    {
                        MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(billRenderingResponse?.StatusDetail?.Title ?? string.Empty)
                            .SetMessage(billRenderingResponse?.StatusDetail?.Message ?? string.Empty)
                            .SetCTALabel(billRenderingResponse?.StatusDetail?.PrimaryCTATitle ?? string.Empty)
                            .Build();
                        errorPopup.Show();
                    }
                    HideProgressDialog();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public string GetEligibleDBRAccount()
        {
            List<string> dBRCAs = DBRUtility.Instance.GetCAList();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            string account = string.Empty;
            if (dBRCAs.Count > 0)
            {
                foreach (var paymentCa in accounts)
                {
                    account = dBRCAs.Where(x => x == paymentCa.accountNumber).FirstOrDefault();
                    break;
                }
            }

            return account;
        }
        public async void OnSetAppointment()
        {
            ShowProgressDialog();

            string appointment = "NewAppointment";
            try
            {
                string businessArea = ApplicationDetailDisplay.BusinessArea ?? string.Empty;
                SchedulerDisplay response = await ScheduleManager.Instance.GetAvailableAppointment(businessArea);
                if (response.StatusDetail.IsSuccess)
                {
                    Intent intent = new Intent(this, typeof(AppointmentSelectActivity));
                    intent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(ApplicationDetailDisplay));
                    intent.PutExtra("newAppointmentResponse", JsonConvert.SerializeObject(response));
                    intent.PutExtra("appointment", appointment);
                    StartActivityForResult(intent, Constants.APPLICATION_STATUS_DETAILS_SCHEDULER_REQUEST_CODE);
                    SetResult(Result.Ok);
                    Finish();
                }
                else
                {
                    MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                       .SetTitle(response.StatusDetail.Title)
                       .SetMessage(response.StatusDetail.Message)
                       .SetCTALabel(response.StatusDetail.PrimaryCTATitle)
                       .Build();
                    errorPopup.Show();
                }
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            HideProgressDialog();
        }

        //  TODO: AndroidX Temporary Fix for Android 5,5.1 
        //  TODO: AndroidX Due to this: https://github.com/xamarin/AndroidX/issues/131
        public override AssetManager Assets =>
            (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop && Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            ? Resources.Assets : base.Assets;

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == Constants.MYHOME_MICROSITE_REQUEST_CODE)
            {
                if (data != null && data.Extras is Bundle extras && extras != null)
                {
                    if (extras.ContainsKey(MyHomeConstants.IS_RATING_SUCCESSFUL))
                    {
                        bool ratingSuccess = extras.GetBoolean(MyHomeConstants.IS_RATING_SUCCESSFUL);
                        if (ratingSuccess)
                        {
                            Intent intent = new Intent();
                            intent.PutExtra(MyHomeConstants.IS_RATING_SUCCESSFUL, true);
                            SetResult(Result.Ok, intent);
                            Finish();
                        }
                    }
                }
            }
        }
    }
}