using myTNB.AndroidApp.Src.ViewReceipt.Model;
using Refit;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.ViewReceipt.Api
{
    public interface GetMultiReceiptByTransId
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_BillingSSP.asmx/GetMultiReceiptByTransId")]
        Task<GetMultiReceiptByTransIdResponse> GetMultiReceiptByTransId([Body] GetReceiptRequest getCards);
    }
}