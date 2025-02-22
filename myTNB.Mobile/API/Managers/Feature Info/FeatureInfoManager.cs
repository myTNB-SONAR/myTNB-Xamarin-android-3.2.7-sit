﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.BaseModel;
using myTNB.Mobile.API.Models.FeatureInfo;
using myTNB.Mobile.API.Services.FeatureInfo;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.Business;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;
using static myTNB.Mobile.EligibilitySessionCache;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB.Mobile
{
    public class FeatureInfoManager
    {
        public FeatureInfoManager() { }

        private static readonly Lazy<FeatureInfoManager> lazy =
         new Lazy<FeatureInfoManager>(() => new FeatureInfoManager());

        public static FeatureInfoManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public enum QueueTopicEnum
        {
            addca,
            getca,
            removeca
        }

        private List<FeatureInfo> Data { set; get; }

        public List<FeatureInfo> GetFeatureInfo()
        {

            if (Data != null)
            {
                return Data;
            }
            else
            {
                return new List<FeatureInfo>();
            }
        }

        public void SetData(GetEligibilityResponse response)
        {
            try
            {
                List<FeatureInfo> ListOfFeature = new List<FeatureInfo>();

                //Convert Enum to list
                List<string> TypeOfFeature = new List<string>() { Features.EB.ToString() };

                if (EBUtility.Instance.IsAccountEligible)
                {

                    List<FeaturesContractAccount> eligibleCA = new List<FeaturesContractAccount>();

                    //Get AWS EB ca
                    List<string> ebCAs = EBUtility.Instance.GetCAList();
                    if (ebCAs != null)
                    {
                        foreach (var ca in ebCAs)
                        {
                            eligibleCA.Add(new FeaturesContractAccount
                            {
                                contractAccount = ca,
                                acted = true,
                                modifiedDate = ""
                            });
                        }
                    }
                    else
                    {   // Add dummy CA if target group false
                        eligibleCA.Add(new FeaturesContractAccount
                        {
                            contractAccount = "1010101001010101",
                            acted = true,
                            modifiedDate = ""
                        });
                    }

                    ListOfFeature.Add(new FeatureInfo
                    {
                        FeatureName = Features.EB.ToString(),
                        ContractAccount = eligibleCA
                    });
                }

                if (SDUtility.Instance.IsAccountEligible)
                {

                    List<FeaturesContractAccount> eligibleCA = new List<FeaturesContractAccount>();

                    //Get AWS EB ca
                    List<string> sdCAs = SDUtility.Instance.GetCAList();


                    if (sdCAs != null)
                    {
                        foreach (var ca in sdCAs)
                        {
                            eligibleCA.Add(new FeaturesContractAccount
                            {
                                contractAccount = ca,
                                acted = true,
                                modifiedDate = ""
                            });
                        }
                    }
                    else
                    {   // Add dummy CA if target group false
                        eligibleCA.Add(new FeaturesContractAccount
                        {
                            contractAccount = "1010101001010101",
                            acted = true,
                            modifiedDate = ""
                        });
                    }

                    ListOfFeature.Add(new FeatureInfo
                    {
                        FeatureName = Features.SD.ToString(),
                        ContractAccount = eligibleCA
                    });
                }


                Data = ListOfFeature;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] FeatureInfoManager SetData Error: " + e.Message);
                Data = new List<FeatureInfo>();
            }
        }

        public void Clear()
        {
            Data = new List<FeatureInfo>();
        }

        #region Save Feature Info
        public async Task<PostSaveFeatureInfoResponse> SaveFeatureInfo(List<ContractAccountModel> accounts
            , QueueTopicEnum queueTopic
            , object userInfo
            , object deviceInfo)
        {
            try
            {
                IFeatureInfoService service = RestService.For<IFeatureInfoService>(MobileConstants.ApiDomain);
                try
                {
                    PostSaveFeatureInfoRequest request = new PostSaveFeatureInfoRequest
                    {
                        Accounts = accounts,
                        QueueTopic = queueTopic.ToString(),
                        FeatureInfo = GetFeatureInfo(),
                        UsrInf = userInfo,
                        DeviceInf = deviceInfo
                    };
                    Debug.WriteLine("[DEBUG] SaveFeatureInfo Request: " + JsonConvert.SerializeObject(request));
                    EncryptedRequest encryptedRequest = APISecurityManager.Instance.GetEncryptedRequest(request);
                    HttpResponseMessage rawResponse = await service.SaveFeatureInfo(encryptedRequest
                        , AppInfoManager.Instance.GetUserInfo()
                        , API.NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    BaseASMXModel responseModel = await rawResponse.ParseAsync<BaseASMXModel>();
                    PostSaveFeatureInfoResponse response = new PostSaveFeatureInfoResponse
                    {
                        d = responseModel
                    };
                    Debug.WriteLine("[DEBUG] SaveFeatureInfo Success Response: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SaveFeatureInfo]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][SaveFeatureInfo]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            PostSaveFeatureInfoResponse res = new PostSaveFeatureInfoResponse
            {
                d = new BaseASMXModel()
            };
            Debug.WriteLine("[DEBUG] SaveFeatureInfo Error Response: " + JsonConvert.SerializeObject(res));
            return res;
        }
        #endregion
    }
}