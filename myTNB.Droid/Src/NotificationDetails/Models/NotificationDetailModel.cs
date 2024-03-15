using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.NotificationDetails.Models
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
            public bool isEnabled = true;
            public bool isRoundedButton = false;

            public NotificationCTA(string btnLabel, Action ctaAction)
            {
                label = btnLabel;
                action = ctaAction;
            }

            public void SetSolidCTA(bool isSolid)
            {
                isSolidBackground = isSolid;
            }

            public void SetEnabled(bool enable)
            {
                isEnabled = enable;
            }

            public void SetIsRoundedButton(bool isRounded)
            {
                isRoundedButton = isRounded;
            }
        }
    }
}
