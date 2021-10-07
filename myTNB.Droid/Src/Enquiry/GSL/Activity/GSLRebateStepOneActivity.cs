using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
{
    [Activity(Label = "GSL Rebate Step One", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class GSLRebateStepOneActivity : BaseToolbarAppCompatActivity, GSLRebateStepOneContract.IView
    {
        [BindView(Resource.Id.gslStepOnePageTitle)]
        TextView gslStepOnePageTitle;

        [BindView(Resource.Id.tenantGSLMobileNumberContainer)]
        LinearLayout tenantGSLMobileNumberContainer;

        private GSLRebateStepOneContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _ = new GSLRebateStepOnePresenter(this);
            this.userActionsListener?.OnInitialize();
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_HEADER_TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            TextViewUtils.SetMuseoSans500Typeface(gslStepOnePageTitle);
            TextViewUtils.SetTextSize12(gslStepOnePageTitle);
            var stepTitleString = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_STEP_TITLE), 1, 2);
            gslStepOnePageTitle.Text = stepTitleString;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.GSLRebateStepOneView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public void SetPresenter(GSLRebateStepOneContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }
    }
}
