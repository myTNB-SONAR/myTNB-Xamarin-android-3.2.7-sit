﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.MyTNBService.InterfaceAPI;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.MyTNBService.ServiceImpl
{
    public class ServiceApiImpl
    {
        private static readonly Lazy<ServiceApiImpl>
            lazy = new Lazy<ServiceApiImpl>(() => new ServiceApiImpl());
        private IServiceV6 api;
        HttpClient httpClient;

        private ServiceApiImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<IServiceV6>(httpClient);
#else
            api = RestService.For<IServiceV6>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public static ServiceApiImpl Instance { get { return lazy.Value; } }

        /// <summary>
        /// Call GetAppLaunchMasterData with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AppLaunchMasterDataResponse> GetAppLaunchMasterData([Body] Request.BaseRequest request)
        {
            return api.GetAppLaunchMasterData<AppLaunchMasterDataResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }
        /// <summary>
        /// Call GetAppLaunchMasterData with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<AppLaunchMasterDataResponse> GetAppLaunchMasterData([Body] Request.BaseRequest request, CancellationToken token)
        {
            return api.GetAppLaunchMasterData<AppLaunchMasterDataResponse>(request, token);
        }

        /// <summary>
        /// Call GetAccounts with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CustomerAccountListResponse> GetCustomerAccountList([Body] Request.BaseRequest request)
        {
            return api.GetCustomerAccountList<CustomerAccountListResponse>(request,CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccounts with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<CustomerAccountListResponse> GetCustomerAccountList([Body] Request.BaseRequest request, CancellationToken token)
        {
            return api.GetCustomerAccountList<CustomerAccountListResponse>(request, token);
        }

        /// <summary>
        /// Call AddAccountToCustomer with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AccountToCustomerResponse> AddAccountToCustomer([Body] Request.BaseRequest request)
        {
            return api.AddAccountToCustomer<AccountToCustomerResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ValidateManualAccount with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ValidateManualAccountResponse> ValidateManualAccount([Body] Request.BaseRequest request)
		{
			return api.ValidateManualAccount<ValidateManualAccountResponse>(request, CancellationTokenSourceWrapper.GetToken());
		}
	}
}
