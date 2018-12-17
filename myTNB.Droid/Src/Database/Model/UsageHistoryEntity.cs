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

namespace myTNB_Android.Src.Database.Model
{
    [Table("UsageHistoryEntity")]
    public class UsageHistoryEntity : UsageHistoryModel
    {
        public static void CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("UsageHistoryEntity");
            db.CreateTable<UsageHistoryEntity>();
        }

        public static void InsertItem(UsageHistoryEntity item)
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<UsageHistoryModel> itemList)
        {
            if (itemList != null)
            {
                foreach (UsageHistoryModel obj in itemList)
                {
                    UsageHistoryEntity item = new UsageHistoryEntity();
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
            if(storedDateTime.Date < DateTime.Now.Date)
                flag = true;
            else
                flag = false;

            return flag;
        }

        public static DateTime GetSMUsgaeHistoryStoredDate(string accNo)
        {
            DateTime storedDate = new DateTime(2050, 1, 1);
            UsageHistoryEntity item = GetItemByAccountNo(accNo);
            if (item != null)
            {
                storedDate = item.Timestamp;
            }            
            return storedDate;
        }

        public static String GetSMHistoryStoredData()
        {
            String response = null;
            List<UsageHistoryEntity> items = GetAllItems();
            if (items != null && items.Count > 0)
            {
                UsageHistoryEntity storedItem = items[0];
                response = storedItem.JsonResponse;
            }
            return response;
        }

        public static List<UsageHistoryEntity> GetAllItems()
        {
            List<UsageHistoryEntity> itemList = new List<UsageHistoryEntity>();
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<UsageHistoryEntity>("select * from UsageHistoryEntity");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static UsageHistoryEntity GetItemByAccountNo(String accNo)
        {
            UsageHistoryEntity entity = null;
            try
            {
                List<UsageHistoryEntity> itemList = new List<UsageHistoryEntity>();
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<UsageHistoryEntity>("select * from UsageHistoryEntity where AccountNo = ?", accNo);
                if(itemList != null && itemList.Count > 0)
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
                var db = new SQLiteConnection(Constants.DB_PATH);
                db.DeleteAll<UsageHistoryEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=UsageHistoryEntity";
            var cmd = connection.CreateCommand(cmdText, typeof(T).Name);
            return cmd.ExecuteScalar<string>() != null;
        }

        public static void RemoveAll()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM UsageHistoryEntity");
        }

        public static void RemoveAccountData(string accNo)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM UsageHistoryEntity where AccountNo = ?", accNo);
        }

    }
}