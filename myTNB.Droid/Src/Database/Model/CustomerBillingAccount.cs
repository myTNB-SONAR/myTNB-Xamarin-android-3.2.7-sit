using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.AddAccount.Models;

namespace myTNB_Android.Src.Database.Model
{
    [Table("CustomerBillingAccountEntity")]
    public class CustomerBillingAccount
    {
        //"__type": "Billing.BillingRegListSSP",
        //   "accNum": "220163099904",
        //   "userAccountID": null,
        //   "accDesc": "MANUFACTURE SDN. BHD. BEST FORM BRAKE",
        //   "icNum": null,
        //   "amCurrentChg": 4710.4,
        //   "isRegistered": "False",
        //   "isPaid": "False",
        [Column("__type")]
        public string Type { get; set; }

        [PrimaryKey , Column("accNum")]
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

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<CustomerBillingAccount>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<CustomerBillingAccount>();
        }

        /// <summary>
        /// Insert or Replace 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="accNum"></param>
        /// <param name="accDesc"></param>
        /// <param name="userAccountID"></param>
        /// <param name="icNum"></param>
        /// <param name="amtCurrentChg"></param>
        /// <param name="isRegistered"></param>
        /// <param name="isPaid"></param>
        /// <param name="isSelected"></param>
        /// <param name="smartMeterCode"></param>
        /// <returns>Rows changed</returns>
        public static int InsertOrReplace(string type 
            , string accNum
            , string accDesc
            , string userAccountID
            , string icNum 
            , string amtCurrentChg
            , bool isRegistered
            , bool isPaid
            , string accountTypeId
            , string accountStAddress
            , string ownerName
            , string accountCategoryId
            , string smartMeterCode
            , bool isSelected)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new CustomerBillingAccount()
            {
                Type = type,
                AccNum = accNum,
                AccDesc = accDesc,
                UserAccountId = userAccountID,
                ICNum = icNum,
                AmtCurrentChg = amtCurrentChg,
                IsRegistered = isRegistered,
                IsPaid = isPaid,
                AccountTypeId = accountTypeId,
                AccountStAddress = accountStAddress,
                OwnerName = ownerName,
                AccountCategoryId = accountCategoryId,
                SmartMeterCode = smartMeterCode == null ? "0" : smartMeterCode,
                IsSelected = isSelected
            };

            int newRecordRow = db.InsertOrReplace(newRecord);
   
            return newRecordRow;
        }


        public static int InsertOrReplace(Account accountResponse)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
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
                isOwned = accountResponse.IsOwned,
                AccountTypeId = accountResponse.AccountTypeId,
                AccountStAddress = accountResponse.AccountStAddress,
                OwnerName = accountResponse.OwnerName,
                AccountCategoryId = accountResponse.AccountCategoryId,
                SmartMeterCode = accountResponse.SmartMeterCode == null ? "0" : accountResponse.SmartMeterCode,
                IsSelected = false
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(NewAccount accountResponse , bool isSelected)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
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
                AccountTypeId = accountResponse.accountTypeId,
                AccountStAddress = accountResponse.accountAddress,
                OwnerName = accountResponse.ownerName,
                AccountCategoryId = accountResponse.accountCategoryId,
                SmartMeterCode = accountResponse.smartMeterCode == null ? "0" : accountResponse.smartMeterCode,
                IsSelected = isSelected
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }


        public static int InsertOrReplace(Account accountResponse , bool isSelected)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
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
                isOwned = accountResponse.IsOwned
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static IEnumerable<CustomerBillingAccount> Enumerate()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity");
        }



        public static List<CustomerBillingAccount> List()
        {
            /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
            // return Enumerate().ToList();
            /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
            List<CustomerBillingAccount> ReAccount = REAccountList();
            List<CustomerBillingAccount> NonReAccount = NonREAccountList();

            List<CustomerBillingAccount> customerAccounts = new List<CustomerBillingAccount>();
            if (ReAccount != null && ReAccount.Count() > 0) {
                customerAccounts.AddRange(ReAccount.OrderBy(x => x.AccDesc).ToList());
            }

            if (NonReAccount != null && NonReAccount.Count() > 0)
            {
                customerAccounts.AddRange(NonReAccount.OrderBy(x => x.AccDesc).ToList());
            }

            return customerAccounts;
        }

        public static bool HasSelected()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE isSelected = ?", true).Count > 0;
        }

        public static bool HasItems()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity ").Count > 0;
        }

        public static CustomerBillingAccount GetSelected()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE isSelected = ?", true).ToList()[0];
        }

        public static CustomerBillingAccount GetFirst()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity", true).ToList()[0];
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

        public static int RemoveSelected()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Execute("UPDATE CustomerBillingAccountEntity SET isSelected = ? WHERE isSelected = ?" , false , true);
        }

        public static int Update(string accNum , bool isSelected)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var existingRecord = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accNum = ? " , accNum);

            if (existingRecord != null && existingRecord.Count > 0)
            {
                var customerBARecord = existingRecord[0];
                customerBARecord.IsSelected = isSelected;
                return db.Update(customerBARecord);
            }

            return 0;
        }
        public static int RemoveActive()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Execute("Delete from CustomerBillingAccountEntity ");
        }

        public static int Remove(string AccountNum)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Execute("Delete from CustomerBillingAccountEntity WHERE accNum = ?", AccountNum);
        }

        public static int UpdateAccountName(string newAccountName , string accNum)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Execute("Update CustomerBillingAccountEntity SET accDesc = ? WHERE accNum = ?", newAccountName, accNum);
        }

        public static void SetSelected(string accNum)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("Update CustomerBillingAccountEntity SET IsSelected = ? WHERE accNum = ?", true, accNum);
        }

        public static CustomerBillingAccount FindByAccNum(string accNum)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var record = db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accNum =?" , accNum);

            if (record != null && record.Count > 0)
            {
                return record[0];
            }
            return null;
        }


        public static List<CustomerBillingAccount> REAccountList()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId = 2 ORDER BY accDesc ASC").ToList().OrderBy(x=>x.AccDesc).ToList();
        }


        public static List<CustomerBillingAccount> NonREAccountList()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<CustomerBillingAccount>("SELECT * FROM CustomerBillingAccountEntity WHERE accountCategoryId != 2 ORDER BY accDesc ASC").ToList().OrderBy(x => x.AccDesc).ToList();
        }

        public static void MakeFirstAsSelected() {
            List<CustomerBillingAccount> ReAccount = REAccountList();
            if (ReAccount != null && ReAccount.Count() > 0) {
                SetSelected(ReAccount[0].AccNum);
            } else {
                List<CustomerBillingAccount> NonReAccount = NonREAccountList();
                if (NonReAccount != null && NonReAccount.Count() > 0)
                {
                    SetSelected(NonReAccount[0].AccNum);
                }
            }


        }

    }
}