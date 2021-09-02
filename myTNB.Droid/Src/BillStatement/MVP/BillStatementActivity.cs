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
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.BillStatement.MVP
{
    [Activity(Label = "Select Creation Date", Theme = "@style/Theme.RegisterForm")]
    public class BillStatementActivity : BaseActivityCustom, DatePickerDialog.IOnDateSetListener
    {
        [BindView(Resource.Id.txtInputLayoutFromDate)]
        TextInputLayout txtInputLayoutFromDate;

        [BindView(Resource.Id.txtFromDate)]
        EditText txtFromDate;

        [BindView(Resource.Id.txtInputLayoutToDate)]
        TextInputLayout txtInputLayoutToDate;

        [BindView(Resource.Id.imgCustomPeriodAction)]
        ImageView imgCustomPeriodAction;

        [BindView(Resource.Id.imgSixMonthsAction)]
        ImageView imgSixMonthsAction;

        [BindView(Resource.Id.customDateContainer)]
        LinearLayout customDateContainer;

        [BindView(Resource.Id.filterDateMainLayout)]
        LinearLayout filterDateMainLayout;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;
        
        bool isSixMonthSelected = false;
        bool isCustomDateelected = false;

        [BindView(Resource.Id.txtToDate)]
        EditText txtToDate;

        AccountData selectedAccount;

        const string PAGE_ID = "ViewAccountStatement";

        private string startDisplayDate = string.Empty;
        private string endDisplayDate = string.Empty;

        private DateTime startDateTime;
        private DateTime endDateTime;

        private bool isStartPickerPopup = false;
        private bool isEndPickerPopup = false;
        MonthYearPickerDialog pd;

        public override int ResourceId()
        {
            return Resource.Layout.BillStatement;
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
                FirebaseAnalyticsUtils.SetScreenName(this, "View Account Statement");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnSubmit)]
        internal void OnSubmitClick(object sender, EventArgs e)
        {
           //Show Statement
        }
        

        [OnClick(Resource.Id.customPeriodContainer)]
        internal void OnCustomPeriodContainerClick(object sender, EventArgs e)
        {
            imgCustomPeriodAction.Visibility = ViewStates.Visible;
            imgSixMonthsAction.Visibility = ViewStates.Gone;
            customDateContainer.Visibility = ViewStates.Visible;
            filterDateMainLayout.Visibility = ViewStates.Visible;
            isCustomDateelected = true;
            isSixMonthSelected = false;
            SetCTAEnable();
        }
        [OnClick(Resource.Id.sixMonthsContainer)]
        internal void OnSixMonthsContainerClick(object sender, EventArgs e)
        {
            imgSixMonthsAction.Visibility = ViewStates.Visible;
            imgCustomPeriodAction.Visibility = ViewStates.Gone;
            customDateContainer.Visibility = ViewStates.Gone;
            filterDateMainLayout.Visibility = ViewStates.Gone;
            isSixMonthSelected = true;
            isCustomDateelected = false;
            SetCTAEnable();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFromDate, txtInputLayoutToDate);
            TextViewUtils.SetMuseoSans300Typeface(txtFromDate, txtToDate);

            txtInputLayoutFromDate.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayout_TextAppearance_ColorFixLarge
                : Resource.Style.TextInputLayout_TextAppearance_ColorFix);
            txtInputLayoutToDate.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayout_TextAppearance_ColorFixLarge
                : Resource.Style.TextInputLayout_TextAppearance_ColorFix);
            TextViewUtils.SetTextSize16(txtFromDate, txtToDate);
           

            SetToolBarTitle("View Account Statement");

            txtFromDate.AddTextChangedListener(new InputFilterFormField(txtFromDate, txtInputLayoutFromDate));
            txtToDate.AddTextChangedListener(new InputFilterFormField(txtToDate, txtInputLayoutToDate));

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("SELECTED_ACCOUNT"))
                {
                    selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString("SELECTED_ACCOUNT")); 
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
            bool isEnabled = (!string.IsNullOrEmpty(startDisplayDate) && !string.IsNullOrEmpty(endDisplayDate)) || isSixMonthSelected;
            btnSubmit.Enabled = isEnabled;
            btnSubmit.Background = ContextCompat.GetDrawable(this, isEnabled
                ? Resource.Drawable.green_button_background
                : Resource.Drawable.silver_chalice_button_background);
        }
    }
}