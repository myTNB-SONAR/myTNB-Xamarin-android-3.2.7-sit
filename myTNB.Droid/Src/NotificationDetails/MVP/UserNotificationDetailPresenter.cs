using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Billing;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.Utils;
using Refit;
using static myTNB_Android.Src.MyTNBService.Response.AccountChargesResponse;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class UserNotificationDetailPresenter
    {
        UserNotificationDetailContract.IView mView;
        public NotificationDetailModel notificationDetailModel;
        List<NotificationDetailModel.NotificationCTA> ctaList;
        BillingApiImpl api;
        AccountData mSelectedAccountData;
        private static readonly Regex accountNamePattern = new Regex("#(.+?)#", RegexOptions.Compiled);

        public UserNotificationDetailPresenter(UserNotificationDetailContract.IView view)
        {
            mView = view;
            api = new BillingApiImpl();
        }

        public void EvaluateDetail(Models.NotificationDetails notificationDetails)
        {
            try
            {
                NotificationDetailModel.NotificationCTA primaryCTA;
                NotificationDetailModel.NotificationCTA secondaryCTA;
                int imageResourceBanner = 0;
                string notificationDetailTitle = notificationDetails.Title;
                string notificationDetailMessage = notificationDetails.Message;
                ctaList = new List<NotificationDetailModel.NotificationCTA>();

                switch (notificationDetails.BCRMNotificationTypeId)
                {
                    case Constants.BCRM_NOTIFICATION_NEW_BILL_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;

                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            notificationDetailTitle = "Your {0} Bill Is Ready";
                            notificationDetailMessage = "Your bill is <b>{0}</b>. Got a minute? Make a quick and easy payment on the myTNB app now. <br/><br/>" +
                                "Account: #name#";
                            notificationDetailTitle = string.Format(notificationDetailTitle, "May 2019");
                            string formatMessageString = string.Format(notificationDetailMessage, "RM 123.45");
                            notificationDetailMessage = Utility.ReplaceValueByPattern(accountNamePattern, formatMessageString, "AccountNameValue");
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_BILL_DUE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_bill_due_banner;

                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            notificationDetailTitle = "Your {0} Bill Is Due";
                            notificationDetailMessage = "Hi, Mohd Zulkifli! On <b>{0}</b>, your Apr 2019 TNB bill amounting to <b>{1}</b> will be due.<br/><br/>" +
                                "No time to queue at TNB? No problem!Pay now on the myTNB app.Please disregard if paid.<br/><br/>Account: #name#";
                            notificationDetailTitle = string.Format(notificationDetailTitle, "April 2019");
                            string formatString = string.Format(notificationDetailMessage, "15 May", "RM 123.45");
                            notificationDetailMessage = Utility.ReplaceValueByPattern(accountNamePattern, formatString, "AccountNameValue");
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_disconnect_notice_banner;

                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            notificationDetailTitle = "Your Supply May Be Disconnected";
                            notificationDetailMessage = "Urgent notice, Mohd Zulkifli. Your electricity supply may be disconnected between <b>{0}</b> if you haven't already paid the bill. No worries, just pay before <b>{1}</b> on the myTNB app and all will be well! <br/><br/> " +
                                "Account: #name#<br/><br/>" +
                                "PS: Alternatively, you can pay via other methods too but it’s quicker and easier on the app!​";
                            string formatString = string.Format(notificationDetailMessage, "22 and 31 May", "21 May");
                            notificationDetailMessage = Utility.ReplaceValueByPattern(accountNamePattern, formatString, "AccountNameValue");
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DISCONNECTED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_disconnected_banner;

                            primaryCTA = new NotificationDetailModel.NotificationCTA("Contact TNB", delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            notificationDetailTitle = "Your Supply Has Been Disconnected";
                            notificationDetailMessage = "Don't panic, you can make a full payment on the myTNB app and you'll see the light again!<br/><br/>" +
                                "Account: #name#<br/><br/>" +
                                "PS: Alternatively, you can pay via other methods too but it’s quicker and easier on the app!​";
                            notificationDetailMessage = Utility.ReplaceValueByPattern(accountNamePattern, notificationDetailMessage, "AccountNameValue");
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_RECONNECTED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_reconnected_banner;
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View My Usage", delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            notificationDetailTitle = "Your Supply Has Been Reconnected";
                            notificationDetailMessage = "Hooray, the lights are back on! Your account has been reconnected. Stay on top of your monthly payments and your usage with the myTNB app.<br/><br/>" +
                                "Account: #name#";
                            notificationDetailMessage = Utility.ReplaceValueByPattern(accountNamePattern, notificationDetailMessage, "AccountNameValue");
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_MAINTENANCE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_maintenance_banner;

                            notificationDetailTitle = "Down For Maintenance from 4PM to 8PM on 31 Aug 2019";
                            notificationDetailMessage = "Don't worry, we'll be up and running quickly and better than before! We apologize for any inconvenience.";
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_METER_READING_OPEN_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;

                            primaryCTA = new NotificationDetailModel.NotificationCTA("Submit Meter Reading", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_METER_READING_REMIND_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Submit Meter Reading", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_smr_generic_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { PayNow(notificationDetails); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_APPLY_SUCCESS_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Usage", delegate () { ViewMyUsage(notificationDetails); });
                            ctaList.Add(primaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_smr_generic_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_APPLY_FAILED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Contact TNB", delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_smr_generic_banner;

                            primaryCTA = new NotificationDetailModel.NotificationCTA("Re-enable Self Meter Reading", delegate () { ViewBillDetails(notificationDetails); });
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_FAILED_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;

                            primaryCTA = new NotificationDetailModel.NotificationCTA("Contact TNB", delegate () { CallUs(); });
                            ctaList.Add(primaryCTA);
                            break;
                        }
                    default:
                        imageResourceBanner = Resource.Drawable.notification_generic_banner;
                        break;
                }

                notificationDetailModel = new NotificationDetailModel(imageResourceBanner, notificationDetailTitle,
                    notificationDetailMessage, ctaList);
            }
            catch (Exception e)
            {

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
