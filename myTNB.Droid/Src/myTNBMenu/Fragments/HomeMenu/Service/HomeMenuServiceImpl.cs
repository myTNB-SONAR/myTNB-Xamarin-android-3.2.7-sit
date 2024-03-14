using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.AppLaunch.Requests;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.Api;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using myTNB.Android.Src.SummaryDashBoard.API;
using myTNB.Android.Src.SummaryDashBoard.Models;
using myTNB.Android.Src.Utils;
using Refit;

namespace myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.Service
{
    public class HomeMenuServiceImpl : HomeMenuContract.IHomeMenuService
    {
        ISummaryDashBoard summaryDashboardInfoApi = null;
        IGetServiceApi getServiceApi = null;
        IAccountsSMRStatusApi getSMRAccountStatusApi = null;
        IGetIsSmrApplyAllowedApi getIsSmrApplyAllowedApi = null;

        public HomeMenuServiceImpl()
        {
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            summaryDashboardInfoApi = RestService.For<ISummaryDashBoard>(httpClient);
            getServiceApi = RestService.For<IGetServiceApi>(httpClient);
            getSMRAccountStatusApi = RestService.For<IAccountsSMRStatusApi>(httpClient);
            getIsSmrApplyAllowedApi = RestService.For<IGetIsSmrApplyAllowedApi>(httpClient);
#else
            summaryDashboardInfoApi = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
            getServiceApi = RestService.For<IGetServiceApi>(Constants.SERVER_URL.END_POINT);
            getSMRAccountStatusApi = RestService.For<IAccountsSMRStatusApi>(Constants.SERVER_URL.END_POINT);
            getIsSmrApplyAllowedApi = RestService.For<IGetIsSmrApplyAllowedApi>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<SummaryDashBoardResponse> GetLinkedSummaryInfo(SummaryDashBordRequest request, System.Threading.CancellationToken token)
        {
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return summaryDashboardInfoApi.GetLinkedAccountsSummaryInfo(encryptedRequest, token);
        }

        public Task<GetServicesResponse> GetServices(GetServiceRequests request)
        {
            return getServiceApi.GetService(request, new System.Threading.CancellationToken());
        }

        public Task<AccountSMRStatusResponse> GetSMRAccountStatus(AccountsSMRStatusRequest request)
        {
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return getSMRAccountStatusApi.AccountsSMRStatusApi(encryptedRequest, new System.Threading.CancellationToken());
        }

        public Task<GetIsSmrApplyAllowedResponse> GetIsSmrApplyAllowed(GetIsSmrApplyAllowedRequest request)
        {
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return getIsSmrApplyAllowedApi.GetIsSmrApplyAllowed(encryptedRequest, new System.Threading.CancellationToken());
        }

        public Task<SummaryDashBoardResponse> GetLinkedSummaryInfoQuery(SummaryDashBordRequest request, System.Threading.CancellationToken token)
        {
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return summaryDashboardInfoApi.GetLinkedAccountsSummaryInfo(encryptedRequest, token);
        }
    }
}
