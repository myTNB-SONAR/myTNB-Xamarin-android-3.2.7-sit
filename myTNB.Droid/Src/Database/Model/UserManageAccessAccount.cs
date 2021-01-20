using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.SummaryDashBoard.Models;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using myTNB_Android.Src.MyTNBService.Response;

namespace myTNB_Android.Src.Database.Model
{
    [Table("UserManageAccountEntity")]
    public class UserManageAccessAccount
    {
        [Column("accNum")]
        public string AccNum { get; set; }

        [Column("accDesc")]
        public string AccDesc { get; set; }

        [PrimaryKey, Column("userAccountID")]
        public string UserAccountId { get; set; }

        [Column("IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

        [Column("IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [Column("IsOwnedAccount")]
        public bool IsOwnedAccount { get; set; }

        [Column("IsPreRegister")]
        public bool IsPreRegister { get; set; }

        [Column("email")]
        public string email { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("isSelected")]
        public bool isSelected { get; set; }

        [Column("userId")]
        public string userId { get; set; }
        
        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<UserManageAccessAccount>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<UserManageAccessAccount>();
        }

        public static int InsertOrReplace(UserManageAccessAccount accountResponse, bool isSelected)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new UserManageAccessAccount()
            {
                AccNum = accountResponse.AccNum,
                AccDesc = string.IsNullOrEmpty(accountResponse.AccDesc) == true ? "--" : accountResponse.AccDesc,
                UserAccountId = accountResponse.UserAccountId,
                IsApplyEBilling = accountResponse.IsApplyEBilling,
                IsHaveAccess = accountResponse.IsHaveAccess,
                IsOwnedAccount = accountResponse.IsOwnedAccount,
                IsPreRegister = accountResponse.IsPreRegister,
                email = accountResponse.email,
                name = accountResponse.name,
                userId = accountResponse.userId,
                isSelected = false
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }


        public static int InsertOrReplace(Account accountResponse, bool isSelected)
        {
            var db = DBHelper.GetSQLiteConnection();

            var newRecord = new UserManageAccessAccount()
            {
                /*Type = accountResponse.Type,
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
                isOwned = accountResponse.IsOwned,*/
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(ManageAccessAccountListResponse.CustomerAccountData accountResponse)
        {
            var db = DBHelper.GetSQLiteConnection();

            var newRecord = new UserManageAccessAccount()
            {
                AccNum = accountResponse.AccountNumber,
                AccDesc = accountResponse.AccountDescription,
                UserAccountId = accountResponse.AccountId,
                IsApplyEBilling = accountResponse.IsApplyEBilling,
                IsHaveAccess = accountResponse.IsHaveAccess,
                IsOwnedAccount = accountResponse.IsOwnedAccount,
                IsPreRegister = accountResponse.IsPreRegister,
                email = accountResponse.Email,
                name = accountResponse.Name,
                userId = accountResponse.UserId,
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(UserManageAccessAccount account)
        {

            var db = DBHelper.GetSQLiteConnection();

            int newRecordRow = db.InsertOrReplace(account);

            return newRecordRow;

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
                        List<UserManageAccessAccount> updatedList = new List<UserManageAccessAccount>();
                        /*updatedList = AccountSortingEntity.List(activeUser.Email, Constants.APP_CONFIG.ENV);
                        if (updatedList != null && updatedList.Count > 0)
                        {
                            UserManageAccessAccount selected = updatedList.Find(x => x.IsSelected);

                            if (selected != null)
                            {
                                return true;
                            }
                        }*/
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var db = DBHelper.GetSQLiteConnection();
            return db.Query<UserManageAccessAccount>("SELECT * FROM UserManageAccountEntity WHERE isSelected = ?", true).Count > 0;
        }

        public static bool HasItems()
        {
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    List<UserManageAccessAccount> updatedList = new List<UserManageAccessAccount>();
                    if (updatedList != null && updatedList.Count > 0)
                    {
                      return updatedList.Count > 0;
                    }
                    
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var db = DBHelper.GetSQLiteConnection();
            return db.Query<UserManageAccessAccount>("SELECT * FROM UserManageAccountEntity").Count > 0;
        }

        public static UserManageAccessAccount GetSelected()
        {
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var db = DBHelper.GetSQLiteConnection();
            return db.Query<UserManageAccessAccount>("SELECT * FROM UserManageAccountEntity WHERE isSelected = ?", true).ToList()[0];
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

                db.Execute("Delete from UserManageAccountEntity ");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static void Remove(string AccountNum, string userId)
        {
            
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    var db = DBHelper.GetSQLiteConnection();
                    db.Execute("Delete from UserManageAccountEntity WHERE accNum = ? AND userId = ?", AccountNum, userId);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void UpdateManageAccess(string AccountNum, string userAccountId, bool haveAccess, bool haveEBiling)
        {          
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    var db = DBHelper.GetSQLiteConnection();
                    db.Execute("Update UserManageAccountEntity SET IsHaveAccess = ?, IsApplyEBilling = ? WHERE userAccountID = ? AND accNum = ?", haveAccess, haveEBiling, userAccountId, AccountNum);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void SetSelected(string accNum, bool flag, string accountId)
        {
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    var db = DBHelper.GetSQLiteConnection();
                    db.Execute("Update UserManageAccountEntity SET isSelected = ? WHERE accNum = ? AND userAccountID = ?", flag, accNum, accountId);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static UserManageAccessAccount FindByAccNum(string accNum)
        {
            var db = DBHelper.GetSQLiteConnection();
            var record = db.Query<UserManageAccessAccount>("SELECT * FROM UserManageAccountEntity WHERE accNum =?", accNum);

            if (record != null && record.Count > 0)
            {
                return record[0];
            }
            return null;
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

        public static List<UserManageAccessAccount> List()
        {
            List<UserManageAccessAccount> sortedList = new List<UserManageAccessAccount>();
            //List<UserManageAccessAccount> excludeNonREList = NonREAccountListExclude(sortedList);
            sortedList.AddRange(NonREAccountList());

            return sortedList;
        }

        public static List<UserManageAccessAccount> List(string accNUm)
        {
            List<UserManageAccessAccount> sortedList = new List<UserManageAccessAccount>();
            sortedList.AddRange(NonREAccountList(accNUm));

            return sortedList;
        }

        public static List<UserManageAccessAccount> ListIsSelected(string accNUm)
        {
            List<UserManageAccessAccount> sortedList = new List<UserManageAccessAccount>();
            sortedList.AddRange(SelectedAccountList(accNUm));

            return sortedList;
        }

        public static List<UserManageAccessAccount> NonREAccountListExclude(List<UserManageAccessAccount> accList)
        {
            List<UserManageAccessAccount> reAccountList = new List<UserManageAccessAccount>();
            if (accList != null && accList.Count > 0)
            {
                string excludeList = "";
                int i = 0;
                foreach (UserManageAccessAccount acc in accList)
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
                reAccountList = db.Query<UserManageAccessAccount>("SELECT * FROM UserManageAccountEntity WHERE userAccountID != null ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            }
            return reAccountList;
        }

        public static List<UserManageAccessAccount> NonREAccountList()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<UserManageAccessAccount> nonREAccountList = new List<UserManageAccessAccount>();
            nonREAccountList = db.Query<UserManageAccessAccount>("SELECT * FROM UserManageAccountEntity WHERE userId != 0 ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
            return nonREAccountList;
        }

        public static List<UserManageAccessAccount> NonREAccountList(string accNUm)
        {
            var db = DBHelper.GetSQLiteConnection();
            List<UserManageAccessAccount> nonREAccountList = new List<UserManageAccessAccount>();
            nonREAccountList = db.Query<UserManageAccessAccount>("SELECT * FROM UserManageAccountEntity WHERE accNum = ? AND IsOwnedAccount = ? ORDER BY accDesc ASC", accNUm, false).ToList().OrderBy(x => x.AccDesc).ToList();
            return nonREAccountList;
        }

        public static List<UserManageAccessAccount> SelectedAccountList(string accNUm)
        {
            var db = DBHelper.GetSQLiteConnection();
            List<UserManageAccessAccount> selectedList = new List<UserManageAccessAccount>();
            selectedList = db.Query<UserManageAccessAccount>("SELECT * FROM UserManageAccountEntity WHERE accNum = ? AND isSelected = ? ORDER BY accDesc ASC", accNUm, true).ToList().OrderBy(x => x.AccDesc).ToList();
            return selectedList;
        }

        public static void DeleteSelected(string accNUm)
        {
            try
            {
                UserEntity activeUser = UserEntity.GetActive();
                if (activeUser != null)
                {
                    var db = DBHelper.GetSQLiteConnection();
                    db.Execute("Delete from UserManageAccountEntity WHERE accNum = ? AND isSelected = ?", accNUm, true);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void UnSelectAll()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                db.Execute("UPDATE UserManageAccountEntity SET IsSelected = ?", false);
            }
        }

        public static int SelectItem(string id)
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Execute("UPDATE UserManageAccountEntity SET IsSelected = ? WHERE userId = ?", true, id);
            }
        }
    }
}
