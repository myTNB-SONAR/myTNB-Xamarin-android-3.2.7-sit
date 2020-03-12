using System;
using System.Collections.Generic;
using myTNB_Android.Src.FindUs.Models;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class GetLocationListByKeywordResponse : BaseResponse<List<LocationData>>
    {
        public List<LocationData> GetData()
        {
            return Response.Data;
        }
    }
}
