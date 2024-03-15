using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Database;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class PreLoginPromoEntity : PreLoginPromoModel
    {
        public void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("PreLoginPromoEntity");
            db.CreateTable<PreLoginPromoEntity>();
            //}
        }

        public void InsertItem(PreLoginPromoEntity item)
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

        public void InsertListOfItems(List<PreLoginPromoModel> itemList)
        {
            if (itemList != null)
            {
                foreach (PreLoginPromoModel obj in itemList)
                {
                    PreLoginPromoEntity item = new PreLoginPromoEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.GeneralLinkUrl = obj.GeneralLinkUrl;
                    item.GeneralLinkText = obj.GeneralLinkText;
                    InsertItem(item);
                }
            }
        }

        public List<PreLoginPromoEntity> GetAllItems()
        {
            List<PreLoginPromoEntity> itemList = new List<PreLoginPromoEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<PreLoginPromoEntity>("select * from PreLoginPromoEntity");
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
                db.DeleteAll<PreLoginPromoEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}