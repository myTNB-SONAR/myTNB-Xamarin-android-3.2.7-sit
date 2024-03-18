using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using myTNB.AndroidApp.Src.AddAccount.Activity;
using myTNB.AndroidApp.Src.AddAccount.Adapter;
using myTNB.AndroidApp.Src.AddAccount.Models;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.AddAccount.Fragment
{
    [Activity(Label = "Select Account Type"
              , Icon = "@drawable/ic_launcher"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class SelectAccountActivity : BaseActivityCustom
    {

        ListView listView;
        private AccountType selectedAccountType;
        private string PAGE_ID = "AddAccount";

        private AccountTypeAdapter accountType;
        private List<AccountType> acctTypes = new List<AccountType>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("selectedAccountType"))
                {
                    selectedAccountType = DeSerialze<AccountType>(extras.GetString("selectedAccountType"));
                }
            }

            SetToolBarTitle(GetLabelByLanguage("selectAccountType"));
            AccountType Residential = new AccountType();
            Residential.Id = "1";
            Residential.Type = GetLabelByLanguage("residential");

            AccountType Commercial = new AccountType();
            Commercial.Id = "2";
            Commercial.Type = GetLabelByLanguage("business");

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
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                selectedAccountType = accountType.GetItemObject(e.Position);
                selectedAccountType.IsSelected = true;
                Intent link_activity = new Intent(this, typeof(AddAccountActivity));
                link_activity.PutExtra("selectedAccountType", JsonConvert.SerializeObject(selectedAccountType));
                SetResult(Result.Ok, link_activity);
                Finish();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Add Account -> Select Account Type");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SelectAccountTypeView;
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}