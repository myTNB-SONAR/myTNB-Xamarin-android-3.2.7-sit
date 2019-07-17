using System;
using System.Diagnostics;
using myTNB.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    [Table("Dues")]
    public class DueEntity : DueAmountDataModel
    {
        /// <summary>
        /// Deletes the table.
        /// </summary>
        public static void DeleteTable()
        {
            try
            {
                SQLiteHelper._db.DeleteAll<DueEntity>();
                Debug.WriteLine("Due Entity Table Deleted");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }
    }
}