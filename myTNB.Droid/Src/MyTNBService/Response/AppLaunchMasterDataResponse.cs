using System;
using System.Collections.Generic;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.FindUs.Models;
using myTNB_Android.Src.FindUs.Response;
using myTNB_Android.Src.MyTNBService.Model;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class AppLaunchMasterDataResponse : BaseResponse<AppLaunchMasterDataModel>
    {
        public AppLaunchMasterDataModel GetData()
        {
            return Response.Data;
        }
    }
}
