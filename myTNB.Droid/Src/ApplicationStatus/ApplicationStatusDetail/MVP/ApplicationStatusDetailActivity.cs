
using Android.App;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Activity;
using myTNB.Mobile;
using System.Collections.Generic;
using Newtonsoft.Json;
using myTNB.Mobile.API.Models.ApplicationStatus;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    [Activity(Label = "Application Details", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusDetailActivity : BaseActivityCustom
    {
        const string PAGE_ID = "ApplicationStatus";

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
                        GetApplicationStatusModel searhApplicationTypeModels = new GetApplicationStatusModel();

                        var test = extras.GetString("applicationStatusResponse");
                        searhApplicationTypeModels = JsonConvert.DeserializeObject<GetApplicationStatusModel>(extras.GetString("applicationStatusResponse"));

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
