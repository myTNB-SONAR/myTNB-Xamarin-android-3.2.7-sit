
using Android.App;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Activity;
using myTNB.Mobile;
using System.Collections.Generic;
using Newtonsoft.Json;
using myTNB.Mobile.API.Models.ApplicationStatus;
using CheeseBind;
using Android.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Adapter;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Models;
using System;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.PreLogin.Activity;
using Android.Preferences;
using AndroidX.Core.Content;
using Android.Views;
using myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.MVP;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    [Activity(Label = "Application Details", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusDetailActivity : BaseActivityCustom
    {
        const string PAGE_ID = "ApplicationStatus";
        ApplicationStatusDetailProgressAdapter adapter;
        ApplicationStatusDetailSubDetailAdapter subAdapter;
        RecyclerView.LayoutManager layoutManager;
        GetApplicationStatusDisplay applicationDetailDisplay; 

        [BindView(Resource.Id.txtApplicationStatusMainTitle)]
        TextView txtApplicationStatusMainTitle;

        [BindView(Resource.Id.txtApplicationStatusSubTitle)]
        TextView txtApplicationStatusSubTitle;
        [BindView(Resource.Id.txtApplicationStatusTitle)]
        TextView txtApplicationStatusTitle;

        

        [BindView(Resource.Id.txtApplicationStatusUpdated)]
        TextView txtApplicationStatusUpdated;


        [BindView(Resource.Id.applicationStatusStatusListRecyclerView)]
        RecyclerView applicationStatusStatusListRecyclerView;

        [BindView(Resource.Id.applicationStatusAdditionalListRecyclerView)]
        RecyclerView applicationStatusAdditionalListRecyclerView;

        [BindView(Resource.Id.applicationStatusBotomPayableLayout)]
        LinearLayout applicationStatusBotomPayableLayout;

        [BindView(Resource.Id.applicationStatusDetailDoubleButtonLayout)]
        LinearLayout applicationStatusDetailDoubleButtonLayout;

        [BindView(Resource.Id.btnViewActivityLogLayout)]
        LinearLayout btnViewActivityLogLayout;

        [BindView(Resource.Id.applicationStatusLine)]
        View applicationStatusLine;
        

        [BindView(Resource.Id.txtApplicationStatusDetail)]
        TextView txtApplicationStatusDetail;

        [BindView(Resource.Id.txtApplicationStatusDetailNote)]
        TextView txtApplicationStatusDetailNote;

        [BindView(Resource.Id.btnSaveApplication)]
        Button btnSaveApplication;

        [BindView(Resource.Id.btnViewActivityLog)]
        Button btnViewActivityLog;

        [OnClick(Resource.Id.btnSaveApplication)]
        internal void OnSaveApplication(object sender, EventArgs e)
        {
            SaveApplication();

        }

        [OnClick(Resource.Id.btnViewActivityLog)]
        internal void OnViewActivityLog(object sender, EventArgs e)
        {
            ViewActivityLog();
        }

        private async void ViewActivityLog()
        {
            //Todo ViewActivityLog
            

            Intent applicationDetailActivityLogIntent = new Intent(this, typeof(ApplicationDetailActivityLogActivity));
            applicationDetailActivityLogIntent.PutExtra("applicationActivityLogDetail", JsonConvert.SerializeObject(applicationDetailDisplay.ApplicationActivityLogDetail));
            //statusLandingIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(applicationDetailActivityLogIntent);


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
        private async void SaveApplication()
        {
            ShowProgressDialog();
            PostSaveApplicationResponse postSaveApplicationResponse = await ApplicationStatusManager.Instance.SaveApplication(
                applicationDetailDisplay.ApplicationDetail.ReferenceNo
                , applicationDetailDisplay.ApplicationDetail.ApplicationModuleId
                , applicationDetailDisplay.ApplicationType
                , applicationDetailDisplay.ApplicationDetail.BackendReferenceNo
                , applicationDetailDisplay.ApplicationDetail.BackendApplicationType
                , applicationDetailDisplay.ApplicationDetail.BackendModule
                , applicationDetailDisplay.ApplicationDetail.StatusCode
                , applicationDetailDisplay.ApplicationDetail.CreatedDate.Value);

            HideProgressDialog();

            UserEntity loggedUser = UserEntity.GetActive();


            if (postSaveApplicationResponse.StatusDetail.IsSuccess && loggedUser != null)
            {
                Intent applicationLandingIntent = new Intent(this, typeof(ApplicationStatusLandingActivity));
                applicationLandingIntent.PutExtra("SaveApplication", JsonConvert.SerializeObject(postSaveApplicationResponse.StatusDetail));
                StartActivity(applicationLandingIntent);
            }
            else
            {
                if (loggedUser != null)
                {
                    MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                   .SetTitle(postSaveApplicationResponse.StatusDetail.Title)
                   .SetMessage(postSaveApplicationResponse.StatusDetail.Message)
                   .SetCTALabel(postSaveApplicationResponse.StatusDetail.PrimaryCTATitle)
                   .SetSecondaryCTALabel(postSaveApplicationResponse.StatusDetail.SecondaryCTATitle)
                   .SetSecondaryCTAaction(() => ShowStatusLanding())
                   .Build();
                    whereisMyacc.Show();
                }
                else
                {
                    MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                  .SetTitle(Utility.GetLocalizedLabel("ApplicationStatusDetails", "loginTitle"))
                  .SetMessage(Utility.GetLocalizedLabel("ApplicationStatusDetails", "loginMessage"))
                  .SetCTALabel(Utility.GetLocalizedLabel("ApplicationStatusDetails", "loginPrimaryCTA"))
                  .SetSecondaryCTALabel(Utility.GetLocalizedLabel("ApplicationStatusDetails", "loginSecondaryCTA"))
                  .SetSecondaryCTAaction(() => ShowPreLogin())
                  .Build();
                    whereisMyacc.Show();
                }

            }
        }

        public void ShowPreLogin()
        {
            Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
            PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(PreLoginIntent);
        }

        public void ShowStatusLanding()
        {
            Intent statusLandingIntent = new Intent(this, typeof(ApplicationStatusLandingActivity));
            //statusLandingIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            statusLandingIntent.PutExtra("ApplicationActivityLogDetail", JsonConvert.SerializeObject(applicationDetailDisplay.ApplicationActivityLogDetail));
            StartActivity(statusLandingIntent);
        }

        internal string test
        { set; private get; } = string.Empty;

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatusDetailLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //  TODO: ApplicationStatus Multilingual
            SetToolBarTitle("Application Details");
            applicationStatusDetailDoubleButtonLayout.Visibility = Android.Views.ViewStates.Gone;
            applicationStatusBotomPayableLayout.Visibility = Android.Views.ViewStates.Gone;
            btnSaveApplication.Visibility = Android.Views.ViewStates.Visible;
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            applicationStatusStatusListRecyclerView.SetLayoutManager(layoutManager);
            applicationStatusStatusListRecyclerView.SetAdapter(adapter);
            TextViewUtils.SetMuseoSans500Typeface(btnViewActivityLog);
            
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

            applicationStatusAdditionalListRecyclerView.SetLayoutManager(layoutManager);
            applicationStatusAdditionalListRecyclerView.SetAdapter(subAdapter);

            // Create your application here
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_DETAIL_TITLE_KEY))
                {
                    SetToolBarTitle(extras.GetString(Constants.APPLICATION_STATUS_DETAIL_TITLE_KEY));
                }
                if (extras != null)
                {
                    if (extras.ContainsKey("applicationStatusResponse"))
                    {
                        applicationDetailDisplay = new GetApplicationStatusDisplay();
                        applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationStatusResponse"));

                        if (applicationDetailDisplay != null
                            && applicationDetailDisplay.ApplicationStatusDetail != null
                            && applicationDetailDisplay.ApplicationStatusDetail.StatusTracker != null
                            && applicationDetailDisplay.ApplicationStatusDetail.StatusTracker.Count > 0)
                        {
                            applicationStatusLine.Visibility = ViewStates.Visible;
                            adapter = new ApplicationStatusDetailProgressAdapter(this, applicationDetailDisplay.ApplicationStatusDetail.StatusTracker, applicationDetailDisplay.ApplicationStatusDetail.IsPayment);
                            applicationStatusStatusListRecyclerView.SetAdapter(adapter);
                            adapter.NotifyDataSetChanged();
                        }
                        else
                        {
                            applicationStatusLine.Visibility = ViewStates.Gone;
                        }

                        if(applicationDetailDisplay != null
                            && applicationDetailDisplay.AdditionalInfoList != null
                            && applicationDetailDisplay.AdditionalInfoList.Count > 0)
                        {
                            subAdapter = new ApplicationStatusDetailSubDetailAdapter(this, applicationDetailDisplay.AdditionalInfoList);
                            applicationStatusAdditionalListRecyclerView.SetAdapter(subAdapter);
                            subAdapter.NotifyDataSetChanged();
                        }

                        if(applicationDetailDisplay != null && applicationDetailDisplay.IsActivityLogDisplayed)
                        {
                            btnViewActivityLogLayout.Visibility = Android.Views.ViewStates.Visible;
                        }
                        else
                        {
                            btnViewActivityLogLayout.Visibility = Android.Views.ViewStates.Visible;
                        }

                        txtApplicationStatusMainTitle.Text = applicationDetailDisplay.ApplicationStatusDetail.StatusDescription;
                        txtApplicationStatusSubTitle.Text = applicationDetailDisplay.ApplicationType;
                        txtApplicationStatusUpdated.Text = applicationDetailDisplay.ApplicationDetail.LastUpdatedDateDisplay;
                        if(applicationDetailDisplay.IsPortalMessageDisplayed)
                        {
                            txtApplicationStatusDetail.Text = applicationDetailDisplay.PortalMessage;
                            txtApplicationStatusDetail.Visibility = Android.Views.ViewStates.Visible;
                        }
                        else
                        {
                            txtApplicationStatusDetail.Visibility = Android.Views.ViewStates.Gone;
                        }

                        if(applicationDetailDisplay.IsSaveMessageDisplayed)
                        {
                            txtApplicationStatusDetailNote.Text = applicationDetailDisplay.SaveMessage;
                            txtApplicationStatusDetailNote.Visibility = Android.Views.ViewStates.Visible;
                        }
                        else
                        {
                            txtApplicationStatusDetailNote.Visibility = Android.Views.ViewStates.Gone;
                        }
                        if (applicationDetailDisplay != null && applicationDetailDisplay.StatusColor.Length <= 3)
                        {
                            Android.Graphics.Color color = Android.Graphics.Color.Rgb(
                                applicationDetailDisplay.StatusColor[0]
                                , applicationDetailDisplay.StatusColor[1]
                                , applicationDetailDisplay.StatusColor[2]);

                            txtApplicationStatusMainTitle.SetTextColor(color);
                        }

                       
                        TextViewUtils.SetMuseoSans500Typeface(txtApplicationStatusMainTitle);
                        TextViewUtils.SetMuseoSans500Typeface(txtApplicationStatusTitle);
                        TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusSubTitle);
                        //TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusUpdated);

                       


                        //foreach (var searchTypeItem in searhApplicationTypeModels)
                        //{
                        //    //mTypeList.Add(new TypeModel(searchTypeItem)
                        //    //{
                        //    //    SearchApplicationTypeId = searchTypeItem.SearchApplicationTypeId,
                        //    //    SearchApplicationTypeDesc = searchTypeItem.SearchApplicationTypeDesc,
                        //    //    SearchTypes = searchTypeItem.SearchTypes,
                        //    //    isChecked = false
                        //    //});
                        //}

                    }
                }

            }

        }
    }
}
