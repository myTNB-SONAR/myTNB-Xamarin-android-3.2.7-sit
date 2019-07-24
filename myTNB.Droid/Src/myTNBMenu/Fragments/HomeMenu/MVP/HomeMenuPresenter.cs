using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Service;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SitecoreCMS.Model;
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
        private CancellationTokenSource cts;
        private static List<MyService> currentMyServiceList = new List<MyService>();

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
                        summaryDetails[i].SmartMeterCode = cbAccount.SmartMeterCode;
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
            await GetMyServiceService();
        }

        public async Task RetryMyService()
        {
            await GetMyServiceService();
        }

        private void ReadMyServiceFromCache()
        {
            // List<MyServiceEntity> cachedDBList = new List<MyServiceEntity>();
            List<MyService> cachedList = new List<MyService>();
            // cachedDBList = MyServiceEntity.GetAll();
            for (int i = 0; i < currentMyServiceList.Count; i++)
            {
                cachedList.Add(new MyService()
                {
                    ServiceCategoryId = currentMyServiceList[i].ServiceCategoryId,
                    serviceCategoryName = currentMyServiceList[i].serviceCategoryName
                });
            }
            cachedList.Sort((a, b) =>
            {
                int bValue = int.Parse(b.ServiceCategoryId);
                int aValue = int.Parse(a.ServiceCategoryId);
                return aValue.CompareTo(bValue);
            });
            this.mView.SetMyServiceResult(cachedList);
        }

        public void ReadNewFAQFromCache()
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

                if (getServicesResponse.Data.ErrorCode == "7200" && getServicesResponse.Data.Data.CurrentServices.Count > 0)
                {
                    // MyServiceEntity.RemoveAll();
                    currentMyServiceList.Clear();
                    List<MyService> fetchList = new List<MyService>();
                    foreach (MyService service in getServicesResponse.Data.Data.CurrentServices)
                    {
                        fetchList.Add(service);
                        currentMyServiceList.Add(service);
                        // MyServiceEntity.InsertOrReplace(service);
                    }
                    fetchList.Sort((a, b) =>
                    {
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
                    // this.mView.ShowMyServiceRetryOptions(getServicesResponse.Data.DisplayMessage);
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                ReadMyServiceFromCache();
                // this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                ReadMyServiceFromCache();
                // this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                ReadMyServiceFromCache();
                // this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public List<MyService> LoadShimmerServiceList(int count)
        {
            if (count <= 0)
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

        public void GetSavedNewFAQTimeStamp()
        {
            try
            {
                NewFAQParentEntity wtManager = new NewFAQParentEntity();
                List<NewFAQParentEntity> items = new List<NewFAQParentEntity>();
                items = wtManager.GetAllItems();
                if (items != null && items.Count() > 0)
                {
                    NewFAQParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        mView.OnSavedTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    mView.OnSavedTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                mView.OnSavedTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetFAQTimeStamp()
        {
            cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    HelpTimeStampResponseModel responseModel = getItemsService.GetHelpTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        NewFAQParentEntity wtManager = new NewFAQParentEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowFAQTimestamp(true);
                    }
                    else
                    {
                        mView.ShowFAQTimestamp(false);
                    }
                }
                catch (Exception e)
                {
                    mView.ShowFAQTimestamp(false);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, cts.Token);
        }

        public Task OnGetFAQs()
        {
            cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    HelpResponseModel responseModel = getItemsService.GetHelpItems();
                    if (responseModel.Status.Equals("Success"))
                    {
                        NewFAQEntity wtManager = new NewFAQEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        ReadNewFAQFromCache();
                    }
                    else
                    {
                        ReadNewFAQFromCache();
                    }
                }
                catch (Exception e)
                {
                    ReadNewFAQFromCache();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, cts.Token);
        }
    }
}
