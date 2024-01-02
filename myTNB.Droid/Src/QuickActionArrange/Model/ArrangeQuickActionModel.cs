
using System.Collections.Generic;
using myTNB_Android.Src.MyHome.Model;
using static myTNB.Mobile.MobileEnums;
using Android.Graphics;
using Newtonsoft.Json;
using myTNB_Android.Src.MyTNBService.Response;
using System;

namespace myTNB_Android.Src.QuickActionArrange.Model
{
    public class Feature
    {
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
        public string isLocked { get; set; }
        public string isAvailable { get; set; }
    }

    public class QuickActionReArrange
    {
        public string Email { get; set; }
        public DateTime timestamp { get; set; }
        public List<Feature> features { get; set; }
    }
}

