using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Utils;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.Android.Src.Database.Model
{
    [Table("FeedbackCategoryEntity")]
    public class FeedbackCategoryEntity
    {
        [PrimaryKey, Column("FeedbackCategoryId")]
        public string Id { get; set; }

        [Column("FeedbackCategoryName")]
        public string Name { get; set; }

        [Column("FeedbackCategoryDesc")]
        public string Desc { get; set; }


        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<FeedbackCategoryEntity>();
            //}
        }

        public static int InsertOrReplace(FeedbackCategory feedback)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new FeedbackCategoryEntity()
            {
                Id = feedback.FeedbackCategoryId,
                Name = feedback.FeedbackCategoryName,
                Desc = feedback.FeedbackCategoryDesc
            };


            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
            //}
        }



        public static void RemoveActive()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM FeedbackCategoryEntity");
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }


        public static bool HasRecords()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackCategoryEntity>("SELECT * FROM FeedbackCategoryEntity").Count > 0;
            //}
        }


        public static List<FeedbackCategoryEntity> GetActiveList()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackCategoryEntity>("SELECT * FROM FeedbackCategoryEntity").ToList();
            //}
        }
    }
}
