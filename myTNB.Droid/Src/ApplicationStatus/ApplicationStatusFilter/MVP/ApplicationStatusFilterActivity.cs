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

        ApplicationStatusFilterPresenter mPresenter;

        const string PAGE_ID = "ApplicationStatus";

        private string filterApplicationType = "";
        private string filterStatus = "";
        private string filterDate = "";
        List<ApplicationStatusCodeModel> statusCodeList = new List<ApplicationStatusCodeModel>();
        List<ApplicationStatusTypeModel> typeList = new List<ApplicationStatusTypeModel>();
        private string displayDate = "";

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

            //  TODO: ApplicationStatus Multilingual
            SetToolBarTitle("Select Filter");

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY))
                {
                    filterApplicationType = extras.GetString(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY);
                }

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY))
                {
                    filterStatus = extras.GetString(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY);
                }

                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_DATE_KEY))
                {
                    filterDate = extras.GetString(Constants.APPLICATION_STATUS_FILTER_DATE_KEY);
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

            if (!string.IsNullOrEmpty(filterStatus) && statusCodeList != null && statusCodeList.Count > 0)
            {
                ApplicationStatusCodeModel displayStatusModel = statusCodeList.Find(x => x.StateCode == filterStatus);
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

            if (!string.IsNullOrEmpty(filterApplicationType) && typeList != null && typeList.Count > 0)
            {
                ApplicationStatusTypeModel displayTypeModel = typeList.Find(x => x.TypeCode == filterApplicationType);
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

            if (!string.IsNullOrEmpty(filterDate) && filterDate.Contains(","))
            {
                string[] filterDateArray = filterDate.Split(",");
                string displayDate = "";
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
                    }
                    else
                    {
                        displayDate += " - " + tempDateTime;
                    }
                }

                filterDateSubTitle.Text = displayDate;
            }
            else
            {
                filterDateSubTitle.Text = "";
            }

            DisableButtons();
        }

        public void DisableButtons()
        {
            btnClearFilter.Enabled = false;
            btnClearFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_outline);
            btnClearFilter.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.silverChalice));
            btnApplyFilter.Enabled = false;
            btnApplyFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableButtons()
        {
            btnClearFilter.Enabled = true;
            btnClearFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);
            btnClearFilter.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
            btnApplyFilter.Enabled = true;
            btnApplyFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
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
                                }
                                else
                                {
                                    displayDate += " - " + tempDateTime;
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
            /*if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent filterIntent = new Intent(this, typeof(ApplicationStatusFilterSelectionActivity));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_REQUEST_KEY, Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY, filterStatus);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_YEAR_KEY, filterYear);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_MONTH_KEY, filterMonth);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_STATUS_LIST_KEY, JsonConvert.SerializeObject(statusCodeList));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_TYPE_LIST_KEY, JsonConvert.SerializeObject(typeList));
                StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_REQUEST_CODE);
            }*/
        }

        [OnClick(Resource.Id.statusMainLayout)]
        internal void OnFilterStatusClick(object sender, EventArgs e)
        {
            
        }

        [OnClick(Resource.Id.filterDateMainLayout)]
        internal void OnFilterDateClick(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent filterIntent = new Intent(this, typeof(ApplicationStatusFilterDateSelectionActivity));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_DATE_KEY, filterDate);
                StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_DATE_REQUEST_CODE);
            }
        }

        [OnClick(Resource.Id.btnClearFilter)]
        internal void OnClearFilterClick(object sender, EventArgs e)
        {
            DisableButtons();
            filterApplicationType = "";
            filterStatus = "";
            filterDate = "";
            displayDate = "";

            applicationStatusSubTitle.Text = "";
            statusSubTitle.Text = "";
            filterDateSubTitle.Text = "";
        }

    }
}
