using System;
using System.IO;
using SQLite;

namespace myTNB.SQLite
{
    public static class SQLiteHelper
    {
        public static SQLiteConnection _db;
        /// <summary>
        /// Call once during app launch
        /// </summary>
        public static void CreateDB()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), TNBGlobal.DB_NAME);
            _db = new SQLiteConnection(dbPath, openFlags: SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create, storeDateTimeAsTicks: true);
        }
    }
}