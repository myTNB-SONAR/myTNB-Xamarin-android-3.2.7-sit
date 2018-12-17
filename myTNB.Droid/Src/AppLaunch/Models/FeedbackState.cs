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
using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class FeedbackState
    {
        [JsonProperty("StateId")]
        public string StateId { get; set; }

        [JsonProperty("StateName")]
        public string StateName { get; set; }

        [JsonProperty("IsSelected" , Required = Newtonsoft.Json.Required.Default)]
        public bool IsSelected { get; set; }

        internal static FeedbackState Copy(FeedbackStateEntity entity)
        {
            return new FeedbackState()
            {
                StateId = entity.Id,
                StateName = entity.Name,
                IsSelected = entity.IsSelected
            };
        }
    }
}