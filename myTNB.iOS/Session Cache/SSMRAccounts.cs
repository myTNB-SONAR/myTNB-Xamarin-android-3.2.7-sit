using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;

namespace myTNB
{
    public sealed class SSMRAccounts
    {
        private static readonly Lazy<SSMRAccounts> lazy = new Lazy<SSMRAccounts>(() => new SSMRAccounts());
        public static SSMRAccounts Instance { get { return lazy.Value; } }
        private static List<CustomerAccountRecordModel> EligibleAccountList = new List<CustomerAccountRecordModel>();

        public static void SetEligibleAccounts()
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                EligibleAccountList = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindAll
                    (x => x.IsOwnedAccount && !x.IsSSMR && !x.IsREAccount && x.IsNormalMeter);
            }
        }

        public static CustomerAccountRecordModel GetFirstAccount()
        {
            if (EligibleAccountList != null && EligibleAccountList.Count > 0 && EligibleAccountList[0] != null)
            {
                return EligibleAccountList[0];
            }
            return null;
        }

        public static CustomerAccountRecordModel GetAccountByIndex(int index)
        {
            if (index > -1 && EligibleAccountList != null && EligibleAccountList.Count > 0 && index < EligibleAccountList.Count)
            {
                return EligibleAccountList[index];
            }
            return null;
        }

        public static bool HasSSMREligibleAccount
        {
            get
            {
                return EligibleAccountList != null && EligibleAccountList.Count > 0;
            }
        }

        public static List<CustomerAccountRecordModel> GetAccounts()
        {
            if (EligibleAccountList != null && EligibleAccountList.Count > 0)
            {
                return EligibleAccountList;
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
    }
}