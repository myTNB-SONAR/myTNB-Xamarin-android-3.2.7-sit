using System;
using System.Collections.Generic;
using System.Diagnostics;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    [Table("PaymentHistory")]
    public class PaymentHistoryEntity
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
            try
            {
                SQLiteHelper._db.CreateTable<PaymentHistoryEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("PaymentHistory");
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
        public static void InsertItem(PaymentHistoryEntity item)
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
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="key">Key.</param>
        public static PaymentHistoryEntity GetItem(string key)
        {
            PaymentHistoryEntity item = null;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    item = SQLiteHelper._db.Get<PaymentHistoryEntity>(key);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Reading from Table : {0}", e.Message);
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
                    SQLiteHelper._db.Delete<PaymentHistoryEntity>(key);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Reading from Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Updates the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public static void UpdateItem(PaymentHistoryEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.Update(item);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Update Item in Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public static void DeleteTable()
        {
            try
            {
                SQLiteHelper._db.DeleteAll<PaymentHistoryEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

    }
}
