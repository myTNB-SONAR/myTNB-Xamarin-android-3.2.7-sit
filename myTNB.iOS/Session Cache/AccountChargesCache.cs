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
        private static List<PopupModel> PopupDetailList = new List<PopupModel>();
        private static Dictionary<string, List<PopupSelectorModel>> PopupDetailsDictionary;
        private static List<PopupSelectorModel> PopupList;
        private static readonly string SelectorKey = Home.Bill.BillConstants.Pagename_Bills;
        private static readonly string PopupKey = Home.Bill.BillConstants.Popup_MandatoryChargesPopUpDetails;

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
                    PopupDetailList = response.d.data.MandatoryChargesPopUpDetails.DeepClone();
                }
            }
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
            PopupDetailList.Clear();
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

        #region Popup
        public static PopupModel GetPopupDetailsByType(string type)
        {
            SetPopupSelectorValues();
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrWhiteSpace(type))
            {
                PopupSelectorModel fallback = GetFallbackPopupValue(type);
                if (PopupDetailList != null)
                {
                    int index = PopupDetailList.FindIndex(x => x.Type.ToLower() == type.ToLower());
                    if (index > -1)
                    {
                        PopupModel popupDetails = PopupDetailList[index];
                        return new PopupModel
                        {
                            Title = popupDetails != null && !string.IsNullOrEmpty(popupDetails.Title)
                                ? popupDetails.Title : fallback.Title,
                            Description = popupDetails != null && !string.IsNullOrEmpty(popupDetails.Description)
                                ? popupDetails.Description : fallback.Description,
                            CTA = popupDetails != null && !string.IsNullOrEmpty(popupDetails.CTA)
                                ? popupDetails.CTA : fallback.CTA,
                            Type = popupDetails != null && !string.IsNullOrEmpty(popupDetails.Type)
                                ? popupDetails.Type : fallback.Type
                        };
                    }
                }
                else
                {
                    return new PopupModel
                    {
                        Title = fallback.Title,
                        Description = fallback.Description,
                        CTA = fallback.CTA,
                        Type = fallback.Type
                    };
                }
            }
            return null;
        }

        private static void SetPopupSelectorValues()
        {
            if (PopupDetailsDictionary == null || PopupDetailsDictionary.Count == 0
                || PopupList == null || PopupList.Count == 0)
            {
                PopupDetailsDictionary = LanguageManager.Instance.GetPopupSelectorsByPage(SelectorKey);
                if (PopupDetailsDictionary.ContainsKey(PopupKey))
                {
                    PopupList = PopupDetailsDictionary[PopupKey];
                }
            }
        }

        private static PopupSelectorModel GetFallbackPopupValue(string type)
        {
            if (PopupList != null || PopupList.Count > 0)
            {
                PopupSelectorModel item = PopupList.Find(x => x.Type == type);
                if (item != null)
                {
                    return item;
                }
            }
            return new PopupSelectorModel();
        }
        #endregion
    }
}