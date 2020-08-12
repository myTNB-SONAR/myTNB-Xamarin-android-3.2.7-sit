
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB_Android.Src.myTNBMenu.Models;
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
            if (extras != null)
            {
                //selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                {
                    selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                }
            }
            // Create your application here

            //FragmentManager.BeginTransaction()
            //    .Replace(Resource.Id.content_layout, ItemisedBillingMenuFragment.NewInstance(selectedAccount))
            //    .CommitAllowingStateLoss();

            SupportFragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, DashboardChartFragment.NewInstance(null, selectedAccount, "", ""),
                                    typeof(DashboardChartFragment).Name)
                           .CommitAllowingStateLoss();
        }
    }
}