using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.FindUs.Models;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class GetLocationListByKeywordResponse : BaseResponse<List<LocationData>>
    {
        public List<LocationData> GetData()
        {
            return Response.Data;
        }
    }
}
