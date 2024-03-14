using myTNB.Android.Src.AddAccount.Models;
using myTNB.Android.Src.SummaryDashBoard.Models;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.Android.Src.Database.Model
{
    [Table("AccountSortingEntity")]
    public class AccountSortingEntity
    {
        [PrimaryKey, Column("EmailAddress")]
        public string EmailAddress { get; set; }

        [Column("Environment")]
        public string Environment { get; set; }

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

        public static int InsertOrReplace(string emailAddress, string env
            , string accountList)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new AccountSortingEntity()
            {
                EmailAddress = emailAddress,
                Environment = env,
                AccountList = accountList
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static void InsertOrReplaceSpecificAccount(string emailAddress, string env
            , CustomerBillingAccount addAcc)
        {
            List<CustomerBillingAccount> existingList = List(emailAddress, env);

            if (existingList != null && existingList.Count > 0 && addAcc != null && !string.IsNullOrEmpty(addAcc.AccNum))
            {
                int index = existingList.FindIndex(x => x.AccNum == addAcc.AccNum);
                if (index == -1)
                {
                    existingList.Add(addAcc);

                    InsertOrReplace(emailAddress, env, existingList);
                }
                else if (index != -1)
                {
                    existingList[index] = addAcc;

                    InsertOrReplace(emailAddress, env, existingList);
                }
            }

        }

        public static void ReplaceSpecificAccount(string emailAddress, string env
                    , CustomerBillingAccount addAcc)
        {
            List<CustomerBillingAccount> existingList = List(emailAddress, env);

            if (existingList != null && existingList.Count > 0 && addAcc != null && !string.IsNullOrEmpty(addAcc.AccNum))
            {
                int index = existingList.FindIndex(x => x.AccNum == addAcc.AccNum);
                if (index != -1)
                {
                    existingList[index] = addAcc;

                    InsertOrReplace(emailAddress, env, existingList);
                }
            }

        }

        public static void RemoveSpecificAccount(string emailAddress, string env
            , string accNum)
        {
            List<CustomerBillingAccount> existingList = List(emailAddress, env);

            if (existingList != null && existingList.Count > 0 && !string.IsNullOrEmpty(accNum))
            {
                int index = existingList.FindIndex(x => x.AccNum == accNum);
                if (index != -1)
                {
                    existingList.RemoveAt(index);

                    InsertOrReplace(emailAddress, env, existingList);
                }
            }

        }

        public static void RemoveSpecificAccountSorting(string emailAddress, string env)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from CustomerBillingAccountEntity WHERE EmailAddress = ? AND Environment = ?", emailAddress, env);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public static int InsertOrReplace(string emailAddress, string env
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
                    Environment = env,
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

        public static List<CustomerBillingAccount> List(string email, string env)
        {
            List<CustomerBillingAccount> customerAccounts = new List<CustomerBillingAccount>();

            if (HasItems(email, env))
            {
                try
                {
                    customerAccounts = ReturnList(email, env);
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
                    customerAccounts = CustomerBillingAccount.GetDefaultSortedCustomerBillingAccounts();
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

        public static List<CustomerBillingAccount> GetRearrangeList(string email, string env)
        {
            List<CustomerBillingAccount> customerAccounts = new List<CustomerBillingAccount>();

            if (HasItems(email, env))
            {
                try
                {
                    customerAccounts = ReturnList(email, env);
                    List<CustomerBillingAccount> excludeNonNCList = CustomerBillingAccount.NonNCAccountListExclude(customerAccounts);
                    List<CustomerBillingAccount> excludeREList = CustomerBillingAccount.REAccountListExclude(customerAccounts);
                    List<CustomerBillingAccount> excludeNonREList = CustomerBillingAccount.NonREAccountListExclude(customerAccounts);
                    if (excludeREList != null && excludeREList.Count > 0)
                    {
                        customerAccounts.AddRange(excludeREList);
                    }
                    if (excludeNonREList != null && excludeNonREList.Count > 0)
                    {
                        customerAccounts.AddRange(excludeNonREList);
                    }
                    if (excludeNonNCList != null && excludeNonNCList.Count > 0)
                    {
                        customerAccounts.AddRange(excludeNonNCList);
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
                    customerAccounts = CustomerBillingAccount.GetDefaultSortedCustomerBillingAccounts();
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

        public static bool HasItems(string email, string env)
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<AccountSortingEntity>("SELECT * FROM AccountSortingEntity WHERE EmailAddress = ? AND Environment = ?", email, env).Count > 0;
        }

        private static List<CustomerBillingAccount> ReturnList(string email, string env)
        {
            var db = DBHelper.GetSQLiteConnection();

            List<CustomerBillingAccount> customerAccounts = new List<CustomerBillingAccount>();

            AccountSortingEntity item = db.Query<AccountSortingEntity>("SELECT * FROM AccountSortingEntity WHERE EmailAddress = ? AND Environment = ?", email, env).ToList()[0];

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
                                customerAccounts = CustomerBillingAccount.GetDefaultSortedCustomerBillingAccounts();
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
                        customerAccounts = CustomerBillingAccount.GetDefaultSortedCustomerBillingAccounts();
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
                    customerAccounts = CustomerBillingAccount.GetDefaultSortedCustomerBillingAccounts();
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
