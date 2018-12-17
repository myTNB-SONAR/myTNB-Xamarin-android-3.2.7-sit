using SQLite;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using System;
using myTNB_Android.Src.Utils;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class TimeStampEntity : TimestampModel
    {
        public void CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("TimeStampEntity");
            db.CreateTable<TimeStampEntity>();
        }

        public void InsertItem(TimeStampEntity item)
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord );
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<TimestampModel> itemList)
        {
            if(itemList != null){
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
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<TimeStampEntity>("select * from TimeStampEntity");
            }catch(Exception e){
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public void DeleteTable(){
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                db.DeleteAll<TimeStampEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}",e.Message);
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