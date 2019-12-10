﻿using Android.Content;
using Android.Content.PM;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.AddAccount.Api;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Api;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.MyTNBService.Notification;
using myTNB_Android.Src.UpdateMobileNo.Api;
using myTNB_Android.Src.UpdateMobileNo.Request;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.RegisterValidation.MVP
{
    public class MobileTokenValidationPresenter : MobileTokenValidationContract.IUserActionsListener
    {
        private MobileTokenValidationContract.IView mView;
        private readonly string TAG = typeof(MobileTokenValidationPresenter).Name;
        private CancellationTokenSource cts;
        private ISharedPreferences mSharedPref;

        private UserAuthenticationRequest authenticationRequest;

        public MobileTokenValidationPresenter(MobileTokenValidationContract.IView mView, ISharedPreferences sharedPreferences)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.mSharedPref = sharedPreferences;
        }

        public void OnComplete()
        {
            this.mView.EnableResendButton();
        }

        public void OnNavigateToAccountListActivity()
        {
            this.mView.ShowAccountListActivity();
        }

        public async void OnVerifyToken(string num1, string num2, string num3, string num4, string newPhone, UserAuthenticationRequest request, bool fromAppLaunch, bool verfiyPhone)
        {

            cts = new CancellationTokenSource();
            if (TextUtils.IsEmpty(num1))
            {
                this.mView.ShowEmptyErrorPin_1();
                return;
            }

            if (TextUtils.IsEmpty(num2))
            {
                this.mView.ShowEmptyErrorPin_2();
                return;
            }

            if (TextUtils.IsEmpty(num3))
            {
                this.mView.ShowEmptyErrorPin_3();
                return;
            }

            if (TextUtils.IsEmpty(num4))
            {
                this.mView.ShowEmptyErrorPin_4();
                return;
            }

            if (mView.IsActive())
            {
                this.mView.ShowRegistrationProgress();
            }
            this.mView.DisableResendButton();
            this.mView.ClearErrors();

            string oldPhoneNo = "";
            if (UserEntity.IsCurrentlyActive() && !verfiyPhone)
            {
                UserEntity entity = UserEntity.GetActive();
                oldPhoneNo = entity.MobileNo;
            }

            string user_id = "";
            string user_email = "";
            if (request != null)
            {
                this.authenticationRequest = request;
                user_id = request.ActiveUserName;
                user_email = request.UserName;
            }
            else if (UserEntity.IsCurrentlyActive())
            {
                UserEntity entity = UserEntity.GetActive();
                user_id = entity.UserID;
                user_email = entity.Email;
            }
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var verifyTokenApi = RestService.For<IUpdateMobileNoApi>(httpClient);
#else
            var verifyTokenApi = RestService.For<IUpdateMobileNoApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var verifyTokenResponse = await verifyTokenApi.UpdatePhoneNumberV2(new UpdateMobileNo.Request.UpdateMobileV2Request()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    UserId = user_id,
                    Email = user_email,
                    OldPhoneNumber = oldPhoneNo,
                    NewPhoneNumber = newPhone,
                    token = string.Format("{0}{1}{2}{3}", num1, num2, num3, num4)
                }, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                }

                if (!verifyTokenResponse.Data.IsError)
                {
                    //this.mView.ShowDashboardMyAccount();
                    /// call login service 
                    if (UserEntity.IsCurrentlyActive())
                    {
                        UserEntity.UpdatePhoneNumber(newPhone);
                        if (fromAppLaunch)
                        {
                            this.mView.ShowDashboard();
                        }
                        else
                        {
                            this.mView.ShowUpdatePhoneResultOk();
                        }
                    }
                    else
                    {
                        CallLoginServce(request, newPhone);
                    }
                }
                else
                {
                    // TODO : ADD REGISTRATION ERROR
                    this.mView.ShowError(verifyTokenResponse.Data.Message);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                }
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                this.mView.ShowRetryOptionsUnknownException(e);
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

        public async void ResendAsync(string newPhoneNumber)
        {
            if (this.mView.IsStillCountingDown())
            {
                this.mView.ShowSnackbarError(Resource.String.registration_validation_please_wait);
                return;
            }
            this.mView.DisableResendButton();
            this.mView.StartProgress();

            cts = new CancellationTokenSource();

            string ssp_userid = "";
            string user_name = "";
            string user_email = "";

            if (this.authenticationRequest != null)
            {
                ssp_userid = authenticationRequest.ActiveUserName;
                user_name = authenticationRequest.UserName;
                user_email = authenticationRequest.UserName;
            }
            else if (UserEntity.IsCurrentlyActive())
            {
                UserEntity entity = UserEntity.GetActive();
                ssp_userid = entity.UserID;
                user_name = entity.UserName;
                user_email = entity.Email;
            }

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var updateMobileApi = RestService.For<ISendUpdatePhoneTokenSMSApi>(httpClient);
#else
            var updateMobileApi = RestService.For<ISendUpdatePhoneTokenSMSApi>(Constants.SERVER_URL.END_POINT);
#endif


            try
            {
                var verificationResponse = await updateMobileApi.SendUpdatePhoneTokenSMS(new SendUpdatePhoneTokenSMSRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    IpAddress = Constants.APP_CONFIG.API_KEY_ID,
                    ClientType = Constants.APP_CONFIG.API_KEY_ID,
                    ActiveUserName = Constants.APP_CONFIG.API_KEY_ID,
                    DevicePlatform = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceVersion = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceCordova = Constants.APP_CONFIG.API_KEY_ID,
                    sspUserId = ssp_userid,
                    username = user_name,
                    userEmail = user_email,
                    mobileNo = newPhoneNumber
                }
                , cts.Token);

                if (verificationResponse.Data.IsError)
                {
                    this.mView.ShowError(verificationResponse.Data.Message);
                }


            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public void Start()
        {
            try
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

                this.mView.DisableResendButton();
                this.mView.StartProgress();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void CallLoginServce(UserAuthenticationRequest request, string newPhone)
        {
            cts = new CancellationTokenSource();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

            if (mView.IsActive())
            {
                this.mView.ShowRegistrationProgress();
            }
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IAuthenticateUser>(httpClient);

            var notificationsApi = RestService.For<INotificationApi>(httpClient);

#else
            var api = RestService.For<IAuthenticateUser>(Constants.SERVER_URL.END_POINT);
            var notificationsApi = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);

#endif

            try
            {
                var userResponse = await api.DoLogin(request, cts.Token);

                if (userResponse.Data.IsError || userResponse.Data.Status.Equals("failed"))
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideRegistrationProgress();
                    }
                    this.mView.ShowRetryLoginUnknownException(userResponse.Data.Message);
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
                    int Id = UserEntity.InsertOrReplace(userResponse.Data.User);
                    if (Id > 0)
                    {


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
                        if (customerAccountsResponse != null && customerAccountsResponse.D != null && customerAccountsResponse.D.ErrorCode == "7200")
                        {
                            if (customerAccountsResponse.D.AccountListData.Count > 0)
                            {
                                ProcessCustomerAccount(customerAccountsResponse.D.AccountListData);
                            }
                            else
                            {
                                AccountSortingEntity.RemoveSpecificAccountSorting(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);
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
                        }

                        if (mView.IsActive())
                        {
                            this.mView.HideRegistrationProgress();
                        }

                        if (UserEntity.IsCurrentlyActive())
                        {
                            UserEntity.UpdatePhoneNumber(newPhone);
                        }
                        UserSessions.SavePhoneVerified(mSharedPref, true);
                        this.mView.ShowDashboardMyAccount();
                    }
                    else
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideRegistrationProgress();
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
                    this.mView.HideRegistrationProgress();
                    this.mView.ShowRetryLoginUnknownException(e.StackTrace);
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                    this.mView.ShowRetryLoginUnknownException(apiException.StackTrace);
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                if (this.mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                    this.mView.ShowRetryLoginUnknownException(e.StackTrace);
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void ShowDashboard()
        {
            cts = new CancellationTokenSource();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

            if (mView.IsActive())
            {
                this.mView.ShowRegistrationProgress();
            }
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<INotificationApi>(httpClient);
            //var weblinkApi = RestService.For<IWeblinksApi>(httpClient);
            //var locationTypesApi = RestService.For<GetLocationTypseApi>(httpClient);
            var feedbackApi = RestService.For<IFeedbackApi>(httpClient);

            var masterDataApi = RestService.For<GetMasterDataApi>(httpClient);

#else
            var api = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
            //var weblinkApi = RestService.For<IWeblinksApi>(Constants.SERVER_URL.END_POINT);
            //var locationTypesApi = RestService.For<GetLocationTypseApi>(Constants.SERVER_URL.END_POINT);
            var feedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);

            var masterDataApi = RestService.For<GetMasterDataApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                if (UserEntity.IsCurrentlyActive())
                {
                    UserEntity loggedUser = UserEntity.GetActive();

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
                    }

                    var submittedFeedbackResponse = await feedbackApi.GetSubmittedFeedbackList(new Base.Request.SubmittedFeedbackRequest()
                    {
                        ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                        Email = loggedUser.Email,
                        DeviceId = this.mView.GetDeviceId()

                    }, cts.Token);

                    if (!submittedFeedbackResponse.Data.IsError)
                    {
                        SubmittedFeedbackEntity.Remove();
                        foreach (SubmittedFeedback sFeed in submittedFeedbackResponse.Data.Data)
                        {
                            int newRecord = SubmittedFeedbackEntity.InsertOrReplace(sFeed);
                            Console.WriteLine(string.Format("SubmitFeedback Id = {0}", newRecord));
                        }
                    }
                    UserSessions.SavePhoneVerified(mSharedPref, true);
                    if (mView.IsActive())
                    {
                        this.mView.HideRegistrationProgress();
                    }
                    this.mView.ShowDashboard();

                }
                else
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideRegistrationProgress();
                    }
                }

            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                    this.mView.ShowRetryLoginUnknownException(e.StackTrace);
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                    this.mView.ShowRetryLoginUnknownException(apiException.StackTrace);
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                if (this.mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                    this.mView.ShowRetryLoginUnknownException(e.StackTrace);
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ProcessCustomerAccount(List<Account> list)
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

                    foreach (Account acc in list)
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

                            Account newAcc = list.Find(x => x.AccountNumber == oldAcc.AccNum);

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
                    foreach (Account acc in list)
                    {
                        int rowChange = CustomerBillingAccount.InsertOrReplace(acc, false);
                    }
                    if (CustomerBillingAccount.HasItems())
                    {
                        CustomerBillingAccount.RemoveSelected();
                        CustomerBillingAccount.MakeFirstAsSelected();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}