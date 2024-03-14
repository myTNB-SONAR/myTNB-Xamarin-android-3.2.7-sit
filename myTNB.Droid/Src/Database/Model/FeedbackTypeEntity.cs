using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Utils;
using SQLite;
using System.Collections.Generic;
using System.Linq;
namespace myTNB.Android.Src.Database.Model
{
    public class FeedbackTypeEntity
    {
        [PrimaryKey, Column("FeedbackTypeId")]
        public string Id { get; set; }

        [Column("FeedbackTypeName")]
        public string Name { get; set; }

        [Column("IsSelected")]
        public bool IsSelected { get; set; }

        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<FeedbackTypeEntity>();
            //}
        }

        public static int InsertOrReplace(FeedbackType type)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new FeedbackTypeEntity()
            {
                Id = type.FeedbackTypeId,
                Name = type.FeedbackTypeName
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
            //}
        }

        public static int InsertOrReplace(FeedbackType type, bool isSelected)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new FeedbackTypeEntity()
            {
                Id = type.FeedbackTypeId,
                Name = type.FeedbackTypeName,
                IsSelected = isSelected
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
            //}
        }


        public static void SetSelected(string Id)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("UPDATE FeedbackTypeEntity SET IsSelected = ? WHERE FeedbackTypeId = ?", true, Id);
            //}
        }

        public static void ResetSelected()
        {
            RemoveActive();
            if (HasRecords())
            {
                FeedbackTypeEntity state = GetFirstOrSelected();
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
                db.Execute("DELETE FROM FeedbackTypeEntity");
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
            db.Execute("UPDATE FeedbackTypeEntity SET IsSelected = ?", false);
            //}
        }


        public static bool HasRecords()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackTypeEntity>("SELECT * FROM FeedbackTypeEntity").Count > 0;
            //}
        }

        public static FeedbackTypeEntity GetFirstOrSelected()
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
            return db.Query<FeedbackTypeEntity>("SELECT * FROM FeedbackTypeEntity WHERE IsSelected = ?", true).Count > 0;
            //}
        }

        public static FeedbackTypeEntity GetSelected()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackTypeEntity>("SELECT * FROM FeedbackTypeEntity WHERE IsSelected = ?", true)[0];
            //}
        }

        public static List<FeedbackTypeEntity> GetActiveList()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FeedbackTypeEntity>("SELECT * FROM FeedbackTypeEntity").ToList();
            //}
        }
    }
}