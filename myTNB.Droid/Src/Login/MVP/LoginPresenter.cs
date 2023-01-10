using Android.Content;
using Android.Text;
using Android.Util;
using fbm = Firebase.Messaging;
using myTNB;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using Android.Gms.Extensions;
using System.Threading.Tasks;
using myTNB.Mobile;
using myTNB_Android.Src.Base.Response;

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


        public async void UpdateUserStatusActivate(string userid)
        {
            try
            {
                string lang;
                if (LanguageUtil.GetAppLanguage() == "MS")
                {
                    lang = "MS";
                }
                else
                {
                    lang = "EN";
                }

                UpdateUserStatusActivateRequest updateUserStatusActivateRequest = new UpdateUserStatusActivateRequest(userid, lang);
                string s = JsonConvert.SerializeObject(updateUserStatusActivateRequest);
                var updateUserStatusActivateResponse = await ServiceApiImpl.Instance.UpdateUserStatusActivate(updateUserStatusActivateRequest);


                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }


                if (updateUserStatusActivateResponse.IsSuccessResponse())
                {
                   
                    this.mView.HideProgressDialog();
                    this.mView.ShowUpdateUserStatusActivate(updateUserStatusActivateResponse.Response.DisplayMessage);
                    UserSessions.DoUnflagDynamicLink(mSharedPref);
                        
                    
                }
                else
                {
                    this.mView.HideProgressDialog();
                }


            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(cancelledException);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception exception)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(exception);
                Utility.LoggingNonFatalError(exception);
            }
        }

        public async void UpdateUserStatusDeactivate(string userid)
        {
            try
            {
                string lang;
                if (LanguageUtil.GetAppLanguage() == "MS")
                {
                    lang = "MS";
                }
                else
                {
                    lang = "EN";
                }

                UpdateUserStatusActivateRequest updateUserStatusActivateRequest = new UpdateUserStatusActivateRequest(userid,lang);
                string s = JsonConvert.SerializeObject(updateUserStatusActivateRequest);
                var updateUserStatusActivateResponse = await ServiceApiImpl.Instance.UpdateUserStatusDeactivate(updateUserStatusActivateRequest);


                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }


                if (updateUserStatusActivateResponse.IsSuccessResponse())
                {

                    this.mView.HideProgressDialog();
                    this.mView.ShowUpdateUserStatusDeactivate(updateUserStatusActivateResponse.Response.DisplayMessage);
                    UserSessions.DoUnflagDynamicLink(mSharedPref);


                }
                else
                {
                    this.mView.HideProgressDialog();
                }


            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(cancelledException);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception exception)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(exception);
                Utility.LoggingNonFatalError(exception);
            }
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
                var token = await fbm.FirebaseMessaging.Instance.GetToken();
                if (token != null)
                {
                    string newfcmToken = token.ToString();
                    if (FirebaseTokenEntity.HasLatest())
                    {
                        fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
                    }

                    if (fcmToken != null && (fcmToken != newfcmToken))
                    {
                        fcmToken = newfcmToken;
                        FirebaseTokenEntity.RemoveLatest();
                        FirebaseTokenEntity.InsertOrReplace(newfcmToken, true);
                        UserInfo usrinf = new UserInfo();
                        UserEntity userEntity = UserEntity.GetActive();
                        myTNB_Android.Src.MyTNBService.Request.BaseRequest baseRequest = new myTNB_Android.Src.MyTNBService.Request.BaseRequest();
                        baseRequest.usrInf.ft = newfcmToken;
                        //string ts = JsonConvert.SerializeObject(baseRequest);
                        APIBaseResponse DataResponse = await ServiceApiImpl.Instance.UpdateUserInfoDevice(baseRequest);
                    }
                    else if (string.IsNullOrEmpty(fcmToken) && !string.IsNullOrEmpty(newfcmToken))
                    {
                        FirebaseTokenEntity.RemoveLatest();
                        FirebaseTokenEntity.InsertOrReplace(newfcmToken, true);
                    }
                }
                Log.Debug(TAG, "[DEBUG] FCM TOKEN: " + fcmToken);
                UserAuthenticateRequest userAuthRequest = new UserAuthenticateRequest(DeviceIdUtils.GetAppVersionName(), pwd);
                userAuthRequest.SetUserName(usrNme);
                string s = JsonConvert.SerializeObject(userAuthRequest);
                var userResponse = await ServiceApiImpl.Instance.UserAuthenticateLogin(userAuthRequest); //OT
                //var userResponse = await ServiceApiImpl.Instance.UserAuthenticate(userAuthRequest); //CEP DBR

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (!userResponse.IsSuccessResponse())
                {
                    if (this.mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                        this.mView.ShowInvalidEmailPasswordError(userResponse.Response.DisplayTitle, userResponse.Response.DisplayMessage);

                        UserSessions.SaveWhiteList(mSharedPref, false);
                    }
                }
                else
                {
                    
                    if (userResponse.Response.Data.IsActivated)
                    {
                        if (mView.IsActive())
                        {
                            this.mView.ShowProgressDialog();
                        }

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
                            SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.EPP_TOOLTIP);
                            SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIP);
                            SitecoreCmsEntity.DeleteSitecoreRecord(SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIPV2);
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
                        UserSessions.SaveLoginflag(mSharedPref, true);

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
                                
                                GetAcccountsV4Request baseRequest = new GetAcccountsV4Request();
                                baseRequest.SetSesParam1(UserEntity.GetActive().DisplayName);
                                baseRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                                string dt = JsonConvert.SerializeObject(baseRequest);
                                CustomerAccountListResponseAppLaunch customerAccountListResponse = await ServiceApiImpl.Instance.GetCustomerAccountListAppLaunch(baseRequest);
                                //if (customerAccountListResponse != null && customerAccountListResponse.GetData() != null && customerAccountListResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                                if (customerAccountListResponse != null && customerAccountListResponse.customerAccountData != null && customerAccountListResponse.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                                {
                                    //if (customerAccountListResponse.GetData().Count > 0)
                                    if (customerAccountListResponse.customerAccountData.Count > 0)
                                    {
                                        //ProcessCustomerAccount(customerAccountListResponse.GetData());
                                        ProcessCustomerAccount(customerAccountListResponse.customerAccountData);

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
                                        UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotificationsV2(new MyTNBService.Request.BaseRequest());
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
                                            , DeviceIdUtils.GetAndroidVersion()
                                            , FirebaseTokenEntity.GetLatest().FBToken
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

                                         _ = await CustomEligibility.Instance.EvaluateEligibility((Context)this.mView, true);

                                        UserInfo usrinf = new UserInfo();
                                        usrinf.ses_param1 = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().DisplayName : "";

                                        _ = Task.Run(async () => await FeatureInfoManager.Instance.SaveFeatureInfo(CustomEligibility.Instance.GetContractAccountList(),
                                            FeatureInfoManager.QueueTopicEnum.getca, usrinf, new DeviceInfoRequest()));

                                        GetNCAccountList();
                                        this.mView.ShowDashboard();
                                    }
                                    else
                                    {
                                        AccountSortingEntity.RemoveSpecificAccountSorting(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);

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
                                        UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotificationsV2(new MyTNBService.Request.BaseRequest());
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
                                            , DeviceIdUtils.GetAndroidVersion()
                                            , FirebaseTokenEntity.GetLatest().FBToken
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

                                         _ = await CustomEligibility.Instance.EvaluateEligibility((Context)this.mView, true);

                                        UserInfo usrinf = new UserInfo();
                                        usrinf.ses_param1 = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().DisplayName : "";

                                        _ = Task.Run(async () => await FeatureInfoManager.Instance.SaveFeatureInfo(CustomEligibility.Instance.GetContractAccountList(),
                                            FeatureInfoManager.QueueTopicEnum.getca, usrinf, new DeviceInfoRequest()));

                                        this.mView.ShowAddAccount();
                                    }
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
                    else
                    {
                        this.mView.ShowEmailVerifyDialog();
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

        public async void GetNCAccountList()
        {
            try
            {
                //bool ncAccounts = UserSessions.GetNCList(mSharedPref);
                string currentDate = UserSessions.GetNCDate(mSharedPref);
                List<CustomerBillingAccount> listNC = CustomerBillingAccount.NCAccountList();

                if (listNC != null)
                {
                    if (listNC.Count > 0)
                    {
                        var OldNCAccDate = currentDate;

                        if (OldNCAccDate != null)
                        {
                            int countNewNCAdded = 0;
                            for (int x = 0; x < listNC.Count; x++)
                            {
                                //if (listNC[x].CreatedBy != "NC Engine" && listNC[x].CreatedBy != UserEntity.GetActive().Email && listNC[x].CreatedBy != "system")
                                if (listNC[x].CreatedBy == null)
                                {
                                    countNewNCAdded++;
                                }
                            }

                            if (countNewNCAdded > 0) //nc overlay
                            {
                                UserSessions.UpdateNewNCFlag(mSharedPref); 
                                UserSessions.UpdateNCFlag(mSharedPref);
                                UserSessions.SetNCDate(mSharedPref, listNC[0].CreatedDate); //save created date
                                UserSessions.SaveNCFlag(mSharedPref, countNewNCAdded); //count nc ca
                                UserSessions.UpdateNCTutorialShown(mSharedPref); //trigger home ovelay tutorial

                                try
                                {
                                    NCAutoAddAccountsRequest ncAccountRequest = new NCAutoAddAccountsRequest(UserEntity.GetActive().IdentificationNo);
                                    string s = JsonConvert.SerializeObject(ncAccountRequest);
                                    var ncAccountResponse = await ServiceApiImpl.Instance.NCAutoAddAccounts(ncAccountRequest);

                                    if (mView.IsActive())
                                    {
                                        this.mView.HideProgressDialog();
                                    }

                                    if (ncAccountResponse.IsSuccessResponse())
                                    {
                                        this.mView.HideProgressDialog();
                                    }
                                    else
                                    {
                                        this.mView.HideProgressDialog();
                                    }


                                }
                                catch (Exception e)
                                {
                                    Utility.LoggingNonFatalError(e);
                                }
                            }
                        }
                        else //nc overlay + BAU overlay
                        {
                            int countNewNCAdded = 0;
                            for (int x = 0; x < listNC.Count; x++)
                            {
                                if (listNC[x].CreatedBy == null)
                                {
                                    countNewNCAdded++;
                                }
                                //else
                                //{
                                //    countNewNCAdded = 0;
                                //}
                            }

                            UserSessions.SaveNewNCFlag(mSharedPref, true);
                            UserSessions.SetNCDate(mSharedPref, listNC[0].CreatedDate); //save date if null
                            UserSessions.UpdateNCFlag(mSharedPref);
                            UserSessions.SaveNCFlag(mSharedPref, countNewNCAdded); //assign total count nc ca = 0
                            UserSessions.UpdateNCTutorialShown(mSharedPref); //trigger home ovelay tutorial
                            //UserSessions.UpdateNewNCFlag(mSharedPref);


                            try
                            {
                                NCAutoAddAccountsRequest ncAccountRequest = new NCAutoAddAccountsRequest(UserEntity.GetActive().IdentificationNo);
                                string s = JsonConvert.SerializeObject(ncAccountRequest);
                                var ncAccountResponse = await ServiceApiImpl.Instance.NCAutoAddAccounts(ncAccountRequest);

                                if (mView.IsActive())
                                {
                                    this.mView.HideProgressDialog();
                                }

                                if (ncAccountResponse.IsSuccessResponse())
                                {
                                    this.mView.HideProgressDialog();
                                }
                                else
                                {
                                    this.mView.HideProgressDialog();
                                }


                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }

                        }

                    }
                    else //BAU overlay
                    {
                        DateTime current = Convert.ToDateTime(DateTime.Now);
                        UserSessions.SetNCDate(mSharedPref, current.ToString()); //save date if null
                    }
                }

            }
            catch (System.Exception exe)
            {
                Utility.LoggingNonFatalError(exe);
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

        private void ProcessCustomerAccount(List<CustomerAccountListResponseAppLaunch.CustomerAccountData> list)
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

                    foreach (CustomerAccountListResponseAppLaunch.CustomerAccountData acc in list)
                    {
                        int index = existingSortedList.FindIndex(x => x.AccNum == acc.AccountNumber);

                        var newRecord = new CustomerBillingAccount()
                        {
                            //Type = acc.Type,
                            AccNum = acc.AccountNumber,
                            AccDesc = string.IsNullOrEmpty(acc.AccDesc) == true ? "--" : acc.AccDesc,
                            UserAccountId = acc.UserAccountID,
                            ICNum = acc.IcNum,
                            AmtCurrentChg = acc.AmCurrentChg,
                            IsRegistered = acc.IsRegistered,
                            IsPaid = acc.IsPaid,
                            isOwned = acc.IsOwned,
                            IsError = acc.IsError,
                            AccountTypeId = acc.AccountTypeId,
                            AccountStAddress = acc.AccountStAddress,
                            OwnerName = acc.OwnerName,
                            AccountCategoryId = acc.AccountCategoryId,
                            SmartMeterCode = acc.SmartMeterCode == null ? "0" : acc.SmartMeterCode,
                            InstallationType = acc.InstallationType == null ? "0" : acc.InstallationType,
                            AMSIDCategory = acc.AMSIDCategory == null ? "0" : acc.AMSIDCategory,
                            IsSelected = false,
                            IsHaveAccess = acc.IsHaveAccess,
                            IsApplyEBilling = acc.IsApplyEBilling,
                            BudgetAmount = acc.BudgetAmount,
                            CreatedDate = acc.CreatedDate,
                            BusinessArea = acc.BusinessArea,
                            RateCategory = acc.RateCategory,
                            IsInManageAccessList = acc.IsInManageAccessList,
                            CreatedBy = acc.CreatedBy,
                            AccountHasOwner = acc.AccountHasOwner
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
                        //newExisitingListArray.Sort();

                        foreach (int index in newExisitingListArray)
                        {
                            CustomerBillingAccount oldAcc = existingSortedList[index];

                            CustomerAccountListResponseAppLaunch.CustomerAccountData newAcc = list.Find(x => x.AccountNumber == oldAcc.AccNum);

                            var newRecord = new CustomerBillingAccount()
                            {
                                //Type = newAcc.Type,
                                AccNum = newAcc.AccountNumber,
                                AccDesc = string.IsNullOrEmpty(newAcc.AccDesc) == true ? "--" : newAcc.AccDesc,
                                UserAccountId = newAcc.UserAccountID,
                                ICNum = newAcc.IcNum,
                                AmtCurrentChg = newAcc.AmCurrentChg,
                                IsRegistered = newAcc.IsRegistered,
                                IsPaid = newAcc.IsPaid,
                                isOwned = newAcc.IsOwned,
                                IsError = newAcc.IsError,
                                AccountTypeId = newAcc.AccountTypeId,
                                AccountStAddress = newAcc.AccountStAddress,
                                OwnerName = newAcc.OwnerName,
                                AccountCategoryId = newAcc.AccountCategoryId,
                                SmartMeterCode = newAcc.SmartMeterCode == null ? "0" : newAcc.SmartMeterCode,
                                InstallationType = newAcc.InstallationType == null ? "0" : newAcc.InstallationType,
                                AMSIDCategory = newAcc.AMSIDCategory == null ? "0" : newAcc.AMSIDCategory,
                                IsSelected = false,
                                IsHaveAccess = newAcc.IsHaveAccess,
                                IsApplyEBilling = newAcc.IsApplyEBilling,
                                BudgetAmount = newAcc.BudgetAmount,
                                CreatedDate = newAcc.CreatedDate,
                                BusinessArea = newAcc.BusinessArea,
                                RateCategory = newAcc.RateCategory,
                                IsInManageAccessList = newAcc.IsInManageAccessList,
                                CreatedBy = newAcc.CreatedBy,
                                AccountHasOwner = newAcc.AccountHasOwner
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
                    foreach (CustomerAccountListResponseAppLaunch.CustomerAccountData acc in list)
                    {
                        int rowChange = CustomerBillingAccount.InsertOrReplace(acc, false);
                    }
                    CustomerBillingAccount.MakeFirstAsSelected();
                }
                CustomerBillingAccount.SetCAListForEligibility();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void ResendEmailVerify(string apiKeyId, string email)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                SendEmailVerificationRequest sendEmailVerificationRequest = new SendEmailVerificationRequest(email);
                string s = JsonConvert.SerializeObject(sendEmailVerificationRequest);
                var emailVerificationResponse = await ServiceApiImpl.Instance.SendEmailVerify(new SendEmailVerificationRequest(email));

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (emailVerificationResponse.IsSuccessResponse())
                {
                    //string message = emailVerificationResponse.Response.Message;
                    this.mView.ShowEmailUpdateSuccess(email);
                }
                else
                {
                    string errorMessage = emailVerificationResponse.Response.Message;
                    this.mView.ShowError(errorMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // CANCLLED
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // API EXCEPTION
                this.mView.ShowRetryOptionsApiException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // UNKNOWN EXCEPTION
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}