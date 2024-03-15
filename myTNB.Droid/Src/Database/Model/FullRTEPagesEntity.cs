using myTNB.SitecoreCMS.Model;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Database.Model
{
    public class FullRTEPagesEntity : FullRTEPagesModel
    {
        public void CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FullRTEPagesEntity");
            db.CreateTable<FullRTEPagesEntity>();
            //}
        }

        public void InsertItem(FullRTEPagesEntity item)
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<FullRTEPagesModel> itemList)
        {
            if (itemList != null)
            {
                foreach (FullRTEPagesModel obj in itemList)
                {
                    FullRTEPagesEntity item = new FullRTEPagesEntity();
                    item.ID = obj.ID;
                    item.Title = obj.Title;
                    item.GeneralText = obj.GeneralText;
                    item.PublishedDate = obj.PublishedDate;
                    InsertItem(item);
                }
            }
        }

        public List<FullRTEPagesEntity> GetAllItems()
        {
            List<FullRTEPagesEntity> itemList = new List<FullRTEPagesEntity>();
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<FullRTEPagesEntity>("select * from FullRTEPagesEntity");
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
                db.DeleteAll<FullRTEPagesEntity>();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static bool TableExists<T>(SQLiteConnection connection)
        {
            const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=FullRTEPagesEntity";
            var cmd = connection.CreateCommand(cmdText, typeof(T).Name);
            return cmd.ExecuteScalar<string>() != null;
        }
    }
}