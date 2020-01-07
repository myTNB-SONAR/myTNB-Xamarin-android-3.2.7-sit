using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                SQLiteHelper._db.CreateTable<PromotionsEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("PromotionsEntity");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Create Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void InsertItem(PromotionsEntity item)
        {
            try
            {
                if (item != null)
                {
                    int newRecord = SQLiteHelper._db.InsertOrReplace(item);
#if DEBUG
                    Debug.WriteLine("Insert Record: {0}", newRecord);
#endif
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="key">Key.</param>
        public static PromotionsEntity GetItem(string key)
        {
            PromotionsEntity item = null;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    item = SQLiteHelper._db.Get<PromotionsEntity>(key);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Reading from Table : {0}", e.Message);
            }
            return item;
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public static void UpdateItem(PromotionsEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.Update(item);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Update Item in Table : {0}", e.Message);
            }
        }

#if false
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
#endif
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
                Debug.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

#if false
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
#endif
        /// <summary>
        /// Deletes the table.
        /// </summary>
        public static void DeleteTable()
        {
            try
            {
                SQLiteHelper._db.DeleteAll<PromotionsEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Inserts the list of items.
        /// </summary>
        /// <param name="itemList">Item list.</param>
        public void InsertListOfItemsV2(List<PromotionsModel> itemList)
        {
            if (itemList != null)
            {
                try
                {
                    foreach (PromotionsModel obj in itemList)
                    {
                        PromotionsEntity item = new PromotionsEntity();
                        item.Title = obj.Title;
                        item.Text = obj.Text;
                        item.SubText = obj.SubText;
                        item.PublishedDate = obj.PublishedDate;
                        item.GeneralLinkUrl = obj.GeneralLinkUrl;
                        item.ID = obj.ID;
                        item.IsRead = obj.IsRead;
                        item.HeaderContent = obj.HeaderContent;
                        item.BodyContent = obj.BodyContent;
                        item.FooterContent = obj.FooterContent;
                        item.PortraitImage = obj.PortraitImage;
                        item.LandscapeImage = obj.LandscapeImage;
                        item.PromoStartDate = obj.PromoStartDate;
                        item.PromoEndDate = obj.PromoEndDate;
                        item.IsPromoExpired = obj.IsPromoExpired;
                        item.ShowAtAppLaunch = obj.ShowAtAppLaunch;
                        item.PromoShownDate = obj.PromoShownDate;
                        InsertItem(item);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error in Insert Promo Items : {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>The all items.</returns>
        public List<PromotionsModel> GetAllItemsV2()
        {
            List<PromotionsModel> itemList = new List<PromotionsModel>();
            List<PromotionsEntity> entityItems = GetAllEntityItems();
            PromotionsModel promotionItem;
            foreach (var item in entityItems)
            {
                promotionItem = new PromotionsModel();
                promotionItem.Title = item.Title;
                promotionItem.Text = item.Text;
                promotionItem.SubText = item.SubText;
                promotionItem.PublishedDate = item.PublishedDate;
                promotionItem.GeneralLinkUrl = item.GeneralLinkUrl;
                //promotionItem.GeneralLinkText = item.GeneralLinkText;
                promotionItem.ID = item.ID;
                promotionItem.IsRead = item.IsRead;

                promotionItem.HeaderContent = item.HeaderContent;
                promotionItem.BodyContent = item.BodyContent;
                promotionItem.FooterContent = item.FooterContent;
                promotionItem.PortraitImage = item.PortraitImage;
                promotionItem.LandscapeImage = item.LandscapeImage;
                promotionItem.PromoStartDate = item.PromoStartDate;
                promotionItem.PromoEndDate = item.PromoEndDate;
                promotionItem.IsPromoExpired = item.IsPromoExpired;
                promotionItem.ShowAtAppLaunch = item.ShowAtAppLaunch;
                promotionItem.PromoShownDate = item.PromoShownDate;

                itemList.Add(promotionItem);
            }
            return itemList;
        }

    }
}