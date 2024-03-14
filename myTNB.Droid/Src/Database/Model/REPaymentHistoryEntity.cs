using myTNB.Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.Database.Model
{
    [Table("REPaymentHistoryEntity")]
    public class REPaymentHistoryEntity : REPaymentHistoryModel
    {
        public static void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("REPaymentHistoryEntity");
            db.CreateTable<REPaymentHistoryEntity>();
            //}
        }

        public static void InsertItem(REPaymentHistoryEntity item)
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

        public void InsertListOfItems(List<REPaymentHistoryModel> itemList)
        {
            if (itemList != null)
            {
                foreach (REPaymentHistoryModel obj in itemList)
                {
                    REPaymentHistoryEntity item = new REPaymentHistoryEntity();
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
            REPaymentHistoryEntity item = GetItemByAccountNo(accNo);
            if (item != null)
            {
                storedDate = item.Timestamp;
            }
            return storedDate;
        }

        public static String GetSMHistoryStoredData()
        {
            String response = null;
            List<REPaymentHistoryEntity> items = GetAllItems();
            if (items != null && items.Count > 0)
            {
                REPaymentHistoryEntity storedItem = items[0];
                response = storedItem.JsonResponse;
            }
            return response;
        }

        public static List<REPaymentHistoryEntity> GetAllItems()
        {
            List<REPaymentHistoryEntity> itemList = new List<REPaymentHistoryEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<REPaymentHistoryEntity>("select * from REPaymentHistoryEntity");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static REPaymentHistoryEntity GetItemByAccountNo(String accNo)
        {
            REPaymentHistoryEntity entity = null;
            try
            {
                List<REPaymentHistoryEntity> itemList = new List<REPaymentHistoryEntity>();
                //using (var db = new SQLiteConnection(Constants.DB_PATH)){
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<REPaymentHistoryEntity>("select * from REPaymentHistoryEntity where AccountNo = ?", accNo);
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
                db.DeleteAll<REPaymentHistoryEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=REPaymentHistoryEntity";
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
                db.Execute("DELETE FROM REPaymentHistoryEntity");
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
            db.Execute("DELETE FROM REPaymentHistoryEntity where AccountNo = ?", accNo);
            //}
        }

    }
}