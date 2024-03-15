using System.Collections.Generic;
using Android.Widget;
using myTNB.AndroidApp.Src.AddAccount.Models;

namespace myTNB.AndroidApp.Src.Utils
{
    public static class AddAccountUtils
    {
        public static List<NewAccount> accountList = new List<NewAccount>();
        public static List<NewAccount> additionalAccountList = new List<NewAccount>();

        public static void SetAccountList(List<NewAccount> newAccountList, List<NewAccount> newAdditionalAccountList)
        {
            if (newAccountList != null && newAccountList.Count > 0)
            {
                accountList = newAccountList;
            }
            else
            {
                accountList = new List<NewAccount>();
            }

            if (newAdditionalAccountList != null && newAdditionalAccountList.Count > 0)
            {
                additionalAccountList = newAdditionalAccountList;
            }
            else
            {
                additionalAccountList = new List<NewAccount>();
            }
        }

        public static void ClearList()
        {
            accountList = new List<NewAccount>();
            additionalAccountList = new List<NewAccount>();
        }

        public static List<NewAccount> GetAccountList()
        {
            return accountList;
        }

        public static List<NewAccount> GetAdditionalAccountList()
        {
            return additionalAccountList;
        }

        public static bool IsFoundAccountList(string accNum)
        {
            bool isFound = false;

            foreach (NewAccount item in accountList)
            {
                if (item.accountNumber.Equals(accNum))
                {
                    isFound = true;
                }
            }
            foreach (NewAccount item in additionalAccountList)
            {
                if (item.accountNumber.Equals(accNum))
                {
                    isFound = true;
                }
            }

            return isFound;
        }
    }
}