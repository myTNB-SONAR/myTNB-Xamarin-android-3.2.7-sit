using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Billing;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Notification;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.SSMRTerminate.Api;
using myTNB_Android.Src.Utils;
using Refit;
using static myTNB_Android.Src.AppLaunch.Models.UserNotificationResponse;
using static myTNB_Android.Src.MyTNBService.Response.AccountChargesResponse;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class UserNotificationDetailPresenter
    {
        UserNotificationDetailContract.IView mView;
        public NotificationDetailModel notificationDetailModel;
        List<NotificationDetailModel.NotificationCTA> ctaList;
        BillingApiImpl api;
        SSMRTerminateImpl terminationApi;
        AccountData mSelectedAccountData;
        private static readonly Regex accountNamePattern = new Regex("#(.+?)#", RegexOptions.Compiled);

        public UserNotificationDetailPresenter(UserNotificationDetailContract.IView view)
        {
            mView = view;
            api = new BillingApiImpl();
            terminationApi = new SSMRTerminateImpl();
        }

        public void EvaluateDetail(Models.NotificationDetails notificationDetails)
        {
            try
            {
                NotificationDetailModel.NotificationCTA primaryCTA;
                NotificationDetailModel.NotificationCTA secondaryCTA;
                int imageResourceBanner = 0;
                string pageTitle = "Notification";
                string notificationDetailTitle = notificationDetails.Title;
                string notificationDetailMessage = notificationDetails.Message;
                
                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
                string accountName = (customerBillingAccount != null) ? customerBillingAccount.AccDesc : "";
                ctaList = new List<NotificationDetailModel.NotificationCTA>();

                switch (notificationDetails.BCRMNotificationTypeId)
                {
                    case Constants.BCRM_NOTIFICATION_NEW_BILL_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            pageTitle = "New Bill";
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            //notificationDetailTitle = "Your May 2019 Bill Is Ready";
                            //notificationDetailMessage = "Your bill is RM 234.25. Got a minute? Make a quick and easy payment on the myTNB app now. <br/><br/>" +
                            //    "Account: #accountName#";
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_BILL_DUE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_bill_due_banner;
                            pageTitle = "Bill Due";
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            //notificationDetailTitle = "Your Apr 2019 Bill Is Due";
                            //notificationDetailMessage = "Hi, Mohd Zulkifli! On 15 May, your Apr 2019 TNB bill amounting to RM 76.65 will be due. <br/><br/>" +
                            //    "No time to queue at TNB? No problem!Pay now on the myTNB app.Please disregard if paid.<br/><br/>" +
                            //    "Account: #accountName#";
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_disconnect_notice_banner;
                            pageTitle = "Disconnection Notice";
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            //notificationDetailTitle = "Your Supply May Be Disconnected";
                            //notificationDetailMessage = "Urgent notice, Mohd Zulkifli. Your electricity supply may be disconnected between 22 and 31 May if you haven't already paid the bill. No worries, just pay before 21 May on the myTNB app and all will be well! <br/><br/> " +
                            //    "Account: #name#<br/><br/>" +
                            //    "PS: Alternatively, you can pay via <a href=\"faqid={B8EBBADE-0918-43B7-8093-BB2B19614033}\">other methods</a> too but it’s quicker and easier on the app!​";
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DISCONNECTED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_disconnected_banner;
                            pageTitle = "Disconnection";
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Contact TNB", delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            //notificationDetailTitle = "Your Supply Has Been Disconnected";
                            //notificationDetailMessage = "Don't panic, you can make a full payment on the myTNB app and you'll see the light again!<br/><br/>" +
                            //    "Account: #name#<br/><br/>" +
                            //    "PS: Alternatively, you can pay via other methods too but it’s quicker and easier on the app!​";
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_RECONNECTED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_reconnected_banner;
                            pageTitle = "Reconnection";
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View My Usage", delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            //notificationDetailTitle = "Your Supply Has Been Reconnected";
                            //notificationDetailMessage = "Hooray, the lights are back on! Your account has been reconnected. Stay on top of your monthly payments and your usage with the myTNB app.<br/><br/>" +
                            //    "Account: #name#";
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_MAINTENANCE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_maintenance_banner;
                            pageTitle = "Maintenance";
                            //notificationDetailTitle = "Down For Maintenance from 4PM to 8PM on 31 Aug 2019";
                            //notificationDetailMessage = "Don't worry, we'll be up and running quickly and better than before! We apologize for any inconvenience.";
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_METER_READING_OPEN_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            pageTitle = "Smart Meter Reading";
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Submit Meter Reading", delegate () { SubmitMeterReading(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            ctaList.Add(primaryCTA);

                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_METER_READING_REMIND_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Submit Meter Reading", delegate () { SubmitMeterReading(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            ctaList.Add(primaryCTA);
                            pageTitle = "Smart Meter Reading";
                            imageResourceBanner = Resource.Drawable.notification_smr_generic_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Re-enable Self Meter Reading", delegate () { EnableSelfMeterReading(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            pageTitle = "Smart Meter Reading";
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_APPLY_SUCCESS_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Usage", delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            pageTitle = "Smart Meter Reading";
                            imageResourceBanner = Resource.Drawable.notification_smr_generic_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_APPLY_FAILED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Contact TNB", delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);
                            pageTitle = "Smart Meter Reading";
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_smr_generic_banner;
                            pageTitle = "Smart Meter Reading";
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Re-enable Self Meter Reading", delegate () { EnableSelfMeterReading(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_FAILED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            pageTitle = "Smart Meter Reading";
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Contact TNB", delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    default:
                        imageResourceBanner = Resource.Drawable.notification_generic_banner;
                        break;
                }
                notificationDetailMessage = Utility.ReplaceValueByPattern(accountNamePattern, notificationDetailMessage, accountName);
                notificationDetailModel = new NotificationDetailModel(imageResourceBanner, pageTitle, notificationDetailTitle,
                    notificationDetailMessage, ctaList);
            }
            catch (Exception e)
            {
                this.mView.ShowRetryOptionsApiException(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void DeleteNotificationDetail(Models.NotificationDetails notificationDetails)
        {
            this.mView.ShowLoadingScreen();
            try
            {
                NotificationApiImpl notificationAPI = new NotificationApiImpl();
                List<Notifications.Models.UserNotificationData> selectedNotificationList = new List<Notifications.Models.UserNotificationData>();
                Notifications.Models.UserNotificationData data = new Notifications.Models.UserNotificationData();
                data.Id = notificationDetails.Id;
                data.NotificationType = notificationDetails.NotificationType;
                selectedNotificationList.Add(data);
                UserNotificationDeleteResponse notificationDeleteResponse = await notificationAPI.DeleteUserNotification<UserNotificationDeleteResponse>(new UserNotificationDeleteRequest(selectedNotificationList));
                if (notificationDeleteResponse.Data.ErrorCode == "7200")
                {
                    UserNotificationEntity.UpdateIsDeleted(notificationDetails.Id, true);
                    this.mView.ShowNotificationListAsDeleted();
                }
                else
                {
                    this.mView.ShowRetryOptionsCancelledException(null);
                }

            }
            catch (System.OperationCanceledException e)
            {
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
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void ViewBillDetails(Models.NotificationDetails notificationDetails)
        {
            this.mView.ShowLoadingScreen();
            try
            {
                List<string> accountList = new List<string>();
                List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
                accountList.Add(notificationDetails.AccountNum);
                AccountsChargesRequest accountChargeseRequest = new AccountsChargesRequest(
                        accountList,
                        true
                        );
                AccountChargesResponse accountChargeseResponse = await api.GetAccountsCharges<AccountChargesResponse>(accountChargeseRequest);
                this.mView.HideLoadingScreen();
                if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "7200")
                {
                    AccountData accountData = new AccountData();
                    accountChargeModelList = GetAccountChargeModelList(accountChargeseResponse.Data.ResponseData.AccountCharges);
                    CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(accountChargeseResponse.Data.ResponseData.AccountCharges[0].ContractAccount);
                    if (account != null)
                    {
                        accountData.AccountNum = account.AccNum;
                        accountData.AccountNickName = account.AccDesc;
                        accountData.AddStreet = account.AccountStAddress;
                        mView.ViewDetails(accountData, accountChargeModelList[0]);
                    }
                    else
                    {
                        this.mView.ShowRetryOptionsApiException(null);
                    }
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
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
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void PayNow(Models.NotificationDetails notificationDetails)
        {
            AccountData accountData = new AccountData();
            CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
            if (account != null)
            {
                accountData.AccountNum = account.AccNum;
                accountData.AccountNickName = account.AccDesc;
                accountData.AddStreet = account.AccountStAddress;
                this.mView.PayNow(accountData);
            }
            else
            {
                this.mView.ShowRetryOptionsApiException(null);
            }
        }

        private void CallUs()
        {
            if (WeblinkEntity.HasRecord("TNBCLE"))
            {
                this.mView.ContactUs(WeblinkEntity.GetByCode("TNBCLE"));
            }
        }

        private void ViewMyUsage(Models.NotificationDetails notificationDetails)
        {
            AccountData accountData = new AccountData();
            CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
            CustomerBillingAccount.RemoveSelected();
            CustomerBillingAccount.SetSelected(notificationDetails.AccountNum);

            if (account != null)
            {
                accountData.AccountNum = account.AccNum;
                accountData.AccountNickName = account.AccDesc;
                accountData.AddStreet = account.AccountStAddress;
                this.mView.ViewUsage(accountData);
            }
            else
            {
                this.mView.ShowRetryOptionsApiException(null);
            }
        }

        private async void SubmitMeterReading(Models.NotificationDetails notificationDetails)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = "",
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var ssmrAccountAPI = RestService.For<ISMRAccountActivityInfoApi>(httpClient);

#else
            var ssmrAccountAPI = RestService.For<ISMRAccountActivityInfoApi>(Constants.SERVER_URL.END_POINT);
#endif 

                SMRActivityInfoResponse SMRAccountActivityInfoResponse = await ssmrAccountAPI.GetSMRAccountActivityInfo(new myTNB_Android.Src.myTNBMenu.Requests.SMRAccountActivityInfoRequest()
                {
                    AccountNumber = customerBillingAccount.AccNum,
                    IsOwnedAccount = customerBillingAccount.isOwned ? "true" : "false",
                    userInterface = currentUsrInf
                }, cts.Token);


                if (SMRAccountActivityInfoResponse != null && SMRAccountActivityInfoResponse.Response != null && SMRAccountActivityInfoResponse.Response.ErrorCode == "7200")
                {
                    SMRPopUpUtils.OnSetSMRActivityInfoResponse(SMRAccountActivityInfoResponse);
                    AccountData accountData = new AccountData();
                    accountData.AccountNum = notificationDetails.AccountNum;
                    this.mView.SubmitMeterReading(accountData, SMRAccountActivityInfoResponse);
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
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
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void EnableSelfMeterReading(Models.NotificationDetails notificationDetails)
        {
            try
            {
                CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = "",
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                CARegisteredContactInfoResponse response = await this.terminationApi.GetCARegisteredContactInfo(new GetRegisteredContactInfoRequest()
                {
                    AccountNumber = notificationDetails.AccountNum,
                    IsOwnedAccount = "true",
                    ICNumber = account.ICNum,
                    usrInf = currentUsrInf
                });

                if (response.Data.ErrorCode == "7200")
                {
                    CAContactDetailsModel contactDetailsModel = new CAContactDetailsModel();
                    contactDetailsModel.email = response.Data.AccountDetailsData.Email;
                    contactDetailsModel.mobile = response.Data.AccountDetailsData.Mobile;
                    contactDetailsModel.isAllowEdit = response.Data.AccountDetailsData.isAllowEdit;

                    AccountData accountData = new AccountData();
                    accountData.AccountNum = notificationDetails.AccountNum;
                    accountData.AddStreet = account.AccountStAddress;
                    accountData.AccountNickName = account.AccDesc;

                    this.mView.EnableSelfMeterReading(accountData, contactDetailsModel);
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
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
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        private List<AccountChargeModel> GetAccountChargeModelList(List<AccountCharge> accountCharges)
        {
            List<AccountChargeModel> accountChargeModelList = new List<AccountChargeModel>();
            accountCharges.ForEach(accountCharge =>
            {
                MandatoryCharge mandatoryCharge = accountCharge.MandatoryCharges;
                List<ChargeModel> chargeModelList = new List<ChargeModel>();
                mandatoryCharge.Charges.ForEach(charge =>
                {
                    ChargeModel chargeModel = new ChargeModel();
                    chargeModel.Key = charge.Key;
                    chargeModel.Title = charge.Title;
                    chargeModel.Amount = charge.Amount;
                    chargeModelList.Add(chargeModel);
                });
                MandatoryChargeModel mandatoryChargeModel = new MandatoryChargeModel();
                mandatoryChargeModel.TotalAmount = mandatoryCharge.TotalAmount;
                mandatoryChargeModel.ChargeModelList = chargeModelList;

                AccountChargeModel accountChargeModel = new AccountChargeModel();
                accountChargeModel.IsCleared = false;
                accountChargeModel.IsNeedPay = false;
                accountChargeModel.IsPaidExtra = false;
                accountChargeModel.ContractAccount = accountCharge.ContractAccount;
                accountChargeModel.CurrentCharges = accountCharge.CurrentCharges;
                accountChargeModel.OutstandingCharges = accountCharge.OutstandingCharges;
                accountChargeModel.AmountDue = accountCharge.AmountDue;
                accountChargeModel.DueDate = accountCharge.DueDate;
                accountChargeModel.BillDate = accountCharge.BillDate;
                accountChargeModel.IncrementREDueDateByDays = accountCharge.IncrementREDueDateByDays;
                accountChargeModel.MandatoryCharges = mandatoryChargeModel;
                EvaluateAmountDue(accountChargeModel);
                accountChargeModelList.Add(accountChargeModel);
            });
            return accountChargeModelList;
        }

        private void EvaluateAmountDue(AccountChargeModel accountChargeModel)
        {
            if (accountChargeModel.AmountDue > 0f)
            {
                accountChargeModel.IsNeedPay = true;
            }

            if (accountChargeModel.AmountDue < 0f)
            {
                accountChargeModel.IsPaidExtra = true;
            }

            if (accountChargeModel.AmountDue == 0f)
            {
                accountChargeModel.IsCleared = true;
            }
        }

        public NotificationDetailModel GetNotificationDetailModel()
        {
            return notificationDetailModel;
        }
    }
}
