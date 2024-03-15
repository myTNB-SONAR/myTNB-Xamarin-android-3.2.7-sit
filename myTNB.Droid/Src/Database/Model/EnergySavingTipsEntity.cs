using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Database;
using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("EnergySavingTipsEntity")]
    public class EnergySavingTipsEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Title")]
        public string Title { set; get; }

        [Column("Description")]
        public string Description { set; get; }

        [Column("Image")]
        public string Image { set; get; }


        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("EnergySavingTipsEntity");
                db.CreateTable<EnergySavingTipsEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(EnergySavingTipsEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.InsertOrReplace(item);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertListOfItems(List<EnergySavingTipsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (EnergySavingTipsModel obj in itemList)
                {
                    EnergySavingTipsEntity item = new EnergySavingTipsEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.Title = obj.Title;
                    item.Description = obj.Description;
                    InsertItem(item);
                }
            }
        }

        public List<EnergySavingTipsEntity> GetAllItems()
        {
            List<EnergySavingTipsEntity> itemList = new List<EnergySavingTipsEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<EnergySavingTipsEntity>("select * from EnergySavingTipsEntity");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return itemList;
        }

        public void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<EnergySavingTipsEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}