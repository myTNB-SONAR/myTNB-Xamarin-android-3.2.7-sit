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
using Android.Content.PM;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.BillsMenu;
using Newtonsoft.Json;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.NotificationNewBill.Activity
{
    [Activity(Label = "@string/notification_detail_view_bill_view_details_activity_title"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationNewBillViewDetailsActivity : BaseToolbarAppCompatActivity
    {
        private AccountData selectedAccount;
        public override int ResourceId()
        {
            return Resource.Layout.NotificationNewBillViewDetailsView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Intent.Extras;
            if (extras != null) {
                //selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT)) {
                    selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                }
            }
            // Create your application here

            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_layout, BillsMenuFragment.NewInstance(selectedAccount))
                .CommitAllowingStateLoss();
        }
    }
}