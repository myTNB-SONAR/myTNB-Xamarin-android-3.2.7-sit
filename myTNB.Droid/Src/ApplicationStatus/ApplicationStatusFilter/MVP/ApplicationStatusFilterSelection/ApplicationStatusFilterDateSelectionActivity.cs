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
using Android.Support.V7.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Models;
using System.Collections.Generic;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.Adapter;
using Android.Support.V4.Content;
using Android.Support.Design.Widget;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP.ApplicationStatusFilterSelection
{
    [Activity(Label = "Select Creation Date", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusFilterDateSelectionActivity : BaseActivityCustom
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
            SetResult(Result.Ok, finishIntent);
            Finish();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFromDate, txtInputLayoutToDate);
            TextViewUtils.SetMuseoSans300Typeface(txtFromDate, txtToDate);
            TextViewUtils.SetMuseoSans500Typeface(btnApplyFilter);

            // ApplicationStatus TODO: Multilingual
            SetToolBarTitle("Select Creation Date");
            // txtInputLayoutFromDate.Hint = GetLabelCommonByLanguage("email");
            // txtInputLayoutToDate.Hint = GetLabelCommonByLanguage("password");

            txtFromDate.AddTextChangedListener(new InputFilterFormField(txtFromDate, txtInputLayoutFromDate));
            txtToDate.AddTextChangedListener(new InputFilterFormField(txtToDate, txtInputLayoutToDate));

            Bundle extras = Intent.Extras;

            DisableButton();

            if (extras != null)
            {

            }

        }
    }
}
