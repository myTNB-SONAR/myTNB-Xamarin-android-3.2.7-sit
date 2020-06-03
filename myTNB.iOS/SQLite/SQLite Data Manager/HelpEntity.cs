using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class HelpEntity : HelpModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<HelpEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("HelpEntity");
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
        public void InsertItem(HelpEntity item)
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
        public void InsertListOfItems(List<HelpModel> itemList)
        {
            if (itemList != null)
            {
                foreach (HelpModel obj in itemList)
                {
                    HelpEntity item = new HelpEntity
                    {
                        ID = obj.ID,
                        BGEndColor = obj.BGEndColor,
                        BGGradientDirection = obj.BGGradientDirection,
                        BGStartColor = obj.BGStartColor,
                        CTA = obj.CTA,
                        Description = obj.Description,
                        Tags = obj.Tags,
                        TargetItem = obj.TargetItem,
                        Title = obj.Title,
                        TopicBodyTitle = obj.TopicBodyTitle,
                        TopicBodyContent = obj.TopicBodyContent,
                        TopicBGImage = obj.TopicBGImage
                    };
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<HelpEntity> GetAllEntityItems()
        {
            List<HelpEntity> itemList = new List<HelpEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<HelpEntity>("select * from HelpEntity");
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
        public List<HelpModel> GetAllItems()
        {
            List<HelpModel> itemList = new List<HelpModel>();
            List<HelpEntity> entityItems = GetAllEntityItems();
            HelpModel helpItem;
            foreach (var item in entityItems)
            {
                helpItem = new HelpModel
                {
                    ID = item.ID,
                    BGEndColor = item.BGEndColor,
                    BGGradientDirection = item.BGGradientDirection,
                    BGStartColor = item.BGStartColor,
                    CTA = item.CTA,
                    Description = item.Description,
                    Tags = item.Tags,
                    TargetItem = item.TargetItem,
                    Title = item.Title,
                    TopicBodyTitle = item.TopicBodyTitle,
                    TopicBodyContent = item.TopicBodyContent,
                    TopicBGImage = item.TopicBGImage
                };
                itemList.Add(helpItem);
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
                SQLiteHelper._db.DeleteAll<HelpEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}