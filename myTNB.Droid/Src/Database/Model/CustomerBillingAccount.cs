using myTNB_Android.Src.AddAccount.Models;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile;
using Android.Preferences;

namespace myTNB_Android.Src.Database.Model
{
    [Table("CustomerBillingAccountEntity")]
    public class CustomerBillingAccount
    {
        [Column("__type")]
        public string Type { get; set; }

        [PrimaryKey, Column("accNum")]
        public string AccNum { get; set; }

        [Column("accDesc")]
        public string AccDesc { get; set; }

        [Column("userAccountID")]
        public string UserAccountId { get; set; }

        [Column("icNum")]
        public string ICNum { get; set; }

        [Column("amCurrentChg")]
        public string AmtCurrentChg { get; set; }

        [Column("isRegistered")]
        public bool IsRegistered { get; set; }

        [Column("isPaid")]
        public bool IsPaid { get; set; }

        [Column("isOwned")]
        public bool isOwned { get; set; }

        [Column("isError")]
        public bool IsError { get; set; }

        [Column("accountTypeId")]
        public string AccountTypeId { get; set; }

        [Column("accountStAddress")]
        public string AccountStAddress { get; set; }

        [Column("ownerName")]
        public string OwnerName { get; set; }

        [Column("isSelected")]
        public bool IsSelected { get; set; }

        [Column("accountCategoryId")]
        public string AccountCategoryId { get; set; }

        [Column("smartMeterCode")]
        public string SmartMeterCode { get; set; }

        [Column("isTaggedSMR")]
        public bool IsTaggedSMR { get; set; }

        [Column("IsPeriodOpen")]
        public bool IsPeriodOpen { get; set; }
        [Column("IsSMROnBoardingDontShowAgain")]
        public bool IsSMROnBoardingDontShowAgain { get; set; }

        [Column("billingDetails")]
        public string billingDetails { get; set; }

        [Column("IsSMROnboardingShown")]
        public bool IsSMROnboardingShown { get; set; }

        [Column("IsSMRMeterReadingOnBoardShown")]
        public bool IsSMRMeterReadingOnBoardShown { get; set; }

        [Column("IsSMRMeterReadingThreePhaseOnBoardShown")]
        public bool IsSMRMeterReadingThreePhaseOnBoardShown { get; set; }

        [Column("IsSMRTakePhotoOnBoardShown")]
        public bool IsSMRTakePhotoOnBoardShown { get; set; }

        [Column("IsWhatNewShown")]
        public bool IsWhatNewShown { get; set; }

        [Column("IsPayBillShown")]
        public bool IsPayBillShown { get; set; }

        [Column("IsViewBillShown")]
        public bool IsViewBillShown { get; set; }

