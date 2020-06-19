﻿using System;
using System.Collections.Generic;
using myTNB_Android.Src.Utils;
using SQLite;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.Database.Model
{
    [Table("WhatsNewParentEntityV3")]
    public class WhatsNewParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("WhatsNewParentEntityV3");
                db.CreateTable<WhatsNewParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void InsertItem(WhatsNewParentEntity item)
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

        public void InsertListOfItems(List<WhatsNewTimeStamp> itemList)
        {
            if (itemList != null)
            {
                foreach (WhatsNewTimeStamp obj in itemList)
                {
                    WhatsNewParentEntity item = new WhatsNewParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<WhatsNewParentEntity> GetAllItems()
        {
            List<WhatsNewParentEntity> itemList = new List<WhatsNewParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<WhatsNewParentEntity>("select * from WhatsNewParentEntityV3");
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
                db.DeleteAll<WhatsNewParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(WhatsNewParentEntity item)
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
