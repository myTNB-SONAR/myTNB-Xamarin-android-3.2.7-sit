using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Database.Model;
using static myTNB_Android.Src.FindUs.Response.GetLocationTypesResponse;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using myTNB.SQLite.SQLiteDataManager;

namespace myTNB_Android.Src.Base
{
    public class MyTNBAccountManagement
    {
        private static MyTNBAccountManagement Instance = null;
        private bool IsNeeUpdate = false;
        private bool IsMaintenanceShown = false;
        private bool IsNotificationFailed = false;
        private bool IsNotificationMaintenance = false;
        private bool IsNotificationComplete = false;
        private static MasterDataResponse currentMasterDataRes = null;
        private static AppLaunchMasterDataResponseAWS appMasterDataResponse = null;
        private List<string> UpdatedAccountNumberList = new List<string>();
        private int appLaunchMasterDataTimeout;
        private bool IsUpdatedMobileNumber = false;
        private bool IsUpdatedPassword = false;
        private bool IsUpdatedName = false;
        private bool IsUpdatedEmail = false;
        private bool IsUpdatedID = false;
        private bool IsAddedNewUser = false;
        private bool IsAppMasterComplete = false;
        private bool IsAppMasterFailed = false;
        private string MaintenanceTitle = "";
        private string MaintenanceContent = "";
        private bool IsAppMasterMaintenance = false;
        private bool IsAccessUsageFromNotification = false;
        private bool IsNotificationListFromLaunch = false;
        private bool IsUpdateAppLanguage = false;
        private bool IsUpdateAppLargeFont = false;
        private bool IsFromLogin = false;
        private bool IsEBUser = false;
        private bool IsMaybeLater = false;
        private bool IsFromApiEB = false;
        private bool IsOnHold = false;
        private int fromClick = 0;
        private bool IsFromViewTips = false;
        private bool IsFromDashboard = false;
        private bool IsCOMCLandNEM = false;
        private bool IsNCAcc = false;
        private bool IsSDUser = false;
        private bool IsTNGEnable = false;

        private MyTNBAccountManagement()
        {
            appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_TIMEOUT;
        }

        public static MyTNBAccountManagement GetInstance()
        {
            if (Instance == null)
            {
                Instance = new MyTNBAccountManagement();
            }
            return Instance;
        }

        public void UpdateCustomerBillingDetails(List<SummaryDashBoardAccountEntity> summaryDetails)
        {
            CustomerBillingAccount.UpdateBillingDetails(summaryDetails);
            IsNeeUpdate = false;
            foreach (SummaryDashBoardAccountEntity entity in summaryDetails)
            {
                if (!UpdatedAccountNumberList.Contains(entity.AccountNo))
                {
                    UpdatedAccountNumberList.Add(entity.AccountNo);
                }
            }
        }

        public bool HasUpdatedBillingDetails(string accountNumber)
        {
            //return CustomerBillingAccount.HasUpdatedBillingDetails(accountNumber);
            return UpdatedAccountNumberList.Contains(accountNumber);
        }

        public bool IsNeedUpdatedBillingDetails()
        {
            return IsNeeUpdate;
        }

        public void RemoveCustomerBillingDetails()
        {
            CustomerBillingAccount.RemoveCustomerBillingDetails();
            UpdatedAccountNumberList = new List<string>();
            IsNeeUpdate = true;
        }

