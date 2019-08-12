using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.Model;

namespace myTNB
{
    public sealed class AmountDueCache
    {
        private static readonly Lazy<AmountDueCache> lazy = new Lazy<AmountDueCache>(() => new AmountDueCache());
        private static Dictionary<string, DueAmountDataModel> AmountDueDictionary = new Dictionary<string, DueAmountDataModel>();

        public static AmountDueCache Instance { get { return lazy.Value; } }

        public static void SaveDues(DueAmountDataModel dueAmount)
        {
            string accountNumber = dueAmount?.accNum ?? string.Empty;
            if (string.IsNullOrEmpty(accountNumber) && string.IsNullOrWhiteSpace(accountNumber))
            {
                Debug.WriteLine("Error AmoutDueCache/SaveDues, Account Number is Empty");
                return;
            }
            if (AmountDueDictionary == null)
            {
                AmountDueDictionary = new Dictionary<string, DueAmountDataModel>();
            }
            if (IsAccountExist(accountNumber))
            {
                AmountDueDictionary[accountNumber] = dueAmount;
            }
            else
            {
                AmountDueDictionary.Add(accountNumber, dueAmount);
            }
        }
        #region Public Functions
        public static DueAmountDataModel GetDues(string accountNumber)
        {
            if (IsAccountExist(accountNumber))
            {
                return AmountDueDictionary[accountNumber];
            }
            return null;
        }

        public static void DeleteDue(string accountNumber)
        {
            if (IsAccountExist(accountNumber))
            {
                AmountDueDictionary.Remove(accountNumber);
            }
        }

        public static void UpdateNickname(string accountNumber, string nickname)
        {
            if (IsAccountExist(accountNumber) && !string.IsNullOrEmpty(nickname) && !string.IsNullOrWhiteSpace(nickname))
            {
                DueAmountDataModel dueAmount = AmountDueDictionary[accountNumber];
                if (dueAmount != null)
                {
                    dueAmount.accNickName = nickname;
                    AmountDueDictionary[accountNumber] = dueAmount;
                }
            }
        }

        public static void UpdateIsSSMR(string accountNumber, string isTaggedSMR)
        {
            if (IsAccountExist(accountNumber) && !string.IsNullOrEmpty(isTaggedSMR) && !string.IsNullOrWhiteSpace(isTaggedSMR))
            {
                DueAmountDataModel dueAmount = AmountDueDictionary[accountNumber];
                if (dueAmount != null)
                {
                    var flag = string.Compare(isTaggedSMR.ToLower(), "true") == 0;
                    dueAmount.IsSSMR = flag;
                    AmountDueDictionary[accountNumber] = dueAmount;
                }
            }
        }

        public static void Reset()
        {
            if (AmountDueDictionary != null)
            {
                AmountDueDictionary.Clear();
            }
        }
        #endregion
        #region Private Functions
        private AmountDueCache() { }

        private static bool IsAccountExist(string accountNumber)
        {
            if (AmountDueDictionary != null && AmountDueDictionary.Count > 0
                && !string.IsNullOrEmpty(accountNumber) && !string.IsNullOrWhiteSpace(accountNumber))
            {
                return AmountDueDictionary.ContainsKey(accountNumber);
            }
            return false;
        }
        #endregion
    }
}