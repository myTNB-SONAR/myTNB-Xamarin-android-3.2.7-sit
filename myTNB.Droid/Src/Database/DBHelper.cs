﻿using myTNB.AndroidApp.Src.Utils;
using SQLite;

namespace myTNB.AndroidApp.Src.Database
{
    public class DBHelper
    {
        private static SQLiteConnection sqliteConnection = null;

        public static SQLiteConnection GetSQLiteConnection()
        {

            if (sqliteConnection != null)
            {
                return sqliteConnection;
            }
            else
            {
                sqliteConnection = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                return sqliteConnection;
            }
        }

    }
}