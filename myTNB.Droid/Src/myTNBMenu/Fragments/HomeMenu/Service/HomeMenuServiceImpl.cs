using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using myTNB_Android.Src.SummaryDashBoard.API;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Service
{
    public class HomeMenuServiceImpl : HomeMenuContract.IHomeMenuService
    {
        ISummaryDashBoard summaryDashboardInfoApi = null;
        IGetServiceApi getServiceApi = null;
        IAccountsSMRStatusApi getSMRAccountStatusApi = null;

        public HomeMenuServiceImpl()
        {
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            summaryDashboardInfoApi = RestService.For<ISummaryDashBoard>(httpClient);
            getServiceApi = RestService.For<IGetServiceApi>(httpClient);
            getSMRAccountStatusApi = RestService.For<IAccountsSMRStatusApi>(httpClient);
#else
            summaryDashboardInfoApi = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
            getServiceApi = RestService.For<IGetServiceApi>(Constants.SERVER_URL.END_POINT);
            getSMRAccountStatusApi = RestService.For<IAccountsSMRStatusApi>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<SummaryDashBoardResponse> GetLinkedSummaryInfo(SummaryDashBordRequest request)
        {
            return summaryDashboardInfoApi.GetLinkedAccountsSummaryInfo(request,new System.Threading.CancellationToken());
        }

        public Task<GetServicesResponse> GetServices(GetServiceRequests request)
        {
            return getServiceApi.GetService(request, new System.Threading.CancellationToken());
        }

        public Task<AccountSMRStatusResponse> GetSMRAccountStatus(AccountsSMRStatusRequest request)
        {
            return getSMRAccountStatusApi.AccountsSMRStatusApi(request, new System.Threading.CancellationToken());
        }
    }
}
