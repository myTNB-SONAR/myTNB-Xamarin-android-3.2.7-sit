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
using myTNB_Android.Src.ViewBill.Activity;

namespace myTNB_Android.Src.BillStatement.MVP
{
    [Activity(Label = "Select Creation Date", Theme = "@style/Theme.RegisterForm")]
    public class BillStatementActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.imgSixMonthsAction)]
        ImageView imgSixMonthsAction;

        [BindView(Resource.Id.imgTheeMonthsAction)]
        ImageView imgTheeMonthsAction;

        [BindView(Resource.Id.txtThreeMonths)]
        TextView txtThreeMonths;

        [BindView(Resource.Id.txtPageTitleInfo)]
        TextView txtPageTitleInfo;

        [BindView(Resource.Id.txtSixMonth)]
        TextView txtSixMonth;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        bool isSixMonthSelected = false;
        bool isThreeMonthSelected = false;


        AccountData selectedAccount;

        const string PAGE_ID = "ViewAccountStatement";

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
            if (!this.GetIsClicked())
            {
                try
                {
                    this.SetIsClicked(true);
                    ShowBillStatementPDF();
                    FirebaseAnalyticsUtils.LogClickEvent(this, "View Bill Buttom Clicked");
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        [OnClick(Resource.Id.sixMonthContainer)]
        internal void OnSixMonthsContainerClick(object sender, EventArgs e)
        {
            imgSixMonthsAction.Visibility = ViewStates.Visible;
            imgTheeMonthsAction.Visibility = ViewStates.Gone;
            isSixMonthSelected = true;
            isThreeMonthSelected = false;
            SetCTAEnable();
        }
        [OnClick(Resource.Id.threeMonthsContainer)]
        internal void OnthreeMonthsContainerClick(object sender, EventArgs e)
        {
            imgSixMonthsAction.Visibility = ViewStates.Gone;
            imgTheeMonthsAction.Visibility = ViewStates.Visible;
            isSixMonthSelected = false;
            isThreeMonthSelected = true;
            SetCTAEnable();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            txtThreeMonths.Text = Utility.GetLocalizedLabel("StatementPeriod", "past3Months");
            txtPageTitleInfo.Text = Utility.GetLocalizedLabel("StatementPeriod", "iWantToViewTitle");
            txtSixMonth.Text = Utility.GetLocalizedLabel("StatementPeriod", "past6Months");

            TextViewUtils.SetMuseoSans500Typeface(txtPageTitleInfo, btnSubmit);
            TextViewUtils.SetMuseoSans300Typeface(txtThreeMonths, txtSixMonth);
            TextViewUtils.SetTextSize16(txtPageTitleInfo, txtThreeMonths, txtSixMonth, btnSubmit);

            SetToolBarTitle("View Account Statement");


            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("SELECTED_ACCOUNT"))
                {
                    selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString("SELECTED_ACCOUNT"));
                }
            }
            imgTheeMonthsAction.SetMaxHeight(txtThreeMonths.Height);
            imgSixMonthsAction.SetMaxHeight(txtSixMonth.Height);

            SetCTAEnable();
        }

        private void SetCTAEnable()
        {
            bool isEnabled = isSixMonthSelected || isThreeMonthSelected;
            btnSubmit.Enabled = isEnabled;
            btnSubmit.Background = ContextCompat.GetDrawable(this, isEnabled
                ? Resource.Drawable.green_button_background
                : Resource.Drawable.silver_chalice_button_background);
        }
        public void ShowBillStatementPDF()
        {
            string selectedMonths = string.Empty;
            Intent viewBill = new Intent(this, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            viewBill.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_STATEMENT_PDF_REQUEST_CODE);
            if (isSixMonthSelected)
            {
                selectedMonths = "6";
            }
            if (isThreeMonthSelected)
            {
                selectedMonths = "3";
            }
            viewBill.PutExtra(Constants.SELECTED_BILL_STATEMENT, selectedMonths);
            StartActivity(viewBill);
            this.SetIsClicked(false);
        }
    }
}