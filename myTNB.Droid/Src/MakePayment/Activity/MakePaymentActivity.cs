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
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using myTNB_Android.Src.MakePayment.Fragment;
using Android.Util;
using myTNB_Android.Src.MakePayment.MVP;
using myTNB_Android.Src.MakePayment.Model;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using AFollestad.MaterialDialogs;

namespace myTNB_Android.Src.MakePayment.Activity
{
    [Activity(Label = "Select Payment Method"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.MakePayment")]
    public class MakePaymentActivity : BaseToolbarAppCompatActivity
    {
        AccountData selectedAccount;
        Android.App.Fragment currentFragment;

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

        public override void SetToolBarTitle(string title){
            base.SetToolBarTitle(title);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        { 
            base.OnCreate(savedInstanceState);
            // Create your application here

            selectedAccount = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));

            OnLoadMainFragment();
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
            if (fragment is SelectPaymentMethodFragment){
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
            Android.App.Fragment selectPaymentFragment = new SelectPaymentMethodFragment();
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

        public override void OnBackPressed()
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
                        .Title("Abort Payment!")
                        .Content(GetString(Resource.String.error_abort_payment))
                        .Cancelable(false)
                        .PositiveText("Abort")
                        .PositiveColor(Resource.Color.black)
                        .OnPositive((dialog, which) => this.FragmentManager.PopBackStack())
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

    }
}