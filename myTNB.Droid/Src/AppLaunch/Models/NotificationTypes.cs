﻿using System;
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

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class NotificationTypes
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("PreferenceMode")]
        public string PreferenceMode { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("MasterId")]
        public string MasterId { get; set; }

        [JsonProperty("IsOpted")]
        public bool IsOpted { get; set; }

        [JsonProperty("ShowInPreference")]
        public bool ShowInPreference { get; set; }

        [JsonProperty("ShowInFilterList")]
        public bool ShowInFilterList { get; set; }
    }
}