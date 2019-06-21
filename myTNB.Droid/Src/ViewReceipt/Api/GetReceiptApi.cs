﻿using myTNB_Android.Src.ViewReceipt.Model;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ViewReceipt.Api
{
    public interface GetReceiptApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_BillingSSP.asmx/GetReceipt")]
        Task<GetReceiptResponse> GetReceipt([Body] GetReceiptRequest getCards);
    }
}