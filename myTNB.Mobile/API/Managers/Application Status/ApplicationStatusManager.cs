using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API;
using myTNB.Mobile.API.Managers;
using myTNB.Mobile.API.Managers.ApplicationStatus;
using myTNB.Mobile.API.Managers.ApplicationStatus.Utilities;
using myTNB.Mobile.API.Managers.Payment;
using myTNB.Mobile.API.Models;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.API.Models.ApplicationStatus.GetApplicationsByCA;
using myTNB.Mobile.API.Models.ApplicationStatus.PostRemoveApplication;
using myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
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
                            , AppInfoManager.Instance.Language.ToString()
                            , AppInfoManager.Instance.Language.ToString())
                        : await service.SearchApplicationType(NetworkService.GetCancellationToken()
                            , AppInfoManager.Instance.Language.ToString()
                            , AppInfoManager.Instance.Language.ToString());
                    if (response.Content != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(Constants.DEFAULT);
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
            res.StatusDetail = Constants.Service_SearchApplicationType.GetStatusDetails(Constants.DEFAULT);
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
        /// <param name="isLoggedIn"></param>
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
                            , AppInfoManager.Instance.Language.ToString()
                            , AppInfoManager.Instance.Language.ToString())
                        : await service.GetApplicationStatus(applicationType
                            , searchType
                            , searchTerm
                            , NetworkService.GetCancellationToken()
                            , AppInfoManager.Instance.Language.ToString()
                            , AppInfoManager.Instance.Language.ToString());

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    //Mark: Check for 404 First
                    NotFoundModel notFoundModel = JsonConvert.DeserializeObject<NotFoundModel>(responseString);
                    if (notFoundModel != null && notFoundModel.Status.IsValid())
                    {
                        displaymodel.Content = null;
                        displaymodel.StatusDetail = new StatusDetail();
                        displaymodel.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(Constants.EMPTY);
                        return displaymodel;
                    }

                    GetApplicationStatusResponse response = JsonConvert.DeserializeObject<GetApplicationStatusResponse>(responseString);
                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        if (response.Content == null)
                        {
                            response.StatusDetail.Code = Constants.EMPTY;
                        }
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetApplicationStatusResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(Constants.DEFAULT);
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
            displaymodel.StatusDetail = Constants.Service_GetApplicationStatus.GetStatusDetails(Constants.DEFAULT);
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
        /// <param name="createdDate">From status search, pass createdDate property</param>
        /// <returns></returns>
        public async Task<PostSaveApplicationResponse> SaveApplication(string referenceNo
            , string applicationModuleId
            , string applicationType
            , string backendReferenceNo
            , string backendApplicationType
            , string backendModule
            , string statusCode
            , DateTime createdDate)
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
                        SrCreatedDate = createdDate
                    };

                    HttpResponseMessage rawResponse = await service.SaveApplication(new PostSaveApplicationRequest
                    {
                        SaveApplication = request,
                        lang = AppInfoManager.Instance.Language.ToString()
                    }
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    PostSaveApplicationResponse response = await rawResponse.ParseAsync<PostSaveApplicationResponse>();
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_SaveApplication.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new PostSaveApplicationResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = Constants.Service_SaveApplication.GetStatusDetails(Constants.DEFAULT);
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
            res.StatusDetail = Constants.Service_SaveApplication.GetStatusDetails(Constants.DEFAULT);
            return res;
        }
        #endregion

        #region GetAllApplication
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">Page starts from 1 and incrementing</param>
        /// <param name="applicationType">Application Type ID</param>
        /// <param name="statusDescription">Full description of status.</param>
        /// <param name="createdDateFrom">yyyy/mm/dd Format</param>
        /// <param name="createdDateTo">yyyy/mm/dd Format</param>
        /// <returns></returns>
        private async Task<GetAllApplicationsResponse> AllApplications(int page
            , string applicationType
            , string statusDescription
            , string createdDateFrom
            , string createdDateTo
            , bool isFilter)
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
                        , applicationType
                        , string.Empty
                        , statusDescription
                        , createdDateFrom
                        , createdDateTo
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString()
                        , AppInfoManager.Instance.Language.ToString());

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    //Mark: Check for 404 First
                    NotFoundModel notFoundModel = JsonConvert.DeserializeObject<NotFoundModel>(responseString);
                    if (notFoundModel != null && notFoundModel.Status.IsValid())
                    {
                        response = new GetAllApplicationsResponse
                        {
                            Content = null,
                            StatusDetail = Constants.Service_GetAllApplications.GetStatusDetails(isFilter ? "Constants. EMPTYFilter" : Constants.EMPTY)
                        };
                        return response;
                    }

                    response = JsonConvert.DeserializeObject<GetAllApplicationsResponse>(responseString);
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        //Mark: Check for 0 Applications
                        if (response.Content != null && response.Content.Applications != null && response.Content.Applications.Count == 0)
                        {
                            response.StatusDetail = Constants.Service_GetAllApplications.GetStatusDetails(isFilter ? "Constants. EMPTYFilter" : Constants.EMPTY);
                        }
                        else
                        {
                            response.StatusDetail = Constants.Service_GetAllApplications.GetStatusDetails(response.StatusDetail.Code);
                        }
                    }
                    else
                    {
                        response = new GetAllApplicationsResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = Constants.Service_GetAllApplications.GetStatusDetails(Constants.DEFAULT);
                    }
                    if (response.StatusDetail.IsSuccess)
                    {
                        AllApplicationsCache.Instance.SetData(response.Content, isFilter);
                        if (isFilter)
                        {
                            //Mark: If this is filter, always set to 1.
                            AllApplicationsCache.Instance.QueryPage = 1;
                            AllApplicationsCache.Instance.IsFiltertriggered = true;
                        }
                        //Mark: Increment Query Page Every Success Call
                        AllApplicationsCache.Instance.QueryPage += 1;
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
            response.StatusDetail = Constants.Service_GetAllApplications.GetStatusDetails(Constants.DEFAULT);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">Page starts from 1 and incrementing. Pass 1 for filter</param>
        /// <param name="applicationType">Application Type ID</param>
        /// <param name="statusDescription">Full description of status.</param>
        /// <param name="createdDateFrom">yyyy/mm/dd Format</param>
        /// <param name="createdDateTo">yyyy/mm/dd Format</param>
        /// <param name="isFilter">Determines if the query is for filter or not</param>
        /// <returns></returns>
        public async Task<GetAllApplicationsResponse> GetAllApplications(int page
            , string applicationType
            , string statusDescription
            , string createdDateFrom
            , string createdDateTo
            , bool isFilter = false)
        {
            GetAllApplicationsResponse response = await AllApplications(page
               , applicationType
               , statusDescription
               , createdDateFrom
               , createdDateTo
               , isFilter);
            return response;
        }

        #endregion

        #region GetApplicationDetail
        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedApplicationID">From all applications, pass SavedApplicationId property</param>
        /// <param name="applicationID">From all applications, pass ApplicationId property</param>
        /// <param name="applicationType">From all applications, pass ApplicationType property</param>
        /// <param name="system">From all applications, pass system property.</param>
        /// <returns></returns>
        public async Task<ApplicationDetailDisplay> GetApplicationDetail(string savedApplicationID
            , string applicationID
            , string applicationType
            , string system = "myTNB")
        {
            string searchTerm = savedApplicationID.IsValid() ? savedApplicationID : applicationID;
            ApplicationDetailDisplay displaymodel = new ApplicationDetailDisplay();
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    HttpResponseMessage rawResponse = await service.GetApplicationDetail(applicationType
                         , searchTerm
                         , system
                         , AppInfoManager.Instance.GetUserInfo()
                         , NetworkService.GetCancellationToken()
                         , AppInfoManager.Instance.Language.ToString()
                         , AppInfoManager.Instance.Language.ToString());

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    //Mark: Check for 404 First
                    NotFoundModel notFoundModel = JsonConvert.DeserializeObject<NotFoundModel>(responseString);
                    if (notFoundModel != null && notFoundModel.Status.IsValid())
                    {
                        displaymodel.Content = null;
                        displaymodel.StatusDetail = new StatusDetail();
                        displaymodel.StatusDetail = Constants.Service_GetApplicationDetail.GetStatusDetails(Constants.EMPTY);
                        return displaymodel;
                    }

                    GetApplicationDetailsResponse response = JsonConvert.DeserializeObject<GetApplicationDetailsResponse>(responseString);
                    if (response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        if (response.Content == null)
                        {
                            response.StatusDetail.Code = Constants.EMPTY;
                        }
                        response.StatusDetail = Constants.Service_GetApplicationDetail.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetApplicationDetailsResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = Constants.Service_GetApplicationDetail.GetStatusDetails(Constants.DEFAULT);
                    }
                    displaymodel = response.Parse(applicationType
                        , applicationID
                        , system
                        , savedApplicationID
                        , savedApplicationID.IsValid());
                    try
                    {
                        //Mark: Call ASMX Payment Details
                        PostApplicationsPaidDetailsResponse paymentResponse = await PaymentManager.Instance.GetApplicationsPaidDetails(
                            AppInfoManager.Instance.GetPlatformUserInfo()
                            , displaymodel.Content.SRNumber);
                        displaymodel.ParseDisplayModel(paymentResponse);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine("[DEBUG][GetApplicationDetail ASMX Payment Details]General Exception: " + ex.Message);
#endif
                    }
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
            displaymodel.StatusDetail = Constants.Service_GetApplicationDetail.GetStatusDetails(Constants.DEFAULT);
            return displaymodel;
        }
        #endregion

        #region RemoveApplication
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationType">Application Type Code. ie: ASR/NC/COA</param>
        /// <param name="appID">SavedApplicationID</param>
        /// <param name="system">System</param>
        /// <returns></returns>
        public async Task<PostRemoveApplicationResponse> RemoveApplication(string applicationType
            , string appID
            , string system)
        {
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    PostRemoveApplicationRequest request = new PostRemoveApplicationRequest
                    {
                        ApplicationType = applicationType,
                        AppId = appID,
                        System = system
                    };

                    HttpResponseMessage rawResponse = await service.RemoveApplication(request
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    PostRemoveApplicationResponse response = await rawResponse.ParseAsync<PostRemoveApplicationResponse>();
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = Constants.Service_RemoveApplication.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new PostRemoveApplicationResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = Constants.Service_RemoveApplication.GetStatusDetails(Constants.DEFAULT);
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][RemoveApplication]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][RemoveApplication]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            PostRemoveApplicationResponse res = new PostRemoveApplicationResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = Constants.Service_RemoveApplication.GetStatusDetails(Constants.DEFAULT);
            return res;
        }
        #endregion

        #region GetApplicationByCA
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountNumber">CA Number</param>
        /// <returns></returns>
        public async Task<GetApplicationsByCAResponse> GetApplicationByCA(string accountNumber)
        {
            GetApplicationsByCAResponse response;
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(Constants.ApiDomain);
                try
                {
                    HttpResponseMessage rawResponse = await service.GetApplicationsByCA(accountNumber
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString()
                        , AppInfoManager.Instance.Language.ToString());

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    //Mark: Check for 404 First.
                    NotFoundModel notFoundModel = JsonConvert.DeserializeObject<NotFoundModel>(responseString);
                    if (notFoundModel != null && notFoundModel.Status.IsValid())
                    {
                        response = new GetApplicationsByCAResponse
                        {
                            Content = null,
                            StatusDetail = Constants.Service_SearchApplicationByCA.GetStatusDetails(Constants.EMPTY)
                        };
                        return response;
                    }

                    response = JsonConvert.DeserializeObject<GetApplicationsByCAResponse>(responseString);
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        //Mark: Check for 0 Applications
                        if (response.Content != null && response.Content.Count == 0)
                        {
                            response.StatusDetail = Constants.Service_SearchApplicationByCA.GetStatusDetails(Constants.EMPTY);
                        }
                        else
                        {
                            response.StatusDetail = Constants.Service_SearchApplicationByCA.GetStatusDetails(response.StatusDetail.Code);
                        }
                    }
                    else
                    {
                        response = new GetApplicationsByCAResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = Constants.Service_SearchApplicationByCA.GetStatusDetails(Constants.DEFAULT);
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
            response = new GetApplicationsByCAResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = Constants.Service_SearchApplicationByCA.GetStatusDetails(Constants.DEFAULT);
            return response;
        }
        #endregion
    }
}