        public void ClearSitecoreItem()
        {
            try
            {
                TimeStampEntity TimeStampEntityManager = new TimeStampEntity();
                TimeStampEntityManager.DeleteTable();
                TimeStampEntityManager.CreateTable();

                NewFAQParentEntity NewFAQParentManager = new NewFAQParentEntity();
                NewFAQParentManager.DeleteTable();
                NewFAQParentManager.CreateTable();

                WhatsNewParentEntity WhatsNewParentEntityVManager = new WhatsNewParentEntity();
                WhatsNewParentEntityVManager.DeleteTable();
                WhatsNewParentEntityVManager.CreateTable();

                RewardsParentEntity RewardsParentEntityManager = new RewardsParentEntity();
                RewardsParentEntityManager.DeleteTable();
                RewardsParentEntityManager.CreateTable();

                EnergySavingTipsParentEntity EnergySavingTipsParentManager = new EnergySavingTipsParentEntity();
                EnergySavingTipsParentManager.DeleteTable();
                EnergySavingTipsParentManager.CreateTable();

                AppLaunchParentEntity AppLaunchParentEntityManager = new AppLaunchParentEntity();
                AppLaunchParentEntityManager.DeleteTable();
                AppLaunchParentEntityManager.CreateTable();

                FAQsParentEntity FAQsParentEntityManager = new FAQsParentEntity();
                FAQsParentEntityManager.DeleteTable();
                FAQsParentEntityManager.CreateTable();

                //deleting epp tooltip data using SitecoreCmsEntity
                SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.EPP_TOOLTIP);
                SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.WHERE_IS_MY_ACC);
                SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.WHO_IS_REGISTERED_OWNER);
                SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.DO_I_NEED_OWNER_CONSENT);
                SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.HOW_DOES_COPY_IC);
                SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.HOW_DOES_PROOF_OF_CONSENT);
                SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIP);
                SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIPV2);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearAppCacheItem()
        {
            try
            {
                SMUsageHistoryEntity.RemoveAll();
                UsageHistoryEntity.RemoveAll();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateIsSMROnboardingShown()
        {
            CustomerBillingAccount.UpdateIsSMROnboardingShown();
        }

        public bool IsSMROnboardingShown()
        {
            return CustomerBillingAccount.GetIsSMROnboardingShown();
        }

        public bool IsPayBillEnabledNeeded()
        {
            return CustomerBillingAccount.GetIsPayBillEnabled();
        }

        public bool IsViewBillEnabledNeeded()
        {
            return CustomerBillingAccount.GetIsViewBillEnabled();
        }

        public int IsHasNonREAccountCount()
        {
            return CustomerBillingAccount.NonREAccountList().Count;
        }

        public int IsHasREAccountCount()
        {
            return CustomerBillingAccount.REAccountList().Count;
        }

        public int IsHasSMAccountCount()
        {
            return CustomerBillingAccount.SMAccountList().Count;
        }

        public void UpdateIsSMRMeterReadingOnePhaseOnboardingShown(bool flag)
        {
            if (flag)
            {
                CustomerBillingAccount.SetIsSMRMeterReadingOnePhaseOnBoardShown();
            }
            else
            {
                CustomerBillingAccount.UnSetIsSMRMeterReadingOnePhaseOnBoardShown();
            }
        }

        public bool GetSMRMeterReadingOnePhaseOnboardingShown()
        {
            return CustomerBillingAccount.GetIsSMRMeterReadingOnePhaseOnBoardShown();
        }

        public void UpdateIsSMRMeterReadingThreePhaseOnboardingShown(bool flag)
        {
            if (flag)
            {
                CustomerBillingAccount.SetIsSMRMeterReadingThreePhaseOnBoardShown();
            }
            else
            {
                CustomerBillingAccount.UnSetIsSMRMeterReadingThreePhaseOnBoardShown();
            }
        }

        public bool GetSMRMeterReadingThreePhaseOnboardingShown()
        {
            return CustomerBillingAccount.GetIsSMRMeterReadingThreePhaseOnBoardShown();
        }

        public void SetAccountActivityInfo(SMRAccountActivityInfo smrAccountActivityInfo)
        {
            List<SMRAccountActivityInfo> accountActivityInfoList = new List<SMRAccountActivityInfo>();
            if (UserSessions.GetAccountActivityInfoList().Count > 0)
            {
                accountActivityInfoList = UserSessions.GetAccountActivityInfoList();
                int itemIndex = accountActivityInfoList.FindIndex(x => x.GetAccountNumber() == smrAccountActivityInfo.GetAccountNumber());
                if (itemIndex != -1)
                {
                    accountActivityInfoList[itemIndex] = smrAccountActivityInfo;
                }
                else
                {
                    accountActivityInfoList.Add(smrAccountActivityInfo);
                }
            }
            else
            {
                accountActivityInfoList.Add(smrAccountActivityInfo);
                UserSessions.SetAccountActivityInfoList(accountActivityInfoList);
            }
        }

        public SMRAccountActivityInfo GetAccountActivityInfoByAccountNumber(string accountNumber)
        {
            SMRAccountActivityInfo accountActivityInfo = null;

            List<SMRAccountActivityInfo> accountActivityInfoList = new List<SMRAccountActivityInfo>();
            if (UserSessions.GetAccountActivityInfoList().Count > 0)
            {
                accountActivityInfoList = UserSessions.GetAccountActivityInfoList();
                int itemIndex = accountActivityInfoList.FindIndex(x => x.GetAccountNumber() == accountNumber);
                if (itemIndex != -1)
                {
                    accountActivityInfo = accountActivityInfoList[itemIndex];
                }
            }
            return accountActivityInfo;
        }

        public void SetIsSMRTakePhotoOnBoardShown()
        {
            CustomerBillingAccount.SetIsSMRTakePhotoOnBoardShown();
        }

        public bool GetIsSMRTakePhotoOnBoardShown()
        {
            return CustomerBillingAccount.GetIsSMRTakePhotoOnBoardShown();
        }

        public void SetCurentMasterData(MasterDataResponse data)
        {
            currentMasterDataRes = data;
        }

        public MasterDataResponse GetCurrentMasterData()
        {
            return currentMasterDataRes;
        }

        public void SetMasterDataResponse(AppLaunchMasterDataResponseAWS data)
        {
            appMasterDataResponse = data;
        }

        public AppLaunchMasterDataResponseAWS GetMasterDataResponse()
        {
            return appMasterDataResponse;
        }

        public bool IsAccountNumberExist(string accountNumber)
        {
            return (CustomerBillingAccount.FindByAccNum(accountNumber) != null);
        }

        public string GetNotificationAccountName(string accountNumber)
        {
           
            string notificationAccountName = string.Empty;
            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountNumber);
            if (customerBillingAccount != null)
            {
                if (!string.IsNullOrEmpty(customerBillingAccount.AccDesc))
                {
                    notificationAccountName = customerBillingAccount.AccDesc;
                }
                else
                {
                    notificationAccountName = "Customer Account Number " + customerBillingAccount.AccNum;
                }
            }
            return notificationAccountName;
        }

        public bool IsEnergyTipsDisabled()
        {
            return appMasterDataResponse.IsEnergyTipsDisabled;
        }

        public bool IsSMRFeatureDisabled()
        {
            return appMasterDataResponse.IsSMRFeatureDisabled;
        }

        public bool IsOCRDown()
        {
            return appMasterDataResponse.IsOCRDown;
        }

        public bool IsRewardsDisabled()
        {
            return appMasterDataResponse.IsRewardsDisabled;
        }

        public bool IsLargeFontDisabled()
        {
            return appMasterDataResponse.IsLargeFontDisabled;
        }

        public bool IsApplicationSyncAPIEnable
        {
            get
            {
                return appMasterDataResponse.IsApplicationSyncAPIEnable;
            }
        }

        public double ApplicationSyncAPIInterval
        {
            get
            {
                return appMasterDataResponse.ApplicationSyncAPIInterval;
            }
        }

        public bool IsAppointmentDisabled
        {
            get
            {
                return appMasterDataResponse.IsAppointmentDisabled;
            }
        }
        public bool IsDigitalBillDisabled
        {
            get
            {
                return false;
            }
        }
        
        public bool IsMaintenanceDialogShown()
        {
            return IsMaintenanceShown;
        }

        public void SetIsMaintenanceDialogShown(bool isShown)
        {
            IsMaintenanceShown = isShown;
        }

        public bool IsNotificationServiceFailed()
        {
            return IsNotificationFailed;
        }

        public void SetIsNotificationServiceFailed(bool isShown)
        {
            IsNotificationFailed = isShown;
        }

        public bool IsNotificationServiceMaintenance()
        {
            return IsNotificationMaintenance;
        }

        public void SetIsNotificationServiceMaintenance(bool isShown)
        {
            IsNotificationMaintenance = isShown;
        }

        public bool IsNotificationServiceCompleted()
        {
            return IsNotificationComplete;
        }

        public void SetIsNotificationServiceCompleted(bool isCompleted)
        {
            IsNotificationComplete = isCompleted;
        }

        public void UpdateAppMasterData()
        {
            IsAppMasterComplete = false;
            IsAppMasterFailed = false;
            IsAppMasterMaintenance = false;
            LoadAppMasterData();
        }

        public bool GetIsAppMasterComplete()
        {
            return IsAppMasterComplete;
        }

        public bool GetIsAppMasterFailed()
        {
            return IsAppMasterFailed;
        }

        public bool GetIsAppMasterMaintenance()
        {
            return IsAppMasterMaintenance;
        }

        public string GetMaintenanceTitle()
        {
            return MaintenanceTitle;
        }

        public string GetMaintenanceContent()
        {
            return MaintenanceContent;
        }

        private void LoadAppMasterData()
        {
            try
            {
                Task<AppLaunchMasterDataResponseAWS> appLaunchMasterDataTask = Task.Run(async () => await ServiceApiImpl.Instance.GetAppLaunchMasterDataAWS(new AppLaunchMasterDataRequest()));
                //appLaunchMasterDataTask.Wait();
                AppLaunchMasterDataResponseAWS masterDataResponse = appLaunchMasterDataTask.Result;
                if (masterDataResponse != null && masterDataResponse.ErrorCode != null && masterDataResponse.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                {
                    SetMasterDataResponse(masterDataResponse);

                    if (masterDataResponse.Data.WebLinks != null)
                    {
                        foreach (Weblink web in masterDataResponse.Data.WebLinks)
                        {
                            int newRecord = WeblinkEntity.InsertOrReplace(web);
                        }
                    }

                    foreach (NotificationTypes notificationType in GetMasterDataResponse().Data.NotificationTypes)
                    {
                        int newRecord = NotificationTypesEntity.InsertOrReplace(notificationType);
                    }
                    List<NotificationTypesEntity> notificationTypeList = NotificationTypesEntity.List();
                    NotificationFilterEntity.InsertOrReplace(Constants.ZERO_INDEX_FILTER, Utility.GetLocalizedCommonLabel("allNotifications"), true);
                    foreach (NotificationTypesEntity notificationType in notificationTypeList)
                    {
                        if (notificationType.ShowInFilterList)
                        {
                            NotificationFilterEntity.InsertOrReplace(notificationType.Id, notificationType.Title, false);
                        }
                    }

                    FeedbackCategoryEntity.RemoveActive();
                    if (masterDataResponse.Data.FeedbackCategorysV2 != null && masterDataResponse.Data.FeedbackCategorysV2.Count > 0)
                    {
                        foreach (FeedbackCategory cat in masterDataResponse.Data.FeedbackCategorysV2)
                        {
                            int newRecord = FeedbackCategoryEntity.InsertOrReplace(cat);
                        }
                    }
                    else
                    {
                        foreach (FeedbackCategory cat in masterDataResponse.Data.FeedbackCategorys)
                        {
                            int newRecord = FeedbackCategoryEntity.InsertOrReplace(cat);
                        }
                    }

                    int ctr = 0;
                    FeedbackStateEntity.RemoveActive();
                    foreach (FeedbackState state in masterDataResponse.Data.States)
                    {
                        bool isSelected = ctr == 0 ? true : false;
                        int newRecord = FeedbackStateEntity.InsertOrReplace(state, isSelected);
                        ctr++;
                    }


                    FeedbackTypeEntity.RemoveActive();
                    ctr = 0;
                    foreach (FeedbackType type in masterDataResponse.Data.FeedbackTypes)
                    {
                        bool isSelected = ctr == 0 ? true : false;
                        int newRecord = FeedbackTypeEntity.InsertOrReplace(type, isSelected);
                        Console.WriteLine(string.Format("FeedbackType Id = {0}", newRecord));
                        ctr++;
                    }

                    LocationTypesEntity.InsertFristRecord();
                    foreach (LocationType loc in masterDataResponse.Data.LocationTypes)
                    {
                        int newRecord = LocationTypesEntity.InsertOrReplace(loc);
                    }

                    DownTimeEntity.RemoveActive();
                    foreach (DownTime cat in masterDataResponse.Data.Downtimes)
                    {
                        int newRecord = DownTimeEntity.InsertOrReplace(cat);
                    }

                    IsAppMasterFailed = false;
                }
                else if (masterDataResponse != null && masterDataResponse.ErrorCode != null && masterDataResponse.ErrorCode == Constants.SERVICE_CODE_MAINTENANCE)
                {
                    if (masterDataResponse.DisplayMessage != null && masterDataResponse.DisplayTitle != null)
                    {
                        IsAppMasterMaintenance = true;
                        MaintenanceTitle = masterDataResponse.DisplayTitle;
                        MaintenanceContent = masterDataResponse.DisplayMessage;
                    }
                    else
                    {
                        IsAppMasterFailed = true;
                    }
                }
                else
                {
                    IsAppMasterFailed = true;
                }

                IsAppMasterComplete = true;
            }
            catch (ApiException apiException)
            {
                IsAppMasterComplete = true;
                IsAppMasterFailed = true;
                Utility.LoggingNonFatalError(apiException);
                //EvaluateServiceRetry();
            }
            catch (JsonReaderException e)
            {
                IsAppMasterComplete = true;
                IsAppMasterFailed = true;
                Utility.LoggingNonFatalError(e);
                //EvaluateServiceRetry();
            }
            catch (Exception e)
            {
                IsAppMasterComplete = true;
                IsAppMasterFailed = true;
                Utility.LoggingNonFatalError(e);
                //EvaluateServiceRetry();
            }
        }

        public void AddNewUserAdded(bool AddedNewUser)
        {
            IsAddedNewUser = AddedNewUser;
        }

        public bool IsNewUserAdd()
        {
            return IsAddedNewUser;
        }

        public void SetIsNameUpdated(bool isUpdated)
        {
            IsUpdatedName = isUpdated;
        }

        public bool IsNameUpdated()
        {
            return IsUpdatedName;
        }

        public void SetIsIDUpdated(bool isUpdated)
        {
            IsUpdatedID = isUpdated;
        }

        public bool IsIDUpdated()
        {
            return IsUpdatedID;
        }
        public void SetIsEmailUpdated(bool isUpdated)
        {
            IsUpdatedEmail = isUpdated;
        }

        public bool IsEmailUpdated()
        {
            return IsUpdatedEmail;
        }

        public void SetIsUpdatedMobile(bool isUpdated)
        {
            IsUpdatedMobileNumber = isUpdated;
        }

        public bool IsUpdatedMobile()
        {
            return IsUpdatedMobileNumber;
        }

        public void SetIsPasswordUpdated(bool isUpdated)
        {
            IsUpdatedPassword = isUpdated;
        }

        public bool IsPasswordUpdated()
        {
            return IsUpdatedPassword;
        }

        public void SetIsUpdateLanguage(bool isUpdated)
        {
            IsUpdateAppLanguage = isUpdated;
        }
        public void SetIsUpdateLargeFont(bool isUpdated)
        {
            IsUpdateAppLargeFont = isUpdated;
        }
        public bool IsUpdateLargeFont()
        {
            return IsUpdateAppLargeFont;
        }
        public bool IsUpdateLanguage()
        {
            return IsUpdateAppLanguage;
        }

        public void SetIsAccessUsageFromNotification(bool isUsageFromNotification)
        {
            IsAccessUsageFromNotification = isUsageFromNotification;
        }

        public bool IsUsageFromNotification()
        {
            return IsAccessUsageFromNotification;
        }

        public void SetIsNotificationListFromLaunch(bool isFromLaunch)
        {
            IsNotificationListFromLaunch = isFromLaunch;
        }

        public bool IsNotificationsFromLaunch()
        {
            return IsNotificationListFromLaunch;
        }

        public void SetFromLoginPage(bool isFromLaunch)
        {
            IsFromLogin = isFromLaunch;
        }

        public bool IsFromLoginPage()
        {
            return IsFromLogin;
        }

        public void SetIsEBUser(bool EBuser)
        {
            IsEBUser = EBuser;
        }

        public bool IsEBUserVerify()
        {
            return IsEBUser;
        }

        public void SetINCAccount(bool ncAcc)
        {
            IsNCAcc = ncAcc;
        }

        public bool IsNCAccount()
        {
            return IsNCAcc;
        }

        public void SetMaybeLater(bool maybeLater)
        {
            IsMaybeLater = maybeLater;
        }

        public bool IsMaybeLaterFlag()
        {
            return IsMaybeLater;
        }

        public void SetFinishApiEB(bool isfromapiEb)
        {
            IsFromApiEB = isfromapiEb;
        }

        public bool IsFromApiEBFinish()
        {
            return IsFromApiEB;
        }

        public void OnHoldWhatNew(bool isonhold)
        {
            IsOnHold = isonhold;
        }

        public bool IsOnHoldWhatNew()
        {
            return IsOnHold;
        }

        public void SetIsFromClickAdapter(int isShown)
        {
            fromClick = isShown;
        }

        public int IsFromClickAdapter()
        {
            return fromClick;
        }

        public void SetIsFromViewTips(bool isfrom)
        {
            IsFromViewTips = isfrom;
        }

        public bool IsFromViewTipsPage()
        {
            return IsFromViewTips;
        }

        public void SetIsFinishFeedback(bool isfrom)
        {
            IsFromDashboard = isfrom;
        }

        public bool IsFinishFeedback()
        {
            return IsFromDashboard;
        }

        public void SetIsCOMCLandNEM(bool isExist)
        {
            IsCOMCLandNEM = isExist;
        }

        public bool COMCLandNEM()
        {
            return IsCOMCLandNEM;
        }

        public void SetIsSDUser(bool SDuser)
        {
            IsSDUser = SDuser;
        }

        public bool IsSDUserVerify()
        {
            return IsSDUser;
        }

        public void SetIsTNGEnable(bool TNGEnable)
        {
            IsTNGEnable = TNGEnable;
        }

        public bool IsTNGEnableVerify()
        {
            return IsTNGEnable;
        }
    }
}
