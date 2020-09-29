
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.Adapter;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.FindUs.MVP;
using myTNB_Android.Src.FindUs.Response;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.MVP
{
    [Activity(Label = "ApplicationDetailActivityLogActivity",Theme = "@style/Theme.RegisterForm")]
    public class ApplicationDetailActivityLogActivity : BaseToolbarAppCompatActivity
    {

        [BindView(Resource.Id.layout_activitylog)]
        RecyclerView layout_activitylog;

        List<ApplicationActivityLogDetail> applicationActivityLogDetail;
        ApplicationDetailActivityAdapter applicationDetailActivityAdapter;

        RecyclerView.LayoutManager layoutManager;

        public bool IsActive()
        {
            return true;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationDetailActivityLogLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true; 
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //layoutManagerService = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("applicationActivityLogDetail"))
                {

                    applicationActivityLogDetail = new List<ApplicationActivityLogDetail>();
                    applicationActivityLogDetail = JsonConvert.DeserializeObject<List<ApplicationActivityLogDetail>>(extras.GetString("applicationActivityLogDetail"));

                    if (applicationActivityLogDetail != null && applicationActivityLogDetail.Count > 0)
                    {
                        applicationActivityLogDetail.Add(applicationActivityLogDetail[0]);
                        applicationDetailActivityAdapter = new ApplicationDetailActivityAdapter(this, applicationActivityLogDetail);

                        layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                        layout_activitylog.SetLayoutManager(layoutManager);
                        layout_activitylog.SetAdapter(applicationDetailActivityAdapter);

                        applicationDetailActivityAdapter.NotifyDataSetChanged();
                    }
                }
            }
            // Create your application here
        }
        public override void OnBackPressed()
        {
            Finish();
        }
    }
}
