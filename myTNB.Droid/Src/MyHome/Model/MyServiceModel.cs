using System;
using System.Collections.Generic;
using static myTNB.Mobile.MobileEnums;

namespace myTNB_Android.Src.MyHome.Model
{
    public class MyServiceModel
    {
        public string ServiceId { get; set; }
        public string ParentServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceIconUrl { get; set; }
        public string DisabledServiceIconUrl { get; set; }
        public string ServiceBannerUrl { get; set; }
        public bool Enabled { get; set; }
        public string SSODomain { get; set; }
        public string OriginURL { get; set; }
        public string RedirectURL { get; set; }
        public int DisplayType { get; set; }
        public List<MyServiceModel> Children { get; set; }
        public ServiceEnum ServiceType { get; set; }
    }
}



