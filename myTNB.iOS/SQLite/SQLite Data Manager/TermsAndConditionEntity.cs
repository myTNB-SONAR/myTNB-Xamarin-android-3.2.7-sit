using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class TermsAndConditionEntity : FullRTEPagesModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<TermsAndConditionEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("TermsAndConditionEntity");
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
        public void InsertItem(TermsAndConditionEntity item)
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
        public void InsertListOfItems(List<FullRTEPagesModel> itemList)
        {
            if (itemList != null)
            {
                foreach (FullRTEPagesModel obj in itemList)
                {
                    TermsAndConditionEntity item = new TermsAndConditionEntity();
                    item.ID = obj.ID;
                    item.GeneralText = obj.GeneralText;
                    item.Title = obj.Title;
                    item.PublishedDate = obj.PublishedDate;
                    InsertItem(item);
                }
            }
        }
        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<TermsAndConditionEntity> GetAllEntityItems()
        {
            List<TermsAndConditionEntity> itemList = new List<TermsAndConditionEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<TermsAndConditionEntity>("select * from TermsAndConditionEntity");
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
        public List<FullRTEPagesModel> GetAllItems()
        {
            List<FullRTEPagesModel> itemList = new List<FullRTEPagesModel>();
            List<TermsAndConditionEntity> entityItems = GetAllEntityItems();
            FullRTEPagesModel tncModel;
            foreach (var item in entityItems)
            {
                tncModel = new FullRTEPagesModel();
                tncModel.GeneralText = item.GeneralText;
                tncModel.ID = item.ID;
                tncModel.PublishedDate = item.PublishedDate;
                tncModel.Title = item.Title;
                itemList.Add(tncModel);
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
                SQLiteHelper._db.DeleteAll<TermsAndConditionEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}