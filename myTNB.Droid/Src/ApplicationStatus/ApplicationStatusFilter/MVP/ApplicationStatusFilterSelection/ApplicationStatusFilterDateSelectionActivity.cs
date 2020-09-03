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


using System.Globalization;
using myTNB_Android.Src.Base.Fragments;
using Google.Android.Material.TextField;
using AndroidX.Core.Content;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP.ApplicationStatusFilterSelection
{
    [Activity(Label = "Select Creation Date", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusFilterDateSelectionActivity : BaseActivityCustom, DatePickerDialog.IOnDateSetListener
    {
        [BindView(Resource.Id.txtInputLayoutFromDate)]
        TextInputLayout txtInputLayoutFromDate;

        [BindView(Resource.Id.txtFromDate)]
        EditText txtFromDate;

        [BindView(Resource.Id.txtInputLayoutToDate)]
        TextInputLayout txtInputLayoutToDate;

        [BindView(Resource.Id.txtToDate)]
        EditText txtToDate;

        [BindView(Resource.Id.btnApplyFilter)]
        Button btnApplyFilter;

        const string PAGE_ID = "ApplicationStatus";

        private string filterDate = "";

        private string startDisplayDate = "";
        private string endDisplayDate = "";

        private DateTime startDateTime;
        private DateTime endDateTime;

        private bool isStartPickerPopup = false;
        private bool isEndPickerPopup = false;

        MonthYearPickerDialog pd;

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatusFilterListDateLayout;
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
            btnApplyFilter.Enabled = false;
            btnApplyFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableButton()
        {
            btnApplyFilter.Enabled = true;
            btnApplyFilter.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        [OnClick(Resource.Id.btnApplyFilter)]
        internal void OnConfirmClick(object sender, EventArgs e)
        {
            OnConfirmFilter();
        }

        private void OnConfirmFilter()
        {
            Intent finishIntent = new Intent();
            string result = "";
            if (!string.IsNullOrEmpty(startDisplayDate) && !string.IsNullOrEmpty(endDisplayDate))
            {
                result += startDateTime.ToString("yyyyMMddTHHmmss");
                result += "," + endDateTime.ToString("yyyyMMddTHHmmss");
            }
            finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_DATE_KEY, result);
            SetResult(Result.Ok, finishIntent);
            Finish();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFromDate, txtInputLayoutToDate);
            TextViewUtils.SetMuseoSans300Typeface(txtFromDate, txtToDate);
            TextViewUtils.SetMuseoSans500Typeface(btnApplyFilter);

            //  TODO: ApplicationStatus Multilingual
            SetToolBarTitle("Select Creation Date");
            // txtInputLayoutFromDate.Hint = GetLabelCommonByLanguage("email");
            // txtInputLayoutToDate.Hint = GetLabelCommonByLanguage("password");

            txtFromDate.AddTextChangedListener(new InputFilterFormField(txtFromDate, txtInputLayoutFromDate));
            txtToDate.AddTextChangedListener(new InputFilterFormField(txtToDate, txtInputLayoutToDate));

            Bundle extras = Intent.Extras;

            DisableButton();

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_DATE_KEY))
                {
                    filterDate = extras.GetString(Constants.APPLICATION_STATUS_FILTER_DATE_KEY);
                }
            }

            if (!string.IsNullOrEmpty(filterDate) && filterDate.Contains(","))
            {
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
                        tempDateTime = dateTimeMalaysia.ToString("MMMM yyyy", currCult);
                    }
                    else
                    {
                        CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                        tempDateTime = dateTimeMalaysia.ToString("MMMM yyyy", currCult);
                    }

                    if (i == 0)
                    {
                        startDisplayDate = tempDateTime;
                        startDateTime = DateTime.ParseExact(filterDateArray[i], "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                    }
                    else
                    {
                        endDisplayDate = tempDateTime;
                        endDateTime = DateTime.ParseExact(filterDateArray[i], "yyyyMMddTHHmmss",
                            CultureInfo.InvariantCulture, DateTimeStyles.None);
                    }
                }

                txtFromDate.Text = startDisplayDate;
                txtToDate.Text = endDisplayDate;
            }

            txtFromDate.Enabled = true;
            txtToDate.Enabled = true;
            txtFromDate.Focusable = false;
            txtToDate.Focusable = false;
            txtFromDate.Click += TxtFromDate_Click;
            txtToDate.Click += TxtToDate_Click;
        }

        private void TxtToDate_Click(object sender, EventArgs e)
        {
            if (!isEndPickerPopup)
            {
                isEndPickerPopup = true;
                ShowDatePicker();
            }
        }

        private void TxtFromDate_Click(object sender, EventArgs e)
        {
            if (!isStartPickerPopup)
            {
                isStartPickerPopup = true;
                ShowDatePicker();
            }
        }

        private void ShowDatePicker()
        {
            if (isStartPickerPopup && !string.IsNullOrEmpty(startDisplayDate))
            {
               pd = new MonthYearPickerDialog(this, 2000, 2099, 1, 12, startDateTime);
            }
            else if (isEndPickerPopup && !string.IsNullOrEmpty(endDisplayDate))
            {
                pd = new MonthYearPickerDialog(this, 2000, 2099, 1, 12, endDateTime);
            }
            else
            {
                pd = new MonthYearPickerDialog(this, 2000, 2099, 1, 12, DateTime.Now);
            }

            pd.SetListener(this);

            pd.mCancelHandler += Pd_mCancelHandler;

            pd.Show(this.FragmentManager, "MonthYearPickerDialog");
        }

        private void Pd_mCancelHandler(object sender, int e)
        {
            pd.Dialog.Cancel();
            isStartPickerPopup = false;
            isEndPickerPopup = false;
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            if (isStartPickerPopup)
            {
                var checkDateTime = new DateTime(year, month, dayOfMonth + 1);

                if (!string.IsNullOrEmpty(endDisplayDate) && checkDateTime.Date > endDateTime.Date)
                {
                    isStartPickerPopup = false;
                    return;
                }

                startDateTime = new DateTime(year, month, dayOfMonth + 1);

                if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                    startDisplayDate = startDateTime.ToString("MMMM yyyy", currCult);
                }
                else
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    startDisplayDate = startDateTime.ToString("MMMM yyyy", currCult);
                }

                txtFromDate.Text = startDisplayDate;

                isStartPickerPopup = false;
            }
            else
            {
                var checkDateTime = new DateTime(year, month, dayOfMonth + 1);

                if (!string.IsNullOrEmpty(startDisplayDate) && checkDateTime.Date < startDateTime.Date)
                {
                    isEndPickerPopup = false;
                    return;
                }

                endDateTime = new DateTime(year, month, dayOfMonth + 1);

                if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                    endDisplayDate = endDateTime.ToString("MMMM yyyy", currCult);
                }
                else
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    endDisplayDate = endDateTime.ToString("MMMM yyyy", currCult);
                }

                txtToDate.Text = endDisplayDate;

                isEndPickerPopup = false;
            }

            if (!string.IsNullOrEmpty(startDisplayDate) && !string.IsNullOrEmpty(endDisplayDate))
            {
                EnableButton();
            }
            else
            {
                DisableButton();
            }
        }
    }
}
