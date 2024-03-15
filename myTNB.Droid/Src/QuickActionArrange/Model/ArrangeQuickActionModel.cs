
using System.Collections.Generic;
using myTNB.AndroidApp.Src.MyHome.Model;
using static myTNB.Mobile.MobileEnums;
using Android.Graphics;
using Newtonsoft.Json;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using System;

namespace myTNB.AndroidApp.Src.QuickActionArrange.Model
{
    public class Feature
    {
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
        public bool isLocked { get; set; }
        public bool isAvailable { get; set; }
    }

    public class QuickActionReArrange
    {
        public string Email { get; set; }
        public DateTime timestamp { get; set; }
        public List<Feature> features { get; set; }
    }
}

