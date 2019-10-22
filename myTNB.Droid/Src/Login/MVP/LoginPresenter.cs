using Android.App;
using Android.Content;
using Android.Text;
using Android.Util;
using Firebase.Iid;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.AddAccount.Api;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Api;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.LogoutRate.Api;
using myTNB_Android.Src.MyTNBService.Notification;
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

namespace myTNB_Android.Src.Login.MVP
{
    public class LoginPresenter : LoginContract.IUserActionsListener
    {
        public static readonly string TAG = "LoginPresenter";
        private LoginContract.IView mView;
        private ISharedPreferences mSharedPref;

        CancellationTokenSource cts;

        private string savedPromoTimeStamp = "0000000";

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
            cts = new CancellationTokenSource();
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

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if STUB
            var api = RestService.For<IAuthenticateUser>(Constants.SERVER_URL.END_POINT);
            var notificationsApi = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IAuthenticateUser>(httpClient);
            var notificationsApi = RestService.For<INotificationApi>(httpClient);
#else
            var api = RestService.For<IAuthenticateUser>(Constants.SERVER_URL.END_POINT);
            var notificationsApi = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
#endif
            Log.Debug(TAG, "Awaiting...");
            try
            {
                //var userResponse = await api.DoLoginV5(new UserAuthenticationRequest(Constants.APP_CONFIG.API_KEY_ID,
                //   username,
                //   password,
                //   Constants.APP_CONFIG.API_KEY_ID,
                //   Constants.APP_CONFIG.API_KEY_ID,
                //   Constants.APP_CONFIG.API_KEY_ID,
                //   Constants.APP_CONFIG.API_KEY_ID,
                //   Constants.APP_CONFIG.API_KEY_ID,
                //   Constants.APP_CONFIG.API_KEY_ID), cts.Token);

                //if (userResponse.userData != null)
                //{
                //    Log.Debug(TAG , "User Response = " + userResponse.userData.status);
                //}
                string fcmToken = String.Empty;

                if (FirebaseTokenEntity.HasLatest())
                {
                    fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
                }
                else
                {
                    fcmToken = FirebaseInstanceId.Instance.Token;
                    FirebaseTokenEntity.InsertOrReplace(fcmToken, true);
                }

                var userResponse = await api.DoLogin(new UserAuthenticationRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    UserName = usrNme,
                    Password = pwd,
                    IpAddress = Constants.APP_CONFIG.API_KEY_ID,
                    ClientType = DeviceIdUtils.GetAppVersionName(),
                    ActiveUserName = Constants.APP_CONFIG.API_KEY_ID,
                    DevicePlatform = Constants.DEVICE_PLATFORM,
                    DeviceVersion = DeviceIdUtils.GetAndroidVersion(),
                    DeviceCordova = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceId = deviceId,
                    FcmToken = fcmToken,




                }, cts.Token);



