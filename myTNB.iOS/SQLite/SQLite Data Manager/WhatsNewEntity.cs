using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class WhatsNewEntity : WhatsNewModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<WhatsNewEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("WhatsNewEntity");
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
        public void InsertItem(WhatsNewEntity item)
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
        /// Deletes the item
        /// </summary>
        /// <param name="key"></param>
        public void DeleteItem(string key)
        {
            try
            {
                if (key.IsValid())
                {
                    SQLiteHelper._db.Delete<WhatsNewEntity>(key);
#if DEBUG
                    Debug.WriteLine("Delete Record Successful");
#endif
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Item in Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Inserts the list of items.
        /// </summary>
        /// <param name="itemList">Item list.</param>
        public void InsertListOfItems(List<WhatsNewModel> itemList)
        {
            if (itemList != null)
            {
                foreach (WhatsNewModel obj in itemList)
                {
                    WhatsNewEntity item = new WhatsNewEntity();

                    item.ID = obj.ID;
                    item.CategoryID = obj.CategoryID;
                    item.CategoryName = obj.CategoryName;
                    item.Title = obj.Title;
                    item.TitleOnListing = obj.TitleOnListing;
                    item.Description = obj.Description;
                    item.Image = obj.Image;
                    item.StartDate = obj.StartDate;
                    item.EndDate = obj.EndDate;
                    item.PublishDate = obj.PublishDate;
                    item.IsRead = obj.IsRead;
                    item.Image_DetailsView = obj.Image_DetailsView;
                    item.Styles_DetailsView = obj.Styles_DetailsView;
                    item.PortraitImage_PopUp = obj.PortraitImage_PopUp;
                    item.ShowEveryCountDays_PopUp = obj.ShowEveryCountDays_PopUp;
                    item.ShowForTotalCountDays_PopUp = obj.ShowForTotalCountDays_PopUp;
                    item.ShowDayDate = obj.ShowDayDate;
                    item.ShowDayDateTotal = obj.ShowDayDateTotal;
                    InsertItem(item);
                }
            }
        }

        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<WhatsNewEntity> GetAllEntityItems()
        {
            List<WhatsNewEntity> itemList = new List<WhatsNewEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<WhatsNewEntity>("select * from WhatsNewEntity");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>The all items.</returns>
        public List<WhatsNewModel> GetAllItems()
        {
            List<WhatsNewModel> itemList = new List<WhatsNewModel>();
            List<WhatsNewEntity> entityItems = GetAllEntityItems();
            WhatsNewModel rewardModel;
            foreach (var item in entityItems)
            {
                rewardModel = new WhatsNewModel
                {
                    CategoryID = item.CategoryID,
                    CategoryName = item.CategoryName,
                    ID = item.ID,
                    Title = item.Title,
                    TitleOnListing = item.TitleOnListing,
                    Description = item.Description,
                    Image = item.Image,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    PublishDate = item.PublishDate,
                    IsRead = item.IsRead,
                    Image_DetailsView = item.Image_DetailsView,
                    Styles_DetailsView = item.Styles_DetailsView,
                    PortraitImage_PopUp = item.PortraitImage_PopUp,
                    ShowEveryCountDays_PopUp = item.ShowEveryCountDays_PopUp,
                    ShowForTotalCountDays_PopUp = item.ShowForTotalCountDays_PopUp,
                    ShowDayDate = item.ShowDayDate,
                    ShowDayDateTotal = item.ShowDayDateTotal,
            };
                itemList.Add(rewardModel);
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
                SQLiteHelper._db.DeleteAll<WhatsNewEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void UpdateItem(WhatsNewEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.Update(item);
#if DEBUG
                Debug.WriteLine("Update Record: {0}", newRecord);
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Update Item in Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="key">Key.</param>
        public static WhatsNewEntity GetItem(string key)
        {
            WhatsNewEntity item = null;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    item = SQLiteHelper._db.Get<WhatsNewEntity>(key);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Reading from Table : {0}", e.Message);
            }
            return item;
        }

        /// <summary>
        /// Updates the entity item.
        /// </summary>
        /// <param name="reward"></param>
        public void UpdateEntity(WhatsNewModel reward)
        {
            if (reward != null)
            {
                WhatsNewEntity item = new WhatsNewEntity
                {
                    CategoryID = reward.CategoryID,
                    CategoryName = reward.CategoryName,
                    ID = reward.ID,
                    Title = reward.Title,
                    TitleOnListing = reward.TitleOnListing,
                    Description = reward.Description,
                    Image = reward.Image,
                    StartDate = reward.StartDate,
                    EndDate = reward.EndDate,
                    PublishDate = reward.PublishDate,
                    IsRead = reward.IsRead,
                    Image_DetailsView = reward.Image_DetailsView,
                    Styles_DetailsView = reward.Styles_DetailsView,
                    PortraitImage_PopUp = reward.PortraitImage_PopUp,
                    ShowEveryCountDays_PopUp = reward.ShowEveryCountDays_PopUp,
                    ShowForTotalCountDays_PopUp = reward.ShowForTotalCountDays_PopUp,
                    ShowDayDate = reward.ShowDayDate,
                    ShowDayDateTotal = reward.ShowDayDateTotal,
                };
                UpdateItem(item);
            }
        }
    }
}
