﻿using Android.Util;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Api;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.Requests;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Billing;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Parser;
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
        public bool isFromBillDetails = false;
        BillingApiImpl api;
        List<AccountChargeModel> accountChargeModelList;

        public MPSelectAccountsPresenter(MPSelectAccountsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            api = new BillingApiImpl();
            accountChargeModelList = new List<AccountChargeModel>();
        }

        public void Start()
        {

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
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargeseResponse.Data.ResponseData.MandatoryChargesPopUpDetails));
                    accountChargeModelList.AddRange(BillingResponseParser.GetAccountCharges(accountChargeseResponse.Data.ResponseData.AccountCharges));
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
                            orgAmount = dueAmount,
                            minimumAmountDue = accountCharge.MandatoryCharges.TotalAmount
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

                    if (preSelectedAccount != null)
                    {
                        int foundIndex = accountChargeModelList.FindIndex(model =>
                        {
                            return model.ContractAccount == preSelectedAccount;
                        });

                        if (foundIndex != -1)
                        {
                            if (!isFromBillDetails) //Checks if coming from Bill Details, dont show if true.
                            {
                                MPAccount mpAccount = newAccountList.Find(account =>
                                {
                                    return account.accountNumber.Equals(preSelectedAccount);
                                });
                                this.mView.ShowHasMinimumAmoutToPayTooltip(mpAccount,accountChargeModelList[foundIndex]);
                            }
                        }
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

        public List<AccountChargeModel> GetSelectedAccountChargesModelList(List<MPAccount> mpAccountList)
        {
            List<AccountChargeModel> selectedList = new List<AccountChargeModel>();
            mpAccountList.ForEach(account =>
            {
                AccountChargeModel foundChargeModel = accountChargeModelList.Find(model => { return model.ContractAccount == account.accountNumber; });
                selectedList.Add(foundChargeModel);
            });
            return selectedList;
        }

        public AccountChargeModel GetAccountChargeModel(MPAccount account)
        {
            return accountChargeModelList.Find(model =>
            {
                return model.ContractAccount == account.accountNumber;
            });
        }
    }
}
