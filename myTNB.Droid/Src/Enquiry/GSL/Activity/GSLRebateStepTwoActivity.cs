using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Enquiry.GSL.Fragment;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
{
    [Activity(Label = "GSL Rebate Step Two"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Enquiry")]
    public class GSLRebateStepTwoActivity : BaseToolbarAppCompatActivity, GSLRebateStepTwoContract.IView
    {
        [BindView(Resource.Id.incidentViewList)]
        readonly LinearLayout incidentViewList;

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
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
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
            GenerateDefaultIncident();
        }

        private void GenerateDefaultIncident()
        {
            GSLRebateIncidentItemListComponent incidentComponent = new GSLRebateIncidentItemListComponent(this, this);
            incidentComponent.SetSelectedDateTimeAction(GetSelectedDate);
            incidentViewList.AddView(incidentComponent);
        }

        private void GetSelectedDate(GSLIncidentDateTimePicker picker, DateTime dateTime)
        {
            this.userActionsListener.SetIncidentData(picker, dateTime);
        }
    }
}
