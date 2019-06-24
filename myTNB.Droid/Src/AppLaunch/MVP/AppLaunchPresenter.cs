using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using myTNB_Android.Src.Utils;
using Refit;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.AppLaunch.Requests;
using Android.Util;
using System.Net.Http;
using myTNB_Android.Src.Database.Model;
using SQLite;
using System.Threading.Tasks;
using myTNB_Android.Src.AppLaunch.Api;
using System.Threading;
using myTNB_Android.Src.AppLaunch.Models;
using Android.Gms.Common;
using myTNB_Android.Src.FindUs.Api;
using myTNB_Android.Src.FindUs.Request;
using static myTNB_Android.Src.FindUs.Response.GetLocationTypesResponse;
using myTNB_Android.Src.Base.Models;
using myTNB.SitecoreCMS.Services;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.SiteCore;
using Newtonsoft.Json;
using myTNB.SQLite.SQLiteDataManager;
using Android.Content.PM;
using myTNB_Android.Src.AddAccount.Api;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Login.Api;
using System.Globalization;
using myTNB_Android.Src.AppLaunch.Async;

using myTNB_Android.Src.Base.Adapter;

namespace myTNB_Android.Src.AppLaunch.MVP
{

    public class AppLaunchPresenter : AppLaunchContract.IUserActionsListener
    {
        private AppLaunchContract.IView mView;
        private ISharedPreferences mSharedPref;
        public static readonly string TAG = "LaunchViewActivity";
        CancellationTokenSource cts;

        private string savedPromoTimeStamp = "0000000";

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

                // load accounts here
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

                    //bool isGranted = this.mView.IsGrantedSMSReceivePermission();
                    //if (!isGranted)
                    //{
                    //    if (this.mView.ShouldShowSMSReceiveRationale())
                    //    {
                    //        this.mView.ShowSMSPermissionRationale();
                    //    }
                    //    else
                    //    {
                    //        this.mView.RequestSMSPermission();
                    //    }


                    //}


                    Console.WriteLine("GooglePlayServices is Installed");
                    ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

                    LoadAccounts();
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
            //var watch = new System.Diagnostics.Stopwatch();

            //watch.Start();
            //Log.Debug("Load Account:", ""+Convert.ToDateTime(new DateTime(), CultureInfo.InvariantCulture));
            //Console.WriteLine("Load account method: " + Convert.ToDateTime(new DateTime(), CultureInfo.InvariantCulture));
            cts = new CancellationTokenSource();
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            //var api = RestService.For<INotificationApi>(httpClient);
            //var weblinkApi = RestService.For<IWeblinksApi>(httpClient);
            //var locationTypesApi = RestService.For<GetLocationTypseApi>(httpClient);
            var feedbackApi = RestService.For<IFeedbackApi>(httpClient);

            var masterDataApi = RestService.For<GetMasterDataApi>(httpClient);

            var getPhoneVerifyApi = RestService.For<GetPhoneVerifyStatusApi>(httpClient);


#else
            var api = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
            //var weblinkApi = RestService.For<IWeblinksApi>(Constants.SERVER_URL.END_POINT);
            //var locationTypesApi = RestService.For<GetLocationTypseApi>(Constants.SERVER_URL.END_POINT);
            var feedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);

            var masterDataApi = RestService.For<GetMasterDataApi>(Constants.SERVER_URL.END_POINT);

            var getPhoneVerifyApi = RestService.For<GetPhoneVerifyStatusApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                //var appNotificationChannelsResponse = await api.GetAppNotificationChannels(new NotificationRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                //}, cts.Token);

                //var appNotificationTypesResponse = await api.GetAppNotificationTypes(new NotificationRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                //}, cts.Token);

                //var webLinkResponse = await weblinkApi.GetWebLinks(new WeblinkRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                //}, cts.Token);

                //var locationTypesResponse = await locationTypesApi.GetLocationTypes(new GetLocationTypesRequest()
                //{
                //    apiKeyID = Constants.APP_CONFIG.API_KEY_ID
                //}, cts.Token);

                //var feedbackCategoryResponse = await feedbackApi.GetFeedbackCategory(new FeedbackCategoryRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                //}, cts.Token);

                //var feedbackStateResponse = await feedbackApi.GetStatesForFeedback(new FeedbackStateRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                //}, cts.Token);

                //var feedbackTypeResponse = await feedbackApi.GetOtherFeedbackType(new FeedbackTypeRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                //} , cts.Token);


