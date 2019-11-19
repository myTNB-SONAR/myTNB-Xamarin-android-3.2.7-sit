using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.RearrangeAccount.MVP
{
    [Activity(Label = "Rearrange Accounts"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Dashboard")]
    public class RearrangeAccountActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        private RearrangeAccountListView listView;

        List<CustomerBillingAccount> items = new List<CustomerBillingAccount>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);

                listView = FindViewById<RearrangeAccountListView>(Resource.Id.list_view);

                items = AccountSortingEntity.List(UserEntity.GetActive().Email);

                listView.Adapter = new RearrangeAccountListAdapter(this, items);

                DisableSaveButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.AccountRearrangeLayout;
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

        public void EnableSaveButton()
        {
            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void DisableSaveButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }
    }
}