using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Database
{
    public class DBHelper
    {
        //public static Boolean TableExists (string tableName)
        //{

        //}

        private static SQLiteConnection sqliteConnection = null;
        //private static SQLiteAsyncConnection sqliteConnection = null;

        public static SQLiteConnection GetSQLiteConnection(){

            if (sqliteConnection != null) {
                return sqliteConnection;
            } else {
                sqliteConnection = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                //sqliteConnection = new SQLiteAsyncConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                return sqliteConnection;
            }
        }

    }
}