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
using Android.Content.PM;
using CheeseBind;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Adapter;
using Newtonsoft.Json;
using myTNB_Android.Src.AddAccount.Activity;

namespace myTNB_Android.Src.AddAccount.Fragment
{
    [Activity(Label = "Select Account Type"
              , Icon = "@drawable/ic_launcher"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.Dashboard")]
    public class SelectAccountActivity : BaseToolbarAppCompatActivity
    {

        ListView listView;
        private AccountType selectedAccountType;

        private AccountTypeAdapter accountType;
        private List<AccountType> acctTypes = new List<AccountType>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            selectedAccountType = JsonConvert.DeserializeObject<AccountType>(Intent.Extras.GetString("selectedAccountType"));

            AccountType Residential = new AccountType();
            Residential.Id = "1";
            Residential.Type = "Residential";

            AccountType Commercial = new AccountType();
            Commercial.Id = "2";
            Commercial.Type = "Commercial";

            //AccountType Government = new AccountType();
            //Government.Id = "3";
            //Government.Type = "Government";

            if (selectedAccountType != null)
            {
                if (selectedAccountType.Id.Equals("1"))
                {
                    Residential.IsSelected = true;
                    Commercial.IsSelected = false;
                    //Government.IsSelected = false;
                }
                else if (selectedAccountType.Id.Equals("2"))
                {
                    Residential.IsSelected = false;
                    Commercial.IsSelected = true;
                    //Government.IsSelected = false;
                }
                else if (selectedAccountType.Id.Equals("3"))
                {
                    Residential.IsSelected = false;
                    Commercial.IsSelected = false;
                    //Government.IsSelected = true;
                }
            }
            else
            {
                Residential.IsSelected = true;
                Commercial.IsSelected = false;
                //Government.IsSelected = false;
            }
            acctTypes.Add(Residential);
            acctTypes.Add(Commercial);
            //acctTypes.Add(Government);

            accountType = new AccountTypeAdapter(this, acctTypes);
            listView = FindViewById<ListView>(Resource.Id.list_view);
            listView.Adapter = accountType;

            listView.ItemClick += OnItemClick;
        }

        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            selectedAccountType = accountType.GetItemObject(e.Position);
            selectedAccountType.IsSelected = true;
            Intent link_activity = new Intent(this, typeof(AddAccountActivity));
            link_activity.PutExtra("selectedAccountType", JsonConvert.SerializeObject(selectedAccountType));
            SetResult(Result.Ok, link_activity);
            Finish();
        }


        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SelectAccountTypeView;
        }
    }
}