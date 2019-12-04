using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class RewardsEntity : RewardsModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<RewardsEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("RewardsEntity");
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
        public void InsertItem(RewardsEntity item)
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
        /// Inserts the list of items.
        /// </summary>
        /// <param name="itemList">Item list.</param>
        public void InsertListOfItems(List<RewardsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (RewardsModel obj in itemList)
                {
                    RewardsEntity item = new RewardsEntity();

                    item.CategoryID = obj.CategoryID;
                    item.CategoryName = obj.CategoryName;
                    item.ID = obj.ID;
                    item.RewardName = obj.RewardName;
                    item.Title = obj.Title;
                    item.TitleOnListing = obj.TitleOnListing;
                    item.Description = obj.Description;
                    item.Image = obj.Image;
                    item.PeriodLabel = obj.PeriodLabel;
                    item.LocationLabel = obj.LocationLabel;
                    item.TandCLabel = obj.TandCLabel;
                    item.StartDate = obj.StartDate;
                    item.EndDate = obj.EndDate;
                    item.RewardUseWithinTime = obj.RewardUseWithinTime;
                    item.RewardUseTitle = obj.RewardUseTitle;
                    item.RewardUseDescription = obj.RewardUseDescription;
                    item.IsSaved = obj.IsSaved;
                    item.IsRead = obj.IsRead;
                    item.IsUsed = obj.IsUsed;
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<RewardsEntity> GetAllEntityItems()
        {
            List<RewardsEntity> itemList = new List<RewardsEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<RewardsEntity>("select * from RewardsEntity");
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
        public List<RewardsModel> GetAllItems()
        {
            List<RewardsModel> itemList = new List<RewardsModel>();
            List<RewardsEntity> entityItems = GetAllEntityItems();
            RewardsModel rewardModel;
            foreach (var item in entityItems)
            {
                rewardModel = new RewardsModel
                {
                    CategoryID = item.CategoryID,
                    CategoryName = item.CategoryName,
                    ID = item.ID,
                    RewardName = item.RewardName,
                    Title = item.Title,
                    TitleOnListing = item.TitleOnListing,
                    Description = item.Description,
                    Image = item.Image,
                    PeriodLabel = item.PeriodLabel,
                    LocationLabel = item.LocationLabel,
                    TandCLabel = item.TandCLabel,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    RewardUseWithinTime = item.RewardUseWithinTime,
                    RewardUseTitle = item.RewardUseTitle,
                    RewardUseDescription = item.RewardUseDescription,
                    IsSaved = item.IsSaved,
                    IsRead = item.IsRead,
                    IsUsed = item.IsUsed
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
                SQLiteHelper._db.DeleteAll<RewardsEntity>();
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
        public void UpdateItem(RewardsEntity item)
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
        public static RewardsEntity GetItem(string key)
        {
            RewardsEntity item = null;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    item = SQLiteHelper._db.Get<RewardsEntity>(key);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Reading from Table : {0}", e.Message);
            }
            return item;
        }

        public void UpdateEntity(RewardsModel reward)
        {
            if (reward != null)
            {
                RewardsEntity item = new RewardsEntity
                {
                    CategoryID = reward.CategoryID,
                    CategoryName = reward.CategoryName,
                    ID = reward.ID,
                    RewardName = reward.RewardName,
                    Title = reward.Title,
                    TitleOnListing = reward.TitleOnListing,
                    Description = reward.Description,
                    Image = reward.Image,
                    PeriodLabel = reward.PeriodLabel,
                    LocationLabel = reward.LocationLabel,
                    TandCLabel = reward.TandCLabel,
                    StartDate = reward.StartDate,
                    EndDate = reward.EndDate,
                    IsSaved = reward.IsSaved,
                    IsRead = reward.IsRead,
                    IsUsed = reward.IsUsed,
                    RewardUseWithinTime = reward.RewardUseWithinTime,
                    RewardUseTitle = reward.RewardUseTitle,
                    RewardUseDescription=reward.RewardUseDescription
                };
                UpdateItem(item);
            }
        }
    }
}