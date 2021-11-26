using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Enquiry.GSL.Fragment;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
{
    [Activity(Label = "GSL Rebate Step Two"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Enquiry")]
    public class GSLRebateStepTwoActivity : BaseToolbarAppCompatActivity, GSLRebateStepTwoContract.IView
    {
        [BindView(Resource.Id.gslStepTwoPageTitle)]
        TextView gslStepTwoPageTitle;

        [BindView(Resource.Id.incidentViewList)]
        readonly LinearLayout incidentViewList;

        [BindView(Resource.Id.gslStepTwobtnNext)]
        Button gslStepTwobtnNext;

        private GSLRebateStepTwoContract.IUserActionsListener userActionsListener;

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.GSLRebateStepTwoView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override bool CameraPermissionRequired()
        {
            return true;
        }

        public override bool StoragePermissionRequired()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                _ = new GSLRebateStepTwoPresenter(this);
                this.userActionsListener?.OnInitialize();

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(GSLRebateConstants.REBATE_MODEL))
                    {
                        var rebateModel = DeSerialze<GSLRebateModel>(extras.GetString(GSLRebateConstants.REBATE_MODEL));
                        this.userActionsListener.SetRebateModel(rebateModel);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(GSLRebateStepTwoContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_HEADER_TITLE));

            TextViewUtils.SetMuseoSans500Typeface(gslStepTwoPageTitle, gslStepTwobtnNext);
            TextViewUtils.SetTextSize12(gslStepTwoPageTitle);
            TextViewUtils.SetTextSize16(gslStepTwobtnNext);

            var stepTitleString = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_STEP_TITLE), 2, 4);
            gslStepTwoPageTitle.Text = stepTitleString;

            gslStepTwobtnNext.Text = Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.NEXT);

            GenerateDefaultIncident();
        }

        private void GenerateDefaultIncident()
        {
            GSLRebateIncidentItemListComponent incidentComponent = new GSLRebateIncidentItemListComponent(this, this);
            incidentComponent.SetItemIndex(0);
            incidentComponent.SetSelectedDateTimeAction(GetSelectedDate);
            incidentComponent.SetResetDateTimeValueAction(ResetDateTimeValue);
            incidentViewList.AddView(incidentComponent);
        }

        private void GetSelectedDate(GSLIncidentDateTimePicker picker, DateTime dateTime, int index)
        {
            this.userActionsListener.SetIncidentData(picker, dateTime, index);
        }

        private void ResetDateTimeValue(GSLIncidentDateTimePicker picker, int index)
        {
            this.userActionsListener.ResetIncidentData(picker, index);
        }

        public void UpdateButtonState(bool isEnabled)
        {
            gslStepTwobtnNext.Enabled = isEnabled;
            gslStepTwobtnNext.Background = ContextCompat.GetDrawable(this, isEnabled ? Resource.Drawable.green_button_background :
                Resource.Drawable.silver_chalice_button_background);
        }

        [OnClick(Resource.Id.gslStepTwobtnNext)]
        public void ButtonNextOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                if (this.userActionsListener.CheckDateTimeFields())
                {
                    OnShowGSLRebateStepThreeActivity();
                }
                else
                {
                    this.SetIsClicked(false);
                }
            }
        }

        private void OnShowGSLRebateStepThreeActivity()
        {
            this.SetIsClicked(true);
            Intent stepThreectivity = new Intent(this, typeof(GSLRebateStepThreeActivity));
            stepThreectivity.PutExtra(GSLRebateConstants.REBATE_MODEL, JsonConvert.SerializeObject(this.userActionsListener.GetGSLRebateModel()));
            StartActivity(stepThreectivity);
        }
    }
}
