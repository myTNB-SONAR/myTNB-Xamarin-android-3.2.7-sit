using System;
using System.Collections.Generic;
using myTNB.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    [Table("Dues")]
    public class DueEntity : DueAmountDataModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public static void CreateTable()
        {
            SQLiteHelper._db.CreateTable<DueEntity>();
            List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("Dues");
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public static void InsertItem(DueEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.InsertOrReplace(item);
#if DEBUG
                Console.WriteLine("Insert Due Record: {0}", newRecord);
#endif
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
        public static DueEntity GetItem(string key)
        {
            DueEntity item = null;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    item = SQLiteHelper._db.Get<DueEntity>(key);
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
                    SQLiteHelper._db.Delete<DueEntity>(key);
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
        public static void UpdateItem(DueEntity item)
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
                SQLiteHelper._db.DeleteAll<DueEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}
