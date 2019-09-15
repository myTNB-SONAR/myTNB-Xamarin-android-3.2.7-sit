using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class AppLaunchImageEntity : AppLaunchImageModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<AppLaunchImageEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("AppLaunchImageEntity");
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
        public void InsertItem(AppLaunchImageEntity item)
        {
            try
            {
                if (item != null)
                {
                    int newRecord = SQLiteHelper._db.InsertOrReplace(item);
#if DEBUG
                    Debug.WriteLine("Insert AppLaunchImageEntity Record: {0}", newRecord);
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
        public void InsertListOfItems(List<AppLaunchImageModel> itemList)
        {
            if (itemList != null)
            {
                foreach (AppLaunchImageModel obj in itemList)
                {
                    AppLaunchImageEntity item = new AppLaunchImageEntity();
                    item.ID = obj.ID;
                    item.Title = obj.Title;
                    item.Description = obj.Description;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.StartDateTime = obj.StartDateTime;
                    item.EndDateTime = obj.EndDateTime;
                    item.ShowForSeconds = obj.ShowForSeconds;
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<AppLaunchImageEntity> GetAllEntityItems()
        {
            List<AppLaunchImageEntity> itemList = new List<AppLaunchImageEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<AppLaunchImageEntity>("select * from AppLaunchImageEntity");
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
        public List<AppLaunchImageModel> GetAllItems()
        {
            List<AppLaunchImageModel> itemList = new List<AppLaunchImageModel>();
            List<AppLaunchImageEntity> entityItems = GetAllEntityItems();
            AppLaunchImageModel wsModel;
            foreach (var item in entityItems)
            {
                wsModel = new AppLaunchImageModel();
                wsModel.ID = item.ID;
                wsModel.Title = item.Title;
                wsModel.Description = item.Description;
                wsModel.Image = item.Image;
                wsModel.StartDateTime = item.StartDateTime;
                wsModel.EndDateTime = item.EndDateTime;
                wsModel.ShowForSeconds = item.ShowForSeconds;
                itemList.Add(wsModel);
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
                SQLiteHelper._db.DeleteAll<AppLaunchImageEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}
