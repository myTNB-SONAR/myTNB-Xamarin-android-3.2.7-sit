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
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.AppLaunch.Api;
using static myTNB_Android.Src.AppLaunch.Models.MasterDataRequest;
using myTNB;

namespace myTNB_Android.Src.AppLaunch.MVP
{

    public class AppLaunchPresenter : AppLaunchContract.IUserActionsListener
    {
        private AppLaunchContract.IView mView;
        private ISharedPreferences mSharedPref;
        public static readonly string TAG = "LaunchViewActivity";
        CancellationTokenSource cts;

        private string savedPromoTimeStamp = "0000000";

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
            this.appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_TIMEOUT;
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
                    LoadAccounts();
                    GetSSMRWalkThrough();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private bool IsAppNeedsUpdate(MasterData masterData)
        {
            MasterData.ForceUpdateInfoData forceUpdatedData = (MasterData.ForceUpdateInfoData)masterData.ForceUpdateInfo;
            if (forceUpdatedData != null && forceUpdatedData.isAndroidForceUpdateOn)
            {
                Log.Debug("TEST", "= " + DeviceIdUtils.GetAppVersionCode());
                if (int.Parse(forceUpdatedData.AndroidLatestVersion) > DeviceIdUtils.GetAppVersionCode())
                {
                    return true;
                }
            }
            return false;
        }

        private async void LoadAccounts()
        {
            this.mView.SetAppLaunchSuccessfulFlag(false, AppLaunchNavigation.Nothing);
            cts = new CancellationTokenSource();
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var feedbackApi = RestService.For<IFeedbackApi>(httpClient);

            var masterDataApi = RestService.For<GetMasterDataApi>(httpClient);

            var getPhoneVerifyApi = RestService.For<GetPhoneVerifyStatusApi>(httpClient);

#else
            var api = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
            var feedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);

            var masterDataApi = RestService.For<GetMasterDataApi>(Constants.SERVER_URL.END_POINT);

