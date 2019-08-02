using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.SQLite.SQLiteDataManager
{
	public class SSMRMeterReadingScreensEntity : SSMRMeterReadingModel
	{
		public void CreateTable()
		{
			var db = DBHelper.GetSQLiteConnection();
			List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("WalkthroughScreensEntity");
			db.CreateTable<SSMRMeterReadingScreensEntity>();
		}

		public void InsertItem(SSMRMeterReadingScreensEntity item)
		{
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				int newRecord = db.InsertOrReplace(item);
				Console.WriteLine("Insert Record: {0}", newRecord);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
			}
		}

		public void InsertListOfItems(List<SSMRMeterReadingModel> itemList)
		{
			if (itemList != null)
			{
				foreach (SSMRMeterReadingModel obj in itemList)
				{
                    SSMRMeterReadingScreensEntity item = new SSMRMeterReadingScreensEntity();
					item.ID = obj.ID;
					item.Image = obj.Image.Replace(" ", "%20");
					item.Title = obj.Title;
					item.Description = obj.Description;
					InsertItem(item);
				}
			}
		}

		public List<SSMRMeterReadingScreensEntity> GetAllItems()
		{
			List<SSMRMeterReadingScreensEntity> itemList = new List<SSMRMeterReadingScreensEntity>();
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				itemList = db.Query<SSMRMeterReadingScreensEntity>("select * from SSMRMeterReadingScreensEntity");
			}
			catch (Exception e)
			{
				Console.WriteLine("Error in Get All Items : {0}", e.Message);
			}
			return itemList;
		}

		public void DeleteTable()
		{
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				db.DeleteAll<SSMRMeterReadingScreensEntity>();
			}
			catch (Exception e)
			{
				Console.WriteLine("Error in Delete Table : {0}", e.Message);
			}
		}
	}
}