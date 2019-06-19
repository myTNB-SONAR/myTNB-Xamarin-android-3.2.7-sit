using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SummaryDashBoard.API;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Base.Api
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
            //mView.ShowProgressDialog();
#if STUB
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<ISummaryDashBoard>(httpClient);

#elif DEVELOP
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);

            //api.DoQuery(new Requests.UsageHistoryRequest(Constants.APP_CONFIG.API_KEY_ID) {
            //    AccountNum = accountSelected.AccNum
            //}, cts.Token)
            //.ReturnsForAnyArgs(
            //    Task.Run<UsageHistoryResponse>(
            //        () => JsonConvert.DeserializeObject<UsageHistoryResponse>(this.mView.GetUsageHistoryStub())
            //    ));
#else
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#endif



            try
            {
                var response = await api.GetLinkedAccountsSummaryInfo(summaryDashboardRequest, cts.Token);

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

                            //SummaryData(summaryDetailsReponse);
                        }
                    }
                }
                //    if (accountSelected.isOwned)
                //    {

                //        if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
                //        {
                //            this.mView.ShowAccountName();
                //            this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                //            if (smDataError)
                //            {
                //                smDataError = false;
                //                if (smErrorCode.Equals("204"))
                //                {
                //                    this.mView.ShowChartWithError(response.Data.UsageHistoryData, AccountData.Copy(accountSelected, true), smErrorCode);
                //                }
                //            }
                //            else
                //            {
                //                this.mView.ShowChart(response.Data.UsageHistoryData, AccountData.Copy(accountSelected, true));
                //            }
                //        }
                //        else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
                //        {
                //            this.mView.ShowAccountName();
                //            this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
                //            LoadBills(accountSelected);
                //        }


                //    }
                //    else
                //    {
                //        this.mView.ShowNonOWner(AccountData.Copy(accountSelected, true));
                //    }
                //    this.mView.SetAccountName(accountSelected.AccDesc);

                //}
                //else
                //{
                //    this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc);
                //    this.mView.SetAccountName(accountSelected.AccDesc);
                //}
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("SummaryDashboardApiCall", "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                //this.mView.ShowRetryOptionsCancelledException(e);
                //this.mView.ShowOwnerNoInternetConnection(accountSelected.AccDesc);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                //this.mView.ShowRetryOptionsApiException(apiException);

            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug("SummaryDashboardApiCall", "Stack " + e.StackTrace);
                //this.mView.ShowRetryOptionsUnknownException(e);

            }



            return summaryDetails;
        }
    }
}
