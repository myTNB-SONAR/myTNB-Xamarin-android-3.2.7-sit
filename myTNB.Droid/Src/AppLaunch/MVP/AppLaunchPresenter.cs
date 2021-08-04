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
using static myTNB_Android.Src.MyTNBService.Response.AppLaunchMasterDataResponse;
using myTNB;
using Firebase.Iid;
using System.Net.Http;
using DynatraceAndroid;

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
            if (FirebaseTokenEntity.HasLatest())
            {
                fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
            }
            else
            {
                fcmToken = FirebaseInstanceId.Instance.Token;
                FirebaseTokenEntity.InsertOrReplace(fcmToken, true);
            }
            System.Diagnostics.Debug.WriteLine("[DEBUG] FCM TOKEN: " + fcmToken);
            Console.WriteLine("[CONSOLE] FCM TOKEN: " + fcmToken);
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
                AppLaunchMasterDataResponse masterDataResponse = await ServiceApiImpl.Instance.GetAppLaunchMasterData
                      (new AppLaunchMasterDataRequest(), CancellationTokenSourceWrapper.GetTokenWithDelay(appLaunchMasterDataTimeout));
                if (masterDataResponse != null && masterDataResponse.Response != null)
                {
                    if (masterDataResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                    {
                        new MasterApiDBOperation(masterDataResponse, mSharedPref).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");

                        bool proceed = true;

                        bool appUpdateAvailable = false;
                        AppLaunchMasterDataModel responseData = masterDataResponse.GetData();

                        UserSessions.SaveFeedbackUpdateDetailDisabled(mSharedPref, responseData.IsFeedbackUpdateDetailDisabled.ToString());  //save sharedpref cater prelogin & after login

                        if (responseData.AppVersionList != null && responseData.AppVersionList.Count > 0)
                        {
                            appUpdateAvailable = IsAppNeedsUpdate(responseData.ForceUpdateInfo);
                            if (appUpdateAvailable)
                            {
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
                                            var phoneVerifyResponse = await ServiceApiImpl.Instance.PhoneVerifyStatus(new BaseRequest());

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
                                        }
                                        BillHistoryEntity.RemoveAll();
                                        PaymentHistoryEntity.RemoveAll();

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
                                            , TextViewUtils.FontInfo
                                            , LanguageUtil.GetAppLanguage() == "MS" ? LanguageManager.Language.MS : LanguageManager.Language.EN);
                                        AppInfoManager.Instance.SetPlatformUserInfo(new BaseRequest().usrInf);

                                        if (UserSessions.GetNotificationType(mSharedPref) != null
                                            && "APPLICATIONSTATUS".Equals(UserSessions.GetNotificationType(mSharedPref).ToUpper())
                                            && UserSessions.ApplicationStatusNotification != null)
                                        {
                                            this.mView.ShowApplicationStatusDetails();
                                        }
                                        /*else if (UserSessions.GetNotificationType(mSharedPref) != null
                                           && "DBROWNER".Equals(UserSessions.GetNotificationType(mSharedPref).ToUpper()))
                                        {
                                            this.mView.OnShowManageBillDDelivery();
                                        }*/
                                        else if (hasNotification && (isODNType || isLoggedInEmail))
                                        {
                                            UserSessions.RemoveNotificationSession(mSharedPref);
                                            this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Notification);
                                            MyTNBAccountManagement.GetInstance().SetIsNotificationListFromLaunch(true);
                                            this.mView.ShowNotification();
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
                                            this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                                            this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Dashboard);
                                            this.mView.ShowDashboard();
                                        }
                                    }
                                }
                                else if (UserSessions.HasSkipped(mSharedPref))
                                {
                                    if (!UserSessions.IsDeviceIdUpdated(mSharedPref) || !this.mView.GetDeviceId().Equals(UserSessions.GetDeviceId(mSharedPref)))
                                    {
                                        UserSessions.UpdateDeviceId(mSharedPref);
                                        UserSessions.SaveDeviceId(mSharedPref, this.mView.GetDeviceId());
                                    }
                                    this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.PreLogin);
                                    AppInfoManager.Instance.SetUserInfo("0"
                                        , string.Empty
                                        , string.Empty
                                        , UserSessions.GetDeviceId()
                                        , DeviceIdUtils.GetAppVersionName()
                                        , TextViewUtils.FontInfo
                                        , LanguageUtil.GetAppLanguage() == "MS" ? LanguageManager.Language.MS : LanguageManager.Language.EN);
                                    mView.ShowPreLogin();
                                }
                                else
                                {
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
                    else if (masterDataResponse.Response.ErrorCode == Constants.SERVICE_CODE_MAINTENANCE)
                    {
                        if (masterDataResponse.Response.DisplayMessage != null && masterDataResponse.Response.DisplayTitle != null)
                        {
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

        /// <summary>
        /// Evaluate failed AppLaunchMasterData service for retry.
        /// </summary>
        private void EvaluateServiceRetry()
        {
            serviceCallCounter++;
            Log.Debug(TAG, string.Format("AppLaunchMasterData Service failed in {0} seconds: Retry: {1} ", appLaunchMasterDataTimeout, serviceCallCounter));
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
                this.mView.ShowSomethingWrongException();
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
                    this.mView.ShowNotification();
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
    }
}
