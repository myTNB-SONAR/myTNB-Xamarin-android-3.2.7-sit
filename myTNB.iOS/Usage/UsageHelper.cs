using System;
using System.Collections.Generic;
using System.Linq;
using myTNB.Model;

namespace myTNB
{
    public static class UsageHelper
    {
        public static string GetRMkWhValueStringForEnum(RMkWhEnum rMkWhEnum)
        {
            string valueString = string.Empty;
            switch (rMkWhEnum)
            {
                case RMkWhEnum.RM:
                    valueString = "RM";
                    break;
                case RMkWhEnum.kWh:
                    valueString = "kWh";
                    break;
            }
            return valueString;
        }

        public static bool IsSSMR(CustomerAccountRecordModel customerModel)
        {
            bool res = false;
            if (customerModel != null)
            {
                if (customerModel.IsNormalMeter)
                {
                    var model = GetModelWithAccountNumber(customerModel.accNum);
                    if (model != null)
                    {
                        res = model.IsSSMR && model.IsOwnedAccount;
                    }
                }
            }
            return res;
        }

        public static DueAmountDataModel GetModelWithAccountNumber(string accountNo)
        {
            DueAmountDataModel model = new DueAmountDataModel();
            if (!string.IsNullOrEmpty(accountNo))
            {
                var accountList = GetAccountList(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
                foreach (var account in accountList)
                {
                    if (account.accNum == accountNo)
                    {
                        model = account;
                        break;
                    }
                }
            }
            return model;
        }

        public static int[] RandomizedTips(int count, int randomSize)
        {
            int[] intValues = new int[randomSize];
            var random = new Random();
            var values = Enumerable.Range(0, count).OrderBy(x => random.Next()).ToArray();

            for (int i = 0; i < randomSize; i++)
            {
                intValues[i] = values[i];
            }
            return intValues;
        }

        private static List<DueAmountDataModel> GetAccountList(List<CustomerAccountRecordModel> acctsList)
        {
            var sortedAccounts = new List<CustomerAccountRecordModel>();

            var results = acctsList.GroupBy(x => x.IsREAccount);
            if (results != null && results?.Count() > 0)
            {
                var reAccts = results.Where(x => x.Key == true).SelectMany(y => y).OrderBy(o => o.accountNickName).ToList();
                var normalAccts = results.Where(x => x.Key == false).SelectMany(y => y).OrderBy(o => o.accountNickName).ToList();
                reAccts.AddRange(normalAccts);
                sortedAccounts = reAccts;
            }

            List<DueAmountDataModel> acctList = new List<DueAmountDataModel>();
            if (sortedAccounts != null &&
                sortedAccounts.Count > 0)
            {
                for (int i = 0; i < sortedAccounts.Count; i++)
                {
                    var acctCached = DataManager.DataManager.SharedInstance.GetDue(sortedAccounts[i].accNum);
                    DueAmountDataModel item = new DueAmountDataModel
                    {
                        accNum = sortedAccounts[i].accNum,
                        accNickName = sortedAccounts[i].accountNickName,
                        IsReAccount = sortedAccounts[i].IsREAccount,
                        IsNormalAccount = sortedAccounts[i].IsNormalMeter,
                        IsOwnedAccount = sortedAccounts[i].IsOwnedAccount,
                        IsSSMR = acctCached != null ? acctCached.IsSSMR : sortedAccounts[i].IsSSMR,
                        amountDue = acctCached != null ? acctCached.amountDue : 0.00,
                        billDueDate = acctCached != null ? acctCached.billDueDate : string.Empty
                    };
                    acctList.Add(item);
                }
            }
            return acctList;
        }
    }
}
