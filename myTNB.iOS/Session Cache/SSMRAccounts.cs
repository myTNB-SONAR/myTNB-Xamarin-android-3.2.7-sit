using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using Foundation;
using myTNB.Model;

namespace myTNB
{
    public sealed class SSMRAccounts
    {
        private static readonly Lazy<SSMRAccounts> lazy = new Lazy<SSMRAccounts>(() => new SSMRAccounts());
        public static SSMRAccounts Instance { get { return lazy.Value; } }
        private static List<CustomerAccountRecordModel> FilteredEligibleAccountList = new List<CustomerAccountRecordModel>();
        private static List<CustomerAccountRecordModel> SSMRAccountList = new List<CustomerAccountRecordModel>();
        private static List<CustomerAccountRecordModel> SSMRCombinedList = new List<CustomerAccountRecordModel>();
        private static List<CustomerAccountRecordModel> EligibleAccountList = new List<CustomerAccountRecordModel>();
        private static List<PopupModel> PopupDetailsList = new List<PopupModel>();
        private static Dictionary<string, List<PopupSelectorModel>> SMREligibilityPopUpDetails;
        private static List<PopupSelectorModel> SMREligibilityPopUpList;
        private static readonly string PopupKey = SSMR.SSMRConstants.Popup_SMREligibiltyPopUpDetails;
        private static readonly string SelectorKey = SSMR.SSMRConstants.Pagename_SSMRReadingHistory;

