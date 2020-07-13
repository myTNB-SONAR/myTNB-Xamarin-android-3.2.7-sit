﻿using Android.App;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Activity;
using Android.Views;
using Android.Util;
using System;
using CheeseBind;
using Android.Widget;
using Android.Support.V7.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Models;
using System.Collections.Generic;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.Adapter;
using Android.Support.V4.Content;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP.ApplicationStatusFilterSelection
{
    [Activity(Label = "Select", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusFilterSelectionActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.filterListView)]
        RecyclerView filterListView;

        [BindView(Resource.Id.multiSelectBottomLayout)]
        LinearLayout multiSelectBottomLayout;

        [BindView(Resource.Id.btnMultiFilterApply)]
        Button btnMultiFilterApply;

        const string PAGE_ID = "ApplicationStatus";

        int mRequestKey = -1;
        bool mMultipleSelectCapable = false;

        RecyclerView.LayoutManager layoutManager;

        ApplicationStatusFilterAdapter mAdapter;

        private string filterMonth = "";
        List<ApplicationStatusStringSelectionModel> displayMonth = new List<ApplicationStatusStringSelectionModel>();

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

        [OnClick(Resource.Id.btnMultiFilterApply)]
        internal void OnConfirmClick(object sender, EventArgs e)
        {
            OnConfirmFilter();
        }

        private void OnConfirmFilter()
        {
            Intent finishIntent = new Intent();
            if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                // mTypeList.Clear();
            }
            else if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
            {
                // mStatusCodeList.Clear();
            }
            SetResult(Result.Ok, finishIntent);
            Finish();
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                if (!mMultipleSelectCapable)
                {
                    OnConfirmFilter();
                }
                else
                {
                    EnableButton();
                }                
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

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY))
                {
                    mRequestKey = extras.GetInt(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY);

                    // ApplicationStatus TODO: add list handling
                    if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
                    {
                        // mTypeList.Clear();
                    }
                    else if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
                    {
                        // mStatusCodeList.Clear();
                    }
                }

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_MULTI_SELECT_KEY))
                {
                    mMultipleSelectCapable = extras.GetBoolean(Constants.APPLICATION_STATUS_FILTER_MULTI_SELECT_KEY);
                }
            }

            if (mMultipleSelectCapable)
            {
                DisableButton();
                multiSelectBottomLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                multiSelectBottomLayout.Visibility = ViewStates.Gone;
            }

            mAdapter = new ApplicationStatusFilterAdapter(this, mRequestKey, mMultipleSelectCapable, null, null, displayMonth);

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            filterListView.SetLayoutManager(layoutManager);
            filterListView.SetAdapter(mAdapter);
            mAdapter.ItemClick += OnItemClick;
        }
    }
}
