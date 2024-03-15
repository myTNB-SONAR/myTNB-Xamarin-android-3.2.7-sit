using myTNB.AndroidApp.Src.AppLaunch.Models;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("WeblinkEntity")]
    public class WeblinkEntity
    {
        [PrimaryKey, Column("Id")]
        public string Id { get; set; }

        [Column("Code")]
        public string Code { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Url")]
        public string Url { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("DateCreated")]
        public string DateCreated { get; set; }

        [Column("OpenWith")]
        public string OpenWith { get; set; }

        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<WeblinkEntity>();
            //}
        }

        public static int InsertOrReplace(Weblink web)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new WeblinkEntity()
            {
                Id = web.Id,
                Code = web.Code,
                Title = web.Title,
                Url = web.Url,
                IsActive = web.IsActive,
                DateCreated = web.DateCreated,
                OpenWith = web.OpenWith
            };
            return (int)db.InsertOrReplace(newRecord);
            //}

        }

        public static IEnumerable<WeblinkEntity> Enumerate()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<WeblinkEntity>("select * from WeblinkEntity");
            //}
        }

        public static int Count()
        {
            return Enumerate().Count();
        }

        public static bool HasRecord(string code)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<WeblinkEntity>("select * from WeblinkEntity WHERE CODE = ?", code).Count > 0;
            //}
        }

        public static WeblinkEntity GetByCode(string code)
        {
            if (HasRecord(code))
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                return db.Query<WeblinkEntity>("select * from WeblinkEntity WHERE CODE = ?", code)[0];
                //}
            }
            else
            {
                return null;
            }
        }


    }
}