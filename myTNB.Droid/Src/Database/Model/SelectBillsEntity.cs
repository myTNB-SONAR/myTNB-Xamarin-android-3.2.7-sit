using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("SelectBillsEntity")]
    public class SelectBillsEntity : SelectBillsModel
    {
        public static void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SelectBillsEntity");
            db.CreateTable<SelectBillsEntity>();
            //}
        }

        public static void InsertItem(SelectBillsEntity item)
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

        public void InsertListOfItems(List<SelectBillsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (SelectBillsModel obj in itemList)
                {
                    SelectBillsEntity item = new SelectBillsEntity();
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
            SelectBillsEntity item = GetItemByAccountNo(accNo);
            if (item != null)
            {
                storedDate = item.Timestamp;
            }
            return storedDate;
        }

        public static String GetSMHistoryStoredData()
        {
            String response = null;
            List<SelectBillsEntity> items = GetAllItems();
            if (items != null && items.Count > 0)
            {
                SelectBillsEntity storedItem = items[0];
                response = storedItem.JsonResponse;
            }
            return response;
        }

        public static List<SelectBillsEntity> GetAllItems()
        {
            List<SelectBillsEntity> itemList = new List<SelectBillsEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SelectBillsEntity>("select * from SelectBillsEntity");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static SelectBillsEntity GetItemByAccountNo(String accNo)
        {
            SelectBillsEntity entity = null;
            try
            {
                List<SelectBillsEntity> itemList = new List<SelectBillsEntity>();
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SelectBillsEntity>("select * from SelectBillsEntity where AccountNo = ?", accNo);
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
                db.DeleteAll<SelectBillsEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=SelectBillsEntity";
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
                db.Execute("DELETE FROM SelectBillsEntity");
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static void RemoveAccountData(string accNo)
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                db.Execute("DELETE FROM SelectBillsEntity where AccountNo = ?", accNo);
            }
        }

    }
}