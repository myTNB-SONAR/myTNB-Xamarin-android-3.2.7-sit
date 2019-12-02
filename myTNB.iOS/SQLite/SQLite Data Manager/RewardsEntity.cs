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
                rewardModel = new RewardsModel();
                rewardModel.CategoryID = item.CategoryID;
                rewardModel.CategoryName = item.CategoryName;
                rewardModel.ID = item.ID;
                rewardModel.RewardName = item.RewardName;
                rewardModel.Title = item.Title;
                rewardModel.TitleOnListing = item.TitleOnListing;
                rewardModel.Description = item.Description;
                rewardModel.Image = item.Image;
                rewardModel.PeriodLabel = item.PeriodLabel;
                rewardModel.LocationLabel = item.LocationLabel;
                rewardModel.TandCLabel = item.TandCLabel;
                rewardModel.StartDate = item.StartDate;
                rewardModel.EndDate = item.EndDate;
                rewardModel.IsSaved = item.IsSaved;
                rewardModel.IsRead = item.IsRead;
                rewardModel.IsUsed = item.IsUsed;
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
    }
}
