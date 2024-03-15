using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("SMUsageHistoryEntity")]
    public class SMUsageHistoryEntity : SMUsageHistoryModel
    {
        public static void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SMUsageHistoryEntity");
            db.CreateTable<SMUsageHistoryEntity>();
            //}
        }

        public static void InsertItem(SMUsageHistoryEntity item)
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<SMUsageHistoryModel> itemList)
        {
            if (itemList != null)
            {
                foreach (SMUsageHistoryModel obj in itemList)
                {
                    SMUsageHistoryEntity item = new SMUsageHistoryEntity();
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
            SMUsageHistoryEntity item = GetItemByAccountNo(accNo);
            if (item != null)
            {
                storedDate = item.Timestamp;
            }
            return storedDate;
        }

        public static String GetSMHistoryStoredData()
        {
            String response = null;
            List<SMUsageHistoryEntity> items = GetAllItems();
            if (items != null && items.Count > 0)
            {
                SMUsageHistoryEntity storedItem = items[0];
                response = storedItem.JsonResponse;
            }
            return response;
        }

        public static List<SMUsageHistoryEntity> GetAllItems()
        {
            List<SMUsageHistoryEntity> itemList = new List<SMUsageHistoryEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SMUsageHistoryEntity>("select * from SMUsageHistoryEntity");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static SMUsageHistoryEntity GetItemByAccountNo(String accNo)
        {
            SMUsageHistoryEntity entity = null;
            try
            {
                List<SMUsageHistoryEntity> itemList = new List<SMUsageHistoryEntity>();
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SMUsageHistoryEntity>("select * from SMUsageHistoryEntity where AccountNo = ?", accNo);
                if (itemList != null && itemList.Count > 0)
                {
                    entity = itemList[0];
                }
                //}
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
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<SMUsageHistoryEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=SMUsageHistoryEntity";
            var cmd = connection.CreateCommand(cmdText, typeof(T).Name);
            return cmd.ExecuteScalar<string>() != null;
        }

        public static void RemoveAll()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM SMUsageHistoryEntity");
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static void RemoveAccountData(string accNo)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("DELETE FROM SMUsageHistoryEntity where AccountNo = ?", accNo);
            //}
        }

    }
}