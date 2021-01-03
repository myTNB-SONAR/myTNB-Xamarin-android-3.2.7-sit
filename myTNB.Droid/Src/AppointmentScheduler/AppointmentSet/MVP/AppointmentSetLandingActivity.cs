
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;


using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB;
using myTNB.Mobile;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB_Android.Src.AppointmentScheduler.AppointmentSetLanding.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using Newtonsoft.Json;

namespace myTNB_Android.Src.AppointmentScheduler.AAppointmentSetLanding.MVP
{
    [Activity(Label = "Application Set", Theme = "@style/Theme.AppointmentScheduler")]
    public class AppointmentSetLandingActivity : BaseAppCompatActivity, AppointmentSetLandingContract.IView
    {
        AppointmentSetLandingPresenter mPresenter;
        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;
        [BindView(Resource.Id.txtMessageInfo)]
        TextView txtMessageInfo;
        [BindView(Resource.Id.appointmentLabel)]
        TextView appointmentLabel;
        [BindView(Resource.Id.appointmentTimeLabel)]
        TextView appointmentTimeLabel;
        [BindView(Resource.Id.appointmentValue)]
        TextView appointmentValue;
        [BindView(Resource.Id.appointmentTimeValue)]
        TextView appointmentTimeValue;
        [BindView(Resource.Id.premiseLabel)]
        TextView premiseLabel;
        [BindView(Resource.Id.premiseaddresstext)]
        TextView premiseaddresstext;
        [BindView(Resource.Id.servicerequest)]
        TextView servicerequest;
        [BindView(Resource.Id.servicerequestLabel)]
        TextView servicerequestLabel;
        [BindView(Resource.Id.btnTrackApplication)]
        Button btnTrackApplication;
        [BindView(Resource.Id.btnAddtoCalendar)]
        readonly Button btnAddtoCalendar;

        [BindView(Resource.Id.rootview)]
        RelativeLayout rootview;


        private Snackbar mNoInternetSnackbar;
        

        string srnumber;
        string selecteddate;
        string timeslot;
        string appointment;
        private GetApplicationStatusDisplay applicationDetailDisplay;

        public override int ResourceId()
        {
            return Resource.Layout.AppointmentSetLandingLayout;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new AppointmentSetLandingPresenter(this);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                txtTitleInfo.Text =  Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "title"), FromHtmlOptions.ModeLegacy).ToString();
            }
            else
            {
                txtTitleInfo.Text = Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "title")).ToString();
            }
          

            appointmentTimeLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "timeTitle").ToUpper();
            appointmentLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "dateTitle").ToUpper();
            premiseLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addressTitle").ToUpper();
            servicerequestLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "srTitle").ToUpper();
            btnTrackApplication.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "viewDetails").ToUpper();
            btnAddtoCalendar.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addToCalendar");

            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfo, txtTitleInfo, servicerequestLabel, servicerequest, appointmentLabel, appointmentTimeLabel, appointmentValue, appointmentTimeValue, premiseLabel, premiseaddresstext);
            TextViewUtils.SetMuseoSans500Typeface(btnTrackApplication, btnAddtoCalendar);

            btnTrackApplication.Click += GetApplication;

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                srnumber = extras.GetString("srnumber"); 
                selecteddate = extras.GetString("selecteddate");
                timeslot = extras.GetString("timeslot");
                appointment = extras.GetString("appointment");
                applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));
                appointmentValue.Text = selecteddate;
                appointmentTimeValue.Text = timeslot;
                servicerequest.Text = srnumber;
                if (appointment == "Reschedule")
                {
                    txtMessageInfo.Text = string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "rescheduleDetails"),selecteddate);
                   
                }
                else
                {
                    txtMessageInfo.Text = string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "appointmentDetails"),selecteddate);
                }

            }
        }
       private void GetApplication(object sender, EventArgs args)
        {
            GetApplicationStatus();
        }
        private async void GetApplicationStatus()
        {
            try
            {
                if (ConnectionUtils.HasInternetConnection(this))
                {

                    ShowProgressDialog();

                   
                    ApplicationDetailDisplay response = await ApplicationStatusManager.Instance.GetApplicationDetail(string.Empty
                            , applicationDetailDisplay.ApplicationDetail.ApplicationId
                            , applicationDetailDisplay.ApplicationTypeCode
                            , applicationDetailDisplay.System);

                    HideProgressDialog();
                    if (!response.StatusDetail.IsSuccess)
                    {
                        ShowApplicaitonPopupMessage(this, response.StatusDetail);
                    }
                    else
                    {
                        Intent applicationStatusDetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                        applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(response.Content));
                        StartActivityForResult(applicationStatusDetailIntent, Constants.APPLICATION_STATUS_DETAILS_REMOVE_REQUEST_CODE);
                        SetResult(Result.Ok, applicationStatusDetailIntent);
                        Finish();
                    }
                }
                else
                {
                    ShowNoInternetSnackbar();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public async void ShowApplicaitonPopupMessage(Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();

        }
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootview, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
            this.SetIsClicked(false);
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
        public void UpdateUI()
        {
            throw new NotImplementedException();
        }
    }
}