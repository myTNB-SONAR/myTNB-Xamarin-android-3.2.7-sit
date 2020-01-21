
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
using myTNB_Android.Src.Common.Adapter;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Common.Activity
{
    [Activity(Label = "SelectCountryActivity"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.UpdateMobile")]
    public class SelectCountryActivity : BaseActivityCustom
    {
        const string PAGE_ID = "SelectCountryPage";
        List<Country> countryList;

        [BindView(Resource.Id.countryListView)]
        ListView countryListView;

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SelectCountryLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            CountryUtil.Instance.SetCountryList();
            countryList = CountryUtil.Instance.GetCountryList();
            SelectCountryISDCodeAdapter adapter = new SelectCountryISDCodeAdapter(this, countryList);
            countryListView.Adapter = adapter;

            countryListView.ItemClick += OnCountryCodeSelect;
        }

        public void OnCountryCodeSelect(object sender, AdapterView.ItemClickEventArgs args)
        {
            int position = args.Position;
            Country selectedCountry = countryList[position];
            Intent intent = new Intent();
            intent.PutExtra("selectedCountry", JsonConvert.SerializeObject(selectedCountry));
            SetResult(Result.Ok,intent);
            Finish();
        }
    }
}
