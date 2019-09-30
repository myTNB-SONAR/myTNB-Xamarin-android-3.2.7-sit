﻿using System;
using System.Collections.Generic;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB_Android.Src.SitecoreCMS.Model;

namespace myTNB_Android.Src.Database.Model
{
	[Table("NewFAQParentEntity")]
	public class NewFAQParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("NewFAQParentEntity");
                db.CreateTable<NewFAQParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(NewFAQParentEntity item)
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

        public void InsertListOfItems(List<HelpTimeStamp> itemList)
        {
            if (itemList != null)
            {
                foreach (HelpTimeStamp obj in itemList)
                {
                    NewFAQParentEntity item = new NewFAQParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<NewFAQParentEntity> GetAllItems()
        {
            List<NewFAQParentEntity> itemList = new List<NewFAQParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<NewFAQParentEntity>("select * from NewFAQParentEntity");
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
                db.DeleteAll<NewFAQParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(NewFAQParentEntity item)
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
