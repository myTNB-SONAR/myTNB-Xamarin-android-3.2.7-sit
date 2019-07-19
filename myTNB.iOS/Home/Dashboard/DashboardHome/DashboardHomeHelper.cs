using System;
using System.Collections.Generic;
using System.Linq;
using myTNB.Model;

namespace myTNB
{
    public class DashboardHomeHelper
    {
        /// <summary>
        /// Returns the Greeting Display Name
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName()
        {
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0 && DataManager.DataManager.SharedInstance.UserEntity[0] != null)
            {
                return string.Format("{0}!", DataManager.DataManager.SharedInstance.UserEntity[0]?.displayName);
            }
            return string.Empty;
        }

        /// <summary>
        /// Groups the linked CAs
        /// </summary>
        /// <param name="accountsList"></param>
        public void GroupAccountsList(List<CustomerAccountRecordModel> accountsList)
        {
            var sortedAccounts = new List<CustomerAccountRecordModel>();

            var results = accountsList.GroupBy(x => x.IsREAccount);

            if (results != null && results?.Count() > 0)
            {
                var reAccts = results.Where(x => x.Key == true).SelectMany(y => y).OrderBy(o => o.accountNickName).ToList();
                var normalAccts = results.Where(x => x.Key == false).SelectMany(y => y).OrderBy(o => o.accountNickName).ToList();
                reAccts.AddRange(normalAccts);
                sortedAccounts = reAccts;
            }

            var groupedAccountsList = new List<List<DueAmountDataModel>>();

            int count = 0;
            List<DueAmountDataModel> batchList = new List<DueAmountDataModel>();
            for (int i = 0; i < sortedAccounts.Count; i++)
            {
                if (count < DashboardHomeConstants.MaxAccountPerCard)
                {
                    DueAmountDataModel item = new DueAmountDataModel
                    {
                        accNum = sortedAccounts[i].accNum,
                        accNickName = sortedAccounts[i].accountNickName,
                        IsReAccount = sortedAccounts[i].IsREAccount,
                        IsNormalAccount = sortedAccounts[i].IsNormalMeter
                    };

                    batchList.Add(item);
                    count++;
                }
                else
                {
                    groupedAccountsList.Add(batchList);
                    batchList = new List<DueAmountDataModel>();
                    DueAmountDataModel item = new DueAmountDataModel
                    {
                        accNum = sortedAccounts[i].accNum,
                        accNickName = sortedAccounts[i].accountNickName,
                        IsReAccount = sortedAccounts[i].IsREAccount,
                        IsNormalAccount = sortedAccounts[i].IsNormalMeter
                    };
                    batchList.Add(item);
                    count = 1;
                }

                if (i + 1 == sortedAccounts.Count)
                {
                    groupedAccountsList.Add(batchList);
                }
            }
            DataManager.DataManager.SharedInstance.AccountsGroupList = new List<List<DueAmountDataModel>>();
            DataManager.DataManager.SharedInstance.AccountsGroupList = groupedAccountsList;
        }

        /// <summary>
        /// Returns the height for Account Cards to be displayed
        /// </summary>
        /// <returns></returns>
        public nfloat GetHeightForAccountCards()
        {
            nfloat pageViewHeight = 0f;
            nfloat addtlMargin = 24f;
            var groupAccountsList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            if (groupAccountsList.Count > 1)
            {
                pageViewHeight = 5 * 68 + 32f + addtlMargin;
            }
            else if (groupAccountsList.Count == 1)
            {
                var accountsList = groupAccountsList[0];
                pageViewHeight = accountsList.Count * 68 + 32f + addtlMargin;
            }
            return pageViewHeight;
        }
    }
}
