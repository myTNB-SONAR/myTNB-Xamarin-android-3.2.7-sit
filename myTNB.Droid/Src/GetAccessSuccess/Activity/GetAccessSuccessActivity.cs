﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Runtime;

namespace myTNB_Android.Src.GetAccessSuccess.Activity
{
    [Activity(Label = "@string/get_access_activity_title"
         , ScreenOrientation = ScreenOrientation.Portrait
         , Theme = "@style/Theme.GetAccessSuccess")]
    public class GetAccessSuccessActivity : BaseAppCompatActivity
    {

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtAccountName)]
        TextView txtAccountName;

        [BindView(Resource.Id.txtAccountNum)]
        TextView txtAccountNum;

        [BindView(Resource.Id.txtAddress)]
        TextView txtAddress;

        [BindView(Resource.Id.btnDashboard)]
        Button btnDashboard;

        AccountData selectedAccount;

        public override int ResourceId()
        {
            return Resource.Layout.GetAccessSuccessView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, txtAccountName, btnDashboard);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountName, txtAddress);

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        //selectedAccount = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
                        selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                }
                txtAccountName.Text = selectedAccount.AccountName;
                txtAccountNum.Text = selectedAccount.AccountNum;
                txtAddress.Text = selectedAccount.AddStreet;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnDashboard)]
        void OnDashboard(object sender, EventArgs eventArgs)
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
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