using SQLite;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using System;
using myTNB_Android.Src.Utils;
using myTNB.SitecoreCM.Models;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class PromotionsParentEntityV2 : PromotionParentModelV2
    {
        public void CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("PromotionsParentEntityV2");
            db.CreateTable<PromotionsParentEntityV2>();
        }

        public void InsertItem(PromotionsParentEntityV2 item)
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

        public void InsertListOfItems(List<PromotionParentModelV2> itemList)
        {
            if(itemList != null){
                foreach (PromotionParentModelV2 obj in itemList)
                {
                    PromotionsParentEntityV2 item = new PromotionsParentEntityV2();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<PromotionsParentEntityV2> GetAllItems()
        {
            List<PromotionsParentEntityV2> itemList = new List<PromotionsParentEntityV2>();
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<PromotionsParentEntityV2>("select * from PromotionsParentEntityV2");
            }catch(Exception e){
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public void DeleteTable(){
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                db.DeleteAll<PromotionsParentEntityV2>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}",e.Message);
            }
        }

        public void UpdateItem(PromotionsParentEntityV2 item)
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

        public static void RemoveAll()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM PromotionsParentEntityV2");
        }
    }
}