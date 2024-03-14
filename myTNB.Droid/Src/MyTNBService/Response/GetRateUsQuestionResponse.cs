using System;
using System.Collections.Generic;
using myTNB.Android.Src.Rating.Model;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class GetRateUsQuestionResponse : BaseResponse<List<RateUsQuestion>>
    {
        public List<RateUsQuestion> GetData()
        {
            return Response.Data;
        }
    }
}
