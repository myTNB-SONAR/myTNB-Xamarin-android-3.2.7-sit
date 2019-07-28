using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class ApplySSMRWalkthroughEntity : ApplySSMRModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<ApplySSMRWalkthroughEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("ApplySSMRWalkthroughEntity");
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
        public void InsertItem(ApplySSMRWalkthroughEntity item)
        {
            try
            {
                if (item != null)
                {
                    int newRecord = SQLiteHelper._db.InsertOrReplace(item);
                    Debug.WriteLine("Insert Record: {0}", newRecord);
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
        public void InsertListOfItems(List<ApplySSMRModel> itemList)
        {
            if (itemList != null)
            {
                foreach (ApplySSMRModel obj in itemList)
                {
                    ApplySSMRWalkthroughEntity item = new ApplySSMRWalkthroughEntity
                    {
                        ID = obj.ID,
                        Image = obj.Image,
                        Title = obj.Title,
                        Description = obj.Description
                    };
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<ApplySSMRWalkthroughEntity> GetAllEntityItems()
        {
            List<ApplySSMRWalkthroughEntity> itemList = new List<ApplySSMRWalkthroughEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<ApplySSMRWalkthroughEntity>("select * from ApplySSMRWalkthroughEntity");
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
        public List<ApplySSMRModel> GetAllItems()
        {
            List<ApplySSMRModel> itemList = new List<ApplySSMRModel>();
            List<ApplySSMRWalkthroughEntity> entityItems = GetAllEntityItems();
            ApplySSMRModel ssmrItem;
            foreach (var item in entityItems)
            {
                ssmrItem = new ApplySSMRModel
                {
                    ID = item.ID,
                    Image = item.Image,
                    Title = item.Title,
                    Description = item.Description
                };
                itemList.Add(ssmrItem);
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
                SQLiteHelper._db.DeleteAll<ApplySSMRWalkthroughEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}
