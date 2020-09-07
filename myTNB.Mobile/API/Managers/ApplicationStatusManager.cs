using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API;
using myTNB.Mobile.API.Services.ApplicationStatus;
using Refit;

namespace myTNB.Mobile
{
    public sealed class ApplicationStatusManager
    {
        private static readonly Lazy<ApplicationStatusManager> lazy =
            new Lazy<ApplicationStatusManager>(() => new ApplicationStatusManager());

        public static ApplicationStatusManager Instance { get { return lazy.Value; } }
        public ApplicationStatusManager() { }

        #region GetApplicationStatusMetaData
        public async Task<GetApplicationStatusMetadataResponse> GetApplicationStatusMetaData(object userInfo, object deviceInfo)
        {
            try
            {
                var service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                var requestParameter = new GetApplicationStatusMetadataRequest
                {
                    UserInfo = userInfo,
                    DeviceInfo = deviceInfo
                };

                try
                {
                    return await service.GetApplicationStatusMetaData(requestParameter, NetworkService.GetCancellationToken());
                }
                catch (ApiException apiEx)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG || MASTER
                Debug.WriteLine(e.Message);
#endif
            }
            return new GetApplicationStatusMetadataResponse();
        }
        #endregion
        #region GetApplicationStatusDetails
        public async Task<GetApplicationStatusDetailsResponse> GetApplicationStatusDetails(object userInfo, object deviceInfo, string refCode, string id)
        {
            try
            {
                var service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                var requestParameter = new GetApplicationStatusDetailsRequest
                {
                    UserInfo = userInfo,
                    DeviceInfo = deviceInfo,
                    RefCode = refCode,
                    ID = id
                };

                try
                {
                    return await service.GetApplicationStatusDetails(requestParameter, NetworkService.GetCancellationToken());
                }
                catch (ApiException apiEx)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG || MASTER
                Debug.WriteLine(e.Message);
#endif
            }
            return new GetApplicationStatusDetailsResponse();
        }
        #endregion
        #region GetSavedProjectStatus
        public async Task<GetSavedProjectStatusResponse> GetSavedProjectStatus(object userInfo, object deviceInfo, int pageNumber, List<string> accountNumbers)
        {
            try
            {
                var service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                var requestParameter = new GetSavedProjectStatusRequest
                {
                    UserInfo = userInfo,
                    DeviceInfo = deviceInfo,
                    PageNumber = pageNumber,
                    AccountNumbers = accountNumbers
                };

                try
                {
                    return await service.GetSavedProjectStatus(requestParameter, NetworkService.GetCancellationToken());
                }
                catch (ApiException apiEx)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG || MASTER
                Debug.WriteLine(e.Message);
#endif
            }
            return new GetSavedProjectStatusResponse();
        }
        #endregion
        #region GetSavedApplicationStatus
        public async Task<GetSavedApplicationStatusResponse> GetSavedApplicationStatus(object userInfo, object deviceInfo, int pageNumber, List<string> accountNumbers)
        {
            try
            {
                var service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                var requestParameter = new GetSavedApplicationStatusRequest
                {
                    UserInfo = userInfo,
                    DeviceInfo = deviceInfo,
                    PageNumber = pageNumber,
                    AccountNumbers = accountNumbers
                };

                try
                {
                    return await service.GetSavedApplicationStatus(requestParameter, NetworkService.GetCancellationToken());
                }
                catch (ApiException apiEx)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG || MASTER
                Debug.WriteLine(e.Message);
#endif
            }
            return new GetSavedApplicationStatusResponse();
        }
        #endregion
        #region SearchApplicationStatus
        public async Task<SearchApplicationStatusResponse> SearchApplicationStatus(object userInfo, object deviceInfo, string referenceCode, string typeCode, string id)
        {
            try
            {
                var service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                var requestParameter = new SearchApplicationStatusRequest
                {
                    UserInfo = userInfo,
                    DeviceInfo = deviceInfo,
                    RefCode = referenceCode,
                    TypeCode = typeCode,
                    ID = id
                };

                try
                {
                    return await service.SearchApplicationStatus(requestParameter, NetworkService.GetCancellationToken());
                }
                catch (ApiException apiEx)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG || MASTER
                Debug.WriteLine(e.Message);
#endif
            }
            return new SearchApplicationStatusResponse();
        }
        #endregion
        #region PatchUserApplicationStatus
        public async Task<PatchUserApplicationStatusResponse> PatchUserApplicationStatus(object userInfo, object deviceInfo, string type, string idtypeCode, string id)
        {
            try
            {
                var service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                var requestParameter = new PatchUserApplicationStatusRequest
                {
                    UserInfo = userInfo,
                    DeviceInfo = deviceInfo,
                    Type = type,
                    IdTypeCode = idtypeCode,
                    ID = id
                };

                try
                {
                    return await service.PatchUserApplicationStatus(requestParameter, NetworkService.GetCancellationToken());
                }
                catch (ApiException apiEx)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG || MASTER
                Debug.WriteLine(e.Message);
#endif
            }
            return new PatchUserApplicationStatusResponse();
        }
        #endregion

        #region SearchApplicationType
        public async Task<SearchApplicationTypeResponse> SearchApplicationType(object userInfo
            , object deviceInfo
            , string roleID
            , string userID
            , string userName)
        {
            try
            {
                var service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                var requestParameter = new SearchApplicationTypeRequest
                {
                    UserInfo = userInfo,
                    DeviceInfo = deviceInfo
                };
                try
                {
                    SearchApplicationTypeResponse response = await service.SearchApplicationType(//requestParameter
                         roleID
                        , userID
                        , userName
                        , NetworkService.GetCancellationToken());

                    ServiceStatus stats = ServiceMappingManager.Instance.GetStatusDetails("SearchApplicationType", response.StatusDetail.Code);

                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG || MASTER
                    Debug.WriteLine("Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG || MASTER
                    Debug.WriteLine("General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG || MASTER
                Debug.WriteLine(e.Message);
#endif
            }
            return new SearchApplicationTypeResponse();
        }
        #endregion
    }
}