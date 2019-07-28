﻿using System;
using System.Collections.Generic;
using myTNB.DataManager;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class SMRAccountActivityInfoResponseModel
    {
        public SMRAccountActivityInfoDataModel d { set; get; }
    }

    public class SMRAccountActivityInfoDataModel : BaseModelV2
    {
        public MeterReadingHistoryModel data { set; get; }
    }

    public class MeterReadingHistoryModel
    {
        string _isDashboardCTADisabled = string.Empty;
        string _showReadingHistoryLink = string.Empty;
        string _isThreePhaseMeter = string.Empty;

        public string DashboardMessage { set; get; }
        public string DashboardCTAText { set; get; }
        public string DashboardCTAType { set; get; }
        public string ReadingHistoryLinkText { set; get; }
        public string HistoryViewTitle { set; get; }
        public string HistoryViewMessage { set; get; }
        public string previousReadingKwh { set; get; }
        public string previousReadingKvarh { set; get; }
        public string previousReadingKw { set; get; }
        public string isDashboardCTADisabled
        {
            set
            {
                _isDashboardCTADisabled = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _isDashboardCTADisabled.ToLower();
            }
        }
        public string showReadingHistoryLink
        {
            set
            {
                _showReadingHistoryLink = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _showReadingHistoryLink.ToLower();
            }
        }
        public string isThreePhaseMeter
        {
            set
            {
                _isThreePhaseMeter = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _isThreePhaseMeter.ToLower();
            }
        }
        [JsonIgnore]
        public bool IsDashboardCTADisabled
        {
            get
            {
                var res = false;

                if (!string.IsNullOrEmpty(isDashboardCTADisabled))
                {
                    res = string.Compare(isDashboardCTADisabled, "true") == 0;
                }
                return res;
            }
        }
        [JsonIgnore]
        public bool ShowReadingHistoryLink
        {
            get
            {
                var res = false;

                if (!string.IsNullOrEmpty(showReadingHistoryLink))
                {
                    res = string.Compare(showReadingHistoryLink, "true") == 0;
                }
                return res;
            }
        }
        [JsonIgnore]
        public bool IsThreePhaseMeter
        {
            get
            {
                var res = false;

                if (!string.IsNullOrEmpty(isThreePhaseMeter))
                {
                    res = string.Compare(isThreePhaseMeter, "true") == 0;
                }
                return res;
            }
        }

        public List<MeterReadingHistoryItemModel> MeterReadingHistory { set; get; }
    }

    public class MeterReadingHistoryItemModel
    {
        public string ReadingDate { set; get; }
        public string ReadingType { set; get; }
        public string ReadingValue { set; get; }
        public string Consumption { set; get; }
        public string ReadingReason { set; get; }
        public string ReadingForMonth { set; get; }
    }
}
