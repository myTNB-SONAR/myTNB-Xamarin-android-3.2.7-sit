using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.AddAccount.Adapter;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.UpdateID.Adapter;
using myTNB_Android.Src.UpdateID.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.UpdateID.Activity
{
    /*[Activity(Label = "Select Account Type"
              , Icon = "@drawable/ic_launcher"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.Dashboard")]*/
    public class SelectIdActivity : BaseActivityCustom
    {

        ListView listView;
        private IdType selectedIdType;
        private string PAGE_ID = "AddId";

        private IdTypeAdapter idType;
        private List<IdType> idTypes = new List<IdType>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("selectedAccountType"))
                {
                    selectedIdType = DeSerialze<IdType>(extras.GetString("selectedAccountType"));
                }
            }

            SetToolBarTitle(GetLabelByLanguage("selectAccountType"));
            IdType Residential = new IdType();
            Residential.Id = "1";
            Residential.Type = GetLabelByLanguage("residental");

            IdType Commercial = new IdType();
            Commercial.Id = "2";
            Commercial.Type = GetLabelByLanguage("commercial");

            //AccountType Government = new AccountType();
            //Government.Id = "3";
            //Government.Type = "Government";

            if (selectedIdType != null)
            {
                if (selectedIdType.Id.Equals("1"))
                {
                    Residential.IsSelected = true;
                    Commercial.IsSelected = false;
                    //Government.IsSelected = false;
                }
                else if (selectedIdType.Id.Equals("2"))
                {
                    Residential.IsSelected = false;
                    Commercial.IsSelected = true;
                    //Government.IsSelected = false;
                }
                else if (selectedIdType.Id.Equals("3"))
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
            idTypes.Add(Residential);
            idTypes.Add(Commercial);
            //acctTypes.Add(Government);

            idType = new IdTypeAdapter(this, idTypes);
            listView = FindViewById<ListView>(Resource.Id.list_view);
            listView.Adapter = idType;

            listView.ItemClick += OnItemClick;
        }

        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                selectedIdType = idType.GetItemObject(e.Position);
                selectedIdType.IsSelected = true;
                Intent link_activity = new Intent(this, typeof(AddAccountActivity));
                link_activity.PutExtra("selectedAccountType", JsonConvert.SerializeObject(selectedIdType));
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
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
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