using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API.Managers.Home.Utilities;
using myTNB.Mobile.API.Models.Home.PostServices;
using myTNB.Mobile.API.Services.Home;
using myTNB.Mobile.Business;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Mobile.AWS.Managers.Home
{
    public sealed class HomeManager
    {
        private static readonly Lazy<HomeManager> lazy =
            new Lazy<HomeManager>(() => new HomeManager());

        public static HomeManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public HomeManager() { }

        public async Task<PostServicesResponse> PostServices(string timeStamp)
        {
            PostServicesResponse response = new PostServicesResponse();

            try
            {
                PostServicesRequest request = new PostServicesRequest
                {
                    UserInfo = new UserInfo
                    {
                        UserName = AppInfoManager.Instance.UserName,
                        UserID = AppInfoManager.Instance.UserId,
                        DeviceID = AppInfoManager.Instance.ViewInfoHeader.DeviceToken,
                        FCMToken = AppInfoManager.Instance.FCMToken,
                        Language = AppInfoManager.Instance.Lang,
                        sec_auth_k1 = MobileConstants.ApiKeyId,
                        sec_auth_k2 = string.Empty,
                        ses_param1 = string.Empty,
                        ses_param2 = string.Empty
                    },
                    DeviceInfo = new myTNB.Mobile.API.Models.Home.PostServices.DeviceInfo
                    {
                        DeviceId = AppInfoManager.Instance.ViewInfoHeader.DeviceToken,
                        AppVersion = AppInfoManager.Instance.ViewInfoHeader.AppVersion,
                        OsType = AppInfoManager.Instance.ViewInfoHeader.OSType,
                        OsVersion = AppInfoManager.Instance.OSVersion,
                        DeviceDesc = string.Empty
                    }
                };
                EncryptedRequest encryptedRequest = APISecurityManager.Instance.GetEncryptedRequest(request);
                IHomeService service = RestService.For<IHomeService>(MobileConstants.ApiDomain);
                HttpResponseMessage rawResponse = await service.PostServices(encryptedRequest
                    , API.NetworkService.GetCancellationToken()
                    , AppInfoManager.Instance.GetUserInfo()
                    , AppInfoManager.Instance.Language.ToString());
                //Mark: Check for 404 First
                if ((int)rawResponse.StatusCode != 200)
                {
                    response.Data = new BaseResponse<PostServicesModel>();
                    response.Data.StatusDetail = new StatusDetail();
                    response.Data.StatusDetail = AWSConstants.Services.PostServices.GetStatusDetails(MobileConstants.DEFAULT);
                    response.Data.StatusDetail.IsSuccess = false;
                    return response;
                }

                string responseString = await rawResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<PostServicesResponse>(responseString);
                if (response != null
                    && response.Data != null
                    && response.Data.Content != null
                    && response.Data.StatusDetail != null
                    && response.Data.StatusDetail.Code.IsValid())
                {
                    response.Data.StatusDetail = AWSConstants.Services.PostServices.GetStatusDetails(response.Data.StatusDetail.Code);
                    if (response.Data.StatusDetail.IsSuccess)
                    {
                        response.Data.Content.SavedTimeStamp = timeStamp;
                    }
                }
                else
                {
                    if (response != null
                        && response.Data != null
                        && response.Data.StatusDetail != null
                        && response.Data.StatusDetail.Code.IsValid())
                    {
                        response.Data.StatusDetail = AWSConstants.Services.PostServices.GetStatusDetails(response.Data.StatusDetail.Code);
                        if (response.Data.StatusDetail.IsSuccess
                            && response.Data.Content != null)
                        {
                            response.Data.Content.SavedTimeStamp = timeStamp;
                        }
                    }
                    else
                    {
                        response = new PostServicesResponse
                        {
                            Data = new BaseResponse<PostServicesModel>()
                        };
                        response.Data.StatusDetail = new StatusDetail();
                        response.Data.StatusDetail = AWSConstants.Services.PostServices.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                }
                Debug.WriteLine("[DEBUG] [PostServices]: " + JsonConvert.SerializeObject(response));
                HomeUtility.UpdateResponse(ref response, AppInfoManager.Instance.ViewInfoHeader.AppVersion);
                Debug.WriteLine("[DEBUG] [PostServices] Parsed: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [PostServices] Refit Exception: " + apiEx.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [PostServices] General Exception: " + ex.Message);
#endif
            }

            response = new PostServicesResponse
            {
                Data = new BaseResponse<PostServicesModel>()
            };
            response.Data.StatusDetail = new StatusDetail();
            response.Data.StatusDetail = AWSConstants.Services.PostServices.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }
    }
}