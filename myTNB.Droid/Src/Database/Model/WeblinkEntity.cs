using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.AppLaunch.Models;

namespace myTNB_Android.Src.Database.Model
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
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<WeblinkEntity>();
        }

        public static int InsertOrReplace(Weblink web)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
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
            return db.InsertOrReplace(newRecord);
            

        }

        public static IEnumerable<WeblinkEntity> Enumerate()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Query<WeblinkEntity>("select * from WeblinkEntity");
            }
        }

        public static int Count()
        {
            return Enumerate().Count();
        }

        public static bool HasRecord(string code)
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Query<WeblinkEntity>("select * from WeblinkEntity WHERE CODE = ?" , code).Count > 0;
            }
        }

        public static WeblinkEntity GetByCode(string code)
        {
            if (HasRecord(code))
            {
                using (var db = new SQLiteConnection(Constants.DB_PATH))
                {
                    return db.Query<WeblinkEntity>("select * from WeblinkEntity WHERE CODE = ?", code)[0];
                }
            }
            else
            {
                return null;
            }
        }

        
    }
}