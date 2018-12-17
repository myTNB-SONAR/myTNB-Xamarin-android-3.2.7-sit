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
using static myTNB_Android.Src.FindUs.Response.GetLocationTypesResponse;

namespace myTNB_Android.Src.Database.Model
{
    [Table("LocationTypesEntity")]
    public class LocationTypesEntity
    {
        [PrimaryKey, Column("Id")]
        public string Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("ImagePath")]
        public string ImagePath { get; set; }

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<LocationTypesEntity>();
        }

        public static int InsertOrReplace(LocationType loc)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new LocationTypesEntity()
            {
                Id = loc.Id,
                Title = loc.Title,
                Description = loc.Description,
                ImagePath = loc.ImagePath
            };
            return db.InsertOrReplace(newRecord);
        }

        public static int InsertFristRecord()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.DeleteAll<LocationTypesEntity>();
            var newRecord = new LocationTypesEntity()
            {
                Id = "0",
                Title = "All",
                Description = "All",
                ImagePath = Constants.SERVER_URL.END_POINT+"/public/images/pkp/default-kt.jpg"
            };
            return db.InsertOrReplace(newRecord);


        }

        public static IEnumerable<LocationTypesEntity> Enumerate()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Query<LocationTypesEntity>("select * from LocationTypesEntity");
            }
        }

        public static int Count()
        {
            return Enumerate().Count();
        }

        public static bool HasRecord()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Query<LocationTypesEntity>("select * from LocationTypesEntity").Count > 0;
            }
        }

        public static LocationTypesEntity GetById(string Id)
        {
            if (HasRecord())
            {
                using (var db = new SQLiteConnection(Constants.DB_PATH))
                {
                    return db.Query<LocationTypesEntity>("select * from LocationTypesEntity WHERE ID = ?", Id)[0];
                }
            }
            else
            {
                return null;
            }
        }

        public static List<myTNB_Android.Src.FindUs.Models.LocationType> GetLocationTypes()
        {
            List<LocationTypesEntity> entityTypes = new List<LocationTypesEntity>();
            if (HasRecord())
            {
                using (var db = new SQLiteConnection(Constants.DB_PATH))
                {
                    entityTypes.AddRange(db.Query<LocationTypesEntity>("select * from LocationTypesEntity"));
                }
            }

            List<myTNB_Android.Src.FindUs.Models.LocationType> types = new List<myTNB_Android.Src.FindUs.Models.LocationType>();
            foreach(LocationTypesEntity item in entityTypes)
            {
                types.Add(new myTNB_Android.Src.FindUs.Models.LocationType() {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    ImagePath = item.ImagePath
                    
                });
            }

            return types;
        }
    
    }
}