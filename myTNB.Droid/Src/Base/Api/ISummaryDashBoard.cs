using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.SummaryDashBoard.Models;
using Refit;

namespace myTNB_Android.Src.SummaryDashBoard.API
{
    public interface ISummaryDashBoard
    {

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetLinkedAccountsSummaryInfo")]
        Task<SummaryDashBoardResponse> GetLinkedAccountsSummaryInfo([Body] SummaryDashBordRequest request, CancellationToken cancellationToken);
    }
}
