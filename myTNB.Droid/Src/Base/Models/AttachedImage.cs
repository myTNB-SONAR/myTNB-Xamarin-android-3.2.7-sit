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

namespace myTNB_Android.Src.Base.Models
{
    public class AttachedImage
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ViewType")]
        public int ViewType { get; set; }

        [JsonProperty("IsLoading")]
        public bool IsLoading { get; set; }
    }
}