
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB_Android.Src.AddAccount.Adapter;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Adapter;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Adapter;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "Select Electricity Account"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.Dashboard")]
    public class SelectDBRAccountActivity : BaseActivityCustom, SelectSMRAccountContract.IView
    {
        List<SMRAccount> accountList = new List<SMRAccount>();
        private SelectAccountAdapter selectAccountAdapter;
        const string PAGE_ID = "SelectElectricityAccounts";

        [BindView(Resource.Id.account_list_view)]
        ListView accountSMRList;

        [BindView(Resource.Id.noEligibleAccountContainer)]
        LinearLayout noEligibleAccountContainer;

        [BindView(Resource.Id.eligibleAccountListContainer)]
        LinearLayout eligibleAccountListContainer;

        [BindView(Resource.Id.noEligibleAccountMessage)]
        TextView noEligibleAccountMessage;


        public override int ResourceId()
        {
            return Resource.Layout.SelectDBRAccountLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void UpdateSelectedAccount(int position)
        {
            for (int i = 0; i < accountList.Count; i++)
            {
                if (i == position)
                {
                    accountList[i].accountSelected = true;
                }
                else
                {
                    accountList[i].accountSelected = false;
                }
            }
            UserSessions.SetRealSMREligibilityAccountList(accountList);
            Intent returnIntent = new Intent();
            returnIntent.PutExtra("SELECTED_ACCOUNT_NUMBER", accountList.Find(x => { return x.accountSelected; }).accountNumber);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            TextViewUtils.SetTextSize14(noEligibleAccountMessage);
            Bundle extras = Intent.Extras;
            if (extras != null && extras.ContainsKey("SMR_ELIGIBLE_ACCOUNT_LIST"))
            {
                try
                {
                    accountList = JsonConvert.DeserializeObject<List<SMRAccount>>(extras.GetString("SMR_ELIGIBLE_ACCOUNT_LIST"));
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            if (accountList != null && accountList.Count > 0)
            {
                if (CustomerBillingAccount.HasSelected())
                {
                    CustomerBillingAccount selectedAccount = CustomerBillingAccount.GetSelected();
                    int index = accountList.FindIndex(x => x.accountNumber == selectedAccount.AccNum);
                    if (index != -1)
                    {
                        accountList[index].accountSelected = true;
                    }
                    else
                    {
                        accountList[0].accountSelected = true;
                    }
                }
                else
                {
                    accountList[0].accountSelected = true;
                }
                noEligibleAccountContainer.Visibility = ViewStates.Gone;
                eligibleAccountListContainer.Visibility = ViewStates.Visible;
                List<SMRAccount> newItemList = accountList.GetRange(0, accountList.Count);
                newItemList.Add(new SMRAccount()); //To show info item
                selectAccountAdapter = new SelectAccountAdapter(this, newItemList);
                accountSMRList.Adapter = selectAccountAdapter;

                accountSMRList.ItemClick += OnItemClick;
            }
            else
            {
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                noEligibleAccountContainer.Visibility = ViewStates.Visible;
                eligibleAccountListContainer.Visibility = ViewStates.Gone;
            }
        }


        public class OnSelectAccountListener : Java.Lang.Object, RecyclerView.IOnClickListener
        {
            public void OnClick(View v)
            {
                throw new NotImplementedException();
            }
        }

        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position == accountList.Count)//Handling Account list Info tooltip from list
            {
                MyTNBAppToolTipBuilder dbrTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                 .SetTitle(Utility.GetLocalizedLabel("SelectElectricityAccounts", "accountsMissing"))
                   .SetMessage(Utility.GetLocalizedLabel("SelectElectricityAccounts", "dbrMissingAccountsMessage"))
                  .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                  .Build();

                dbrTooltip.Show();
            }
            else
            {
                for (int i = 0; i < accountList.Count; i++)
                {
                    if (i == e.Position)
                    {
                        accountList[i].accountSelected = true;
                    }
                    else
                    {
                        accountList[i].accountSelected = false;
                    }
                }
                Intent returnIntent = new Intent();
                returnIntent.PutExtra("SELECTED_ACCOUNT_NUMBER", accountList.Find(x => { return x.accountSelected; }).accountNumber);
                SetResult(Result.Ok, returnIntent);
                Finish();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Select SMR Electricity Account");
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
