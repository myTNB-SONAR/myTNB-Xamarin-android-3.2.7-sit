using System;
using System.Threading.Tasks;
using myTNB_Android.Src.MakePayment.Models;
using Refit;

namespace myTNB_Android.Src.MakePayment.Api
{
    public interface GetRegisteredCardsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetRegisteredCards")]
        Task<GetRegisteredCardsResponse> GetRegisteredCards([Body] GetRegisteredCardsRequest getCards);
    }
}
