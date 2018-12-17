using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refit;
using myTNB_Android.Src.myTNBMenu.Models;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Requests;
using System.Threading;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.AddAccount.Models;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IUsageHistoryApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetAccountUsageHistoryForGraph")]
        Task<UsageHistoryResponse> DoQuery([Body] UsageHistoryRequest usageHistoryRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetBillingAccountDetails")]
        Task<AccountDetailsResponse> GetDetailedAccount([Body] AccountDetailsRequest accountDetailResponse);

        //Smart Meter APi
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetSmartMeterAccountData")]
        Task<SMUsageHistoryResponse> DoSMQuery([Body] SMUsageHistoryRequest usageHistoryRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetSmartMeterAccountData_V2")]
        Task<SMUsageHistoryResponse> DoSMQueryV2([Body] SMUsageHistoryRequest usageHistoryRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SmartMeterHourHistoryGraphStatus")]
        Task<SMHourUsageHistoryResponse> DoSMHourQuery([Body] UsageHistoryRequest usageHistoryRequest, CancellationToken cancellationToken);
    }
}