﻿using System;
using myTNB_Android.Src.MyTNBService.Response;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class GetPaymentReceiptResponse : BaseResponse<AccountReceiptResponse.MultiReceiptDetails>
    {
        public AccountReceiptResponse.MultiReceiptDetails GetData()
        {
            return Response.Data;
        }
    }
}
