using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class BillDetailsTooltipEntity : BillsTooltipModelEntity
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<BillDetailsTooltipEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("BillDetailsTooltipEntity");
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
        public void InsertItem(BillDetailsTooltipEntity item)
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
        public void InsertListOfItems(List<BillsTooltipModelEntity> itemList)
        {
            if (itemList != null)
            {
                foreach (BillsTooltipModelEntity obj in itemList)
                {
                    BillDetailsTooltipEntity item = new BillDetailsTooltipEntity
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
        List<BillDetailsTooltipEntity> GetAllEntityItems()
        {
            List<BillDetailsTooltipEntity> itemList = new List<BillDetailsTooltipEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<BillDetailsTooltipEntity>("select * from BillDetailsTooltipEntity");
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
        public List<BillsTooltipModelEntity> GetAllItems()
        {
            List<BillsTooltipModelEntity> itemList = new List<BillsTooltipModelEntity>();
            List<BillDetailsTooltipEntity> entityItems = GetAllEntityItems();
            BillsTooltipModelEntity ssmrItem;
            foreach (BillDetailsTooltipEntity item in entityItems)
            {
                ssmrItem = new BillsTooltipModelEntity
                {
                    ID = item.ID,
                    Image = item.Image,
                    Title = item.Title,
                    Description = item.Description,
                    ImageByteArray = item.ImageByteArray
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
                SQLiteHelper._db.DeleteAll<BillDetailsTooltipEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}