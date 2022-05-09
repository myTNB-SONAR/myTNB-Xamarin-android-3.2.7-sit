using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using System.IO;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.AppLaunch.Async;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.Request;
using static myTNB_Android.Src.MyTNBService.Response.AppLaunchMasterDataResponseAWS;
using myTNB;
using System.Net.Http;
using DynatraceAndroid;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.NotificationDetails.Models;
using static myTNB_Android.Src.MyTNBService.Response.UserNotificationDetailsResponse;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Async;
using fbm = Firebase.Messaging;
using Android.Gms.Extensions;
using Android.Preferences;
using myTNB_Android.Src.Utils.Deeplink;
using myTNB.Mobile;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils.Notification;

using NotificationType = myTNB_Android.Src.Utils.Notification.Notification.TypeEnum;
using System.Net.Http;
using myTNB_Android.Src.Base.Response;

namespace myTNB_Android.Src.AppLaunch.MVP
{
    public class AppLaunchPresenter : AppLaunchContract.IUserActionsListener
    {
        private AppLaunchContract.IView mView;
        private ISharedPreferences mSharedPref;
        public static readonly string TAG = "LaunchViewActivity";
        CancellationTokenSource cts;

        private static int AppLaunchDefaultTimeOutMillisecond = 4000;
        private int AppLaunchTimeOutMillisecond = AppLaunchDefaultTimeOutMillisecond;
        private bool IsOnGetPhotoRunning = false;
        private string mApplySSMRSavedTimeStamp = "0000000";
        private int serviceCallCounter = 0;
        private int appLaunchMasterDataTimeout;

        public AppLaunchPresenter(AppLaunchContract.IView mView, ISharedPreferences sharedPreferences)
        {
            this.mView = mView;
            this.mSharedPref = sharedPreferences;
            appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_TIMEOUT;
            this.mView.SetPresenter(this);
        }

        public void NavigateWalkThrough()
        {
            this.mView.ShowWalkThrough();
        }

