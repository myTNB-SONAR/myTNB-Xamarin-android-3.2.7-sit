using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Utils;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.Android.Src.Database.Model
{
    [Table("FeedbackStateEntity")]
    public class FeedbackStateEntity
    {
        [PrimaryKey, Column("StateId")]
        public string Id { get; set; }

        [Column("StateName")]
        public string Name { get; set; }

        [Column("IsSelected")]
        public bool IsSelected { get; set; }

        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<FeedbackStateEntity>();
            //}
        }

        public static int InsertOrReplace(FeedbackState feedback)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new FeedbackStateEntity()
            {
                Id = feedback.StateId,
                Name = feedback.StateName
            };


            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
            //}
        }

        public static int InsertOrReplace(FeedbackState feedback, bool isSelected)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new FeedbackStateEntity()
            {
                Id = feedback.StateId,
                Name = feedback.StateName,
                IsSelected = isSelected
            };


            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
            //}
        }

        public static void SetSelected(string Id)
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                db.Execute("UPDATE FeedbackStateEntity SET IsSelected = ? WHERE StateId = ?", true, Id);
            }
        }

        public static void ResetSelected()
        {
            RemoveActive();
            if (HasRecords())
            {
                FeedbackStateEntity state = GetFirstOrSelected();
                if (state != null)
                {

                    //using (var db = new SQLiteConnection(Constants.DB_PATH))
                    //{
                    var db = DBHelper.GetSQLiteConnection();
                    int newRecordRow = db.InsertOrReplace(state);
                    //}

                }
            }
        }

        public static void Remove()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM FeedbackStateEntity");
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static void RemoveActive()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("UPDATE FeedbackStateEntity SET IsSelected = ?", false);
            //}
        }


        public static bool HasRecords()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackStateEntity>("SELECT * FROM FeedbackStateEntity").Count > 0;
            //}
        }

        public static FeedbackStateEntity GetFirstOrSelected()
        {
            if (HasSelected())
            {
                return GetSelected();
            }
            else
            {
                return GetActiveList()[0];
            }
        }

        public static bool HasSelected()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackStateEntity>("SELECT * FROM FeedbackStateEntity WHERE IsSelected = ?", true).Count > 0;
            //}
        }

        public static FeedbackStateEntity GetSelected()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackStateEntity>("SELECT * FROM FeedbackStateEntity WHERE IsSelected = ?", true)[0];
            //}
        }

        public static List<FeedbackStateEntity> GetActiveList()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackStateEntity>("SELECT * FROM FeedbackStateEntity ORDER BY StateId").ToList();
            //}
        }
    }
}