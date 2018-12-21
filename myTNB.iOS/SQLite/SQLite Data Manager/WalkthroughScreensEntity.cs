using SQLite;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using System;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class WalkthroughScreensEntity : WalkthroughScreensModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<WalkthroughScreensEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("WalkthroughScreensEntity");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Create Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void InsertItem(WalkthroughScreensEntity item)
        {
            try
            {
                if (item != null)
                {
                    int newRecord = SQLiteHelper._db.InsertOrReplace(item);
#if DEBUG
                    Console.WriteLine("Insert Record: {0}", newRecord);
#endif
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }
        /// <summary>
        /// Inserts the list of items.
        /// </summary>
        /// <param name="itemList">Item list.</param>
        public void InsertListOfItems(List<WalkthroughScreensModel> itemList)
        {
            if (itemList != null)
            {
                foreach (WalkthroughScreensModel obj in itemList)
                {
                    WalkthroughScreensEntity item = new WalkthroughScreensEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.SubText = obj.SubText;
                    item.Text = obj.Text;
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<WalkthroughScreensEntity> GetAllEntityItems()
        {
            List<WalkthroughScreensEntity> itemList = new List<WalkthroughScreensEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<WalkthroughScreensEntity>("select * from WalkthroughScreensEntity");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }
        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>The all items.</returns>
        public List<WalkthroughScreensModel> GetAllItems()
        {
            List<WalkthroughScreensModel> itemList = new List<WalkthroughScreensModel>();
            List<WalkthroughScreensEntity> entityItems = GetAllEntityItems();
            WalkthroughScreensModel wsModel;
            foreach (var item in entityItems)
            {
                wsModel = new WalkthroughScreensModel();
                wsModel.ID = item.ID;
                wsModel.Image = item.Image;
                wsModel.SubText = item.SubText;
                wsModel.Text = item.Text;
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
                SQLiteHelper._db.DeleteAll<WalkthroughScreensEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}