        /// <summary>
        /// Load accounts from API
        /// </summary>
        public void Start()
        {
            Log.Debug(TAG, "Start()");

            try
            {
                int resultCode = this.mView.PlayServicesResultCode();
                if (resultCode != ConnectionResult.Success)
                {
                    if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    {
                        this.mView.ShowPlayServicesErrorDialog(resultCode);
                    }
                    else
                    {
                        this.mView.ShowDeviceNotSupported();
                    }
                }
                else
                {
                    Console.WriteLine("GooglePlayServices is Installed");
                    ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

                    if (UserSessions.GetDeviceId() == null)
                    {
                        UserSessions.SaveDeviceId(this.mView.GetDeviceId());
                    }
                    LoadAppMasterData();
                    GetCountryList();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private bool IsAppNeedsUpdate(ForceUpdateInfoData forceUpdateInfoData)
        {
            if (forceUpdateInfoData != null && forceUpdateInfoData.isAndroidForceUpdateOn)
            {
                if (int.Parse(forceUpdateInfoData.AndroidLatestVersion) > DeviceIdUtils.GetAppVersionCode())
                {
                    return true;
                }
            }
            return false;
        }

        private async void LoadAppMasterData()
        {
            //For Testing Start
            string fcmToken = string.Empty;
            try
            {
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
                        UserEntity userEntity = UserEntity.GetActive();
                        myTNB_Android.Src.MyTNBService.Request.BaseRequest baseRequest = new myTNB_Android.Src.MyTNBService.Request.BaseRequest();
                        baseRequest.usrInf.ft = newfcmToken;
                        //string ts = JsonConvert.SerializeObject(baseRequest);
                        APIBaseResponse DataResponse = await ServiceApiImpl.Instance.UpdateUserInfoDevice(baseRequest);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            System.Diagnostics.Debug.WriteLine("[DEBUG] FCM TOKEN: " + fcmToken);
            Log.Debug("[DEBUG]", "FCM TOKEN: " + fcmToken);
            //Testing End
            this.mView.SetAppLaunchSuccessfulFlag(false, AppLaunchNavigation.Nothing);
            cts = new CancellationTokenSource();
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var updateAppUserDeviceApi = RestService.For<IUpdateAppUserDeviceApi>(httpClient);
#else
            var updateAppUserDeviceApi = RestService.For<IUpdateAppUserDeviceApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                UserEntity.UpdateDeviceId(this.mView.GetDeviceId());

                AppLaunchMasterDataResponseAWS masterDataResponse = await ServiceApiImpl.Instance.GetAppLaunchMasterDataAWS(new AppLaunchMasterDataRequest());
                /*AppLaunchMasterDataResponse masterDataResponse = await ServiceApiImpl.Instance.GetAppLaunchMasterData
                      (new AppLaunchMasterDataRequest(), CancellationTokenSourceWrapper.GetTokenWithDelay(appLaunchMasterDataTimeout));*/
                string dt = JsonConvert.SerializeObject(new AppLaunchMasterDataRequest());
                if (masterDataResponse != null && masterDataResponse.ErrorCode != null)
                {
                    if (masterDataResponse.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                    {
                        new MasterApiDBOperation(masterDataResponse, mSharedPref).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");

                        bool proceed = true;

                        bool appUpdateAvailable = false;
                        //AppLaunchMasterDataModel responseData = masterDataResponse.GetData();
                        bool updateDetail = masterDataResponse.Data.IsFeedbackUpdateDetailDisabled;

                        UserSessions.SaveFeedbackUpdateDetailDisabled(mSharedPref, updateDetail.ToString());  //save sharedpref cater prelogin & after login
                        AppLaunchMasterDataModel responseData = masterDataResponse.Data;

                        //UserSessions.SaveCheckEmailVerified(mSharedPref, responseData.UserVerificationInfo.Email.ToString());  //save sharedpref check email  //wan

                        if (responseData.AppVersionList != null && responseData.AppVersionList.Count > 0)
                        {
                            appUpdateAvailable = IsAppNeedsUpdate(responseData.ForceUpdateInfo);
                            if (appUpdateAvailable)
                            {
                                DeeplinkUtil.Instance.ClearDeeplinkData();
                                string modalTitle = responseData.ForceUpdateInfo.ModalTitle;
                                string modalMessage = responseData.ForceUpdateInfo.ModalBody;
                                string modalBtnLabel = responseData.ForceUpdateInfo.ModalBtnText;
                                this.mView.ShowUpdateAvailable(modalTitle, modalMessage, modalBtnLabel);
                            }
                            else
                            {
                                if (UserEntity.IsCurrentlyActive())
                                {
                                    try
                                    {
                                        UserEntity entity = UserEntity.GetActive();
                                        bool phoneVerified = UserSessions.GetPhoneVerifiedFlag(mSharedPref);
                                        if (!phoneVerified)
                                        {
                                            var phoneVerifyResponse = await ServiceApiImpl.Instance.PhoneVerifyStatus(new MyTNBService.Request.BaseRequest());

                                            if (phoneVerifyResponse != null && phoneVerifyResponse.Response != null &&
                                                phoneVerifyResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                                            {
                                                if (!phoneVerifyResponse.GetData().IsPhoneVerified)
                                                {
                                                    this.mView.ShowUpdatePhoneNumber(phoneVerifyResponse.GetData().PhoneNumber);
                                                    proceed = false;
                                                }
                                                else
                                                {
                                                    proceed = true;
                                                    try
                                                    {
                                                        if (UserEntity.IsCurrentlyActive())
                                                        {
                                                            UserEntity.UpdatePhoneNumber(phoneVerifyResponse.GetData().PhoneNumber);
                                                        }
                                                        UserSessions.SavePhoneVerified(mSharedPref, true);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Utility.LoggingNonFatalError(e);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                proceed = true;
                                            }
                                        }
                                        else
                                        {
                                            proceed = true;
                                        }
                                    }
                                    catch (System.Exception e)
                                    {
                                        Log.Debug("Package Manager", e.StackTrace);
                                        Utility.LoggingNonFatalError(e);
                                    }

                                    if (proceed)
                                    {
                                        UserEntity loggedUser = UserEntity.GetActive();

                                        MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                                        HomeMenuUtils.ResetAll();
                                        SummaryDashBoardAccountEntity.RemoveAll();
                                        if (CustomerBillingAccount.HasItems())
                                        {
                                            CustomerBillingAccount.RemoveSelected();
                                            CustomerBillingAccount.MakeFirstAsSelected();
                                            CustomerBillingAccount.SetCAListForEligibility();
                                        }
                                        BillHistoryEntity.RemoveAll();
                                        PaymentHistoryEntity.RemoveAll();

                                        // Reset Login count for AppUpdate to show DBR Popup for eligible CA
                                        try
                                        {
                                            if (!UserSessions.HasUpdateSkipped(this.mSharedPref))
                                            {
                                                MarketingPopUpEntity.RemoveAll();
                                                UserSessions.SaveDBRPopUpFlag(this.mSharedPref, false);
                                                _ = UserLoginCountEntity.UpdateLoginCountWithEmail(UserEntity.GetActive().Email, 1);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }

                                        if (!UserSessions.HasCleanUpdateReceiveCache(this.mSharedPref))
                                        {
                                            UserSessions.DoCleanUpdateReceiveCache(this.mSharedPref);
                                            try
                                            {
                                                TimeStampEntity TimeStampEntityManager = new TimeStampEntity();
                                                TimeStampEntityManager.DeleteTable();
                                                TimeStampEntityManager.CreateTable();

                                                FAQsParentEntity FAQsParentEntityManager = new FAQsParentEntity();
                                                FAQsParentEntityManager.DeleteTable();
                                                FAQsParentEntityManager.CreateTable();
                                            }
                                            catch (Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }

                                            SMRPopUpUtils.OnResetSSMRMeterReadingTimestamp();
                                        }

                                        //Checks for saved language preference
                                        LanguageUtil.SaveLanguagePrefInBackground();

                                        //If has Notification
                                        bool hasNotification = UserSessions.HasNotification(mSharedPref);
                                        //If Notification Type is equals to ODN (On-Demand Notification)
                                        bool isODNType = "ODN".Equals(UserSessions.GetNotificationType(mSharedPref));
                                        //If Notification Email is equals to logged-in email
                                        bool isLoggedInEmail = loggedUser.Email.Equals(UserSessions.GetUserEmailNotification(mSharedPref));

                                        AppInfoManager.Instance.SetUserInfo("16"
                                            , loggedUser.UserID
                                            , loggedUser.UserName
                                            , UserSessions.GetDeviceId()
                                            , DeviceIdUtils.GetAppVersionName()
                                            , myTNB.Mobile.MobileConstants.OSType.Android
                                            , TextViewUtils.FontInfo
                                            , LanguageUtil.GetAppLanguage() == "MS"
                                                ? LanguageManager.Language.MS
                                                : LanguageManager.Language.EN);
                                        AppInfoManager.Instance.SetPlatformUserInfo(new MyTNBService.Request.BaseRequest().usrInf);

                                        if (hasNotification && isLoggedInEmail && (NotificationUtil.Instance.Type == NotificationType.AppUpdate ||
                                            NotificationUtil.Instance.Type == NotificationType.AccountStatement ||
                                            NotificationUtil.Instance.Type == NotificationType.NewBillDesign ||
                                            NotificationUtil.Instance.Type == NotificationType.DigitalSignature))
                                        {
                                            GetAccountAWS();
                                        }
                                        else if (UserSessions.GetNotificationType(mSharedPref) != null
                                            && "APPLICATIONSTATUS".Equals(UserSessions.GetNotificationType(mSharedPref).ToUpper())
                                            && UserSessions.ApplicationStatusNotification != null)
                                        {
                                            this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Dashboard);
                                            this.mView.ShowApplicationStatusDetails();
                                            UserSessions.RemoveNotificationSession(mSharedPref);
                                        }
                                        else if (UserSessions.GetNotificationType(mSharedPref) != null
                                           && "DBROWNER".Equals(UserSessions.GetNotificationType(mSharedPref).ToUpper()))
                                        {
                                            DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Notifications.Combined_Comms_Owner);
                                            this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Dashboard);
                                            this.mView.OnShowManageBillDelivery();
                                            UserSessions.RemoveNotificationSession(mSharedPref);
                                        }
                                        else if (hasNotification && isLoggedInEmail && UserSessions.Notification != null)
                                        {
                                            string notificationType = UserSessions.GetNotificationType(mSharedPref) != null
                                                ? UserSessions.GetNotificationType(mSharedPref).ToUpper()
                                                : string.Empty;
                                            if (notificationType == MobileConstants.PushNotificationTypes.DBR_NonOwner)
                                            {
                                                DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Notifications.Combined_Comms_Non_Owner);
                                            }
                                            UserSessions.RemoveNotificationSession(mSharedPref);
                                            this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Dashboard);
                                            MyTNBAccountManagement.GetInstance().SetIsNotificationListFromLaunch(true);
                                            this.mView.ShowNotificationDetails();
                                        }
                                        else if (hasNotification && (isODNType || isLoggedInEmail))
                                        {
                                            GetAccountNoti();
                                        }
                                        else
                                        {
                                            if (!UserSessions.IsDeviceIdUpdated(mSharedPref)
                                                || !this.mView.GetDeviceId().Equals(UserSessions.GetDeviceId(mSharedPref)))
                                            {
                                                //If DeviceId is not the same with the saved, call UpdateAppUserDevice service.
                                                var updateAppDeviceResponse = await updateAppUserDeviceApi.UpdateAppUserDevice(new UpdateAppUserDeviceRequest()
                                                {
                                                    apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                                                    Email = UserEntity.GetActive().Email,
                                                    FCMToken = FirebaseTokenEntity.GetLatest().FBToken,
                                                    AppVersion = DeviceIdUtils.GetAppVersionName(),
                                                    OsType = Constants.DEVICE_PLATFORM,
                                                    OsVersion = DeviceIdUtils.GetAndroidVersion(),
                                                    DeviceIdOld = UserSessions.GetDeviceId(mSharedPref),
                                                    DeviceIdNew = this.mView.GetDeviceId()
                                                }, CancellationTokenSourceWrapper.GetToken());
                                            }
                                            GetAccountAWS();
                                        }
                                    }
                                }
                                else if (UserSessions.HasSkipped(mSharedPref))
                                {
                                    DeeplinkUtil.Instance.ClearDeeplinkData();
                                    if (!UserSessions.IsDeviceIdUpdated(mSharedPref) || !this.mView.GetDeviceId().Equals(UserSessions.GetDeviceId(mSharedPref)))
                                    {
                                        UserSessions.UpdateDeviceId(mSharedPref);
                                        UserSessions.SaveDeviceId(mSharedPref, this.mView.GetDeviceId());
                                    }


                                    bool link = UserSessions.HasDynamicLink(mSharedPref);
                                    if (link == true)
                                    {
                                        this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Login);
                                    }
                                    else
                                    {
                                        this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.PreLogin);
                                    }

                                    AppInfoManager.Instance.SetUserInfo("0"
                                        , string.Empty
                                        , string.Empty
                                        , UserSessions.GetDeviceId()
                                        , DeviceIdUtils.GetAppVersionName()
                                        , myTNB.Mobile.MobileConstants.OSType.Android
                                        , TextViewUtils.FontInfo
                                        , LanguageUtil.GetAppLanguage() == "MS" ? LanguageManager.Language.MS : LanguageManager.Language.EN);

                                    if (!UserSessions.HasDynamicLink(mSharedPref))
                                    {
                                        mView.ShowPreLogin();
                                    }
                                   
                                }
                                else //baru install
                                {
                                    DeeplinkUtil.Instance.ClearDeeplinkData();
                                    if (!UserSessions.IsDeviceIdUpdated(mSharedPref) || !this.mView.GetDeviceId().Equals(UserSessions.GetDeviceId(mSharedPref)))
                                    {
                                        UserSessions.UpdateDeviceId(mSharedPref);
                                        UserSessions.SaveDeviceId(mSharedPref, this.mView.GetDeviceId());
                                    }
                                    this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Walkthrough);
                                    mView.ShowWalkThrough();
                                }
                            }
                        }
                        else
                        {
                            EvaluateServiceRetry();
                        }
                    }
                    else if (masterDataResponse.ErrorCode == Constants.SERVICE_CODE_MAINTENANCE)
                    {
                        if (masterDataResponse.DisplayMessage != null && masterDataResponse.DisplayTitle != null)
                        {
                            DeeplinkUtil.Instance.ClearDeeplinkData();
                            this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Maintenance);
                            this.mView.ShowMaintenance(masterDataResponse);
                        }
                        else
                        {
                            EvaluateServiceRetry();
                        }
                    }
                    else
                    {
                        EvaluateServiceRetry();
                    }
                }
                else
                {
                    EvaluateServiceRetry();
                }
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
                EvaluateServiceRetry();
            }
            catch (JsonReaderException e)
            {
                Utility.LoggingNonFatalError(e);
                EvaluateServiceRetry();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                EvaluateServiceRetry();
            }
        }

