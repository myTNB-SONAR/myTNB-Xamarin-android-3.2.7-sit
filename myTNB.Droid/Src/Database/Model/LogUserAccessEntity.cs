using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.Database.Model
{
    [Table("LogUserAccessEntity")]
    public class LogUserAccessEntity
    {
        [Column("AccountNo")]
        public string AccountNo { get; set; }

        [Column("Action")]
        public string Action { get; set; }

        [PrimaryKey, Column("ActivityLogID")]
        public string ActivityLogID { get; set; }

        [Column("CreateBy")]
        public string CreateBy { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

        [Column("IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [Column("UserID")]
        public string UserID { get; set; }

        [Column("UserName")]
        public string UserName { get; set; }

        public static int CreateTable()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return (int)db.CreateTable<LogUserAccessEntity>();
            }
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<LogUserAccessEntity>();
        }

        public static int InsertOrReplace(LogUserAccessData logUserAcessData)
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                var newRecord = new LogUserAccessEntity()
                {
                    AccountNo = logUserAcessData.AccountNo,
                    Action = logUserAcessData.Action,
                    ActivityLogID = logUserAcessData.ActivityLogID,
                    CreateBy = logUserAcessData.CreateBy,
                    CreatedDate = logUserAcessData.CreatedDate,
                    IsApplyEBilling = logUserAcessData.IsApplyEBilling,
                    IsHaveAccess = logUserAcessData.IsHaveAccess,
                    UserID = logUserAcessData.UserID,
                    UserName = logUserAcessData.UserName
                };

                int newRecordRow = db.InsertOrReplace(newRecord);

                return newRecordRow;
            }
        }

        public static LogUserAccessEntity GetActive()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                List<LogUserAccessEntity> logUserAccessEntityList = new List<LogUserAccessEntity>();
                logUserAccessEntityList = db.Query<LogUserAccessEntity>("SELECT * FROM LogUserAccessEntity WHERE ActivityLogID != ?", 0).ToList();
                if (logUserAccessEntityList != null && logUserAccessEntityList.Count() > 0)
                {
                    return logUserAccessEntityList[0];
                }
                return null;
            }
        }

        public static void RemoveAll()
        {
            try
            {
                using (var db = new SQLiteConnection(Constants.DB_PATH))
                {
                    db.Execute("DELETE FROM LogUserAccessEntity");
                }
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static List<LogUserAccessEntity> List()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Query<LogUserAccessEntity>("SELECT * FROM LogUserAccessEntity").ToList();
            }
        }

        public static void UnSelectAll()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                db.Execute("UPDATE LogUserAccessEntity SET IsSelected = ?", false);
            }
        }

        public static int SelectItem(string id)
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Execute("UPDATE LogUserAccessEntity SET IsSelected = ? WHERE Id = ?", true, id);
            }
        }
    }
}