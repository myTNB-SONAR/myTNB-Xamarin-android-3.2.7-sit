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
    [Table("DownTimeEntity")]
    public class DownTimeEntity
    {
        [PrimaryKey, Column("System")]
        public string System { get; set; }

        [Column("IsDown")]
        public bool IsDown { get; set; }

        [Column("DowntimeMessage")]
        public string DowntimeMessage { get; set; }

        [Column("DowntimeTextMessage")]
        public string DowntimeTextMessage { get; set; }

        [Column("DowntimeStart")]
        public string DowntimeStart { get; set; }

        [Column("DowntimeEnd")]
        public string DowntimeEnd { get; set; }


        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<DownTimeEntity>();
            //}
        }

        public static int InsertOrReplace(DownTime downTime)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
                var newRecord = new DownTimeEntity()
                {
                    System = downTime.System,
                    IsDown = downTime.IsDown,
                    DowntimeMessage = downTime.DowntimeMessage,
                    DowntimeTextMessage = downTime.DowntimeTextMessage,
                    DowntimeStart = downTime.DowntimeStart,
                    DowntimeEnd = downTime.DowntimeEnd
                };
                return db.InsertOrReplace(newRecord);
            //}

        }

        public static IEnumerable<DownTimeEntity> Enumerate()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
                return db.Query<DownTimeEntity>("select * from DownTimeEntity");
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
                return db.Query<DownTimeEntity>("select * from DownTimeEntity WHERE System = ?", code).Count > 0;
            //}
        }

        public static bool IsSystemDown(string systemCode)
        {
            bool isDOWN = false;
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
                List<DownTimeEntity> itemList = new List<DownTimeEntity>();
                itemList = db.Query<DownTimeEntity>("select * from DownTimeEntity WHERE System = ?", systemCode);
                if(itemList.Count > 0)
                {
                    DownTimeEntity entity = itemList[0];
                    isDOWN = entity.IsDown;
                }
                return isDOWN;
            //}
        }

        public static DownTimeEntity GetByCode(string code)
        {
            if (HasRecord(code))
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                List<DownTimeEntity> list = new List<DownTimeEntity>();
                list = db.Query<DownTimeEntity>("select * from DownTimeEntity WHERE System = ?", code);
                if (list != null && list.Count() > 0) {
                    return list[0];
                } else {
                    return null;
                }
                //}
            }
            else
            {
                return null;
            }
        }


        public static bool IsBCRMDown() {
            return IsSystemDown(Constants.BCRM_SYSTEM);
        }

        public static void RemoveActive()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM DownTimeEntity");
            //}
        }


    }
}