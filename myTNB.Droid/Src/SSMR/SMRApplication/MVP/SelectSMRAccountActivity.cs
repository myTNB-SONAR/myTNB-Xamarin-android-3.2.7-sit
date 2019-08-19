
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
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Adapter;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Adapter;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "Select Account"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.Dashboard")]
    public class SelectSMRAccountActivity : BaseToolbarAppCompatActivity, SelectSMRAccountContract.IView
    {
        [BindView(Resource.Id.account_list_view)]
        ListView accountSMRList;

        [BindView(Resource.Id.whyAccountsNotHere)]
        TextView whyAccountsNotHere;

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
            SetResult(Result.Canceled, returnIntent);
            Finish();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            TextViewUtils.SetMuseoSans500Typeface(whyAccountsNotHere);
            accountList = new List<SMRAccount>();
            List<SMRAccount> list = UserSessions.GetRealSMREligibilityAccountList();
            List<CustomerBillingAccount> eligibleAccountList = CustomerBillingAccount.GetEligibleAndSMRAccountList();
            if (list == null)
            {
                list = UserSessions.GetSMREligibilityAccountList();
            }
            foreach (CustomerBillingAccount custBillingAccount in eligibleAccountList)
            {
                SMRAccount account = new SMRAccount();
                account.accountName = custBillingAccount.AccDesc;
                account.accountNumber = custBillingAccount.AccNum;
                account.accountSelected = custBillingAccount.IsSelected;
                account.isTaggedSMR = custBillingAccount.IsTaggedSMR;
                accountList.Add(account);
            }

            selectAccountAdapter = new SelectAccountAdapter(this, accountList);
            accountSMRList.Adapter = selectAccountAdapter;

            accountSMRList.ItemClick += OnItemClick;
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
            //SMRAccount smrAccount = selectAccountAdapter.GetItemObject(e.Position);
            //smrAccount.accountSelected = true;
            //Intent link_activity = new Intent(this, typeof(AddAccountActivity));
            //link_activity.PutExtra("selectedAccountType", JsonConvert.SerializeObject(selectedAccountType));
            //SetResult(Result.Ok, link_activity);
            //Finish();

            UpdateSelectedAccount(e.Position);
        }

        [OnClick(Resource.Id.smrWhyTheseAccountsInfo)]
        internal void OnWhyTheseAccountsTap(object sender, EventArgs eventArgs)
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(GetString(Resource.String.ssmr_reading_history_tooltip_title))
                .SetMessage(GetString(Resource.String.ssmr_readint_history_tooltip_message))
                .SetCTALabel("Got It!")
                .Build().Show();
        }
    }
}
