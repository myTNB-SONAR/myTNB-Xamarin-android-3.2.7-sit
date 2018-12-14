using System;
using System.Collections.Generic;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    [Table("BillingAccounts")]
    public class BillingAccountEntity
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
            SQLiteHelper._db.CreateTable<BillingAccountEntity>();
            List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("BillingAccounts");
        }
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public static void InsertItem(BillingAccountEntity item)
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
        public static BillingAccountEntity GetItem(string key)
        {
            BillingAccountEntity item = null;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    item = SQLiteHelper._db.Get<BillingAccountEntity>(key);
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
                    SQLiteHelper._db.Delete<BillingAccountEntity>(key);
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
        public static void UpdateItem(BillingAccountEntity item)
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
                SQLiteHelper._db.DeleteAll<BillingAccountEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

    }
}
