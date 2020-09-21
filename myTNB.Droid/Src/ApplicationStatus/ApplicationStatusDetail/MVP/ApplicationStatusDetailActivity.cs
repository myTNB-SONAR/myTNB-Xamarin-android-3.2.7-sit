
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

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    [Activity(Label = "Application Details", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusDetailActivity : BaseActivityCustom
    {
        const string PAGE_ID = "ApplicationStatus";
        ApplicationStatusDetailProgressAdapter adapter;

        List<StatusTrackerDisplay> StatusTrackerList = new List<StatusTrackerDisplay>();
        RecyclerView.LayoutManager layoutManager;

        [BindView(Resource.Id.txtApplicationStatusMainTitle)]
        TextView txtApplicationStatusMainTitle;

        [BindView(Resource.Id.txtApplicationStatusSubTitle)]
        TextView txtApplicationStatusSubTitle;

        [BindView(Resource.Id.txtApplicationStatusUpdated)]
        TextView txtApplicationStatusUpdated;


        [BindView(Resource.Id.applicationStatusStatusListRecyclerView)]
        RecyclerView applicationStatusStatusListRecyclerView;
        





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

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            applicationStatusStatusListRecyclerView.SetLayoutManager(layoutManager);
            applicationStatusStatusListRecyclerView.SetAdapter(adapter);

            

                    
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

                        if (applicationDetailDisplay.ApplicationStatusDetail.StatusTracker.Count > 0)
                        {
                            adapter = new ApplicationStatusDetailProgressAdapter(this, StatusTrackerList);
                            applicationStatusStatusListRecyclerView.SetAdapter(adapter);
                            adapter.NotifyDataSetChanged();
                        }

                        txtApplicationStatusMainTitle.Text = applicationDetailDisplay.ApplicationStatusDetail.StatusDescription;
                        txtApplicationStatusSubTitle.Text = applicationDetailDisplay.ApplicationStatusDetail.StatusDescriptionDisplay;
                        txtApplicationStatusUpdated.Text = applicationDetailDisplay.ApplicationDetail.LastUpdatedDateDisplay;

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
