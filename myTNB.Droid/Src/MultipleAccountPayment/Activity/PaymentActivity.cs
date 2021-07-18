using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.AppBar;
using myTNB.Mobile;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB.Mobile.API.Managers.Scheduler;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB_Android.Src.AppointmentScheduler.AppointmentSelect.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using myTNB_Android.Src.MultipleAccountPayment.Fragment;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace myTNB_Android.Src.MultipleAccountPayment.Activity
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

        public bool paymentReceiptGenerated = false;

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
                appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBar);
                toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
                frameContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
                coordinatorLayout = FindViewById<AndroidX.CoordinatorLayout.Widget.CoordinatorLayout>(Resource.Id.coordinatorLayout);
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }

                    if (extras.ContainsKey("PAYMENT_ITEMS"))
                    {
                        accounts = DeSerialze<List<MPAccount>>(extras.GetString("PAYMENT_ITEMS"));
                    }

                    if (extras.ContainsKey("ACCOUNT_CHARGES_LIST"))
                    {
                        accountChargeList = DeSerialze<List<AccountChargeModel>>(extras.GetString("ACCOUNT_CHARGES_LIST"));
                    }

                    if (extras.ContainsKey("ISAPPLICATIONPAYMENT") && Intent.Extras.GetBoolean("ISAPPLICATIONPAYMENT"))
                    {
                        IsApplicationPayment = true;
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
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
        public async void OnManageBillDelivery()
        {
            ShowProgressDialog();
            try
            {
                CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
                AccountData selectedAccountData = AccountData.Copy(customerAccount, true);
                Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
                intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
                intent.PutExtra("OptedEBill", "OptedEBill");
                StartActivity(intent);
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            HideProgressDialog();
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

    }
}