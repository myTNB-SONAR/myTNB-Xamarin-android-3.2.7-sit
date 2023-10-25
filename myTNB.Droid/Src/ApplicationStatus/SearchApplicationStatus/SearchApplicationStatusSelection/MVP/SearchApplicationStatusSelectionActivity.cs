using Android.App;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Activity;
using Android.Views;
using Android.Util;
using System;
using CheeseBind;
using Android.Widget;
using System.Collections.Generic;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.SearchApplicationStatusSelection.Adapter;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.Models;
using Newtonsoft.Json;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Content;
using System.ComponentModel;

namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.SearchApplicationStatusSelection.MVP
{
    [Activity(Label = "Select", Theme = "@style/Theme.RegisterForm")]
    public class SearchApplicationStatusSelectionActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.filterListView)]
        RecyclerView filterListView;

        [BindView(Resource.Id.multiSelectBottomLayout)]
        LinearLayout multiSelectBottomLayout;

        [BindView(Resource.Id.btnClear)]
        Button btnMultiFilterApply;

        const string PAGE_ID = "ApplicationStatus";

        int mRequestKey = -1;

        RecyclerView.LayoutManager layoutManager;

        SearchApplicationStatusAdapter mAdapter;

        List<TypeModel> mTypeList = new List<TypeModel>();

        List<SearchByModel> mSearchByList = new List<SearchByModel>();

        List<SMRTypeModel> mSMRTypeList = new List<SMRTypeModel>();

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatusFilterListLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Application Status Filter Selection");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisableButton()
        {
            btnMultiFilterApply.Enabled = false;
            btnMultiFilterApply.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableButton()
        {
            btnMultiFilterApply.Enabled = true;
            btnMultiFilterApply.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        [OnClick(Resource.Id.btnClear)]
        internal void OnConfirmClick(object sender, EventArgs e)
        {
            OnConfirmFilter();
        }

        private void OnConfirmFilter()
        {
            Intent finishIntent = new Intent();
            if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_TYPE_LIST_KEY, JsonConvert.SerializeObject(mTypeList));
            }
            else if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
            {
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY, JsonConvert.SerializeObject(mSearchByList));
            }
            else if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_SMRTYPE_REQUEST_CODE)
            {
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_SMRTYPE_LIST_KEY, JsonConvert.SerializeObject(mSMRTypeList));
            }
            SetResult(Result.Ok, finishIntent);
            Finish();
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                OnConfirmFilter();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextViewUtils.SetMuseoSans500Typeface(btnMultiFilterApply);
            TextViewUtils.SetTextSize16(btnMultiFilterApply);

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY))
                {
                    mRequestKey = extras.GetInt(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY);

                    if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
                    {
                        //  TODO: ApplicationStatus Multilingual
                        SetToolBarTitle(Utility.GetLocalizedLabel("SelectApplicationType", "title"));

                        if (extras.ContainsKey(Constants.APPLICATION_STATUS_TYPE_LIST_KEY))
                        {
                            mTypeList = DeSerialze<List<TypeModel>>(extras.GetString(Constants.APPLICATION_STATUS_TYPE_LIST_KEY));
                        }
                    }
                    else if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
                    {

                        SetToolBarTitle(Utility.GetLocalizedLabel("SearchByNumber", "title"));

                        if (extras.ContainsKey(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY))
                        {
                            mSearchByList = JsonConvert.DeserializeObject<List<SearchByModel>>(extras.GetString(Constants.APPLICATION_STATUS_SEARCH_BY_LIST_KEY));
                        }
                    }
                    else if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_SMRTYPE_REQUEST_CODE)
                    {
                        //  TODO: ApplicationStatus Multilingual
                        SetToolBarTitle(Utility.GetLocalizedLabel("SearchByNumber", "title"));

                        if (extras.ContainsKey(Constants.APPLICATION_STATUS_SMRTYPE_LIST_KEY))
                        {
                            mSMRTypeList = DeSerialze<List<SMRTypeModel>>(extras.GetString(Constants.APPLICATION_STATUS_SMRTYPE_LIST_KEY)); 
                        }
                    }
                }
            }

            multiSelectBottomLayout.Visibility = ViewStates.Gone;
            mAdapter = new SearchApplicationStatusAdapter(this, mRequestKey, mTypeList, mSearchByList, mSMRTypeList);

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            filterListView.SetLayoutManager(layoutManager);
            filterListView.SetAdapter(mAdapter);
            mAdapter.ItemClick += OnItemClick;
        }
    }
}