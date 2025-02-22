﻿using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class OVISRequest
    {
        public List<string> contactAccountNumbers { get; set; }
    }
    public class TriggerOVISServicesRequestModel : BaseRequest
    {
        public string userID { get; set; }
        public string action { get; set; }
        public string actionUrl { get; set; }
        public OVISRequest OVISRequest { get; set; }
        public TriggerOVISServicesRequestModel(string userID, string action, string actionUrl, OVISRequest OVISRequest)
        {
            this.userID = userID;
            this.action = action;
            this.actionUrl = actionUrl;
            this.OVISRequest = OVISRequest;
        }
    }
}