        public static void SetFilteredEligibleAccounts()
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                FilteredEligibleAccountList = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindAll
                    (x => x.IsOwnedAccount && !x.IsSSMR && !x.IsREAccount && x.IsNormalMeter);
                SSMRAccountList = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindAll
                    (x => x.IsOwnedAccount && x.IsSSMR && !x.IsREAccount && x.IsNormalMeter);
                SSMRCombinedList.Clear();
                if (SSMRAccountList != null)
                {
                    SSMRCombinedList.AddRange(SSMRAccountList);
                }
                if (FilteredEligibleAccountList != null)
                {
                    SSMRCombinedList.AddRange(FilteredEligibleAccountList);
                }
            }
        }

        public static CustomerAccountRecordModel GetFirstSSMRAccount()
        {
            if (SSMRAccountList != null && SSMRAccountList.Count > 0 && SSMRAccountList[0] != null)
            {
                return SSMRAccountList[0];
            }
            return null;
        }

        public static CustomerAccountRecordModel GetFirstAccount()
        {
            if (FilteredEligibleAccountList != null && FilteredEligibleAccountList.Count > 0 && FilteredEligibleAccountList[0] != null)
            {
                return FilteredEligibleAccountList[0];
            }
            return null;
        }

        public static CustomerAccountRecordModel GetAccountByIndex(int index)
        {
            if (index > -1 && EligibleAccountList != null && EligibleAccountList.Count > 0 && index < EligibleAccountList.Count)
            {
                return EligibleAccountList[index].DeepClone();
            }
            return null;
        }

        public static void SetData(AccountsSMREligibilityDataModel response)
        {
            var accounts = response.data.accountEligibilities;
            if (accounts == null || SSMRCombinedList == null) { return; }
            EligibleAccountList.Clear();
            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].IsEligible || accounts[i].IsSSMR)
                {
                    int index = SSMRCombinedList.FindIndex(x => x.accNum == accounts[i].ContractAccount);
                    if (index > -1)
                    {
                        CustomerAccountRecordModel item = SSMRCombinedList[index];
                        item.isTaggedSMR = accounts[i].IsSMRTagged;
                        EligibleAccountList.Add(item.DeepClone());
                    }
                }
                DataManager.DataManager.SharedInstance.UpdateDueIsSSMR(accounts[i].ContractAccount, accounts[i].IsSMRTagged);
            }
            if (response.data.SMREligibiltyPopUpDetails != null)
            {
                PopupDetailsList = response.data.SMREligibiltyPopUpDetails;
            }
        }

        public static List<CustomerAccountRecordModel> GetEligibleAccountList()
        {
            return EligibleAccountList != null ? EligibleAccountList : new List<CustomerAccountRecordModel>();
        }

        public static CustomerAccountRecordModel GetAccountbyAccountNumber(string accNo)
        {
            if (string.IsNullOrEmpty(accNo) || string.IsNullOrWhiteSpace(accNo)) { return null; }
            if (SSMRCombinedList != null && SSMRCombinedList.Count > 0)
            {
                int index = SSMRCombinedList.FindIndex(x => x.accNum == accNo);
                if (index > -1)
                {
                    return SSMRCombinedList[index];
                }
            }
            return null;
        }

        public static List<string> GetFilteredAccountNumberList()
        {
            if (SSMRCombinedList != null && SSMRCombinedList.Count > 0)
            {
                List<string> accountList = SSMRCombinedList.Select(x => x.accNum).ToList();
                if (accountList != null)
                {
                    return accountList;
                }
            }
            return new List<string>();
        }

        public static bool HasSSMREligibleAccount
        {
            get
            {
                return FilteredEligibleAccountList != null && FilteredEligibleAccountList.Count > 0;
            }
        }

        public static bool HasSSMRAccount
        {
            get { return SSMRAccountList != null && SSMRAccountList.Count > 0; }
        }

        public static List<CustomerAccountRecordModel> GetAccounts()
        {
            if (FilteredEligibleAccountList != null && FilteredEligibleAccountList.Count > 0)
            {
                return FilteredEligibleAccountList;
            }
            return new List<CustomerAccountRecordModel>();
        }

        public static bool IsHideOnboarding
        {
            set
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(value, "SSMROnboarding");
                sharedPreference.Synchronize();
            }
            get
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                return sharedPreference.BoolForKey("SSMROnboarding");
            }
        }

        public static bool IsHideReadMeterWalkthrough
        {
            set
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(value, "ReadMeterWalkthrough");
                sharedPreference.Synchronize();
            }
            get
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                return sharedPreference.BoolForKey("ReadMeterWalkthrough");
            }
        }

        public static bool IsHideReadMeterWalkthroughV2
        {
            set
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(value, "ReadMeterWalkthroughV2");
                sharedPreference.Synchronize();
            }
            get
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                return sharedPreference.BoolForKey("ReadMeterWalkthroughV2");
            }
        }

        private static void SetPopupSelectorValues()
        {
            if (SMREligibilityPopUpDetails == null || SMREligibilityPopUpDetails.Count == 0
                || SMREligibilityPopUpList == null || SMREligibilityPopUpList.Count == 0)
            {
                SMREligibilityPopUpDetails = LanguageManager.Instance.GetPopupSelectorsByPage(SelectorKey);
                if (SMREligibilityPopUpDetails.ContainsKey(PopupKey))
                {
                    SMREligibilityPopUpList = SMREligibilityPopUpDetails[PopupKey];
                }
            }
        }

        private static PopupSelectorModel GetFallbackPopupValue(string type)
        {
            if (SMREligibilityPopUpList != null || SMREligibilityPopUpList.Count > 0)
            {
                PopupSelectorModel item = SMREligibilityPopUpList.Find(x => x.Type == type);
                if (item != null)
                {
                    return item;
                }
            }
            return new PopupSelectorModel();
        }

        public static PopupModel GetPopupDetailsByType(string type)
        {
            SetPopupSelectorValues();
            PopupSelectorModel fallback = GetFallbackPopupValue(type);
            if (PopupDetailsList != null && !string.IsNullOrEmpty(type) && !string.IsNullOrWhiteSpace(type))
            {
                int index = PopupDetailsList.FindIndex(x => x.Type.ToLower() == type.ToLower());
                if (index > -1)
                {
                    PopupModel popupDetails = PopupDetailsList[index];
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
    }
}