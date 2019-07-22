using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Service;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuPresenter : HomeMenuContract.IHomeMenuPresenter
    {
        HomeMenuContract.IHomeMenuView mView;
        HomeMenuContract.IHomeMenuService serviceApi;
        private Constants.GREETING greeting;
        private IEnumerable<IGrouping<int, string>> batchAccountList;
        private List<SummaryDashBoardDetails> summaryDashboardInfoList;
        private static bool FirstTimeMyServiceInitiate = true;
        private static bool FirstTimeNewFAQInitiate = true;

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

        private void SortAccounts(List<SummaryDashBoardDetails> summaryDetails)
        {
            List<SummaryDashBoardDetails> reAccount = (from item in summaryDetails
                                                       where item.AccType == "2"
                                                       select item).ToList();


            List<SummaryDashBoardDetails> normalAccount = (from item in summaryDetails
                                                           where item.AccType != "2"
                                                           select item).ToList();


            List<SummaryDashBoardDetails> totalAccountList = new List<SummaryDashBoardDetails>();
            totalAccountList.AddRange(reAccount.OrderBy(x => x.AccName).ToList());
            totalAccountList.AddRange(normalAccount.OrderBy(x => x.AccName).ToList());
            summaryDashboardInfoList.AddRange(totalAccountList);
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

                        ///*** Save account data For the Day***/
                        //SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                        //accountModel.Timestamp = DateTime.Now.ToLocalTime();
                        //accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                        //accountModel.AccountNo = summaryDetails[i].AccNumber;
                        //SummaryDashBoardAccountEntity.InsertItem(accountModel);
                        ///*****/
                    }
                    SortAccounts(summaryDetails);
                    this.mView.UpdateAccountListCards(summaryDashboardInfoList);
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

        private void LoadSummaryDetails(List<string> accountList)
        {
            if (accountList.Count > 0)
            {
                SummaryDashBordRequest request = new SummaryDashBordRequest();
                request.AccNum = accountList;
                request.SspUserId = UserEntity.GetActive().UserID;
                request.ApiKeyId = Constants.APP_CONFIG.API_KEY_ID;
                _ = GetAccountSummaryInfo(request);
            }
        }

        private void BatchLoadSummaryDetails(List<CustomerBillingAccount> customerBillingAccountList)
        {
            List<string> accountList = new List<string>();
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum))
                {
                    accountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            batchAccountList = accountList.Select((x, index) => new { x, index })
                   .GroupBy(x => x.index / 5, y => y.x);

            //LoadSummaryDetails(batchAccountList.ToList()[0].ToList());
            for (int i = 0; i < batchAccountList.ToList().Count; i++)
            {
                LoadSummaryDetails(batchAccountList.ToList()[i].ToList());
            }
        }

        public void LoadAccounts()
        {
            var RenewableAccountList = CustomerBillingAccount.REAccountList();
            var NonRenewableAccountList = CustomerBillingAccount.NonREAccountList();

            List<CustomerBillingAccount> customerBillingAccountList = new List<CustomerBillingAccount>();
            customerBillingAccountList.AddRange(RenewableAccountList);
            customerBillingAccountList.AddRange(NonRenewableAccountList);

            summaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            foreach (CustomerBillingAccount customerBillintAccount in customerBillingAccountList)
            {
                SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();
                summaryDashBoardDetails.AccNumber = customerBillintAccount.AccNum;
                summaryDashboardInfoList.Add(summaryDashBoardDetails);
            }

            this.mView.SetAccountListCards(summaryDashboardInfoList);
            summaryDashboardInfoList.Clear();
            BatchLoadSummaryDetails(customerBillingAccountList);
        }

        public void LoadBatchSummaryAccounts()
        {
            if (batchAccountList.ToList().Count > 1)
            {
                for (int i = 1; i < batchAccountList.ToList().Count; i++)
                {
                    LoadSummaryDetails(batchAccountList.ToList()[i].ToList());
                }
            }
        }

        public void InitiateService()
        {
            this.mView.SetMyServiceRecycleView();
            this.mView.SetNewFAQRecycleView();
        }

        public async Task InitiateMyService()
        {
            if (MyServiceEntity.Count() == 0 || FirstTimeMyServiceInitiate)
            {
                await GetMyServiceService();
            }
            else
            {
                ReadMyServiceFromCache();
            }
        }

        public async Task RetryMyService()
        {
            await GetMyServiceService();
        }

        public async Task InitiateNewFAQ()
        {
            if (NewFAQEntity.Count() == 0 || FirstTimeNewFAQInitiate)
            {
                await GetNewFAQService();
            }
            else
            {
                ReadNewFAQFromCache();
            }
        }

        private void ReadMyServiceFromCache()
        {
            List<MyServiceEntity> cachedDBList = new List<MyServiceEntity>();
            List<MyService> cachedList = new List<MyService>();
            cachedDBList = MyServiceEntity.GetAll();
            for (int i = 0; i < cachedDBList.Count; i++)
            {
                cachedList.Add(new MyService()
                {
                    ServiceCategoryId = cachedDBList[i].ServiceCategoryId,
                    serviceCategoryName = cachedDBList[i].serviceCategoryName
                });
            }
            cachedList.Sort((a, b) => {
                int bValue = int.Parse(b.ServiceCategoryId);
                int aValue = int.Parse(a.ServiceCategoryId);
                return aValue.CompareTo(bValue);
            });
            this.mView.SetMyServiceResult(cachedList);
        }

        private void ReadNewFAQFromCache()
        {
            List<NewFAQEntity> cachedDBList = new List<NewFAQEntity>();
            List<NewFAQ> cachedList = new List<NewFAQ>();
            cachedDBList = NewFAQEntity.GetAll();
            for (int i = 0; i < cachedDBList.Count; i++)
            {
                cachedList.Add(new NewFAQ()
                {
                    ID = cachedDBList[i].ID,
                    Image = cachedDBList[i].Image,
                    BGStartColor = cachedDBList[i].BGStartColor,
                    BGEndColor = cachedDBList[i].BGEndColor,
                    BGDirection = cachedDBList[i].BGDirection,
                    Title = cachedDBList[i].Title,
                    Description = cachedDBList[i].Description,
                    TopicBodyTitle = cachedDBList[i].TopicBodyTitle,
                    TopicBodyContent = cachedDBList[i].TopicBodyContent,
                    CTA = cachedDBList[i].CTA,
                    Tags = cachedDBList[i].Tags,
                    TargetItem = cachedDBList[i].TargetItem
                });
            }
            this.mView.SetNewFAQResult(cachedList);
        }

        private async Task GetMyServiceService()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };
                GetServicesResponse getServicesResponse = await this.serviceApi.GetServices(new GetServiceRequests()
                {
                    usrInf = currentUsrInf
                });
                
                if (getServicesResponse.Data.ErrorCode == "7200" && getServicesResponse.Data.Data.Count > 0)
                {
                    MyServiceEntity.RemoveAll();
                    List<MyService> fetchList = new List<MyService>();
                    foreach(MyService service in getServicesResponse.Data.Data)
                    {
                        fetchList.Add(service);
                        MyServiceEntity.InsertOrReplace(service);
                    }
                    fetchList.Sort((a, b) => {
                        int bValue = int.Parse(b.ServiceCategoryId);
                        int aValue = int.Parse(a.ServiceCategoryId);
                        return aValue.CompareTo(bValue);
                    });
                    this.mView.SetMyServiceResult(fetchList);
                    FirstTimeMyServiceInitiate = false;
                }
                else
                {
                    ReadMyServiceFromCache();
                    this.mView.ShowMyServiceRetryOptions(getServicesResponse.Data.DisplayMessage);
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                ReadMyServiceFromCache();
                this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                ReadMyServiceFromCache();
                this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                ReadMyServiceFromCache();
                this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private async Task GetNewFAQService()
        {
            try
            {
                await Task.Delay(3000);
                NewFAQEntity.RemoveAll();
                List<NewFAQ> dummyList = new List<NewFAQ>();
                NewFAQ newItem = new NewFAQ();
                for (int i = 0; i < 6; i++)
                {
                    if (i == 0)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "0",
                            Title = "How do I reset my password?"
                        };
                    }
                    else if (i == 1)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "1",
                            Title = "Learn how to read your meter."
                        };
                    }
                    else if (i == 2)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "2",
                            Title = "Check out how you can apply for AutoPay."
                        };
                        
                    }
                    else if (i == 3)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "3",
                            Title = "How can I contact TNB?"
                        };
                    }
                    else if (i == 4)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "4",
                            Title = "How do i pay my bills through myTNB app?"
                        };
                    }
                    else if (i == 5)
                    {
                        newItem = new NewFAQ()
                        {
                            ID = "5",
                            Title = "What’s new on this app?"
                        };
                    }
                    dummyList.Add(newItem);
                    NewFAQEntity.InsertOrReplace(newItem);
                }
                this.mView.SetNewFAQResult(dummyList);

                FirstTimeNewFAQInitiate = false;
            }
            catch (TaskCanceledException timeoutEx)
            {
                System.Diagnostics.Debug.WriteLine(timeoutEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error {0}", ex.Message);
            }
        }

        public List<MyService> LoadShimmerServiceList(int count)
        {
            if(count <= 0)
            {
                count = 1;
            }

            List<MyService> list = new List<MyService>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new MyService()
                {
                    serviceCategoryName = ""
                });
            }

            return list;
        }

        public List<NewFAQ> LoadShimmerFAQList(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            List<NewFAQ> list = new List<NewFAQ>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new NewFAQ()
                {
                    Title = ""
                });
            }

            return list;
        }
    }
}
