using System;
using System.Collections.Generic;
using System.Linq;
using myTNB.Android.Src.MyHome.Model;
using myTNB.Android.Src.Utils;
using SQLite;

namespace myTNB.Android.Src.Database.Model
{
    [Table("MyServiceChildEntity")]
    public class MyServiceChildEntity : MyServiceEntity
    {
        [Column("ParentServiceId")]
        public string ParentServiceId { get; set; }

        public static new int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<MyServiceChildEntity>();
        }

        public static new int InsertOrReplace(MyServiceModel myService)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new MyServiceChildEntity()
            {
                ParentServiceId = myService.ParentServiceId,
                ServiceId = myService.ServiceId,
                ServiceName = myService.ServiceName,
                ServiceIconUrl = myService.ServiceIconUrl,
                DisabledServiceIconUrl = myService.DisabledServiceIconUrl,
                ServiceBannerUrl = myService.ServiceBannerUrl,
                Enabled = myService.Enabled,
                SSODomain = myService.SSODomain,
                OriginURL = myService.OriginURL,
                RedirectURL = myService.RedirectURL,
                DisplayType = myService.DisplayType,
                ServiceType = (int)myService.ServiceType
            };
            int rows = db.InsertOrReplace(newRecord);

            return rows;
        }

        public static new void RemoveAll()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM MyServiceChildEntity");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static new int Count()
        {
            var db = DBHelper.GetSQLiteConnection();
            int count = db.Query<MyServiceChildEntity>("SELECT * FROM MyServiceChildEntity").Count;
            return count;
        }

        public static new List<MyServiceChildEntity> GetAll()
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<MyServiceChildEntity>("SELECT * FROM MyServiceChildEntity").ToList<MyServiceChildEntity>();
        }
    }
}

