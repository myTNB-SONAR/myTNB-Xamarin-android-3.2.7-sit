using System;
using System.Threading.Tasks;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using Refit;

namespace myTNB_Android.Src.MultipleAccountPayment.Api
{
    public interface MPGetRegisteredCardsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetRegisteredCards")]
        Task<MPGetRegisteredCardsResponse> GetRegisteredCards([Body] MPGetRegisteredCardsRequest getCards);
    }
}
