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
using myTNB.SitecoreCMS.Model;
using SQLite;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Database.Model
{
    public class FullRTEPagesEntity : FullRTEPagesModel
    {
        public void CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FullRTEPagesEntity");
            db.CreateTable<FullRTEPagesEntity>();
        }

        public void InsertItem(FullRTEPagesEntity item)
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
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
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<FullRTEPagesEntity>("select * from FullRTEPagesEntity");
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
                var db = new SQLiteConnection(Constants.DB_PATH);
                db.DeleteAll<FullRTEPagesEntity>();
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