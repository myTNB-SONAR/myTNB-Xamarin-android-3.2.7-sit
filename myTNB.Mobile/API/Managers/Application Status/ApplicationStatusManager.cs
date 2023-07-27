using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API;
using myTNB.Mobile.API.Managers.ApplicationStatus;
using myTNB.Mobile.API.Managers.ApplicationStatus.Utilities;
using myTNB.Mobile.API.Managers.Payment;
using myTNB.Mobile.API.Models;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.API.Models.ApplicationStatus.GetApplicationsByCA;
using myTNB.Mobile.API.Models.ApplicationStatus.PostRemoveApplication;
using myTNB.Mobile.API.Models.ApplicationStatus.PostSyncSRApplication;
using myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using myTNB.Mobile.API.Services.ApplicationStatus;
using myTNB.Mobile.Business;
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
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(MobileConstants.ApiDomain);
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
                    if (response != null && response.Content != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = MobileConstants.Service_SearchApplicationType.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                        {
                            response.StatusDetail = MobileConstants.Service_SearchApplicationType.GetStatusDetails(response.StatusDetail.Code);
                        }
                        else
                        {
                            response = new SearchApplicationTypeResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            response.StatusDetail = MobileConstants.Service_SearchApplicationType.GetStatusDetails(MobileConstants.DEFAULT);
                        }
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
            res.StatusDetail = MobileConstants.Service_SearchApplicationType.GetStatusDetails(MobileConstants.DEFAULT);
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
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(MobileConstants.ApiDomain);
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
                        displaymodel.StatusDetail = MobileConstants.Service_GetApplicationStatus.GetStatusDetails(MobileConstants.EMPTY);
                        return displaymodel;
                    }

                    GetApplicationStatusResponse response = JsonConvert.DeserializeObject<GetApplicationStatusResponse>(responseString);
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        if (response.Content == null && response.StatusDetail.Code == MobileConstants.SUCCESS_CODE)
                        {
                            response.StatusDetail.Code = MobileConstants.EMPTY;
                        }
                        response.StatusDetail = MobileConstants.Service_GetApplicationStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetApplicationStatusResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_GetApplicationStatus.GetStatusDetails(MobileConstants.DEFAULT);
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
            displaymodel.StatusDetail = MobileConstants.Service_GetApplicationStatus.GetStatusDetails(MobileConstants.DEFAULT);
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
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(MobileConstants.ApiDomain);
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
                    PostSaveApplicationRequest req = new PostSaveApplicationRequest
                    {
                        SaveApplication = request,
                        lang = AppInfoManager.Instance.Language.ToString()
                    };
                    EncryptedRequest encryptedRequest = APISecurityManager.Instance.GetEncryptedRequest(req);
                    HttpResponseMessage rawResponse = await service.SaveApplication(encryptedRequest
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    PostSaveApplicationResponse response = await rawResponse.ParseAsync<PostSaveApplicationResponse>();
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = MobileConstants.Service_SaveApplication.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new PostSaveApplicationResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_SaveApplication.GetStatusDetails(MobileConstants.DEFAULT);
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
            res.StatusDetail = MobileConstants.Service_SaveApplication.GetStatusDetails(MobileConstants.DEFAULT);
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
                applicationType = applicationType.IsValid() && applicationType == "ALL" ? string.Empty : applicationType;
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(MobileConstants.ApiDomain);
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
                            StatusDetail = MobileConstants.Service_GetAllApplications.GetStatusDetails(isFilter ? "Constants. EMPTYFilter" : MobileConstants.EMPTY)
                        };
                        return response;
                    }

                    response = JsonConvert.DeserializeObject<GetAllApplicationsResponse>(responseString);
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        //Mark: Check for 0 Applications
                        if (response.Content != null && response.Content.Applications != null
                            && response.Content.Applications.Count == 0 && response.StatusDetail.Code == MobileConstants.SUCCESS_CODE)
                        {
                            response.StatusDetail = MobileConstants.Service_GetAllApplications.GetStatusDetails(isFilter ? MobileConstants.EMPTY_FILTER : MobileConstants.EMPTY);
                        }
                        else
                        {
                            response.StatusDetail = MobileConstants.Service_GetAllApplications.GetStatusDetails(response.StatusDetail.Code);
                        }
                    }
                    else
                    {
                        response = new GetAllApplicationsResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_GetAllApplications.GetStatusDetails(MobileConstants.DEFAULT);
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
            response.StatusDetail = MobileConstants.Service_GetAllApplications.GetStatusDetails(MobileConstants.DEFAULT);
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

        public async Task<GetAllApplicationsResponse> GetNCDraftApplications(int page)
        {
            GetAllApplicationsResponse response = await AllApplications(page
               , "NC"
               , "Draft"
               , string.Empty
               , string.Empty
               , true);
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
            system = system.IsValid() ? system : "myTNB";
            ApplicationDetailDisplay displaymodel = new ApplicationDetailDisplay();
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(MobileConstants.ApiDomain);
                try
                {
                    Debug.WriteLine("[DEBUG] GetApplicationDetail applicationType: " + applicationType);
                    Debug.WriteLine("[DEBUG] GetApplicationDetail searchTerm: " + searchTerm);
                    Debug.WriteLine("[DEBUG] GetApplicationDetail system: " + system);
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
                        displaymodel.StatusDetail = MobileConstants.Service_GetApplicationDetail.GetStatusDetails(MobileConstants.EMPTY);
                        return displaymodel;
                    }

                    GetApplicationDetailsResponse response = JsonConvert.DeserializeObject<GetApplicationDetailsResponse>(responseString);
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        if (response.Content == null && response.StatusDetail.Code == MobileConstants.SUCCESS_CODE)
                        {
                            response.StatusDetail.Code = MobileConstants.EMPTY;
                        }
                        response.StatusDetail = MobileConstants.Service_GetApplicationDetail.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetApplicationDetailsResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_GetApplicationDetail.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                    displaymodel = response.Parse(applicationType
                        , applicationID
                        , system
                        , savedApplicationID
                        , savedApplicationID.IsValid());
                    try
                    {
                        if (displaymodel.StatusDetail.IsSuccess)
                        {
                            //Mark: Call ASMX Payment Details
                            string refNumber = displaymodel.Content.SRNumber.IsValid()
                                ? displaymodel.Content.SRNumber
                                : displaymodel.Content.applicationPaymentDetail?.srNo ?? string.Empty;
                            //Mark: Pass SNNumber for RE_TS
                            if (applicationType == "RE_TS")
                            {
                                refNumber = displaymodel.Content.SNNumber.IsValid()
                                    ? displaymodel.Content.SNNumber
                                    : displaymodel.Content.applicationPaymentDetail?.snNo ?? string.Empty;
                            }
                            if (refNumber.IsValid())
                            {
                                PostApplicationsPaidDetailsResponse paymentResponse = await PaymentManager.Instance.GetApplicationsPaidDetails(
                                    AppInfoManager.Instance.GetPlatformUserInfo()
                                    , refNumber
                                    , displaymodel.Content.ApplicationStatusDetail?.StatusId.ToString() ?? string.Empty
                                    , displaymodel.Content.ApplicationStatusDetail?.StatusCode ?? string.Empty
                                    , applicationType);
                                displaymodel.ParseDisplayModel(paymentResponse);
                            }
                        }
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
                Debug.WriteLine("[DEBUG][GetApplicationDetail]General Exception: " + e.Message);
#endif
            }
            displaymodel = new ApplicationDetailDisplay
            {
                StatusDetail = new StatusDetail()
            };
            displaymodel.StatusDetail = MobileConstants.Service_GetApplicationDetail.GetStatusDetails(MobileConstants.DEFAULT);
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
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(MobileConstants.ApiDomain);
                try
                {
                    PostRemoveApplicationRequest request = new PostRemoveApplicationRequest
                    {
                        ApplicationType = applicationType,
                        AppId = appID,
                        System = system
                    };
                    EncryptedRequest encryptedRequest = APISecurityManager.Instance.GetEncryptedRequest(request);
                    HttpResponseMessage rawResponse = await service.RemoveApplication(encryptedRequest
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    PostRemoveApplicationResponse response = await rawResponse.ParseAsync<PostRemoveApplicationResponse>();
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = MobileConstants.Service_RemoveApplication.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new PostRemoveApplicationResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_RemoveApplication.GetStatusDetails(MobileConstants.DEFAULT);
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
            res.StatusDetail = MobileConstants.Service_RemoveApplication.GetStatusDetails(MobileConstants.DEFAULT);
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
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(MobileConstants.ApiDomain);
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
                            StatusDetail = MobileConstants.Service_SearchApplicationByCA.GetStatusDetails(MobileConstants.EMPTY)
                        };
                        return response;
                    }

                    response = JsonConvert.DeserializeObject<GetApplicationsByCAResponse>(responseString);
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        //Mark: Check for 0 Applications
                        if (response.Content != null && response.Content.Count == 0 && response.StatusDetail.Code == MobileConstants.SUCCESS_CODE)
                        {
                            response.StatusDetail = MobileConstants.Service_SearchApplicationByCA.GetStatusDetails(MobileConstants.EMPTY);
                        }
                        else
                        {
                            response.StatusDetail = MobileConstants.Service_SearchApplicationByCA.GetStatusDetails(response.StatusDetail.Code);
                        }
                    }
                    else
                    {
                        response = new GetApplicationsByCAResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_SearchApplicationByCA.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationByCA]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationByCA]General Exception: " + ex.Message);
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
            response.StatusDetail = MobileConstants.Service_SearchApplicationByCA.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }
        #endregion

        #region SyncSRApplication
        public async Task<PostSyncSRApplicationResponse> SyncSRApplication()
        {
            try
            {
                IApplicationStatusService service = RestService.For<IApplicationStatusService>(MobileConstants.ApiDomain);
                try
                {
                    HttpResponseMessage rawResponse = await service.SyncSRApplication(AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    PostSyncSRApplicationResponse response = await rawResponse.ParseAsync<PostSyncSRApplicationResponse>();
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = MobileConstants.Service_SyncSRApplication.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new PostSyncSRApplicationResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = MobileConstants.Service_SyncSRApplication.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SyncSRApplication]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SyncSRApplication]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            PostSyncSRApplicationResponse res = new PostSyncSRApplicationResponse
            {
                StatusDetail = new StatusDetail()
            };
            res.StatusDetail = MobileConstants.Service_SyncSRApplication.GetStatusDetails(MobileConstants.DEFAULT);
            return res;
        }
        #endregion
    }
}