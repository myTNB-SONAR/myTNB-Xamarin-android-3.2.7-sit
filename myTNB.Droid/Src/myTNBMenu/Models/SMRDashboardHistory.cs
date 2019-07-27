using System.Collections.Generic;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class SMRDashboardHistory
    {

        [JsonProperty("DashboardMessage")]
        [AliasAs("DashboardMessage")]
        public string DashboardMessage { get; set; }

        [JsonProperty("isDashboardCTADisabled")]
        [AliasAs("isDashboardCTADisabled")]
        public string isDashboardCTADisabled { get; set; }

        [JsonProperty("DashboardCTAText")]
        [AliasAs("DashboardCTAText")]
        public string DashboardCTAText { get; set; }

        [JsonProperty("DashboardCTAType")]
        [AliasAs("DashboardCTAType")]
        public string DashboardCTAType { get; set; }

        [JsonProperty("showReadingHistoryLink")]
        [AliasAs("showReadingHistoryLink")]
        public string showReadingHistoryLink { get; set; }

        [JsonProperty("ReadingHistoryLinkText")]
        [AliasAs("ReadingHistoryLinkText")]
        public string ReadingHistoryLinkText { get; set; }

        [JsonProperty("HistoryViewTitle")]
        [AliasAs("HistoryViewTitle")]
        public string HistoryViewTitle { get; set; }

        [JsonProperty("HistoryViewMessage")]
        [AliasAs("HistoryViewMessage")]
        public string HistoryViewMessage { get; set; }

        [JsonProperty("isThreePhaseMeter")]
        [AliasAs("isThreePhaseMeter")]
        public string isThreePhaseMeter { get; set; }

        [JsonProperty("previousReadingKwh")]
        [AliasAs("previousReadingKwh")]
        public string previousReadingKwh { get; set; }

        [JsonProperty("previousReadingKvarh")]
        [AliasAs("previousReadingKvarh")]
        public string previousReadingKvarh { get; set; }

        [JsonProperty("previousReadingKw")]
        [AliasAs("previousReadingKw")]
        public string previousReadingKw { get; set; }

        [JsonProperty("MeterReadingHistory")]
        [AliasAs("MeterReadingHistory")]
        public List<SSMRMeterHistoryModel> MeterReadingHistory { get; set; }
    }
}