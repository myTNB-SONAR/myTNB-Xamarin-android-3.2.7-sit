﻿using System.Collections.Generic;
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
        public bool IsOCRDisabled { set; get; }
        public bool IsOCRDown { set; get; }
    }

    public class MeterReadingHistoryModel
    {
        string _isDashboardCTADisabled = string.Empty;
        string _showReadingHistoryLink = string.Empty;
        string _isThreePhaseMeter = string.Empty;
        string _isCurrentPeriodSubmitted = string.Empty;

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
        public string isCurrentPeriodSubmitted
        {
            set
            {
                _isCurrentPeriodSubmitted = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _isCurrentPeriodSubmitted.ToLower();
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
        [JsonIgnore]
        public bool IsCurrentPeriodSubmitted
        {
            get
            {
                var res = false;

                if (!string.IsNullOrEmpty(isCurrentPeriodSubmitted))
                {
                    res = string.Compare(isCurrentPeriodSubmitted, "true") == 0;
                }
                return res;
            }
        }

        public List<MeterReadingHistoryItemModel> MeterReadingHistory { set; get; }
        public List<MoreOptionsItemModel> MoreOptions { set; get; }
        public List<PopupModel> SMRPhotoPopUpDetails { set; get; }
        public List<SMRMROValidateRegisterDetailsInfoModel> SMRMROValidateRegisterDetails { set; get; }
    }

    public class MeterReadingHistoryItemModel
    {
        public string ReadingDate { set; get; }
        public string ReadingType { set; get; }
        public string ReadingValue { set; get; }
        public string Consumption { set; get; }
        public string ReadingReason { set; get; }
        public string ReadingForMonth { set; get; }
        public string ReadingTypeCode { set; get; }
        private List<string> EstimatedCodeList = new List<string> { "03", "53", "73", "83", "93" };
        [JsonIgnore]
        public bool IsEstimatedReading
        {
            get
            {
                return EstimatedCodeList.Contains(ReadingTypeCode);
            }
        }
    }

    public class MoreOptionsItemModel
    {
        string _isHighlighted = string.Empty;
        public string MenuId { set; get; }
        public string MenuName { set; get; }
        public string MenuIcon { set; get; }
        public string MenuCTA { set; get; }
        public string MenuDescription { set; get; }
        public string OrderId { set; get; }
        public string IsHighlighted
        {
            set
            {
                _isHighlighted = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _isHighlighted.ToLower();
            }
        }
        [JsonIgnore]
        public bool isHighlighted
        {
            get
            {
                var res = false;

                if (!string.IsNullOrEmpty(IsHighlighted))
                {
                    res = string.Compare(IsHighlighted, "true") == 0;
                }
                return res;
            }
        }
    }

    public class PopupModel
    {
        public string Title { set; get; } = string.Empty;
        public string Description { set; get; } = string.Empty;
        public string CTA { set; get; } = string.Empty;
        public string Type { set; get; } = string.Empty;
    }

    public class SMRMROValidateRegisterDetailsInfoModel
    {
        private string _readingUnitDisplay = string.Empty;

        public string RegisterNumber { set; get; }
        public string MroID { set; get; }
        public string PrevMrDate { set; get; }
        public string SchMrDate { set; get; }
        public string PrevMeterReading { set; get; }
        public string ReadingUnit { set; get; }
        public string ReadingUnitDisplayTitle { set; get; }
        [JsonIgnore]
        public RegisterNumberEnum RegisterNumberType
        {
            get
            {
                RegisterNumberEnum registerNumberType = default(RegisterNumberEnum);

                if (!string.IsNullOrEmpty(RegisterNumber))
                {
                    switch (RegisterNumber)
                    {
                        case "001":
                            registerNumberType = RegisterNumberEnum.kWh;
                            break;
                        case "002":
                            registerNumberType = RegisterNumberEnum.kVARh;
                            break;
                        case "003":
                            registerNumberType = RegisterNumberEnum.kW;
                            break;
                    }
                }
                return registerNumberType;
            }
        }
        [JsonIgnore]
        public bool IsValidManualReading { set; get; }
        [JsonIgnore]
        public string CurrentReading { set; get; }
        [JsonIgnore]
        public bool IsErrorFromOCR { set; get; }
        [JsonIgnore]
        public string ErrorMessage { set; get; }
    }

    public enum RegisterNumberEnum
    {
        None = 0,
        kWh,
        kVARh,
        kW
    }
}