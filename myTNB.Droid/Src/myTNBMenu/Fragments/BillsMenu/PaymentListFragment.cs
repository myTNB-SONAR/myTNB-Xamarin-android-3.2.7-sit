using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Fragments;
using CheeseBind;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Adapter.BillsMenu;
using myTNB_Android.Src.ViewBill.Activity;
using myTNB_Android.Src.ViewReceipt.Activity;

namespace myTNB_Android.Src.myTNBMenu.Fragments.BillsMenu
{
    public class PaymentListFragment : BaseFragment
    {

        [BindView(Resource.Id.list_view)]
        ListView listView;

        [BindView(Resource.Id.emptyLayout)]
        LinearLayout emptyLayout;

        PaymentListAdapter paymentListAdapter;
        REPaymentListAdapter rePaymentListAdapter;

        private PaymentHistoryResponseV5 responseData;

        private PaymentHistoryREResponse responseDataRE;

        AccountData selectedAccount;

        internal static PaymentListFragment NewInstance(PaymentHistoryResponseV5 response, AccountData selectedAccount)
        {
            PaymentListFragment fragment = new PaymentListFragment();
            Bundle args = new Bundle();
            args.PutString(Constants.PAYMENT_RESPONSE, JsonConvert.SerializeObject(response));
            args.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            fragment.Arguments = args;
            return fragment;
        }

        internal static PaymentListFragment NewInstance(PaymentHistoryREResponse response, AccountData selectedAccount)
        {
            PaymentListFragment fragment = new PaymentListFragment();
            Bundle args = new Bundle();
            args.PutString(Constants.PAYMENT_RESPONSE, JsonConvert.SerializeObject(response));
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
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                if (selectedAccount.AccountCategoryId.Equals("2"))
                {
                    responseDataRE = JsonConvert.DeserializeObject<PaymentHistoryREResponse>(Arguments.GetString(Constants.PAYMENT_RESPONSE));
                }
                else
                {
                    responseData = JsonConvert.DeserializeObject<PaymentHistoryResponseV5>(Arguments.GetString(Constants.PAYMENT_RESPONSE));
                }
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            paymentListAdapter = new PaymentListAdapter(Activity, true);
            rePaymentListAdapter = new REPaymentListAdapter(Activity, true);
            listView.EmptyView = emptyLayout;
            if (selectedAccount != null)
            {
                if (selectedAccount.AccountCategoryId.Equals("2"))
                {
                    listView.Adapter = rePaymentListAdapter;
                }
                else
                {
                    listView.Adapter = paymentListAdapter;
                }
            }

            if (responseData != null && responseData.Data.PaymentHistory != null && responseData.Data.PaymentHistory.Count > 0)
            {
                paymentListAdapter.AddAll(responseData.Data.PaymentHistory);
            }

            if (responseDataRE != null && responseDataRE.Data.PaymentHistoryRE != null && responseDataRE.Data.PaymentHistoryRE.Count > 0)
            {
                rePaymentListAdapter.AddAll(responseDataRE.Data.PaymentHistoryRE);
            }

            listView.SetNoScroll();

            listView.ItemClick += ListView_ItemClick;

            

            //View footerView = LayoutInflater.From(Activity).Inflate(Resource.Layout.PaymentListFragmentFooterView, null, false);
            //listView.AddFooterView(footerView);

            //TextView txtFooter = footerView.FindViewById<TextView>(Resource.Id.txtFooter);
            //TextView txtFooter1 = footerView.FindViewById<TextView>(Resource.Id.txtFooter1);

            //TextViewUtils.SetMuseoSans300Typeface(txtFooter, txtFooter1);
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
                if (selectedAccount.AccountCategoryId.Equals("2"))
                {

                }
                else
                {
                    PaymentHistoryV5 selectedPayment = paymentListAdapter.GetItemObject(e.Position);
                    if (!String.IsNullOrEmpty(selectedPayment.MechantTransId))
                    {
                        Intent viewReceipt = new Intent(this.Activity, typeof(ViewReceiptMultiAccountNewDesignActivty));
                        viewReceipt.PutExtra("merchantTransId", selectedPayment.MechantTransId);
                        StartActivity(viewReceipt);
                    }
                }
            }

        }

        public override int ResourceId()
        {
            return Resource.Layout.PaymentListFragmentView;
        }
    }
}