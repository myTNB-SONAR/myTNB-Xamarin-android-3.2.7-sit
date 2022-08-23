using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Services.DBR;
using myTNB.Mobile.AWS.Services.DS;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;

namespace myTNB.Mobile
{
    public sealed class EligibilityManager
    {
        private static readonly Lazy<EligibilityManager> lazy =
           new Lazy<EligibilityManager>(() => new EligibilityManager());

        public static EligibilityManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public EligibilityManager() { }

        private struct ConfigConstants
        {
            internal const string ServiceConfiguration = "ServiceConfiguration";
            internal const string CallInterval = "callInterval";
            internal const string Enabled = "enabled";
        }

        /// <summary>
        /// Get Eligible CAs
        /// Gte Eligible Features
        /// </summary>
        /// <param name="userID">User Id from IsUserAuthV2</param>
        /// <param name="caList">List of CA and it's properties from GetAccounts</param>
        /// <param name="accessToken">Generated Token</param>
        /// <returns></returns>
        public async Task<GetEligibilityResponse> PostEligibility(string userID
            , List<ContractAccountModel> caList
            , string accessToken)
        {
            PostEligibilityResponse postResponse = new PostEligibilityResponse();
            GetEligibilityResponse response = new GetEligibilityResponse();
            int maxAccountList = LanguageManager.Instance.GetConfigTimeout(LanguageManager.ConfigPropertyEnum.MaxAccountList);
            maxAccountList = maxAccountList == 0 ? MobileConstants.MaxAccountList : maxAccountList;
            if (caList != null
                && caList.Count <= maxAccountList)
            {
                try
                {
                    IDBRService service = RestService.For<IDBRService>(AWSConstants.Domains.Domain);
                    AppInfoManager.Instance.SetAccountList(caList);
                    PostEligibilityRequest request = new PostEligibilityRequest
                    {
                        UserID = userID ?? string.Empty,
                        ContractAccounts = caList
                    };
                    Debug.WriteLine("[DEBUG] PostEligibility Request: " + JsonConvert.SerializeObject(request));
                    Debug.WriteLine("[DEBUG] PostEligibility ViewInfo: " + AppInfoManager.Instance.ViewInfo);

                    HttpResponseMessage rawResponse = await service.PostEligibility(request
                       , NetworkService.GetCancellationToken()
                       , accessToken
                       , AppInfoManager.Instance.ViewInfo);

                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        GetEligibilityResponse httpErrorResponse = new GetEligibilityResponse();
                        httpErrorResponse.StatusDetail = new StatusDetail();
                        httpErrorResponse.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
                        httpErrorResponse.StatusDetail.IsSuccess = false;
                        return httpErrorResponse;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    postResponse = JsonConvert.DeserializeObject<PostEligibilityResponse>(responseString);
                    if (postResponse != null
                        && postResponse.Content != null
                        && postResponse.StatusDetail != null
                        && postResponse.StatusDetail.Code.IsValid())
                    {
                        postResponse.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(postResponse.StatusDetail.Code);

                        response.StatusDetail = postResponse.StatusDetail;
                        response.Content = new GetEligibilityModel
                        {
                            EligibileFeatures = postResponse.Content.EligibileFeaturesList
                        };
                        ParsePostEleigibilityFeature(ref response, postResponse);
                    }
                    else
                    {
                        if (postResponse != null
                            && postResponse.StatusDetail != null
                            && postResponse.StatusDetail.Code.IsValid())
                        {
                            postResponse.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(postResponse.StatusDetail.Code);
                        }
                        else
                        {
                            postResponse = new PostEligibilityResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            postResponse.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
                        }
                        response.StatusDetail = postResponse.StatusDetail;
                    }

                    Debug.WriteLine("[DEBUG] PostEligibility Response: " + JsonConvert.SerializeObject(postResponse));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][PostEligibility]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][PostEligibility]General Exception: " + ex.Message);
#endif
                }
            }

