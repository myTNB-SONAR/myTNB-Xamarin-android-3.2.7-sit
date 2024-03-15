using System.Collections.Generic;
using System.Linq;
using Android.Nfc;
using myTNB.AndroidApp.Src.MyHome.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.AndroidApp.Src.Utils;
using SQLite;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("MyServiceEntity")]
    public class MyServiceEntity
    {
        [Unique, Column("ServiceId")]
        public string ServiceId { get; set; }

        [Column("ServiceName")]
        public string ServiceName { get; set; }

        [Column("ServiceIconUrl")]
        public string ServiceIconUrl { get; set; }

        [Column("DisabledServiceIconUrl")]
        public string DisabledServiceIconUrl { get; set; }

        [Column("ServiceBannerUrl")]
        public string ServiceBannerUrl { get; set; }

        [Column("Enabled")]
        public bool Enabled { get; set; }

        [Column("SSODomain")]
        public string SSODomain { get; set; }

        [Column("OriginURL")]
        public string OriginURL { get; set; }

        [Column("RedirectURL")]
        public string RedirectURL { get; set; }

        [Column("DisplayType")]
        public int DisplayType { get; set; }

        [Column("ServiceType")]
        public int ServiceType { get; set; }

        [Column("HasChildren")]
        public bool HasChildren { get; set; }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<MyServiceEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<MyServiceEntity>();
        }

        public static int InsertOrReplace(MyServiceModel myService)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new MyServiceEntity()
            {
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
                ServiceType = (int)myService.ServiceType,
                HasChildren = myService.Children != null && myService.Children.Count > 0 ? true : false
            };
            int rows = db.InsertOrReplace(newRecord);

            return rows;
        }

        public static void RemoveAll()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM MyServiceEntity");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static int Count()
        {
            var db = DBHelper.GetSQLiteConnection();
            int count = db.Query<MyServiceEntity>("SELECT * FROM MyServiceEntity").Count;
            return count;
        }

        public static List<MyServiceEntity> GetAll()
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<MyServiceEntity>("SELECT * FROM MyServiceEntity").ToList<MyServiceEntity>();
        }
    }
}
