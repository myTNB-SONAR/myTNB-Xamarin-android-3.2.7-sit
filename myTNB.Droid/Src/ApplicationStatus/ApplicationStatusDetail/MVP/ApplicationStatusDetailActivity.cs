
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

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    [Activity(Label = "Application Details", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusDetailActivity : BaseActivityCustom
    {
        const string PAGE_ID = "ApplicationStatus";
        ApplicationStatusDetailProgressAdapter adapter;
        ApplicationStatusDetailSubDetailAdapter subAdapter;
        RecyclerView.LayoutManager layoutManager;

        [BindView(Resource.Id.txtApplicationStatusMainTitle)]
        TextView txtApplicationStatusMainTitle;

        [BindView(Resource.Id.txtApplicationStatusSubTitle)]
        TextView txtApplicationStatusSubTitle;

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
            //Todo SaveApplication
        }
        private async void SaveApplication()
        {
            //Todo SaveApplication


            //PostSaveApplicationResponse postSaveApplicationResponse = await ApplicationStatusManager.Instance.SaveApplication(
            //      referenceNo
            //    , moduleName
            //    , srNo
            //    , srType
            //    , statusCode
            //    , srCreatedDate);

           

            
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
                        GetApplicationStatusDisplay applicationDetailDisplay = new GetApplicationStatusDisplay();
                        applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationStatusResponse"));

                        if (applicationDetailDisplay != null
                            && applicationDetailDisplay.ApplicationStatusDetail != null
                            && applicationDetailDisplay.ApplicationStatusDetail.StatusTracker != null
                            && applicationDetailDisplay.ApplicationStatusDetail.StatusTracker.Count > 0)
                        {
                            adapter = new ApplicationStatusDetailProgressAdapter(this, applicationDetailDisplay.ApplicationStatusDetail.StatusTracker);
                            applicationStatusStatusListRecyclerView.SetAdapter(adapter);
                            adapter.NotifyDataSetChanged();
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

                        TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusMainTitle);
                        TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusSubTitle);
                        TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusUpdated);


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
