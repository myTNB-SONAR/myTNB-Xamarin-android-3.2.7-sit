using System;
using System.Collections.Generic;
using myTNB.Android.Src.Utils;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.Android.Src.Database.Model
{
    [Table("OnboardingSSMRParentEntity")]
    public class OnboardingSSMRParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("OnboardingSSMRParentEntity");
                db.CreateTable<OnboardingSSMRParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void InsertItem(OnboardingSSMRParentEntity item)
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

        public void InsertListOfItems(List<ApplySSMRTimeStamp> itemList)
        {
            if (itemList != null)
            {
                foreach (ApplySSMRTimeStamp obj in itemList)
                {
                    OnboardingSSMRParentEntity item = new OnboardingSSMRParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<OnboardingSSMRParentEntity> GetAllItems()
        {
            List<OnboardingSSMRParentEntity> itemList = new List<OnboardingSSMRParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<OnboardingSSMRParentEntity>("select * from OnboardingSSMRParentEntity");
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
                db.DeleteAll<OnboardingSSMRParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(OnboardingSSMRParentEntity item)
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
