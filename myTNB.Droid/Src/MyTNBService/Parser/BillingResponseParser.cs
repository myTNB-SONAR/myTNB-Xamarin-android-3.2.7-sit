using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.MyTNBService.Model;
using static myTNB_Android.Src.MyTNBService.Response.AccountChargesResponse;

namespace myTNB_Android.Src.MyTNBService.Parser
{
    public class BillingResponseParser
    {
        public BillingResponseParser()
        {
        }
        /// <summary>
        /// Gets the parsed AccountCharges object from GetAccountsCharges API
        /// </summary>
        /// <param name="accountCharges"></param>
        /// <returns></returns>
        public static List<AccountChargeModel> GetAccountCharges(List<AccountCharge> accountCharges)
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
                MandatoryChargeModel mandatoryChargeModel = new MandatoryChargeModel
                {
                    TotalAmount = mandatoryCharge.TotalAmount,
                    ChargeModelList = chargeModelList
                };

                AccountChargeModel accountChargeModel = new AccountChargeModel
                {
                    IsCleared = false,
                    IsNeedPay = false,
                    IsPaidExtra = false,
                    ContractAccount = accountCharge.ContractAccount,
                    CurrentCharges = accountCharge.CurrentCharges,
                    ActualCurrentCharges = accountCharge.ActualCurrentCharges,
                    OutstandingCharges = accountCharge.OutstandingCharges,
                    AmountDue = accountCharge.AmountDue,
                    RoundingAmount = accountCharge.RoundingAmount,
                    DueDate = accountCharge.DueDate,
                    BillDate = accountCharge.BillDate,
                    IncrementREDueDateByDays = accountCharge.IncrementREDueDateByDays,
                    ShowEppToolTip = accountCharge.ShowEppToolTip,
                    MandatoryCharges = mandatoryChargeModel
                };
                EvaluateAmountDue(accountChargeModel);
                accountChargeModelList.Add(accountChargeModel);
            });
            return accountChargeModelList;
        }

        /// <summary>
        /// Gets the parsed BillMandatoryChargesTooltipModel from GetAccountsCharges API
        /// </summary>
        /// <param name="mandatoryChargesPopUpDetailList"></param>
        /// <returns></returns>
        public static List<BillMandatoryChargesTooltipModel> GetMandatoryChargesTooltipModelList(List<MandatoryChargesPopUpDetail> mandatoryChargesPopUpDetailList)
        {
            List<BillMandatoryChargesTooltipModel> billMandatoryChargesTooltipModelList = new List<BillMandatoryChargesTooltipModel>();
            BillMandatoryChargesTooltipModel model;
            mandatoryChargesPopUpDetailList.ForEach(popupDetail =>
            {
                model = new BillMandatoryChargesTooltipModel();
                model.Title = popupDetail.Title;
                model.Description = popupDetail.Description;
                model.Type = popupDetail.Type;
                model.CTA = popupDetail.CTA;
                billMandatoryChargesTooltipModelList.Add(model);
            });
            return billMandatoryChargesTooltipModelList;
        }

        /// <summary>
        /// Evaluate the amount status
        /// </summary>
        /// <param name="accountChargeModel"></param>
        private static void EvaluateAmountDue(AccountChargeModel accountChargeModel)
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
