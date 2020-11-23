
using System;
using System.Collections.Generic;
using System.Linq;
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
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    [Activity(Label = "Filter Views", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class FilterBillHistoryActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.billFilterMessage)]
        TextView billFilterMessage;

        [BindView(Resource.Id.txtFilterLabel)]
        TextView txtFilterLabel;

        [BindView(Resource.Id.txtFilterSelected)]
        TextView txtFilterSelected;

        [BindView(Resource.Id.btnBillFilter)]
        Button btnBillFilter;

        const int SELECT_FILTER = 10001;
        List<Item> itemFilterList;

        private const string PAGE_ID = "BillFilter";

        [OnClick(Resource.Id.btnBillFilter)]
        void OnApplyFilter(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent resultIntent = new Intent();
                    resultIntent.PutExtra("SELECTED_ITEM_FILTER", JsonConvert.SerializeObject(itemFilterList.Find(itemFilter=> { return itemFilter.selected; })));
                    SetResult(Result.Ok, resultIntent);
                    Finish();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.filterSelectContainer)]
        void OnSelectFilter(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent newIntent = new Intent(this, typeof(SelectItemActivity));
                    newIntent.PutExtra("ITEM_LIST", JsonConvert.SerializeObject(itemFilterList));
                    StartActivityForResult(newIntent, SELECT_FILTER);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.FilterBillHistoryLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            Bundle extras = Intent.Extras;
            TextViewUtils.SetMuseoSans500Typeface(billFilterMessage, btnBillFilter);
            TextViewUtils.SetMuseoSans300Typeface(txtFilterLabel, txtFilterSelected);
            btnBillFilter.TextSize = TextViewUtils.GetFontSize(16f);
            billFilterMessage.TextSize = TextViewUtils.GetFontSize(16f);
            txtFilterLabel.TextSize = TextViewUtils.GetFontSize(9f);
            txtFilterSelected.TextSize = TextViewUtils.GetFontSize(16f);

            btnBillFilter.Text = GetLabelByLanguage("applyFilter");
            txtFilterLabel.Text = GetLabelByLanguage("filterBy").ToUpper();
            txtFilterSelected.Text = "";
            if (extras != null)
            {
                if (extras.ContainsKey("ITEM_LIST"))
                {
                    itemFilterList = DeSerialze<List<Item>>(extras.GetString("ITEM_LIST"));
                    if (itemFilterList.Count > 0)
                    {
                        int foundItemIndex = itemFilterList.FindIndex(itemFilter =>
                        {
                            return itemFilter.selected;
                        });

                        if (foundItemIndex != -1)
                        {
                            txtFilterSelected.Text = itemFilterList[foundItemIndex].title;
                        }
                    }
                }

                if (extras.ContainsKey("FILTER_DESCRIPTION") && !string.IsNullOrEmpty(extras.GetString("FILTER_DESCRIPTION")))
                {
                    if (extras.GetString("FILTER_DESCRIPTION") == "RE")
                    {
                        billFilterMessage.Text = GetLabelByLanguage("descriptionRE");
                    }
                    else
                    {
                        billFilterMessage.Text = GetLabelByLanguage("description");
                    }
                }
                else
                {
                    billFilterMessage.Text = GetLabelByLanguage("description");
                }
            }
            else
            {
                billFilterMessage.Text = GetLabelByLanguage("description");
            }

            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == SELECT_FILTER)
                {
                    if (resultCode == Result.Ok)
                    {
                        itemFilterList = JsonConvert.DeserializeObject<List<Item>>(data.GetStringExtra("SELECTED_ITEM_LIST"));
                        txtFilterSelected.Text = itemFilterList.Find(itemFilter =>
                        {
                            return itemFilter.selected;
                        }).title;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
