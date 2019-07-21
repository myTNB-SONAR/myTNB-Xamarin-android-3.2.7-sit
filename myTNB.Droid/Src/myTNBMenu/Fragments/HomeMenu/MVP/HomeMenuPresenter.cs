﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Service;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuPresenter : HomeMenuContract.IHomeMenuPresenter
    {
        HomeMenuContract.IHomeMenuView mView;
        HomeMenuContract.IHomeMenuService serviceApi;
        private Constants.GREETING greeting;
        public HomeMenuPresenter(HomeMenuContract.IHomeMenuView view)
        {
            this.mView = view;
            this.serviceApi = new HomeMenuServiceImpl();
        }

        public string GetAccountDisplay()
        {
            return UserEntity.GetActive().DisplayName;
        }

        public Constants.GREETING GetGreeting()
        {
            DateTime dt = DateTime.Now.ToLocalTime();
            int hour_only = dt.Hour;

            if (hour_only >= 6 && hour_only < 12)
            {
                greeting = Constants.GREETING.MORNING;
            }
            else if (hour_only >= 12 && hour_only < 18)
            {
                greeting = Constants.GREETING.AFTERNOON;
            }
            else if (hour_only >= 0 && hour_only < 6)
            {
                greeting = Constants.GREETING.EVENING;
            }
            else
            {
                greeting = Constants.GREETING.EVENING;
            }

            return greeting;
        }

        private async Task GetAccountSummaryInfo(SummaryDashBordRequest request)
        {
            SummaryDashBoardResponse response = await this.serviceApi.GetLinkedSummaryInfo(request);
            if (response != null)
            {
                if (response.Data != null && response.Data.Status.ToUpper() == Constants.REFRESH_MODE)
                {
                    //mView.ShowRefreshSummaryDashboard(true, response.Data.RefreshMessage, response.Data.RefreshBtnText);
                    //LoadEmptySummaryDetails();
                }
                else if (response.Data != null && !response.Data.isError && response.Data.data != null && response.Data.data.Count > 0)
                {
                    List<SummaryDashBoardDetails> summaryDetails = response.Data.data;
                    for (int i = 0; i < summaryDetails.Count; i++)
                    {
                        CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
                        summaryDetails[i].AccName = cbAccount.AccDesc;
                        summaryDetails[i].AccType = cbAccount.AccountCategoryId;
                        summaryDetails[i].IsAccSelected = cbAccount.IsSelected;

                        /*** Save account data For the Day***/
                        SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                        accountModel.Timestamp = DateTime.Now.ToLocalTime();
                        accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                        accountModel.AccountNo = summaryDetails[i].AccNumber;
                        SummaryDashBoardAccountEntity.InsertItem(accountModel);
                        /*****/
                    }


                    List<SummaryDashBoardDetails> reAccount = (from item in summaryDetails
                                                               where item.AccType == "2"
                                                               select item).ToList();


                    List<SummaryDashBoardDetails> normalAccount = (from item in summaryDetails
                                                                   where item.AccType != "2"
                                                                   select item).ToList();


                    List<SummaryDashBoardDetails> totalAccountList = new List<SummaryDashBoardDetails>();
                    totalAccountList.AddRange(reAccount.OrderBy(x => x.AccName).ToList());
                    totalAccountList.AddRange(normalAccount.OrderBy(x => x.AccName).ToList());
                    this.mView.UpdateAccountListCards(totalAccountList);
                    //SummaryData(summaryDetails);
                    //mView.ShowRefreshSummaryDashboard(false, null, null);

                }
                else
                {
                    //mView.ShowRefreshSummaryDashboard(true, null, null);
                    //LoadEmptySummaryDetails();
                }
            }
        }

        public void LoadAccounts()
        {
            var RenewableAccountList = CustomerBillingAccount.REAccountList();
            var NonRenewableAccountList = CustomerBillingAccount.NonREAccountList();

            List<CustomerBillingAccount> customerBillingAccountList = new List<CustomerBillingAccount>();
            customerBillingAccountList.AddRange(RenewableAccountList);
            customerBillingAccountList.AddRange(NonRenewableAccountList);

            List<SummaryDashBoardDetails> summaryDashboardDetailList = new List<SummaryDashBoardDetails>();
            foreach (CustomerBillingAccount customerBillintAccount in customerBillingAccountList)
            {
                SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();
                summaryDashBoardDetails.AccNumber = customerBillintAccount.AccNum;
                summaryDashboardDetailList.Add(summaryDashBoardDetails);
            }

            this.mView.SetAccountListCards(summaryDashboardDetailList);

            List<string> accountList = new List<string>();
            foreach (CustomerBillingAccount customerBillingAccount in customerBillingAccountList)
            {
                if (!string.IsNullOrEmpty(customerBillingAccount.AccNum))
                {
                    accountList.Add(customerBillingAccount.AccNum);
                }
            }

            if (accountList.Count > 0)
            {
                SummaryDashBordRequest request = new SummaryDashBordRequest();
                request.AccNum = accountList;
                request.SspUserId = UserEntity.GetActive().UserID;
                request.ApiKeyId = Constants.APP_CONFIG.API_KEY_ID;
                _ = GetAccountSummaryInfo(request);
            }
        }
    }
}
