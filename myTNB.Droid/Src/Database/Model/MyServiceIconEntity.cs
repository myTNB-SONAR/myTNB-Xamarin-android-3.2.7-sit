using System;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.Utils;
using SQLite;

namespace myTNB_Android.Src.Database.Model
{
    [Table("MyServiceIconEntity")]
    public class MyServiceIconEntity
	{
        [Unique, Column("ServiceId")]
        public string ServiceId { get; set; }

        [Column("ServiceIconUrl")]
        public string ServiceIconUrl { set; get; }

        [Column("ServiceBannerUrl")]
        public string ServiceBannerUrl { set; get; }

        [Column("DisabledServiceIconUrl")]
        public string DisabledServiceIconUrl { set; get; }

        [Column("ServiceIconB64")]
        public string ServiceIconB64 { set; get; }

        [Column("ServiceBannerB64")]
        public string ServiceBannerB64 { set; get; }

        [Column("DisabledServiceIconB64")]
        public string DisabledServiceIconB64 { set; get; }

        [Column("TimeStamp")]
        public string TimeStamp { set; get; }

        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("MyServiceIconEntity");
                db.CreateTable<MyServiceIconEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(MyServiceIconEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.InsertOrReplace(item);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertListOfItems(List<MyServiceIconModel> itemList)
        {
            if (itemList != null)
            {
                foreach (MyServiceIconModel obj in itemList)
                {
                    MyServiceIconEntity item = new MyServiceIconEntity();
                    item.ServiceId = obj.ServiceId;
                    item.ServiceIconUrl = obj.ServiceIconUrl;
                    item.ServiceIconB64 = obj.ServiceIconB64;
                    item.ServiceBannerUrl = obj.ServiceBannerUrl;
                    item.ServiceBannerB64 = obj.ServiceBannerB64;
                    item.DisabledServiceIconUrl = obj.DisabledServiceIconUrl;
                    item.DisabledServiceIconB64 = obj.DisabledServiceIconB64;
                    item.TimeStamp = obj.TimeStamp;
                    InsertItem(item);
                }
            }
        }

        public List<MyServiceIconEntity> GetAllItems()
        {
            List<MyServiceIconEntity> itemList = new List<MyServiceIconEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<MyServiceIconEntity>("select * from MyServiceIconEntity");
                if (itemList == null)
                {
                    itemList = new List<MyServiceIconEntity>();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return itemList;
        }

        public void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<MyServiceIconEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}

