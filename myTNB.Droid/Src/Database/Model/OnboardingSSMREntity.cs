using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database;
using myTNB_Android.Src.SitecoreCMS.Model;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.SQLite.SQLiteDataManager
{
	public class OnboardingSSMREntity : OnboardingSSMRModel
    {
		public void CreateTable()
		{
			//using (var db = new SQLiteConnection(Constants.DB_PATH))
			//{
			var db = DBHelper.GetSQLiteConnection();
			List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("OnboardingSSMREntity");
			db.CreateTable<OnboardingSSMREntity>();
			//}
		}

		public void InsertItem(OnboardingSSMREntity item)
		{
			try
			{
				//using (var db = new SQLiteConnection(Constants.DB_PATH))
				//{
				var db = DBHelper.GetSQLiteConnection();
				int newRecord = db.InsertOrReplace(item);
				Console.WriteLine("Insert Record: {0}", newRecord);
				//}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
			}
		}

		public void InsertListOfItems(List<ApplySSMRModel> itemList)
		{
			if (itemList != null)
			{
				foreach (ApplySSMRModel obj in itemList)
				{
                    OnboardingSSMREntity item = new OnboardingSSMREntity();
					item.ID = obj.ID;
					item.Image = obj.Image.Replace(" ", "%20");
					item.Description = obj.Description;
					item.Title = obj.Title;
					InsertItem(item);
				}
			}
		}

		public List<OnboardingSSMREntity> GetAllItems()
		{
			List<OnboardingSSMREntity> itemList = new List<OnboardingSSMREntity>();
			try
			{
				//using (var db = new SQLiteConnection(Constants.DB_PATH))
				//{
				var db = DBHelper.GetSQLiteConnection();
				itemList = db.Query<OnboardingSSMREntity>("select * from OnboardingSSMREntity");
				//}
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
				//using (var db = new SQLiteConnection(Constants.DB_PATH))
				//{
				var db = DBHelper.GetSQLiteConnection();
				db.DeleteAll<OnboardingSSMREntity>();
				//}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error in Delete Table : {0}", e.Message);
			}
		}
	}
}