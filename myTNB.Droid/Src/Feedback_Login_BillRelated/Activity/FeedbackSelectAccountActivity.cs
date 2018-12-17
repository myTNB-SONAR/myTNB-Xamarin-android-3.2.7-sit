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
using myTNB_Android.Src.Base.Activity;
using CheeseBind;
using Android.Content.PM;
using myTNB_Android.Src.Feedback_Login_BillRelated.Adapter;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using myTNB_Android.Src.Database.Model;
using System.Runtime;

namespace myTNB_Android.Src.Feedback_Login_BillRelated.Activity
{
    [Activity(Label = "@string/bill_related_select_account_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.Dashboard")]
    public class FeedbackSelectAccountActivity : BaseToolbarAppCompatActivity
    {

        [BindView(Resource.Id.list_view)]
        ListView listView;

        FeedbackSelectAccountAdapter feedbackSelectAdapter;

        AccountData previouslySelected;

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackSelectAccountView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            previouslySelected = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));

            feedbackSelectAdapter = new FeedbackSelectAccountAdapter(this);
            listView.Adapter = feedbackSelectAdapter;

            if (CustomerBillingAccount.HasItems())
            {
                List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
                foreach(CustomerBillingAccount customer in accountList)
                {
                    if (previouslySelected.AccountNum.Equals(customer.AccNum))
                    {
                        feedbackSelectAdapter.Add(AccountData.Copy(customer , true));
                    }
                    else
                    {
                        feedbackSelectAdapter.Add(AccountData.Copy(customer, false));
                    }
                    
                }
            }
            listView.ItemClick += OnItemClick;

        }

        [Preserve]
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AccountData accountData = feedbackSelectAdapter.GetItemObject(e.Position);
            Intent resultIntent = new Intent();
            resultIntent.PutExtra(Constants.SELECTED_ACCOUNT , JsonConvert.SerializeObject(accountData));
            SetResult(Result.Ok, resultIntent);
            Finish();
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}