            response = new GetEligibilityResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }

        private void ParsePostEleigibilityFeature(ref GetEligibilityResponse eligibilityResponse
            , PostEligibilityResponse postEligibilityResponse)
        {
            try
            {
                if (postEligibilityResponse != null
                    && postEligibilityResponse.Content != null
                    && postEligibilityResponse.Content.FeatureCAList != null
                    && postEligibilityResponse.Content.FeatureCAList.Count > 0)
                {
                    List<FeatureCAModel> dbr = postEligibilityResponse.Content.FeatureCAList.FindAll(
                        x => x.FeatureName.ToUpper() == EligibilitySessionCache.Features.DBR.ToString().ToUpper()).ToList();
                    List<FeatureCAModel> br = postEligibilityResponse.Content.FeatureCAList.FindAll(
                        x => x.FeatureName.ToUpper() == EligibilitySessionCache.Features.BR.ToString().ToUpper()).ToList();
                    List<FeatureCAModel> eb = postEligibilityResponse.Content.FeatureCAList.FindAll(
                        x => x.FeatureName.ToUpper() == EligibilitySessionCache.Features.EB.ToString().ToUpper()).ToList();
                    List<FeatureCAModel> sd = postEligibilityResponse.Content.FeatureCAList.FindAll(
                        x => x.FeatureName.ToUpper() == EligibilitySessionCache.Features.SD.ToString().ToUpper()).ToList();
                    List<FeatureCAModel> ds = postEligibilityResponse.Content.FeatureCAList.FindAll(
                        x => x.FeatureName.ToUpper() == EligibilitySessionCache.Features.DS.ToString().ToUpper()).ToList();
                    if (dbr != null && dbr.Count > 0)
                    {
                        BaseCAListModel baseContent = new BaseCAListModel
                        {
                            ContractAccounts = new List<ContractAccountsModel>()
                        };
                        for (int i = 0; i < dbr.Count; i++)
                        {
                            FeatureCAModel item = dbr[i];
                            baseContent.ContractAccounts.Add(new ContractAccountsModel
                            {
                                ContractAccount = item.ContractAccount,
                                Acted = item.Acted,
                                ModifiedDate = item.ModifiedDate
                            });
                        }
                        eligibilityResponse.Content.DBR = baseContent;
                    }

                    if (br != null && br.Count > 0)
                    {
                        BaseCAListModel baseContent = new BaseCAListModel
                        {
                            ContractAccounts = new List<ContractAccountsModel>()
                        };
                        for (int i = 0; i < br.Count; i++)
                        {
                            FeatureCAModel item = br[i];
                            baseContent.ContractAccounts.Add(new ContractAccountsModel
                            {
                                ContractAccount = item.ContractAccount,
                                Acted = item.Acted,
                                ModifiedDate = item.ModifiedDate
                            });
                        }
                        eligibilityResponse.Content.BR = baseContent;
                    }

                    if (eb != null && eb.Count > 0)
                    {
                        BaseCAListModel baseContent = new BaseCAListModel
                        {
                            ContractAccounts = new List<ContractAccountsModel>()
                        };
                        for (int i = 0; i < eb.Count; i++)
                        {
                            FeatureCAModel item = eb[i];
                            baseContent.ContractAccounts.Add(new ContractAccountsModel
                            {
                                ContractAccount = item.ContractAccount,
                                Acted = item.Acted,
                                ModifiedDate = item.ModifiedDate
                            });
                        }
                        eligibilityResponse.Content.EB = baseContent;
                    }

                    if (sd != null && sd.Count > 0)
                    {
                        BaseCAListModel baseContent = new BaseCAListModel
                        {
                            ContractAccounts = new List<ContractAccountsModel>()
                        };
                        for (int i = 0; i < sd.Count; i++)
                        {
                            FeatureCAModel item = sd[i];
                            baseContent.ContractAccounts.Add(new ContractAccountsModel
                            {
                                ContractAccount = item.ContractAccount,
                                Acted = item.Acted,
                                ModifiedDate = item.ModifiedDate
                            });
                        }
                        eligibilityResponse.Content.SD = baseContent;
                    }

                    if (ds != null && ds.Count > 0)
                    {
                        BaseCAListModel baseContent = new BaseCAListModel
                        {
                            ContractAccounts = new List<ContractAccountsModel>()
                        };
                        for (int i = 0; i < ds.Count; i++)
                        {
                            FeatureCAModel item = ds[i];
                            baseContent.ContractAccounts.Add(new ContractAccountsModel
                            {
                                ContractAccount = item.ContractAccount,
                                Acted = item.Acted,
                                ModifiedDate = item.ModifiedDate
                            });
                        }
                        eligibilityResponse.Content.DS = baseContent;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG][ParsePostEleigibilityFeature]General Exception: " + e.Message);
            }
        }

        /// <summary>
        /// Returns the API interval of specified service
        /// </summary>
        /// <param name="serviceName">Name of service to query from language file under ServiceConfiguration</param>
        /// <returns>Value in Minutes</returns>
        public int GetAPICallInterval(string serviceName)
        {
            JToken config = LanguageManager.Instance.GetServiceConfig(ConfigConstants.ServiceConfiguration, serviceName);
            if (config != null
                && config[ConfigConstants.CallInterval] is JToken enabledJToken
                && enabledJToken != null)
            {
                return enabledJToken.ToObject<int>();
            }
            return 0;
        }

        /// <summary>
        /// Checks if service is enabled based on language file
        /// </summary>
        /// <param name="serviceName">Name of service to query from language file under ServiceConfiguration</param>
        /// <returns>isEnabled</returns>
        public bool IsEnabled(string serviceName)
        {
            JToken config = LanguageManager.Instance.GetServiceConfig(ConfigConstants.ServiceConfiguration, serviceName);
            if (config != null
                && config[ConfigConstants.Enabled] is JToken enabledJToken
                && enabledJToken != null)
            {
                return enabledJToken.ToObject<bool>();
            }
            return false;
        }

        /// <summary>
        /// Determines if Service should be called based on language config
        /// </summary>
        /// <param name="serviceName">Name of service to query from language file under ServiceConfiguration</param>
        /// <param name="saveTimeStamp">saved time stamp from device</param>
        /// <returns></returns>
        public bool ShouldCallApi(string serviceName
            , string saveTimeStamp)
        {
            if (IsEnabled(serviceName))
            {
                if (!saveTimeStamp.IsValid())
                {
                    return true;
                }
                DateTime timeStamp = DateTime.Parse(saveTimeStamp);
                int interval = GetAPICallInterval(serviceName);
                DateTime now = DateTime.Now;
                TimeSpan timeSpan = now - timeStamp;
                double minutes = timeSpan.TotalMinutes;
                return interval < minutes;
            }
            return false;
        }


        internal async Task<GetEligibilityResponse> PostEligibilityByCriteria(List<string> businessAreaList
            , string accessToken)
        {
            PostEligibilityResponse postResponse = new PostEligibilityResponse();
            GetEligibilityResponse response = new GetEligibilityResponse();
            if (businessAreaList != null && businessAreaList.Count > 0)
            {
                try
                {
                    IDSService service = RestService.For<IDSService>(AWSConstants.Domains.Domain);
                    List<PremiseCriteriaModel> premistCriteriaList = new List<PremiseCriteriaModel>();
                    for (int i = 0; i < businessAreaList.Count; i++)
                    {
                        premistCriteriaList.Add(new PremiseCriteriaModel
                        {
                            BusinessArea = businessAreaList[i]
                        });
                    }

                    PostEligibilityByCriteriaRequest request = new PostEligibilityByCriteriaRequest
                    {
                        PremiseCriteria = premistCriteriaList
                    };

                    Debug.WriteLine("[DEBUG] PostEligibilityByCriteria Request: " + JsonConvert.SerializeObject(request));
                    Debug.WriteLine("[DEBUG] PostEligibilityByCriteria ViewInfo: " + AppInfoManager.Instance.ViewInfo);

                    HttpResponseMessage rawResponse = await service.PostEligibilityByCriteria(request
                       , NetworkService.GetCancellationToken()
                       , accessToken
                       , AppInfoManager.Instance.ViewInfo);

                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        GetEligibilityResponse httpErrorResponse = new GetEligibilityResponse();
                        httpErrorResponse.StatusDetail = new StatusDetail();
                        httpErrorResponse.StatusDetail = AWSConstants.Services.EligibilityByCriteria.GetStatusDetails(MobileConstants.DEFAULT);
                        httpErrorResponse.StatusDetail.IsSuccess = false;
                        return httpErrorResponse;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    postResponse = JsonConvert.DeserializeObject<PostEligibilityResponse>(responseString);
                    if (postResponse != null
                        && postResponse.Content != null
                        && postResponse.StatusDetail != null
                        && postResponse.StatusDetail.Code.IsValid())
                    {
                        postResponse.StatusDetail = AWSConstants.Services.EligibilityByCriteria.GetStatusDetails(postResponse.StatusDetail.Code);

                        response.StatusDetail = postResponse.StatusDetail;
                        response.Content = new GetEligibilityModel
                        {
                            EligibileFeatures = postResponse.Content.EligibileFeaturesList
                        };
                        ParsePostEleigibilityByCriteriaFeature(ref response, postResponse);
                    }
                    else
                    {
                        if (postResponse != null
                            && postResponse.StatusDetail != null
                            && postResponse.StatusDetail.Code.IsValid())
                        {
                            postResponse.StatusDetail = AWSConstants.Services.EligibilityByCriteria.GetStatusDetails(postResponse.StatusDetail.Code);
                        }
                        else
                        {
                            postResponse = new PostEligibilityResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            postResponse.StatusDetail = AWSConstants.Services.EligibilityByCriteria.GetStatusDetails(MobileConstants.DEFAULT);
                        }
                        response.StatusDetail = postResponse.StatusDetail;
                    }

                    Debug.WriteLine("[DEBUG] PostEligibilityByCriteria postResponse: " + JsonConvert.SerializeObject(postResponse));
                    Debug.WriteLine("[DEBUG] PostEligibilityByCriteria response: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][PostEligibilityByCriteria]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][PostEligibilityByCriteria]General Exception: " + ex.Message);
#endif
                }
            }

            response = new GetEligibilityResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }

        private void ParsePostEleigibilityByCriteriaFeature(ref GetEligibilityResponse eligibilityResponse
            , PostEligibilityResponse postEligibilityResponse)
        {
            try
            {
                if (postEligibilityResponse != null
                    && postEligibilityResponse.Content != null
                    && postEligibilityResponse.Content.EligibilityByCriteria != null
                    && postEligibilityResponse.Content.EligibilityByCriteria.Count > 0)
                {
                    List<FeatureCAModel> ds = postEligibilityResponse.Content.EligibilityByCriteria.FindAll(
                        x => x.FeatureName.ToUpper() == EligibilitySessionCache.Features.DS.ToString().ToUpper()).ToList();
                    if (ds != null && ds.Count > 0)
                    {
                        BaseCAListModel baseContent = new BaseCAListModel
                        {
                            ContractAccounts = new List<ContractAccountsModel>()
                        };
                        for (int i = 0; i < ds.Count; i++)
                        {
                            FeatureCAModel item = ds[i];
                            baseContent.ContractAccounts.Add(new ContractAccountsModel
                            {
                                ContractAccount = item.ContractAccount,
                                Acted = item.Acted,
                                ModifiedDate = item.ModifiedDate,
                                BusinessArea = item.BusinessArea
                            });
                        }
                        eligibilityResponse.Content.DS = baseContent;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG][ParsePostEleigibilityByCriteriaFeature]General Exception: " + e.Message);
            }
        }
    }
}