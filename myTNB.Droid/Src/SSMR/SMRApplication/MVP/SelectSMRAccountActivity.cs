
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
    }
}
