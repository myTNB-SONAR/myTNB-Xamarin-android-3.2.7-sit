using System.Collections.Generic;
using System.Linq;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using SQLite;

namespace myTNB_Android.Src.Database.Model
{
    [Table("MyServiceEntity")]
    public class MyServiceEntity
    {
        [Unique, Column("ServiceCategoryId")]
        public string ServiceCategoryId { get; set; }

        [Column("serviceCategoryName")]
        public string serviceCategoryName { get; set; }

        [Column("serviceCategoryIcon")]
        public string serviceCategoryIcon { get; set; }

        [Column("serviceCategoryIconUrl")]
        public string serviceCategoryIconUrl { get; set; }

        [Column("serviceCategoryDesc")]
        public string serviceCategoryDesc { get; set; }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<MyServiceEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<MyServiceEntity>();
        }

        public static int InsertOrReplace(MyService myServices)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new MyServiceEntity()
            {
                ServiceCategoryId = myServices.ServiceCategoryId,
                serviceCategoryName = myServices.serviceCategoryName,
                serviceCategoryIcon = myServices.serviceCategoryIcon,
                serviceCategoryIconUrl = myServices.serviceCategoryIconUrl,
                serviceCategoryDesc = myServices.serviceCategoryDesc,
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