        public async void GetAccountAWS()
        {
            try
            {
                GetAcccountsV4Request baseRequest = new GetAcccountsV4Request();
                baseRequest.SetSesParam1(UserEntity.GetActive().DisplayName);
                baseRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                string dt = JsonConvert.SerializeObject(baseRequest);
                CustomerAccountListResponseAppLaunch customerAccountListResponse = await ServiceApiImpl.Instance.GetCustomerAccountListAppLaunch(baseRequest);
                if (customerAccountListResponse != null && customerAccountListResponse.customerAccountData != null && customerAccountListResponse.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                {
                    //if (customerAccountListResponse.GetData().Count > 0)
                    if (customerAccountListResponse.customerAccountData.Count == 0 || customerAccountListResponse.customerAccountData.Count > 0)
                    {
                        CustomerBillingAccount.RemoveActive();
                        ProcessCustomerAccount(customerAccountListResponse.customerAccountData);
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
                    UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotificationsV2(new myTNB_Android.Src.MyTNBService.Request.BaseRequest());
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

                    if (LanguageUtil.GetAppLanguage() == "MS")
                    {
                        AppInfoManager.Instance.SetLanguage(LanguageManager.Language.MS);
                    }
                    else
                    {
                        AppInfoManager.Instance.SetLanguage(LanguageManager.Language.EN);
                    }

                    MyTNBAccountManagement.GetInstance().SetFromLoginPage(true);
                    this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                    this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Dashboard);

                    bool isForceCall = !UserSessions.HasUpdateSkipped(this.mSharedPref);
                    _ = await CustomEligibility.Instance.EvaluateEligibility((Context)this.mView, isForceCall);

                    UserInfo usrinf = new UserInfo();
                    usrinf.ses_param1 = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().DisplayName : "";

                    _ = Task.Run(async () => await FeatureInfoManager.Instance.SaveFeatureInfo(CustomEligibility.Instance.GetContractAccountList(),
                        FeatureInfoManager.QueueTopicEnum.getca, usrinf, new DeviceInfoRequest()));

                    GetNCAccountList();
                    this.mView.ShowDashboard();
                }
                else
                {
                    
                    MyTNBAccountManagement.GetInstance().SetFromLoginPage(true);
                    this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                    this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Dashboard);
                    this.mView.ShowDashboard();
                }

            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
                EvaluateServiceRetryAWS();
            }
            catch (JsonReaderException e)
            {
                Utility.LoggingNonFatalError(e);
                EvaluateServiceRetryAWS();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                EvaluateServiceRetryAWS();
            }
        }

