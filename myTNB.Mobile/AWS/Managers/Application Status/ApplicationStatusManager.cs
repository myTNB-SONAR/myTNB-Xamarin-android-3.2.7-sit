using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Managers.ApplicationStatus;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Models.ApplicationStatus.PostNCDraftApplications;
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

        public async Task<PostGetNCDraftResponse> PostGetNCDraftApplications(string userID
            , string email
            , List<string> localNCList)
        {
            PostGetNCDraftResponse response = new PostGetNCDraftResponse();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(userID);
            if (accessToken.IsValid())
            {
                try
                {
                    IApplicationStatusService service = RestService.For<IApplicationStatusService>(AWSConstants.Domains.Domain);
                    PostGetNCDraftRequest request = new PostGetNCDraftRequest
                    {
                        UserID = userID,
                        Email = email
                    };
                    Debug.WriteLine("[DEBUG] [PostGetNCDraftApplications] Request: " + JsonConvert.SerializeObject(request));
                    HttpResponseMessage rawResponse = await service.PostGetNCDraftApplications(request
                       , NetworkService.GetCancellationToken()
                       , "Bearer " + accessToken
                       , AppInfoManager.Instance.ViewInfo);
                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = AWSConstants.Services.PostGetNCDraftApplications.GetStatusDetails(MobileConstants.DEFAULT);
                        response.StatusDetail.IsSuccess = false;
                        return response;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<PostGetNCDraftResponse>(responseString);
                    if (response != null
                        && response.Content != null
                        && response.StatusDetail != null
                        && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.PostGetNCDraftApplications.GetStatusDetails(response.StatusDetail.Code);
                        response.StatusDetail.AccessToken = accessToken;
                    }
                    else
                    {
                        if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                        {
                            response.StatusDetail = AWSConstants.Services.PostGetNCDraftApplications.GetStatusDetails(response.StatusDetail.Code);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                        else
                        {
                            response = new PostGetNCDraftResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            response.StatusDetail = AWSConstants.Services.PostGetNCDraftApplications.GetStatusDetails(MobileConstants.DEFAULT);
                            response.StatusDetail.AccessToken = accessToken;
                        }
                    }
                    Debug.WriteLine("[DEBUG] [PostGetNCDraftApplications] Response: " + JsonConvert.SerializeObject(response));
                    NCDraftUtility.UpdateNCDraftResponse(ref response
                        , localNCList);
                    Debug.WriteLine("[DEBUG] [PostGetNCDraftApplications] Parsed Response: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostGetNCDraftApplications] Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostGetNCDraftApplications] General Exception: " + ex.Message);
#endif
                }
            }
            response = new PostGetNCDraftResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.PostGetNCDraftApplications.GetStatusDetails(MobileConstants.DEFAULT);
            response.StatusDetail.AccessToken = accessToken;
            return response;
        }
    }
}