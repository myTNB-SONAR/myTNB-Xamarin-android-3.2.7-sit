using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Home.PostServices;
using myTNB.Mobile.AWS.Models.MoreIcon;
using myTNB.Mobile.AWS.Services.Eligibility;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Mobile.AWS.Managers.MoreIcon
{
    public sealed class MoreIconManager
    {
        private static readonly Lazy<MoreIconManager> lazy =
           new Lazy<MoreIconManager>(() => new MoreIconManager());

        public static MoreIconManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public MoreIconManager() { }

        public async Task<MoreIconResponse> GetMoreIconList(DeviceInfoExtra deviceinfo
            , UserInfoExtra userinfo
            , string accessToken)
        {
            MoreIconResponse response = new MoreIconResponse();
            int timeout = LanguageManager.Instance.GetConfigTimeout(LanguageManager.ConfigPropertyEnum.AccountStatementTimeout);
            IEligibilityService service = RestService.For<IEligibilityService>(AWSConstants.Domains.Domain);
            Guid referenceNumber = Guid.NewGuid();
            try
            {
                MoreIconRequest request = new MoreIconRequest
                {
                    usrInf = userinfo,
                    deviceInf = deviceinfo
                };
                Debug.WriteLine("[DEBUG] GetMoreIconList Request: " + JsonConvert.SerializeObject(request));

                HttpResponseMessage rawResponse = await service.GetMoreIconList(request
                   , NetworkService.GetCancellationToken(timeout == 0 ? AWSConstants.AccountStatementTimeOut : timeout)
                   , accessToken
                   , AppInfoManager.Instance.ViewInfo);
                //Mark: Check for 404 First
                if ((int)rawResponse.StatusCode != 200)
                {
                    response.StatusDetail = new StatusDetail();
                    response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(MobileConstants.DEFAULT);
                    response.StatusDetail.IsSuccess = false;
                    return response;
                }

                string responseString = await rawResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<MoreIconResponse>(responseString);
                if (response != null
                    && response.Content != null
                    && response.StatusDetail != null
                    && response.StatusDetail.Code.IsValid())
                {
                    response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(response.StatusDetail.Code);
                }
                else
                {
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new MoreIconResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                }
                Debug.WriteLine("[DEBUG] [MoreIcon] Response: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [MoreIcon] Refit Exception: " + apiEx.Message);
#endif
            }
            catch (OperationCanceledException operationCancelledError)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [MoreIcon] OperationCanceledException: " + operationCancelledError.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [MoreIcon] General Exception: " + ex.Message);
#endif
            }

            response = new MoreIconResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(MobileConstants.DEFAULT);
            Debug.WriteLine("[DEBUG] [MoreIcon] Response: " + JsonConvert.SerializeObject(response));
            return response;
        }

        public async Task<MoreIconResponse> UpdateMoreIconList(DeviceInfoExtra deviceinfo
            , UserInfoExtra userinfo
            , string accessToken
            , string email
            , string userID
            , List<Features> moreIconList)
        {
            MoreIconResponse response = new MoreIconResponse();
            int timeout = LanguageManager.Instance.GetConfigTimeout(LanguageManager.ConfigPropertyEnum.AccountStatementTimeout);
            IEligibilityService service = RestService.For<IEligibilityService>(AWSConstants.Domains.Domain);
            Guid referenceNumber = Guid.NewGuid();
            try
            {
                MoreIconModel moreIconModel = new MoreIconModel
                {
                    userId = userID,
                    email = email,
                    featureIcon = moreIconList
                };


                UpdateMoreIconRequest request = new UpdateMoreIconRequest
                {
                    usrInf = userinfo,
                    deviceInf = deviceinfo,
                    content = moreIconModel
                };

                Debug.WriteLine("[DEBUG] UpdateMoreIconList Request: " + JsonConvert.SerializeObject(request));

                HttpResponseMessage rawResponse = await service.UpdateMoreIconList(request
                   , NetworkService.GetCancellationToken(timeout == 0 ? AWSConstants.AccountStatementTimeOut : timeout)
                   , accessToken
                   , AppInfoManager.Instance.ViewInfo);
                //Mark: Check for 404 First
                if ((int)rawResponse.StatusCode != 200)
                {
                    response.StatusDetail = new StatusDetail();
                    response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(MobileConstants.DEFAULT);
                    response.StatusDetail.IsSuccess = false;
                    return response;
                }

                string responseString = await rawResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<MoreIconResponse>(responseString);
                if (response != null
                    && response.Content != null
                    && response.StatusDetail != null
                    && response.StatusDetail.Code.IsValid())
                {
                    response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(response.StatusDetail.Code);
                }
                else
                {
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new MoreIconResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                }
                Debug.WriteLine("[DEBUG] [UpdateMoreIcon] Response: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [UpdateMoreIcon] Refit Exception: " + apiEx.Message);
#endif
            }
            catch (OperationCanceledException operationCancelledError)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [UpdateMoreIcon] OperationCanceledException: " + operationCancelledError.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [UpdateMoreIcon] General Exception: " + ex.Message);
#endif
            }

            response = new MoreIconResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetMoreIconStatus.GetStatusDetails(MobileConstants.DEFAULT);
            Debug.WriteLine("[DEBUG] [UpdateMoreIcon] Response: " + JsonConvert.SerializeObject(response));
            return response;
        }
    }

}

