﻿using System;
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
using Android.Support.V7.Widget;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.myTNBMenu.Activity;
using Android.Content.PM;
using Newtonsoft.Json;
using myTNB_Android.Src.AddAccount.Adapter;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.AddAccount.Activity
{
    [Activity(Label = "Success",  
        ScreenOrientation = ScreenOrientation.Portrait, 
        Theme = "@style/Theme.LinkAccount")]
    public class AddAccountSuccessActivity : BaseToolbarAppCompatActivity
    {

        AddedAccountsAdapter adapter;
        List<NewAccount> accountList = new List<NewAccount>();
        RecyclerView.LayoutManager layoutManager;
        private Android.Support.Design.Widget.AppBarLayout appBarLayout;

        [BindView(Resource.Id.account_list_recycler_view)]
        RecyclerView accountListRecyclerView;

        [BindView(Resource.Id.txtTitleInfo)]
        TextView textAddAccountSuccess;

        public override int ResourceId()
        {
            return Resource.Layout.AddAccountsSuccess;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            accountListRecyclerView.SetLayoutManager(layoutManager);
            accountListRecyclerView.SetAdapter(adapter);

            if (Intent.Extras != null)
            {
                accountList = JsonConvert.DeserializeObject<List<NewAccount>>(Intent.Extras.GetString("Accounts"));
                if (accountList != null)
                {
                    adapter = new AddedAccountsAdapter(this, accountList);
                    accountListRecyclerView.SetAdapter(adapter);
                    adapter.NotifyDataSetChanged();
                }
            }

            Button done = FindViewById<Button>(Resource.Id.btnGetStarted);
            done.Click += delegate
            {
                GetStarted();
            };

            TextViewUtils.SetMuseoSans500Typeface(textAddAccountSuccess);
            TextViewUtils.SetMuseoSans500Typeface(done);

            appBarLayout = FindViewById<Android.Support.Design.Widget.AppBarLayout>(Resource.Id.appBar);
            appBarLayout.Visibility = ViewStates.Gone;
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            GetStarted();
        }

        public void GetStarted()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }
    }
}