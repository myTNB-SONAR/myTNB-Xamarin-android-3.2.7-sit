using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Util;
using Firebase.Iid;
using fbm = Firebase.Messaging;
using myTNB;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Android.Gms.Extensions;

namespace myTNB_Android.Src.Login.MVP
{
    public class LoginPresenter : LoginContract.IUserActionsListener
    {
        public static readonly string TAG = "LoginPresenter";
        private LoginContract.IView mView;
        private ISharedPreferences mSharedPref;

        CancellationTokenSource cts;

        public LoginPresenter(LoginContract.IView mView, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mSharedPref = mSharedPref;
            this.mView.SetPresenter(this);
        }

        public void CancelLogin()
        {
            try
            {
                if (cts != null && cts.Token.CanBeCanceled)
                {
                    cts.Cancel();
                    if (mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }
                }
                else
                {
                    // TODO : COULD NOT BE CANCELLED SHOW TO USERS
                }

                this.mView.EnableLoginButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void LoginAsync(string usrNme, string pwd, string deviceId, bool rememberMe)
        {
            if (TextUtils.IsEmpty(usrNme))
            {
                this.mView.ShowEmptyEmailError();
                return;
            }

            if (!Patterns.EmailAddress.Matcher(usrNme).Matches())
            {
                this.mView.ShowInvalidEmailError();
                return;
            }

            if (TextUtils.IsEmpty(pwd))
            {
                this.mView.ShowEmptyPasswordError();
                return;
            }

            this.mView.DisableLoginButton();

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            Log.Debug(TAG, "Awaiting...");
            try
            {
                string fcmToken = string.Empty;

                if (FirebaseTokenEntity.HasLatest())
                {
                    fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
                }
                if (string.IsNullOrEmpty(fcmToken) || string.IsNullOrWhiteSpace(fcmToken))
                {
                    var fcmData = await fbm.FirebaseMessaging.Instance.GetToken();
                    fcmToken = fcmData.ToString();
                    FirebaseTokenEntity.InsertOrReplace(fcmToken, true);

                }
                Log.Debug(TAG, "[DEBUG] FCM TOKEN: " + fcmToken);
                UserAuthenticateRequest userAuthRequest = new UserAuthenticateRequest(DeviceIdUtils.GetAppVersionName(), pwd);
                userAuthRequest.SetUserName(usrNme);
                string s = JsonConvert.SerializeObject(userAuthRequest);
                var userResponse = await ServiceApiImpl.Instance.UserAuthenticateLogin(userAuthRequest);

                if (!userResponse.IsSuccessResponse())
                {
                    if (this.mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                        this.mView.ShowInvalidEmailPasswordError(userResponse.Response.DisplayMessage);

                        UserSessions.SaveWhiteList(mSharedPref, false);
                    }
                }
                else
                {
                    try
                    {
                        WhatsNewParentEntity mWhatsNewParentEntity = new WhatsNewParentEntity();
                        mWhatsNewParentEntity.DeleteTable();
                        mWhatsNewParentEntity.CreateTable();
                        WhatsNewEntity mWhatsNewEntity = new WhatsNewEntity();
                        mWhatsNewEntity.DeleteTable();
                        mWhatsNewEntity.CreateTable();
                        WhatsNewCategoryEntity mWhatsNewCategoryEntity = new WhatsNewCategoryEntity();
                        mWhatsNewCategoryEntity.DeleteTable();
                        mWhatsNewCategoryEntity.CreateTable();
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    ///<summary>
                    ///THIS TO SAVE UPDATE THAT LOGOUT HAS BEEN DONE - WHILE UPGRADING VERSION 6 TO 7
                    ///</summary>
                    UserSessions.SaveLogoutFlag(mSharedPref, true);

                    MyTNBAccountManagement.GetInstance().SetFromLoginPage(true);

                    if (rememberMe)
                    {
                        UserSessions.SaveUserEmail(mSharedPref, usrNme);
                    }
                    else
                    {
                        UserSessions.SaveUserEmail(mSharedPref, "");
                    }

                    ///<summary>
                    ///THIS TO SAVE WHITELIST
                    ///</summary>
                    UserSessions.SaveWhiteList(mSharedPref, userResponse.GetData().IsWhiteList);

                    // TODO : REMOVE PASSWORD PERSISTANCE INSTEAD FOLLOW IOS WORKFLOW
                    // TODO : IF THERES AN EXISTING FORGET PASSWORD DO NOT SAVE USER
                    // TODO : SAVE USER ONLY WHEN FORGET PASSWORD IS SUCCESS
                    if (UserSessions.HasResetFlag(mSharedPref))
                    {
                        if (this.mView.IsActive())
                        {
                            this.mView.HideProgressDialog();
                            this.mView.ShowResetPassword(usrNme, pwd);
                        }
                    }
                    else if (!userResponse.GetData().isPhoneVerified)
                    {
                        UserAuthenticationRequest loginRequest = new UserAuthenticationRequest(Constants.APP_CONFIG.API_KEY_ID)
                        {
                            UserName = usrNme,
                            Password = pwd,
                            IpAddress = Constants.APP_CONFIG.API_KEY_ID,
                            ClientType = DeviceIdUtils.GetAppVersionName(),
                            ActiveUserName = userResponse.GetData().UserId,
                            DevicePlatform = Constants.DEVICE_PLATFORM,
                            DeviceVersion = DeviceIdUtils.GetAndroidVersion(),
                            DeviceCordova = Constants.APP_CONFIG.API_KEY_ID,
                            DeviceId = deviceId,
                            FcmToken = fcmToken
                        };

                        if (mView.IsActive())
                        {
                            this.mView.HideProgressDialog();
                        }
                        this.mView.ShowUpdatePhoneNumber(loginRequest, userResponse.GetData().MobileNo);
                    }
                    else
                    {
                        UserEntity.RemoveActive();
                        UserNotificationEntity.RemoveAll();
                        CustomerBillingAccount.RemoveActive();
                        SMUsageHistoryEntity.RemoveAll();
                        UsageHistoryEntity.RemoveAll();
                        BillHistoryEntity.RemoveAll();
                        PaymentHistoryEntity.RemoveAll();
                        REPaymentHistoryEntity.RemoveAll();
                        AccountDataEntity.RemoveAll();
                        SummaryDashBoardAccountEntity.RemoveAll();
                        SelectBillsEntity.RemoveAll();
                        MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                        HomeMenuUtils.ResetAll();
                        UserSessions.RemoveSessionData();
                        NewFAQParentEntity NewFAQParentManager = new NewFAQParentEntity();
                        NewFAQParentManager.DeleteTable();
                        NewFAQParentManager.CreateTable();
                        SSMRMeterReadingScreensParentEntity SSMRMeterReadingScreensParentManager = new SSMRMeterReadingScreensParentEntity();
                        SSMRMeterReadingScreensParentManager.DeleteTable();
                        SSMRMeterReadingScreensParentManager.CreateTable();
                        SSMRMeterReadingScreensOCROffParentEntity SSMRMeterReadingScreensOCROffParentManager = new SSMRMeterReadingScreensOCROffParentEntity();
                        SSMRMeterReadingScreensOCROffParentManager.DeleteTable();
                        SSMRMeterReadingScreensOCROffParentManager.CreateTable();
                        SSMRMeterReadingThreePhaseScreensParentEntity SSMRMeterReadingThreePhaseScreensParentManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                        SSMRMeterReadingThreePhaseScreensParentManager.DeleteTable();
                        SSMRMeterReadingThreePhaseScreensParentManager.CreateTable();
                        SSMRMeterReadingThreePhaseScreensOCROffParentEntity SSMRMeterReadingThreePhaseScreensOCROffParentManager = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();
                        SSMRMeterReadingThreePhaseScreensOCROffParentManager.DeleteTable();
                        SSMRMeterReadingThreePhaseScreensOCROffParentManager.CreateTable();
                        EnergySavingTipsParentEntity EnergySavingTipsParentManager = new EnergySavingTipsParentEntity();
                        EnergySavingTipsParentManager.DeleteTable();
                        EnergySavingTipsParentManager.CreateTable();

                        try
                        {
                            SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.EPP_TOOLTIP);
                            SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIP);
                        }
                        catch (Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }

                        try
                        {
                            RewardsParentEntity mRewardParentEntity = new RewardsParentEntity();
                            mRewardParentEntity.DeleteTable();
                            mRewardParentEntity.CreateTable();
                            RewardsCategoryEntity mRewardCategoryEntity = new RewardsCategoryEntity();
                            mRewardCategoryEntity.DeleteTable();
                            mRewardCategoryEntity.CreateTable();
                            RewardsEntity mRewardEntity = new RewardsEntity();
                            mRewardEntity.DeleteTable();
                            mRewardEntity.CreateTable();
                        }
                        catch (Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }

                        try
                        {
                            UserEntity.UpdatePhoneNumber(userResponse.GetData().MobileNo);
                            UserSessions.SavePhoneVerified(mSharedPref, true);
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }

                        int Id = UserEntity.InsertOrReplace(userResponse.GetData());
                        try
                        {
                            int loginCount = UserLoginCountEntity.GetLoginCount(userResponse.GetData().Email);
                            int recordId;
                            if (loginCount < 2)
                            {
                                recordId = UserLoginCountEntity.InsertOrReplace(userResponse.GetData(), loginCount + 1);
                            }
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                        if (Id > 0)
                        {
                            UserEntity.UpdateDeviceId(deviceId);
                            await LanguageUtil.SaveUpdatedLanguagePreference();

                            AppInfoManager.Instance.SetUserInfo("16"
                                , UserEntity.GetActive().UserID
                                , UserEntity.GetActive().UserName
                                , UserSessions.GetDeviceId()
                                , DeviceIdUtils.GetAppVersionName()
                                , myTNB.Mobile.MobileConstants.OSType.Android
                                , TextViewUtils.FontInfo
                                , LanguageUtil.GetAppLanguage() == "MS" ? LanguageManager.Language.MS : LanguageManager.Language.EN);
                            AppInfoManager.Instance.SetPlatformUserInfo(new MyTNBService.Request.BaseRequest().usrInf);
                            bool EbUser = await CustomEligibility.Instance.EvaluateEligibility((Context)this.mView);

                            GetCustomerAccountListRequest baseRequest = new GetCustomerAccountListRequest();
                            baseRequest.SetSesParam1(UserEntity.GetActive().DisplayName);
                            CustomerAccountListResponse customerAccountListResponse = await ServiceApiImpl.Instance.GetCustomerAccountList(baseRequest);
                            if (customerAccountListResponse != null && customerAccountListResponse.GetData() != null && customerAccountListResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                            {
                                if (customerAccountListResponse.GetData().Count > 0)
                                {
                                    ProcessCustomerAccount(customerAccountListResponse.GetData());
                                }
                                else
                                {
                                    AccountSortingEntity.RemoveSpecificAccountSorting(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);
                                }

                                List<NotificationTypesEntity> notificationTypes = NotificationTypesEntity.List();
                                NotificationFilterEntity.InsertOrReplace(Constants.ZERO_INDEX_FILTER, Utility.GetLocalizedCommonLabel("allNotifications"), true);
                                foreach (NotificationTypesEntity notificationType in notificationTypes)
                                {
                                    if (notificationType.ShowInFilterList)
                                    {
                                        NotificationFilterEntity.InsertOrReplace(notificationType.Id, notificationType.Title, false);
                                    }
                                }
                                UserNotificationEntity.RemoveAll();
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(false);
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(false);
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(false);
                                UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotificationsV2(new BaseRequest());
                                if (response.IsSuccessResponse())
                                {
                                    if (response.GetData() != null)
                                    {
                                        try
                                        {
                                            UserNotificationEntity.RemoveAll();
                                        }
                                        catch (System.Exception ne)
                                        {
                                            Utility.LoggingNonFatalError(ne);
                                        }

                                        foreach (UserNotification userNotification in response.GetData().UserNotificationList)
                                        {
                                            // tODO : SAVE ALL NOTIFICATIONs
                                            int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                        }

                                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(true);
                                    }
                                    else
                                    {
                                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                    }
                                }
                                else if (response != null && response.Response != null && response.Response.ErrorCode == "8400")
                                {
                                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(true);
                                }
                                else
                                {
                                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                }

                                //Console.WriteLine(string.Format("Rows updated {0}" , CustomerBillingAccount.List().Count));
                                if (this.mView.IsActive())
                                {
                                    this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                                }

                                await LanguageUtil.SaveUpdatedLanguagePreference();
                                AppInfoManager.Instance.SetUserInfo("16"
                                    , UserEntity.GetActive().UserID
                                    , UserEntity.GetActive().UserName
                                    , UserSessions.GetDeviceId()
                                    , DeviceIdUtils.GetAppVersionName()
                                    , myTNB.Mobile.MobileConstants.OSType.Android
                                    , TextViewUtils.FontInfo
                                    , LanguageUtil.GetAppLanguage() == "MS" ? LanguageManager.Language.MS : LanguageManager.Language.EN);
                                AppInfoManager.Instance.SetPlatformUserInfo(new MyTNBService.Request.BaseRequest().usrInf);


                                if (LanguageUtil.GetAppLanguage() == "MS")
                                {
                                    AppInfoManager.Instance.SetLanguage(LanguageManager.Language.MS);
                                }
                                else
                                {
                                    AppInfoManager.Instance.SetLanguage(LanguageManager.Language.EN);
                                }

                                this.mView.ShowDashboard();
                            }
                        }
                        else
                        {
                            if (this.mView.IsActive())
                            {
                                this.mView.HideProgressDialog();
                                this.mView.ShowRetryOptionsCancelledException(null);
                            }
                            ClearDataCache();
                        }
                    }
                    if (this.mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowRetryOptionsCancelledException(e);
                }
                ClearDataCache();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowRetryOptionsApiException(apiException);
                }
                ClearDataCache();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowRetryOptionsUnknownException(e);
                }
                ClearDataCache();
                Utility.LoggingNonFatalError(e);
            }

            if (this.mView.IsActive())
            {
                this.mView.EnableLoginButton();
                //this.mView.HideProgressDialog();
            }
        }

        private void ClearDataCache()
        {
            try
            {
                UserEntity.RemoveActive();
                UserRegister.RemoveActive();
                CustomerBillingAccount.RemoveActive();
                UserManageAccessAccount.RemoveActive();
                LogUserAccessEntity.RemoveAll();
                NotificationFilterEntity.RemoveAll();
                UserNotificationEntity.RemoveAll();
                SubmittedFeedbackEntity.Remove();
                SMUsageHistoryEntity.RemoveAll();
                UsageHistoryEntity.RemoveAll();
                BillHistoryEntity.RemoveAll();
                PaymentHistoryEntity.RemoveAll();
                REPaymentHistoryEntity.RemoveAll();
                AccountDataEntity.RemoveAll();
                SummaryDashBoardAccountEntity.RemoveAll();
                SelectBillsEntity.RemoveAll();
                NewFAQParentEntity NewFAQParentManager = new NewFAQParentEntity();
                NewFAQParentManager.DeleteTable();
                SSMRMeterReadingScreensParentEntity SSMRMeterReadingScreensParentManager = new SSMRMeterReadingScreensParentEntity();
                SSMRMeterReadingScreensParentManager.DeleteTable();
                SSMRMeterReadingScreensOCROffParentEntity SSMRMeterReadingScreensOCROffParentManager = new SSMRMeterReadingScreensOCROffParentEntity();
                SSMRMeterReadingScreensOCROffParentManager.DeleteTable();
                SSMRMeterReadingThreePhaseScreensParentEntity SSMRMeterReadingThreePhaseScreensParentManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                SSMRMeterReadingThreePhaseScreensParentManager.DeleteTable();
                SSMRMeterReadingThreePhaseScreensOCROffParentEntity SSMRMeterReadingThreePhaseScreensOCROffParentManager = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();
                SSMRMeterReadingThreePhaseScreensOCROffParentManager.DeleteTable();
                EnergySavingTipsParentEntity EnergySavingTipsParentManager = new EnergySavingTipsParentEntity();
                EnergySavingTipsParentManager.DeleteTable();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public void NavigateToDashboard()
        {
            this.mView.ShowDashboard();
        }

        public void NavigateToForgetPassword()
        {
            this.mView.ShowForgetPassword();
        }

        public void NavigateToRegistrationForm()
        {
            this.mView.ShowRegisterForm();
        }

        public void Start()
        {
            // NO IMPL
        }

        private void ProcessCustomerAccount(List<CustomerAccountListResponse.CustomerAccountData> list)
        {
            try
            {
                int ctr = 0;
                if (AccountSortingEntity.HasItems(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV))
                {
                    List<CustomerBillingAccount> existingSortedList = AccountSortingEntity.List(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);

                    List<CustomerBillingAccount> fetchList = new List<CustomerBillingAccount>();

                    List<CustomerBillingAccount> newExistingList = new List<CustomerBillingAccount>();
                    List<int> newExisitingListArray = new List<int>();
                    List<CustomerBillingAccount> newAccountList = new List<CustomerBillingAccount>();

                    foreach (CustomerAccountListResponse.CustomerAccountData acc in list)
                    {
                        int index = existingSortedList.FindIndex(x => x.AccNum == acc.AccountNumber);

                        var newRecord = new CustomerBillingAccount()
                        {
                            Type = acc.Type,
                            AccNum = acc.AccountNumber,
                            AccDesc = string.IsNullOrEmpty(acc.AccDesc) == true ? "--" : acc.AccDesc,
                            UserAccountId = acc.UserAccountID,
                            ICNum = acc.IcNum,
                            AmtCurrentChg = acc.AmCurrentChg,
                            IsRegistered = acc.IsRegistered,
                            IsPaid = acc.IsPaid,
                            isOwned = acc.IsOwned,
                            AccountTypeId = acc.AccountTypeId,
                            AccountStAddress = acc.AccountStAddress,
                            OwnerName = acc.OwnerName,
                            AccountCategoryId = acc.AccountCategoryId,
                            SmartMeterCode = acc.SmartMeterCode == null ? "0" : acc.SmartMeterCode,
                            IsSelected = false,
                            IsHaveAccess = acc.IsHaveAccess,
                            IsApplyEBilling = acc.IsApplyEBilling,
                            BudgetAmount = acc.BudgetAmount
                        };

                        if (index != -1)
                        {
                            newExisitingListArray.Add(index);
                        }
                        else
                        {
                            newAccountList.Add(newRecord);
                        }
                    }

                    if (newExisitingListArray.Count > 0)
                    {
                        newExisitingListArray.Sort();

                        foreach (int index in newExisitingListArray)
                        {
                            CustomerBillingAccount oldAcc = existingSortedList[index];

                            CustomerAccountListResponse.CustomerAccountData newAcc = list.Find(x => x.AccountNumber == oldAcc.AccNum);

                            var newRecord = new CustomerBillingAccount()
                            {
                                Type = newAcc.Type,
                                AccNum = newAcc.AccountNumber,
                                AccDesc = string.IsNullOrEmpty(newAcc.AccDesc) == true ? "--" : newAcc.AccDesc,
                                UserAccountId = newAcc.UserAccountID,
                                ICNum = newAcc.IcNum,
                                AmtCurrentChg = newAcc.AmCurrentChg,
                                IsRegistered = newAcc.IsRegistered,
                                IsPaid = newAcc.IsPaid,
                                isOwned = newAcc.IsOwned,
                                AccountTypeId = newAcc.AccountTypeId,
                                AccountStAddress = newAcc.AccountStAddress,
                                OwnerName = newAcc.OwnerName,
                                AccountCategoryId = newAcc.AccountCategoryId,
                                SmartMeterCode = newAcc.SmartMeterCode == null ? "0" : newAcc.SmartMeterCode,
                                IsSelected = false,
                                IsHaveAccess = newAcc.IsHaveAccess,
                                IsApplyEBilling = newAcc.IsApplyEBilling,
                                BudgetAmount = newAcc.BudgetAmount
                            };

                            newExistingList.Add(newRecord);
                        }
                    }

                    if (newExistingList.Count > 0)
                    {
                        newExistingList[0].IsSelected = true;
                        foreach (CustomerBillingAccount acc in newExistingList)
                        {
                            int rowChange = CustomerBillingAccount.InsertOrReplace(acc);
                            ctr++;
                        }

                        string accountList = JsonConvert.SerializeObject(newExistingList);

                        AccountSortingEntity.InsertOrReplace(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV, accountList);
                    }
                    else
                    {
                        AccountSortingEntity.RemoveSpecificAccountSorting(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);
                    }

                    if (newAccountList.Count > 0)
                    {
                        newAccountList.Sort((x, y) => string.Compare(x.AccDesc, y.AccDesc));
                        foreach (CustomerBillingAccount acc in newAccountList)
                        {
                            int rowChange = CustomerBillingAccount.InsertOrReplace(acc);
                            ctr++;
                        }
                    }
                }
                else
                {
                    foreach (CustomerAccountListResponse.CustomerAccountData acc in list)
                    {
                        int rowChange = CustomerBillingAccount.InsertOrReplace(acc, false);
                    }
                    CustomerBillingAccount.MakeFirstAsSelected();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}