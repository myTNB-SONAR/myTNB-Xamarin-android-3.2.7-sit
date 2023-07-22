using myTNB.Mobile.Business;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IUsageHistoryApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountUsage")]
        Task<UsageHistoryResponse> DoQuery([Body] EncryptedRequest usageHistoryRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountUsageSmart")]
        Task<SMUsageHistoryResponse> DoSMQueryV2([Body] EncryptedRequest usageHistoryRequest, CancellationToken cancellationToken);
    }
}