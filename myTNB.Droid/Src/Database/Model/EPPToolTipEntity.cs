﻿using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB_Android.Src.Database.Model
{
    //syahmi add
    [Table("EPPToolTipEntity")]
    public class EPPToolTipEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Title")]
        public string Title { set; get; }
        
        [Column("PopUpTitle")]
        public string PopUpTitle { set; get; }

        [Column("PopUpBody")]
        public string PopUpBody { set; get; }

        [Column("Image")]
        public string Image { set; get; }




        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("EPPToolTipEntity");
                db.CreateTable<EPPToolTipEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public void InsertItem(EPPToolTipEntity item)
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

        public void InsertListOfItems(List<EppToolTipModel> itemList)
        {
            if (itemList != null)
            {
                foreach (EppToolTipModel obj in itemList)
                {
                    EPPToolTipEntity item = new EPPToolTipEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.Title = obj.Title;
                    item.PopUpTitle = obj.PopUpTitle;
                    item.PopUpBody = obj.PopUpBody;

                    InsertItem(item);
                }
            }
        }

        public List<EPPToolTipEntity> GetAllItems()
        {
            List<EPPToolTipEntity> itemList = new List<EPPToolTipEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<EPPToolTipEntity>("select * from EPPToolTipEntity");
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
                db.DeleteAll<EPPToolTipEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }


  

}