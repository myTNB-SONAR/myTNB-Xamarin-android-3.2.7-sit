using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.Database.Model
{
    [Table("AccountSortingEntity")]
    public class AccountSortingEntity
    {
        [PrimaryKey, Column("EmailAddress")]
        public string EmailAddress { get; set; }

        [Column("AccountList")]
        public string AccountList { get; set; }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<AccountSortingEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<AccountSortingEntity>();
        }

        public static int InsertOrReplace(string emailAddress
            , string accountList)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new AccountSortingEntity()
            {
                EmailAddress = emailAddress,
                AccountList = accountList
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(string emailAddress
                , List<CustomerBillingAccount> customerBillingAccounts)
        {
            int newRecordRow = 0;
            try
            {
                var db = DBHelper.GetSQLiteConnection();

                string accountList = JsonConvert.SerializeObject(customerBillingAccounts);

                var newRecord = new AccountSortingEntity()
                {
                    EmailAddress = emailAddress,
                    AccountList = accountList
                };

                newRecordRow = db.InsertOrReplace(newRecord);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return newRecordRow;
        }

        public static List<CustomerBillingAccount> List(string email)
        {
            List<CustomerBillingAccount> customerAccounts = new List<CustomerBillingAccount>();

            if (HasItems(email))
            {
                try
                {
                    customerAccounts = ReturnList(email);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            else
            {
                try
                {
                    customerAccounts = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                    if (customerAccounts == null)
                    {
                        customerAccounts = new List<CustomerBillingAccount>();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            return customerAccounts;
        }

        private static bool HasItems(string email)
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<AccountSortingEntity>("SELECT * FROM AccountSortingEntity WHERE EmailAddress = ?", email).Count > 0;
        }

        private static List<CustomerBillingAccount> ReturnList(string email)
        {
            var db = DBHelper.GetSQLiteConnection();

            List<CustomerBillingAccount> customerAccounts = new List<CustomerBillingAccount>();

            AccountSortingEntity item = db.Query<AccountSortingEntity>("SELECT * FROM AccountSortingEntity WHERE EmailAddress = ?", email).ToList()[0];

            if (item != null)
            {
                string accountList = item.AccountList;
                if (!string.IsNullOrEmpty(accountList))
                {
                    try
                    {
                        customerAccounts = JsonConvert.DeserializeObject<List<CustomerBillingAccount>>(accountList);
                        if (customerAccounts == null)
                        {
                            try
                            {
                                customerAccounts = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                                if (customerAccounts == null)
                                {
                                    customerAccounts = new List<CustomerBillingAccount>();
                                }
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
                else
                {
                    try
                    {
                        customerAccounts = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                        if (customerAccounts == null)
                        {
                            customerAccounts = new List<CustomerBillingAccount>();
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }
            else
            {
                try
                {
                    customerAccounts = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                    if (customerAccounts == null)
                    {
                        customerAccounts = new List<CustomerBillingAccount>();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            return customerAccounts;
        }
    }
}
