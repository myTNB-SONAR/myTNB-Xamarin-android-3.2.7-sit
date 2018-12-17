using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.SelectNotification.Models
{
    public class NotificationChannelUserPreference
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string PreferenceMode { get; set; }

        public string Type { get; set; }

        public string CreatedDate { get; set; }

        public string MasterId { get; set; }

        public bool IsOpted { get; set; }

        public bool ShowInPreference { get; set; }

        public bool ShowInFilterList { get; set; }

        public static NotificationChannelUserPreference Get(UserNotificationChannelEntity entity)
        {
            return new NotificationChannelUserPreference()
            {
                Id = entity.Id,
                Title = entity.Title,
                Code = entity.Code,
                PreferenceMode = entity.PreferenceMode,
                Type = entity.Type,
                CreatedDate = entity.CreatedDate,
                MasterId = entity.MasterId,
                IsOpted = entity.IsOpted,
                ShowInFilterList = entity.ShowInFilterList,
                ShowInPreference = entity.ShowInPreference
            };
        }
    }
}