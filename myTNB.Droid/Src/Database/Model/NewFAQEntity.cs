using System.Collections.Generic;
using System.Linq;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;
using SQLite;

namespace myTNB_Android.Src.Database.Model
{
    [Table("NewFAQEntity")]
    public class NewFAQEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Image")]
        public string Image { set; get; }

        [Column("BgStartColor")]
        public string BgStartColor { set; get; }

        [Column("BgEndColor")]
        public string BgEndColor { set; get; }

        [Column("BgDirection")]
        public string BgDirection { set; get; }

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

        [Column("Tag")]
        public string Tag { set; get; }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<NewFAQEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<NewFAQEntity>();
        }

        public static int InsertOrReplace(NewFAQ newFAQ)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new NewFAQEntity()
            {
                ID = newFAQ.ID,
                Image = newFAQ.Image,
                BgStartColor = newFAQ.BgStartColor,
                BgEndColor = newFAQ.BgEndColor,
                BgDirection = newFAQ.BgDirection,
                Title = newFAQ.Title,
                Description = newFAQ.Description,
                TopicBodyTitle = newFAQ.TopicBodyTitle,
                TopicBodyContent = newFAQ.TopicBodyContent,
                CTA = newFAQ.CTA,
                Tag = newFAQ.Tag,
            };

            int rows = db.InsertOrReplace(newRecord);

            return rows;
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
