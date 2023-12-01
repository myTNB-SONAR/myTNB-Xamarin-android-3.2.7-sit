
using System.Collections.Generic;
using myTNB_Android.Src.MyHome.Model;
using static myTNB.Mobile.MobileEnums;
using Android.Graphics;
using Newtonsoft.Json;
using myTNB_Android.Src.MyTNBService.Response;
namespace myTNB_Android.Src.QuickActionArrange.Model
{
    public class ArrangeQuickActionModel
    {
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
        public string ServiceName { get; set; }
        public bool IsUserDeleted { get; set; }
        public ServiceEnum ServiceType { get; set; }
    }
}

