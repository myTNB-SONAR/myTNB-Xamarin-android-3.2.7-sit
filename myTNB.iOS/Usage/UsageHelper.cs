﻿using myTNB.Model;

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
                var groupAccountList = DataManager.DataManager.SharedInstance.AccountsGroupList;
                foreach (var accountList in groupAccountList)
                {
                    foreach (var account in accountList)
                    {
                        if (account.accNum == accountNo)
                        {
                            model = account;
                            break;
                        }
                    }
                }
            }
            return model;
        }
    }
}
