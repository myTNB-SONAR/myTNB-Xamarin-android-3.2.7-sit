using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MakePayment.Fragment;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Runtime;

namespace myTNB_Android.Src.MakePayment.Activity
{
    [Activity(Label = "Select Payment Method"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.MakePayment")]
    public class MakePaymentActivity : BaseToolbarAppCompatActivity
    {
        AccountData selectedAccount;
        AndroidX.Fragment.App.Fragment  currentFragment;

        private MaterialDialog mCancelPaymentDialog;

        private Toolbar toolbar;

        public static bool paymentReceiptGenerated = false;

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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        //selectedAccount = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
                        selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                }
                OnLoadMainFragment();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //AndroidX.Fragment.App.Fragment  selectPaymentFragment = new SelectPaymentMethodFragment();
            //Bundle bundle = new Bundle();
            //bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            //selectPaymentFragment.Arguments = bundle;
            //var fragmentTransaction = FragmentManager.BeginTransaction();
            //fragmentTransaction.Add(Resource.Id.fragment_container, selectPaymentFragment);
            //fragmentTransaction.Commit();
            //currentFragment = selectPaymentFragment;
        }

        public void nextFragment(AndroidX.Fragment.App.Fragment  fragment, Bundle bundle)
        {
            if (fragment is SelectPaymentMethodFragment)
            {
                var paymentWebViewFragment = new PaymentWebViewFragment();
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
            AndroidX.Fragment.App.Fragment  selectPaymentFragment = new SelectPaymentMethodFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            selectPaymentFragment.Arguments = bundle;
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, selectPaymentFragment);
            fragmentTransaction.Commit();
            currentFragment = selectPaymentFragment;
        }

        internal static void SetPaymentReceiptFlag(bool flag)
        {
            paymentReceiptGenerated = flag;
        }

        public void ClearBackStack()
        {
            FragmentManager manager = this.FragmentManager;
            if (manager.BackStackEntryCount > 0)
            {
                manager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, FragmentManager.PopBackStackInclusive);
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

        public override void OnBackPressed()
        {
            try
            {
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
                    if (currentFragment is PaymentWebViewFragment)
                    {
                        mCancelPaymentDialog = new MaterialDialog.Builder(this)
                            .Title(Utility.GetLocalizedLabel("MakePayment", "abortTitle"))
                            .Content(Utility.GetLocalizedLabel("MakePayment", "abortMessage"))
                            .Cancelable(false)
                            .PositiveText(Utility.GetLocalizedCommonLabel("yes"))
                            .PositiveColor(Resource.Color.black)
                            .OnPositive((dialog, which) => this.FragmentManager.PopBackStack())
                            .NeutralText(Utility.GetLocalizedCommonLabel("no"))
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