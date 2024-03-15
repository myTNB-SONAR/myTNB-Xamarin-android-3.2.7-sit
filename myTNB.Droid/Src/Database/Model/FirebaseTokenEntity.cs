using myTNB.AndroidApp.Src.Utils;
using SQLite;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("FirebaseTokenEntity")]
    internal class FirebaseTokenEntity
    {

        [Unique, Column("FBToken")]
        public string FBToken { get; set; }

        [Column("IsLatest")]
        public bool IsLatest { get; set; }

        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<FirebaseTokenEntity>();
            //}
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<FirebaseTokenEntity>();
        }

        public static int InsertOrReplace(string token, bool isLatest)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new FirebaseTokenEntity()
            {
                FBToken = token,
                IsLatest = isLatest
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
            //}

        }

        public static void RemoveLatest()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("UPDATE FirebaseTokenEntity SET IsLatest = ? where IsLatest = ?", false, true);
            //}
        }

        public static void RemoveAll()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM FirebaseTokenEntity");
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static bool HasLatest()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FirebaseTokenEntity>("SELECT * FROM FirebaseTokenEntity WHERE IsLatest = ?", true).Count > 0;
            //}
        }

        public static FirebaseTokenEntity GetLatest()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<FirebaseTokenEntity>("SELECT * FROM FirebaseTokenEntity WHERE IsLatest = ?", true)[0];
            //}
        }
    }
}