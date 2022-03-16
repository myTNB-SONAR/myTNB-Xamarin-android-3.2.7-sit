using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.DS.Identification;
using myTNB.Mobile.AWS.Models.DS.Status;
using myTNB.Mobile.AWS.Services.DS;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Mobile.AWS.Managers.DS
{
    public sealed class DSManager
    {
        private static readonly Lazy<DSManager> lazy =
           new Lazy<DSManager>(() => new DSManager());

        public static DSManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public DSManager() { }

        public async Task<GetEKYCStatusResponse> GetEKYCStatus(string userID
           , string accessToken)
        {
            GetEKYCStatusResponse response = new GetEKYCStatusResponse();

            try
            {
                IDSService service = RestService.For<IDSService>(AWSConstants.Domains.Domain);
                HttpResponseMessage rawResponse = await service.GetEKYCStatus(userID
                   , NetworkService.GetCancellationToken()
                   , accessToken
                   , AppInfoManager.Instance.ViewInfo);
                //Mark: Check for 404 First
                if ((int)rawResponse.StatusCode != 200)
                {
                    response.StatusDetail = new StatusDetail();
                    response.StatusDetail = AWSConstants.Services.GetEKYCStatus.GetStatusDetails(MobileConstants.DEFAULT);
                    response.StatusDetail.IsSuccess = false;
                    return response;
                }

                string responseString = await rawResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<GetEKYCStatusResponse>(responseString);
                if (response != null
                    && response.Content != null
                    && response.StatusDetail != null
                    && response.StatusDetail.Code.IsValid())
                {
                    response.StatusDetail = AWSConstants.Services.GetEKYCStatus.GetStatusDetails(response.StatusDetail.Code);
                }
                else
                {
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.GetEKYCStatus.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetEKYCStatusResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = AWSConstants.Services.GetEKYCStatus.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                }
                Debug.WriteLine("[DEBUG] [GetEKYCStatus]: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [GetEKYCStatus] Refit Exception: " + apiEx.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [GetEKYCStatus] General Exception: " + ex.Message);
#endif
            }

            response = new GetEKYCStatusResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetEKYCStatus.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }


        public async Task<GetEKYCIdentificationResponse> GetEKYCIdentification(string userID
          , string accessToken)
        {
            GetEKYCIdentificationResponse response = new GetEKYCIdentificationResponse();

            try
            {
                IDSService service = RestService.For<IDSService>(AWSConstants.Domains.Domain);
                HttpResponseMessage rawResponse = await service.GetEKYCIdentification(userID
                   , NetworkService.GetCancellationToken()
                   , accessToken
                   , AppInfoManager.Instance.ViewInfo);
                //Mark: Check for 404 First
                if ((int)rawResponse.StatusCode != 200)
                {
                    response.StatusDetail = new StatusDetail();
                    response.StatusDetail = AWSConstants.Services.GetEKYCIdentification.GetStatusDetails(MobileConstants.DEFAULT);
                    response.StatusDetail.IsSuccess = false;
                    return response;
                }

                string responseString = await rawResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<GetEKYCIdentificationResponse>(responseString);
                if (response != null
                    && response.Content != null
                    && response.StatusDetail != null
                    && response.StatusDetail.Code.IsValid())
                {
                    response.StatusDetail = AWSConstants.Services.GetEKYCIdentification.GetStatusDetails(response.StatusDetail.Code);
                    GetEKYCStatusResponse ekycStatusResponse = await GetEKYCStatus(userID
                        , accessToken);
                    if (ekycStatusResponse.Content != null
                        && ekycStatusResponse.StatusDetail != null
                        && ekycStatusResponse.StatusDetail.IsSuccess)
                    {
                        response.Content.Status = ekycStatusResponse.Content.Status;
                    }
                }
                else
                {
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.GetEKYCIdentification.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetEKYCIdentificationResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = AWSConstants.Services.GetEKYCIdentification.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                }
                Debug.WriteLine("[DEBUG] [GetEKYCIdentification]: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [GetEKYCIdentification] Refit Exception: " + apiEx.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [GetEKYCIdentification] General Exception: " + ex.Message);
#endif
            }

            response = new GetEKYCIdentificationResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetEKYCIdentification.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }
    }
}