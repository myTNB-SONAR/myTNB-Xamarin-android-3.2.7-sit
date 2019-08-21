using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IUsageHistoryApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v5/my_billingssp.asmx/GetAccountUsageHistoryForGraph")]
        [Post("/v6/mytnbappws.asmx/GetAccountUsage")]
        Task<UsageHistoryResponse> DoQuery([Body] UsageHistoryRequest usageHistoryRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetBillingAccountDetails")]
        Task<AccountDetailsResponse> GetDetailedAccount([Body] AccountDetailsRequest accountDetailResponse);

        //Smart Meter APi
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetSmartMeterAccountData")]
        Task<SMUsageHistoryResponse> DoSMQuery([Body] SMUsageHistoryRequest usageHistoryRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetSmartMeterAccountData_V3")]
        Task<SMUsageHistoryResponse> DoSMQueryV2([Body] SMUsageHistoryRequest usageHistoryRequest, CancellationToken cancellationToken);
        //[Post("/v5/my_billingssp.asmx/GetSmartMeterAccountData_V2")]
        //[Post("/v5/my_billingssp.asmx/GetSmartMeterAccountData_V3")]


        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SmartMeterHourHistoryGraphStatus")]
        Task<SMHourUsageHistoryResponse> DoSMHourQuery([Body] UsageHistoryRequest usageHistoryRequest, CancellationToken cancellationToken);
    }
}