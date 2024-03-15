using myTNB.AndroidApp.Src.AddAccount.Models;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("AccountDataEntity")]
    public class AccountDataEntity : AccountDataModel
    {
        public static void CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("AccountDataEntity");
            db.CreateTable<AccountDataEntity>();
        }

        public static void InsertItem(AccountDataEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.InsertOrReplace(item);
            }
            catch (Exception e)
            {

                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<AccountDataModel> itemList)
        {
            if (itemList != null)
            {
                foreach (AccountDataModel obj in itemList)
                {
                    AccountDataEntity item = new AccountDataEntity();
                    item.Timestamp = obj.Timestamp;
                    item.JsonResponse = obj.JsonResponse;
                    item.AccountNo = obj.AccountNo;
                    InsertItem(item);
                }
            }
        }

        public static bool IsSMDataUpdated(string accNo)
        {
            bool flag = false;
            DateTime storedDateTime = GetSMUsgaeHistoryStoredDate(accNo);
            if (storedDateTime.Date < DateTime.Now.Date)
                flag = true;
            else
                flag = false;

            return flag;
        }

        public static DateTime GetSMUsgaeHistoryStoredDate(string accNo)
        {
            DateTime storedDate = new DateTime(2050, 1, 1);
            AccountDataEntity item = GetItemByAccountNo(accNo);
            if (item != null)
            {
                storedDate = item.Timestamp;
            }
            return storedDate;
        }

        public static String GetSMHistoryStoredData()
        {
            String response = null;
            List<AccountDataEntity> items = GetAllItems();
            if (items != null && items.Count > 0)
            {
                AccountDataEntity storedItem = items[0];
                response = storedItem.JsonResponse;
            }
            return response;
        }

        public static List<AccountDataEntity> GetAllItems()
        {
            List<AccountDataEntity> itemList = new List<AccountDataEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<AccountDataEntity>("select * from AccountDataEntity");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static AccountDataEntity GetItemByAccountNo(String accNo)
        {
            AccountDataEntity entity = null;
            try
            {
                List<AccountDataEntity> itemList = new List<AccountDataEntity>();
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<AccountDataEntity>("select * from AccountDataEntity where AccountNo = ?", accNo);
                if (itemList != null && itemList.Count > 0)
                {
                    entity = itemList[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return entity;
        }

        public void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<AccountDataEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=AccountDataEntity";
            var cmd = connection.CreateCommand(cmdText, typeof(T).Name);
            return cmd.ExecuteScalar<string>() != null;
        }

        public static void RemoveAll()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM AccountDataEntity");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static void RemoveAccountData(string accNo)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM AccountDataEntity where AccountNo = ?", accNo);
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }


        public static bool UpdateNickName(string nickName, string accNo)
        {
            try
            {
                AccountDataEntity accountDataEntity = GetItemByAccountNo(accNo);

                if (accountDataEntity != null)
                {
                    AccountDetailsResponse customerBillingDetails = JsonConvert.DeserializeObject<AccountDetailsResponse>(accountDataEntity.JsonResponse);
                    customerBillingDetails.Data.AccountData.AccountName = nickName;
                    accountDataEntity.JsonResponse = JsonConvert.SerializeObject(customerBillingDetails);
                    InsertItem(accountDataEntity);
                    return true;
                }
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            return false;
        }

    }
}