using myTNB_Android.Src.MultipleAccountPayment.Model;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.MultipleAccountPayment.Api
{
    public interface MPGetAccountsDueAmountApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetMultiAccountDueAmount")]
        //[Post("/v5/my_billingssp.asmx/GetMultiAccountDueAmountV2")]
        Task<MPAccountDueResponse> GetMultiAccountDueAmount([Body] MPAccountDueRequest getCards);
    }
}
