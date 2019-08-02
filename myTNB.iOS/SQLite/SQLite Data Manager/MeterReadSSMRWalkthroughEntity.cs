using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class MeterReadSSMRWalkthroughEntity : MeterReadSSMRModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<MeterReadSSMRWalkthroughEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("MeterReadSSMRWalkthroughEntity");
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
        public void InsertItem(MeterReadSSMRWalkthroughEntity item)
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
        public void InsertListOfItems(List<MeterReadSSMRModel> itemList)
        {
            if (itemList != null)
            {
                foreach (MeterReadSSMRModel obj in itemList)
                {
                    MeterReadSSMRWalkthroughEntity item = new MeterReadSSMRWalkthroughEntity
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
        List<MeterReadSSMRWalkthroughEntity> GetAllEntityItems()
        {
            List<MeterReadSSMRWalkthroughEntity> itemList = new List<MeterReadSSMRWalkthroughEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<MeterReadSSMRWalkthroughEntity>("select * from MeterReadSSMRWalkthroughEntity");
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
        public List<MeterReadSSMRModel> GetAllItems()
        {
            List<MeterReadSSMRModel> itemList = new List<MeterReadSSMRModel>();
            List<MeterReadSSMRWalkthroughEntity> entityItems = GetAllEntityItems();
            MeterReadSSMRModel ssmrItem;
            foreach (var item in entityItems)
            {
                ssmrItem = new MeterReadSSMRModel
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
                SQLiteHelper._db.DeleteAll<MeterReadSSMRWalkthroughEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}
