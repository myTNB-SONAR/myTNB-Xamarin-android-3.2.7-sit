using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API;
using myTNB.Mobile.API.Managers.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication;
using myTNB.Mobile.API.Services.ApplicationStatus;
using myTNB.Mobile.Extensions;
using myTNB.Mobile.SessionCache;
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
                    if (response.Content != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails("default");
                    }
                    SearchTypeUtility.SearchApplicationTypeParser(ref response
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
        /// <param name="applicationTypeTitle"></param>
        /// <param name="searchTypeTitle"></param>
        /// <returns></returns>
        public async Task<ApplicationDetailDisplay> GetApplicationStatus(string applicationType
            , string searchType
            , string searchTerm
            , string applicationTypeTitle
            , string searchTypeTitle)
        {
            ApplicationDetailDisplay displaymodel = new ApplicationDetailDisplay();
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
                        if (response.Content == null)
                        {
                            response.StatusDetail.Code = "empty";
                        }
                        else
                        {
                            response.Parse(applicationType
                                , applicationTypeTitle
                                , searchTypeTitle
                                , searchTerm);
                        }
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails("default");
                    }
                    displaymodel = response.Parse(applicationType
                               , applicationTypeTitle
                               , searchTypeTitle
                               , searchTerm);
                    return displaymodel;
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
            displaymodel = new ApplicationDetailDisplay
            {
                StatusDetail = new StatusDetail()
            };
            displaymodel.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails("default");
            return displaymodel;
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
        public async Task<PostSaveApplicationResponse> SaveApplication(string referenceNo
            , string moduleName
            , string srNo
            , string srType
            , string statusCode
            , DateTime srCreatedDate)
        {
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    PostSaveApplication request = new PostSaveApplication
                    {
                        ReferenceNo = referenceNo ?? string.Empty,
                        ModuleName = moduleName ?? string.Empty,
                        SrNo = srNo ?? string.Empty,
                        SrType = srType ?? string.Empty,
                        StatusCode = statusCode ?? string.Empty,
                        SrCreatedDate = srCreatedDate
                    };

                    HttpResponseMessage rawResponse = await service.SaveApplication(new PostSaveApplicationRequest
                    {
                        SaveApplication = request
                    }
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken());
                    PostSaveApplicationResponse response = await rawResponse.ParseAsync<PostSaveApplicationResponse>();
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_SaveApplication.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_SaveApplication.GetStatusDetails("default");
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SaveApplication]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SaveApplication]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            PostSaveApplicationResponse res = new PostSaveApplicationResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = Constants.Service_SaveApplication.GetStatusDetails("default");
            return res;
        }
        #endregion

        #region GetAllApplication
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">Page starts from 1 and incrementing</param>
        /// <param name="searchApplicationType">Application Type ID</param>
        /// <param name="statusDescription">Full description of status.</param>
        /// <param name="createdDateFrom">yyyy/mm/dd Format</param>
        /// <param name="createdDateTo">yyyy/mm/dd Format</param>
        /// <returns></returns>
        private async Task<GetAllApplicationsResponse> AllApplications(int page
           , string searchApplicationType
           , string statusDescription
           , string createdDateFrom
           , string createdDateTo)
        {
            GetAllApplicationsResponse response;
            try
            {
                var limitString = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusLanding", "displayPerQuery");
                //bool isSuccess = int.TryParse(limitString, out int limit);
                if (!int.TryParse(limitString, out int limit))
                {
                    limit = 5;
                }
                AllApplicationsCache.Instance.Limit = limit;
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    response = await service.GetAllApplications(page
                       , limit
                       , string.Empty
                       , string.Empty
                       , string.Empty
                       , string.Empty
                       , searchApplicationType
                       , string.Empty
                       , statusDescription
                       , createdDateFrom
                       , createdDateTo
                       , AppInfoManager.Instance.GetUserInfo()
                       , NetworkService.GetCancellationToken());
                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails("default");
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetAllApplications]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetAllApplications]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            response = new GetAllApplicationsResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = Constants.Service_GetAllApplications.GetStatusDetails("default");
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">Page starts from 1 and incrementing</param>
        /// <returns></returns>
        public async Task<GetAllApplicationsResponse> GetAllApplications(int page)
        {
            GetAllApplicationsResponse response = await AllApplications(page
                , string.Empty
                , string.Empty
                , string.Empty
                , string.Empty);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">Page starts from 1 and incrementing</param>
        /// <param name="searchApplicationType">Application Type ID</param>
        /// <param name="statusDescription">Full description of status.</param>
        /// <param name="createdDateFrom">yyyy/mm/dd Format</param>
        /// <param name="createdDateTo">yyyy/mm/dd Format</param>
        /// <returns></returns>
        public async Task<GetAllApplicationsResponse> FilterApplications(int page
           , string searchApplicationType
           , string statusDescription
           , string createdDateFrom
           , string createdDateTo)
        {
            GetAllApplicationsResponse response = await AllApplications(page
               , searchApplicationType
               , statusDescription
               , createdDateFrom
               , createdDateTo);
            return response;
        }

        #endregion

        #region GetApplicationDetail
        public async Task<ApplicationDetailDisplay> GetApplicationDetail(string id//=searchTerm
            , string applicationType
            , string applicationTypeTitle)
            //, string searchTypeTitle)
        {
            ApplicationDetailDisplay displaymodel = new ApplicationDetailDisplay();
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                   GetApplicationDetailsResponse response = await service.GetApplicationDetail(applicationType
                        , id
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken());
                    Debug.WriteLine("Test");
                    /*if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        if (response.Content == null)
                        {
                            response.StatusDetail.Code = "empty";
                        }
                        else
                        {
                            response.Parse(applicationType
                                , applicationTypeTitle
                                , searchTypeTitle
                                , searchTerm);
                        }
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails("default");
                    }
                    displaymodel = response.Parse(applicationType
                               , applicationTypeTitle
                               , searchTypeTitle
                               , searchTerm);*/
                    return displaymodel;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationDetail]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationDetail]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            displaymodel = new ApplicationDetailDisplay
            {
                StatusDetail = new StatusDetail()
            };
            displaymodel.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails("default");
            return displaymodel;
        }
        #endregion
    }
}