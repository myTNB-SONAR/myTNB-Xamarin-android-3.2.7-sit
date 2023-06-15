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
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Models;
using System.Collections.Generic;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.Adapter;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Content;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP.ApplicationStatusFilterSelection
{
    [Activity(Label = "Select", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusFilterSelectionActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.filterListView)]
        RecyclerView filterListView;

        [BindView(Resource.Id.multiSelectBottomLayout)]
        LinearLayout multiSelectBottomLayout;

        [BindView(Resource.Id.btnClear)]
        Button btnClear;

        private const string PAGE_ID = "ApplicationStatus";

        private int mRequestKey = -1;
        private bool isClearEnabled = false;
        private bool isApplicationType;
        private bool isApplicationStatus;

        RecyclerView.LayoutManager layoutManager;

        ApplicationStatusFilterAdapter mAdapter;

        private List<ApplicationStatusStringSelectionModel> displayMonth = new List<ApplicationStatusStringSelectionModel>();
        private List<ApplicationStatusCodeModel> statusCodeList = new List<ApplicationStatusCodeModel>();
        private List<ApplicationStatusTypeModel> typeList = new List<ApplicationStatusTypeModel>();

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

        [OnClick(Resource.Id.btnClear)]
        internal void OnClearClick(object sender, EventArgs e)
        {
            OnConfirm(true);
        }

        private void OnConfirm(bool isClear = false)
        {
            Intent finishIntent = new Intent();
            if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                if (isClear)
                {
                    for (int i = 0; i < typeList.Count; i++)
                    {
                        typeList[i].isChecked = false;
                    }
                }
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_TYPE_LIST_KEY, JsonConvert.SerializeObject(typeList));
            }
            else if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
            {
                if (isClear)
                {
                    for (int i = 0; i < statusCodeList.Count; i++)
                    {
                        statusCodeList[i].isChecked = false;
                    }
                }
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_STATUS_LIST_KEY, JsonConvert.SerializeObject(statusCodeList));
            }
            SetResult(Result.Ok, finishIntent);
            Finish();
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                OnConfirm();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            TextViewUtils.SetMuseoSans500Typeface(btnClear);
            TextViewUtils.SetTextSize16(btnClear);
            btnClear.Text = Utility.GetLocalizedLabel("ApplicationStatusFilter", "clear");
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY))
                {
                    mRequestKey = extras.GetInt(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY);

                    //  TODO: ApplicationStatus add list handling
                    if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
                    {
                        SetToolBarTitle(Utility.GetLocalizedLabel("SelectApplicationType", "title"));
                    }
                    else if (mRequestKey == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
                    {
                        SetToolBarTitle(Utility.GetLocalizedLabel("SelectApplicationStatus", "title"));
                    }
                }

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_INDIVIDUAL_CLEAR_KEY))
                {
                    isClearEnabled = extras.GetBoolean(Constants.APPLICATION_STATUS_FILTER_INDIVIDUAL_CLEAR_KEY);
                }
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_STATUS_LIST_KEY))
                {
                    try
                    {
                        string statusListKey = extras.GetString(Constants.APPLICATION_STATUS_STATUS_LIST_KEY);
                        statusCodeList = JsonConvert.DeserializeObject<List<ApplicationStatusCodeModel>>(statusListKey);
                        isApplicationStatus = true;
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_TYPE_LIST_KEY))
                {
                    try
                    {
                        string typeListKey = extras.GetString(Constants.APPLICATION_STATUS_TYPE_LIST_KEY);
                        typeList = JsonConvert.DeserializeObject<List<ApplicationStatusTypeModel>>(typeListKey);
                        isApplicationType = true;
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }

            if (isClearEnabled)
            {
                multiSelectBottomLayout.Visibility = ViewStates.Visible;
                int index = -1;
                bool isCTAEnabled = false;
                if (isApplicationType)
                {
                    index = typeList.FindIndex(x => x.isChecked);
                }
                else if (isApplicationStatus)
                {
                    index = statusCodeList.FindIndex(x => x.isChecked);
                }
                isCTAEnabled = index > -1;
                SetCTAEnabled(isCTAEnabled);
            }
            else
            {
                multiSelectBottomLayout.Visibility = ViewStates.Gone;
            }

            mAdapter = new ApplicationStatusFilterAdapter(this, mRequestKey, isClearEnabled, statusCodeList, typeList, displayMonth);

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            filterListView.SetLayoutManager(layoutManager);
            filterListView.SetAdapter(mAdapter);
            mAdapter.ItemClick += OnItemClick;
        }

        private void SetCTAEnabled(bool isEnabled)
        {
            btnClear.Enabled = isEnabled;
            btnClear.Background = ContextCompat.GetDrawable(this, isEnabled
                ? Resource.Drawable.light_green_outline_round_button_background
                : Resource.Drawable.silver_chalice_outline_round_button_background);
            btnClear.SetTextColor(ContextCompat.GetColorStateList(this, isEnabled
                ? Resource.Color.freshGreen
                : Resource.Color.silverChalice));
        }
    }
}