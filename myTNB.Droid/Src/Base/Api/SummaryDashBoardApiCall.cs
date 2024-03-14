using Android.Util;
using myTNB.Mobile.Business;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.SummaryDashBoard.API;
using myTNB.Android.Src.SummaryDashBoard.Models;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.Base.Api
{
    public class SummaryDashBoardApiCall
    {

        public SummaryDashBoardApiCall()
        {
        }


        public static async Task<List<SummaryDashBoardDetails>> GetSummaryDetails(SummaryDashBordRequest summaryDashboardRequest)
        {
            List<SummaryDashBoardDetails> summaryDetails = new List<SummaryDashBoardDetails>();
            CancellationTokenSource cts = new CancellationTokenSource();
#if STUB
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<ISummaryDashBoard>(httpClient);

#elif DEVELOP
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#else
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#endif



            try
            {
                var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(summaryDashboardRequest);
                var response = await api.GetLinkedAccountsSummaryInfo(encryptedRequest, cts.Token);

                if (response != null)
                {
                    if (response.Data != null)
                    {
                        var summaryDetailsReponse = response.Data.data;
                        if (summaryDetailsReponse != null && summaryDetailsReponse.Count > 0)
                        {
                            for (int i = 0; i < summaryDetailsReponse.Count; i++)
                            {
                                CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetailsReponse[i].AccNumber);
                                summaryDetailsReponse[i].AccName = cbAccount.AccDesc;
                                summaryDetailsReponse[i].AccType = cbAccount.AccountCategoryId;
                                summaryDetailsReponse[i].IsAccSelected = cbAccount.IsSelected;

                                /*** Save account data For the Day***/
                                SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                                accountModel.Timestamp = DateTime.Now.ToLocalTime();
                                accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetailsReponse[i]);
                                accountModel.AccountNo = summaryDetailsReponse[i].AccNumber;
                                SummaryDashBoardAccountEntity.InsertItem(accountModel);
                                /*****/

                                summaryDetails.AddRange(summaryDetailsReponse);
                            }
                        }
                    }
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("SummaryDashboardApiCall", "Cancelled Exception");
            }
            catch (ApiException apiException)
            {

            }
            catch (Exception e)
            {
                Log.Debug("SummaryDashboardApiCall", "Stack " + e.StackTrace);
            }



            return summaryDetails;
        }
    }
}
