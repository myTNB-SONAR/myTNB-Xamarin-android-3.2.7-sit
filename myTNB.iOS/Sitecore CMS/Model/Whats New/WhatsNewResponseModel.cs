﻿using System;
using System.Collections.Generic;
using Foundation;
using myTNB.SQLite.SQLiteDataManager;
using SQLite;

namespace myTNB.SitecoreCMS.Model
{
    public class WhatsNewResponseModel
    {
        public string Status { set; get; }
        public List<WhatsNewCategoryModel> Data { set; get; }
    }

    public class WhatsNewCategoryModel
    {
        public string ID { set; get; }
        public string CategoryName { set; get; }
        public List<WhatsNewModel> WhatsNewItems { set; get; }
    }

    public class WhatsNewModel
    {
        [PrimaryKey]
        public string ID { set; get; }
        public string CategoryID { set; get; }
        public string CategoryName { set; get; }
        public string Title { set; get; }
        public string TitleOnListing { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string StartDate { set; get; }
        public string EndDate { set; get; }
        public string PublishDate { set; get; }
        public string Image_DetailsView { set; get; }
        public string Styles_DetailsView { set; get; }
        public string PortraitImage_PopUp { set; get; }
        public int ShowEveryCountDays_PopUp { set; get; }
        public int ShowForTotalCountDays_PopUp { set; get; }
        public bool ShowAtAppLaunchPopUp { set; get; }
        public string ShowDateForDay { set; get; }
        public int ShowCountForDay { set; get; }
        public bool SkipShowOnAppLaunch { set; get; }
        public bool IsRead { set; get; }

        public WhatsNewEntity ToEntity()
        {
            var entity = new WhatsNewEntity
            {
                CategoryID = CategoryID,
                CategoryName = CategoryName,
                ID = ID,
                Title = Title,
                TitleOnListing = TitleOnListing,
                Description = Description,
                Image = Image,
                StartDate = StartDate,
                EndDate = EndDate,
                PublishDate = PublishDate,
                Image_DetailsView = Image_DetailsView,
                Styles_DetailsView = Styles_DetailsView,
                PortraitImage_PopUp = PortraitImage_PopUp,
                ShowEveryCountDays_PopUp = ShowEveryCountDays_PopUp,
                ShowForTotalCountDays_PopUp = ShowForTotalCountDays_PopUp,
                ShowAtAppLaunchPopUp = ShowAtAppLaunchPopUp,
                ShowDateForDay = ShowDateForDay,
                ShowCountForDay = ShowCountForDay,
                SkipShowOnAppLaunch = SkipShowOnAppLaunch,
                IsRead = IsRead
            };

            return entity;
        }
    }

    public class WhatsNewTimestampResponseModel
    {
        public string Status { set; get; }
        public List<WhatsNewTimestamp> Data { set; get; }
    }

    public class WhatsNewTimestamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
