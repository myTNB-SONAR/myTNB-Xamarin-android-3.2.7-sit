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
                accountChargeModel.RoundingAmount = accountCharge.RoundingAmount;
                accountChargeModel.DueDate = accountCharge.DueDate;
                accountChargeModel.BillDate = accountCharge.BillDate;
                accountChargeModel.IncrementREDueDateByDays = accountCharge.IncrementREDueDateByDays;
                accountChargeModel.ShowEppToolTip = accountCharge.ShowEppToolTip;
                accountChargeModel.MandatoryCharges = mandatoryChargeModel;
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
