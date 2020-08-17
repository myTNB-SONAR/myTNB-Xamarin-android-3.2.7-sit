using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

//Created by Syahmi ICS 05052020

namespace myTNB.SQLite.SQLiteDataManager
{
    public class EppInfoTooltipEntity : EppTooltipModelEntity
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<EppInfoTooltipEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("EppInfoTooltipEntity");
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
        public void InsertItem(EppInfoTooltipEntity item)
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
        public void InsertListOfItems(List<EppTooltipModelEntity> itemList)
        {
            if (itemList != null)
            {
                foreach (EppTooltipModelEntity obj in itemList)
                {
                    EppInfoTooltipEntity item = new EppInfoTooltipEntity
                    {
                        ID = obj.ID,
                        Image = obj.Image,
                        Title = obj.Title,
                        PopUpTitle = obj.PopUpTitle,
                        PopUpBody = obj.PopUpBody,
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
        List<EppInfoTooltipEntity> GetAllEntityItems()
        {
            List<EppInfoTooltipEntity> itemList = new List<EppInfoTooltipEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<EppInfoTooltipEntity>("select * from EppInfoTooltipEntity");
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
        public List<EppTooltipModelEntity> GetAllItems()
        {
            List<EppTooltipModelEntity> itemList = new List<EppTooltipModelEntity>();
            List<EppInfoTooltipEntity> entityItems = GetAllEntityItems();
            EppTooltipModelEntity ssmrItem;
            foreach (EppInfoTooltipEntity item in entityItems)
            {
                ssmrItem = new EppTooltipModelEntity
                {
                    ID = item.ID,
                    Image = item.Image,
                    Title = item.Title,
                    PopUpTitle = item.PopUpTitle,
                    PopUpBody = item.PopUpBody,
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
                SQLiteHelper._db.DeleteAll<EppInfoTooltipEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}
