using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Fragments;
using CheeseBind;
using myTNB_Android.Src.myTNBMenu.Adapter.BillsMenu;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using myTNB_Android.Src.ViewBill.Activity;
using Android.Text;

namespace myTNB_Android.Src.myTNBMenu.Fragments.BillsMenu
{
    public class BillingListFragment : BaseFragment
    {
        [BindView(Resource.Id.list_view)]
        ListView listView;

        BillsListAdapter billsListAdapter;
        AccountData selectedAccount;

        [BindView(Resource.Id.emptyLayout)]
        LinearLayout emptyLayout;

        private BillHistoryResponseV5 responseData;
        
        internal static BillingListFragment NewInstance(BillHistoryResponseV5 response , AccountData selectedAccount)
        {
            BillingListFragment fragment = new BillingListFragment();
            Bundle args = new Bundle();
            args.PutString(Constants.BILL_RESPONSE , JsonConvert.SerializeObject(response));
            args.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Arguments;
            if (extras != null)
            {
                responseData = JsonConvert.DeserializeObject<BillHistoryResponseV5>(extras.GetString(Constants.BILL_RESPONSE));
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
            }
          
        }


        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            if (selectedAccount != null)
            {
                billsListAdapter = new BillsListAdapter(Activity, true, selectedAccount);
            }
            else
            {
                billsListAdapter = new BillsListAdapter(Activity, true);
            }
           
            listView.EmptyView = emptyLayout;
            listView.Adapter = billsListAdapter;


            if (responseData != null && responseData.Data.BillHistory != null && responseData.Data.BillHistory.Count > 0)
            {
                billsListAdapter.AddAll(responseData.Data.BillHistory);

            }
            listView.SetNoScroll();


            listView.ItemClick += ListView_ItemClick;

            
        }
        [Preserve]
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            listView.Enabled = false;
            Handler h = new Handler();
            Action myAction = () =>
            {
                listView.Enabled = true;
            };
            h.PostDelayed(myAction, 2000);

            if (selectedAccount != null)
            {
                ///<summary>
                /// Revert non owner CR changes
                ///</summary>
                //if (selectedAccount.IsOwner)
                //{
                BillHistoryV5 selectedBill = billsListAdapter.GetItemObject(e.Position);
                    Intent viewBill = new Intent(this.Activity, typeof(ViewBillActivity));
                    viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                    viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
                    StartActivity(viewBill);
                //}
            }

        }

        public override int ResourceId()
        {
            return Resource.Layout.BillingListFragmentView;
        }
    }
}