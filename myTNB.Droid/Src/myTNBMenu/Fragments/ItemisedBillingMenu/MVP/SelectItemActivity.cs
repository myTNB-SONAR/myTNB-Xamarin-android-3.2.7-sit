using System;
using System.Collections.Generic;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Common;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    [Activity(Label = "Filter", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class SelectItemActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.item_liste_view)]
        ListView listItemView;

        [BindView(Resource.Id.itemListTitle)]
        readonly TextView itemListTitle;

        SelectItemAdapter selectItemAdapter;
        List<Item> itemList;
        private string PAGE_ID = "";
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
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);

            TextViewUtils.SetMuseoSans500Typeface(itemListTitle);
            TextViewUtils.SetTextSize16(itemListTitle);

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

                if (extras.ContainsKey("LIST_TITLE"))
                {
                    SetToolBarTitle(extras.GetString("LIST_TITLE"));
                }

                if (extras.ContainsKey("LIST_DESCRIPTION"))
                {
                    itemListTitle.Text = extras.GetString("LIST_DESCRIPTION");
                }
                else
                {
                    itemListTitle.Visibility = ViewStates.Gone;
                }
            }

            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
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