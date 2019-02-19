using SQLite;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using System;
using myTNB_Android.Src.Utils;
using myTNB.SitecoreCM.Models;
using myTNB_Android.Src.Database;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class PromotionsParentEntity : PromotionParentModel
    {
        public void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("PromotionsParentEntity");
                db.CreateTable<PromotionsParentEntity>();
            //}
        }

        public void InsertItem(PromotionsParentEntity item)
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

        public void InsertListOfItems(List<PromotionParentModel> itemList)
        {
            if(itemList != null){
                foreach (PromotionParentModel obj in itemList)
                {
                    PromotionsParentEntity item = new PromotionsParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<PromotionsParentEntity> GetAllItems()
        {
            List<PromotionsParentEntity> itemList = new List<PromotionsParentEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                    itemList = db.Query<PromotionsParentEntity>("select * from PromotionsParentEntity");
                //}
            }catch(Exception e){
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public void DeleteTable(){
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                    db.DeleteAll<PromotionsParentEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}",e.Message);
            }
        }

        public void UpdateItem(PromotionsParentEntity item)
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