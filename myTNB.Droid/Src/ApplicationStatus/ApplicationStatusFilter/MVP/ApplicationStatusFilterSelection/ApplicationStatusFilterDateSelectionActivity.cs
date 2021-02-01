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
        Button btnApply;

        [BindView(Resource.Id.btnClearFilter)]
        Button btnClear;

        const string PAGE_ID = "ApplicationStatus";

        private string filterDate = string.Empty;

        private string startDisplayDate = string.Empty;
        private string endDisplayDate = string.Empty;

        private DateTime startDateTime;
        private DateTime endDateTime;

        private bool isStartPickerPopup = false;
        private bool isEndPickerPopup = false;
        private bool isClearTapped = false;

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

        [OnClick(Resource.Id.btnApplyFilter)]
        internal void OnApplyClick(object sender, EventArgs e)
        {
            OnConfirmFilter();
        }

        [OnClick(Resource.Id.btnClearFilter)]
        internal void OnClearClick(object sender, EventArgs e)
        {
            startDisplayDate = string.Empty;
            endDisplayDate = string.Empty;
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            txtToDate.Enabled = false;
            isClearTapped = true;
            SetCTAEnable();
        }

        private void OnConfirmFilter()
        {
            Intent finishIntent = new Intent();
            string result = string.Empty;
            if (!string.IsNullOrEmpty(startDisplayDate) && !string.IsNullOrEmpty(endDisplayDate))
            {
                result += startDateTime.ToString("yyyyMMddTHHmmss");
                result += "," + endDateTime.ToString("yyyyMMddTHHmmss");
            }
            finishIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_DATE_KEY, result);
            SetResult(Result.Ok, finishIntent);
            Finish();
        }

        public override void OnBackPressed()
        {
            if (isClearTapped
                && string.IsNullOrEmpty(txtFromDate.Text)
                && string.IsNullOrWhiteSpace(txtFromDate.Text)
                && string.IsNullOrEmpty(txtToDate.Text)
                && string.IsNullOrWhiteSpace(txtToDate.Text))
            {
                OnConfirmFilter();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFromDate, txtInputLayoutToDate);
            TextViewUtils.SetMuseoSans300Typeface(txtFromDate, txtToDate);
            TextViewUtils.SetMuseoSans500Typeface(btnApply);

            txtInputLayoutFromDate.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayout_TextAppearance_ColorFixLarge
                : Resource.Style.TextInputLayout_TextAppearance_ColorFix);
            txtInputLayoutToDate.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayout_TextAppearance_ColorFixLarge
                : Resource.Style.TextInputLayout_TextAppearance_ColorFix);
            txtFromDate.TextSize = TextViewUtils.GetFontSize(16f);
            txtToDate.TextSize = TextViewUtils.GetFontSize(16f);
            btnApply.TextSize = TextViewUtils.GetFontSize(16f);
            btnApply.Text = Utility.GetLocalizedLabel("SelectCreationDate", "apply");
            btnClear.Text = Utility.GetLocalizedLabel("SelectCreationDate", "clear");
            btnClear.TextSize= TextViewUtils.GetFontSize(16f);
            btnApply.TextSize = TextViewUtils.GetFontSize(16f);

            SetToolBarTitle(Utility.GetLocalizedLabel("SelectCreationDate", "title"));

            txtFromDate.AddTextChangedListener(new InputFilterFormField(txtFromDate, txtInputLayoutFromDate));
            txtToDate.AddTextChangedListener(new InputFilterFormField(txtToDate, txtInputLayoutToDate));

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_DATE_KEY))
                {
                    filterDate = extras.GetString(Constants.APPLICATION_STATUS_FILTER_DATE_KEY);
                }
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY))
                {
                    startDisplayDate = extras.GetString(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY);
                }
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY))
                {
                    endDisplayDate = extras.GetString(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY);
                }
            }

            if (startDisplayDate != string.Empty)
            {
                DateTime dateTimeParse = DateTime.ParseExact(startDisplayDate
                    , "yyyy/MM/dd"
                    , CultureInfo.InvariantCulture
                    , DateTimeStyles.None);
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
                DateTime dateTimeMalaysia = TimeZoneInfo.ConvertTimeFromUtc(dateTimeParse, tzi);
                startDateTime = dateTimeMalaysia;
                if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                    txtFromDate.Text = dateTimeMalaysia.ToString("MMMM yyyy", currCult);
                }
                else
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    txtFromDate.Text = dateTimeMalaysia.ToString("MMMM yyyy", currCult);
                }
            }
            if (endDisplayDate != string.Empty)
            {
                DateTime dateTimeParse = DateTime.ParseExact(endDisplayDate, "yyyy/MM/dd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None);
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
                DateTime dateTimeMalaysia = TimeZoneInfo.ConvertTimeFromUtc(dateTimeParse, tzi);
                endDateTime = dateTimeMalaysia;
                if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                    txtToDate.Text = dateTimeMalaysia.ToString("MMMM yyyy", currCult);
                }
                else
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    txtToDate.Text = dateTimeMalaysia.ToString("MMMM yyyy", currCult);
                }
            }

            txtFromDate.Enabled = true;
            txtToDate.Enabled = !string.IsNullOrEmpty(txtToDate.Text) && !string.IsNullOrWhiteSpace(txtToDate.Text);

            txtFromDate.Focusable = false;
            txtToDate.Focusable = false;

            txtFromDate.Click += TxtFromDate_Click;
            txtToDate.Click += TxtToDate_Click;

            SetCTAEnable();
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
                pd = new MonthYearPickerDialog(this
                    , Convert.ToInt32(Utility.GetLocalizedLabel("SelectCreationDate", "minimumYear"))
                    , DateTime.Now.Year
                    , 1
                    , 12
                    , startDateTime);
            }
            else if (isEndPickerPopup && !string.IsNullOrEmpty(endDisplayDate))
            {
                pd = new MonthYearPickerDialog(this
                    , Convert.ToInt32(Utility.GetLocalizedLabel("SelectCreationDate", "minimumYear"))
                    , DateTime.Now.Year
                    , 1
                    , 12
                    , endDateTime);
            }
            else
            {
                pd = new MonthYearPickerDialog(this
                    , Convert.ToInt32(Utility.GetLocalizedLabel("SelectCreationDate", "minimumYear"))
                    , DateTime.Now.Year
                    , 1
                    , 12
                    , DateTime.Now);
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
                txtToDate.Enabled = true;
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

            SetCTAEnable();
        }

        private void SetCTAEnable()
        {
            bool isClearEnabled = !string.IsNullOrEmpty(startDisplayDate) || !string.IsNullOrEmpty(endDisplayDate);
            bool isApplyEnabled = !string.IsNullOrEmpty(startDisplayDate) && !string.IsNullOrEmpty(endDisplayDate);

            btnClear.Enabled = isClearEnabled;
            btnClear.Background = ContextCompat.GetDrawable(this, isClearEnabled
                ? Resource.Drawable.light_green_outline_button_background
                : Resource.Drawable.silver_chalice_button_outline);
            btnClear.SetTextColor(ContextCompat.GetColorStateList(this, isClearEnabled
                ? Resource.Color.freshGreen
                : Resource.Color.silverChalice));

            btnApply.Enabled = isApplyEnabled;
            btnApply.Background = ContextCompat.GetDrawable(this, isApplyEnabled
                ? Resource.Drawable.green_button_background
                : Resource.Drawable.silver_chalice_button_background);
        }
    }
}