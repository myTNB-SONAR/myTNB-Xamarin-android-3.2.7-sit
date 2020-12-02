using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using myTNB_Android.Src.AddAccount.Fragment;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Feedback_Prelogin_NewIC.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.UpdateID.Activity;
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

        private bool fromRegisterPage;

        public override int ResourceId()
        {
            return Resource.Layout.AddAccountView;
        }

        AndroidX.Fragment.App.FragmentTransaction fragmentTransaction;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Intent.HasExtra("fromRegisterPage"))
            {
                fromRegisterPage = Intent.Extras.GetBoolean("fromRegisterPage", true);
            }

            Bundle bundle = new Bundle();
            bundle.PutBoolean("fromRegisterPage", fromRegisterPage);
            //bundle.PutBoolean("hasRights", false);


            AndroidX.Fragment.App.Fragment  addAccountTypeFragment = new AddAccountTypeFragmentNew();
            addAccountTypeFragment.Arguments = bundle;
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, addAccountTypeFragment);
            fragmentTransaction.Commit();

            //Initialize scanner
            MobileBarcodeScanner.Initialize(Application);
        }

        public void nextFragment(AndroidX.Fragment.App.Fragment  fragment, Bundle bundle)
        {
            if (fragment is AddAccountTypeFragmentNew)
            {
                bool isOwner = bundle.GetBoolean("isOwner");
                bool fromRegister = bundle.GetBoolean("fromRegister");
                bool isCommercial = bundle.GetBoolean("isCommercial");
                if (isOwner)
                {
                    var addAccountForm = new AddAccountTypeFragmentOwner();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                    fragmentTransaction.AddToBackStack(null);
                    fragmentTransaction.Commit();

                    /*var addAccountForm = new AddAccountFormFragment();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                    fragmentTransaction.AddToBackStack(null);
                    fragmentTransaction.Commit();*/
                }
                else if (fromRegister)
                {
                    Intent nextIntent = new Intent(this, typeof(DashboardHomeActivity));
                    StartActivity(nextIntent);
                }
                else if (isCommercial)
                {
                    //var addAccountForm = new AddAccountTypeFragmentBusiness();
                    var addAccountForm = new AddAccountFormFragmentBusiness();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
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
                //var addAccountForm = new AddAccountFormFragment();
                var addAccountForm = new AddAccountFormFragmentNonOwner();
                addAccountForm.Arguments = bundle;
                var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                fragmentTransaction.AddToBackStack(null);
                fragmentTransaction.Commit();
            }
            else if (fragment is AddAccountFormFragmentBusiness)
            {
                bool isResidential = bundle.GetBoolean("isResidential");
                bool fromRegister = bundle.GetBoolean("fromRegister");
                if (isResidential)
                {
                    var addAccountForm = new AddAccountTypeFragmentNew();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                    fragmentTransaction.Commit();
                }
                else if (fromRegister)
                {
                    Intent nextIntent = new Intent(this, typeof(DashboardHomeActivity));
                    StartActivity(nextIntent);
                }
            }
            else if (fragment is AddAccountTypeFragmentOwner)
            {
                bool isUpdateId = bundle.GetBoolean("isUpdateId");
                if (isUpdateId)
                {
                    var addAccountForm = new AddAccountFormFragmentNonOwner();
                    addAccountForm.Arguments = bundle;
                    var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                    fragmentTransaction.Add(Resource.Id.fragment_container, addAccountForm);
                    fragmentTransaction.AddToBackStack(null);
                    fragmentTransaction.Commit();
                }
                else
                {
                    Intent nextIntent = new Intent(this, typeof(FeedbackPreloginNewICActivity));
                    StartActivity(nextIntent);
                }
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