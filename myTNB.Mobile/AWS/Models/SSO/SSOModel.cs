﻿using System;

namespace myTNB.Mobile
{
    public class SSOModel
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string DeviceToken { get; set; }
        public string AppVersion { get; set; }
        public int RoleId { get; set; }
        public string Lang { get; set; }
        public string FontSize { get; set; }
        public string OriginUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string CaNo { get; set; }
    }

    public class DSModel : SSOModel
    {
        public string UserID { get; set; }
        public int? IdType { get; set; }
        public string IdNo { get; set; }
        public string TransactionType { get; set; }
        public DateTime InitiateTime { get; set; }
        public string QRMappingID { get; set; }
        /// <summary>
        /// 0 = Portal
        /// 1 = Android
        /// 2 = iOS
        /// 3 = Huawei
        /// </summary>
        public int OSType { set; get; }
        public bool IsContractorApplied { set; get; }
        public string appRef { set; get; }
    }
}