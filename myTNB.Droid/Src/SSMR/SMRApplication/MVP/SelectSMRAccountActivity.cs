
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Adapter;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Adapter;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "SelectSMRAccountActivity", Theme = "@style/Theme.Dashboard")]
    public class SelectSMRAccountActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.account_list_recycler_view)]
        RecyclerView accountListRecyclerView;

        List<SMRAccount> accountList = new List<SMRAccount>();
        public override int ResourceId()
        {
            return Resource.Layout.SelectSMRAccountLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            List<CustomerBillingAccount> customerBillingAccountList = UserSessions.GetSelectAccountList();
            // Create your application here
            accountList = new List<SMRAccount>();
            foreach (CustomerBillingAccount customerBillingAccount in customerBillingAccountList)
            {
                SMRAccount account = new SMRAccount();
                account.accountName = customerBillingAccount.AccDesc;
                accountList.Add(account);
            }
            accountList[0].accountSelected = true;
            SelectSMRAccountAdapter adapter = new SelectSMRAccountAdapter(this,accountList);
            LinearLayoutManager layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            accountListRecyclerView.SetLayoutManager(layoutManager);
            accountListRecyclerView.AddItemDecoration(new DividerItemDecoration(accountListRecyclerView.Context, DividerItemDecoration.Vertical));
            accountListRecyclerView.SetAdapter(adapter);
        }
    }
}
