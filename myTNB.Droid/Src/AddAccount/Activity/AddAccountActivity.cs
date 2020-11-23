using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using myTNB_Android.Src.AddAccount.Fragment;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Runtime;
using ZXing.Mobile;

namespace myTNB_Android.Src.AddAccount.Activity
{
    [Activity(Label = "Add Electricity Account"
        , ScreenOrientation = ScreenOrientation.Portrait
        , WindowSoftInputMode = SoftInput.StateHidden
        , Theme = "@style/Theme.AddAccount")]
    public class AddAccountActivity : BaseToolbarAppCompatActivity
    {
        public override int ResourceId()
        {
            return Resource.Layout.AddAccountView;
        }

        AndroidX.Fragment.App.FragmentTransaction fragmentTransaction;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AndroidX.Fragment.App.Fragment  addAccountTypeFragment = new AddAccountTypeFragment();
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, addAccountTypeFragment);
            fragmentTransaction.Commit();

           
            SetTheme(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.Theme_AddAccountLarge : Resource.Style.Theme_AddAccount);
            //Initialize scanner
            MobileBarcodeScanner.Initialize(Application);
        }
      

        public void nextFragment(AndroidX.Fragment.App.Fragment  fragment, Bundle bundle)
        {
            if (fragment is AddAccountTypeFragment)
            {
                bool isOwner = bundle.GetBoolean("isOwner");
                if (isOwner)
                {
                    var addAccountForm = new AddAccountFormFragment();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                    fragmentTransaction.AddToBackStack(null);
                    fragmentTransaction.Commit();
                }
                else
                {
                    var addAccountForm = new AddAccountByRightsFragment();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                    fragmentTransaction.AddToBackStack(null);
                    fragmentTransaction.Commit();
                }
            }
            else if (fragment is AddAccountByRightsFragment)
            {
                var addAccountForm = new AddAccountFormFragment();
                addAccountForm.Arguments = bundle;
                var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                fragmentTransaction.AddToBackStack(null);
                fragmentTransaction.Commit();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnBackPressed()
        {
            int count = this.SupportFragmentManager.BackStackEntryCount;
            Log.Debug("OnBackPressed", "fragment stack count :" + count);
            if (count == 0)
            {
                Finish();
            }
            else
            {
                this.SupportFragmentManager.PopBackStack();
            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Add Account / Link Account");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return Utility.GetLocalizedLabel("AddAccount","title");
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