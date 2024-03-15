using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Database;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class TimeStampEntity : TimestampModel
    {
        public void CreateTable()
        {
            //using(var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("TimeStampEntity");
            db.CreateTable<TimeStampEntity>();
            //}
        }

        public void InsertItem(TimeStampEntity item)
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

        public void InsertListOfItems(List<TimestampModel> itemList)
        {
            if (itemList != null)
            {
                foreach (TimestampModel obj in itemList)
                {
                    TimeStampEntity item = new TimeStampEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<TimeStampEntity> GetAllItems()
        {
            List<TimeStampEntity> itemList = new List<TimeStampEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<TimeStampEntity>("select * from TimeStampEntity");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public void DeleteTable()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<TimeStampEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=TimeStampEntity";
            var cmd = connection.CreateCommand(cmdText, typeof(T).Name);
            return cmd.ExecuteScalar<string>() != null;
        }
    }
}