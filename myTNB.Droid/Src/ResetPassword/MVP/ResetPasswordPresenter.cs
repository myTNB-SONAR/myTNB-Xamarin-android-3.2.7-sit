using Android.Content;
using Android.Text;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
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

            try
            {
                ChangeNewPasswordRequest changePasswordRequest = new ChangeNewPasswordRequest(oldPassword, newPassword, newPassword);
                changePasswordRequest.SetUserName(username);
                var changePasswordResponse = await ServiceApiImpl.Instance.ChangeNewPassword(changePasswordRequest);

                if (!changePasswordResponse.IsSuccessResponse())
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }
                    string message = changePasswordResponse.Response.DisplayMessage;
                    this.mView.ShowErrorMessage(message);
                }
                else
                {
                    UserSessions.DoUnflagResetPassword(mSharedPref);

                    UserAuthenticateRequest userAuthenticateRequest = new UserAuthenticateRequest(DeviceIdUtils.GetAppVersionName(), newPassword);
                    userAuthenticateRequest.SetUserName(username);
                    var userResponse = await ServiceApiImpl.Instance.UserAuthenticate(userAuthenticateRequest);

                    if (userResponse.IsSuccessResponse())
                    {

                        mView.ClearTextFields();
                        int Id = UserEntity.InsertOrReplace(userResponse.GetData());
                        if (Id > 0)
                        {
                            GetAcccountsV4Request baseRequest = new GetAcccountsV4Request();
                            baseRequest.SetSesParam1(UserEntity.GetActive().DisplayName);
                            baseRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                            string dt = JsonConvert.SerializeObject(baseRequest);
                            CustomerAccountListResponseAppLaunch customerAccountListResponse = await ServiceApiImpl.Instance.GetCustomerAccountListAppLaunch(baseRequest);
                            if (customerAccountListResponse != null && customerAccountListResponse.customerAccountData != null)
                            {
                                //if (customerAccountListResponse.GetData().Count > 0)
                                if (customerAccountListResponse.customerAccountData.Count > 0)
                                {
                                    
                                    ProcessCustomerAccount(customerAccountListResponse.customerAccountData);
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
                            UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotificationsV2(new BaseRequest());
                            if (response.IsSuccessResponse())
                            {
                                try
                                {
                                    UserNotificationEntity.RemoveAll();
                                }
                                catch (System.Exception ne)
                                {
                                    Utility.LoggingNonFatalError(ne);
                                }

                                if (response.GetData() != null && response.GetData().UserNotificationList != null &&
                                    response.GetData().UserNotificationList.Count > 0)
                                {
                                    foreach (UserNotification userNotification in response.GetData().UserNotificationList)
                                    {
                                        // tODO : SAVE ALL NOTIFICATIONs
                                        int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                    }
                                }
                            }

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
                                WhatsNewParentEntity mWhatsNewParentEntity = new WhatsNewParentEntity();
                                mWhatsNewParentEntity.DeleteTable();
                                mWhatsNewParentEntity.CreateTable();
                                WhatsNewCategoryEntity mWhatsNewCategoryEntity = new WhatsNewCategoryEntity();
                                mWhatsNewCategoryEntity.DeleteTable();
                                mWhatsNewCategoryEntity.CreateTable();
                                WhatsNewEntity mWhatsNewEntity = new WhatsNewEntity();
                                mWhatsNewEntity.DeleteTable();
                                mWhatsNewEntity.CreateTable();
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
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
                        this.mView.ShowErrorMessage(userResponse.Response.DisplayMessage);
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
                           // Type = acc.Type,
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
                            IsSelected = false,
                            IsInManageAccessList = acc.IsInManageAccessList,
                            CreatedBy = acc.CreatedBy
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
                                AccountTypeId = newAcc.AccountTypeId,
                                AccountStAddress = newAcc.AccountStAddress,
                                OwnerName = newAcc.OwnerName,
                                AccountCategoryId = newAcc.AccountCategoryId,
                                SmartMeterCode = newAcc.SmartMeterCode == null ? "0" : newAcc.SmartMeterCode,
                                IsSelected = false,
                                IsInManageAccessList = newAcc.IsInManageAccessList,
                                CreatedBy = newAcc.CreatedBy
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
