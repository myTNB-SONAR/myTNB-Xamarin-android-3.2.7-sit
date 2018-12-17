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
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using myTNB_Android.Src.MultipleAccountPayment.Fragment;
using myTNB_Android.Src.Utils;
using Android.Util;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Base.Api;
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
        Android.App.Fragment currentFragment;

        private MaterialDialog mCancelPaymentDialog;
        public readonly static int SELECT_PAYMENT_ACTIVITY_CODE = 2367;

        private Android.Support.V7.Widget.Toolbar toolbar;
        private Android.Support.Design.Widget.AppBarLayout appBarLayout;
        private FrameLayout frameContainer;
        private Android.Support.Design.Widget.CoordinatorLayout coordinatorLayout;

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
                Android.Support.Design.Widget.CoordinatorLayout.LayoutParams lp = new Android.Support.Design.Widget.CoordinatorLayout.LayoutParams(Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent, Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent);
                lp.SetMargins(0, actionBarHeight, 0, 0);

                frameContainer.LayoutParameters = lp;
            }
        }

        public void HideToolBar()
        {
            if (appBarLayout != null)
            {
                appBarLayout.Visibility = ViewStates.Gone;
                Android.Support.Design.Widget.CoordinatorLayout.LayoutParams lp = new Android.Support.Design.Widget.CoordinatorLayout.LayoutParams(Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent, Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent);
                lp.SetMargins(0, 0, 0, 0);

                frameContainer.LayoutParameters = lp;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            try {
            appBarLayout = FindViewById<Android.Support.Design.Widget.AppBarLayout>(Resource.Id.appBar);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            frameContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
            coordinatorLayout = FindViewById<Android.Support.Design.Widget.CoordinatorLayout>(Resource.Id.coordinatorLayout);
            selectedAccount = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
            accounts = JsonConvert.DeserializeObject<List<MPAccount>>(Intent.Extras.GetString("PAYMENT_ITEMS"));
            total = Intent.Extras.GetString("TOTAL");

            OnLoadMainFragment();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //Android.App.Fragment selectPaymentFragment = new SelectPaymentMethodFragment();
            //Bundle bundle = new Bundle();
            //bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            //selectPaymentFragment.Arguments = bundle;
            //var fragmentTransaction = FragmentManager.BeginTransaction();
            //fragmentTransaction.Add(Resource.Id.fragment_container, selectPaymentFragment);
            //fragmentTransaction.Commit();
            //currentFragment = selectPaymentFragment;
        }

        public void nextFragment(Android.App.Fragment fragment, Bundle bundle)
        {
            if (fragment is MPSelectPaymentMethodFragment)
            {
                var paymentWebViewFragment = new MPPaymentWebViewFragment();
                paymentWebViewFragment.Arguments = bundle;
                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.fragment_container, paymentWebViewFragment);
                fragmentTransaction.AddToBackStack(null);
                fragmentTransaction.Commit();
                currentFragment = paymentWebViewFragment;
            }
        }

        public void OnLoadMainFragment()
        {
            Android.App.Fragment selectPaymentFragment = new MPSelectPaymentMethodFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            bundle.PutString("PAYMENT_ITEMS", JsonConvert.SerializeObject(accounts));
            bundle.PutString("TOTAL", total);
            selectPaymentFragment.Arguments = bundle;
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, selectPaymentFragment);
            fragmentTransaction.Commit();
            currentFragment = selectPaymentFragment;
        }

        internal void SetPaymentReceiptFlag(bool flag, SummaryDashBordRequest summaryDashBoardRequest)
        {
            try {
            paymentReceiptGenerated = flag;
            if (paymentReceiptGenerated) {
                if (ConnectionUtils.HasInternetConnection(this)) {
                    SummaryDashBaordUpdate(summaryDashBoardRequest);    
                }
                }
        }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearBackStack()
        {
            try {
            FragmentManager manager = this.FragmentManager;
            if (manager.BackStackEntryCount > 0)
            {
                manager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, FragmentManager.PopBackStackInclusive);
            }
        }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override void OnBackPressed()
        {
            try {
            int count = this.FragmentManager.BackStackEntryCount;
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
                    mCancelPaymentDialog = new MaterialDialog.Builder(this)
                        .Title("Abort Payment!")
                        .Content(GetString(Resource.String.error_abort_payment))
                        .Cancelable(false)
                        .PositiveText("Abort")
                        .PositiveColor(Resource.Color.black)
                        .OnPositive((dialog, which) => {
                            this.FragmentManager.PopBackStack();
                            this.SetToolBarTitle("Select Payment Method");
                            })
                        .NeutralText("Cancel")
                        .NeutralColor(Resource.Color.black)
                        .OnNeutral((dialog, which) => mCancelPaymentDialog.Dismiss()).Show();
                }
                else
                {
                    this.FragmentManager.PopBackStack();
                }
            }
        }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void SummaryDashBaordUpdate(SummaryDashBordRequest summaryDashBoardRequest)
        {
            try {
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
            //SelectAccountsActivity.selectAccountsActivity.Finish();
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

    }
}