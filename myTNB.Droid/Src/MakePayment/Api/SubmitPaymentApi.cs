﻿using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.MakePayment.Requests;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.MakePayment.Api
{
    public interface SubmitPaymentApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v4/my_billingssp.asmx/SubmitPaymentPG")]
        Task<SubmitPaymentResponse> SubmitPayment([Body] SubmitPaymentRequestPG submitPaymentRequest);
    }
}
