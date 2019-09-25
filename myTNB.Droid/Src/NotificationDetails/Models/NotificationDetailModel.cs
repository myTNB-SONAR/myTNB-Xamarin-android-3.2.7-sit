using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.NotificationDetails.Models
{
    public class NotificationDetailModel
    {
        public int imageResourceBanner;
        public string detailPageTitle, title, message;
        public List<NotificationCTA> ctaList;

        public NotificationDetailModel(int imageResourceBanner, string pageTitle, string title, string message, List<NotificationCTA> ctaList)
        {
            this.imageResourceBanner = imageResourceBanner;
            this.detailPageTitle = pageTitle;
            this.title = title;
            this.message = message;
            this.ctaList = ctaList;
        }

        public class NotificationCTA
        {
            public string label;
            public Action action;
            public bool isSolidBackground;

            public NotificationCTA(string btnLabel, Action ctaAction)
            {
                label = btnLabel;
                action = ctaAction;
            }

            public void SetSolidCTA(bool isSolid)
            {
                isSolidBackground = isSolid;
            }

        }
    }
}