                Context mContext = MyTNBApplication.Context;
                string email = null, sspUserID = null;
                if (UserEntity.IsCurrentlyActive())
                {
                    email = UserEntity.GetActive().UserName;
                    sspUserID = UserEntity.GetActive().UserID;
                }



                //var masterWatch = new System.Diagnostics.Stopwatch();

                //masterWatch.Start();

                var masterDataResponse = await masterDataApi.GetAppLaunchMasterData(new MasterDataRequest()
                {
                    ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceId = this.mView.GetDeviceId(),
                    AppVersion = DeviceIdUtils.GetAppVersionName(),
                    Email = email,
                    SSPUserId = sspUserID,
                    OsType = Constants.DEVICE_PLATFORM,
                    OsVersion = DeviceIdUtils.GetAndroidVersion()
                }, cts.Token);


                //MasterDataRequest masterRequest = new MasterDataRequest();
                //masterRequest.ApiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                //masterRequest.DeviceId = this.mView.GetDeviceId();
                //masterRequest.AppVersion = DeviceIdUtils.GetAppVersionName();
                //masterRequest.Email = email;
                //masterRequest.SSPUserId = sspUserID;
                //masterRequest.OsType = Constants.DEVICE_PLATFORM;
                //masterRequest.OsVersion = DeviceIdUtils.GetAndroidVersion();

                //String jsonStr = JsonConvert.SerializeObject(masterRequest);

                //string response = await new HttpUtility().MakeRequest(Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetAppLaunchMasterData", eRequestMethod.ePost,  jsonStr);

                //Console.WriteLine("Execution time response: {0}", response);

                //MasterDataResponse masterDataResponse = JsonConvert.DeserializeObject<MasterDataResponse>(response);

