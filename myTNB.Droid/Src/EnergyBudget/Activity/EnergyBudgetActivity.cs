using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.EnergyBudget.Adapter;
using myTNB_Android.Src.EnergyBudget.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.EnergyBudget.Activity
{
    [Activity(Label = "@string/my_account_activity_title"
        //, Icon = "@drawable/Logo"
        //, MainLauncher = true
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.MyAccount")]
    public class EnergyBudgetActivity : BaseActivityCustom, EnergyBudgetContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.listView)]
        ListView listView;

        EnergyBudgetContract.IUserActionsListener userActionsListener;
        EnergyBudgetPresenter mPresenter;

        MaterialDialog accountRetrieverDialog;

        SmartMeterListAdapter adapter;

        const string PAGE_ID = "Smart Meter";

        public override int ResourceId()
        {
            return Resource.Layout.MySmartMeterView;
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
                // Create your application here
               
                adapter = new SmartMeterListAdapter(this, false);
                listView.Adapter = adapter;
                listView.SetNoScroll();
                listView.ItemClick += ListView_ItemClick;

                SetToolBarTitle(Utility.GetLocalizedLabel("AddUserAccess", "title"));

                mPresenter = new EnergyBudgetPresenter(this);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                /*CustomerBillingAccount customerBillingAccount = adapter.GetItemObject(e.Position);
                ShowManageSupplyAccount(AccountData.Copy(customerBillingAccount, false), e.Position);*/
            }
        }

        public void ShowAccountList(List<SMRAccount> accountList)
        {
            try
            {
                adapter.AddAll(accountList);
                adapter.NotifyDataSetChanged();
                listView.SetNoScroll();
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

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "More -> My Account");
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

        public void HideShowProgressDialog()
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

        public void SetPresenter(EnergyBudgetContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void ShowAccountList(List<CustomerBillingAccount> accountList)
        {
            try
            {
                /*adapter.AddAll(accountList);
                adapter.NotifyDataSetChanged();
                listView.SetNoScroll();
                btnAddAnotherAccount.Visibility = ViewStates.Visible;*/
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        
        public void ClearAccountsAdapter()
        {
            //adapter.Clear();
        }
       
        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
