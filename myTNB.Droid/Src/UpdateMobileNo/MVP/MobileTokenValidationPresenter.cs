using Android.Content;
using Android.Content.PM;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
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
                //this.mView.ShowEmptyErrorPin_4();
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

            if (request != null)
            {
                this.authenticationRequest = request;
            }

            try
            {
                UpdateNewPhoneNumberRequest updateNewPhoneNumberRequest = new UpdateNewPhoneNumberRequest(oldPhoneNo, newPhone, string.Format("{0}{1}{2}{3}", num1, num2, num3, num4));
                if (this.authenticationRequest != null)
                {
                    updateNewPhoneNumberRequest.SetUserName(this.authenticationRequest.UserName);
                    if (!UserEntity.IsCurrentlyActive())
                    {
                        updateNewPhoneNumberRequest.SetSSPID(this.authenticationRequest.ActiveUserName);
                    }
                }

                var verifyTokenResponse = await ServiceApiImpl.Instance.UpdatePhoneNumber(updateNewPhoneNumberRequest);

                if (mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                }

                if (verifyTokenResponse.IsSuccessResponse())
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
                    this.mView.ShowError(verifyTokenResponse.Response.DisplayMessage);
                    this.mView.ShowEmptyErrorPin();
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

            try
            {
                string oldPhoneNumber = "";

                try
                {
                    UserEntity userEntity = UserEntity.GetActive();
                    oldPhoneNumber = userEntity.MobileNo;
                }
                catch (Exception ex)
                {
                    Utility.LoggingNonFatalError(ex);
                }

                MyTNBService.Request.SendUpdatePhoneTokenSMSRequest sendUpdatePhoneTokenSMSRequest = new MyTNBService.Request.SendUpdatePhoneTokenSMSRequest(newPhoneNumber, oldPhoneNumber);
                if (this.authenticationRequest != null)
                {
                    sendUpdatePhoneTokenSMSRequest.SetUserName(this.authenticationRequest.UserName);
                }
                var verificationResponse = await ServiceApiImpl.Instance.SendUpdatePhoneTokenSMSV2(sendUpdatePhoneTokenSMSRequest);

                if (!verificationResponse.IsSuccessResponse())
                {
                    this.mView.ShowError(verificationResponse.Response.DisplayMessage);
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
            if (mView.IsActive())
            {
                this.mView.ShowRegistrationProgress();
            }

            try
            {
                UserAuthenticateRequest userAuthenticateRequest = new UserAuthenticateRequest(request.ClientType, request.Password);
                userAuthenticateRequest.SetUserName(request.UserName);
                var userResponse = await ServiceApiImpl.Instance.UserAuthenticate(userAuthenticateRequest);

                if (!userResponse.IsSuccessResponse())
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideRegistrationProgress();
                    }
                    this.mView.ShowRetryLoginUnknownException(userResponse.Response.DisplayMessage);
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

                    int Id = UserEntity.InsertOrReplace(userResponse.GetData());
                    if (Id > 0)
                    {
                        string datetime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");

                        GetCustomerAccountListRequest customerAccountListRequest = new GetCustomerAccountListRequest(Convert.ToDateTime(datetime));
                        CustomerAccountListResponse customerAccountListResponse = await ServiceApiImpl.Instance.GetCustomerAccountList(customerAccountListRequest);
                        if (customerAccountListResponse.IsSuccessResponse())
                        {
                            if (customerAccountListResponse.GetData().Count > 0)
                            {
                                ProcessCustomerAccount(customerAccountListResponse.GetData());
                            }
                            else
                            {
                                AccountSortingEntity.RemoveSpecificAccountSorting(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);
                            }
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
                        UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotifications(new MyTNBService.Request.BaseRequest());
                        if(response.IsSuccessResponse())
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
                                    // TODO : SAVE ALL NOTIFICATIONs
                                    int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                }

                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(true);
                            }
                            else
                            {
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                            }
                        }
                        else if(response != null && response.Response != null && response.Response.ErrorCode == "8400")
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
            if (mView.IsActive())
            {
                this.mView.ShowRegistrationProgress();
            }
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var masterDataApi = RestService.For<GetMasterDataApi>(httpClient);

#else
            var masterDataApi = RestService.For<GetMasterDataApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                if (UserEntity.IsCurrentlyActive())
                {
                    UserEntity loggedUser = UserEntity.GetActive();
                    UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotifications(new MyTNBService.Request.BaseRequest());
                    if (response.IsSuccessResponse())
                    {
                        if (response.GetData() != null && response.GetData().UserNotificationList != null &&
                            response.GetData().UserNotificationList.Count > 0)
                        {
                            foreach (UserNotification userNotification in response.GetData().UserNotificationList)
                            {
                                // TODO : SAVE ALL NOTIFICATIONs
                                int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                            }
                        }
                    }

                    var submittedFeedbackResponse = await ServiceApiImpl.Instance.SubmittedFeedbackList(new SubmittedFeedbackListRequest());

                    if (submittedFeedbackResponse.IsSuccessResponse())
                    {
                        SubmittedFeedbackEntity.Remove();
                        foreach (SubmittedFeedbackListResponse.ResponseData responseData in submittedFeedbackResponse.GetData())
                        {
                            SubmittedFeedback sf = new SubmittedFeedback();
                            sf.FeedbackId = responseData.ServiceReqNo;
                            sf.FeedbackCategoryId = responseData.FeedbackCategoryId;
                            sf.DateCreated = responseData.DateCreated;
                            sf.FeedbackMessage = responseData.FeedbackMessage;
                            sf.FeedbackCategoryName = responseData.FeedbackCategoryName;
                            sf.FeedbackNameInListView = responseData.FeedbackNameInListView;
                            SubmittedFeedbackEntity.InsertOrReplace(sf);
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