                // TODO: ADD THIS TO REGISTER & LOGIN
                if (!masterDataResponse.Data.IsError && !masterDataResponse.Data.Status.ToUpper().Equals(Constants.MAINTENANCE_MODE))
                {
                    new MasterApiDBOperation(masterDataResponse, mSharedPref).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                    //Console.WriteLine("Excution time enters if");
                    //foreach (Weblink web in masterDataResponse.Data.MasterData.WebLinks)
                    //{
                    //    int newRecord = WeblinkEntity.InsertOrReplace(web);
                    //    Log.Debug(TAG, "New Weblink Record " + newRecord);
                    //}

                    //Log.Debug(TAG, "Weblink Records " + WeblinkEntity.Count());

                    //FeedbackCategoryEntity.RemoveActive();
                    //foreach (FeedbackCategory cat in masterDataResponse.Data.MasterData.FeedbackCategorys)
                    //{
                    //    int newRecord = FeedbackCategoryEntity.InsertOrReplace(cat);
                    //}

                    //int ctr = 0;
                    //FeedbackStateEntity.RemoveActive();
                    //foreach (FeedbackState state in masterDataResponse.Data.MasterData.States)
                    //{
                    //    bool isSelected = ctr == 0 ? true : false;
                    //    int newRecord = FeedbackStateEntity.InsertOrReplace(state, isSelected);
                    //    ctr++;
                    //}


                    //FeedbackTypeEntity.RemoveActive();
                    //ctr = 0;
                    //foreach (FeedbackType type in masterDataResponse.Data.MasterData.FeedbackTypes)
                    //{
                    //    bool isSelected = ctr == 0 ? true : false;
                    //    int newRecord = FeedbackTypeEntity.InsertOrReplace(type, isSelected);
                    //    Console.WriteLine(string.Format("FeedbackType Id = {0}", newRecord));
                    //    ctr++;
                    //}


                    //foreach (NotificationChannels notificationChannel in masterDataResponse.Data.MasterData.NotificationTypeChannels)
                    //{
                    //    int newRecord = NotificationChannelEntity.InsertOrReplace(notificationChannel);
                    //    Log.Debug(TAG, "New Channel Record " + newRecord);
                    //}

                    //foreach (NotificationTypes notificationTypes in masterDataResponse.Data.MasterData.NotificationTypes)
                    //{
                    //    int newRecord = NotificationTypesEntity.InsertOrReplace(notificationTypes);
                    //    Log.Debug(TAG, "New Type Record " + newRecord);
                    //}

                    //LocationTypesEntity.InsertFristRecord();
                    //foreach (LocationType loc in masterDataResponse.Data.MasterData.LocationTypes)
                    //{
                    //    int newRecord = LocationTypesEntity.InsertOrReplace(loc);
                    //    Log.Debug(TAG, "Location Types Record " + newRecord);
                    //}


                    //Log.Debug(TAG, "Location Records " + LocationTypesEntity.Count());

                    //DownTimeEntity.RemoveActive();
                    //foreach (DownTime cat in masterDataResponse.Data.MasterData.Downtimes)
                    //{
                    //    int newRecord = DownTimeEntity.InsertOrReplace(cat);
                    //}

                    //Log.Debug(TAG, "DownTime Records " + DownTimeEntity.Count());


                    // Save promotions
                    //var promotionWatch = new System.Diagnostics.Stopwatch();
                    //promotionWatch.Start();

                    //try
                    //{
                    //    PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                    //    wtManager.CreateTable();
                    //    List<PromotionsParentEntityV2> saveditems = wtManager.GetAllItems();
                    //    if (saveditems != null && saveditems.Count > 0)
                    //    {
                    //        PromotionsParentEntityV2 entity = saveditems[0];
                    //        if (entity != null)
                    //        {
                    //            savedPromoTimeStamp = entity.Timestamp;
                    //        }
                    //    }

                    //    //Get Sitecore promotion timestamp
                    //    bool getSiteCorePromotions = false;
                    //    cts = new CancellationTokenSource();                        
                    //    await Task.Factory.StartNew(() =>
                    //     {
                    //         try
                    //         {
                    //             string density = DPUtils.GetDeviceDensity(Application.Context);
                    //             GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    //             string json = getItemsService.GetPromotionsTimestampItem();
                    //             PromotionsParentV2ResponseModel responseModel = JsonConvert.DeserializeObject<PromotionsParentV2ResponseModel>(json);
                    //             if (responseModel.Status.Equals("Success"))
                    //             {
                    //                //PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                    //                wtManager.DeleteTable();
                    //                wtManager.CreateTable();
                    //                wtManager.InsertListOfItems(responseModel.Data);
                    //                 List<PromotionsParentEntityV2> items = wtManager.GetAllItems();
                    //                if (items != null && items.Count() > 0)
                    //                 {
                    //                     PromotionsParentEntityV2 entity = items[0];
                    //                     if (entity != null)
                    //                     {
                    //                         if (!entity.Timestamp.Equals(savedPromoTimeStamp))
                    //                         {
                    //                             getSiteCorePromotions = true;
                    //                         }
                    //                         else
                    //                         {
                    //                             getSiteCorePromotions = false;
                    //                         }
                    //                     }
                    //                 }

                    //                Log.Debug("WalkThroughResponse", responseModel.Data.ToString());
                    //             }
                    //             else
                    //             {
                    //                 getSiteCorePromotions = true;
                    //             }
                    //         }
                    //         catch (Exception e)
                    //         {
                    //             Log.Error("API Exception", e.StackTrace);
                    //            Utility.LoggingNonFatalError(e);
                    //         }
                    //     }).ContinueWith((Task previous) =>
                    //     {
                    //     }, cts.Token);

                    //    //promotionWatch.Stop();
                    //    //Console.WriteLine($"Execution Time for promotion: {promotionWatch.ElapsedMilliseconds} ms");

                    //    if (getSiteCorePromotions)
                    //    {
                    //        //var getPromotionWatch = new System.Diagnostics.Stopwatch();
                    //        //getPromotionWatch.Start();

                    //        cts = new CancellationTokenSource();
                    //        await Task.Factory.StartNew(() =>
                    //        {
                    //            try
                    //            {
                    //                string density = DPUtils.GetDeviceDensity(Application.Context);
                    //                GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    //                string json = getItemsService.GetPromotionsV2Item();
                    //                PromotionsV2ResponseModel promoResponseModel = JsonConvert.DeserializeObject<PromotionsV2ResponseModel>(json);
                    //                if (promoResponseModel.Status.Equals("Success"))
                    //                {
                    //                    PromotionsEntityV2 wtManager2 = new PromotionsEntityV2();
                    //                    wtManager2.DeleteTable();
                    //                    wtManager2.CreateTable();
                    //                    wtManager2.InsertListOfItems(promoResponseModel.Data);
                    //                    Log.Debug("DashboardPresenter", promoResponseModel.Data.ToString());
                    //                }

                    //            }
                    //            catch (Exception e)
                    //            {
                    //                Log.Error("API Exception", e.StackTrace);
                    //                Utility.LoggingNonFatalError(e);
                    //            }
                    //        }).ContinueWith((Task previous) => {
                    //        }, cts.Token);

                    //        //getPromotionWatch.Stop();
                    //        //Console.WriteLine($"Execution Time for get promotion: {getPromotionWatch.ElapsedMilliseconds} ms");
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    Log.Error("DB Exception", e.StackTrace);
                    //    Utility.LoggingNonFatalError(e);
                    //}

                    ///<summary>
                    /// This is to check users phone number is verified or not
                    ///</summary>
                    ///<START></START>s

                    bool proceed = false;
                    //if (UserEntity.IsCurrentlyActive())
                    //{

                    //try
                    //{
                    //    UserEntity entity = UserEntity.GetActive();
                    //    bool phoneVerified = UserSessions.GetPhoneVerifiedFlag(mSharedPref);
                    //    if (!phoneVerified)
                    //    {
                    //        //var phoneVerifyWatch = new System.Diagnostics.Stopwatch();
                    //        //phoneVerifyWatch.Start();
                    //        PhoneVerifyStatusResponse phoneVerifyResponse = await getPhoneVerifyApi.GetPhoneVerifyStatus(new GetPhoneVerifyStatusRequest()
                    //        {
                    //            ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    //            Email = entity.Email,
                    //            SSPUserID = entity.UserID,
                    //            DeviceID = this.mView.GetDeviceId()
                    //        }, cts.Token);
                    //        //phoneVerifyWatch.Stop();
                    //        //Console.WriteLine($"Execution Time for phone verification: {phoneVerifyWatch.ElapsedMilliseconds} ms");

                    //        if (!phoneVerifyResponse.verificationData.IsError)
                    //        {
                    //            if (!phoneVerifyResponse.verificationData.Data.IsPhoneVerified)
                    //            {
                    //                this.mView.ShowUpdatePhoneNumber(phoneVerifyResponse.verificationData.Data.PhoneNumber);
                    //                proceed = false;
                    //            }
                    //            else
                    //            {
                    //                proceed = true;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            proceed = true;
                    //        }


                    //    }
                    //    else
                    //    {
                    //        proceed = true;
                    //    }
                    //}
                    //catch (System.Exception e)
                    //{
                    //    Log.Debug("Package Manager", e.StackTrace);
                    //    Utility.LoggingNonFatalError(e);
                    //}
                    //}
                    //else
                    //{
                    //    proceed = true;
                    //}
                    ///<END></END>

                    //if (proceed)
                    //{
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
                                        //var phoneVerifyWatch = new System.Diagnostics.Stopwatch();
                                        //phoneVerifyWatch.Start();
                                        PhoneVerifyStatusResponse phoneVerifyResponse = await getPhoneVerifyApi.GetPhoneVerifyStatus(new GetPhoneVerifyStatusRequest()
                                        {
                                            ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                                            Email = entity.Email,
                                            SSPUserID = entity.UserID,
                                            DeviceID = this.mView.GetDeviceId()
                                        }, cts.Token);
                                        //phoneVerifyWatch.Stop();
                                        //Console.WriteLine($"Execution Time for phone verification: {phoneVerifyWatch.ElapsedMilliseconds} ms");

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



                                //var userNotiWatch = new System.Diagnostics.Stopwatch();
                                //userNotiWatch.Start();  
                                if (proceed)
                                {
                                    UserEntity loggedUser = UserEntity.GetActive();
                                    SummaryDashBoardAccountEntity.RemoveAll();
                                    CustomerBillingAccount.RemoveSelected();
                                    CustomerBillingAccount.MakeFirstAsSelected();
                                    BillHistoryEntity.RemoveAll();
                                    PaymentHistoryEntity.RemoveAll();
                                    //var userNotificationResponse = await api.GetUserNotifications(new UserNotificationRequest()
                                    //{
                                    //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                                    //    Email = loggedUser.Email,
                                    //    DeviceId = this.mView.GetDeviceId()

                                    //}, cts.Token);

                                    ////userNotiWatch.Stop();
                                    ////Console.WriteLine($"Execution Time for user notification: {userNotiWatch.ElapsedMilliseconds} ms");

                                    //if (!userNotificationResponse.Data.IsError)
                                    //{
                                    //    foreach (UserNotification userNotification in userNotificationResponse.Data.Data)
                                    //    {
                                    //        // tODO : SAVE ALL NOTIFICATIONs
                                    //        int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                    //    }


                                    //}



                                    //var feedBackWatch = new System.Diagnostics.Stopwatch();
                                    //feedBackWatch.Start();
                                    //var submittedFeedbackResponse = await feedbackApi.GetSubmittedFeedbackList(new Base.Request.SubmittedFeedbackRequest()
                                    //{
                                    //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                                    //    Email = loggedUser.Email,
                                    //    DeviceId = this.mView.GetDeviceId()

                                    //}, cts.Token);

                                    //feedBackWatch.Stop();
                                    //Console.WriteLine($"Execution Time for Feedback: {feedBackWatch.ElapsedMilliseconds} ms");

                                    //if (!submittedFeedbackResponse.Data.IsError)
                                    //{
                                    //    SubmittedFeedbackEntity.Remove();
                                    //    foreach (SubmittedFeedback sFeed in submittedFeedbackResponse.Data.Data)
                                    //    {
                                    //        int newRecord = SubmittedFeedbackEntity.InsertOrReplace(sFeed);
                                    //        Console.WriteLine(string.Format("SubmitFeedback Id = {0}", newRecord));
                                    //    }
                                    //}

                                    if (UserSessions.HasNotification(mSharedPref) && loggedUser.Email.Equals(UserSessions.GetUserEmailNotification(mSharedPref)))
                                    {
                                        UserSessions.RemoveNotificationSession(mSharedPref);
                                        this.mView.ShowNotification();
                                    }
                                    else
                                    {
                                        //#if DEBUG || DEVELOP
                                        //                            var newHttpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                                        //                            var customerAccountsApi = RestService.For<GetCustomerAccounts>(newHttpClient);
                                        //#else
                                        //                            var customerAccountsApi = RestService.For<GetCustomerAccounts>(Constants.SERVER_URL.END_POINT);
                                        //#endif
                                        //                            var customerAccountsResponse = await customerAccountsApi.GetCustomerAccountV5(new AddAccount.Requests.GetCustomerAccountsRequest(Constants.APP_CONFIG.API_KEY_ID, UserEntity.GetActive().UserID));
                                        //                            if (!customerAccountsResponse.D.IsError && customerAccountsResponse.D.AccountListData.Count > 0)
                                        //                            {
                                        //                                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
                                        //                                foreach (Account acc in customerAccountsResponse.D.AccountListData)
                                        //                                {
                                        //                                    bool isSelected = acc.AccountNumber.Equals(customerBillingAccount.AccNum) ? true : false;
                                        //                                    int rowChange = CustomerBillingAccount.InsertOrReplace(acc, isSelected);

                                        //                                }
                                        //                            }

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
                    //}
                }
                else if (masterDataResponse.Data.Status.ToUpper().Equals(Constants.MAINTENANCE_MODE))
                {
                    if (masterDataResponse.Data.MasterData.MaintainanceMessage != null && masterDataResponse.Data.MasterData.MaintainanceTitle != null)
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
                    // TODO : SHOW ERROR
                    this.mView.ShowRetryOptionApiException(null);
                }



            }
            catch (ApiException apiException)
            {
                //Log.Debug(TAG, "Api Exception " + apiException.GetContentAs<string>());
                //Log.Debug(TAG, "Api Exception " + apiException.StatusCode);
                //Log.Debug(TAG, "Api Exception " + apiException);
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
            //Log.Debug("Load Account:", "" + Convert.ToDateTime(new DateTime(), CultureInfo.InvariantCulture));
            //Console.WriteLine("Load account method: " + Convert.ToDateTime(new DateTime(), CultureInfo.InvariantCulture));
            //watch.Stop();
            //Console.WriteLine($"Execution Time for user notification: {watch.ElapsedMilliseconds} ms");
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
                // SILENTLY DIE , SMS RECEIVE IS ONLY OPTIONAL
                if (requestCode == Constants.RUNTIME_PERMISSION_SMS_REQUEST_CODE)
                {
                    if (Utility.IsPermissionHasCount(grantResults))
                    {
                        if (grantResults[0] == Permission.Denied)
                        {
                            //if (this.mView.ShouldShowSMSReceiveRationale())
                            //{
                            //    this.mView.ShowSMSPermissionRationale();
                            //}
                        }
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

    }




}