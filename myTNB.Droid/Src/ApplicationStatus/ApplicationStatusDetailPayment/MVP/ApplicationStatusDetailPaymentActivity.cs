
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;


using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetailPayment.MVP
{
    [Activity(Label = "Bill Details", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class ApplicationStatusDetailPaymentActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.accountPayAmountLabel)]
        TextView accountPayAmountLabel;

        [BindView(Resource.Id.accountPayAmountCurrency)]
        TextView accountPayAmountCurrency;

        [BindView(Resource.Id.accountPayAmountValue)]
        TextView accountPayAmountValue;

        [BindView(Resource.Id.otherChargesExpandableView)]
        ExpandableTextViewComponent otherChargesExpandableView;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.btnPayBill)]
        Button btnPayBill;

        [BindView(Resource.Id.bottomLayout)]
        LinearLayout bottomLayout;

        [BindView(Resource.Id.topLayout)]
        LinearLayout topLayout;

        [BindView(Resource.Id.detailLayout)]
        LinearLayout detailLayout;

        [BindView(Resource.Id.refreshLayout)]
        LinearLayout refreshLayout;

        [BindView(Resource.Id.refreshBillingDetailImg)]
        ImageView refreshBillingDetailImg;

        [BindView(Resource.Id.refreshBillingDetailMessage)]
        TextView refreshBillingDetailMessage;

        [BindView(Resource.Id.btnBillingDetailefresh)]
        Button btnBillingDetailefresh;

        private const string PAGE_ID = "ApplicationStatus";
        ISharedPreferences mPref;

        [OnClick(Resource.Id.btnPayBill)]
        void OnPayBill(object sender, EventArgs eventArgs)
        {

        }

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatusDetailPaymentDetailLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            TextViewUtils.SetMuseoSans300Typeface(accountPayAmountValue, refreshBillingDetailMessage);
            TextViewUtils.SetMuseoSans500Typeface(accountPayAmountLabel, accountPayAmountCurrency,
                btnPayBill, btnBillingDetailefresh);
            // TODO: ApplicationStatus MultiLingual
            btnPayBill.Text = GetLabelByLanguage("pay");
            mPref = PreferenceManager.GetDefaultSharedPreferences(this);
            Bundle extras = Intent.Extras;

            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
        }

        private void EnablePayBillButtons()
        {
            bool isPaymentButtonEnable = Utility.IsEnablePayment();
            btnPayBill.Enabled = isPaymentButtonEnable;
            if (isPaymentButtonEnable)
            {
                btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
            else
            {
                btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }

    }
}
