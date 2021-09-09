using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base;
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

        [BindView(Resource.Id.infoLabelAccNotListed)]
        TextView infoLabelAccNotListed;

        [BindView(Resource.Id.txttitleEBList)]
        TextView txttitleEBList;

        EnergyBudgetContract.IUserActionsListener userActionsListener;
        EnergyBudgetPresenter mPresenter;

        MaterialDialog accountRetrieverDialog;

        SmartMeterListAdapter adapter;

        List<SMRAccount> listSmartMeter;

        SMRAccount accountData;

        private ISharedPreferences mSharedPref;

        const string PAGE_ID = "EnregyBudgetListing";

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
               
                adapter = new SmartMeterListAdapter(this, true);
                listView.Adapter = adapter;
                listView.SetNoScroll();
                listView.ItemClick += ListView_ItemClick;

                mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);

                SetToolBarTitle(Utility.GetLocalizedLabel("EnregyBudgetListing", "title"));

                TextViewUtils.SetMuseoSans500Typeface(infoLabelAccNotListed);
                TextViewUtils.SetMuseoSans300Typeface(txttitleEBList);
                TextViewUtils.SetTextSize14(txttitleEBList);
                TextViewUtils.SetTextSize13(infoLabelAccNotListed);

                txttitleEBList.Text = Utility.GetLocalizedLabel("EnregyBudgetListing", "headerTitle");
                infoLabelAccNotListed.Text = Utility.GetLocalizedLabel("EnregyBudgetListing", "tootltipTitle");

                listSmartMeter = UserSessions.GetEnergyBudgetList();

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
                accountData = adapter.GetItemObject(e.Position);
                List<SMRAccount> SMeterAccountList = new List<SMRAccount>();
                foreach (var item in listSmartMeter)
                {
                    if (item.accountNumber.Equals(accountData.accountNumber))
                    {
                        item.accountSelected = true;
                    }
                    SMeterAccountList.Add(item);
                }

                UserSessions.DeleteEnergyBudgetList(mSharedPref);
                UserSessions.EnergyBudget(SMeterAccountList);
                adapter.Clear();
                this.userActionsListener.Start();
                selectedsmaccount();
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

        [OnClick(Resource.Id.infoLabelContainerAccNotListed)]
        internal void OnUserClickRM(object sender, EventArgs e)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    showAccNotListedTooltip();
                }
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }

        }

        private void showAccNotListedTooltip()
        {
            MyTNBAppToolTipBuilder eppTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle(Utility.GetLocalizedLabel("EnregyBudgetListing", "tootltipTitle"))
                    .SetMessage(Utility.GetLocalizedLabel("EnregyBudgetListing", "tooltipBody"))
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                    .SetCTAaction(() => { this.SetIsClicked(false); })
                    .Build();
            eppTooltip.Show();
        }

        public void selectedsmaccount()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            SetResult(Result.Ok, result);
            Finish();
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
