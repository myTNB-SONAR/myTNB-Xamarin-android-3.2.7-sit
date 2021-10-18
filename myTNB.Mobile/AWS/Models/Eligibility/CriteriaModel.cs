﻿namespace myTNB.Mobile.AWS.Models
{
    public class CriteriaModel
    {
        public string CA { set; get; } = string.Empty;
        public bool IsOwner { set; get; }
        // CA Type
        public bool IsSmartMeter { set; get; }
        public bool IsNormalMeter { set; get; }
        public bool IsRenewableEnergy { set; get; }
        public bool IsSMR { set; get; }
    }
}
