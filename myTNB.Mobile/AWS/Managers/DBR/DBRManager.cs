﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Models.DBR;
using myTNB.Mobile.AWS.Services.DBR;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;

namespace myTNB.Mobile
{
    public sealed class DBRManager
    {
        private static readonly Lazy<DBRManager> lazy =
            new Lazy<DBRManager>(() => new DBRManager());

        public static DBRManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public DBRManager() { }

        /// <summary>
        /// Gets Bill Rendering Method
        /// </summary>
        /// <param name="ca">Electricity Account Number</param>
        /// <param name="accessToken">Generated Access Token</param>
        /// <returns>Rendeting method of CA</returns>
        public async Task<GetBillRenderingResponse> GetBillRendering(string ca
            , string accessToken)
        {
            GetBillRenderingResponse response = new GetBillRenderingResponse();
            if (IsBillRenderingEnabled)
            {
                try
                {
                    IDBRService service = RestService.For<IDBRService>(AWSConstants.Domains.GetBillRendering);
                    HttpResponseMessage rawResponse = await service.GetBillRendering(ca
                       , NetworkService.GetCancellationToken()
                       , accessToken
                       , AppInfoManager.Instance.ViewInfo);
                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = AWSConstants.Services.GetBillRendering.GetStatusDetails(MobileConstants.DEFAULT);
                        response.StatusDetail.IsSuccess = false;
                        return response;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<GetBillRenderingResponse>(responseString);
                    if (response != null
                        && response.Content != null
                        && response.StatusDetail != null
                        && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.GetBillRendering.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                        {
                            response.StatusDetail = AWSConstants.Services.GetBillRendering.GetStatusDetails(response.StatusDetail.Code);
                        }
                        else
                        {
                            response = new GetBillRenderingResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            response.StatusDetail = AWSConstants.Services.GetBillRendering.GetStatusDetails(MobileConstants.DEFAULT);
                        }
                    }
                    Debug.WriteLine("[DEBUG] [GetBillRendering]: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [GetBillRendering] Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [GetBillRendering] General Exception: " + ex.Message);
#endif
                }
            }
            response = new GetBillRenderingResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetBillRendering.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }

        public async Task<PostMultiBillRenderingResponse> PostMultiBillRendering(List<string> caList
            , string accessToken)
        {
            PostMultiBillRenderingResponse response = new PostMultiBillRenderingResponse();
            if (IsBillRenderingEnabled)
            {
                try
                {
                    PostMultiBillRenderingRequest request = new PostMultiBillRenderingRequest
                    {
                        CANumbers = caList
                    };

                    IDBRService service = RestService.For<IDBRService>(AWSConstants.Domains.GetMultiBillRendering);
                    HttpResponseMessage rawResponse = await service.PostMultiBillRendering(request
                       , NetworkService.GetCancellationToken()
                       , accessToken
                       , AppInfoManager.Instance.ViewInfo);
                    //Mark: Check for 404 First
                    if ((int)rawResponse.StatusCode != 200)
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = AWSConstants.Services.PostMultiBillRendering.GetStatusDetails(MobileConstants.DEFAULT);
                        response.StatusDetail.IsSuccess = false;
                        return response;
                    }

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<PostMultiBillRenderingResponse>(responseString);
                    if (response != null
                        && response.Content != null
                        && response.StatusDetail != null
                        && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.PostMultiBillRendering.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                        {
                            response.StatusDetail = AWSConstants.Services.PostMultiBillRendering.GetStatusDetails(response.StatusDetail.Code);
                        }
                        else
                        {
                            response = new PostMultiBillRenderingResponse
                            {
                                StatusDetail = new StatusDetail()
                            };
                            response.StatusDetail = AWSConstants.Services.PostMultiBillRendering.GetStatusDetails(MobileConstants.DEFAULT);
                        }
                    }
                    Debug.WriteLine("[DEBUG] [PostMultiBillRendering]: " + JsonConvert.SerializeObject(response));
                    return response;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostMultiBillRendering] Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG] [PostMultiBillRendering] General Exception: " + ex.Message);
#endif
                }
            }
            response = new PostMultiBillRenderingResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.PostMultiBillRendering.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }

        private bool IsBillRenderingEnabled
        {
            get
            {
                JToken config = LanguageManager.Instance.GetServiceConfig("ServiceConfiguration", "BillRendering");
                if (config != null
                    && config["enabled"] is JToken enabledJToken
                    && enabledJToken != null)
                {
                    return enabledJToken.ToObject<bool>();
                }
                return false;
            }
        }
    }
}