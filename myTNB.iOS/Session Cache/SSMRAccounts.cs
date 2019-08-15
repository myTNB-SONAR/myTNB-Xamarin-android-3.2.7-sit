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
        private static List<CustomerAccountRecordModel> SSMRAccountList = new List<CustomerAccountRecordModel>();
        private static List<CustomerAccountRecordModel> SSMRCombinedList = new List<CustomerAccountRecordModel>();

        public static void SetEligibleAccounts()
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                EligibleAccountList = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindAll
                    (x => x.IsOwnedAccount && !x.IsSSMR && !x.IsREAccount && x.IsNormalMeter);
                SSMRAccountList = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindAll
                    (x => x.IsOwnedAccount && x.IsSSMR && !x.IsREAccount && x.IsNormalMeter);
                if (SSMRAccountList != null)
                {
                    SSMRCombinedList.AddRange(SSMRAccountList);
                }
                if (EligibleAccountList != null)
                {
                    SSMRCombinedList.AddRange(EligibleAccountList);
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
    }
}