        public async void GetAccountNoti()
        {
            try
            {
                GetAcccountsV4Request baseRequest = new GetAcccountsV4Request();
                baseRequest.SetSesParam1(UserEntity.GetActive().DisplayName);
                baseRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                string dt = JsonConvert.SerializeObject(baseRequest);
                CustomerAccountListResponseAppLaunch customerAccountListResponse = await ServiceApiImpl.Instance.GetCustomerAccountListAppLaunch(baseRequest);
                if (customerAccountListResponse != null && customerAccountListResponse.customerAccountData != null && customerAccountListResponse.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                {
                    //if (customerAccountListResponse.GetData().Count > 0)
                    if (customerAccountListResponse.customerAccountData.Count == 0 || customerAccountListResponse.customerAccountData.Count > 0)
                    {
                        CustomerBillingAccount.RemoveActive();
                        ProcessCustomerAccount(customerAccountListResponse.customerAccountData);


                    }
                    else
                    {
                        AccountSortingEntity.RemoveSpecificAccountSorting(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);
                    }

                    List<NotificationTypesEntity> notificationTypes = NotificationTypesEntity.List();
                    NotificationFilterEntity.InsertOrReplace(Constants.ZERO_INDEX_FILTER, Utility.GetLocalizedCommonLabel("allNotifications"), true);
                    foreach (NotificationTypesEntity notificationTypeNoti in notificationTypes)
                    {
                        if (notificationTypeNoti.ShowInFilterList)
                        {
                            NotificationFilterEntity.InsertOrReplace(notificationTypeNoti.Id, notificationTypeNoti.Title, false);
                        }
                    }
                    UserNotificationEntity.RemoveAll();
                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(false);
                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(false);
                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(false);
                    UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotificationsV2(new myTNB_Android.Src.MyTNBService.Request.BaseRequest());
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

                    if (LanguageUtil.GetAppLanguage() == "MS")
                    {
                        AppInfoManager.Instance.SetLanguage(LanguageManager.Language.MS);
                    }
                    else
                    {
                        AppInfoManager.Instance.SetLanguage(LanguageManager.Language.EN);
                    }

                    string notificationType = UserSessions.GetNotificationType(mSharedPref) != null
                        ? UserSessions.GetNotificationType(mSharedPref).ToUpper()
                        : string.Empty;
                    if (notificationType == MobileConstants.PushNotificationTypes.DBR_NonOwner)
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Notifications.Combined_Comms_Non_Owner);
                    }
                    UserSessions.RemoveNotificationSession(mSharedPref);
                    this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Notification);
                    MyTNBAccountManagement.GetInstance().SetIsNotificationListFromLaunch(true);
                    this.mView.ShowNotification();
                }
                else
                {
                    string notificationType = UserSessions.GetNotificationType(mSharedPref) != null
                        ? UserSessions.GetNotificationType(mSharedPref).ToUpper()
                        : string.Empty;
                    if (notificationType == MobileConstants.PushNotificationTypes.DBR_NonOwner)
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Notifications.Combined_Comms_Non_Owner);
                    }
                    UserSessions.RemoveNotificationSession(mSharedPref);
                    this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Notification);
                    MyTNBAccountManagement.GetInstance().SetIsNotificationListFromLaunch(true);
                    this.mView.ShowNotification();
                }

            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
                EvaluateServiceRetryNoti();
            }
            catch (JsonReaderException e)
            {
                Utility.LoggingNonFatalError(e);
                EvaluateServiceRetryNoti();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                EvaluateServiceRetryNoti();
            }
        }

        public async void GetNCAccountList()
        {
            try
            {
                string currentDate = UserSessions.GetNCDate(mSharedPref);
                List<CustomerBillingAccount> listNC = CustomerBillingAccount.NCAccountList();

                if (listNC != null)
                {
                    if (listNC.Count > 0)
                    {
                        var OldNCAccDate = currentDate;

                        if (OldNCAccDate != null)
                        {
                            //listNC[0].CreatedBy = "NC Engine"; //hardcode temp
                            int countNewNCAdded = 0;
                            for (int x = 0; x < listNC.Count; x++)
                            {
                                //if (listNC[x].CreatedBy != "NC Engine" && listNC[x].CreatedBy != UserEntity.GetActive().Email && listNC[x].CreatedBy != "system")
                                if (listNC[x].CreatedBy == null)
                                {
                                    countNewNCAdded++;
                                }
                            }

                            if (countNewNCAdded > 0)
                            {
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
                        else //first time nc overlay for app launch
                        {
                            UserSessions.SetNCDate(mSharedPref, listNC[0].CreatedDate); //save date if null
                            UserSessions.UpdateNCFlag(mSharedPref);
                            UserSessions.SaveNCFlag(mSharedPref, listNC.Count); //assign total count nc ca = 0
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


                }

            }
            catch (System.Exception exe)
            {
                Utility.LoggingNonFatalError(exe);
            }
        }


        public async void OnShowNotificationDetails(string NotificationTypeId, string BCRMNotificationTypeId, string NotificationRequestId)
        {
            try
            {
                this.mView.ShowProgress();
                UserNotificationDetailsRequestNew request = new UserNotificationDetailsRequestNew(NotificationTypeId, BCRMNotificationTypeId, NotificationRequestId);
                string dt = JsonConvert.SerializeObject(request);
                UserNotificationDetailsResponse response = await ServiceApiImpl.Instance.GetNotificationDetailsByRequestId(request);
                if (response.IsSuccessResponse())
                {
                    bool isForceCall = !UserSessions.HasUpdateSkipped(this.mSharedPref);
                    _ = await CustomEligibility.Instance.EvaluateEligibility((Context)this.mView, isForceCall);

                    UserInfo usrinf = new UserInfo();
                    usrinf.ses_param1 = UserEntity.IsCurrentlyActive() ? UserEntity.GetActive().DisplayName : "";

                    _ = Task.Run(async () => await FeatureInfoManager.Instance.SaveFeatureInfo(CustomEligibility.Instance.GetContractAccountList(),
                        FeatureInfoManager.QueueTopicEnum.getca, usrinf, new DeviceInfoRequest()));

                    Utility.SetIsPayDisableNotFromAppLaunch(!response.Response.IsPayEnabled);
                    UserNotificationEntity.UpdateIsRead(response.GetData().UserNotificationDetail.Id, true);
                    this.mView.ShowDetails(response.GetData().UserNotificationDetail);
                    UserSessions.ClearNotification();
                }
                else
                {
                    this.mView.ShowRetryOptionsCancelledException(null);
                }

                ////MOCK RESPONSE
                //this.mView.ShowDetails(GetMockDetails(userNotification.BCRMNotificationTypeId), userNotification, position);
                this.mView.HideProgressDialog();
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        /// <summary>
        /// Evaluate failed AppLaunchMasterData service for retry.
        /// </summary>
        private void EvaluateServiceRetry()
        {
            serviceCallCounter++;
            if (serviceCallCounter == 1)
            {
                DynatraceHelper.IdentifyUser();
            }
            DynatraceHelper.OnTrack(myTNB.Mobile.DynatraceConstants.App_Launch_Master_Fail);
            Log.Debug(TAG, string.Format("AppLaunchMasterData Service failed in {0} seconds: Retry: {1} "
                , appLaunchMasterDataTimeout
                , serviceCallCounter));
            if (serviceCallCounter == 1)//If first failed, do auto-retry.
            {
                appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT;
                LoadAppMasterData();
            }
            if (serviceCallCounter == 2)//If still failed, do auto-retry.
            {
                appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT;
                LoadAppMasterData();
            }
            if (serviceCallCounter == 3)//If still failed after auto-retry, inform the user.
            {
                DeeplinkUtil.Instance.ClearDeeplinkData();
                this.mView.ShowSomethingWrongException();
                serviceCallCounter = 0;
            }
        }

        /// <summary>
        /// Evaluate failed GetAccountAWS service for retry.
        /// </summary>
        private void EvaluateServiceRetryAWS()
        {
            serviceCallCounter++;
            if (serviceCallCounter == 1)
            {
                DynatraceHelper.IdentifyUser();
            }
            Log.Debug(TAG, string.Format("AWSGetAccount Service failed in {0} seconds: Retry: {1} "
                , appLaunchMasterDataTimeout
                , serviceCallCounter));
            if (serviceCallCounter == 1)//If first failed, do auto-retry.
            {
                appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT;
                GetAccountAWS();
            }
            if (serviceCallCounter == 2)//If still failed, do auto-retry.
            {
                appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT;
                GetAccountAWS();
            }
            if (serviceCallCounter == 3)//If still failed after auto-retry, inform the user.
            {
                this.mView.ShowExceptionDashboard();
                serviceCallCounter = 0;
            }
        }

        /// <summary>
        /// Evaluate failed GetAccountNoti service for retry.
        /// </summary>
        private void EvaluateServiceRetryNoti()
        {
            serviceCallCounter++;
            if (serviceCallCounter == 1)
            {
                DynatraceHelper.IdentifyUser();
            }
            Log.Debug(TAG, string.Format("AWSGetAccount Service failed in {0} seconds: Retry: {1} "
                , appLaunchMasterDataTimeout
                , serviceCallCounter));
            if (serviceCallCounter == 1)//If first failed, do auto-retry.
            {
                appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT;
                GetAccountNoti();
            }
            if (serviceCallCounter == 2)//If still failed, do auto-retry.
            {
                appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT;
                GetAccountNoti();
            }
            if (serviceCallCounter == 3)//If still failed after auto-retry, inform the user.
            {
                UserSessions.RemoveNotificationSession(mSharedPref);
                this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Notification);
                MyTNBAccountManagement.GetInstance().SetIsNotificationListFromLaunch(true);
                this.mView.ShowNotification();
                serviceCallCounter = 0;
            }
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == Constants.PLAY_SERVICES_RESOLUTION_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        Start();
                    }
                }
                else if (requestCode == Constants.UPDATE_MOBILE_NO_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        LoadAppMasterData();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void NavigateNotification()
        {
            try
            {
                if (UserEntity.IsCurrentlyActive())
                {
                    if (UserSessions.Notification != null)
                    {
                        this.mView.ShowNotificationDetails();
                    }
                    else
                    {
                        this.mView.ShowNotification();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    TimestampResponseModel responseModel = getItemsService.GetTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        TimeStampEntity wtManager = new TimeStampEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.OnTimeStampRecieved(responseModel.Data[0].Timestamp);
                    }
                    else
                    {
                        mView.OnTimeStampRecieved(null);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                    mView.OnTimeStampRecieved(null);
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public void GetSavedTimeStamp()
        {
            try
            {
                TimeStampEntity wtManager = new TimeStampEntity();
                List<TimeStampEntity> items = wtManager.GetAllItems();
                if (items != null && items.Count != 0)
                {
                    foreach (TimeStampEntity obj in items)
                    {
                        this.mView.OnSavedTimeStampRecievd(obj.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedTimeStampRecievd(null);
                }

            }
            catch (Exception e)
            {
                Log.Error("API Exception", e.StackTrace);
                this.mView.OnSavedTimeStampRecievd(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnGetAppLaunchItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    AppLaunchResponseModel responseModel = getItemsService.GetAppLaunchItem();
                    sw.Stop();
                    try
                    {
                        if (AppLaunchTimeOutMillisecond > 0)
                        {
                            AppLaunchTimeOutMillisecond = AppLaunchTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (AppLaunchTimeOutMillisecond <= 0)
                            {
                                AppLaunchTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {
                        IsOnGetPhotoRunning = false;
                        AppLaunchEntity wtManager = new AppLaunchEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        OnGetAppLaunchCache();
                    }
                    else
                    {
                        OnGetAppLaunchCache();
                    }
                }
                catch (Exception e)
                {
                    OnGetAppLaunchCache();
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (AppLaunchTimeOutMillisecond > 0)
            {
                _ = Task.Delay(AppLaunchTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (AppLaunchTimeOutMillisecond > 0)
                    {
                        AppLaunchTimeOutMillisecond = 0;
                        OnGetAppLaunchCache();
                    }
                });
            }
        }

        public Task OnGetAppLaunchCache()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            return Task.Run(() =>
            {
                try
                {
                    AppLaunchEntity wtManager = new AppLaunchEntity();
                    List<AppLaunchEntity> appLaunchList = wtManager.GetAllItems();
                    if (appLaunchList.Count > 0)
                    {
                        AppLaunchModel item = new AppLaunchModel()
                        {
                            ID = appLaunchList[0].ID,
                            Image = appLaunchList[0].Image,
                            ImageB64 = appLaunchList[0].ImageB64,
                            Title = appLaunchList[0].Title,
                            Description = appLaunchList[0].Description,
                            StartDateTime = appLaunchList[0].StartDateTime,
                            EndDateTime = appLaunchList[0].EndDateTime,
                            ShowForSeconds = appLaunchList[0].ShowForSeconds,
                            ImageBitmap = null
                        };
                        OnProcessAppLaunchItem(item);
                    }
                    else
                    {
                        AppLaunchTimeOutMillisecond = 0;
                        if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                        {
                            this.mView.SetDefaultAppLaunchImage();
                        }
                    }
                }
                catch (Exception e)
                {
                    AppLaunchTimeOutMillisecond = 0;
                    if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                    {
                        this.mView.SetDefaultAppLaunchImage();
                    }
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);
        }

        private void OnProcessAppLaunchItem(AppLaunchModel item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.ImageB64))
                {
                    Bitmap convertedImageCache = Base64ToBitmap(item.ImageB64);
                    if (convertedImageCache != null)
                    {
                        AppLaunchTimeOutMillisecond = 0;
                        item.ImageBitmap = convertedImageCache;
                        AppLaunchUtils.SetAppLaunchBitmap(item);
                        if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                        {
                            this.mView.SetCustomAppLaunchImage(item);
                        }
                    }
                    else
                    {
                        OnGetPhoto(item);
                    }
                }
                else
                {
                    OnGetPhoto(item);
                }
            }
            catch (Exception e)
            {
                AppLaunchTimeOutMillisecond = 0;
                if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                {
                    this.mView.SetDefaultAppLaunchImage();
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnGetPhoto(AppLaunchModel item)
        {
            if (!IsOnGetPhotoRunning)
            {
                IsOnGetPhotoRunning = true;
                CancellationTokenSource token = new CancellationTokenSource();
                Bitmap imageCache = null;
                Stopwatch sw = Stopwatch.StartNew();
                _ = Task.Run(() =>
                {
                    try
                    {
                        //imageCache = ImageUtils.GetImageBitmapFromUrl(item.Image);  
                        imageCache = ImageUtils.GetImageBitmapFromUrlWithTimeOut(item.Image);
                        sw.Stop();
                        AppLaunchTimeOutMillisecond = 0;

                        if (imageCache != null)
                        {
                            item.ImageBitmap = imageCache;
                            item.ImageB64 = BitmapToBase64(imageCache);
                            AppLaunchEntity wtManager = new AppLaunchEntity();
                            wtManager.DeleteTable();
                            wtManager.CreateTable();
                            AppLaunchEntity newItem = new AppLaunchEntity()
                            {
                                ID = item.ID,
                                Image = item.Image,
                                ImageB64 = item.ImageB64,
                                Title = item.Title,
                                Description = item.Description,
                                StartDateTime = item.StartDateTime,
                                EndDateTime = item.EndDateTime,
                                ShowForSeconds = item.ShowForSeconds
                            };
                            wtManager.InsertItem(newItem);
                            AppLaunchUtils.SetAppLaunchBitmap(item);
                            if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                            {
                                this.mView.SetCustomAppLaunchImage(item);
                            }
                        }
                        else
                        {
                            if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                            {
                                this.mView.SetDefaultAppLaunchImage();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        AppLaunchTimeOutMillisecond = 0;
                        if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                        {
                            this.mView.SetDefaultAppLaunchImage();
                        }
                        Utility.LoggingNonFatalError(e);
                    }
                }, token.Token);

                if (AppLaunchTimeOutMillisecond > 0)
                {
                    _ = Task.Delay(AppLaunchTimeOutMillisecond).ContinueWith(_ =>
                    {
                        if (AppLaunchTimeOutMillisecond > 0)
                        {
                            AppLaunchTimeOutMillisecond = 0;
                            if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                            {
                                this.mView.SetDefaultAppLaunchImage();
                            }
                        }
                    });
                }
            }
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        public Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        public async Task OnWaitSplashScreenDisplay(int millisecondDelay)
        {
            try
            {
                if (millisecondDelay > 0)
                {
                    await Task.Delay(millisecondDelay);
                }
                this.mView.SetAppLaunchSiteCoreDoneFlag(true);
                this.mView.OnGoAppLaunchEvent();
            }
            catch (Exception e)
            {
                this.mView.SetAppLaunchSiteCoreDoneFlag(true);
                this.mView.OnGoAppLaunchEvent();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnGetAppLaunchTimeStamp()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    AppLaunchTimeStampResponseModel responseModel = getItemsService.GetAppLaunchTimestampItem();
                    sw.Stop();
                    try
                    {
                        if (AppLaunchTimeOutMillisecond > 0)
                        {
                            AppLaunchTimeOutMillisecond = AppLaunchTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (AppLaunchTimeOutMillisecond <= 0)
                            {
                                AppLaunchTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {
                        AppLaunchParentEntity wtManager = new AppLaunchParentEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.OnAppLaunchTimeStampRecieved(responseModel.Data[0].Timestamp);
                    }
                    else
                    {
                        mView.OnAppLaunchTimeStampRecieved(null);
                    }
                }
                catch (Exception e)
                {
                    mView.OnAppLaunchTimeStampRecieved(null);
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (AppLaunchTimeOutMillisecond > 0)
            {
                _ = Task.Delay(AppLaunchTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (AppLaunchTimeOutMillisecond > 0)
                    {
                        AppLaunchTimeOutMillisecond = 0;
                        OnGetAppLaunchCache();
                    }
                });
            }
        }

        public void GetSavedAppLaunchTimeStamp()
        {
            try
            {
                AppLaunchTimeOutMillisecond = AppLaunchDefaultTimeOutMillisecond;
                AppLaunchParentEntity wtManager = new AppLaunchParentEntity();
                List<AppLaunchParentEntity> items = wtManager.GetAllItems();
                if (items != null && items.Count != 0)
                {
                    foreach (AppLaunchParentEntity obj in items)
                    {
                        this.mView.OnSavedAppLaunchTimeStampRecievd(obj.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedAppLaunchTimeStampRecievd(null);
                }

            }
            catch (Exception e)
            {
                this.mView.OnSavedAppLaunchTimeStampRecievd(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                if (requestCode == Constants.RUNTIME_PERMISSION_SMS_REQUEST_CODE)
                {
                    if (Utility.IsPermissionHasCount(grantResults))
                    {

                    }

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnRequestSMSPermission()
        {
            this.mView.RequestSMSPermission();
        }

        public void OnUpdateApp()
        {
            this.mView.OnAppUpdateClick();
        }

        public void GetCountryList()
        {
            Task.Factory.StartNew(() =>
            {
                CountryUtil.Instance.SetCountryList();
            });
        }

        
       
       
        //private void ProcessCustomerAccount(List<CustomerAccountListResponse.CustomerAccountData> list)
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
                            IsSelected = false,
                            IsHaveAccess = acc.IsHaveAccess,
                            IsApplyEBilling = acc.IsApplyEBilling,
                            BudgetAmount = acc.BudgetAmount,
                            CreatedDate = acc.CreatedDate,
                            BusinessArea = acc.BusinessArea,
                            RateCategory = acc.RateCategory,
                            IsInManageAccessList = acc.IsInManageAccessList
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
                                IsSelected = false,
                                IsHaveAccess = newAcc.IsHaveAccess,
                                IsApplyEBilling = newAcc.IsApplyEBilling,
                                BudgetAmount = newAcc.BudgetAmount,
                                CreatedDate = newAcc.CreatedDate,
                                BusinessArea = newAcc.BusinessArea,
                                RateCategory = newAcc.RateCategory,
                                IsInManageAccessList = newAcc.IsInManageAccessList
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

                UpdateUserStatusActivateRequest updateUserStatusActivateRequest = new UpdateUserStatusActivateRequest(userid,lang);
                string s = JsonConvert.SerializeObject(updateUserStatusActivateRequest);
                var updateUserStatusActivateResponse = await ServiceApiImpl.Instance.UpdateUserStatusActivate(updateUserStatusActivateRequest);


                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }


                if (updateUserStatusActivateResponse.IsSuccessResponse())
                {

                    this.mView.HideProgressDialog();
                    //this.mView.ShowUpdateUserStatusActivate();
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
                    //this.mView.ShowUpdateUserStatusDeactivate();
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
    }
}