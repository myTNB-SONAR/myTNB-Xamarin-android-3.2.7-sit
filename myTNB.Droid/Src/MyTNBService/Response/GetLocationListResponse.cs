using System;
using System.Collections.Generic;
using myTNB.Android.Src.FindUs.Models;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class GetLocationListResponse : BaseResponse<List<LocationData>>
    {
        public List<LocationData> GetData()
        {
            return Response.Data;
        }
    }
}
