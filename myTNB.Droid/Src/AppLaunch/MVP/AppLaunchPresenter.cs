using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.AppLaunch.Api;
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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static myTNB_Android.Src.AppLaunch.Models.MasterDataRequest;

namespace myTNB_Android.Src.AppLaunch.MVP
{

    public class AppLaunchPresenter : AppLaunchContract.IUserActionsListener
    {
        private AppLaunchContract.IView mView;
        private ISharedPreferences mSharedPref;
        public static readonly string TAG = "LaunchViewActivity";
        CancellationTokenSource cts;

        private string savedPromoTimeStamp = "0000000";

        private string mApplySSMRSavedTimeStamp = "0000000";

        public AppLaunchPresenter(AppLaunchContract.IView mView, ISharedPreferences sharedPreferences)
        {
            this.mView = mView;
            this.mSharedPref = sharedPreferences;
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
                    OnGetSSMRTimeStamp();
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
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
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
                }, cts.Token);


                if (masterDataResponse != null && masterDataResponse.Data != null)
                {
                    if (masterDataResponse.Data.ErrorCode == "7200" && masterDataResponse.Data.ErrorCode != "7000")
                    {
                        new MasterApiDBOperation(masterDataResponse, mSharedPref).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");

                        bool proceed = false;

                        bool appUpdateAvailable = false;
                        if (masterDataResponse.Data.MasterData.AppVersionList != null && masterDataResponse.Data.MasterData.AppVersionList.Count > 0)
                        {
                            //foreach (AppVersionList versionList in masterDataResponse.Data.MasterData.AppVersionList)
                            //{
                            //    int serverVerison;
                            //    if (versionList.Platform.Equals("1") || versionList.Platform.Equals("Android"))
                            //    {
                            //        if (string.IsNullOrEmpty(versionList.Version))
                            //        {
                            //            appUpdateAvailable = false;
                            //        }
                            //        else if (int.TryParse(versionList.Version, out serverVerison))
                            //        {
                            //            serverVerison = int.Parse(versionList.Version);
                            //            if (serverVerison > DeviceIdUtils.GetAppVersionCode())
                            //            {
                            //                appUpdateAvailable = true;
                            //            }
                            //        }
                            //    }
                            //}
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

                                        if (UserSessions.HasNotification(mSharedPref) && (loggedUser.Email.Equals(UserSessions.GetUserEmailNotification(mSharedPref)) ||
                                            "ALL_MYTNB_USERS".Equals(UserSessions.GetUserEmailNotification(mSharedPref))))
                                        {
                                            UserSessions.RemoveNotificationSession(mSharedPref);
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
                                                this.mView.ShowLogout();

                                            }
                                            else
                                            {
                                                this.mView.ShowNotificationCount(UserNotificationEntity.Count());
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
                                    mView.ShowPreLogin();
                                }
                                else
                                {
                                    if (!UserSessions.IsDeviceIdUpdated(mSharedPref) || !this.mView.GetDeviceId().Equals(UserSessions.GetDeviceId(mSharedPref)))
                                    {
                                        UserSessions.UpdateDeviceId(mSharedPref);
                                        UserSessions.SaveDeviceId(mSharedPref, this.mView.GetDeviceId());
                                    }
                                    mView.ShowWalkThrough();
                                }
                            }
                        }
                        else
                        {
                            this.mView.ShowRetryOptionApiException(null);
                        }
                    }
                    else if (masterDataResponse.Data.ErrorCode == "7000")
                    {
                        if (masterDataResponse.Data.DisplayMessage != null && masterDataResponse.Data.DisplayTitle != null)
                        {
                            this.mView.ShowMaintenance(masterDataResponse);
                        }
                        else
                        {
                            this.mView.ShowRetryOptionApiException(null);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Excution time enters else");
                        this.mView.ShowRetryOptionApiException(null);
                    }
                }
                else
                {
                    Console.WriteLine("Excution time enters else");
                    this.mView.ShowRetryOptionApiException(null);
                }
            }
            catch (ApiException apiException)
            {
                
                this.mView.ShowRetryOptionApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                
                this.mView.ShowRetryOptionUknownException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                
                this.mView.ShowRetryOptionUknownException(e);
                Utility.LoggingNonFatalError(e);
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

        public void OnGetSSMRTimeStamp()
        {
            try
            {
                OnboardingSSMRParentEntity wtManager = new OnboardingSSMRParentEntity();
                List<OnboardingSSMRParentEntity> items = wtManager.GetAllItems();
                if (items != null && items.Count != 0)
                {
                    foreach (OnboardingSSMRParentEntity obj in items)
                    {
                        mApplySSMRSavedTimeStamp = obj.Timestamp;
                    }
                }
                GetSSMRWalkThroughTimeStamp();
            }
            catch (Exception e)
            {
                GetSSMRWalkThroughTimeStamp();
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task GetSSMRWalkThroughTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    ApplySSMRTimeStampResponseModel test = getItemsService.GetApplySSMRWalkthroughTimestampItem();

                    if (test.Status.Equals("Success"))
                    {
                        OnboardingSSMRParentEntity wtManager = new OnboardingSSMRParentEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(test.Data);
                    }
                    if (test.Data[0].Timestamp != mApplySSMRSavedTimeStamp)
                    {
                        GetSSMRWalkThrough();
                    }
                }
                catch (Exception e)
                {
                    GetSSMRWalkThrough();
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public Task GetSSMRWalkThrough()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    ApplySSMRResponseModel responseModel = getItemsService.GetApplySSMRWalkthroughItems();

                    if (responseModel.Status.Equals("Success"))
                    {
                        OnboardingSSMREntity wtManager = new OnboardingSSMREntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
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
