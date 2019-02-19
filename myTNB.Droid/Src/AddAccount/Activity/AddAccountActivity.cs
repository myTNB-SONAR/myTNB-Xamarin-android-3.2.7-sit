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
using myTNB_Android.Src.AddAccount.Fragment;
using Android.Util;
using ZXing.Mobile;
using System.Runtime;

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

        FragmentTransaction fragmentTransaction;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            Android.App.Fragment addAccountTypeFragment = new AddAccountTypeFragment();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, addAccountTypeFragment);
            fragmentTransaction.Commit();

            //Initialize scanner
            MobileBarcodeScanner.Initialize(Application);
        }

        public void nextFragment(Android.App.Fragment fragment, Bundle bundle)
        {
            if (fragment is AddAccountTypeFragment)
            {
                bool isOwner = bundle.GetBoolean("isOwner");
                if (isOwner)
                {
                    var addAccountForm = new AddAccountFormFragment();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = FragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                    fragmentTransaction.AddToBackStack(null);
                    fragmentTransaction.Commit();
                }
                else
                {
                    var addAccountForm = new AddAccountByRightsFragment();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = FragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                    fragmentTransaction.AddToBackStack(null);
                    fragmentTransaction.Commit();
                }
            }
            else if(fragment is AddAccountByRightsFragment)
            {
                var addAccountForm = new AddAccountFormFragment();
                addAccountForm.Arguments = bundle;
                var fragmentTransaction = FragmentManager.BeginTransaction();
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
            int count = this.FragmentManager.BackStackEntryCount;
            Log.Debug("OnBackPressed", "fragment stack count :"+count);
            if (count == 0)
            {
                Finish();
            }
            else
            {
                this.FragmentManager.PopBackStack();
            }

        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return GetString(Resource.String.add_electricity_account_title);
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