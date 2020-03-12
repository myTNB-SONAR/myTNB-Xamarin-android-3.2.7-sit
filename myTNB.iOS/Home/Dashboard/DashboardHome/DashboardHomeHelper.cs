using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;
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

        public List<DueAmountDataModel> GetAccountListForDashboard(List<CustomerAccountRecordModel> acctsList)
        {
            if (acctsList == null) { return new List<DueAmountDataModel>(); }
            List<CustomerAccountRecordModel> sortedAccounts = acctsList;
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
                var accountList = GetAccountListForDashboard(DataManager.DataManager.SharedInstance.AccountRecordsList.d);
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
        /// <param name="acctNoList"></param>
        /// <param name="acctList"></param>
        /// <returns></returns>
        public List<string> FilterAccountNoForSSMR(List<string> acctNoList, List<DueAmountDataModel> acctList)
        {
            List<string> accounts = new List<string>();

            if (acctNoList == null || acctNoList.Count <= 0 || acctList == null || acctList.Count <= 0)
                return accounts;

            foreach (var acct in acctList)
            {
                foreach (string accNo in acctNoList)
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
        /// Returns the list of account numbers that is SSMR
        /// </summary>
        /// <param name="acctNoList"></param>
        /// <param name="acctList"></param>
        /// <returns></returns>
        public List<string> FilterAccountNoForSSMR(List<string> acctNoList, List<CustomerAccountRecordModel> acctList)
        {
            List<string> accounts = new List<string>();

            if (acctNoList == null || acctNoList.Count <= 0 || acctList == null || acctList.Count <= 0)
                return accounts;

            foreach (var acct in acctList)
            {
                foreach (string accNo in acctNoList)
                {
                    if (acct.accNum == accNo)
                    {
                        if (acct.IsNormalMeter && acct.IsOwnedAccount)
                        {
                            accounts.Add(accNo);
                        }
                    }
                }
            }
            return accounts;
        }

        public List<string> GetOwnedAccountsList(List<CustomerAccountRecordModel> acctList)
        {
            List<string> accounts = new List<string>();

            if (acctList == null || acctList.Count <= 0)
                return accounts;

            foreach (var acct in acctList)
            {
                if (acct.IsNormalMeter && acct.IsOwnedAccount)
                {
                    accounts.Add(acct.accNum);
                }
            }
            return accounts;
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
                if (allAcctList != null && activeAcctList != null)
                {
                    return activeAcctList.Count == allAcctList.Count;
                }
                return false;
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

        public bool HasSmartMeterAccounts
        {
            get
            {
                var result = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindAll(x => !x.IsREAccount && !x.IsNormalMeter);
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
        public nfloat GetHeightForServices(bool isShimmering, bool isServiceFail = false)
        {
            if (isServiceFail)
            {
                return ScaleUtility.GetScaledHeight(224);
            }
            nfloat tableViewCellHeight = 0;
            nfloat cardHeight = ScaleUtility.GetScaledHeight(84F);

            if (isShimmering)
            {
                tableViewCellHeight = ScaleUtility.GetScaledHeight(100F);
            }
            else
            {
                var serviceList = DataManager.DataManager.SharedInstance.ActiveServicesList;
                bool isMoreThanThreeItems = DataManager.DataManager.SharedInstance.ServicesList != null
                    && DataManager.DataManager.SharedInstance.ServicesList.Count > 3;
                if (serviceList != null && serviceList.Count > 0)
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
                else
                {
                    tableViewCellHeight = ScaleUtility.GetScaledHeight(100F);
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
                    tableViewCellHeight += ScaleUtility.GetScaledHeight(108F) + DashboardHomeConstants.PageControlHeight;
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
                List<PromotionsModel> promotions = entity.GetAllItemsV2();
                return promotions != null && promotions.Count > 0;
            }
        }
    }
}
