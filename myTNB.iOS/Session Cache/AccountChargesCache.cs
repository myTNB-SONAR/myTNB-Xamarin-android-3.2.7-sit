using System;
using System.Collections.Generic;
using System.Globalization;
using Force.DeepCloner;
using myTNB.Model;

namespace myTNB
{
    public sealed class AccountChargesCache
    {
        private static readonly Lazy<AccountChargesCache> lazy = new Lazy<AccountChargesCache>(() => new AccountChargesCache());
        public static AccountChargesCache Instance { get { return lazy.Value; } }

        private static Dictionary<string, AccountChargesModel> AccountChargesDisctionary = new Dictionary<string, AccountChargesModel>();
        private static List<PopupModel> AccountChargesPopupList = new List<PopupModel>();

        public static void SetData(GetAccountsChargesResponseModel response)
        {
            if (response != null && response.d != null && response.d.IsSuccess && response.d.data != null
                && response.d.data.AccountCharges != null && response.d.data.AccountCharges.Count > 0)
            {
                for (int i = 0; i < response.d.data.AccountCharges.Count; i++)
                {
                    AccountChargesModel item = response.d.data.AccountCharges[i];
                    if (AccountChargesDisctionary.ContainsKey(item.ContractAccount))
                    {
                        AccountChargesDisctionary[item.ContractAccount] = item;
                    }
                    else
                    {
                        AccountChargesDisctionary.Add(item.ContractAccount, item);
                    }
                }
                if (response.d.data.MandatoryChargesPopUpDetails != null && response.d.data.MandatoryChargesPopUpDetails.Count > 0)
                {
                    AccountChargesPopupList = response.d.data.MandatoryChargesPopUpDetails.DeepClone();
                }
            }
        }

        public static PopupModel GetPopupByType(string type)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrWhiteSpace(type)
                || AccountChargesPopupList == null || AccountChargesPopupList.Count < 1)
            {
                return null;
            }

            int popupIndex = AccountChargesPopupList.FindIndex(x => x.Type == type);
            if (popupIndex > -1)
            {
                return AccountChargesPopupList[popupIndex].DeepClone();
            }
            return null;
        }

        public static AccountChargesModel GetAccountCharges(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber) && AccountChargesDisctionary != null)
            {
                if (AccountChargesDisctionary.ContainsKey(accountNumber))
                {
                    return AccountChargesDisctionary[accountNumber];
                }
            }
            return new AccountChargesModel();
        }

        public static MandatoryChargesModel GetMandatoryCharges(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber) && AccountChargesDisctionary != null)
            {
                if (AccountChargesDisctionary.ContainsKey(accountNumber))
                {
                    AccountChargesModel item = AccountChargesDisctionary[accountNumber];
                    if (item != null && item.MandatoryCharges != null)
                    {
                        return item.MandatoryCharges;
                    }
                }
            }
            return new MandatoryChargesModel();
        }

        public static List<PaymentTypeModel> GetAccountPayments(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber) && AccountChargesDisctionary != null)
            {
                if (AccountChargesDisctionary.ContainsKey(accountNumber))
                {
                    AccountChargesModel accountCharges = AccountChargesDisctionary[accountNumber];
                    if (accountCharges != null && accountCharges.MandatoryCharges != null && accountCharges.MandatoryCharges.Charges != null)
                    {
                        List<PaymentTypeModel> mandatoryList = new List<PaymentTypeModel>();
                        for (int i = 0; i < accountCharges.MandatoryCharges.Charges.Count; i++)
                        {
                            ChargesModel charges = accountCharges.MandatoryCharges.Charges[i];
                            mandatoryList.Add(new PaymentTypeModel
                            {
                                PaymentAmount = charges.Amount.ToString("N2", CultureInfo.InvariantCulture),
                                PaymentType = charges.Key
                            });
                        }
                        return mandatoryList;
                    }
                }
            }
            return new List<PaymentTypeModel>();
        }

        public static void Clear()
        {
            AccountChargesDisctionary.Clear();
            AccountChargesPopupList.Clear();
        }

        public static bool HasMandatory(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber) && AccountChargesDisctionary != null)
            {
                if (AccountChargesDisctionary.ContainsKey(accountNumber))
                {
                    AccountChargesModel item = AccountChargesDisctionary[accountNumber];
                    return item.MandatoryCharges.TotalAmount > 0;
                }
            }
            return false;
        }
    }
}