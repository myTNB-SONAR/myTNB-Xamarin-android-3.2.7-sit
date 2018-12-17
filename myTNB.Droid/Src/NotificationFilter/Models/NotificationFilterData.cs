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
using Newtonsoft.Json;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.NotificationFilter.Models
{
    public class NotificationFilterData
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("IsSelected")]
        public bool IsSelected { get; set; }

        public static NotificationFilterData Get(NotificationFilterEntity entity)
        {
            return new NotificationFilterData()
            {
                Id = entity.Id,
                Title = entity.Title,
                IsSelected = entity.IsSelected
            };
        }
    }
}