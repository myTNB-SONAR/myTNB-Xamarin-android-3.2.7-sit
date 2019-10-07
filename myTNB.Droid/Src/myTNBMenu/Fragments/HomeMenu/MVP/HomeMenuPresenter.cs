using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
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
using Android.Graphics;
using System.IO;

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
        private static List<MyService> currentMyServiceList = new List<MyService>();
        private static List<NewFAQ> currentNewFAQList = new List<NewFAQ>();
        private static NewFAQParentEntity NewFAQParentManager;
        private static NewFAQEntity NewFAQManager;
        private static SSMRMeterReadingScreensParentEntity SSMRMeterReadingScreensParentManager;
        private static SSMRMeterReadingScreensEntity SSMRMeterReadingScreensManager;
        private static SSMRMeterReadingThreePhaseScreensParentEntity SSMRMeterReadingThreePhaseScreensParentManager;
        private static SSMRMeterReadingThreePhaseScreensEntity SSMRMeterReadingThreePhaseScreensManager;
        private static EnergySavingTipsParentEntity EnergySavingTipsParentManager;
        private static EnergySavingTipsEntity EnergySavingTipsManager;
        private static List<string> loadedSummaryList;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();


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
            try
            {
                SummaryDashBoardResponse response = await this.serviceApi.GetLinkedSummaryInfo(request);
                if (response != null)
                {
                    if (response.Data != null && response.Data.ErrorCode != "7200")
                    {
                        this.mView.ShowRefreshScreen(response.Data.RefreshMessage, response.Data.RefreshBtnText);
                    }
                    else if (response.Data != null && response.Data.ErrorCode == "7200" && response.Data.data != null && response.Data.data.Count > 0)
                    {
                        
                        List<SummaryDashBoardDetails> summaryDetails = response.Data.data;
                        List<SummaryDashBoardAccountEntity> billingDetails = new List<SummaryDashBoardAccountEntity>();
                        for (int i = 0; i < summaryDetails.Count; i++)
                        {
                            AccountSMRStatus selectedUpdateAccount = new AccountSMRStatus();
                            CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
                            summaryDetails[i].AccName = cbAccount.AccDesc;
                            summaryDetails[i].AccType = cbAccount.AccountCategoryId;
                            summaryDetails[i].IsAccSelected = cbAccount.IsSelected;
                            summaryDetails[i].SmartMeterCode = cbAccount.SmartMeterCode;
                            summaryDetails[i].IsTaggedSMR = cbAccount.IsTaggedSMR;
                            /*** Save account data For the Day***/
                            SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                            accountModel.Timestamp = DateTime.Now.ToLocalTime();
                            accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                            accountModel.AccountNo = summaryDetails[i].AccNumber;
                            billingDetails.Add(accountModel);
                            SummaryDashBoardAccountEntity.InsertItem(accountModel);
                            loadedSummaryList.Add(summaryDetails[i].AccNumber);
                            /*****/
                        }
                        MyTNBAccountManagement.GetInstance().UpdateCustomerBillingDetails(billingDetails);
                        this.mView.UpdateAccountListCards(summaryDetails);

                    }
                    else
                    {
                        this.mView.ShowRefreshScreen(null, null);
                    }
                }
                else
                {
                    this.mView.ShowRefreshScreen(null, null);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private async Task GetAccountSummaryInfoInBatch(SummaryDashBordRequest request)
        {
            try
            {
                SummaryDashBoardResponse response = await this.serviceApi.GetLinkedSummaryInfo(request);
                if (response != null)
                {
                    if (response.Data != null && response.Data.ErrorCode != "7200")
                    {
                        this.mView.ShowRefreshScreen(response.Data.RefreshMessage, response.Data.RefreshBtnText);
                    }
                    else if (response.Data != null && response.Data.ErrorCode == "7200" && response.Data.data != null && response.Data.data.Count > 0)
                    {
                        List<SummaryDashBoardDetails> summaryDetails = response.Data.data;
                        List<SummaryDashBoardAccountEntity> billingDetails = new List<SummaryDashBoardAccountEntity>();
                        for (int i = 0; i < summaryDetails.Count; i++)
                        {
                            AccountSMRStatus selectedUpdateAccount = new AccountSMRStatus();
                            CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
                            summaryDetails[i].AccName = cbAccount.AccDesc;
                            summaryDetails[i].AccType = cbAccount.AccountCategoryId;
                            summaryDetails[i].IsAccSelected = cbAccount.IsSelected;
                            summaryDetails[i].SmartMeterCode = cbAccount.SmartMeterCode;
                            summaryDetails[i].IsTaggedSMR = cbAccount.IsTaggedSMR;

                            /*** Save account data For the Day***/
                            SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                            accountModel.Timestamp = DateTime.Now.ToLocalTime();
                            accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                            accountModel.AccountNo = summaryDetails[i].AccNumber;
                            SummaryDashBoardAccountEntity.InsertItem(accountModel);
                            /*****/
                            billingDetails.Add(accountModel);
                            loadedSummaryList.Add(summaryDetails[i].AccNumber);
                        }
                        MyTNBAccountManagement.GetInstance().UpdateCustomerBillingDetails(billingDetails);
                        this.mView.UpdateAccountListCards(summaryDetails);

                        List<CustomerBillingAccount> list = CustomerBillingAccount.List();
                    }
                    else
                    {
                        this.mView.ShowRefreshScreen(null, null);
                    }
                }
                else
                {
                    this.mView.ShowRefreshScreen(null, null);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private void LoadSummaryDetails(List<string> accountList)
        {
            try
            {
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

                if (accountList.Count > 0)
                {
                    SummaryDashBordRequest request = new SummaryDashBordRequest();
                    request.AccNum = accountList;
                    request.usrInf = currentUsrInf;
                    _ = GetAccountSummaryInfo(request);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void LoadSummaryDetailsInBatch(List<string> accountList)
        {
            try
            {
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

                if (accountList.Count > 0)
                {
                    SummaryDashBordRequest request = new SummaryDashBordRequest();
                    request.AccNum = accountList;
                    request.usrInf = currentUsrInf;
                    _ = GetAccountSummaryInfoInBatch(request);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void BatchLoadSummaryDetails(List<CustomerBillingAccount> customerBillingAccountList)
        {
            LoadSummaryDetails(batchAccountList.ToList()[0].ToList());
        }

        public void LoadLocalAccounts()
        {
            summaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            SummaryDashBoardDetails summaryDashBoardDetails;
            foreach (CustomerBillingAccount customerBillingAccount in customerBillingAccountList)
            {

                if (customerBillingAccount.billingDetails != null)
                {
                    summaryDashBoardDetails = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(customerBillingAccount.billingDetails);
                    summaryDashBoardDetails.IsTaggedSMR = customerBillingAccount.IsTaggedSMR;
                    summaryDashboardInfoList.Add(summaryDashBoardDetails);
                }
                else
                {
                    summaryDashBoardDetails = new SummaryDashBoardDetails();
                    summaryDashBoardDetails.AccName = customerBillingAccount.AccDesc;
                    summaryDashBoardDetails.AccNumber = customerBillingAccount.AccNum;
                    summaryDashBoardDetails.AccType = customerBillingAccount.AccountCategoryId;
                    summaryDashBoardDetails.IsTaggedSMR = customerBillingAccount.IsTaggedSMR;
                    summaryDashboardInfoList.Add(summaryDashBoardDetails);
                }
            }
            this.mView.SetAccountListCardsFromLocal(summaryDashboardInfoList);

            List<string> smrAccountList = new List<string>();
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum) && customerBillingAccountList[i].isOwned && customerBillingAccountList[i].AccountCategoryId != "2" && customerBillingAccountList[i].SmartMeterCode == "0")
                {
                    smrAccountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            if (smrAccountList.Count > 0)
            {
                _ = OnStartCheckSMRAccountStatus(smrAccountList);
            }
        }

        public void LoadAccounts()
        {
            List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            summaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            loadedSummaryList = new List<string>();
            foreach (CustomerBillingAccount customerBillintAccount in customerBillingAccountList)
            {
                SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();
                summaryDashBoardDetails.AccName = customerBillintAccount.AccDesc;
                summaryDashBoardDetails.AccNumber = customerBillintAccount.AccNum;
                summaryDashBoardDetails.AccType = customerBillintAccount.AccountCategoryId;
                summaryDashBoardDetails.SmartMeterCode = customerBillintAccount.SmartMeterCode;
                summaryDashBoardDetails.IsTaggedSMR = customerBillintAccount.IsTaggedSMR;
                summaryDashboardInfoList.Add(summaryDashBoardDetails);
            }

            List<string> accountList = new List<string>();
            List<string> smrAccountList = new List<string>();
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum))
                {
                    accountList.Add(customerBillingAccountList[i].AccNum);
                }
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum) && customerBillingAccountList[i].isOwned && customerBillingAccountList[i].AccountCategoryId != "2" && customerBillingAccountList[i].SmartMeterCode == "0")
                {
                    smrAccountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            batchAccountList = accountList.Select((x, index) => new { x, index })
                   .GroupBy(x => x.index / 5, y => y.x);
            this.mView.SetAccountListCards(summaryDashboardInfoList);
            if (batchAccountList.ToList().Count > 0)
            {
                BatchLoadSummaryDetails(customerBillingAccountList);
            }
            if (smrAccountList.Count > 0)
            {
                _ = OnStartCheckSMRAccountStatus(smrAccountList);
            }
        }

        private async Task OnStartCheckSMRAccountStatus(List<string> accountList)
        {
            await OnCheckSMRAccountStatus(accountList);
            List<SummaryDashBoardDetails> updateSummaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            if (loadedSummaryList != null && loadedSummaryList.Count > 0)
            {
                List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                foreach (CustomerBillingAccount customerBillintAccount in customerBillingAccountList)
                {
                    string findAcc = loadedSummaryList.Find(x => x == customerBillintAccount.AccNum);

                    if (!string.IsNullOrEmpty(findAcc) && customerBillintAccount.billingDetails != null)
                    {
                        SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();
                        summaryDashBoardDetails = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(customerBillintAccount.billingDetails);
                        summaryDashBoardDetails.IsTaggedSMR = customerBillintAccount.IsTaggedSMR;
                        updateSummaryDashboardInfoList.Add(summaryDashBoardDetails);
                    }
                }
                if (updateSummaryDashboardInfoList.Count > 0)
                {
                    this.mView.UpdateAccountListCards(updateSummaryDashboardInfoList);
                }
            }
        }

        private async Task OnCheckSMRAccountStatus(List<string> accountList)
        {
            List<AccountSMRStatus> updateSMRStatus = new List<AccountSMRStatus>();
            try
            {
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

                AccountSMRStatusResponse accountSMRResponse = await this.serviceApi.GetSMRAccountStatus(new AccountsSMRStatusRequest()
                {
                    ContractAccounts = accountList,
                    UserInterface = currentUsrInf
                });

                if (accountSMRResponse.Response.ErrorCode == "7200" && accountSMRResponse.Response.Data.Count > 0)
                {
                    updateSMRStatus = accountSMRResponse.Response.Data;
                    foreach (AccountSMRStatus status in updateSMRStatus)
                    {
                        CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(status.ContractAccount);
                        bool selectedUpdateIsTaggedSMR = false;
                        if (status.IsTaggedSMR == "true")
                        {
                            selectedUpdateIsTaggedSMR = true;
                        }

                        if (selectedUpdateIsTaggedSMR != cbAccount.IsTaggedSMR)
                        {
                            CustomerBillingAccount.UpdateIsSMRTagged(status.ContractAccount, selectedUpdateIsTaggedSMR);
                            cbAccount.IsTaggedSMR = selectedUpdateIsTaggedSMR;
                            for (int i = 0; i < summaryDashboardInfoList.Count; i++)
                            {
                                if (summaryDashboardInfoList[i].AccNumber == status.ContractAccount)
                                {
                                    summaryDashboardInfoList[i].IsTaggedSMR = cbAccount.IsTaggedSMR;
                                    break;
                                }
                            }
                        }
                    }
                    this.mView.UpdateCurrentSMRAccountList();
                    this.mView.UpdateEligibilitySMRAccountList();
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public void InitiateService()
        {
            tokenSource = new CancellationTokenSource();
            this.mView.SetMyServiceRecycleView();
            this.mView.SetNewFAQRecycleView();
            this.mView.SetNewPromotionRecycleView();
        }

        public void OnCancelToken()
        {
            tokenSource.Cancel();
        }

        public async Task InitiateMyService()
        {
            await GetMyServiceService();
        }

        public async Task RetryMyService()
        {
            await GetMyServiceService();
        }

        public async Task InitiateNewPromotion()
        {
            await Task.Delay(3000);
            List<NewPromotion> list = new List<NewPromotion>();
            list.Add(new NewPromotion()
            {
                Title = "TNB Energy Night Run",
                Description = "Join some excited 3,500 runners to raise awareness towards energy conservation."
            });
            list.Add(new NewPromotion()
            {
                Title = "Maevi - Celcom Bonanza 2019",
                Description = "Get 15% discount off all MAEVI devices and extra 30% discount off MAEVI Gateway."
            });
            this.mView.SetNewPromotionResult(list);
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
            this.mView.SetMyServiceResult(cachedList);
        }

        public void ReadNewFAQFromCache()
        {
            try
            {
                if (NewFAQManager == null)
                {
                    NewFAQManager = new NewFAQEntity();
                }
                currentNewFAQList.Clear();
                List<NewFAQEntity> cachedDBList = new List<NewFAQEntity>();
                cachedDBList = NewFAQManager.GetAll();
                for (int i = 0; i < cachedDBList.Count; i++)
                {
                    currentNewFAQList.Add(new NewFAQ()
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
                this.mView.SetNewFAQResult(currentNewFAQList);
            }
            catch (Exception e)
            {
                if (currentMyServiceList.Count > 0)
                {
                    this.mView.SetNewFAQResult(currentNewFAQList);
                }
                Utility.LoggingNonFatalError(e);
            }
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
                    this.mView.SetMyServiceResult(fetchList);
                    FirstTimeMyServiceInitiate = false;
                }
                else
                {
                    // ReadMyServiceFromCache();
                    // this.mView.ShowMyServiceRetryOptions(getServicesResponse.Data.DisplayMessage);
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                // ReadMyServiceFromCache();
                // this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                // ReadMyServiceFromCache();
                // this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                // ReadMyServiceFromCache();
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

        public List<NewPromotion> LoadShimmerPromotionList(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            List<NewPromotion> list = new List<NewPromotion>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new NewPromotion()
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
                if (NewFAQParentManager == null)
                {
                    NewFAQParentManager = new NewFAQParentEntity();
                }
                List<NewFAQParentEntity> items = new List<NewFAQParentEntity>();
                items = NewFAQParentManager.GetAllItems();
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
                ReadNewFAQFromCache();
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetFAQTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    HelpTimeStampResponseModel responseModel = getItemsService.GetHelpTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (NewFAQParentManager == null)
                        {
                            NewFAQParentManager = new NewFAQParentEntity();
                        }
                        NewFAQParentManager.DeleteTable();
                        NewFAQParentManager.CreateTable();
                        NewFAQParentManager.InsertListOfItems(responseModel.Data);
                        mView.ShowFAQTimestamp(true);
                    }
                    else
                    {
                        mView.ShowFAQTimestamp(false);
                    }
                }
                catch (Exception e)
                {
                    ReadNewFAQFromCache();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }

        public Task OnGetFAQs()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    HelpResponseModel responseModel = getItemsService.GetHelpItems();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (NewFAQManager == null)
                        {
                            NewFAQManager = new NewFAQEntity();
                        }
                        NewFAQManager.DeleteTable();
                        NewFAQManager.CreateTable();
                        NewFAQManager.InsertListOfItems(responseModel.Data);
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
            }, tokenSource.Token);
        }

        public void LoadBatchSummarDetailsByIndex(int batchIndex)
        {
            LoadSummaryDetailsInBatch(this.mView.GetAccountAdapter().GetAccountCardNumberListByPosition(batchIndex));
        }

        public void GetSmartMeterReadingWalkthroughtTimeStamp()
        {
            try
            {
                if (SSMRMeterReadingScreensParentManager == null)
                {
                    SSMRMeterReadingScreensParentManager = new SSMRMeterReadingScreensParentEntity();
                }
                List<SSMRMeterReadingScreensParentEntity> items = new List<SSMRMeterReadingScreensParentEntity>();
                items = SSMRMeterReadingScreensParentManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    SSMRMeterReadingScreensParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedSSMRMeterReadingTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedSSMRMeterReadingTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedSSMRMeterReadingTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetSmartMeterReadingWalkthroughtTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseWalkthroughTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingScreensParentManager == null)
                        {
                            SSMRMeterReadingScreensParentManager = new SSMRMeterReadingScreensParentEntity();
                        }
                        SSMRMeterReadingScreensParentManager.DeleteTable();
                        SSMRMeterReadingScreensParentManager.CreateTable();
                        SSMRMeterReadingScreensParentManager.InsertListOfItems(responseModel.Data);
                        this.mView.CheckSSMRMeterReadingTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }

        public Task OnGetSSMRMeterReadingScreens()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseWalkthroughItems();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingScreensManager == null)
                        {
                            SSMRMeterReadingScreensManager = new SSMRMeterReadingScreensEntity();
                        }
                        SSMRMeterReadingScreensManager.DeleteTable();
                        SSMRMeterReadingScreensManager.CreateTable();
                        SSMRMeterReadingScreensManager.InsertListOfItems(responseModel.Data);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }

        public void GetSmartMeterReadingThreePhaseWalkthroughtTimeStamp()
        {
            try
            {
                if (SSMRMeterReadingThreePhaseScreensParentManager == null)
                {
                    SSMRMeterReadingThreePhaseScreensParentManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                }
                List<SSMRMeterReadingThreePhaseScreensParentEntity> items = new List<SSMRMeterReadingThreePhaseScreensParentEntity>();
                items = SSMRMeterReadingThreePhaseScreensParentManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    SSMRMeterReadingThreePhaseScreensParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedSSMRMeterReadingThreePhaseTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedSSMRMeterReadingThreePhaseTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedSSMRMeterReadingThreePhaseTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetSmartMeterReadingThreePhaseWalkthroughtTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseWalkthroughTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingThreePhaseScreensParentManager == null)
                        {
                            SSMRMeterReadingThreePhaseScreensParentManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                        }
                        SSMRMeterReadingThreePhaseScreensParentManager.DeleteTable();
                        SSMRMeterReadingThreePhaseScreensParentManager.CreateTable();
                        SSMRMeterReadingThreePhaseScreensParentManager.InsertListOfItems(responseModel.Data);
                        this.mView.CheckSSMRMeterReadingThreePhaseTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }

        public Task OnGetSSMRMeterReadingThreePhaseScreens()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseWalkthroughItems();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingThreePhaseScreensManager == null)
                        {
                            SSMRMeterReadingThreePhaseScreensManager = new SSMRMeterReadingThreePhaseScreensEntity();
                        }
                        SSMRMeterReadingThreePhaseScreensManager.DeleteTable();
                        SSMRMeterReadingThreePhaseScreensManager.CreateTable();

                        SSMRMeterReadingThreePhaseScreensManager.InsertListOfItems(responseModel.Data);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }

        public void GetEnergySavingTipsTimeStamp()
        {
            try
            {
                if (EnergySavingTipsParentManager == null)
                {
                    EnergySavingTipsParentManager = new EnergySavingTipsParentEntity();
                }
                List<EnergySavingTipsParentEntity> items = new List<EnergySavingTipsParentEntity>();
                items = EnergySavingTipsParentManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    EnergySavingTipsParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedEnergySavingTipsTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedEnergySavingTipsTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedEnergySavingTipsTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetEnergySavingTipsTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    EnergySavingTipsTimeStampResponseModel responseModel = getItemsService.GetEnergySavingTipsTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (EnergySavingTipsParentManager == null)
                        {
                            EnergySavingTipsParentManager = new EnergySavingTipsParentEntity();
                        }
                        EnergySavingTipsParentManager.DeleteTable();
                        EnergySavingTipsParentManager.CreateTable();
                        EnergySavingTipsParentManager.InsertListOfItems(responseModel.Data);
                        this.mView.CheckEnergySavingTipsTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }

        public Task OnGetEnergySavingTips()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    EnergySavingTipsResponseModel responseModel = getItemsService.GetEnergySavingTipsItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (EnergySavingTipsManager == null)
                        {
                            EnergySavingTipsManager = new EnergySavingTipsEntity();
                        }
                        EnergySavingTipsManager.DeleteTable();
                        EnergySavingTipsManager.CreateTable();
                        EnergySavingTipsManager.InsertListOfItems(responseModel.Data);
                        OnSetEnergySavingTipsToCache();
                    }
                    else
                    {
                        OnSetEnergySavingTipsToCache();
                    }
                }
                catch (Exception e)
                {
                    OnSetEnergySavingTipsToCache();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }

        public Task OnSetEnergySavingTipsToCache()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    if (EnergySavingTipsManager == null)
                    {
                        EnergySavingTipsManager = new EnergySavingTipsEntity();
                    }
                    List<EnergySavingTipsEntity> energyList = EnergySavingTipsManager.GetAllItems();
                    if (energyList.Count > 0)
                    {
                        List<EnergySavingTipsModel> savedList = new List<EnergySavingTipsModel>();
                        foreach (EnergySavingTipsEntity item in energyList)
                        {

                            savedList.Add(new EnergySavingTipsModel()
                            {
                                Title = item.Title,
                                Description = item.Description,
                                Image = item.Image,
                                isUpdateNeeded = true,
                                ImageBitmap = null,
                                ID = item.ID
                            }); 
                        }
                        EnergyTipsUtils.OnSetEnergyTipsList(savedList);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }
    }
}
