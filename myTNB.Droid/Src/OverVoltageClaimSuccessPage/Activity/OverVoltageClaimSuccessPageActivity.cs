
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.OverVoltageFeedback;
using myTNB_Android.Src.OverVoltageFeedback.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.OverVoltageClaimSuccessPage.Activity
{
    [Activity(Label = "OverVoltageClaimSuccessPage"
          , ScreenOrientation = ScreenOrientation.Portrait
                  , WindowSoftInputMode = SoftInput.AdjustPan
          , Theme = "@style/Theme.FaultyStreetLamps")]
    public class OverVoltageClaimSuccessPageActivity : BaseAppCompatActivity
    {

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.txtFeedbackIdContent)]
        TextView txtFeedbackIdContent;

        [BindView(Resource.Id.txtFeedbackIdTitle)]
        TextView txtFeedbackIdTitle;

        [BindView(Resource.Id.buttonBackToHome)]
        Button buttonBackToHome;

        [BindView(Resource.Id.btnViewSubmitted)]
        Button btnViewSubmitted;

        string SerialNumber;

        public override int ResourceId()
        {
            return Resource.Layout.OverVoltageClaimSuccessPage;
        }


        public override void OnBackPressed()
        {
            try
            {
                Intent intent = new Intent(this, typeof(DashboardHomeActivity));
                // generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());           
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
            }
            catch (Exception ex)
            {

            }        

        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                // string SerialNumber =  Intent.GetStringExtra("SerialNumber");
                SerialNumber = Intent.GetStringExtra("SerialNumber");
                txtFeedbackIdContent.Text = SerialNumber;
                txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitICFeedback_OverVoltageClainSuccessPageThankyouRequest");
                txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitICFeedback_OverVoltageClainSuccessPageThankyouRequestContent");
                txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
                buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
                btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");
                SetUI();
            }
            catch (Exception ex)
            {

            }

        }

        private void SetUI()
        {
            //buttonBackToHome.Click += ButtonBackToHome_Click;
        }

      

        [OnClick(Resource.Id.buttonBackToHome)]
        void OnToHome(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(DashboardHomeActivity));            
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
        }

        [OnClick(Resource.Id.btnViewSubmitted)]
        void GoToClimDetail(object sender, EventArgs eventArgs)
        {
            GetClaimID();
            //Intent intent = new Intent(this, typeof(DashboardHomeActivity));
            //intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            //StartActivity(intent);
        }

        private async void GetClaimID()
        {
            try
            {
                var claimDetailResponce = await ServiceApiImpl.Instance.SubmittedFeedbackClaimIdDetail(new SubmittedFeedbeckClaimIdDetailRequestModel(SerialNumber));

                if (claimDetailResponce.d.data != null)
                {
                    var ClaimId = claimDetailResponce.d.data.ClaimId;
                    var othersIntent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));
                    othersIntent.PutExtra("TITLE", Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                    othersIntent.PutExtra("ClaimId", ClaimId);
                    StartActivity(othersIntent);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
