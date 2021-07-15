using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Services.DBR;
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
                response.StatusDetail = new StatusDetail
                {
                    IsSuccess = true
                };
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
    }
}