            var getPhoneVerifyApi = RestService.For<GetPhoneVerifyStatusApi>(Constants.SERVER_URL.END_POINT);

#endif
            try
            {
                Context mContext = MyTNBApplication.Context;

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = "",
                    sspuid = "",
                    did = this.mView.GetDeviceId(),
                    ft = "",
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                if (UserEntity.IsCurrentlyActive())
                {
                    currentUsrInf.eid = UserEntity.GetActive().Email;
                    currentUsrInf.sspuid = UserEntity.GetActive().UserID;
                }

                DeviceInterface currentDeviceInf = new DeviceInterface()
                {
                    DeviceId = this.mView.GetDeviceId(),
                    AppVersion = DeviceIdUtils.GetAppVersionName(),
                    OsType = Constants.DEVICE_PLATFORM,
                    OsVersion = DeviceIdUtils.GetAndroidVersion(),
                    DeviceDesc = Constants.DEFAULT_LANG

                };

                var masterDataResponse = await masterDataApi.GetAppLaunchMasterData(new MasterDataRequest()
                {
                    deviceInf = currentDeviceInf,
                    usrInf = currentUsrInf
                }, CancellationTokenSourceWrapper.GetTokenWithDelay(this.appLaunchMasterDataTimeout));
                string language = LanguageUtil.GetAppLanguage();
                LanguageUtil.SaveAppLanguage("MS");
                string language2 = LanguageUtil.GetAppLanguage();
                if (masterDataResponse != null && masterDataResponse.Data != null)
                {
                    if (masterDataResponse.Data.ErrorCode == "7200" && masterDataResponse.Data.ErrorCode != "7000")
                    {
                        new MasterApiDBOperation(masterDataResponse, mSharedPref).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");

                        bool proceed = true;

                        bool appUpdateAvailable = false;
                        if (masterDataResponse.Data.MasterData.AppVersionList != null && masterDataResponse.Data.MasterData.AppVersionList.Count > 0)
                        {
                            appUpdateAvailable = IsAppNeedsUpdate(masterDataResponse.Data.MasterData);
                            if (appUpdateAvailable)
                            {
                                string modalTitle = masterDataResponse.Data.MasterData.ForceUpdateInfo.ModalTitle;
                                string modalMessage = masterDataResponse.Data.MasterData.ForceUpdateInfo.ModalBody;
                                string modalBtnLabel = masterDataResponse.Data.MasterData.ForceUpdateInfo.ModalBtnText;
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
                                            PhoneVerifyStatusResponse phoneVerifyResponse = await getPhoneVerifyApi.GetPhoneVerifyStatus(new GetPhoneVerifyStatusRequest()
                                            {
                                                ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                                                Email = entity.Email,
                                                SSPUserID = entity.UserID,
                                                DeviceID = this.mView.GetDeviceId()
                                            }, cts.Token);

                                            if (!phoneVerifyResponse.verificationData.IsError)
                                            {
                                                if (!phoneVerifyResponse.verificationData.Data.IsPhoneVerified)
                                                {
                                                    this.mView.ShowUpdatePhoneNumber(phoneVerifyResponse.verificationData.Data.PhoneNumber);
                                                    proceed = false;
                                                }
                                                else
                                                {
                                                    proceed = true;
                                                    try
                                                    {
                                                        if (UserEntity.IsCurrentlyActive())
                                                        {
                                                            UserEntity.UpdatePhoneNumber(phoneVerifyResponse.verificationData.Data.PhoneNumber);
                                                        }
                                                        UserSessions.SavePhoneVerified(mSharedPref, true);
                                                    }
                                                    catch (System.Exception e)
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
                                        SummaryDashBoardAccountEntity.RemoveAll();
                                        CustomerBillingAccount.RemoveSelected();
                                        CustomerBillingAccount.MakeFirstAsSelected();
                                        BillHistoryEntity.RemoveAll();
                                        PaymentHistoryEntity.RemoveAll();

                                        //If has Notification
                                        bool hasNotification = UserSessions.HasNotification(mSharedPref);
                                        //If Notification Type is equals to ODN (On-Demand Notification)
                                        bool isODNType = "ODN".Equals(UserSessions.GetNotificationType(mSharedPref));
                                        //If Notification Email is equals to logged-in email
                                        bool isLoggedInEmail = loggedUser.Email.Equals(UserSessions.GetUserEmailNotification(mSharedPref));
                                        if (hasNotification && (isODNType || isLoggedInEmail))
                                        {
                                            UserSessions.RemoveNotificationSession(mSharedPref);
                                            this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Notification);
                                            this.mView.ShowNotification();
                                        }
                                        else
                                        {
                                            if (!UserSessions.IsDeviceIdUpdated(mSharedPref) || !this.mView.GetDeviceId().Equals(UserSessions.GetDeviceId(mSharedPref)))
                                            {
                                                UserEntity.RemoveActive();
                                                UserRegister.RemoveActive();
                                                CustomerBillingAccount.RemoveActive();
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
                                                UserSessions.UpdateDeviceId(mSharedPref);
                                                UserSessions.SaveDeviceId(mSharedPref, this.mView.GetDeviceId());
                                                this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Logout);
                                                this.mView.ShowLogout();

                                            }
                                            else
                                            {
                                                this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                                                this.mView.SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Dashboard);
                                                this.mView.ShowDashboard();
                                            }
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
                    else if (masterDataResponse.Data.ErrorCode == "7000")
                    {
                        if (masterDataResponse.Data.DisplayMessage != null && masterDataResponse.Data.DisplayTitle != null)
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
            catch (Newtonsoft.Json.JsonReaderException e)
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
            Log.Debug(TAG, string.Format("AppLaunchMasterData Service failed in {0} seconds: Retry: {1} ", this.appLaunchMasterDataTimeout, serviceCallCounter));
            if (serviceCallCounter == 1)//If first failed, do auto-retry.
            {
                this.appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT;
                LoadAccounts();
            }
            if (serviceCallCounter == 2)//If still failed, do auto-retry.
            {
                this.appLaunchMasterDataTimeout = Constants.APP_LAUNCH_MASTER_DATA_RETRY_TIMEOUT;
                LoadAccounts();
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
                        LoadAccounts();
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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    string json = getItemsService.GetTimestampItem();
                    TimestampResponseModel responseModel = JsonConvert.DeserializeObject<TimestampResponseModel>(json);
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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
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
                        imageCache = ImageUtils.GetImageBitmapFromUrl(item.Image);
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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
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

        public Task GetSSMRWalkThrough()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);

                    ApplySSMRTimeStampResponseModel timestampModel = getItemsService.GetApplySSMRWalkthroughTimestampItem();
                    if (timestampModel.Status.Equals("Success") && timestampModel.Data != null && timestampModel.Data.Count > 0)
                    {
                        if (SitecoreCmsEntity.IsNeedUpdates(SitecoreCmsEntity.SITE_CORE_ID.APPLY_SSMR_WALKTHROUGH, timestampModel.Data[0].Timestamp))
                        {
                            ApplySSMRResponseModel responseModel = getItemsService.GetApplySSMRWalkthroughItems();

                            if (responseModel.Status.Equals("Success"))
                            {
                                SitecoreCmsEntity.InsertSiteCoreItem(SitecoreCmsEntity.SITE_CORE_ID.APPLY_SSMR_WALKTHROUGH, JsonConvert.SerializeObject(responseModel.Data), timestampModel.Data[0].Timestamp);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }
    }
}
