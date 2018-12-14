using System;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class PromotionsEntity : PromotionsModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            SQLiteHelper._db.CreateTable<PromotionsEntity>();
            List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("PromotionsEntity");
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void InsertItem(PromotionsEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Inserts the list of items.
        /// </summary>
        /// <param name="itemList">Item list.</param>
        public void InsertListOfItems(List<PromotionsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (PromotionsModel obj in itemList)
                {
                    PromotionsEntity item = new PromotionsEntity();
                    item.Image = obj.Image;
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
                    item.ID = obj.ID;
                    item.IsRead = obj.IsRead;
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<PromotionsEntity> GetAllEntityItems()
        {
            List<PromotionsEntity> itemList = new List<PromotionsEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<PromotionsEntity>("select * from PromotionsEntity");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }
        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>The all items.</returns>
        public List<PromotionsModel> GetAllItems()
        {
            List<PromotionsModel> itemList = new List<PromotionsModel>();
            List<PromotionsEntity> entityItems = GetAllEntityItems();
            PromotionsModel promotionItem;
            foreach (var item in entityItems)
            {
                promotionItem = new PromotionsModel();
                promotionItem.Image = item.Image;
                promotionItem.Title = item.Title;
                promotionItem.Text = item.Text;
                promotionItem.SubText = item.SubText;
                promotionItem.CampaignPeriod = item.CampaignPeriod;
                promotionItem.Prizes = item.Prizes;
                promotionItem.HowToWin = item.HowToWin;
                promotionItem.FooterNote = item.FooterNote;
                promotionItem.PublishedDate = item.PublishedDate;
                promotionItem.GeneralLinkUrl = item.GeneralLinkUrl;
                promotionItem.GeneralLinkText = item.GeneralLinkText;
                promotionItem.ID = item.ID;
                promotionItem.IsRead = item.IsRead;
                itemList.Add(promotionItem);
            }
            return itemList;
        }
        /// <summary>
        /// Deletes the table.
        /// </summary>
        public void DeleteTable()
        {
            try
            {
                SQLiteHelper._db.DeleteAll<PromotionsEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}