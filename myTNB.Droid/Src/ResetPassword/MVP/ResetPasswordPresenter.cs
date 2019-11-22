using Android.Content;
using Android.Text;
using myTNB_Android.Src.AddAccount.Api;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Api;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.MyTNBService.Notification;
using myTNB_Android.Src.ResetPassword.Api;
using myTNB_Android.Src.ResetPassword.Request;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace myTNB_Android.Src.ResetPassword.MVP
{
    public class ResetPasswordPresenter : ResetPasswordContract.IUserActionsListener
    {
        private ResetPasswordContract.IView mView;
        private ISharedPreferences mSharedPref;

        //private readonly string MIN_6_ALPHANUMERIC_PATTERN = "[a-zA-Z0-9#?!@$%^&*-]{6,}";
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[a-zA-Z]+");
        private Regex hasMinimum8Chars = new Regex(@".{8,}");

        CancellationTokenSource cts;

        public ResetPasswordPresenter(ResetPasswordContract.IView mView, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mSharedPref = mSharedPref;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {
            // NO IMPL
        }

        public async void Submit(string apiKeyId, string newPassword, string confirmNewPassword, string oldPassword, string username, string deviceId)
        {
            cts = new CancellationTokenSource();

            if (TextUtils.IsEmpty(newPassword))
            {
                this.mView.ShowEmptyNewPasswordError();
                return;
            }

            if (!CheckPasswordIsValid(newPassword))
            {
                this.mView.ShowPasswordMinimumOf6CharactersError();
                return;
            }


            if (TextUtils.IsEmpty(confirmNewPassword))
            {
                this.mView.ShowEmptyConfirmNewPasswordError();
                return;
            }

            if (!newPassword.Equals(confirmNewPassword))
            {
                this.mView.ShowNotEqualConfirmNewPasswordToNewPasswordError();
                return;
            }

            this.mView.DisableSubmitButton();

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var resetPasswordApi = RestService.For<IResetPassword>(httpClient);
            var loginApi = RestService.For<IAuthenticateUser>(httpClient);

#else
            var resetPasswordApi = RestService.For<IResetPassword>(Constants.SERVER_URL.END_POINT);
            var loginApi = RestService.For<IAuthenticateUser>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var changePasswordResponse = await resetPasswordApi.ChangeNewPassword(new ResetPasswordRequest(apiKeyId,
                                                                           username,
                                                                           oldPassword,
                                                                           newPassword,
                                                                           confirmNewPassword,
                                                                           apiKeyId,
                                                                           apiKeyId,
                                                                           apiKeyId,
                                                                           apiKeyId,
                                                                           apiKeyId,
                                                                           apiKeyId), cts.Token);

                if (changePasswordResponse.Data.IsError)
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }
                    string message = changePasswordResponse.Data.Message;
                    this.mView.ShowErrorMessage(message);
                }
                else
                {
                    UserSessions.DoUnflagResetPassword(mSharedPref);
                    string fcmToken = "";
                    if (FirebaseTokenEntity.HasLatest())
                    {
                        fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
                    }
                    var userResponse = await loginApi.DoLogin(new UserAuthenticationRequest(Constants.APP_CONFIG.API_KEY_ID)
                    {
                        UserName = username,
                        Password = newPassword,
                        IpAddress = Constants.APP_CONFIG.API_KEY_ID,
                        ClientType = DeviceIdUtils.GetAppVersionName(),
                        ActiveUserName = Constants.APP_CONFIG.API_KEY_ID,
                        DevicePlatform = Constants.DEVICE_PLATFORM,
                        DeviceVersion = DeviceIdUtils.GetAndroidVersion(),
                        DeviceCordova = Constants.APP_CONFIG.API_KEY_ID,
                        DeviceId = deviceId,
                        FcmToken = fcmToken



                    }, cts.Token);



                    if (!userResponse.Data.IsError && userResponse.Data.Status.Equals("success"))
                    {

                        mView.ClearTextFields();
                        int Id = UserEntity.InsertOrReplace(userResponse.Data.User);
                        if (Id > 0)
                        {


                            //#if STUB
                            //                        var customerAccountsApi = Substitute.For<GetCustomerAccounts>();
                            //                        customerAccountsApi.GetCustomerAccountV5(new AddAccount.Requests.GetCustomerAccountsRequest(Constants.APP_CONFIG.API_KEY_ID, userResponse.Data.User.UserId))
                            //                            .ReturnsForAnyArgs(Task.Run<AccountResponseV5>(
                            //                                    () => JsonConvert.DeserializeObject<AccountResponseV5>(this.mView.GetCustomerAccountsStubV5())
                            //                                ));

#if DEBUG || STUB
                            var newHttpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                            var customerAccountsApi = RestService.For<GetCustomerAccounts>(newHttpClient);
                            var notificationsApi = RestService.For<INotificationApi>(httpClient);
#else
                            var customerAccountsApi = RestService.For<GetCustomerAccounts>(Constants.SERVER_URL.END_POINT);
                            var notificationsApi = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
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
                                this.mView.HideProgressDialog();
                            }

                            this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                            this.mView.ShowResetPasswordSuccess();

                        }
                        else
                        {
                            if (mView.IsActive())
                            {
                                this.mView.HideProgressDialog();
                            }
                        }

                    }
                    else
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideProgressDialog();
                        }
                        this.mView.ShowErrorMessage(userResponse.Data.Message);
                    }

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



            this.mView.EnableSubmitButton();

        }

        public bool CheckPasswordIsValid(string password)
        {

            bool isValid = false;
            try
            {
                isValid = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return isValid;
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
                    CustomerBillingAccount.RemoveSelected();
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