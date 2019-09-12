﻿using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.NotificationDetails.Models
{
    public class NotificationDetailModel
    {
        public int imageResourceBanner;
        public string title, message;
        public List<NotificationCTA> ctaList;

        public NotificationDetailModel(int imageResourceBanner, string title, string message, List<NotificationCTA> ctaList)
        {
            this.imageResourceBanner = imageResourceBanner;
            this.title = title;
            this.message = message;
            this.ctaList = ctaList;
        }

        public class NotificationCTA
        {
            public string label;
            public Action action;

            public NotificationCTA(string btnLabel, Action ctaAction)
            {
                label = btnLabel;
                action = ctaAction;
            }

        }
    }
}
