using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Text;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.EnergyBudgetRating.Request;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Service;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.NotificationDetails.Activity;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMRTerminate.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using static myTNB_Android.Src.MyTNBService.Response.AccountChargesResponse;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class UserNotificationDetailPresenter
    {
        UserNotificationDetailContract.IView mView;
        public NotificationDetailModel notificationDetailModel;
        List<NotificationDetailModel.NotificationCTA> ctaList;
        SSMRTerminateImpl terminationApi;
        AccountData mSelectedAccountData;
        private ISharedPreferences mSharedPref;
        bool isTaggedSMR = true;
        bool isSixHaveQuestion = false;
        bool isSevenHaveQuestion = false;
        private Android.App.Activity mActivity;

        public UserNotificationDetailPresenter(UserNotificationDetailContract.IView view, ISharedPreferences mSharedPref)
        {
            mView = view;
            this.mSharedPref = mSharedPref;
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
                string accountName = MyTNBAccountManagement.GetInstance().GetNotificationAccountName(notificationDetails.AccountNum);

                ctaList = new List<NotificationDetailModel.NotificationCTA>();

                switch (notificationDetails.BCRMNotificationTypeId)
                {
                    case Constants.BCRM_NOTIFICATION_NEW_BILL_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            //pageTitle = "New Bill";
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewBill"),
                                delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "payNow"), delegate () { PayNow(notificationDetails); });
                            secondaryCTA.SetEnabled(Utility.IsEnablePayment());
                            ctaList.Add(secondaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_BILL_DUE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_bill_due_banner;
                            //pageTitle = "Bill Due";
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewBill"),
                                delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "payNow"),
                                delegate () { PayNow(notificationDetails); });
                            secondaryCTA.SetEnabled(Utility.IsEnablePayment());
                            ctaList.Add(secondaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_disconnect_notice_banner;
                            //pageTitle = "Disconnection Notice";
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewBill"),
                                delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "payNow"),
                                delegate () { PayNow(notificationDetails); });
                            secondaryCTA.SetEnabled(Utility.IsEnablePayment());
                            ctaList.Add(secondaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DISCONNECTED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_disconnected_banner;
                            //pageTitle = "Disconnection";
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "contactTNB"),
                                delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "payNow"),
                                delegate () { PayNow(notificationDetails); });
                            secondaryCTA.SetEnabled(Utility.IsEnablePayment());
                            ctaList.Add(secondaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_RECONNECTED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_reconnected_banner;
                            //pageTitle = "Reconnection";
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewMyUsage"),
                                delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_MAINTENANCE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_maintenance_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_METER_READING_OPEN_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_smr_check_banner;
                            //pageTitle = "Smart Meter Reading";
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "submitMeterReading"),
                                delegate () { SubmitMeterReading(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            primaryCTA.SetEnabled(notificationDetails.IsSMRPeriodOpen);
                            ctaList.Add(primaryCTA);

                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_METER_READING_REMIND_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "submitMeterReading"),
                                delegate () { SubmitMeterReading(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            primaryCTA.SetEnabled(notificationDetails.IsSMRPeriodOpen);
                            ctaList.Add(primaryCTA);
                            imageResourceBanner = Resource.Drawable.notification_smr_check_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "submitMeterReading"),
                                delegate () { SubmitMeterReading(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            primaryCTA.SetEnabled(notificationDetails.IsSMRPeriodOpen);
                            ctaList.Add(primaryCTA);
                            imageResourceBanner = Resource.Drawable.notification_smr_check_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_APPLY_SUCCESS_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewMyUsage"),
                                delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            imageResourceBanner = Resource.Drawable.notification_smr_generic_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_APPLY_FAILED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "contactTNB"),
                                delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);
                            imageResourceBanner = Resource.Drawable.notification_smr_fail_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID:
                        {
                            Task.Run(async () => await GetSMRAccountStatus(notificationDetails.AccountNum)).Wait();
                            imageResourceBanner = Resource.Drawable.notification_smr_generic_banner;
                            //pageTitle = "Smart Meter Reading";
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "reenableSSMR"), delegate () { EnableSelfMeterReading(notificationDetails); });
                            primaryCTA.SetEnabled(!isTaggedSMR);
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_FAILED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_smr_fail_banner;
                            //pageTitle = "Smart Meter Reading";
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "contactTNB"), delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_PAYMENT_FAILED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_payment_failed_banner;
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedCommonLabel("tryAgain"),
                                delegate () { ShowSelectBill(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_PAYMENT_SUCCESS_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_payment_success_banner;
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "paymentHistory"),
                                delegate () { ViewBillHistory(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            if (notificationDetails.MerchantTransId != null)
                            {
                                secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewReceipt"),
                                delegate () { ShowPaymentReceipt(notificationDetails); });
                                ctaList.Add(secondaryCTA);
                            }
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_NEW_ACCOUNT_ADDED:
                        {

                            imageResourceBanner = Resource.Drawable.noti_nonowner_to_owner;
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewManageAccess"),
                                delegate () { ViewManageAccess(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            ctaList.Add(primaryCTA);

                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_REMOVE_ACCESS:
                        {

                            imageResourceBanner = Resource.Drawable.noti_removed_by_owner;

                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_NEW_ACCESS_ADDED:
                        {

                            imageResourceBanner = Resource.Drawable.noti_access_changed_by_owner;
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewMyUsage"),
                               delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            
                            secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "addNickname"),
                            delegate () { ViewManageAccess(notificationDetails); });
                            ctaList.Add(secondaryCTA);
                            

                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_UPDATE_ACCESS:
                        {

                            imageResourceBanner = Resource.Drawable.noti_access_changed_by_owner;
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewAccount"),
                                delegate () { ViewManageAccess(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            ctaList.Add(primaryCTA);
                            break;
                        }

                    case Constants.BCRM_NOTIFICATION_DBR_PAPER:
                        {
                            imageResourceBanner = Resource.Drawable.notification_dbr_banner_paper;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DBR_EMAIL:
                        {
                            imageResourceBanner = Resource.Drawable.notification_dbr_banner_email;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DBR_EBILL:
                    case Constants.BCRM_NOTIFICATION_DBR_EMAIL_REMOVED:
                        {
                            imageResourceBanner = Resource.Drawable.notification_dbr_banner_ebill;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_BILL_ESTIMATION_NEWS:
                        {
                            imageResourceBanner = Resource.Drawable.notification_smr_check_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_80:
                        {
                            imageResourceBanner = Resource.Drawable.notification_reaching_eb_icon;
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewBudget"),
                                delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewTips"),
                            delegate () { ViewTips(); });
                            ctaList.Add(secondaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_100:
                        {
                            imageResourceBanner = Resource.Drawable.notification_reached_eb_icon;
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewBudget"),
                                delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "viewTips"),
                            delegate () { ViewTips(); });
                            ctaList.Add(secondaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_ACCT_STATEMENT_READY:
                        {
                            imageResourceBanner = Resource.Drawable.Banner_Acct_Stmnt_Notification_Detail;
                            primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.VIEW_ACCT_STMNT),
                                delegate () { ViewAccountStatement(notificationDetails); });
                            primaryCTA.SetSolidCTA(true);
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_OUTAGE:
                        {
                            imageResourceBanner = Resource.Drawable.sd_outage_notification;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_INPROGRESS:
                        {
                            imageResourceBanner = Resource.Drawable.sd_in_progress_notification;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_RESTORATION:
                        {
                            imageResourceBanner = Resource.Drawable.sd_restoration_notification;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_UPDATE_NOW:
                        {
                            secondaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel("PushNotificationDetails", "updateNow"),
                                   delegate () { UpdateNow(); });
                            ctaList.Add(secondaryCTA);
                            imageResourceBanner = Resource.Drawable.notification_generic_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_APP_UPDATE:
                    case Constants.BCRM_NOTIFICATION_APP_UPDATE_2:
                        {
                            imageResourceBanner = Resource.Drawable.Banner_Notification_App_Update;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DIGITAL_SIGNATURE:
                        {
                            imageResourceBanner = Resource.Drawable.Icon_Notification_Details_Header;
                            //primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.NOTIF_TITLE_DIGITAL_SIGNATURE),
                            //    delegate () { DSNotificationDetails(); });
                            //primaryCTA.SetSolidCTA(true);
                            //ctaList.Add(primaryCTA);
                            break;
                        }
                    default:
                        imageResourceBanner = Resource.Drawable.notification_generic_banner;
                        break;
                }
                notificationDetailMessage = Regex.Replace(notificationDetailMessage, Constants.ACCOUNT_NICKNAME_PATTERN, accountName);

                if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_NEW_ACCOUNT_ADDED)
                {
                    notificationDetailMessage = Regex.Replace(notificationDetailMessage, Constants.ACCOUNT_FULLNAME_EMAIL_PATTERN, UserEntity.GetActive().DisplayName + "/" + UserEntity.GetActive().Email);

                }

                if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_UPDATE_ACCESS)
                {
                    notificationDetailTitle = Regex.Replace(notificationDetailTitle, Constants.ACCOUNT_NICKNAME_PATTERN, accountName);

                }

                if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_REMOVE_ACCESS)
                {
                    CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
                    if (account == null)
                    {
                        string notificationAccountName = "Contract Account Number " + notificationDetails.AccountNum;
                        notificationDetailMessage = Regex.Replace(notificationDetails.Message, Constants.ACCOUNT_NICKNAME_PATTERN, notificationAccountName);
                    }
                    else
                    {
                        notificationDetailMessage = Regex.Replace(notificationDetailMessage, Constants.ACCOUNT_NICKNAME_PATTERN, accountName);
                    }
                }

                if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_NEW_ACCESS_ADDED)
                {
                    notificationDetailMessage = Regex.Replace(notificationDetailMessage, Constants.ACCOUNT_FULLNAME_PATTERN, UserEntity.GetActive().DisplayName);
                }

                if (notificationDetailMessage.Contains(Constants.ACCOUNT_PROFILENAME_PATTERN))
                {
                    notificationDetailMessage = Regex.Replace(notificationDetailMessage, Constants.ACCOUNT_PROFILENAME_PATTERN, UserEntity.GetActive().DisplayName);
                }

                if (notificationDetailMessage.Contains(Constants.ACCOUNT_ACCNO_PATTERN))
                {
                    notificationDetailMessage = Regex.Replace(notificationDetailMessage, Constants.ACCOUNT_ACCNO_PATTERN, "\"" + accountName + "\"");
                }

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
                List<Notifications.Models.UserNotificationData> selectedNotificationList = new List<Notifications.Models.UserNotificationData>();
                Notifications.Models.UserNotificationData data = new Notifications.Models.UserNotificationData();
                data.Id = notificationDetails.Id;
                data.NotificationType = notificationDetails.NotificationType;
                selectedNotificationList.Add(data);
                UserNotificationDeleteResponse notificationDeleteResponse = await ServiceApiImpl.Instance.DeleteUserNotification(new UserNotificationDeleteRequest(selectedNotificationList));
                if (notificationDeleteResponse.IsSuccessResponse())
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

        private void ViewBillDetails(Models.NotificationDetails notificationDetails)
        {
            AccountData accountData = new AccountData();
            CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
            if (account != null)
            {
                accountData.AccountNum = account.AccNum;
                accountData.AccountNickName = account.AccDesc;
                accountData.AddStreet = account.AccountStAddress;
                accountData.AccountCategoryId = account.AccountCategoryId;
                mView.ViewDetails(accountData);
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
                accountData.AccountCategoryId = account.AccountCategoryId;
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
                accountData.AccountCategoryId = account.AccountCategoryId;
                this.mView.ViewUsage(accountData);
            }
            else
            {
                this.mView.ShowRetryOptionsApiException(null);
            }
        }

        private void UpdateNow()
        {
            try
            {
                this.mView.ShowUpateApp();
            }
            catch (Exception e)
            {
                this.mView.ShowRetryOptionsApiException(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ViewAccountStatement(Models.NotificationDetails notificationDetails)
        {
            CustomerBillingAccount.RemoveSelected();
            CustomerBillingAccount.SetSelected(notificationDetails.AccountNum);
            AccountData accountData = new AccountData();
            CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);

            if (account != null)
            {
                accountData.AccountNickName = account.AccDesc;
                accountData.AccountName = account.OwnerName;
                accountData.AddStreet = account.AccountStAddress;
                accountData.IsOwner = account.isOwned;
                accountData.AccountNum = account.AccNum;
                accountData.AccountCategoryId = account.AccountCategoryId;

                if (notificationDetails.AccountStatementDetail != null && notificationDetails.AccountStatementDetail.StatementPeriod.IsValid())
                {
                    this.mView.ViewAccountStatement(accountData, notificationDetails.AccountStatementDetail.StatementPeriod);
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


        //string url= Utility.GetLocalizedLabel("PushNotificationDetails", "linkEB");
        private void ViewTips()
        {
            this.mView.ViewTips();
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
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
                    MyTNBAppToolTipData.SetSMRActivityInfo(SMRAccountActivityInfoResponse.Response);
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
            CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
            AccountData accountData = new AccountData();
            accountData.AccountNum = notificationDetails.AccountNum;
            accountData.AddStreet = account.AccountStAddress;
            accountData.AccountNickName = account.AccDesc;
            accountData.AccountCategoryId = account.AccountCategoryId;
            this.mView.EnableSelfMeterReading(accountData);
        }

        private async Task GetSMRAccountStatus(string accountContractNumber)
        {
            HomeMenuServiceImpl serviceImpl = new HomeMenuServiceImpl();
            List<string> accountList = new List<string>();
            accountList.Add(accountContractNumber);
            this.mView.ShowLoadingScreen();
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = UserEntity.GetActive().DeviceId,
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                AccountSMRStatusResponse accountSMRResponse = await serviceImpl.GetSMRAccountStatus(new AccountsSMRStatusRequest()
                {
                    ContractAccounts = accountList,
                    UserInterface = currentUsrInf
                });
                if (accountSMRResponse.Response.ErrorCode == "7200" && accountSMRResponse.Response.Data.Count > 0)
                {
                    List<AccountSMRStatus> updateSMRStatus = accountSMRResponse.Response.Data;
                    foreach (AccountSMRStatus status in updateSMRStatus)
                    {
                        if (status.ContractAccount == accountContractNumber)
                        {
                            CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(status.ContractAccount);
                            isTaggedSMR = (status.IsTaggedSMR == "true");
                            if (isTaggedSMR != cbAccount.IsTaggedSMR)
                            {
                                CustomerBillingAccount.UpdateIsSMRTagged(status.ContractAccount, isTaggedSMR);
                            }
                            break;
                        }
                    }

                    List<CustomerBillingAccount> currentSMRBillingAccounts = CustomerBillingAccount.CurrentSMRAccountList();
                    List<SMRAccount> currentSmrAccountList = new List<SMRAccount>();
                    if (currentSMRBillingAccounts.Count > 0)
                    {
                        foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccounts)
                        {
                            SMRAccount currentSMRAccount = new SMRAccount();
                            currentSMRAccount.accountNumber = billingAccount.AccNum;
                            currentSMRAccount.accountName = billingAccount.AccDesc;
                            currentSMRAccount.accountAddress = billingAccount.AccountStAddress;
                            currentSMRAccount.accountSelected = false;
                            currentSmrAccountList.Add(currentSMRAccount);
                        }
                    }
                    UserSessions.SetSMRAccountList(currentSmrAccountList);

                    List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
                    List<SMRAccount> eligibleSmrAccountList = new List<SMRAccount>();
                    if (eligibleSMRBillingAccounts.Count > 0)
                    {
                        foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccounts)
                        {
                            SMRAccount currentSMRAccount = new SMRAccount();
                            currentSMRAccount.accountNumber = billingAccount.AccNum;
                            currentSMRAccount.accountName = billingAccount.AccDesc;
                            currentSMRAccount.accountAddress = billingAccount.AccountStAddress;
                            currentSMRAccount.accountSelected = false;
                            eligibleSmrAccountList.Add(currentSMRAccount);
                        }
                    }
                    UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);
                    UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
            finally
            {
                this.mView.HideLoadingScreen();
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

        private void ViewManageAccess(Models.NotificationDetails notificationDetails)
        {
            AccountData accountData = new AccountData();
            CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
            CustomerBillingAccount.RemoveSelected();
            CustomerBillingAccount.SetSelected(notificationDetails.AccountNum);

            accountData.AccountTypeId = account.AccountTypeId;
            accountData.AccountNickName = account.AccDesc;
            accountData.AccountName = account.OwnerName;
            accountData.AccountNum = account.AccNum;
            accountData.AddStreet = account.AccountStAddress;
            accountData.IsOwner = account.isOwned;
            accountData.AccountCategoryId = account.AccountCategoryId;
            accountData.IsHaveAccess = account.IsHaveAccess;
            accountData.IsApplyEBilling = account.IsApplyEBilling;
            this.mView.ViewManageAccess(accountData);
        }

        private void ViewBillHistory(Models.NotificationDetails notificationDetails)
        {
            AccountData accountData = new AccountData();
            CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationDetails.AccountNum);
            CustomerBillingAccount.RemoveSelected();
            CustomerBillingAccount.SetSelected(notificationDetails.AccountNum);

            accountData.AccountNickName = account.AccDesc;
            accountData.AccountName = account.OwnerName;
            accountData.AccountNum = account.AccNum;
            accountData.AddStreet = account.AccountStAddress;
            accountData.IsOwner = account.isOwned;
            accountData.AccountCategoryId = account.AccountCategoryId;
            this.mView.ViewBillHistory(accountData);
        }

        private async void ShowPaymentReceipt(Models.NotificationDetails notificationDetails)
        {
            string selectedAccountNumber = notificationDetails.AccountNum;
            string detailedInfoNumber = notificationDetails.MerchantTransId;
            bool isOwnedAccount = true;
            bool showAllReceipt = true;
            this.mView.ShowLoadingScreen();
            try
            {
                GetPaymentReceiptResponse result = await ServiceApiImpl.Instance.GetPaymentReceipt(new GetPaymentReceiptRequest(selectedAccountNumber, detailedInfoNumber, isOwnedAccount, showAllReceipt),
                    CancellationTokenSourceWrapper.GetTokenWithDelay(Constants.PAYMENT_RECEIPT_TIMEOUT));

                if (result.IsSuccessResponse())
                {
                    this.mView.ShowPaymentReceipt(result);
                }
                else
                {
                    this.mView.HideLoadingScreen();
                    this.mView.ShowPaymentReceiptError();
                }
            }
            catch (Exception e)
            {
                this.mView.HideLoadingScreen();
                Utility.LoggingNonFatalError(e);
                this.mView.ShowPaymentReceiptError();
            }
        }

        private void ShowSelectBill(Models.NotificationDetails notificationDetails)
        {
            AccountData accountData = new AccountData();
            accountData.AccountNum = notificationDetails.AccountNum;
            CustomerBillingAccount.RemoveSelected();
            CustomerBillingAccount.SetSelected(notificationDetails.AccountNum);

            this.mView.ShowSelectBill(accountData);
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

        public async void OnShowNotificationDetails(string NotificationTypeId, string BCRMNotificationTypeId, string NotificationRequestId)
        {
            try
            {
                this.mView.ShowLoadingScreen();
                UserNotificationDetailsRequestNew request = new UserNotificationDetailsRequestNew(NotificationTypeId, BCRMNotificationTypeId, NotificationRequestId);
                string dt = JsonConvert.SerializeObject(request);
                UserNotificationDetailsResponse response = await ServiceApiImpl.Instance.GetNotificationDetailsByRequestId(request);
                if (response.IsSuccessResponse())
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!response.Response.IsPayEnabled);
                    UserNotificationEntity.UpdateIsRead(response.GetData().UserNotificationDetail.Id, true);
                    UserSessions.ClearNotification();
                    EvaluateDetail(response.GetData().UserNotificationDetail);
                    this.mView.RenderUI();
                }
                else
                {
                    if (response.GetData() == null)
                    {
                        this.mView.ReturnToDashboard();
                    }
                    //this.mView.ShowRetryOptionsCancelledException(null);
                }

                ////MOCK RESPONSE
                //this.mView.ShowDetails(GetMockDetails(userNotification.BCRMNotificationTypeId), userNotification, position);
                this.mView.HideLoadingScreen();
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideLoadingScreen();
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideLoadingScreen();
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideLoadingScreen();
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        //checking count feedback EnergyBudget
        public async void OnCheckFeedbackCount()
        {
            try
            {
                int[] myIntArray = { 6, 7 };
                var questionRespone = await ServiceApiImpl.Instance.ShowEnergyBudgetRatingPage(new GetFeedbackTwoQuestionRequest(myIntArray));

                if (questionRespone.IsSuccessResponse())
                {
                    if (questionRespone.Response.ShowWLTYPage)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsFromClickAdapter(6);
                        GetRateUsQuestionsNo();
                    }
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (Exception e)
            {
                this.mView.HideLoadingScreen();
                Utility.LoggingNonFatalError(e);
            }
        }

        //checking count feedback EnergyBudget
        public async void OnCheckUserLeaveOut()
        {
            try
            {
                string questionCategoryID = "6";
                var questionRespone = await ServiceApiImpl.Instance.ExperienceRatingUserLeaveOut(new GetRateUsQuestionRequest(questionCategoryID));

                if (!questionRespone.IsSuccessResponse())
                {
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (Exception e)
            {
                this.mView.HideLoadingScreen();
                Utility.LoggingNonFatalError(e);
            }
        }

        //feedback EnergyBudget API QuestionCategoryIdNo
        public async void GetRateUsQuestionsNo()
        {
            try
            {
                string questionCategoryID = "6";
                var questionRespone = await ServiceApiImpl.Instance.GetRateUsQuestions(new GetRateUsQuestionRequest(questionCategoryID));
                if (!questionRespone.IsSuccessResponse())
                {
                    isSixHaveQuestion = false;
                    this.mView.ShowRetryOptionsApiException(null);
                }
                else
                {
                    isSixHaveQuestion = true;
                    this.mView.GetFeedbackTwoQuestionsNo(questionRespone);
                    GetRateUsQuestionsYes();

                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        //feedback EnergyBudget API QuestionCategoryIdYes
        public async void GetRateUsQuestionsYes()
        {
            try
            {
                string questionCategoryID = "7";
                var questionRespone = await ServiceApiImpl.Instance.GetRateUsQuestions(new GetRateUsQuestionRequest(questionCategoryID));
                if (!questionRespone.IsSuccessResponse())
                {
                    isSevenHaveQuestion = false;
                    this.mView.ShowRetryOptionsApiException(null);
                }
                else
                {
                    isSevenHaveQuestion = true;
                    this.mView.GetFeedbackTwoQuestionsYes(questionRespone);
                    if (isSixHaveQuestion && isSevenHaveQuestion)
                    {
                        this.mView.ShowFeedBackSetupPageRating();
                    }
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }
    }
}
