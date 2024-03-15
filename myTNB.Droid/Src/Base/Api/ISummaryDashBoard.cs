using myTNB.Mobile.Business;
using myTNB.AndroidApp.Src.SummaryDashBoard.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.SummaryDashBoard.API
{
    public interface ISummaryDashBoard
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountsBillSummary")]
        Task<SummaryDashBoardResponse> GetLinkedAccountsSummaryInfo([Body] EncryptedRequest request, CancellationToken cancellationToken);
    }
}
