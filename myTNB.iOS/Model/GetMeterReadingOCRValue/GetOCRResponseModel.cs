using Newtonsoft.Json;

namespace myTNB.Model
{
    public class GetOCRReadingResponseModel
    {
        public GetOCRReadingDataModel d { set; get; }
    }

    public class GetOCRReadingDataModel : BaseModelV2
    {
        public GetOCRReadingModel data { set; get; }
    }

    public class GetOCRReadingModel
    {
        public string RequestReadingUnit { set; get; }
        public string ImageId { set; get; }
        public string OCRValue { set; get; }
        public string OCRUnit { set; get; }
        [JsonIgnore]
        public RegisterNumberEnum RegisterNumberTypeFromOCRUnit
        {
            get
            {
                RegisterNumberEnum registerNumberType = default(RegisterNumberEnum);

                if (!string.IsNullOrEmpty(OCRUnit))
                {
                    switch (OCRUnit.ToLower())
                    {
                        case "kwh":
                            registerNumberType = RegisterNumberEnum.kWh;
                            break;
                        case "kvarh":
                            registerNumberType = RegisterNumberEnum.kVARh;
                            break;
                        case "kw":
                            registerNumberType = RegisterNumberEnum.kW;
                            break;
                    }
                }
                return registerNumberType;
            }
        }
        [JsonIgnore]
        public RegisterNumberEnum RegisterNumberTypeFromRRUnit
        {
            get
            {
                RegisterNumberEnum registerNumberType = default(RegisterNumberEnum);

                if (!string.IsNullOrEmpty(RequestReadingUnit))
                {
                    switch (RequestReadingUnit.ToLower())
                    {
                        case "kwh":
                            registerNumberType = RegisterNumberEnum.kWh;
                            break;
                        case "kvarh":
                            registerNumberType = RegisterNumberEnum.kVARh;
                            break;
                        case "kw":
                            registerNumberType = RegisterNumberEnum.kW;
                            break;
                    }
                }
                return registerNumberType;
            }
        }
    }

    public class OCRReadingModel : GetOCRReadingModel
    {
        public bool IsSuccess { set; get; } = true;
        public string Message { set; get; }
    }
}