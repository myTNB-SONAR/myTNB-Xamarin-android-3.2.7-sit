﻿using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
    public class SSMRMeterHistoryMenuModel
    {
        [JsonProperty(PropertyName = "MenuId")]
        [AliasAs("MenuId")]
        public string MenuId { get; set; }

        [JsonProperty(PropertyName = "MenuName")]
        [AliasAs("MenuName")]
        public string MenuName { get; set; }

        [JsonProperty(PropertyName = "MenuIcon")]
        [AliasAs("MenuIcon")]
        public string MenuIcon { get; set; }

        [JsonProperty(PropertyName = "MenuCTA")]
        [AliasAs("MenuCTA")]
        public string MenuCTA { get; set; }

        [JsonProperty(PropertyName = "MenuDescription")]
        [AliasAs("MenuDescription")]
        public string MenuDescription { get; set; }

        [JsonProperty(PropertyName = "OrderId")]
        [AliasAs("OrderId")]
        public string OrderId { get; set; }
    }
}