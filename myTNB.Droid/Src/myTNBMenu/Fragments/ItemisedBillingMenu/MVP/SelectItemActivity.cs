﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    [Activity(Label = "Filter", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class SelectItemActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.item_liste_view)]
        ListView listItemView;

        SelectItemAdapter selectItemAdapter;
        List<Item> itemList;
        public override int ResourceId()
        {
            return Resource.Layout.SelectItemLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Intent.Extras;

            itemList = new List<Item>();
            listItemView.ItemClick += OnItemClick;

            if (extras != null)
            {
                if (extras.ContainsKey("ITEM_LIST"))
                {
                    itemList = DeSerialze<List<Item>>(extras.GetString("ITEM_LIST"));
                    selectItemAdapter = new SelectItemAdapter(this, itemList);
                    listItemView.Adapter = selectItemAdapter;
                }
            }

            SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Itemised Billing -> Select Item");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [Preserve]
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Item selectedItem = selectItemAdapter.GetItemObject(e.Position);
            itemList.ForEach(item =>
            {
                item.selected = (selectedItem.title == item.title) ? true : false;
            });
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("SELECTED_ITEM_LIST", JsonConvert.SerializeObject(itemList));
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
