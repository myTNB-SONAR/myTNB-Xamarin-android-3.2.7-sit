using SQLite;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using System;
using myTNB_Android.Src.Utils;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class WalkthroughScreensEntity : WalkthroughScreensModel
    {
        public void CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("WalkthroughScreensEntity");
            db.CreateTable<WalkthroughScreensEntity>();
        }

        public void InsertItem(WalkthroughScreensEntity item)
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord );
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<WalkthroughScreensModel> itemList)
        {
            if(itemList != null){
                foreach (WalkthroughScreensModel obj in itemList)
                {
                    WalkthroughScreensEntity item = new WalkthroughScreensEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.SubText = obj.SubText;
                    item.Text = obj.Text;
                    InsertItem(item);
                }
            }
        }

        public List<WalkthroughScreensEntity> GetAllItems()
        {
            List<WalkthroughScreensEntity> itemList = new List<WalkthroughScreensEntity>();
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<WalkthroughScreensEntity>("select * from WalkthroughScreensEntity");
            }catch(Exception e){
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public void DeleteTable(){
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                db.DeleteAll<WalkthroughScreensEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}",e.Message);
            }
        }
    }
}