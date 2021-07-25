using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Services.DBR;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
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
                    response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
                    response.StatusDetail.IsSuccess = false;
                    return response;
                }

                string responseString = await rawResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<GetBillRenderingResponse>(responseString);

                if (ca == "210007946106")
                {
                    response = JsonConvert.DeserializeObject<GetBillRenderingResponse>(ca_210007946106);
                }
                if (ca == "210008964806")
                {
                    response = JsonConvert.DeserializeObject<GetBillRenderingResponse>(ca_210008964806);
                }
                if (ca == "210019137106")
                {
                    response = JsonConvert.DeserializeObject<GetBillRenderingResponse>(ca_210019137106);
                }
                if (ca == "210033055708")
                {
                    response = JsonConvert.DeserializeObject<GetBillRenderingResponse>(ca_210033055708);
                }
                if (ca == "210124772804")
                {
                    response = JsonConvert.DeserializeObject<GetBillRenderingResponse>(ca_210124772804);
                }

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

                return response;
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][GetBillRendering]Refit Exception: " + apiEx.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][GetBillRendering]General Exception: " + ex.Message);
#endif
            }

            response = new GetBillRenderingResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetBillRendering.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }

        private string ca_210007946106 = "{ \"content\": { \"caNo\": \"210007946106\", \"bpNo\": \"1100896326\", \"digitalBillEligibility\": \"X\", \"digitalBillStatus\": \"X\", \"ownerBillRenderingMethod\": \"ZV02\", \"ownerBillingEmail\": \"chris.lui@avanade.com\", \"bcRecord\": [ { \"bpNo\": \"1200000017\", \"firstName\": null, \"lastName\": null, \"renderingMethod\": \"ZNTF\", \"billingEmail\": null } ], \"isInProgress\": false, \"isUpdateCtaAllow\": true, \"isUpdateCtaOptInPaperBill\": true }, \"statusDetail\": { \"code\": \"7200\", \"title\": \"Success\", \"description\": \"Success\", \"displayMode\": null, \"ctaText\": null } }";
        private string ca_210008964806 = "{ \"content\": { \"caNo\": \"210008964806\", \"bpNo\": \"1100896326\", \"digitalBillEligibility\": \"X\", \"digitalBillStatus\": \"X\", \"ownerBillRenderingMethod\": \"ZV03\", \"ownerBillingEmail\": \"chris.lui@avanade.com\", \"bcRecord\": [ { \"bpNo\": \"1200000017\", \"firstName\": null, \"lastName\": null, \"renderingMethod\": \"ZNTF\", \"billingEmail\": null } ], \"isInProgress\": false, \"isUpdateCtaAllow\": true, \"isUpdateCtaOptInPaperBill\": true }, \"statusDetail\": { \"code\": \"7200\", \"title\": \"Success\", \"description\": \"Success\", \"displayMode\": null, \"ctaText\": null } }";
        private string ca_210019137106 = "{ \"content\": { \"caNo\": \"210019137106\", \"bpNo\": \"1100896326\", \"digitalBillEligibility\": \"X\", \"digitalBillStatus\": \"X\", \"ownerBillRenderingMethod\": \"ZV03\", \"ownerBillingEmail\": \"chris.lui@avanade.com\", \"bcRecord\": [ { \"bpNo\": \"1200000017\", \"firstName\": null, \"lastName\": null, \"renderingMethod\": \"ZNTF\", \"billingEmail\": null } ], \"isInProgress\": false, \"isUpdateCtaAllow\": false, \"isUpdateCtaOptInPaperBill\": true }, \"statusDetail\": { \"code\": \"7200\", \"title\": \"Success\", \"description\": \"Success\", \"displayMode\": null, \"ctaText\": null } }";
        private string ca_210033055708 = "{ \"content\": { \"caNo\": \"210033055708\", \"bpNo\": \"1100896326\", \"digitalBillEligibility\": \"X\", \"digitalBillStatus\": \"X\", \"ownerBillRenderingMethod\": \"ZV04\", \"ownerBillingEmail\": \"chris.lui@avanade.com\", \"bcRecord\": [ { \"bpNo\": \"1200000017\", \"firstName\": null, \"lastName\": null, \"renderingMethod\": \"ZNTF\", \"billingEmail\": null } ], \"isInProgress\": false, \"isUpdateCtaAllow\": false, \"isUpdateCtaOptInPaperBill\": true }, \"statusDetail\": { \"code\": \"7200\", \"title\": \"Success\", \"description\": \"Success\", \"displayMode\": null, \"ctaText\": null } }";
        private string ca_210124772804 = "{ \"content\": { \"caNo\": \"210124772804\", \"bpNo\": \"1100896326\", \"digitalBillEligibility\": \"X\", \"digitalBillStatus\": \"X\", \"ownerBillRenderingMethod\": \"ZV04\", \"ownerBillingEmail\": \"chris.lui@avanade.com\", \"bcRecord\": [ { \"bpNo\": \"1200000017\", \"firstName\": null, \"lastName\": null, \"renderingMethod\": \"ZNTF\", \"billingEmail\": null } ], \"isInProgress\": false, \"isUpdateCtaAllow\": true, \"isUpdateCtaOptInPaperBill\": true }, \"statusDetail\": { \"code\": \"7200\", \"title\": \"Success\", \"description\": \"Success\", \"displayMode\": null, \"ctaText\": null } }";
    }
}