using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Database;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class LargeFontEntity : LargeFontModel
    {
        public void CreateTable()
        {
            //using(var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("LargeFontEntity");
            db.CreateTable<LargeFontEntity>();
            //}
        }

        public void InsertItem(LargeFontEntity item)
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
                //var selected = db.Query<LargeFontEntity>("select * from LargeFontEntity");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<LargeFontEntity> itemList)
        {
            if (itemList != null)
            {
                foreach (LargeFontEntity obj in itemList)
                {
                    LargeFontEntity item = new LargeFontEntity();
                    item.selected = obj.selected;
                    item.Key = obj.Key;
                    item.Value = obj.Value;
                    InsertItem(item);
                }
            }
        }

        public List<LargeFontEntity> GetAllItems()
        {
            List<LargeFontEntity> itemList = new List<LargeFontEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<LargeFontEntity>("select * from LargeFontEntity");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public void DeleteTable()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<LargeFontEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'LargeFontEntity';";
            var cmd = connection.CreateCommand(cmdText, typeof(T).Name);
            return cmd.ExecuteScalar<string>() != null;
        }
    }
}