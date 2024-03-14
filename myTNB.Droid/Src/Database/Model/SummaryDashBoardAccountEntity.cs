using myTNB.Android.Src.SummaryDashBoard.Models;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.Database.Model
{
    [Table("SummaryDashBoardDetailsEntity")]
    public class SummaryDashBoardAccountEntity : AccountDataModel
    {
        public static void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SummaryDashBoardDetailsEntity");
            db.CreateTable<SummaryDashBoardAccountEntity>();
            //}
        }

        public static void InsertItem(SummaryDashBoardAccountEntity item)
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

        public void InsertListOfItems(List<AccountDataModel> itemList)
        {
            if (itemList != null)
            {
                foreach (AccountDataModel obj in itemList)
                {
                    SummaryDashBoardAccountEntity item = new SummaryDashBoardAccountEntity();
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
            SummaryDashBoardAccountEntity item = GetItemByAccountNo(accNo);
            if (item != null)
            {
                storedDate = item.Timestamp;
            }
            return storedDate;
        }

        public static String GetSMHistoryStoredData()
        {
            String response = null;
            List<SummaryDashBoardAccountEntity> items = GetAllItems();
            if (items != null && items.Count > 0)
            {
                SummaryDashBoardAccountEntity storedItem = items[0];
                response = storedItem.JsonResponse;
            }
            return response;
        }

        public static List<SummaryDashBoardAccountEntity> GetAllItems()
        {
            List<SummaryDashBoardAccountEntity> itemList = new List<SummaryDashBoardAccountEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SummaryDashBoardAccountEntity>("select * from SummaryDashBoardDetailsEntity");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static SummaryDashBoardAccountEntity GetItemByAccountNo(String accNo)
        {
            SummaryDashBoardAccountEntity entity = null;
            try
            {
                List<SummaryDashBoardAccountEntity> itemList = new List<SummaryDashBoardAccountEntity>();
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SummaryDashBoardAccountEntity>("select * from SummaryDashBoardDetailsEntity where AccountNo = ?", accNo);
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
                db.DeleteAll<SummaryDashBoardAccountEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=SummaryDashBoardDetailsEntity";
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
                db.Execute("DELETE FROM SummaryDashBoardDetailsEntity");
                //}
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
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM SummaryDashBoardDetailsEntity where AccountNo = ?", accNo);
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }


        public static bool UpdateNickName(string nickName, string accNo)
        {
            SummaryDashBoardAccountEntity summaryDashBoardEntity = SummaryDashBoardAccountEntity.GetItemByAccountNo(accNo);
            if (summaryDashBoardEntity != null)
            {
                SummaryDashBoardDetails accountDetails = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(summaryDashBoardEntity.JsonResponse);
                if (accountDetails != null)
                {
                    accountDetails.AccName = nickName;
                    summaryDashBoardEntity.JsonResponse = JsonConvert.SerializeObject(accountDetails);
                    InsertItem(summaryDashBoardEntity);
                    return true;
                }
            }
            return false;
        }

    }
}