using System;
using System.Collections.Generic;
using myTNB.Android.Src.Utils;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.Android.Src.Database.Model
{   
    /// <summary>
    /// syahmi sto add 
    /// </summary>
    [Table("EPPToolTipParentEntity")]
    public class EPPToolTipParentEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Timestamp")]
        public string Timestamp { set; get; }

        public void CreateTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("EPPToolTipParentEntity");
                db.CreateTable<EPPToolTipParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void InsertItem(EPPToolTipParentEntity item)
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

        public void InsertListOfItems(List<EppToolTipTimeStamp> itemList)
        {
            if (itemList != null)
            {
                foreach (EppToolTipTimeStamp obj in itemList)
                {
                    EPPToolTipParentEntity item = new EPPToolTipParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<EPPToolTipParentEntity> GetAllItems()
        {
            List<EPPToolTipParentEntity> itemList = new List<EPPToolTipParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<EPPToolTipParentEntity>("select * from EPPToolTipParentEntity");
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
                db.DeleteAll<EPPToolTipParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


        }

        public void UpdateItem(EPPToolTipParentEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Update(item);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }








    }
}