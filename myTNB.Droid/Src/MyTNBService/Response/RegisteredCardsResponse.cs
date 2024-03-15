using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class RegisteredCardsResponse : BaseResponse<List<CreditCard>>
    {
        public List<CreditCard> GetData()
        {
            return Response.Data;
        }
    }
}
