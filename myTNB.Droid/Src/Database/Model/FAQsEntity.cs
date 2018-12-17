using SQLite;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using System;
using myTNB_Android.Src.Utils;
using myTNB.SitecoreCM.Models;
using myTNB.SitecoreCMS.Models;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class FAQsEntity : FAQsModel
    {
        public void CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FAQsEntity");
            db.CreateTable<FAQsEntity>();
        }

        public void InsertItem(FAQsEntity item)
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

        public void InsertListOfItems(List<FAQsModel> itemList)
        {
            if(itemList != null){
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
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<FAQsEntity>("select * from FAQsEntity");
            }catch(Exception e){
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public void DeleteTable(){
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                db.DeleteAll<FAQsEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}",e.Message);
            }
        }

        public void UpdateItem(FAQsEntity item)
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