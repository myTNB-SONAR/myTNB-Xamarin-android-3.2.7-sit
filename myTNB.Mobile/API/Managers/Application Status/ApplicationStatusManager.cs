using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API;
using myTNB.Mobile.API.Managers.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication;
using myTNB.Mobile.API.Services.ApplicationStatus;
using myTNB.Mobile.Extensions;
using Refit;

namespace myTNB.Mobile
{
    public sealed class ApplicationStatusManager
    {
        private static readonly Lazy<ApplicationStatusManager> lazy =
            new Lazy<ApplicationStatusManager>(() => new ApplicationStatusManager());

        public static ApplicationStatusManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public ApplicationStatusManager()
        {
        }

        #region SearchApplicationType
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="roleID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<SearchApplicationTypeResponse> SearchApplicationType(string roleID
            , string userID
            , string userName)
        {
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    SearchApplicationTypeResponse response = await service.SearchApplicationType(AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken());
                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails("default");
                    }
                    ApplicationStatusUtility.SearchApplicationTypeParser(ref response
                        , roleID);
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SearchApplicationType]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SearchApplicationType]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            SearchApplicationTypeResponse res = new SearchApplicationTypeResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails("default");
            return res;
        }
        #endregion

        #region GetApplicationStatus
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationType"></param>
        /// <param name="searchType"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<GetApplicationStatusResponse> GetApplicationStatus(string applicationType
            , string searchType
            , string searchTerm)
        {
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    GetApplicationStatusResponse response = await service.GetApplicationStatus(applicationType
                        , searchType
                        , searchTerm
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken());

                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails("default");
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationStatus]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationStatus]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            GetApplicationStatusResponse res = new GetApplicationStatusResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails("default");
            return res;
        }
        #endregion

        #region SaveApplication
        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceNo"></param>
        /// <param name="moduleName"></param>
        /// <param name="srNo"></param>
        /// <param name="srType"></param>
        /// <param name="statusCode"></param>
        /// <param name="srCreatedDate"></param>
        /// <returns></returns>
        public async Task<SaveApplicationResponse> SaveApplication(string referenceNo
            , string moduleName
            , string srNo
            , string srType
            , string statusCode
            , string srCreatedDate)
        {
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    SaveApplicationResponse response = await service.SaveApplication(new SaveApplicationRequest
                    {
                        SaveApplication = new SaveApplication
                        {
                            ReferenceNo = referenceNo,
                            ModuleName = moduleName,
                            SrNo = srNo,
                            SrType = srType,
                            StatusCode = statusCode,
                            SrCreatedDate = srCreatedDate
                        }
                    }
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken());

                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails("default");
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationStatus]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationStatus]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            SaveApplicationResponse res = new SaveApplicationResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails("default");
            return res;
        }
        #endregion
    }
}