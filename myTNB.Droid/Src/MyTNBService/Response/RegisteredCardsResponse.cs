using System;
using System.Collections.Generic;
using myTNB_Android.Src.MakePayment.Models;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class RegisteredCardsResponse : BaseResponse<List<CreditCard>>
    {
        public List<CreditCard> GetData()
        {
            return Response.Data;
        }
    }
}
