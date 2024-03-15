using myTNB.SitecoreCMS.Models;
using myTNB.AndroidApp.Src.Database;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class FAQsEntity : FAQsModel
    {
        public void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FAQsEntity");
            db.CreateTable<FAQsEntity>();
            //}
        }

        public void InsertItem(FAQsEntity item)
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

        public void InsertListOfItems(List<FAQsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (FAQsModel obj in itemList)
                {
                    FAQsEntity item = new FAQsEntity();
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.Question = obj.Question;
                    item.Answer = obj.Answer;
                    item.ID = obj.ID;
                    InsertItem(item);
                }
            }
        }

        public List<FAQsEntity> GetAllItems()
        {
            List<FAQsEntity> itemList = new List<FAQsEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<FAQsEntity>("select * from FAQsEntity");
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
                db.DeleteAll<FAQsEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public void UpdateItem(FAQsEntity item)
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