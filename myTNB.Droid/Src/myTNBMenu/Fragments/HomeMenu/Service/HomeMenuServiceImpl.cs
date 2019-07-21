using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.SummaryDashBoard.API;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Service
{
    public class HomeMenuServiceImpl : HomeMenuContract.IHomeMenuService
    {
        ISummaryDashBoard summaryDashboardInfoApi = null;
        public HomeMenuServiceImpl()
        {
#if STUB
            summaryDashboardInfoApi = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            summaryDashboardInfoApi = RestService.For<ISummaryDashBoard>(httpClient);
#elif DEVELOP
            summaryDashboardInfoApi = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#else
            summaryDashboardInfoApi = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<SummaryDashBoardResponse> GetLinkedSummaryInfo(SummaryDashBordRequest request)
        {
            return summaryDashboardInfoApi.GetLinkedAccountsSummaryInfo(request,new System.Threading.CancellationToken());
        }
    }
}
