using System;
using System.Collections.Generic;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    [Table("BillHistory")]
    public class BillHistoryEntity
    {
        #region Properties

        [PrimaryKey]
        public string AccNum { get; set; }

        public string Data { get; set; }

        public string DateUpdated { get; set; }

        public bool IsRefreshNeeded { get; set; }

        #endregion

        /// <summary>
        /// Creates the table.
        /// </summary>
        public static void CreateTable()
        {
            SQLiteHelper._db.CreateTable<BillHistoryEntity>();
            List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("BillHistory");
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public static void InsertItem(BillHistoryEntity item)
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
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="key">Key.</param>
        public static BillHistoryEntity GetItem(string key)
        {
            BillHistoryEntity item = null;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    item = SQLiteHelper._db.Get<BillHistoryEntity>(key);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Reading from Table : {0}", e.Message);
            }
            return item;
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="key">Key.</param>
        public static void DeleteItem(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    SQLiteHelper._db.Delete<BillHistoryEntity>(key);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Reading from Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Updates the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public static void UpdateItem(BillHistoryEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.Update(item);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Update Item in Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public static void DeleteTable()
        {
            try
            {
                SQLiteHelper._db.DeleteAll<BillHistoryEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

    }
}
