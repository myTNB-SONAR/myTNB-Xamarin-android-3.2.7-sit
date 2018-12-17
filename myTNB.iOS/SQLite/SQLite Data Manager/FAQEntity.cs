using System;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class FAQEntity : FAQsModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            SQLiteHelper._db.CreateTable<FAQEntity>();
            List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("FAQEntity");
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void InsertItem(FAQEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
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
        public void InsertListOfItems(List<FAQsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (FAQsModel obj in itemList)
                {
                    FAQEntity item = new FAQEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image;
                    item.Question = obj.Question;
                    item.Answer = obj.Answer;
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<FAQEntity> GetAllEntityItems()
        {
            List<FAQEntity> itemList = new List<FAQEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<FAQEntity>("select * from FAQEntity");
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
        public List<FAQsModel> GetAllItems()
        {
            List<FAQsModel> itemList = new List<FAQsModel>();
            List<FAQEntity> entityItems = GetAllEntityItems();
            FAQsModel faqItems;
            foreach (var item in entityItems)
            {
                faqItems = new FAQsModel();
                faqItems.Image = item.Image;
                faqItems.ID = item.ID;
                faqItems.Question = item.Question;
                faqItems.Answer = item.Answer;
                itemList.Add(faqItems);
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
                SQLiteHelper._db.DeleteAll<FAQEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}