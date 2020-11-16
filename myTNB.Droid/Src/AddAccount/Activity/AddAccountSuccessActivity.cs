using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.AppBar;
using myTNB_Android.Src.AddAccount.Adapter;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace myTNB_Android.Src.AddAccount.Activity
{
    [Activity(Label = "Success",
        ScreenOrientation = ScreenOrientation.Portrait,
        Theme = "@style/Theme.LinkAccount")]
    public class AddAccountSuccessActivity : BaseToolbarAppCompatActivity
    {

        AddedAccountsAdapter adapter;
        List<NewAccount> accountList = new List<NewAccount>();
        RecyclerView.LayoutManager layoutManager;
        private AppBarLayout appBarLayout;

        [BindView(Resource.Id.account_list_recycler_view)]
        RecyclerView accountListRecyclerView;

        [BindView(Resource.Id.txtTitleInfo)]
        TextView textAddAccountSuccess;

        [BindView(Resource.Id.txtVerifyNotification)]
        TextView txtAddAccVerifyNotification;

        public override int ResourceId()
        {
            return Resource.Layout.AddAccountsSuccess;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            accountListRecyclerView.SetLayoutManager(layoutManager);
            accountListRecyclerView.SetAdapter(adapter);

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("Accounts"))
                {
                    accountList = DeSerialze<List<NewAccount>>(extras.GetString("Accounts"));
                }
            }
            if (accountList != null && accountList.Count() > 0)
            {
                adapter = new AddedAccountsAdapter(this, accountList);
                accountListRecyclerView.SetAdapter(adapter);
                adapter.NotifyDataSetChanged();
            }


            Button done = FindViewById<Button>(Resource.Id.btnGetStarted);
            done.Click += delegate
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    GetStarted();
                }
            };

            TextViewUtils.SetMuseoSans500Typeface(textAddAccountSuccess);
            TextViewUtils.SetMuseoSans300Typeface(txtAddAccVerifyNotification);
            TextViewUtils.SetMuseoSans500Typeface(done);

            textAddAccountSuccess.Text = Utility.GetLocalizedLabel("AddAccount", "addAcctSuccessMsg");
            txtAddAccVerifyNotification.Text = Utility.GetLocalizedLabel("AddAccount", "addAcctSuccessMsgBody");
            done.Text = Utility.GetLocalizedCommonLabel("done");

            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBar);
            appBarLayout.Visibility = ViewStates.Gone;
        }

        public override void OnBackPressed()
        {
            GetStarted();
        }

        public void GetStarted()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Add Account Success");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
           base.OnPause(); 
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