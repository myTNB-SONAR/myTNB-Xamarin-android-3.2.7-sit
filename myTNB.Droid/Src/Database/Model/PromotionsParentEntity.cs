using SQLite;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using System;
using myTNB_Android.Src.Utils;
using myTNB.SitecoreCM.Models;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class PromotionsParentEntity : PromotionParentModel
    {
        public void CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("PromotionsParentEntity");
            db.CreateTable<PromotionsParentEntity>();
        }

        public void InsertItem(PromotionsParentEntity item)
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
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<PromotionsParentEntity>("select * from PromotionsParentEntity");
            }catch(Exception e){
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public void DeleteTable(){
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                db.DeleteAll<PromotionsParentEntity>();
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
                var db = new SQLiteConnection(Constants.DB_PATH);
                int newRecord = db.Update(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }
    }
}