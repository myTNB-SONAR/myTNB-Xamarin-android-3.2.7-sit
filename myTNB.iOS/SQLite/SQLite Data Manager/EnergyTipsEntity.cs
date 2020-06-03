using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class EnergyTipsEntity : TipsModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<EnergyTipsEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("EnergyTipsEntity");
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
        public void InsertItem(EnergyTipsEntity item)
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
        public void InsertListOfItems(List<TipsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (TipsModel obj in itemList)
                {
                    EnergyTipsEntity item = new EnergyTipsEntity
                    {
                        ID = obj.ID,
                        Image = obj.Image,
                        Title = obj.Title,
                        Description = obj.Description,
                        ImageByteArray = obj.ImageByteArray
                    };
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<EnergyTipsEntity> GetAllEntityItems()
        {
            List<EnergyTipsEntity> itemList = new List<EnergyTipsEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<EnergyTipsEntity>("select * from EnergyTipsEntity");
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
        public List<TipsModel> GetAllItems()
        {
            List<TipsModel> itemList = new List<TipsModel>();
            List<EnergyTipsEntity> entityItems = GetAllEntityItems();
            TipsModel tipsItem;
            foreach (EnergyTipsEntity item in entityItems)
            {
                tipsItem = new TipsModel
                {
                    ID = item.ID,
                    Image = item.Image,
                    Title = item.Title,
                    Description = item.Description,
                    ImageByteArray = item.ImageByteArray
                };
                itemList.Add(tipsItem);
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
                SQLiteHelper._db.DeleteAll<EnergyTipsEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}