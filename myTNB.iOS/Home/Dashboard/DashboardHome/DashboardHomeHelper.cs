using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
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
                if (count < DashboardHomeConstants.MaxAccountPerLoad)
                {
                    DueAmountDataModel item = new DueAmountDataModel
                    {
                        accNum = sortedAccounts[i].accNum,
                        accNickName = sortedAccounts[i].accountNickName,
                        IsReAccount = sortedAccounts[i].IsREAccount,
                        IsNormalAccount = sortedAccounts[i].IsNormalMeter,
                        IsSSMR = sortedAccounts[i].IsSSMR,
                        IsOwnedAccount = sortedAccounts[i].IsOwnedAccount,
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
                        IsOwnedAccount = sortedAccounts[i].IsOwnedAccount,
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

        public List<DueAmountDataModel> GeAccountList(List<CustomerAccountRecordModel> acctsList)
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
                        IsSSMR = sortedAccounts[i].IsSSMR,
                        IsOwnedAccount = sortedAccounts[i].IsOwnedAccount,
                        amountDue = acctCached != null ? acctCached.amountDue : 0.00,
                        billDueDate = acctCached != null ? acctCached.billDueDate : string.Empty
                    };
                    acctList.Add(item);
                }
            }
            return acctList;
        }

        /// <summary>
        /// Returns the account model using account number
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public DueAmountDataModel GetModelWithAccountNumber(string accountNo)
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

        /// <summary>
        /// Returns if the account is SSMR or not
        /// </summary>
        /// <param name="customerModel"></param>
        /// <returns></returns>
        public bool IsSSMR(CustomerAccountRecordModel customerModel)
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

        /// <summary>
        /// Returns the list of account numbers that is SSMR
        /// </summary>
        /// <param name="acctList"></param>
        /// <param name="groupedAcctList"></param>
        /// <returns></returns>
        public List<string> FilterAccountNoForSSMR(List<string> acctList, List<DueAmountDataModel> groupedAcctList)
        {
            List<string> accounts = new List<string>();

            if (acctList.Count <= 0 || groupedAcctList.Count <= 0)
                return accounts;

            foreach (var acct in groupedAcctList)
            {
                foreach (string accNo in acctList)
                {
                    if (acct.accNum == accNo)
                    {
                        if (acct.IsNormalAccount && acct.IsOwnedAccount)
                        {
                            accounts.Add(accNo);
                        }
                    }
                }
            }
            return accounts;
        }

        /// <summary>
        /// Returns the height for Account Cards + other views height to be displayed
        /// </summary>
        /// <returns>pageViewHeight</returns>
        public nfloat GetHeightForAccountCards()
        {
            bool hasAccounts = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0;
            nfloat additionalHeight = DashboardHomeConstants.SearchViewHeight + ScaleUtility.GetScaledHeight(32f);
            nfloat pageViewHeight = hasAccounts ? additionalHeight : ScaleUtility.GetScaledHeight(60f) + ScaleUtility.GetScaledHeight(16F) + additionalHeight;
            var groupAccountsList = DataManager.DataManager.SharedInstance.AccountsGroupList;
            if (groupAccountsList.Count > 1)
            {
                pageViewHeight = 5 * (ScaleUtility.GetScaledHeight(60f) +
                    ScaleUtility.GetScaledHeight(8f)) +
                    additionalHeight +
                    ScaleUtility.GetScaledHeight(16f) +
                    DashboardHomeConstants.PageControlHeight -
                    ScaleUtility.GetScaledHeight(8f);
            }
            else if (groupAccountsList.Count == 1)
            {
                var accountsList = groupAccountsList[0];
                if (accountsList.Count > 1)
                {
                    pageViewHeight = accountsList.Count * (ScaleUtility.GetScaledHeight(60f) + ScaleUtility.GetScaledHeight(8f)) +
                        ScaleUtility.GetScaledHeight(16F) +
                        additionalHeight -
                        ScaleUtility.GetScaledHeight(8f);
                }
                else
                {
                    pageViewHeight = ScaleUtility.GetScaledHeight(16F) + ScaleUtility.GetScaledHeight(60f) +
                        additionalHeight;
                }

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
                tableViewCellHeight = 5 * (ScaleUtility.GetScaledHeight(60f) + ScaleUtility.GetScaledHeight(8f));
            }
            else if (groupAccountsList.Count == 1)
            {
                var accountsList = groupAccountsList[0];
                tableViewCellHeight = accountsList.Count * (ScaleUtility.GetScaledHeight(60f) + ScaleUtility.GetScaledHeight(8f));
            }
            return tableViewCellHeight;
        }

        public nfloat GetHeightForAccountList()
        {
            nfloat totalCellHeight;
            if (HasAccounts)
            {
                nfloat footerHeight = HasMoreThanThreeAccts ? AllAccountsAreVisible ? ScaleUtility.GetScaledHeight(85F) : ScaleUtility.GetScaledHeight(44F) : ScaleUtility.GetScaledHeight(16F);
                var activeAcctList = DataManager.DataManager.SharedInstance.ActiveAccountList;
                nfloat acctListTotalHeight = ScaleUtility.GetScaledHeight(61F) * activeAcctList.Count;
                totalCellHeight = acctListTotalHeight + DashboardHomeConstants.SearchViewHeight + ScaleUtility.GetScaledHeight(24F);
                totalCellHeight += DataManager.DataManager.SharedInstance.AccountListIsLoaded ? footerHeight : ScaleUtility.GetScaledHeight(28F);
            }
            else
            {
                totalCellHeight = DashboardHomeConstants.SearchViewHeight + ScaleUtility.GetScaledHeight(24F);
                totalCellHeight += DataManager.DataManager.SharedInstance.IsOnSearchMode ? ScaleUtility.GetScaledHeight(28F) : ScaleUtility.GetScaledHeight(101F);
            }
            return totalCellHeight;
        }

        public bool AllAccountsAreVisible
        {
            get
            {
                var allAcctList = DataManager.DataManager.SharedInstance.CurrentAccountList;
                var activeAcctList = DataManager.DataManager.SharedInstance.ActiveAccountList;
                return activeAcctList.Count == allAcctList.Count;
            }
        }

        public bool HasAccounts
        {
            get
            {
                return DataManager.DataManager.SharedInstance.CurrentAccountList?.Count > 0;
            }
        }

        public bool HasMoreThanThreeAccts
        {
            get
            {
                return DataManager.DataManager.SharedInstance.CurrentAccountList?.Count > 3;
            }
        }

        public bool HasREAccounts
        {
            get
            {
                var result = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindAll(x => x.IsREAccount);
                return result?.Count > 0;
            }
        }

        public bool IsEmptyAccount
        {
            get
            {
                return DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count <= 0;
            }
        }

        public bool HasNormalAccounts
        {
            get
            {
                var result = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindAll(x => x.IsNormalMeter && !x.IsREAccount);
                return result?.Count > 0;
            }
        }

        public bool HasMultipleNormalAccounts
        {
            get
            {
                var result = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindAll(x => x.IsNormalMeter && !x.IsREAccount);
                return result?.Count > 1;
            }
        }

        public bool HasMultipleREAccounts
        {
            get
            {
                var result = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindAll(x => x.IsREAccount);
                return result?.Count > 1;
            }
        }

        /// <summary>
        /// Returns the height for Services Cell
        /// </summary>
        /// <returns></returns>
        public nfloat GetHeightForServices(bool isShimmering)
        {
            nfloat tableViewCellHeight = 0;
            nfloat cardHeight = ScaleUtility.GetScaledHeight(84F);

            if (isShimmering)
            {
                tableViewCellHeight = ScaleUtility.GetScaledHeight(100F);
            }
            else
            {
                var serviceList = DataManager.DataManager.SharedInstance.ActiveServicesList;
                bool isMoreThanThreeItems = DataManager.DataManager.SharedInstance.ServicesList.Count > 3;
                if (serviceList != null &&
                    serviceList.Count > 0)
                {
                    var multiplier = Math.Ceiling((double)serviceList.Count / 3);
                    tableViewCellHeight += cardHeight * (nfloat)multiplier;
                    if (isMoreThanThreeItems)
                    {
                        tableViewCellHeight += ScaleUtility.GetScaledHeight(41F) + ScaleUtility.GetScaledHeight(16F);
                    }
                    else
                    {
                        tableViewCellHeight += ScaleUtility.GetScaledHeight(16F);
                    }
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
            nfloat tableViewCellHeight = DeviceHelper.IsIphoneXUpResolution() ? ScaleUtility.GetScaledHeight(12F) : 0;
            if (isShimmering)
            {
                tableViewCellHeight += ScaleUtility.GetScaledHeight(108F);
            }
            else
            {
                if (DataManager.DataManager.SharedInstance.HelpList?.Count > 0)
                {
                    tableViewCellHeight += ScaleUtility.GetScaledHeight(108F);
                }
            }
            return tableViewCellHeight;
        }

        public nfloat GetHeightForPromotions
        {
            get
            {
                return 58F + (UIApplication.SharedApplication.KeyWindow.Frame.Width * 0.64F * 0.98F);
                /*if (HasPromotion)
                {
                    return 50F + (UIApplication.SharedApplication.KeyWindow.Frame.Width * 0.64F * 0.98F);
                }
                return 0;*/
            }
        }

        public nfloat GetDefaulthHeightForRefreshScreen()
        {
            return 384f;
        }

        public static bool HasPromotion
        {
            get
            {
                PromotionsEntity entity = new PromotionsEntity();
                List<PromotionsModelV2> promotions = entity.GetAllItemsV2();
                return promotions != null && promotions.Count > 0;
            }
        }
    }
}
