using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.Mobile;
using myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.Adapter;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.MVP
{
    [Activity(Label = "ApplicationDetailActivityLogActivity", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.RegisterForm")]
    public class ApplicationDetailActivityLogActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.layout_activitylog)]
        RecyclerView layout_activitylog;

        List<ApplicationActivityLogDetailDisplay> applicationActivityLogDetail;
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
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "title"), FromHtmlOptions.ModeLegacy).ToString());
            }
            else
            {
                SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "title")).ToString());
            }
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("applicationActivityLogDetail"))
                {
                    applicationActivityLogDetail = new List<ApplicationActivityLogDetailDisplay>();
                    applicationActivityLogDetail = JsonConvert.DeserializeObject<List<ApplicationActivityLogDetailDisplay>>(extras.GetString("applicationActivityLogDetail"));

                    if (applicationActivityLogDetail != null && applicationActivityLogDetail.Count > 0)
                    {

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