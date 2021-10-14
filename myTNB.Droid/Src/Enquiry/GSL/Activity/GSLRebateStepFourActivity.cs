using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
{
    public class GSLRebateStepFourActivity : BaseToolbarAppCompatActivity, GSLRebateStepFourContract.IView
    {
        [BindView(Resource.Id.gslStepFourbtnNext)]
        Button gslStepFourbtnNext;

        private GSLRebateStepFourContract.IUserActionsListener presenter;

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.GSLRebateStepFourView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public void SetPresenter(GSLRebateStepFourContract.IUserActionsListener userActionListener)
        {
            this.presenter = userActionListener;
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_HEADER_TITLE));

            //TextViewUtils.SetMuseoSans500Typeface(gslStepThreePageTitle, gslStepThreebtnNext, txtStepThreeUploadTitle);
            //TextViewUtils.SetTextSize12(gslStepThreePageTitle);
            //TextViewUtils.SetTextSize16(gslStepThreebtnNext, txtStepThreeUploadTitle); ;

            //var stepTitleString = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_STEP_TITLE), 3, 4);
            //gslStepThreePageTitle.Text = stepTitleString;

            //txtStepThreeUploadTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_UPLOAD_TITLE);
            //gslStepThreebtnNext.Text = Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.NEXT);

            //RenderUploadDocumentLayoutList();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public void UpdateButtonState(bool isEnabled)
        {
            gslStepFourbtnNext.Enabled = isEnabled;
            gslStepFourbtnNext.Background = ContextCompat.GetDrawable(this, isEnabled ? Resource.Drawable.green_button_background :
                Resource.Drawable.silver_chalice_button_background);
        }
    }
}
