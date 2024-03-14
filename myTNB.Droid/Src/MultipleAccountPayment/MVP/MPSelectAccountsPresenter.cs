using Android.Util;
using myTNB.Android.Src.Base;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.MultipleAccountPayment.Model;
using myTNB.Android.Src.MultipleAccountPayment.Requests;
using myTNB.Android.Src.myTNBMenu.Api;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.MyTNBService.Model;
using myTNB.Android.Src.MyTNBService.Parser;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.Response;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using static myTNB.Android.Src.MyTNBService.Response.AccountChargesResponse;

namespace myTNB.Android.Src.MultipleAccountPayment.MVP
{
    public class MPSelectAccountsPresenter : MPSelectAccountsContract.IUserActionsListener
    {
        private static readonly string TAG = "MPSelectAccountsPresenter";
        private MPSelectAccountsContract.IView mView;
        public bool isFromBillDetails = false;
        List<AccountChargeModel> accountChargeModelList;

        public MPSelectAccountsPresenter(MPSelectAccountsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            accountChargeModelList = new List<AccountChargeModel>();
        }

        public void Start()
        {

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
                AccountChargesResponse accountChargeseResponse = await ServiceApiImpl.Instance.GetAccountsCharges(accountChargeseRequest);
                if (accountChargeseResponse.IsSuccessResponse())
                {
                    MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargeseResponse.GetData().MandatoryChargesPopUpDetails));
                    accountChargeModelList.AddRange(BillingResponseParser.GetAccountCharges(accountChargeseResponse.GetData().AccountCharges));
                    List<MPAccount> newAccountList = new List<MPAccount>();
                    accountChargeseResponse.GetData().AccountCharges.ForEach(accountCharge =>
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
                            OpenChargeTotal = 0.00,
                            amount = dueAmount,
                            orgAmount = dueAmount,
                            minimumAmountDue = accountCharge.MandatoryCharges.TotalAmount,
                            isOwner = customerBillingAccount.isOwned
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
                    this.mView.ShowError(accountChargeseResponse.Response.DisplayMessage);
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
                this.mView.ShowError(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
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
