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

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class Weblink
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("OpenWith")]
        public string OpenWith { get; set; }

        public static Weblink Copy(WeblinkEntity weblinkEntity)
        {
            return new Weblink()
            {
                Id = weblinkEntity.Id,
                Code = weblinkEntity.Code,
                Title = weblinkEntity.Title,
                Url = weblinkEntity.Url,
                IsActive = weblinkEntity.IsActive,
                DateCreated = weblinkEntity.DateCreated,
                OpenWith = weblinkEntity.OpenWith
            };
        }
    }
}