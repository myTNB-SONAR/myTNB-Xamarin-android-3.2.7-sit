using System;
using System.Threading.Tasks;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using Refit;

namespace myTNB_Android.Src.MultipleAccountPayment.Api
{
    public interface MPGetAccountsDueAmountApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetMultiAccountDueAmount")]
        Task<MPAccountDueResponse> GetMultiAccountDueAmount([Body] MPAccountDueRequest getCards);
    }
}
