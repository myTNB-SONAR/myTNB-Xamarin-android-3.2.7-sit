
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
    public class SelectSMRAccountActivity : BaseToolbarAppCompatActivity, SelectSMRAccountContract.IView
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
            UserSessions.SetSMRAccountList(accountList);
            Intent returnIntent = new Intent();
            SetResult(Result.Canceled, returnIntent);
            Finish();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            accountList = new List<SMRAccount>();
            foreach (SMRAccount currentSMRAccount in UserSessions.GetSMRAccountList())
            {
                SMRAccount account = new SMRAccount();
                account.accountName = currentSMRAccount.accountName;
                account.accountNumber = currentSMRAccount.accountNumber;
                account.accountSelected = currentSMRAccount.accountSelected;
                account.accountAddress = currentSMRAccount.accountAddress;
                account.email = currentSMRAccount.email;
                account.mobileNumber = currentSMRAccount.mobileNumber;
                accountList.Add(account);
            }
            SelectSMRAccountAdapter adapter = new SelectSMRAccountAdapter(this,accountList);
            LinearLayoutManager layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            accountListRecyclerView.SetLayoutManager(layoutManager);
            accountListRecyclerView.AddItemDecoration(new DividerItemDecoration(accountListRecyclerView.Context, DividerItemDecoration.Vertical));
            accountListRecyclerView.SetAdapter(adapter);
        }

        public class OnSelectAccountListener : Java.Lang.Object, RecyclerView.IOnClickListener
        {
            public void OnClick(View v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
