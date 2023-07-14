using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Managers.ApplicationStatus;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Services.ApplicationStatus;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Mobile.AWS
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

        public async Task<PostDeleteNCDraftResponse> PostDeleteNCDraft(string applicationNumber
            , string userID
            , string token)
        {
            PostDeleteNCDraftResponse response = new PostDeleteNCDraftResponse();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(userID
                , token);
            if (accessToken.IsValid())
            {
                try
                {
                    IApplicationStatusService service = RestService.For<IApplicationStatusService>(AWSConstants.Domains.Domain);
                    HttpResponseMessage rawResponse = await service.PostDeleteNCDraft(applicationNumber
                       , NetworkService.GetCancellationToken()
                       , "Bearer " + accessToken
                       , AppInfoManager.Instance.ViewInfo);
                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = AWSConstants.Services.PostDeleteNCDraft.GetStatusDetails(MobileConstants.DEFAULT);
                        response.StatusDetail.IsSuccess = false;
                        return response;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<PostDeleteNCDraftResponse>(responseString);
                    if (response != null
                        && response.Content != null
                        && response.StatusDetail != null
                        && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.PostDeleteNCDraft.GetStatusDetails(response.StatusDetail.Code);
                        response.StatusDetail.AccessToken = accessToken;
                    }
                    else
                    {
                        if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                        {
                            response.StatusDetail = AWSConstants.Services.PostDeleteNCDraft.GetStatusDetails(response.StatusDetail.Code);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                        else
                        {
                            response = new PostDeleteNCDraftResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            response.StatusDetail = AWSConstants.Services.PostDeleteNCDraft.GetStatusDetails(MobileConstants.DEFAULT);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                    }
                    Debug.WriteLine("[DEBUG] [PostDeleteNCDraft]: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostDeleteNCDraft] Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostDeleteNCDraft] General Exception: " + ex.Message);
#endif
                }
            }
            response = new PostDeleteNCDraftResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.PostDeleteNCDraft.GetStatusDetails(MobileConstants.DEFAULT);
            response.StatusDetail.AccessToken = accessToken;
            return response;
        }

        public async Task<PostGetDraftResponse> PostGetDraftApplications(string userID
            , List<string> localList)
        {
            PostGetDraftResponse response = new PostGetDraftResponse();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(userID);
            if (accessToken.IsValid())
            {
                try
                {
                    IApplicationStatusService service = RestService.For<IApplicationStatusService>(AWSConstants.Domains.Domain);
                    HttpResponseMessage rawResponse = await service.PostGetDraftApplications(NetworkService.GetCancellationToken()
                       , "Bearer " + accessToken
                       , AppInfoManager.Instance.ViewInfo);
                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = AWSConstants.Services.PostGetDraftApplications.GetStatusDetails(MobileConstants.DEFAULT);
                        response.StatusDetail.IsSuccess = false;
                        return response;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<PostGetDraftResponse>(responseString);
                    if (response != null
                        && response.Content != null
                        && response.StatusDetail != null
                        && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.PostGetDraftApplications.GetStatusDetails(response.StatusDetail.Code);
                        response.StatusDetail.AccessToken = accessToken;
                    }
                    else
                    {
                        if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                        {
                            response.StatusDetail = AWSConstants.Services.PostGetDraftApplications.GetStatusDetails(response.StatusDetail.Code);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                        else
                        {
                            response = new PostGetDraftResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            response.StatusDetail = AWSConstants.Services.PostGetDraftApplications.GetStatusDetails(MobileConstants.DEFAULT);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                    }
                    Debug.WriteLine("[DEBUG] [PostGetDraftApplications] Response: " + JsonConvert.SerializeObject(response));
                    ApplicationDraftUtility.UpdateDraftResponse(ref response
                        , localList);
                    Debug.WriteLine("[DEBUG] [PostGetDraftApplications] Parsed Response: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostGetDraftApplications] Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostGetDraftApplications] General Exception: " + ex.Message);
#endif
                }
            }
            response = new PostGetDraftResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.PostGetDraftApplications.GetStatusDetails(MobileConstants.DEFAULT);
            response.StatusDetail.AccessToken = accessToken;
            return response;
        }

        public async Task<PostDeleteCOTDraftResponse> PostDeleteCOTDraft(string applicationNumber
            , string userID
            , string token)
        {
            PostDeleteCOTDraftResponse response = new PostDeleteCOTDraftResponse();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(userID
                , token);
            if (accessToken.IsValid())
            {
                try
                {
                    IApplicationStatusService service = RestService.For<IApplicationStatusService>(AWSConstants.Domains.Domain);
                    HttpResponseMessage rawResponse = await service.PostDeleteCOTDraft(applicationNumber
                       , NetworkService.GetCancellationToken()
                       , "Bearer " + accessToken
                       , AppInfoManager.Instance.ViewInfo);
                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = AWSConstants.Services.PostDeleteCOTDraft.GetStatusDetails(MobileConstants.DEFAULT);
                        response.StatusDetail.IsSuccess = false;
                        return response;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<PostDeleteCOTDraftResponse>(responseString);
                    if (response != null
                        && response.Content != null
                        && response.StatusDetail != null
                        && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.PostDeleteCOTDraft.GetStatusDetails(response.StatusDetail.Code);
                        response.StatusDetail.AccessToken = accessToken;
                    }
                    else
                    {
                        if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                        {
                            response.StatusDetail = AWSConstants.Services.PostDeleteCOTDraft.GetStatusDetails(response.StatusDetail.Code);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                        else
                        {
                            response = new PostDeleteCOTDraftResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            response.StatusDetail = AWSConstants.Services.PostDeleteCOTDraft.GetStatusDetails(MobileConstants.DEFAULT);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                    }
                    Debug.WriteLine("[DEBUG] [PostDeleteCOTDraft]: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostDeleteCOTDraft] Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostDeleteCOTDraft] General Exception: " + ex.Message);
#endif
                }
            }
            response = new PostDeleteCOTDraftResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.PostDeleteCOTDraft.GetStatusDetails(MobileConstants.DEFAULT);
            response.StatusDetail.AccessToken = accessToken;
            return response;
        }

        public async Task<PostDeleteCOADraftResponse> PostDeleteCOADraft(string applicationNumber
            , string userID
            , string token)
        {
            PostDeleteCOADraftResponse response = new PostDeleteCOADraftResponse();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(userID
                , token);
            if (accessToken.IsValid())
            {
                try
                {
                    IApplicationStatusService service = RestService.For<IApplicationStatusService>(AWSConstants.Domains.Domain);
                    HttpResponseMessage rawResponse = await service.PostDeleteCOADraft(applicationNumber
                       , NetworkService.GetCancellationToken()
                       , "Bearer " + accessToken
                       , AppInfoManager.Instance.ViewInfo);
                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = AWSConstants.Services.PostDeleteCOADraft.GetStatusDetails(MobileConstants.DEFAULT);
                        response.StatusDetail.IsSuccess = false;
                        return response;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<PostDeleteCOADraftResponse>(responseString);
                    if (response != null
                        && response.Content != null
                        && response.StatusDetail != null
                        && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.PostDeleteCOADraft.GetStatusDetails(response.StatusDetail.Code);
                        response.StatusDetail.AccessToken = accessToken;
                    }
                    else
                    {
                        if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                        {
                            response.StatusDetail = AWSConstants.Services.PostDeleteCOADraft.GetStatusDetails(response.StatusDetail.Code);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                        else
                        {
                            response = new PostDeleteCOADraftResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            response.StatusDetail = AWSConstants.Services.PostDeleteCOADraft.GetStatusDetails(MobileConstants.DEFAULT);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                    }
                    Debug.WriteLine("[DEBUG] [PostDeleteCOADraft]: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostDeleteCOADraft] Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostDeleteCOADraft] General Exception: " + ex.Message);
#endif
                }
            }
            response = new PostDeleteCOADraftResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.PostDeleteCOADraft.GetStatusDetails(MobileConstants.DEFAULT);
            response.StatusDetail.AccessToken = accessToken;
            return response;
        }
    }
}