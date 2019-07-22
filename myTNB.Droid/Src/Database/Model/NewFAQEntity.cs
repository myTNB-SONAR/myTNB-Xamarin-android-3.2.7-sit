using System;
using System.Collections.Generic;
using System.Linq;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.SitecoreCMS.Model;

namespace myTNB_Android.Src.Database.Model
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
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("NewFAQEntity");
            db.CreateTable<NewFAQEntity>();
        }

        public void DeleteTable()
        {
            try
            {
                using (var db = new SQLiteConnection(Constants.DB_PATH))
                {
                    db.DeleteAll<NewFAQEntity>();
                }
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
                int newRecord = db.InsertOrReplace(item);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void RemoveAll()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("DELETE FROM NewFAQEntity");
        }

        public static int Count()
        {
            var db = DBHelper.GetSQLiteConnection();
            int count = db.Query<NewFAQEntity>("SELECT * FROM NewFAQEntity").Count;
            return count;
        }

        public static List<NewFAQEntity> GetAll()
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<NewFAQEntity>("SELECT * FROM NewFAQEntity").ToList<NewFAQEntity>();
        }
    }
}
