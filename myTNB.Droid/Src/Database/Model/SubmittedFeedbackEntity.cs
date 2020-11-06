using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.Database.Model
{
    [Table("SubmittedFeedbackEntity")]
    public class SubmittedFeedbackEntity
    {
        [PrimaryKey, Column("FeedbackId")]
        public string Id { get; set; }

        [Column("DateCreated")]
        public string DateCreated { get; set; }

        [Column("FeedbackMessage")]
        public string FeedbackMessage { get; set; }

        [Column("FeedbackCategoryName")]
        public string FeedbackCategoryName { get; set; }

        [Column("FeedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        [Column("FeedbackNameInListView")]
        public string FeedbackNameInListView { get; set; }

        [Column("StatusCode")]
        public string StatusCode { get; set; }

        [Column("StatusDesc")]
        public string StatusDesc { get; set; }

        public static int CreateTable()
        {
            //var db = new SQLiteConnection(Constants.DB_PATH);
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<SubmittedFeedbackEntity>();
        }



        public static int InsertOrReplace(SubmittedFeedback submittedFeedback)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new SubmittedFeedbackEntity()
            {
                Id = submittedFeedback.FeedbackId,
                DateCreated = submittedFeedback.DateCreated,
                FeedbackMessage = submittedFeedback.FeedbackMessage,
                FeedbackCategoryName = submittedFeedback.FeedbackCategoryName,
                FeedbackCategoryId = submittedFeedback.FeedbackCategoryId,
                FeedbackNameInListView = submittedFeedback.FeedbackNameInListView,
                StatusCode = submittedFeedback.StatusCode,
                StatusDesc = submittedFeedback.StatusDesc
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
            //}
        }

        public static int InsertOrReplace(string Id, string DateCreated, string FeedbackMessage, string FeedbackCategoryName, string FeedbackCategoryId, string FeedbackNameInListView)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new SubmittedFeedbackEntity()
            {
                Id = Id,
                DateCreated = DateCreated,
                FeedbackMessage = FeedbackMessage,
                FeedbackCategoryName = FeedbackCategoryName,
                FeedbackCategoryId = FeedbackCategoryId,
                FeedbackNameInListView = FeedbackNameInListView
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
            //}
        }

        public static void Remove()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM SubmittedFeedbackEntity");
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
            return db.Query<SubmittedFeedbackEntity>("SELECT * FROM SubmittedFeedbackEntity").Count > 0;
            //}
        }

        public static int Count()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<SubmittedFeedbackEntity>("SELECT * FROM SubmittedFeedbackEntity").Count();
            //}
        }


        public static List<SubmittedFeedbackEntity> GetActiveList()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<SubmittedFeedbackEntity>("SELECT * FROM SubmittedFeedbackEntity").ToList();
            //}
        }
    }
}