        [Column("IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [Column("IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

        [Column("BudgetAmount")]
        public string BudgetAmount { get; set; }

        [Column("InstallationType")]
        public string InstallationType { get; set; }

        [JsonProperty("createdDate")]
        public string CreatedDate { set; get; }

        [Column("BusinessArea")]
        public string BusinessArea { get; set; }

        [Column("RateCategory")]
        public string RateCategory { get; set; }

        [Column("IsInManageAccessList")]
        public bool IsInManageAccessList { get; set; }

        [Column("CreatedBy")]
        public string CreatedBy { get; set; }

        [Column("AccountHasOwner")]
        public bool AccountHasOwner { get; set; }

        [JsonIgnore]
        public bool IsNormalMeter
        {
            get
            {
                var res = true;

                if (!string.IsNullOrEmpty(SmartMeterCode))
                {
                    res = string.Compare(SmartMeterCode, "0") == 0;
                }
                return res;
            }
        }

        [JsonIgnore]
        public bool IsREAccount
        {
            get
            {
                return AccountCategoryId != null && AccountCategoryId.Equals("2");
            }
        }

        [JsonIgnore]
        public bool IsSmartMeter
        {
            get
            {
                return !IsNormalMeter && !IsREAccount;
            }
        }

        [JsonIgnore]
        public bool IsSSMR
        {
            get
            {
                return IsTaggedSMR;
            }
        }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<CustomerBillingAccount>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<CustomerBillingAccount>();
        }

        public static int InsertOrReplace(NewAccount accountResponse, bool isSelected)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new CustomerBillingAccount()
            {
                Type = accountResponse.type,
                AccNum = accountResponse.accountNumber,
                AccDesc = string.IsNullOrEmpty(accountResponse.accountLabel) == true ? "--" : accountResponse.accountLabel,
                UserAccountId = accountResponse.userAccountId,
                ICNum = accountResponse.icNum,
                AmtCurrentChg = accountResponse.amCurrentChg,
                IsRegistered = accountResponse.isRegistered,
                IsPaid = accountResponse.isPaid,
                isOwned = accountResponse.isOwner,
                IsError = accountResponse.IsError,
                AccountTypeId = accountResponse.accountTypeId,
                AccountStAddress = accountResponse.accountAddress,
                OwnerName = accountResponse.ownerName,
                AccountCategoryId = accountResponse.accountCategoryId,
                SmartMeterCode = accountResponse.smartMeterCode == null ? "0" : accountResponse.smartMeterCode,
                IsSelected = isSelected,
                IsTaggedSMR = accountResponse.IsTaggedSMR,
                BudgetAmount = accountResponse.BudgetAmount == null ? "0" : accountResponse.BudgetAmount,
                InstallationType = accountResponse.InstallationType == null ? "0" : accountResponse.InstallationType,
                CreatedDate = accountResponse.CreatedDate,
                IsInManageAccessList = accountResponse.IsInManageAccessList,
                CreatedBy = accountResponse.CreatedBy,
                IsHaveAccess = accountResponse.IsHaveAccess,
                AccountHasOwner = accountResponse.AccountHasOwner
                
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }


        public static int InsertOrReplace(Account accountResponse, bool isSelected)
        {
            var db = DBHelper.GetSQLiteConnection();

            var newRecord = new CustomerBillingAccount()
            {
                Type = accountResponse.Type,
                AccNum = accountResponse.AccountNumber,
                AccDesc = string.IsNullOrEmpty(accountResponse.AccDesc) == true ? "--" : accountResponse.AccDesc,
                UserAccountId = accountResponse.UserAccountID,
                ICNum = accountResponse.IcNum,
                AmtCurrentChg = accountResponse.AmCurrentChg,
                IsRegistered = accountResponse.IsRegistered,
                IsPaid = accountResponse.IsPaid,
                IsSelected = isSelected,
                AccountTypeId = accountResponse.AccountTypeId,
                AccountStAddress = accountResponse.AccountStAddress,
                OwnerName = accountResponse.OwnerName,
                AccountCategoryId = accountResponse.AccountCategoryId,
                SmartMeterCode = accountResponse.SmartMeterCode == null ? "0" : accountResponse.SmartMeterCode,
                IsTaggedSMR = accountResponse.IsTaggedSMR == "true" ? true : false,
                isOwned = accountResponse.IsOwned,
                IsSMROnBoardingDontShowAgain = false,
                IsPeriodOpen = false
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(CustomerAccountListResponse.CustomerAccountData accountResponse, bool isSelected)
        {
            var db = DBHelper.GetSQLiteConnection();

            var newRecord = new CustomerBillingAccount()
            {
                Type = accountResponse.Type,
                AccNum = accountResponse.AccountNumber,
                AccDesc = string.IsNullOrEmpty(accountResponse.AccDesc) == true ? "--" : accountResponse.AccDesc,
                UserAccountId = accountResponse.UserAccountID,
                ICNum = accountResponse.IcNum,
                AmtCurrentChg = accountResponse.AmCurrentChg,
                IsRegistered = accountResponse.IsRegistered,
                IsPaid = accountResponse.IsPaid,
                IsSelected = isSelected,
                AccountTypeId = accountResponse.AccountTypeId,
                AccountStAddress = accountResponse.AccountStAddress,
                OwnerName = accountResponse.OwnerName,
                AccountCategoryId = accountResponse.AccountCategoryId,
                SmartMeterCode = accountResponse.SmartMeterCode == null ? "0" : accountResponse.SmartMeterCode,
                IsTaggedSMR = accountResponse.IsTaggedSMR == "true" ? true : false,
                isOwned = accountResponse.IsOwned,
                IsError = accountResponse.IsError,
                IsSMROnBoardingDontShowAgain = false,
                IsPeriodOpen = false,
                IsHaveAccess = accountResponse.IsHaveAccess,
                IsApplyEBilling = accountResponse.IsApplyEBilling,
                BudgetAmount = accountResponse.BudgetAmount,
                InstallationType = accountResponse.InstallationType == null ? "0" : accountResponse.InstallationType,
                CreatedDate = accountResponse.CreatedDate,
                BusinessArea = accountResponse.BusinessArea,
                RateCategory = accountResponse.RateCategory,
                IsInManageAccessList = accountResponse.IsInManageAccessList,
                CreatedBy = accountResponse.CreatedBy,
                AccountHasOwner = accountResponse.AccountHasOwner
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(CustomerAccountListResponseAppLaunch.CustomerAccountData accountResponse, bool isSelected)
        {
            var db = DBHelper.GetSQLiteConnection();

            var newRecord = new CustomerBillingAccount()
            {
                //Type = accountResponse.Type,
                AccNum = accountResponse.AccountNumber,
                AccDesc = string.IsNullOrEmpty(accountResponse.AccDesc) == true ? "--" : accountResponse.AccDesc,
                UserAccountId = accountResponse.UserAccountID,
                ICNum = accountResponse.IcNum,
                AmtCurrentChg = accountResponse.AmCurrentChg,
                IsRegistered = accountResponse.IsRegistered,
                IsPaid = accountResponse.IsPaid,
                IsSelected = isSelected,
                AccountTypeId = accountResponse.AccountTypeId,
                AccountStAddress = accountResponse.AccountStAddress,
                OwnerName = accountResponse.OwnerName,
                AccountCategoryId = accountResponse.AccountCategoryId,
                SmartMeterCode = accountResponse.SmartMeterCode == null ? "0" : accountResponse.SmartMeterCode,
                IsTaggedSMR = accountResponse.IsTaggedSMR == "true" ? true : false,
                isOwned = accountResponse.IsOwned,
                IsError = accountResponse.IsError,
                IsSMROnBoardingDontShowAgain = false,
                IsPeriodOpen = false,
                IsHaveAccess = accountResponse.IsHaveAccess,
                IsApplyEBilling = accountResponse.IsApplyEBilling,
                BudgetAmount = accountResponse.BudgetAmount,
                InstallationType = accountResponse.InstallationType == null ? "0" : accountResponse.InstallationType,
                CreatedDate = accountResponse.CreatedDate,
                BusinessArea = accountResponse.BusinessArea,
                RateCategory = accountResponse.RateCategory,
                IsInManageAccessList = accountResponse.IsInManageAccessList,
                CreatedBy = accountResponse.CreatedBy,
                AccountHasOwner = accountResponse.AccountHasOwner
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(CustomerBillingAccount account)
        {

            var db = DBHelper.GetSQLiteConnection();

            int newRecordRow = db.InsertOrReplace(account);

            return newRecordRow;

        }

        public static List<CustomerBillingAccount> List()
        {
            List<CustomerBillingAccount> sortedList = new List<CustomerBillingAccount>();
            UserEntity activeUser = UserEntity.GetActive();

            if (activeUser != null)
            {
                if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                {
                    sortedList.AddRange(AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV));
                    List<CustomerBillingAccount> excludeNonNCList = NonNCAccountListExclude(sortedList);
                    List<CustomerBillingAccount> excludeREList = REAccountListExclude(sortedList);
                    List<CustomerBillingAccount> excludeNonREList = NonREAccountListExclude(sortedList);
                    if (excludeREList != null && excludeREList.Count > 0)
                    {
                        sortedList.AddRange(excludeREList);
                    }
                    if (excludeNonREList != null && excludeNonREList.Count > 0)
                    {
                        sortedList.AddRange(excludeNonREList);
                    }
                    if (excludeNonNCList != null && excludeNonNCList.Count > 0)
                    {
                        sortedList.AddRange(excludeNonNCList);
                    }
                }
                else
                {
                    sortedList.AddRange(NCAccountList());
                    sortedList.AddRange(REAccountList());
                    sortedList.AddRange(NonREAccountList());
                }
            }
            else
            {
                sortedList.AddRange(NCAccountList());
                sortedList.AddRange(REAccountList());
                sortedList.AddRange(NonREAccountList());
            }
            return sortedList;
        }

        public static bool HasSelected()
        {
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            CustomerBillingAccount selected = updatedList.Find(x => x.IsSelected);

                            if (selected != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var db = DBHelper.GetSQLiteConnection();
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE isSelected = ?", true).Count > 0;
        }

        public static bool HasItems()
        {
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            return updatedList.Count > 0;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var db = DBHelper.GetSQLiteConnection();
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity").Count > 0;
        }

        public static CustomerBillingAccount GetSelected()
        {
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            CustomerBillingAccount selected = updatedList.Find(x => x.IsSelected);
                            if (selected != null)
                            {
                                return selected;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var db = DBHelper.GetSQLiteConnection();
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE isSelected = ?", true).ToList()[0];
        }

        public static CustomerBillingAccount GetFirst()
        {
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            return updatedList[0];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


            return GetSortedCustomerBillingAccounts()[0];
        }

        public static CustomerBillingAccount GetSelectedOrFirst()
        {
            if (HasSelected())
            {
                return GetSelected();
            }
            else if (HasItems())
            {
                return GetFirst();
            }
            return null;
        }

        public static void RemoveSelected()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("UPDATE CustomerBillingAccountEntity SET isSelected = ? WHERE isSelected = ?", false, true);

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            foreach (CustomerBillingAccount acc in updatedList)
                            {
                                acc.IsSelected = false;
                            }
                            string updatedListString = JsonConvert.SerializeObject(updatedList);
                            AccountSortingEntity.InsertOrReplace(activeUser.Email, Constants.APP_CONFIG.ENV, updatedListString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void UpdateIsSMRTagged(string accNum, bool isTaggedSMR)
        {
            var db = DBHelper.GetSQLiteConnection();
            var existingRecord = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accNum = ? ", accNum);

            if (existingRecord != null && existingRecord.Count > 0)
            {
                var customerBARecord = existingRecord[0];
                customerBARecord.IsTaggedSMR = isTaggedSMR;
                db.Update(customerBARecord);

                try
                {
                    UserEntity activeUser = UserEntity.GetActive();
                    if (activeUser != null)
                    {
                        if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                        {
                            AccountSortingEntity.ReplaceSpecificAccount(activeUser.Email, Constants.APP_CONFIG.ENV, customerBARecord);
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        public static void RemoveActive()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();

                UserEntity activeUser = UserEntity.GetActive();

                try
                {
                    if (activeUser != null)
                    {
                        AccountSortingEntity.RemoveSpecificAccountSorting(activeUser.Email, Constants.APP_CONFIG.ENV);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                db.Execute("Delete from CustomerBillingAccountEntity ");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static void Remove(string AccountNum)
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Delete from CustomerBillingAccountEntity WHERE accNum = ?", AccountNum);

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        AccountSortingEntity.RemoveSpecificAccount(activeUser.Email, Constants.APP_CONFIG.ENV, AccountNum);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void UpdateAccountName(string newAccountName, string accNum)
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET accDesc = ? WHERE accNum = ?", newAccountName, accNum);

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        CustomerBillingAccount updatedAccount = FindByAccNum(accNum);
                        if (updatedAccount != null)
                        {
                            AccountSortingEntity.ReplaceSpecificAccount(activeUser.Email, Constants.APP_CONFIG.ENV, updatedAccount);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void UpdateBillingDetails(List<SummaryDashBoardAccountEntity> summaryDetails)
        {
            var db = DBHelper.GetSQLiteConnection();
            bool isUpdateNeeded = false;

            UserEntity activeUser = UserEntity.GetActive();

            try
            {
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        isUpdateNeeded = true;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            foreach (SummaryDashBoardAccountEntity billingDetails in summaryDetails)
            {
                db.Execute("Update CustomerBillingAccountEntity SET billingDetails = ? WHERE accNum = ?", billingDetails.JsonResponse, billingDetails.AccountNo);

                if (isUpdateNeeded && activeUser != null)
                {
                    try
                    {
                        CustomerBillingAccount acc = FindByAccNum(billingDetails.AccountNo);
                        AccountSortingEntity.ReplaceSpecificAccount(activeUser.Email, Constants.APP_CONFIG.ENV, acc);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }
        }

        public static void RemoveCustomerBillingDetails()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET billingDetails = null");

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            foreach (CustomerBillingAccount acc in updatedList)
                            {
                                acc.billingDetails = null;
                            }
                            string updatedListString = JsonConvert.SerializeObject(updatedList);
                            AccountSortingEntity.InsertOrReplace(activeUser.Email, Constants.APP_CONFIG.ENV, updatedListString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void SetSelected(string accNum)
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET IsSelected = ? WHERE accNum = ?", true, accNum);

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        CustomerBillingAccount updatedAccount = FindByAccNum(accNum);
                        if (updatedAccount != null)
                        {
                            AccountSortingEntity.ReplaceSpecificAccount(activeUser.Email, Constants.APP_CONFIG.ENV, updatedAccount);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static CustomerBillingAccount FindByAccNum(string accNum)
        {
            var db = DBHelper.GetSQLiteConnection();
            var record = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accNum =?", accNum);

            if (record != null && record.Count > 0)
            {
                return record[0];
            }
            return null;
        }


        public static List<CustomerBillingAccount> REAccountList()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<CustomerBillingAccount> reAccountList = new List<CustomerBillingAccount>();
            reAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId = 2 ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            return reAccountList;
        }

        public static List<CustomerBillingAccount> REAccountListExclude(List<CustomerBillingAccount> accList)
        {
            List<CustomerBillingAccount> reAccountList = new List<CustomerBillingAccount>();
            if (accList != null && accList.Count > 0)
            {
                string excludeList = "";
                int i = 0;
                foreach (CustomerBillingAccount acc in accList)
                {
                    if (i == accList.Count - 1)
                    {
                        excludeList += "'" + acc.AccNum + "'";
                    }
                    else
                    {
                        excludeList += "'" + acc.AccNum + "',";
                    }
                    i++;
                }
                var db = DBHelper.GetSQLiteConnection();
                reAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId = 2 AND accNum NOT IN (" + excludeList + ") ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            }
            return reAccountList;
        }

        public static List<CustomerBillingAccount> NonREAccountListExclude(List<CustomerBillingAccount> accList)
        {
            List<CustomerBillingAccount> reAccountList = new List<CustomerBillingAccount>();
            if (accList != null && accList.Count > 0)
            {
                string excludeList = "";
                int i = 0;
                foreach (CustomerBillingAccount acc in accList)
                {
                    if (i == accList.Count - 1)
                    {
                        excludeList += "'" + acc.AccNum + "'";
                    }
                    else
                    {
                        excludeList += "'" + acc.AccNum + "',";
                    }
                    i++;
                }
                var db = DBHelper.GetSQLiteConnection();
                reAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND accNum NOT IN (" + excludeList + ") ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            }
            return reAccountList;
        }

        public static List<CustomerBillingAccount> NonNCAccountListExclude(List<CustomerBillingAccount> accList)
        {
            List<CustomerBillingAccount> ncAccountList = new List<CustomerBillingAccount>();
            if (accList != null && accList.Count > 0)
            {
                string excludeList = "";
                int i = 0;
                foreach (CustomerBillingAccount acc in accList)
                {
                    if (i == accList.Count - 1)
                    {
                        excludeList += "'" + acc.AccNum + "'";
                    }
                    else
                    {
                        excludeList += "'" + acc.AccNum + "',";
                    }
                    i++;
                }
                var db = DBHelper.GetSQLiteConnection();
                ncAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND accDesc LIKE ('Account%') AND accNum NOT IN (" + excludeList + ") ORDER BY CreatedDate ASC").ToList().OrderByDescending(x => x.CreatedDate).ToList();
            }
            return ncAccountList;
        }


        public static List<CustomerBillingAccount> NonREAccountList()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<CustomerBillingAccount> nonREAccountList = new List<CustomerBillingAccount>();
            nonREAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND accDesc NOT LIKE ('Account%') ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            return nonREAccountList;
        }

        public static List<CustomerBillingAccount> NCAccountList()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<CustomerBillingAccount> ncAccountList = new List<CustomerBillingAccount>();
            ncAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND accDesc LIKE ('Account%') ORDER BY CreatedDate ASC").ToList().OrderByDescending(x => x.CreatedDate).ToList();
            return ncAccountList;
        }


        public static List<CustomerBillingAccount> GetSortedCustomerBillingAccounts()
        {
            List<CustomerBillingAccount> sortedList = new List<CustomerBillingAccount>();
            UserEntity activeUser = UserEntity.GetActive();

            if (activeUser != null)
            {
                if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                {
                    sortedList.AddRange(AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV));
                    List<CustomerBillingAccount> excludeNonNCList = NonNCAccountListExclude(sortedList);
                    List<CustomerBillingAccount> excludeREList = REAccountListExclude(sortedList);
                    List<CustomerBillingAccount> excludeNonREList = NonREAccountListExclude(sortedList);
                    if (excludeREList != null && excludeREList.Count > 0)
                    {
                        sortedList.AddRange(excludeREList);
                    }
                    if (excludeNonREList != null && excludeNonREList.Count > 0)
                    {
                        sortedList.AddRange(excludeNonREList);
                    }
                }
                else
                {
                    sortedList.AddRange(NCAccountList());
                    sortedList.AddRange(REAccountList());
                    sortedList.AddRange(NonREAccountList());
                }
            }
            else
            {
                sortedList.AddRange(NCAccountList());
                sortedList.AddRange(REAccountList());
                sortedList.AddRange(NonREAccountList());
            }
            return sortedList;
        }

        public static bool HasOneItemOnly()
        {
            List<CustomerBillingAccount> sortedList = GetSortedCustomerBillingAccounts();

            return sortedList != null && sortedList.Count == 1;
        }

        public static List<CustomerBillingAccount> GetDefaultSortedCustomerBillingAccounts()
        {
            List<CustomerBillingAccount> sortedList = new List<CustomerBillingAccount>();
            sortedList.AddRange(NCAccountList());
            sortedList.AddRange(REAccountList());
            sortedList.AddRange(NonREAccountList());
            return sortedList;
        }

        public static List<CustomerBillingAccount> EligibleSMRAccountList()
        {
            List<CustomerBillingAccount> eligibleSMRAccounts = new List<CustomerBillingAccount>();

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> list = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);

                        eligibleSMRAccounts = list.FindAll(x => (x.AccountCategoryId != "2" && x.SmartMeterCode == "0" && !x.IsTaggedSMR && x.isOwned));
                        List<CustomerBillingAccount> existingList = EligibleSMRAccountListExclude(eligibleSMRAccounts);
                        if (existingList != null && existingList.Count > 0)
                        {
                            eligibleSMRAccounts.AddRange(existingList);
                        }

                        return eligibleSMRAccounts;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var db = DBHelper.GetSQLiteConnection();
            eligibleSMRAccounts = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND SmartMeterCode == '0' AND isTaggedSMR = 0 AND isOwned = 1").ToList().OrderBy(x => x.AccDesc).ToList();
            return eligibleSMRAccounts;
        }

        public static List<CustomerBillingAccount> EligibleSMRAccountListExclude(List<CustomerBillingAccount> accList)
        {
            List<CustomerBillingAccount> reAccountList = new List<CustomerBillingAccount>();
            if (accList != null && accList.Count > 0)
            {
                string excludeList = "";
                int i = 0;
                foreach (CustomerBillingAccount acc in accList)
                {
                    if (i == accList.Count - 1)
                    {
                        excludeList += "'" + acc.AccNum + "'";
                    }
                    else
                    {
                        excludeList += "'" + acc.AccNum + "',";
                    }
                    i++;
                }
                var db = DBHelper.GetSQLiteConnection();
                reAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND SmartMeterCode == '0' AND isTaggedSMR = 0 AND isOwned = 1 AND accNum NOT IN (" + excludeList + ") ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            }
            return reAccountList;
        }

        public static List<CustomerBillingAccount> SMAccountList()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<CustomerBillingAccount> eligibleSMAccounts = new List<CustomerBillingAccount>();
            eligibleSMAccounts = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE SmartMeterCode != '0'").ToList().OrderBy(x => x.AccDesc).ToList();
            return eligibleSMAccounts;
        }

        public static List<CustomerBillingAccount> GetEligibleAndSMRAccountList()
        {
            List<CustomerBillingAccount> eligibleSMRAccounts = new List<CustomerBillingAccount>();

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> list = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);

                        eligibleSMRAccounts = list.FindAll(x => (x.AccountCategoryId != "2" && x.SmartMeterCode == "0" && x.isOwned));
                        List<CustomerBillingAccount> existingList = GetEligibleAndSMRAccountListExclude(eligibleSMRAccounts);
                        if (existingList != null && existingList.Count > 0)
                        {
                            eligibleSMRAccounts.AddRange(existingList);
                        }
                        return eligibleSMRAccounts;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            var db = DBHelper.GetSQLiteConnection();
            eligibleSMRAccounts = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND SmartMeterCode == '0' AND isOwned = 1").ToList().OrderBy(x => !x.IsTaggedSMR).ToList();
            return eligibleSMRAccounts;
        }

        public static List<CustomerBillingAccount> GetEligibleAndSMRAccountListExclude(List<CustomerBillingAccount> accList)
        {
            List<CustomerBillingAccount> reAccountList = new List<CustomerBillingAccount>();
            if (accList != null && accList.Count > 0)
            {
                string excludeList = "";
                int i = 0;
                foreach (CustomerBillingAccount acc in accList)
                {
                    if (i == accList.Count - 1)
                    {
                        excludeList += "'" + acc.AccNum + "'";
                    }
                    else
                    {
                        excludeList += "'" + acc.AccNum + "',";
                    }
                    i++;
                }
                var db = DBHelper.GetSQLiteConnection();
                reAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND SmartMeterCode == '0' AND isOwned = 1 AND accNum NOT IN (" + excludeList + ") ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            }
            return reAccountList;
        }

        public static List<CustomerBillingAccount> CurrentSMRAccountList()
        {
            List<CustomerBillingAccount> eligibleSMRAccounts = new List<CustomerBillingAccount>();

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> list = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);

                        eligibleSMRAccounts = list.FindAll(x => (x.AccountCategoryId != "2" && x.SmartMeterCode == "0" && x.IsTaggedSMR && x.isOwned));
                        List<CustomerBillingAccount> existingList = CurrentSMRAccountListExclude(eligibleSMRAccounts);
                        if (existingList != null && existingList.Count > 0)
                        {
                            eligibleSMRAccounts.AddRange(existingList);
                        }
                        return eligibleSMRAccounts;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var db = DBHelper.GetSQLiteConnection();
            eligibleSMRAccounts = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND SmartMeterCode == '0' AND isTaggedSMR = 1 AND isOwned = 1").ToList().OrderBy(x => x.AccDesc).ToList();
            return eligibleSMRAccounts;
        }

        public static List<CustomerBillingAccount> CurrentSMRAccountListExclude(List<CustomerBillingAccount> accList)
        {
            List<CustomerBillingAccount> reAccountList = new List<CustomerBillingAccount>();
            if (accList != null && accList.Count > 0)
            {
                string excludeList = "";
                int i = 0;
                foreach (CustomerBillingAccount acc in accList)
                {
                    if (i == accList.Count - 1)
                    {
                        excludeList += "'" + acc.AccNum + "'";
                    }
                    else
                    {
                        excludeList += "'" + acc.AccNum + "',";
                    }
                    i++;
                }
                var db = DBHelper.GetSQLiteConnection();
                reAccountList = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 AND SmartMeterCode == '0' AND isTaggedSMR = 1 AND isOwned = 1 AND accNum NOT IN (" + excludeList + ") ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            }
            return reAccountList;
        }

        public static void MakeFirstAsSelected()
        {
            SetSelected(GetSortedCustomerBillingAccounts()[0].AccNum);
        }

        public static bool HasUpdatedBillingDetails(string accountNumber)
        {
            bool isUpdated = false;
            var db = DBHelper.GetSQLiteConnection();
            List<CustomerBillingAccount> customerBillingAccounts = db.Query<CustomerBillingAccount>("SELECT billingDetails FROM CustomerBillingAccountEntity WHERE accNum = ?", accountNumber);
            if (customerBillingAccounts.Count > 0)
            {
                isUpdated = customerBillingAccounts[0].billingDetails != null;
            }
            return isUpdated;
        }

        public static void UpdateIsSMROnboardingShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET IsSMROnboardingShown = 1");

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            foreach (CustomerBillingAccount acc in updatedList)
                            {
                                acc.IsSMROnboardingShown = true;
                            }
                            string updatedListString = JsonConvert.SerializeObject(updatedList);
                            AccountSortingEntity.InsertOrReplace(activeUser.Email, Constants.APP_CONFIG.ENV, updatedListString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static bool GetIsSMROnboardingShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            bool isShown = false;
            List<CustomerBillingAccount> customerBillingAccounts = db.Query<CustomerBillingAccount>("Select IsSMROnboardingShown from CustomerBillingAccountEntity");
            if (customerBillingAccounts.Count > 0)
            {
                isShown = customerBillingAccounts[0].IsSMROnboardingShown;
            }
            return isShown;
        }

        public static void SetIsSMRMeterReadingOnePhaseOnBoardShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET IsSMRMeterReadingOnBoardShown = 1");

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            foreach (CustomerBillingAccount acc in updatedList)
                            {
                                acc.IsSMRMeterReadingOnBoardShown = true;
                            }
                            string updatedListString = JsonConvert.SerializeObject(updatedList);
                            AccountSortingEntity.InsertOrReplace(activeUser.Email, Constants.APP_CONFIG.ENV, updatedListString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void UnSetIsSMRMeterReadingOnePhaseOnBoardShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET IsSMRMeterReadingOnBoardShown = 0");

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            foreach (CustomerBillingAccount acc in updatedList)
                            {
                                acc.IsSMRMeterReadingOnBoardShown = false;
                            }
                            string updatedListString = JsonConvert.SerializeObject(updatedList);
                            AccountSortingEntity.InsertOrReplace(activeUser.Email, Constants.APP_CONFIG.ENV, updatedListString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static bool GetIsSMRMeterReadingOnePhaseOnBoardShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            bool isShown = false;
            List<CustomerBillingAccount> customerBillingAccounts = db.Query<CustomerBillingAccount>("Select IsSMRMeterReadingOnBoardShown from CustomerBillingAccountEntity");
            if (customerBillingAccounts.Count > 0)
            {
                isShown = customerBillingAccounts[0].IsSMRMeterReadingOnBoardShown;
            }
            return isShown;
        }

        public static void SetIsSMRMeterReadingThreePhaseOnBoardShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET IsSMRMeterReadingThreePhaseOnBoardShown = 1");

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            foreach (CustomerBillingAccount acc in updatedList)
                            {
                                acc.IsSMRMeterReadingThreePhaseOnBoardShown = true;
                            }
                            string updatedListString = JsonConvert.SerializeObject(updatedList);
                            AccountSortingEntity.InsertOrReplace(activeUser.Email, Constants.APP_CONFIG.ENV, updatedListString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void UnSetIsSMRMeterReadingThreePhaseOnBoardShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET IsSMRMeterReadingThreePhaseOnBoardShown = 0");

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            foreach (CustomerBillingAccount acc in updatedList)
                            {
                                acc.IsSMRMeterReadingThreePhaseOnBoardShown = false;
                            }
                            string updatedListString = JsonConvert.SerializeObject(updatedList);
                            AccountSortingEntity.InsertOrReplace(activeUser.Email, Constants.APP_CONFIG.ENV, updatedListString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void SetIsSMRTakePhotoOnBoardShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update CustomerBillingAccountEntity SET IsSMRTakePhotoOnBoardShown = 1");

            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    if (AccountSortingEntity.HasItems(activeUser.Email, Constants.APP_CONFIG.ENV))
                    {
                        List<CustomerBillingAccount> updatedList = new List<CustomerBillingAccount>();
                        updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            foreach (CustomerBillingAccount acc in updatedList)
                            {
                                acc.IsSMRTakePhotoOnBoardShown = true;
                            }
                            string updatedListString = JsonConvert.SerializeObject(updatedList);
                            AccountSortingEntity.InsertOrReplace(activeUser.Email, Constants.APP_CONFIG.ENV, updatedListString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public static bool GetIsSMRMeterReadingThreePhaseOnBoardShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            bool isShown = false;
            List<CustomerBillingAccount> customerBillingAccounts = db.Query<CustomerBillingAccount>("Select IsSMRMeterReadingThreePhaseOnBoardShown from CustomerBillingAccountEntity");
            if (customerBillingAccounts.Count > 0)
            {
                isShown = customerBillingAccounts[0].IsSMRMeterReadingThreePhaseOnBoardShown;
            }
            return isShown;
        }

        public static bool GetIsSMRTakePhotoOnBoardShown()
        {
            var db = DBHelper.GetSQLiteConnection();
            bool isShown = false;
            List<CustomerBillingAccount> customerBillingAccounts = db.Query<CustomerBillingAccount>("Select IsSMRTakePhotoOnBoardShown from CustomerBillingAccountEntity");
            if (customerBillingAccounts.Count > 0)
            {
                isShown = customerBillingAccounts[0].IsSMRTakePhotoOnBoardShown;
            }
            return isShown;
        }

        public static bool GetIsPayBillEnabled()
        {
            bool enabled = false;

            if (NonREAccountList().Count > 0 || NCAccountList().Count > 0)
            {
                enabled = true;
            }

            return enabled;
        }

        public static bool GetIsViewBillEnabled()
        {
            bool enabled = false;

            if (HasItems())
            {
                enabled = true;
            }

            return enabled;
        }

        public static List<CustomerBillingAccount> SMeterBudgetAccountList()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<CustomerBillingAccount> eligibleSMAccounts = new List<CustomerBillingAccount>();
            eligibleSMAccounts = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE SmartMeterCode != '0' AND accountTypeId == 1 AND InstallationType != 25").ToList().OrderBy(x => x.AccDesc).ToList();
            return eligibleSMAccounts;
        }

        public static List<CustomerBillingAccount> SMeterBudgetAccountListALL()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<CustomerBillingAccount> eligibleSMAccounts = new List<CustomerBillingAccount>();
            eligibleSMAccounts = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE SmartMeterCode != '0'").ToList().OrderBy(x => x.AccDesc).ToList();
            return eligibleSMAccounts;
        }

        public static void UpdateEnergyBudgetRM(string TotalBudget, string accNum)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Update CustomerBillingAccountEntity SET BudgetAmount = ? WHERE accNum = ?", TotalBudget, accNum);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static List<CustomerBillingAccount> EnergyBudgetRM(string accNum)
        {
            var db = DBHelper.GetSQLiteConnection();
            List<CustomerBillingAccount> eligibleSMAccounts = new List<CustomerBillingAccount>();
            eligibleSMAccounts = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accNum = ?", accNum).ToList().OrderBy(x => x.AccDesc).ToList();
            return eligibleSMAccounts;
        }

        public static void SetCAListForEligibility()
        {
            List<CustomerBillingAccount> allAccountList = List();
            List<CACriteriaModel> criteriaModelList = new List<CACriteriaModel>();

            allAccountList.ForEach(account =>
            {
                CACriteriaModel criteriaModel = new CACriteriaModel();
                criteriaModel.CA = account.AccNum;
                criteriaModel.IsOwner = account.isOwned;
                criteriaModel.IsSmartMeter = account.IsSmartMeter;
                criteriaModel.IsNormalMeter = account.IsNormalMeter;
                criteriaModel.IsRenewableEnergy = account.IsREAccount;
                criteriaModel.IsSMR = account.IsSSMR;
                criteriaModelList.Add(criteriaModel);
            });
        }

        public static bool HasOwnerCA()
        {
            var eligibleCAs = DBRUtility.Instance.GetCAList();
            List<CustomerBillingAccount> allAccountList = List();
            List<CustomerBillingAccount> eligibleCAList = new List<CustomerBillingAccount>();

            eligibleCAs.ForEach(ca =>
            {
                var account = allAccountList.Find(item => item.AccNum == ca);
                if (account != null)
                {
                    eligibleCAList.Add(account);
                }
            });
            if (eligibleCAList.Count > 0)
            {
                return eligibleCAList.Any(s => s.isOwned);
            }
            else
            {
                return false;
            }
        }

        public static bool CAIsOwner(string ca)
        {
            List<CustomerBillingAccount> allAccountList = List();
            return allAccountList.Find(x => x.AccNum == ca).isOwned;
        }
    }
}