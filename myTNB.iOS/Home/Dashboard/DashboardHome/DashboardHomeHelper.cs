using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Model;
using UIKit;

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
            DataManager.DataManager.SharedInstance.AccountsGroupList = new List<List<DueAmountDataModel>>();
            DataManager.DataManager.SharedInstance.AccountsGroupList = GetGroupAccountsList(accountsList);
        }

        /// <summary>
        /// Returns the Grouped Linked CAs
        /// </summary>
        /// <param name="accountsList"></param>
        public List<List<DueAmountDataModel>> GetGroupAccountsList(List<CustomerAccountRecordModel> accountsList)
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
                var acctCached = DataManager.DataManager.SharedInstance.GetDue(sortedAccounts[i].accNum);
                if (count < DashboardHomeConstants.MaxAccountPerCard)
                {
                    DueAmountDataModel item = new DueAmountDataModel
                    {
                        accNum = sortedAccounts[i].accNum,
                        accNickName = sortedAccounts[i].accountNickName,
                        IsReAccount = sortedAccounts[i].IsREAccount,
                        IsNormalAccount = sortedAccounts[i].IsNormalMeter,
                        IsSSMR = sortedAccounts[i].IsSSMR,
                        amountDue = acctCached != null ? acctCached.amountDue : 0.00,
                        billDueDate = acctCached != null ? acctCached.billDueDate : string.Empty
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
                        IsNormalAccount = sortedAccounts[i].IsNormalMeter,
                        IsSSMR = sortedAccounts[i].IsSSMR,
                        amountDue = acctCached != null ? acctCached.amountDue : 0.00,
                        billDueDate = acctCached != null ? acctCached.billDueDate : string.Empty
                    };
                    batchList.Add(item);
                    count = 1;
                }

                if (i + 1 == sortedAccounts.Count)
                {
                    groupedAccountsList.Add(batchList);
                }
            }

            return groupedAccountsList;
        }

        /// <summary>
        /// Returns the height for Account Cards + other views height to be displayed
        /// </summary>
        /// <returns>pageViewHeight</returns>
        public nfloat GetHeightForAccountCards()
        {
            nfloat additionalHeight = DashboardHomeConstants.GreetingViewHeight + DashboardHomeConstants.SearchViewHeight + DashboardHomeConstants.PageControlHeight;
            nfloat pageViewHeight = 0f + additionalHeight;
            var groupAccountsList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            if (groupAccountsList.Count > 1)
            {
                pageViewHeight = 5 * 68 + 20f + additionalHeight;
            }
            else if (groupAccountsList.Count == 1)
            {
                var accountsList = groupAccountsList[0];
                pageViewHeight = accountsList.Count * 68 + 20f + additionalHeight;
            }
            return pageViewHeight;
        }

        /// <summary>
        /// Returns the height for Accounts Cell
        /// </summary>
        /// <returns>pageViewHeight</returns>
        public nfloat GetHeightForAccountCardsOnly()
        {
            nfloat tableViewCellHeight = 0f;
            var groupAccountsList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            if (groupAccountsList.Count > 1)
            {
                tableViewCellHeight = 5 * 68 + 20f;
            }
            else if (groupAccountsList.Count == 1)
            {
                var accountsList = groupAccountsList[0];
                tableViewCellHeight = accountsList.Count * 68 + 20f;
            }
            return tableViewCellHeight;
        }

        /// <summary>
        /// Returns the height for Services Cell
        /// </summary>
        /// <returns></returns>
        public nfloat GetHeightForServices(bool isShimmering)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat cardWidth = (cellWidth - 24) / 3;
            nfloat cardHeight = cardWidth * 0.9545F;
            nfloat tableViewCellHeight = 20f;
            if (isShimmering)
            {
                tableViewCellHeight = tableViewCellHeight + (cardHeight * 2) + 24f;
            }
            else
            {
                if (DataManager.DataManager.SharedInstance.ServicesList?.Count > 0)
                {
                    var rowNumber = Math.Ceiling((double)DataManager.DataManager.SharedInstance.ServicesList.Count / 3);
                    tableViewCellHeight = ((System.nfloat)(tableViewCellHeight + (cardHeight * rowNumber) + (12f * rowNumber)));
                }
            }
            return tableViewCellHeight;
        }

        /// <summary>
        /// Returns the height for Help Cell
        /// </summary>
        /// <returns></returns>
        public nfloat GetHeightForHelp(bool isShimmering)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat tableViewCellHeight = 20f;
            nfloat cardHeight = 0f;
            if (isShimmering)
            {
                cardHeight = cellWidth * 0.30f + 24.0f + 40f;
            }
            else
            {
                if (DataManager.DataManager.SharedInstance.HelpList?.Count > 0)
                {
                    cardHeight = cellWidth * 0.30f + 24.0f + 40f;
                }
            }
            tableViewCellHeight += cardHeight;
            return tableViewCellHeight;
        }
    }
}
