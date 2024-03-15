using myTNB.SitecoreCMS.Models;
using myTNB.AndroidApp.Src.Database;
using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class FAQsParentEntity : FAQsParentModel
    {
        public void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FAQsParentEntity");
            db.CreateTable<FAQsParentEntity>();
            //}
        }

        public void InsertItem(FAQsParentEntity item)
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

        public void InsertListOfItems(List<FAQsParentModel> itemList)
        {
            if (itemList != null)
            {
                foreach (FAQsParentModel obj in itemList)
                {
                    FAQsParentEntity item = new FAQsParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<FAQsParentEntity> GetAllItems()
        {
            List<FAQsParentEntity> itemList = new List<FAQsParentEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<FAQsParentEntity>("select * from FAQsParentEntity");
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
                using (var db = new SQLiteConnection(Constants.DB_PATH))
                {
                    db.DeleteAll<FAQsParentEntity>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public void UpdateItem(FAQsParentEntity item)
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