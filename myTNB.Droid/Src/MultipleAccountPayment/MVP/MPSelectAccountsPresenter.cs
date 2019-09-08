using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Api;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.Requests;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Billing;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using static myTNB_Android.Src.MyTNBService.Response.AccountChargesResponse;

namespace myTNB_Android.Src.MultipleAccountPayment.MVP
{
    public class MPSelectAccountsPresenter : MPSelectAccountsContract.IUserActionsListener
    {
        private static readonly string TAG = "MPSelectAccountsPresenter";
        private MPSelectAccountsContract.IView mView;
        BillingApiImpl api;
        List<AccountChargeModel> accountChargeModelList;

        public MPSelectAccountsPresenter(MPSelectAccountsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            api = new BillingApiImpl();
        }

        public void Start()
        {

        }

        public void GetMultiAccountDueAmount(string apiKeyID, List<string> accounts, string preSelectedAccount)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetMultiAccountDueAmountAsync(apiKeyID, accounts, preSelectedAccount);
        }

        public void OnSelectAccount(CustomerBillingAccount selectedCustomerBilling)
        {
            try
            {
            AccountData accountData = AccountData.Copy(selectedCustomerBilling, true);
            this.mView.ShowDashboardChart(accountData);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void GetAccountsCharges(List<string> accountList, string preSelectedAccount)
        {
            try
            {
                this.mView.ShowProgressDialog();
                AccountsChargesRequest accountChargeseRequest = new AccountsChargesRequest(
                    accountList,
                    true
                    );
                AccountChargesResponse accountChargeseResponse = await api.GetAccountsCharges<AccountChargesResponse>(accountChargeseRequest);
                if (accountChargeseResponse.Data != null && accountChargeseResponse.Data.ErrorCode == "7200")
                {
                    accountChargeModelList = GetAccountCharges(accountChargeseResponse.Data.ResponseData.AccountCharges);
                    List<MPAccount> newAccountList = new List<MPAccount>();
                    accountChargeseResponse.Data.ResponseData.AccountCharges.ForEach(accountCharge =>
                    {
                        CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountCharge.ContractAccount);
                        double dueAmount = accountCharge.AmountDue;

                        bool isSelectedAccount = false;
                        if (preSelectedAccount != null)
                        {
                            isSelectedAccount = preSelectedAccount.Equals(customerBillingAccount.AccNum) ? true && dueAmount > 0 : false;
                        }
                        MPAccount mpAccount = new MPAccount()
                        {
                            accountLabel = customerBillingAccount.AccDesc,
                            accountNumber = customerBillingAccount.AccNum,
                            accountAddress = customerBillingAccount.AccountStAddress,
                            isSelected = isSelectedAccount,
                            isTooltipShow = false,
#if STUB
                                    OpenChargeTotal = account.OpenChargesTotal == 0.00 ? 0.00 : account.OpenChargesTotal,
#else
                            OpenChargeTotal = 0.00,
#endif
                            amount = dueAmount,
                            orgAmount = dueAmount
                        };
                        newAccountList.Add(mpAccount);
                        /*** Save SM Usage History For the Day***/
                        SelectBillsEntity smUsageModel = new SelectBillsEntity();
                        smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                        smUsageModel.JsonResponse = JsonConvert.SerializeObject(mpAccount);
                        smUsageModel.AccountNo = customerBillingAccount.AccNum;
                        SelectBillsEntity.InsertItem(smUsageModel);
                        /*****/
                    });

                    this.mView.SetAccountsDueAmountResult(newAccountList);

                    int foundIndex = accountChargeModelList.FindIndex(model =>
                    {
                        return model.ContractAccount == preSelectedAccount;
                    });

                    if (foundIndex != -1)
                    {
                        this.mView.ShowHasMinimumAmoutToPayTooltip(accountChargeModelList[foundIndex]);
                    }
                }
                else
                {
                    this.mView.ShowError(accountChargeseResponse.Data.DisplayMessage);
                    this.mView.DisablePayButton();
                }
                this.mView.HideProgressDialog();
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowError("Something went wrong, Please try again.");
                this.mView.DisablePayButton();
            }
        }

        public async void GetMultiAccountDueAmountAsync(string apiKeyId, List<string> accounts, string preSelectedAccount)
        {
            try
            {
                this.mView.ShowProgressDialog();

#if DEBUG || STUB
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var api = RestService.For<MPGetAccountsDueAmountApi>(httpClient);
#else
            var api = RestService.For<MPGetAccountsDueAmountApi>(Constants.SERVER_URL.END_POINT);
#endif

                List<MPAccount> storeAccounts = new List<MPAccount>();
                bool getDetailsFromApi = true;

                if (getDetailsFromApi)
                {
                    MPAccountDueResponse result = await api.GetMultiAccountDueAmount(new MPGetAccountDueAmountRequest(apiKeyId, accounts));
                    this.mView.HideProgressDialog();
                    if (result.accountDueAmountResponse != null && !result.accountDueAmountResponse.IsError)
                    {
                        this.mView.GetAccountDueAmountResult(result);
                    }
                    else
                    {
                        this.mView.ShowError(result.accountDueAmountResponse.Message);
                        this.mView.DisablePayButton();
                    }
                }
                else
                {
                    this.mView.HideProgressDialog();
                    this.mView.GetAccountDueAmountResult(storeAccounts);
                }
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowError("Something went wrong, Please try again.");
                this.mView.DisablePayButton();
            }

        }

        private List<AccountChargeModel> GetAccountCharges(List<AccountCharge> accountCharges)
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

        public void EvaluateAmountDue(AccountChargeModel accountChargeModel)
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
    }
}
