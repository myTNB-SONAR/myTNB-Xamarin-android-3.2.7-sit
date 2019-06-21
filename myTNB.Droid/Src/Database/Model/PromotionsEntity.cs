using myTNB.SitecoreCM.Models;
using myTNB_Android.Src.Database;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class PromotionsEntity : PromotionsModel
    {
        public void CreateTable()
        {
            //var db = new SQLiteConnection(Constants.DB_PATH);
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("PromotionsEntity");
            db.CreateTable<PromotionsEntity>();
        }

        public void InsertItem(PromotionsEntity item)
        {
            try
            {
                //var db = new SQLiteConnection(Constants.DB_PATH);
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<PromotionsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (PromotionsModel obj in itemList)
                {
                    PromotionsEntity item = new PromotionsEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.Title = obj.Title;
                    item.Text = obj.Text;
                    item.SubText = obj.SubText;
                    item.CampaignPeriod = obj.CampaignPeriod;
                    item.Prizes = obj.Prizes;
                    item.HowToWin = obj.HowToWin;
                    item.FooterNote = obj.FooterNote;
                    item.PublishedDate = obj.PublishedDate;
                    item.GeneralLinkUrl = obj.GeneralLinkUrl;
                    item.GeneralLinkText = obj.GeneralLinkText;
                    item.Read = obj.Read;
                    InsertItem(item);
                }
            }
        }

        public List<PromotionsEntity> GetAllItems()
        {
            List<PromotionsEntity> itemList = new List<PromotionsEntity>();
            try
            {
                //var db = new SQLiteConnection(Constants.DB_PATH);
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<PromotionsEntity>("select * from PromotionsEntity");
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
                //var db = new SQLiteConnection(Constants.DB_PATH);
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<PromotionsEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public void UpdateItem(PromotionsEntity item)
        {
            try
            {
                //var db = new SQLiteConnection(Constants.DB_PATH);
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<PromotionsEntity>("SELECT * FROM PromotionsEntity WHERE ID = ? ", item.ID);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    var promotionRecord = existingRecord[0];
                    promotionRecord.Image = item.Image;
                    promotionRecord.Read = item.Read;
                    db.Update(promotionRecord);
                    Console.WriteLine("Update Record: {0}", promotionRecord);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        internal static bool HasUnread()
        {
            try
            {
                //var db = new SQLiteConnection(Constants.DB_PATH);
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<PromotionsEntity>("SELECT * FROM PromotionsEntity WHERE Read = ? ", false);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }

            return false;
        }
    }
}