using System;
using System.Collections.Generic;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.Mobile;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP.ApplicationStatusFilterSelection;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP
{
    [Activity(Label = "Select Filter", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusFilterActivity : BaseActivityCustom, ApplicationStatusFilterContract.IView
    {
        [BindView(Resource.Id.rootview)]
        LinearLayout rootview;

        [BindView(Resource.Id.applicationTypeMainLayout)]
        LinearLayout applicationTypeMainLayout;

        [BindView(Resource.Id.applicationStatusItemTitle)]
        TextView applicationStatusItemTitle;

        [BindView(Resource.Id.applicationStatusSubTitle)]
        TextView applicationStatusSubTitle;

        [BindView(Resource.Id.applicationStatusItemRightArrow)]
        ImageView applicationStatusItemRightArrow;

        [BindView(Resource.Id.applicationStatusItemGroupContentSeparator)]
        View applicationStatusItemGroupContentSeparator;

        [BindView(Resource.Id.statusMainLayout)]
        LinearLayout statusMainLayout;

        [BindView(Resource.Id.statusItemTitle)]
        TextView statusItemTitle;

        [BindView(Resource.Id.statusSubTitle)]
        TextView statusSubTitle;

        [BindView(Resource.Id.statusItemRightArrow)]
        ImageView statusItemRightArrow;

        [BindView(Resource.Id.statusItemGroupContentSeparator)]
        View statusItemGroupContentSeparator;

        [BindView(Resource.Id.filterDateMainLayout)]
        LinearLayout filterDateMainLayout;

        [BindView(Resource.Id.filterDateItemTitle)]
        TextView filterDateItemTitle;

        [BindView(Resource.Id.filterDateSubTitle)]
        TextView filterDateSubTitle;

        [BindView(Resource.Id.filterDateItemRightArrow)]
        ImageView filterDateItemRightArrow;

        [BindView(Resource.Id.filterDateItemGroupContentSeparator)]
        View filterDateItemGroupContentSeparator;

        [BindView(Resource.Id.btnClearFilter)]
        Button btnClearFilter;

        [BindView(Resource.Id.btnApplyFilter)]
        Button btnApplyFilter;

        bool isApplications = false;
        bool isApplyFilter = false;
        ApplicationStatusFilterPresenter mPresenter;

        const string PAGE_ID = "ApplicationStatus";

        private string filterDate = "";
        private string targetApplicationTypeId = "";
        private string targetApplicationStatusCode = "";
        internal GetAllApplicationsResponse AllApplicationResponse;
        List<ApplicationStatusCodeModel> statusCodeList = new List<ApplicationStatusCodeModel>();
        List<ApplicationStatusTypeModel> typeList = new List<ApplicationStatusTypeModel>();
        ApplicationStatusTypeModel selectedType = new ApplicationStatusTypeModel();
        ApplicationStatusCodeModel selectedStatus = new ApplicationStatusCodeModel();
        private string displayDate = "";
        private string fromDate = "";
        private string toDate = "";

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatusFilterLandingLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ApplicationStatusFilterPresenter(this);

            TextViewUtils.SetMuseoSans300Typeface(applicationStatusItemTitle, applicationStatusSubTitle);
            TextViewUtils.SetMuseoSans300Typeface(statusItemTitle, statusSubTitle);
            TextViewUtils.SetMuseoSans300Typeface(filterDateItemTitle, filterDateSubTitle);
            TextViewUtils.SetMuseoSans500Typeface(btnClearFilter, btnApplyFilter);

            SetText();

            SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusFilter", "title"));
            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                AllApplicationsCache.Instance.ApplicationTypeID = string.Empty;
                AllApplicationsCache.Instance.StatusDescription = string.Empty;
                AllApplicationsCache.Instance.CreatedDateFrom = string.Empty;
                AllApplicationsCache.Instance.CreatedDateTo = string.Empty;

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY))
                {
                    targetApplicationTypeId = extras.GetString(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY);
                    AllApplicationsCache.Instance.ApplicationTypeID = targetApplicationTypeId;
                }

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY))
                {
                    targetApplicationStatusCode = extras.GetString(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY);
                    AllApplicationsCache.Instance.StatusDescription = targetApplicationStatusCode;
                }

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_DATE_KEY))
                {
                    filterDate = extras.GetString(Constants.APPLICATION_STATUS_FILTER_DATE_KEY);
                    filterDateSubTitle.Text = filterDate;
                }
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY))
                {
                    fromDate = extras.GetString(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY);
                    AllApplicationsCache.Instance.CreatedDateFrom = fromDate;
                }
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY))
                {
                    toDate = extras.GetString(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY);
                    AllApplicationsCache.Instance.CreatedDateTo = toDate;
                }

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_STATUS_LIST_KEY))
                {
                    try
                    {
                        string statusListKey = extras.GetString(Constants.APPLICATION_STATUS_STATUS_LIST_KEY);
                        statusCodeList = JsonConvert.DeserializeObject<List<ApplicationStatusCodeModel>>(statusListKey);
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
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }

            if (!string.IsNullOrEmpty(targetApplicationStatusCode) && statusCodeList != null && statusCodeList.Count > 0)
            {
                ApplicationStatusCodeModel displayStatusModel = statusCodeList.Find(x => x.StateCode == targetApplicationStatusCode);
                if (displayStatusModel != null)
                {
                    statusSubTitle.Text = displayStatusModel.Status;
                }
                else
                {
                    statusSubTitle.Text = "";
                }
            }
            else
            {
                statusSubTitle.Text = "";
            }

            if (!string.IsNullOrEmpty(targetApplicationTypeId) && typeList != null && typeList.Count > 0)
            {
                ApplicationStatusTypeModel displayTypeModel = typeList.Find(x => x.TypeCode == targetApplicationTypeId);
                if (displayTypeModel != null)
                {
                    applicationStatusSubTitle.Text = displayTypeModel.Type;
                }
                else
                {
                    applicationStatusSubTitle.Text = "";
                }
            }
            else
            {
                applicationStatusSubTitle.Text = "";
            }

            //if (!string.IsNullOrEmpty(filterDate) && filterDate.Contains(","))
            //{
            //    string[] filterDateArray = filterDate.Split(",");
            //    string displayDate = "";
            //    for (int i = 0; i < filterDateArray.Length; i++)
            //    {
            //        string tempDateTime = "";
            //        DateTime dateTimeParse = DateTime.ParseExact(filterDateArray[i], "yyyyMMddTHHmmss",
            //                    CultureInfo.InvariantCulture, DateTimeStyles.None);
            //        TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
            //        DateTime dateTimeMalaysia = TimeZoneInfo.ConvertTimeFromUtc(dateTimeParse, tzi);
            //        if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
            //        {
            //            CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
            //            tempDateTime = dateTimeMalaysia.ToString("MMM yyyy", currCult);
            //        }
            //        else
            //        {
            //            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            //            tempDateTime = dateTimeMalaysia.ToString("MMM yyyy", currCult);
            //        }

            //        if (i == 0)
            //        {
            //            displayDate += tempDateTime;
            //        }
            //        else
            //        {
            //            displayDate += " - " + tempDateTime;
            //        }
            //    }

            //    filterDateSubTitle.Text = displayDate;
            //}
            //else
            //{
            //    filterDateSubTitle.Text = "";
            //}

            DisableButtons();
        }

        private void SetText()
        {
            applicationStatusItemTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusFilter", "applicationType");
            statusItemTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusFilter", "status");
            filterDateItemTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusFilter", "creationDate");
            btnClearFilter.Text = Utility.GetLocalizedLabel("ApplicationStatusFilter", "clear");
            btnApplyFilter.Text = Utility.GetLocalizedLabel("ApplicationStatusFilter", "apply");
        }

        public void DisableButtons()
        {
            if (string.IsNullOrEmpty(targetApplicationTypeId) && string.IsNullOrEmpty(targetApplicationStatusCode) && string.IsNullOrEmpty(filterDate))
            {
                btnClearFilter.Enabled = false;
                btnClearFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_outline);
                btnClearFilter.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.silverChalice));
                btnApplyFilter.Enabled = false;
                btnApplyFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            }
        }

        public void EnableButtons()
        {
            if (!string.IsNullOrEmpty(targetApplicationTypeId) || !string.IsNullOrEmpty(targetApplicationStatusCode) || !string.IsNullOrEmpty(filterDate))
            {
                btnClearFilter.Enabled = true;
                btnClearFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);
                btnClearFilter.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
                btnApplyFilter.Enabled = true;
                btnApplyFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);

            }
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (requestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {

                        Bundle extra = data.Extras;
                        List<ApplicationStatusTypeModel> resultTypeList = new List<ApplicationStatusTypeModel>();

                        if (extra.ContainsKey(Constants.APPLICATION_STATUS_TYPE_LIST_KEY))
                        {
                            resultTypeList = JsonConvert.DeserializeObject<List<ApplicationStatusTypeModel>>(extra.GetString(Constants.APPLICATION_STATUS_TYPE_LIST_KEY));
                            selectedType = resultTypeList.Find(x => x.isChecked);
                            targetApplicationTypeId = selectedType.TypeCode;
                            AllApplicationsCache.Instance.ApplicationTypeID = targetApplicationTypeId;
                            applicationStatusSubTitle.Text = selectedType.Type;
                        }
                    }
                }
                if (requestCode == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {

                        Bundle extra = data.Extras;
                        List<ApplicationStatusCodeModel> resultTypeList = new List<ApplicationStatusCodeModel>();

                        if (extra.ContainsKey(Constants.APPLICATION_STATUS_STATUS_LIST_KEY))
                        {
                            resultTypeList = JsonConvert.DeserializeObject<List<ApplicationStatusCodeModel>>(extra.GetString(Constants.APPLICATION_STATUS_STATUS_LIST_KEY));
                            selectedStatus = resultTypeList.Find(x => x.isChecked);
                            targetApplicationStatusCode = selectedStatus.StateCode;
                            AllApplicationsCache.Instance.StatusDescription = targetApplicationStatusCode;
                            statusSubTitle.Text = selectedStatus.Status;
                        }
                    }
                }
                if (requestCode == Constants.APPLICATION_STATUS_FILTER_DATE_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Bundle extra = data.Extras;
                        string resultDate = "";

                        if (extra.ContainsKey(Constants.APPLICATION_STATUS_FILTER_DATE_KEY))
                        {
                            resultDate = extra.GetString(Constants.APPLICATION_STATUS_FILTER_DATE_KEY);
                        }

                        if (!string.IsNullOrEmpty(resultDate) && resultDate.Contains(","))
                        {
                            filterDate = resultDate;
                            string[] filterDateArray = filterDate.Split(",");
                            for (int i = 0; i < filterDateArray.Length; i++)
                            {
                                string tempDateTime = "";
                                DateTime dateTimeParse = DateTime.ParseExact(filterDateArray[i], "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
                                DateTime dateTimeMalaysia = TimeZoneInfo.ConvertTimeFromUtc(dateTimeParse, tzi);
                                if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                                {
                                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                                    tempDateTime = dateTimeMalaysia.ToString("MMM yyyy", currCult);
                                }
                                else
                                {
                                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                    tempDateTime = dateTimeMalaysia.ToString("MMM yyyy", currCult);
                                }

                                if (i == 0)
                                {
                                    displayDate += tempDateTime;
                                    fromDate = dateTimeMalaysia.ToString("yyyy/MM/dd");
                                    AllApplicationsCache.Instance.CreatedDateFrom = fromDate;
                                }
                                else
                                {
                                    displayDate += " - " + tempDateTime;
                                    toDate = dateTimeMalaysia.ToString("yyyy/MM/dd");
                                    AllApplicationsCache.Instance.CreatedDateTo = toDate;
                                }
                            }
                        }
                        else
                        {
                            filterDate = "";
                            displayDate = "";
                        }

                        filterDateSubTitle.Text = displayDate;

                        EnableButtons();
                    }
                }
                if (targetApplicationTypeId != string.Empty || targetApplicationStatusCode != string.Empty || filterDate != string.Empty)
                {
                    EnableButtons();
                }
                else
                {
                    DisableButtons();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public int GetDeviceHorizontalScaleInPixel(float percentageValue)
        {
            var deviceWidth = Resources.DisplayMetrics.WidthPixels;
            return GetScaleInPixel(deviceWidth, percentageValue);
        }

        public int GetDeviceVerticalScaleInPixel(float percentageValue)
        {
            var deviceHeight = Resources.DisplayMetrics.HeightPixels;
            return GetScaleInPixel(deviceHeight, percentageValue);
        }

        public int GetScaleInPixel(int basePixel, float percentageValue)
        {
            int scaledInPixel = (int)((float)basePixel * percentageValue);
            return scaledInPixel;
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void EnableButton()
        {
            this.SetIsClicked(false);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Application Status Landing");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.applicationTypeMainLayout)]
        internal void OnFilterTypeClick(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent filterIntent = new Intent(this, typeof(ApplicationStatusFilterSelectionActivity));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY, Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY, targetApplicationStatusCode);

                //filterIntent.PutExtra(Constants.APPLICATION_STATUS_STATUS_LIST_KEY, JsonConvert.SerializeObject(statusCodeList));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_TYPE_LIST_KEY, JsonConvert.SerializeObject(typeList));
                StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE);
            }
        }

        [OnClick(Resource.Id.statusMainLayout)]
        internal void OnFilterStatusClick(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent filterIntent = new Intent(this, typeof(ApplicationStatusFilterSelectionActivity));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY, Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY, targetApplicationStatusCode);
                //filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_YEAR_KEY, filterYear);
                //filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_MONTH_KEY, filterMonth);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_STATUS_LIST_KEY, JsonConvert.SerializeObject(statusCodeList));
                //filterIntent.PutExtra(Constants.APPLICATION_STATUS_TYPE_LIST_KEY, JsonConvert.SerializeObject(typeList));
                StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE);
            }
        }

        [OnClick(Resource.Id.filterDateMainLayout)]
        internal void OnFilterDateClick(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent filterIntent = new Intent(this, typeof(ApplicationStatusFilterDateSelectionActivity));

                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_DATE_KEY, filterDate);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY, fromDate);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY, toDate);
                StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_DATE_REQUEST_CODE);
            }
        }

        [OnClick(Resource.Id.btnApplyFilter)]
        internal void OnConfirmClick(object sender, EventArgs e)
        {
            OnConfirmFilterAsync();
        }
        private async System.Threading.Tasks.Task OnConfirmFilterAsync()
        {
            try
            {
                if (ConnectionUtils.HasInternetConnection(this))
                {
                    isApplyFilter = true;
                    //AllApplicationsCache.Instance.Clear();
                    //AllApplicationsCache.Instance.Reset();

                    ShowProgressDialog();
                    AllApplicationResponse = await ApplicationStatusManager.Instance.GetAllApplications(1
                                           , targetApplicationTypeId
                                           , targetApplicationStatusCode
                                           , fromDate
                                           , toDate
                                           , true);
                    if (AllApplicationResponse != null)
                    {
                        if (AllApplicationResponse.StatusDetail.IsSuccess && AllApplicationResponse.Content.Applications.Count > 0)
                        {
                            isApplyFilter = true;
                            AllApplicationsCache.Instance.HasFilterResult = true;
                            AllApplicationsCache.Instance.AllApplicationResponse = AllApplicationResponse;

                            Intent finishIntent = new Intent();

                            finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY, JsonConvert.SerializeObject(targetApplicationTypeId));
                            finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY, JsonConvert.SerializeObject(targetApplicationStatusCode));
                            finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_DATE_KEY, JsonConvert.SerializeObject(displayDate));
                            finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY, JsonConvert.SerializeObject(fromDate));
                            finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY, JsonConvert.SerializeObject(toDate));
                            finishIntent.PutExtra("ApplicaitonFlilterList", JsonConvert.SerializeObject(AllApplicationResponse));
                            SetResult(Result.Ok, finishIntent);
                            Finish();
                        }

                        else
                        {
                            isApplications = true;
                            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                            .SetTitle(AllApplicationResponse.StatusDetail.Title)
                            .SetMessage(AllApplicationResponse.StatusDetail.Message)
                            .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                            .Build();
                            whereisMyacc.Show();
                        }
                    }
                    HideProgressDialog();
                }
                else
                {
                    ShowNoInternetSnackbar();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootview, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
            this.SetIsClicked(false);
        }
        [OnClick(Resource.Id.btnClearFilter)]
        internal void OnClearFilterClick(object sender, EventArgs e)
        {
            isApplyFilter = true;
            filterDate = string.Empty;
            displayDate = string.Empty;
            targetApplicationTypeId = string.Empty;
            targetApplicationStatusCode = string.Empty;
            applicationStatusSubTitle.Text = string.Empty;
            statusSubTitle.Text = string.Empty;
            filterDateSubTitle.Text = string.Empty;
            AllApplicationsCache.Instance.ApplicationTypeID = string.Empty;
            AllApplicationsCache.Instance.StatusDescription = string.Empty;
            AllApplicationsCache.Instance.CreatedDateFrom = string.Empty;
            AllApplicationsCache.Instance.CreatedDateTo = string.Empty;
            AllApplicationsCache.Instance.Clear();

            AllApplicationsCache.Instance.Reset();
            DisableButtons();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetFilterData();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        private void SetFilterData()
        {
            Intent finishIntent = new Intent();
            if (!isApplications && isApplyFilter)
            {

                isApplyFilter = false;
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY, JsonConvert.SerializeObject(targetApplicationTypeId));
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY, JsonConvert.SerializeObject(targetApplicationStatusCode));
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_DATE_KEY, JsonConvert.SerializeObject(displayDate));
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY, JsonConvert.SerializeObject(fromDate));
                finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY, JsonConvert.SerializeObject(toDate));
                finishIntent.PutExtra("ApplicaitonFlilterList", JsonConvert.SerializeObject(new GetAllApplicationsResponse()));
                SetResult(Result.Ok, finishIntent);
                Finish();
            }
            else
            {
                isApplications = false;
                SetResult(Result.Ok, finishIntent);
                Finish();
            }
        }
        public override void OnBackPressed()
        {
            SetFilterData();
        }
    }
}