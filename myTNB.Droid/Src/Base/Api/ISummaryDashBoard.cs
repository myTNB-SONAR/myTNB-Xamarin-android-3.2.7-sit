﻿using myTNB_Android.Src.SummaryDashBoard.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.SummaryDashBoard.API
{
    public interface ISummaryDashBoard
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountsBillSummary")]
        Task<SummaryDashBoardResponse> GetLinkedAccountsSummaryInfo([Body] SummaryDashBordRequest request, CancellationToken cancellationToken);
    }
}
