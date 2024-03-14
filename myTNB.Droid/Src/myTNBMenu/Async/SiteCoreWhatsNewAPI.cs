using Android.App;
using Android.OS;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.MVP;
using myTNB.Android.Src.SiteCore;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.myTNBMenu.Async
{
	public class SiteCoreWhatsNewAPI : AsyncTask
	{

		private string savedWhatsNewTimeStamp = "0000000";
		CancellationTokenSource cts = null;

        private DashboardHomeContract.IView mHomeView = null;

        private bool isSitecoreApiFailed = false;

        private WhatsNewTimeStampResponseModel responseMasterModel = new WhatsNewTimeStampResponseModel();

        public SiteCoreWhatsNewAPI(DashboardHomeContract.IView mView)
		{
            this.mHomeView = mView;
        }

		protected override void OnPreExecute()
		{

		}

		protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
		{
			try
			{
                isSitecoreApiFailed = false;
                Console.WriteLine("000 SiteCoreWhatsNewAPI started");
                WhatsNewParentEntity wtManager = new WhatsNewParentEntity();

                try
                {
                    List<WhatsNewParentEntity> saveditems = wtManager.GetAllItems();
                    if (saveditems != null && saveditems.Count > 0)
                    {
                        WhatsNewParentEntity entity = saveditems[0];
                        if (entity != null)
                        {
                            savedWhatsNewTimeStamp = entity.Timestamp;
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

				//Get Sitecore whatsnew timestamp
				bool getSiteCoreWhatsNew = false;
				cts = new CancellationTokenSource();
				Task.Factory.StartNew(() =>
				{
					try
					{
						string density = DPUtils.GetDeviceDensity(Application.Context);
                        GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                        responseMasterModel = getItemsService.GetWhatsNewTimestampItem();
                        if (responseMasterModel.Status.Equals("Success"))
                        {
                            if (responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                            {
                                if (!responseMasterModel.Data[0].Timestamp.Equals(savedWhatsNewTimeStamp))
                                {
                                    getSiteCoreWhatsNew = true;
                                }
                                else
                                {
                                    getSiteCoreWhatsNew = false;
                                }
                            }
                            else
                            {
                                List<WhatsNewParentEntity> items = wtManager.GetAllItems();
                                if (items != null && items.Count > 0)
                                {
                                    WhatsNewParentEntity entity = items[0];
                                    if (entity != null)
                                    {
                                        if (!entity.Timestamp.Equals(savedWhatsNewTimeStamp))
                                        {
                                            getSiteCoreWhatsNew = true;
                                        }
                                        else
                                        {
                                            WhatsNewEntity mWhatsNewEntityCheck = new WhatsNewEntity();
                                            List<WhatsNewEntity> mCheckList = mWhatsNewEntityCheck.GetAllItems();
                                            if (mCheckList == null || mCheckList.Count == 0)
                                            {
                                                getSiteCoreWhatsNew = true;
                                            }
                                            else
                                            {
                                                getSiteCoreWhatsNew = false;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        getSiteCoreWhatsNew = true;
                                    }
                                }
                                else
                                {
                                    getSiteCoreWhatsNew = true;
                                }
                            }
                        }
                        else
                        {
                            getSiteCoreWhatsNew = true;
                        }

						if (getSiteCoreWhatsNew)
						{
							cts = new CancellationTokenSource();
							Task.Factory.StartNew(() =>
							{
								try
								{
                                    string newDensity = DPUtils.GetDeviceDensity(Application.Context);
                                    GetItemsService getWhatsNewItemsService = new GetItemsService(SiteCoreConfig.OS, newDensity, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                                    WhatsNewResponseModel responseModel = getWhatsNewItemsService.GetWhatsNewItems();
                                    if (responseModel != null && !string.IsNullOrEmpty(responseModel.Status))
                                    {
                                        if (responseModel.Status.Equals("Success"))
                                        {
                                            if (responseMasterModel != null && responseMasterModel.Status != null && responseMasterModel.Status.Equals("Success") && responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                                            {
                                                WhatsNewParentEntity mWhatsNewParentEntity = new WhatsNewParentEntity();
                                                mWhatsNewParentEntity.DeleteTable();
                                                mWhatsNewParentEntity.CreateTable();
                                                mWhatsNewParentEntity.InsertListOfItems(responseMasterModel.Data);
                                            }

                                            WhatsNewCategoryEntity mWhatsNewCategoryEntity = new WhatsNewCategoryEntity();
                                            WhatsNewEntity mWhatsNewEntity = new WhatsNewEntity();

                                            if (responseModel != null && responseModel.Data != null && responseModel.Data.Count > 0)
                                            {
                                                List<WhatsNewCategoryModel> ToStoredList = new List<WhatsNewCategoryModel>();
                                                List<WhatsNewModel> ToStoredWhatsNewList = new List<WhatsNewModel>();

                                                for (int i = 0; i < responseModel.Data.Count; i++)
                                                {
                                                    if (responseModel.Data[i].WhatsNewList != null && responseModel.Data[i].WhatsNewList.Count > 0)
                                                    {
                                                        List<WhatsNewModel> localList = new List<WhatsNewModel>();
                                                        for (int j = 0; j < responseModel.Data[i].WhatsNewList.Count; j++)
                                                        {
                                                            int startResult = -1;
                                                            int endResult = 1;
                                                            try
                                                            {
                                                                if (!string.IsNullOrEmpty(responseModel.Data[i].WhatsNewList[j].StartDate) && !string.IsNullOrEmpty(responseModel.Data[i].WhatsNewList[j].EndDate))
                                                                {
                                                                    DateTime startDateTime = DateTime.ParseExact(responseModel.Data[i].WhatsNewList[j].StartDate, "yyyyMMddTHHmmss",
                                                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                                                    DateTime stopDateTime = DateTime.ParseExact(responseModel.Data[i].WhatsNewList[j].EndDate, "yyyyMMddTHHmmss",
                                                                        CultureInfo.InvariantCulture, DateTimeStyles.None);
                                                                    DateTime nowDateTime = DateTime.Now;
                                                                    startResult = DateTime.Compare(nowDateTime, startDateTime);
                                                                    endResult = DateTime.Compare(nowDateTime, stopDateTime);
                                                                }
                                                            }
                                                            catch (Exception ne)
                                                            {
                                                                Utility.LoggingNonFatalError(ne);
                                                            }
                                                            if (startResult >= 0 && endResult <= 0)
                                                            {
                                                                WhatsNewModel mModel = responseModel.Data[i].WhatsNewList[j];

                                                                WhatsNewEntity searchItem = mWhatsNewEntity.GetItem(mModel.ID);
                                                                if (searchItem != null)
                                                                {
                                                                    mModel.Read = searchItem.Read;
                                                                    mModel.ReadDateTime = searchItem.ReadDateTime;
                                                                    mModel.ShowDateForDay = searchItem.ShowDateForDay;
                                                                    mModel.ShowCountForDay = searchItem.ShowCountForDay;
                                                                    mModel.SkipShowOnAppLaunch = searchItem.SkipShowOnAppLaunch;
                                                                }
                                                                else
                                                                {
                                                                    mModel.ShowDateForDay = GetCurrentDate();
                                                                    mModel.ShowCountForDay = 0;
                                                                    mModel.SkipShowOnAppLaunch = false;
                                                                }
                                                                localList.Add(mModel);
                                                            }
                                                        }

                                                        if (localList.Count > 0)
                                                        {
                                                            ToStoredList.Add(new WhatsNewCategoryModel()
                                                            {
                                                                ID = responseModel.Data[i].ID,
                                                                CategoryName = responseModel.Data[i].CategoryName
                                                            });

                                                            ToStoredWhatsNewList.AddRange(localList);
                                                        }
                                                    }
                                                }

                                                mWhatsNewCategoryEntity.DeleteTable();
                                                mWhatsNewEntity.DeleteTable();
                                                mWhatsNewCategoryEntity.CreateTable();
                                                mWhatsNewEntity.CreateTable();

                                                if (ToStoredList.Count > 0)
                                                {
                                                    mWhatsNewCategoryEntity.InsertListOfItems(ToStoredList);
                                                    mWhatsNewEntity.InsertListOfItems(ToStoredWhatsNewList);
                                                }

                                                if (mHomeView != null)
                                                {
                                                    mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                                                }
                                            }
                                            else
                                            {
                                                mWhatsNewCategoryEntity.DeleteTable();
                                                mWhatsNewEntity.DeleteTable();
                                                mWhatsNewCategoryEntity.CreateTable();
                                                mWhatsNewEntity.CreateTable();

                                                if (mHomeView != null)
                                                {
                                                    mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            WhatsNewParentEntity wtManager2 = new WhatsNewParentEntity();
                                            wtManager2.DeleteTable();
                                            wtManager2.CreateTable();
                                            WhatsNewCategoryEntity mWhatsNewCategoryEntity2 = new WhatsNewCategoryEntity();
                                            WhatsNewEntity mWhatsNewEntity2 = new WhatsNewEntity();
                                            mWhatsNewCategoryEntity2.DeleteTable();
                                            mWhatsNewEntity2.DeleteTable();
                                            mWhatsNewCategoryEntity2.CreateTable();
                                            mWhatsNewEntity2.CreateTable();
                                            if (mHomeView != null)
                                            {
                                                mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        WhatsNewParentEntity wtManager2 = new WhatsNewParentEntity();
                                        wtManager2.DeleteTable();
                                        wtManager2.CreateTable();
                                        isSitecoreApiFailed = true;
                                        if (mHomeView != null)
                                        {
                                            mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                                        }
                                    }
								}
								catch (System.Exception e)
								{
                                    WhatsNewParentEntity wtManager2 = new WhatsNewParentEntity();
                                    wtManager2.DeleteTable();
                                    wtManager2.CreateTable();
                                    isSitecoreApiFailed = true;
                                    if (mHomeView != null)
                                    {
                                        mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                                    }
                                    Utility.LoggingNonFatalError(e);
								}
							}).ContinueWith((Task previous) =>
							{
							}, cts.Token);
						}
                        else
                        {
                            if (mHomeView != null)
                            {
                                mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                            }
                        }
					}
					catch (System.Exception e)
					{
                        WhatsNewParentEntity wtManager2 = new WhatsNewParentEntity();
                        wtManager2.DeleteTable();
                        wtManager2.CreateTable();
                        isSitecoreApiFailed = true;
                        if (mHomeView != null)
                        {
                            mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                        }
                        Utility.LoggingNonFatalError(e);
					}
				}).ContinueWith((Task previous) =>
				{
				}, cts.Token);

			}
			catch (ApiException apiException)
			{
                WhatsNewParentEntity wtManager2 = new WhatsNewParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                if (mHomeView != null)
                {
                    mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                }
                Utility.LoggingNonFatalError(apiException);
			}
			catch (Newtonsoft.Json.JsonReaderException e)
			{
                WhatsNewParentEntity wtManager2 = new WhatsNewParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                if (mHomeView != null)
                {
                    mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                }
                Utility.LoggingNonFatalError(e);
			}
			catch (System.Exception e)
			{
                WhatsNewParentEntity wtManager2 = new WhatsNewParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                if (mHomeView != null)
                {
                    mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                }
                Utility.LoggingNonFatalError(e);
			}
			Console.WriteLine("000 SiteCoreWhatsNewAPI ended");
			return null;
		}

        private string GetCurrentDate()
        {
            DateTime currentDate = DateTime.Now;
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            return currentDate.ToString(@"yyyyMMddTHHmmss", currCult);
        }

        protected override void OnPostExecute(Java.Lang.Object result)
		{
			base.OnPostExecute(result);
        }

	}
}
