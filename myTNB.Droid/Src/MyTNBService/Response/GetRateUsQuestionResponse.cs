using System;
using System.Collections.Generic;
using myTNB_Android.Src.Rating.Model;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class GetRateUsQuestionResponse : BaseResponse<List<RateUsQuestion>>
    {
        public List<RateUsQuestion> GetData()
        {
            return Response.Data;
        }
    }
}
