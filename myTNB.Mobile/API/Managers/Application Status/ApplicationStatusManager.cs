using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API;
using myTNB.Mobile.API.Managers.ApplicationStatus;
using myTNB.Mobile.API.Managers.ApplicationStatus.Utilities;
using myTNB.Mobile.API.Models;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication;
using myTNB.Mobile.API.Services.ApplicationStatus;
using myTNB.Mobile.Extensions;
using myTNB.Mobile.SessionCache;
using Newtonsoft.Json;
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
        public ApplicationStatusManager() { }

        private const string EMPTY = "empty";
        private const string DEFAULT = "default";

        #region SearchApplicationType
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleID">0 for pre-login. 16 for post login</param>
        /// <param name="isLoggedIn"></param>
        /// <returns></returns>
        public async Task<SearchApplicationTypeResponse> SearchApplicationType(string roleID
            , bool isLoggedIn)
        {
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    SearchApplicationTypeResponse response = isLoggedIn
                        ? await service.SearchApplicationType(AppInfoManager.Instance.GetUserInfo()
                            , NetworkService.GetCancellationToken()
                            , AppInfoManager.Instance.Language.ToString())
                        : await service.SearchApplicationType(NetworkService.GetCancellationToken()
                            , AppInfoManager.Instance.Language.ToString());
                    if (response.Content != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(DEFAULT);
                    }
                    response.SearchApplicationTypeParser(roleID);
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
            res.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(DEFAULT);
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
            , string searchTypeTitle
            , bool isLoggedIn)
        {
            ApplicationDetailDisplay displaymodel = new ApplicationDetailDisplay();
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    HttpResponseMessage rawResponse = isLoggedIn
                        ? await service.GetApplicationStatus(applicationType
                            , searchType
                            , searchTerm
                            , AppInfoManager.Instance.GetUserInfo()
                            , NetworkService.GetCancellationToken()
                            , AppInfoManager.Instance.Language.ToString())
                        : await service.GetApplicationStatus(applicationType
                            , searchType
                            , searchTerm
                            , NetworkService.GetCancellationToken()
                            , AppInfoManager.Instance.Language.ToString());

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    //Mark: Check for 404 First
                    NotFoundModel notFoundModel = JsonConvert.DeserializeObject<NotFoundModel>(responseString);
                    if (notFoundModel != null && notFoundModel.Status.IsValid())
                    {
                        displaymodel.Content = null;
                        displaymodel.StatusDetail = new StatusDetail();
                        displaymodel.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(EMPTY);
                        return displaymodel;
                    }

                    GetApplicationStatusResponse response = JsonConvert.DeserializeObject<GetApplicationStatusResponse>(responseString);
                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        if (response.Content == null)
                        {
                            response.StatusDetail.Code = EMPTY;
                        }
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(DEFAULT);
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
            displaymodel.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(DEFAULT);
            return displaymodel;
        }
        #endregion

        #region SaveApplication
        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceNo">From status search, pass referenceNo property</param>
        /// <param name="applicationModuleId">From status search, pass applicationModuleId property</param>
        /// <param name="applicationType">From status search, pass applicationTypeID property</param>
        /// <param name="backendReferenceNo">From status search, pass backendReferenceNo property</param>
        /// <param name="backendApplicationType">From status search, pass backendApplicationType property</param>
        /// <param name="backendModule">From status search, pass backendModule property</param>
        /// <param name="statusCode">From status search, pass statusCode property</param>
        /// <param name="srCreatedDate">From status search, pass srCreatedDate property</param>
        /// <returns></returns>
        public async Task<PostSaveApplicationResponse> SaveApplication(string referenceNo
            , string applicationModuleId
            , string applicationType
            , string backendReferenceNo
            , string backendApplicationType
            , string backendModule
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
                        ReferenceNo = referenceNo,
                        ApplicationModuleId = applicationModuleId,
                        ApplicationType = applicationType,
                        BackendReferenceNo = backendReferenceNo,
                        BackendApplicationType = backendApplicationType,
                        BackendModule = backendModule,
                        StatusCode = statusCode,
                        SrCreatedDate = srCreatedDate
                    };

                    HttpResponseMessage rawResponse = await service.SaveApplication(new PostSaveApplicationRequest
                    {
                        SaveApplication = request,
                        lang = AppInfoManager.Instance.Language.ToString()
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
                        response.StatusDetail = Constants.Service_SaveApplication.GetStatusDetails(DEFAULT);
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
            res.StatusDetail = Constants.Service_SaveApplication.GetStatusDetails(DEFAULT);
            return res;
        }
        #endregion
        private int count = 0;
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
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    HttpResponseMessage rawResponse = await service.GetAllApplications(page
                        , AllApplicationsCache.Instance.Limit
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
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    //Mark: Check for 404 First
                    NotFoundModel notFoundModel = JsonConvert.DeserializeObject<NotFoundModel>(responseString);
                    if (notFoundModel != null && notFoundModel.Status.IsValid())
                    {
                        response = new GetAllApplicationsResponse
                        {
                            Content = null,
                            StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(EMPTY)
                        };
                        return response;
                    }

                    response = JsonConvert.DeserializeObject<GetAllApplicationsResponse>(responseString);
                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(DEFAULT);
                    }
                    if (response.StatusDetail.IsSuccess)
                    {
                        AllApplicationsCache.Instance.SetData(response.Content);
                    }
                    /*if (count < 2)
                    {
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(DEFAULT);
                    }
                    count++;*/
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
            response.StatusDetail = Constants.Service_GetAllApplications.GetStatusDetails(DEFAULT);
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
        public async Task<GetAllApplicationsResponse> GetAllApplications(int page
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedApplicationID">From all applications, pass SavedApplicationId property</param>
        /// <param name="applicationID">From all applications, pass ApplicationId property</param>
        /// <param name="searchApplicationType">From all applications, pass SearchApplicationType property</param>
        /// <param name="applicationModuleDescription">From all applications, pass ApplicationModuleDescription property</param>
        /// <param name="applicationModuleId">From all applications, pass applicationModuleId property. Used to determine NC Display.</param>
        /// <param name="createdByRoleID">From all applications, pass createdByRoleID property. Used to determine NC Display.</param>
        /// <param name="system">From all applications, pass system property.</param>
        /// <param name="isPremiseServiceReady">From all applications, pass isPremiseServiceReady property. Used to determine NC Display.</param>
        /// <returns></returns>
        public async Task<ApplicationDetailDisplay> GetApplicationDetail(string savedApplicationID
            , string applicationID
            , string searchApplicationType
            , string applicationModuleDescription
            , string applicationModuleId
            , string createdByRoleID
            , string system
            , bool isPremiseServiceReady)
        {
            string searchTerm = savedApplicationID.IsValid() ? savedApplicationID : applicationID;
            ApplicationDetailDisplay displaymodel = new ApplicationDetailDisplay();
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    HttpResponseMessage rawResponse = await service.GetApplicationDetail(searchApplicationType
                         , searchTerm
                         , system
                         , AppInfoManager.Instance.GetUserInfo()
                         , NetworkService.GetCancellationToken()
                         , AppInfoManager.Instance.Language.ToString());

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    //Mark: Check for 404 First
                    NotFoundModel notFoundModel = JsonConvert.DeserializeObject<NotFoundModel>(responseString);
                    if (notFoundModel != null && notFoundModel.Status.IsValid())
                    {
                        displaymodel.Content = null;
                        displaymodel.StatusDetail = new StatusDetail();
                        displaymodel.StatusDetail = Constants.Service_GetApplicationDetail.GetStatusDetails(EMPTY);
                        return displaymodel;
                    }

                    GetApplicationDetailsResponse response = JsonConvert.DeserializeObject<GetApplicationDetailsResponse>(responseString);
                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        if (response.Content == null)
                        {
                            response.StatusDetail.Code = EMPTY;
                        }
                        response.StatusDetail = Constants.Service_GetApplicationDetail.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_GetApplicationDetail.GetStatusDetails(DEFAULT);
                    }
                    displaymodel = response.Parse(searchApplicationType
                        , applicationModuleDescription
                        , applicationID
                        , applicationModuleId
                        , createdByRoleID
                        , isPremiseServiceReady
                        , savedApplicationID.IsValid());
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
            displaymodel.StatusDetail = Constants.Service_GetApplicationDetail.GetStatusDetails(DEFAULT);
            return displaymodel;
        }
        #endregion
    }
}