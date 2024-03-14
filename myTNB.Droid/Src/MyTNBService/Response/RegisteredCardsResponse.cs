using System;
using System.Collections.Generic;
using myTNB.Android.Src.MultipleAccountPayment.Models;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class RegisteredCardsResponse : BaseResponse<List<CreditCard>>
    {
        public List<CreditCard> GetData()
        {
            return Response.Data;
        }
    }
}
