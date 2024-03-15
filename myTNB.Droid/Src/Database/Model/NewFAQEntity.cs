using System;
using System.Collections.Generic;
using System.Linq;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("NewFAQEntity")]
    public class NewFAQEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Image")]
        public string Image { set; get; }

        [Column("BGStartColor")]
        public string BGStartColor { set; get; }

        [Column("BGEndColor")]
        public string BGEndColor { set; get; }

        [Column("BGDirection")]
        public string BGDirection { set; get; }

        [Column("Title")]
        public string Title { set; get; }

        [Column("Description")]
        public string Description { set; get; }

        [Column("TopicBodyTitle")]
        public string TopicBodyTitle { set; get; }

        [Column("TopicBodyContent")]
        public string TopicBodyContent { set; get; }

        [Column("CTA")]
        public string CTA { set; get; }

        [Column("Tags")]
        public string Tags { set; get; }

        [Column("TargetItem")]
        public string TargetItem { set; get; }

        public void CreateTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("NewFAQEntity");
                db.CreateTable<NewFAQEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<NewFAQEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<NewFAQEntity>();
        }

        public void InsertListOfItems(List<HelpModel> itemList)
        {
            if (itemList != null)
            {
                foreach (HelpModel obj in itemList)
                {
                    NewFAQEntity item = new NewFAQEntity();
                    item.ID = obj.ID;
                    item.Image = obj.TopicBGImage;
                    item.BGStartColor = obj.BGStartColor;
                    item.BGEndColor = obj.BGEndColor;
                    item.BGDirection = obj.BGGradientDirection;
                    item.Title = obj.Title;
                    item.Description = obj.Description;
                    item.TopicBodyTitle = obj.TopicBodyTitle;
                    item.TopicBodyContent = obj.TopicBodyContent;
                    item.CTA = obj.CTA;
                    item.Tags = obj.Tags;
                    item.TargetItem = obj.TargetItem;
                    InsertItem(item);
                }
            }
        }

        public void InsertItem(NewFAQEntity item)
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

        public List<NewFAQEntity> GetAll()
        {
            List<NewFAQEntity> itemList = new List<NewFAQEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<NewFAQEntity>("SELECT * FROM NewFAQEntity");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return itemList;
        }
    }
}
