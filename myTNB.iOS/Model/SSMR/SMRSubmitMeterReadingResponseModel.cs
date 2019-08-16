using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class SMRSubmitMeterReadingResponseModel
    {
        public SMRSubmitMeterReadingDataModel d { set; get; }
    }

    public class SMRSubmitMeterReadingDataModel : BaseModelV2
    {
        public MeterReadingResponseModel data { set; get; }
        public bool IsBusinessFail
        {
            get
            {
                return ErrorCode == "7100";
            }
        }
    }

    public class MeterReadingResponseModel
    {
        public string ContractAccount { set; get; }
        public List<MeterReadingItemModel> SubmitSMRMeterReadingsResp { set; get; }
    }

    public class MeterReadingItemModel
    {
        public string MessageID { set; get; }
        public string Message { set; get; }
        public string RegisterNumber { set; get; }
        public bool IsSuccess { set; get; }
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
    }
}
