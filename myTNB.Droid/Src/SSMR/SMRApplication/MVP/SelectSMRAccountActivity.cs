
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AddAccount.Adapter;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
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
    public class SelectSMRAccountActivity : BaseToolbarAppCompatActivity, SelectSMRAccountContract.IView
    {
        [BindView(Resource.Id.account_list_view)]
        ListView accountSMRList;

        [BindView(Resource.Id.whyAccountsNotHere)]
        TextView whyAccountsNotHere; 

        [BindView(Resource.Id.noEligibleAccountContainer)]
        LinearLayout noEligibleAccountContainer; 

        [BindView(Resource.Id.eligibleAccountListContainer)]
        LinearLayout eligibleAccountListContainer;

        List<SMRAccount> accountList = new List<SMRAccount>();
        private SelectAccountAdapter selectAccountAdapter;
        public override int ResourceId()
        {
            return Resource.Layout.SelectSMRAccountLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void UpdateSelectedAccount(int position)
        {
            for (int i=0; i < accountList.Count; i++)
            {
                if (i==position)
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
            TextViewUtils.SetMuseoSans500Typeface(whyAccountsNotHere);
            Bundle extras = Intent.Extras;
            if (extras.ContainsKey("SMR_ELIGIBLE_ACCOUNT_LIST"))
            {
                accountList = JsonConvert.DeserializeObject<List<SMRAccount>>(extras.GetString("SMR_ELIGIBLE_ACCOUNT_LIST"));
            }
            if (accountList.Count > 0)
            {
                if (CustomerBillingAccount.HasSelected())
                {
                    CustomerBillingAccount selectedAccount =  CustomerBillingAccount.GetSelected();
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
                selectAccountAdapter = new SelectAccountAdapter(this, accountList);
                accountSMRList.Adapter = selectAccountAdapter;

                accountSMRList.ItemClick += OnItemClick;
            }
            else
            {
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

        [OnClick(Resource.Id.smrWhyTheseAccountsInfo)]
        internal void OnWhyTheseAccountsTap(object sender, EventArgs eventArgs)
        {
            MyTNBAppToolTipData.SMREligibiltyPopUpDetailData tooltipData = MyTNBAppToolTipData.GetInstance().GetSMREligibiltyPopUpDetails();

            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(tooltipData.title)
                .SetMessage(tooltipData.description)
                .SetCTALabel(tooltipData.cta)
                .Build().Show();
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Select SMR Electricity Account Screen");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
