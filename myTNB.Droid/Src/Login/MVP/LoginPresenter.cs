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
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.MyTNBService.Notification;
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
                UserAuthenticateRequest userAuthRequest = new UserAuthenticateRequest(DeviceIdUtils.GetAppVersionName(), pwd);
                userAuthRequest.SetUserName(usrNme);
                var userResponse = await ServiceApiImpl.Instance.UserAuthenticate(userAuthRequest);

                if (!userResponse.IsSuccessResponse())
                {
                    if (this.mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                        this.mView.ShowInvalidEmailPasswordError(userResponse.Response.DisplayMessage);
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
                        if (Id > 0)
                        {
                            UserEntity.UpdateDeviceId(deviceId);

                            CustomerAccountListResponse customerAccountListResponse = await ServiceApiImpl.Instance.GetCustomerAccountList(new BaseRequest());
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
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(false);
                                NotificationApiImpl notificationAPI = new NotificationApiImpl();
                                MyTNBService.Response.UserNotificationResponse response = await notificationAPI.GetUserNotifications<MyTNBService.Response.UserNotificationResponse>(new Base.Request.APIBaseRequest());
                                if(response != null && response.Data != null && response.Data.ErrorCode == "7200")
                                {
                                    if (response.Data.ResponseData != null && response.Data.ResponseData.UserNotificationList != null)
                                    {
                                        try
                                        {
                                            UserNotificationEntity.RemoveAll();
                                        }
                                        catch (System.Exception ne)
                                        {
                                            Utility.LoggingNonFatalError(ne);
                                        }

                                        foreach (UserNotification userNotification in response.Data.ResponseData.UserNotificationList)
                                        {
                                            // tODO : SAVE ALL NOTIFICATIONs
                                            int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                        }
                                    }
                                    else
                                    {
                                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                    }
                                }
                                else
                                {
                                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
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
                                            string json = getItemsService.GetPromotionsV2TimestampItem();
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
                                await LanguageUtil.CheckUpdatedLanguage();
                                this.mView.ShowDashboard();
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
                            IsSelected = false
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

                        foreach(int index in newExisitingListArray)
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
                                IsSelected = false
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
