using System;
using System.Collections.Generic;
using myTNB_Android.Src.SitecoreCMS.Model;
using Newtonsoft.Json;
using SQLite;

namespace myTNB_Android.Src.Database.Model
{
    public class SitecoreCmsEntity
    {
        [PrimaryKey, Column("itemId")]
        public string itemId { get; set; }

        [Column("jsonStringData")]
        public string jsonStringData { get; set; }

        [Column("jsonTimeStampData")]
        public string jsonTimeStampData { get; set; }

        public static void CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SitecoreCmsEntity");
            db.CreateTable<SitecoreCmsEntity>();
        }

        public static void InsertItem(SitecoreCmsEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public static void InsertListOfItems(string itemId, string jsonStringDataList)
        {
            SitecoreCmsEntity item = new SitecoreCmsEntity();
            item.itemId = itemId;
            item.jsonStringData = jsonStringDataList;
            InsertItem(item);
        }

        public static List<SitecoreCmsEntity> GetItemById(string id)
        {
            List<SitecoreCmsEntity> itemList = new List<SitecoreCmsEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SitecoreCmsEntity>("select * from SitecoreCmsEntity WHERE itemId = ?",id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static List<SitecoreCmsEntity> GetAllItems()
        {
            List<SitecoreCmsEntity> itemList = new List<SitecoreCmsEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SitecoreCmsEntity>("select * from SitecoreCmsEntity");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<SitecoreCmsEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static void UpdateItem(SitecoreCmsEntity item)
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.Update(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }
    }
}
