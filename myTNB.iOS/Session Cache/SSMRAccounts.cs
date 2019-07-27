using System;
using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public sealed class SSMRAccounts
    {
        private static readonly Lazy<SSMRAccounts> lazy = new Lazy<SSMRAccounts>(() => new SSMRAccounts());
        public static SSMRAccounts Instance { get { return lazy.Value; } }
        private static List<CustomerAccountRecordModel> EligibleAccount = new List<CustomerAccountRecordModel>();

        public static void SetEligibleAccounts()
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                EligibleAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindAll
                    (x => !x.IsREAccount && !x.IsSSMR && (x.IsNormalMeter || x.IsOwnedAccount));
            }
        }

        public static CustomerAccountRecordModel GetFirstAccount()
        {
            if (EligibleAccount != null && EligibleAccount.Count > 0 && EligibleAccount[0] != null)
            {
                return EligibleAccount[0];
            }
            return null;
        }

        public static CustomerAccountRecordModel GetAccountByIndex(int index)
        {
            if (index > -1 && EligibleAccount != null && EligibleAccount.Count > 0 && index < EligibleAccount.Count)
            {
                return EligibleAccount[index];
            }
            return null;
        }

        public static bool HasSSMREligibleAccount
        {
            get
            {
                return EligibleAccount != null && EligibleAccount.Count > 0;
            }
        }
    }
}