                if (userResponse.Data.IsError || userResponse.Data.Status.Equals("failed"))
                {
                    if (this.mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                        this.mView.ShowInvalidEmailPasswordError(userResponse.Data.Message);
                    }
                }
                else
                {
                    ///<summary>
                    ///THIS TO SAVE UPDATE THAT LOGOUT HAS BEEN DONE - WHILE UPGRADING VERSION 6 TO 7
                    ///</summary>
                    UserSessions.SaveLogoutFlag(mSharedPref, true);

                    if (rememberMe)
                    {
                        UserSessions.SaveUserEmail(mSharedPref, usrNme);
                    }
                    else
                    {
                        UserSessions.SaveUserEmail(mSharedPref, "");
                    }

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
                    else if (!userResponse.Data.User.isPhoneVerified)
                    {
                        UserAuthenticationRequest loginRequest = new UserAuthenticationRequest(Constants.APP_CONFIG.API_KEY_ID)
                        {
                            UserName = usrNme,
                            Password = pwd,
                            IpAddress = Constants.APP_CONFIG.API_KEY_ID,
                            ClientType = DeviceIdUtils.GetAppVersionName(),
                            ActiveUserName = userResponse.Data.User.UserId,
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
                        this.mView.ShowUpdatePhoneNumber(loginRequest, userResponse.Data.User.MobileNo);
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
                        UserSessions.RemoveSessionData();
                        try
                        {
                            UserEntity.UpdatePhoneNumber(userResponse.Data.User.MobileNo);
                            UserSessions.SavePhoneVerified(mSharedPref, true);
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }

                        int Id = UserEntity.InsertOrReplace(userResponse.Data.User);
                        if (Id > 0)
                        {
                            UserEntity.UpdateDeviceId(deviceId);

#if STUB
                            var customerAccountsApi = RestService.For<GetCustomerAccounts>(Constants.SERVER_URL.END_POINT);

#elif DEBUG
                            var newHttpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                            var customerAccountsApi = RestService.For<GetCustomerAccounts>(newHttpClient);
#else
                        var customerAccountsApi = RestService.For<GetCustomerAccounts>(Constants.SERVER_URL.END_POINT);
#endif
                            var newObject = new
                            {
                                usrInf = new
                                {
                                    eid = UserEntity.GetActive().UserName,
                                    sspuid = userResponse.Data.User.UserId,
                                    lang = "EN",
                                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                                    sec_auth_k2 = "",
                                    ses_param1 = "",
                                    ses_param2 = ""
                                }
                            };
                            var customerAccountsResponse = await customerAccountsApi.GetCustomerAccountV6(newObject);
                            if (customerAccountsResponse.D.ErrorCode == "7200" && customerAccountsResponse.D.AccountListData.Count > 0)
                            {
                                int ctr = 0;
                                foreach (Account acc in customerAccountsResponse.D.AccountListData)
                                {
                                    bool isSelected = ctr == 0 ? true : false;
                                    int rowChange = CustomerBillingAccount.InsertOrReplace(acc, isSelected);
                                    ctr++;
                                }
                            }

                            List<NotificationTypesEntity> notificationTypes = NotificationTypesEntity.List();
                            NotificationFilterEntity.InsertOrReplace(Constants.ZERO_INDEX_FILTER, Constants.ZERO_INDEX_TITLE, true);
                            foreach (NotificationTypesEntity notificationType in notificationTypes)
                            {
                                if (notificationType.ShowInFilterList)
                                {
                                    NotificationFilterEntity.InsertOrReplace(notificationType.Id, notificationType.Title, false);
                                }
                            }

                            NotificationApiImpl notificationAPI = new NotificationApiImpl();
                            MyTNBService.Response.UserNotificationResponse response = await notificationAPI.GetUserNotifications<MyTNBService.Response.UserNotificationResponse>(new Base.Request.APIBaseRequest());
                            if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
                            {
                                if (response.Data.ResponseData != null && response.Data.ResponseData.UserNotificationList != null &&
                                    response.Data.ResponseData.UserNotificationList.Count > 0)
                                {
                                    foreach (UserNotification userNotification in response.Data.ResponseData.UserNotificationList)
                                    {
                                        // tODO : SAVE ALL NOTIFICATIONs
                                        int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        UserNotificationEntity.RemoveAll();
                                    }
                                    catch (System.Exception ne)
                                    {
                                        Utility.LoggingNonFatalError(ne);
                                    }
                                }
                            }

                            // Save promotions
                            try
                            {
                                PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                                wtManager.CreateTable();
                                List<PromotionsParentEntityV2> saveditems = wtManager.GetAllItems();
                                if (saveditems != null && saveditems.Count > 0)
                                {
                                    PromotionsParentEntityV2 entity = saveditems[0];
                                    if (entity != null)
                                    {
                                        savedPromoTimeStamp = entity.Timestamp;
                                    }
                                }

                                //Get Sitecore promotion timestamp
                                bool getSiteCorePromotions = false;
                                cts = new CancellationTokenSource();
                                await Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        string density = DPUtils.GetDeviceDensity(Application.Context);
                                        GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                                        string json = getItemsService.GetPromotionsTimestampItem();
                                        PromotionsParentV2ResponseModel responseModel = JsonConvert.DeserializeObject<PromotionsParentV2ResponseModel>(json);
                                        if (responseModel.Status.Equals("Success"))
                                        {
                                            //PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                                            wtManager.DeleteTable();
                                            wtManager.CreateTable();
                                            wtManager.InsertListOfItems(responseModel.Data);
                                            List<PromotionsParentEntityV2> items = wtManager.GetAllItems();
                                            if (items != null)
                                            {
                                                PromotionsParentEntityV2 entity = items[0];
                                                if (entity != null)
                                                {
                                                    if (!entity.Timestamp.Equals(savedPromoTimeStamp))
                                                    {
                                                        getSiteCorePromotions = true;
                                                    }
                                                    else
                                                    {
                                                        getSiteCorePromotions = false;
                                                    }
                                                }
                                            }

                                            Log.Debug("WalkThroughResponse", responseModel.Data.ToString());
                                        }
                                        else
                                        {
                                            getSiteCorePromotions = true;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        if (mView.IsActive())
                                        {
                                            this.mView.HideProgressDialog();
                                        }
                                        Log.Error("API Exception", e.StackTrace);
                                        Utility.LoggingNonFatalError(e);
                                    }
                                }).ContinueWith((Task previous) =>
                                {
                                }, cts.Token);



                                if (getSiteCorePromotions)
                                {
                                    cts = new CancellationTokenSource();
                                    await Task.Factory.StartNew(() =>
                                    {
                                        try
                                        {
                                            string density = DPUtils.GetDeviceDensity(Application.Context);
                                            GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                                            string json = getItemsService.GetPromotionsV2Item();
                                            PromotionsV2ResponseModel promoResponseModel = JsonConvert.DeserializeObject<PromotionsV2ResponseModel>(json);
                                            if (promoResponseModel.Status.Equals("Success"))
                                            {
                                                PromotionsEntityV2 wtManager2 = new PromotionsEntityV2();
                                                wtManager2.DeleteTable();
                                                wtManager2.CreateTable();
                                                wtManager2.InsertListOfItems(promoResponseModel.Data);
                                                Log.Debug("DashboardPresenter", promoResponseModel.Data.ToString());
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error("API Exception", e.StackTrace);
                                            Utility.LoggingNonFatalError(e);
                                        }
                                    }).ContinueWith((Task previous) =>
                                    {
                                    }, cts.Token);
                                }
                            }
                            catch (Exception e)
                            {
                                if (mView.IsActive())
                                {
                                    this.mView.HideProgressDialog();
                                }
                                Log.Error("DB Exception", e.StackTrace);
                                Utility.LoggingNonFatalError(e);
                            }


                            //Console.WriteLine(string.Format("Rows updated {0}" , CustomerBillingAccount.List().Count));
                            if (this.mView.IsActive())
                            {
                                this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                            }
                            //UserSessions.SavePhoneVerified(mSharedPref, true);
                            this.mView.ShowDashboard();

                        }
                        if (this.mView.IsActive())
                        {
                            this.mView.HideProgressDialog();
                        }
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
                NotificationFilterEntity.RemoveAll();
                UserNotificationEntity.RemoveAll();
                SubmittedFeedbackEntity.Remove();
                SMUsageHistoryEntity.RemoveAll();
                UsageHistoryEntity.RemoveAll();
                PromotionsEntityV2 promotionTable = new PromotionsEntityV2();
                promotionTable.DeleteTable();
                PromotionsParentEntityV2 promotionEntityTable = new PromotionsParentEntityV2();
                promotionEntityTable.DeleteTable();
                BillHistoryEntity.RemoveAll();
                PaymentHistoryEntity.RemoveAll();
                REPaymentHistoryEntity.RemoveAll();
                AccountDataEntity.RemoveAll();
                SummaryDashBoardAccountEntity.RemoveAll();
                SelectBillsEntity.RemoveAll();
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
    }
}