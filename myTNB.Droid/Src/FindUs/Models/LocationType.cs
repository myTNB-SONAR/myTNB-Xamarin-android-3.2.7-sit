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

namespace myTNB_Android.Src.FindUs.Models
{
    public class LocationType
    {
        public string Id { get; set; }
        public string Title { get; set; }   
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsSelected { get; set; }
    }
}