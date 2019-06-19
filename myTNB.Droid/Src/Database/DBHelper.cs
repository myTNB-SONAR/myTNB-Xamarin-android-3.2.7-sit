using myTNB_Android.Src.Utils;
using SQLite;

namespace myTNB_Android.Src.Database
{
    public class DBHelper
    {
        //public static Boolean TableExists (string tableName)
        //{

        //}

        private static SQLiteConnection sqliteConnection = null;
        //private static SQLiteAsyncConnection sqliteConnection = null;

        public static SQLiteConnection GetSQLiteConnection()
        {

            if (sqliteConnection != null)
            {
                return sqliteConnection;
            }
            else
            {
                sqliteConnection = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                //sqliteConnection = new SQLiteAsyncConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                return sqliteConnection;
            }
        }

    }
}